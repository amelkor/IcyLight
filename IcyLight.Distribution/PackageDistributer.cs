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
    /// <summary>
    /// Intended to get a package and save it into the local directory to run update over it later.
    /// </summary>
    public class PackageDistributer : IProgressReporter
    {
        private const int DEFAULT_RETRY_COUNT = 3;

        private readonly ILogger<PackageDistributer> _logger;
        private readonly IDownloader _downloader;
        private readonly IDecompressor _decompressor;
        private readonly IVerifier _verifier;

        public event Action<long, long> ProgressChanged;

        public PackageDistributer(ILogger<PackageDistributer> logger, IDownloader downloader, IDecompressor decompressor, IVerifier verifier)
        {
            _logger = logger;
            _downloader = downloader;
            _decompressor = decompressor;
            _verifier = verifier;
        }

        public DistributionResultCode PreparePackage(DistributionPackage package, DirectoryInfo targetDirectory)
        {
            if (package.PackageSize > targetDirectory.GetAvailableFreeSpace())
            {
                _logger.LogWarning($"Not enough space for update package in target path: {targetDirectory.FullName}");
                return DistributionResultCode.NotEnoughSpace;
            }

            if (targetDirectory.Exists)
            {
                _logger.LogDebug("Cleaning target directory");
                targetDirectory.Delete(true);
            }

            targetDirectory.Create();

            var totalFilesCount = package.DistributionFiles.Count();
            long failedFilesCount = 0;
            long progress = 0;

            Parallel.ForEach(package.DistributionFiles, PrepareDistributionFiles);

            if (failedFilesCount > 0)
                return DistributionResultCode.HasFailedFiles;

            package.SaveToFile(targetDirectory.FullName);
            
            return DistributionResultCode.Finished;

            // local method
            void PrepareDistributionFiles(DistributionFile distFile)
            {
                var ok = false;
                for (var i = 0; i < DEFAULT_RETRY_COUNT; i++)
                {
                    ok = PrepareFile(distFile, targetDirectory);
                    if (ok)
                        break;
                }

                if (!ok)
                    Interlocked.Increment(ref failedFilesCount);

                Interlocked.Increment(ref progress);
                ProgressChanged?.Invoke(Interlocked.Read(ref progress), totalFilesCount);
            }
        }

        #region private

        private bool PrepareFile(DistributionFile distributionFile, FileSystemInfo targetDirectory)
        {
            if (distributionFile.Action == FileProcessingAction.Delete)
            {
                _logger.LogTrace($"Skipping file marked for delete: {distributionFile.Name}");
                return true;
            }

            var compressed = new FileInfo(Path.Combine(targetDirectory.FullName, distributionFile.InternalPath, distributionFile.CompressedName));
            var decompressed = new FileInfo(Path.Combine(targetDirectory.FullName, distributionFile.InternalPath, distributionFile.Name));

            try
            {
                if (compressed.Exists)
                    compressed.Delete();
                if (decompressed.Exists)
                    decompressed.Delete();

                if (!_downloader.TryDownload(distributionFile, compressed.FullName))
                {
                    _logger.LogError($"Can not download file: {compressed.Name}");
                    return false;
                }

                if (compressed.Length != distributionFile.CompressedSize)
                {
                    _logger.LogError($"Downloaded file {compressed.Name} has wrong size: {compressed.Length.ToString()}, has to be: {distributionFile.CompressedSize.ToString()}");
                    return false;
                }

                if (!_decompressor.TryDecompress(distributionFile, compressed, decompressed))
                {
                    _logger.LogError($"Can not decompress file {compressed.Name}");
                    decompressed.DeleteIfExists();
                    return false;
                }

                if (decompressed.Length != distributionFile.Size)
                {
                    _logger.LogError($"Decompressed file {decompressed.Name} has wrong size: {decompressed.Length.ToString()}, has to be: {distributionFile.Size.ToString()}");
                    decompressed.DeleteIfExists();
                    return false;
                }

                if (!_verifier.VerifyFile(distributionFile, decompressed))
                {
                    _logger.LogError($"Decompressed file {decompressed.Name} verification failed");
                    decompressed.DeleteIfExists();
                    return false;
                }

                return true;
            }
            finally
            {
                compressed.DeleteIfExists();
            }
        }

        #endregion
    }
}