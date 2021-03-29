using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IcyLight.Core;
using IcyLightPacker.Extensions;
using IcyLightPacker.Tools;
using IcyLightPacker.Verbs;
using Ionic.Zip;
using Serilog;

namespace IcyLightPacker
{
    public class Package
    {
        private static readonly ILogger _log = Logger.Log;

        private readonly PackOptions _options;

        private readonly DirectoryInfo _inputPath;
        private readonly DirectoryInfo _outputPath;
        private readonly SigningTool _signingTool;

        public Package(PackOptions options)
        {
            _options = options;
            _outputPath = new DirectoryInfo(_options.TargetPath);
            _inputPath = new DirectoryInfo(_options.SourcePath);
            _signingTool = new SigningTool(_options.Ed25519PrivateKeyPath);
        }

        public DistributionPackage Build()
        {
            var packageinfo = new DistributionPackage
            {
                ApplicationId = _options.ApplicationId,
                ApplicationVersion = _options.Version,
                OnePiece = _options.CompressSingleZip
            };
            var files = Directory.EnumerateFiles(_inputPath.FullName, _options.FilesPattern, SearchOption.AllDirectories).ToList();

            if (!files.Any())
            {
                _log.Error("Source directory contains no files");
                Environment.Exit(Codes.ERROR_SRC_DIR_NO_FILES);
            }

            Directory.CreateDirectory(_options.TargetPath);

            Parallel.ForEach(files, filePath =>
            {
                var distFile = ProcessFile(new FileInfo(filePath));
                Debug.Assert(distFile != null);
                lock (packageinfo)
                {
                    packageinfo.DistributionFiles.Add(distFile);
                    packageinfo.PackageSize += _options.CompressSingleZip ? distFile.Size : distFile.CompressedSize;
                }
            });

            if (_options.CompressSingleZip)
            {
                var (size, hash, name) = CompressOnePieceFile(_inputPath);
                packageinfo.OnePieceArchieveName = name;
                packageinfo.OnePiece = true;
                packageinfo.OnePieceArchieveSize = size;
                packageinfo.OnePieceArchieveHash = hash;
            }

            return packageinfo;
        }

        #region Private

        private DistributionFile ProcessFile(FileInfo fileInfo)
        {
            _log.Information($"Processing file {fileInfo.Name}");

            var internalPath = fileInfo.GetSubPathDirectory(_inputPath.FullName);
            var compressed = _options.CompressSingleZip ? (0, 0, string.Empty) : CompressFile(fileInfo, internalPath);
            var bytes = File.ReadAllBytes(fileInfo.FullName);

            return new DistributionFile
            {
                Action = FileProcessingAction.Install,
                Name = fileInfo.Name,
                InternalPath = internalPath,
                Version = GetFileVersion(fileInfo),
                Size = fileInfo.Length,
                Hash = PackageTool.ComputeHash(in bytes),
                Signature = _signingTool.Sign(in bytes),
                CompressedSize = compressed.size,
                CompressedHash = compressed.hash,
                CompressedName = compressed.name
            };
        }

        private static string GetFileVersion(FileSystemInfo fileInfo)
        {
            if (fileInfo.Extension == "exe" || fileInfo.Extension == "dll")
            {
                return FileVersionInfo.GetVersionInfo(fileInfo.FullName).FileVersion;
            }

            return string.Empty;
        }

        private (long size, uint hash, string name) CompressFile(FileSystemInfo fileInfo, string internalPath)
        {
            const string zipExt = "zip";
            var name = $"{fileInfo.Name}.{zipExt}";

            using var zip = new ZipFile {CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression};
            var zipFile = new FileInfo(Path.Combine(_outputPath.FullName, internalPath, name));

            _log.Verbose($"Compressing {zipFile.Name}");

            zipFile.Directory?.Create();

            zip.AddFile(fileInfo.FullName, internalPath);
            zip.Save(zipFile.FullName);

            var size = zipFile.Length;
            var hash = PackageTool.ComputeHash(File.ReadAllBytes(zipFile.FullName));

            return (size, hash, name);
        }

        private (long size, uint hash, string name) CompressOnePieceFile(DirectoryInfo fileInfo)
        {
            const string zipExt = "zip";
            var name = $"{_options.ApplicationId}-{_options.Version}.{zipExt}";

            using var zip = new ZipFile {CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression};
            var zipFile = new FileInfo(Path.Combine(_outputPath.FullName, name));

            _log.Verbose($"Compressing one piece package archieve {zipFile.Name}");

            zipFile.Directory?.Create();
            
            foreach (var file in fileInfo.EnumerateFiles(_options.FilesPattern, SearchOption.AllDirectories))
            {
                zip.AddFile(file.FullName, file.GetSubPathDirectory(_inputPath.FullName));
            }

            zip.Save(zipFile.FullName);

            var size = zipFile.Length;
            var hash = PackageTool.ComputeHash(File.ReadAllBytes(zipFile.FullName));

            return (size, hash, name);
        }

        #endregion
    }
}