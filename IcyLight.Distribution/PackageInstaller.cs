using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IcyLight.Core;
using IcyLight.Distribution.Services;
using Microsoft.Extensions.Logging;

namespace IcyLight.Distribution
{
    public class PackageInstaller : IProgressReporter
    {
        private readonly ILogger<PackageInstaller> _logger;
        private readonly IPatcher _patcher;

        public event Action<long, long> ProgressChanged;
        
        public PackageInstaller(ILogger<PackageInstaller> logger, IPatcher patcher)
        {
            _logger = logger;
            _patcher = patcher;
        }

        public DistributionResultCode InstallPackage(DirectoryInfo packageDirectory, DirectoryInfo targetDirectory)
        {
            var package = DistributionPackage.FindInDirectory(packageDirectory);
            if (package == null)
            {
                _logger.LogError($"Package directory {packageDirectory.Name} contains no package info file");
                return DistributionResultCode.PackageInfoNotFound;
            }

            var totalFilesCount = package.DistributionFiles.Count();
            long failedFilesCount = 0;
            long progress = 0;
            
            Parallel.ForEach(package.DistributionFiles, ProcessDistributionFile);

            if (failedFilesCount > 0)
                return DistributionResultCode.HasFailedFiles;
            
            return DistributionResultCode.Finished;
            
            // local method
            void ProcessDistributionFile(DistributionFile distFile)
            {
                Interlocked.Increment(ref progress);
                ProgressChanged?.Invoke(Interlocked.Read(ref progress), totalFilesCount);
                
                var target = new FileInfo(Path.Combine(targetDirectory.FullName, distFile.InternalPath, distFile.Name));

                if (distFile.Action == FileProcessingAction.Delete)
                {
                    _logger.LogTrace($"Deleting file: {distFile.InternalPath}\\{target.Name}");
                    target.DeleteIfExists();
                    return;
                }
                
                var temp = new FileInfo(Path.Combine(packageDirectory.FullName, distFile.InternalPath, distFile.Name));

                switch (distFile.Action)
                {
                    case FileProcessingAction.Install:
                        _logger.LogTrace($"Installing file: {distFile.InternalPath}\\{target.Name}");
                        target.DeleteIfExists();
                        File.Move(temp.FullName, target.FullName);
                        return;
                    case FileProcessingAction.Patch:
                    {
                        _logger.LogTrace($"Patching file: {distFile.InternalPath}\\{target.Name}");
                    
                        if (!_patcher.TryApplyPatch(distFile, target, temp))
                        {
                            Interlocked.Increment(ref failedFilesCount);
                            _logger.LogError($"Can not patch file: {distFile.InternalPath}\\{target.Name}");
                        }
                        return;
                    }
                }
            }
        }
    }
}