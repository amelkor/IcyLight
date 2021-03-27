using System.IO;
using IcyLight.Core;
using IcyLight.Distribution.Services;
using Ionic.Zip;
using Microsoft.Extensions.Logging;

namespace IcyLight.Distribution.Impl.Services
{
    public class Decompressor : IDecompressor
    {
        private readonly ILogger<Decompressor> _logger;

        public Decompressor(ILogger<Decompressor> logger)
        {
            _logger = logger;
        }

        public bool TryDecompress(DistributionFile distributionFile, FileInfo compressedFile, FileInfo targetFile)
        {
            targetFile.DeleteIfExists();

            using var zip = ZipFile.Read(compressedFile.FullName);

            if (zip.Count != 1)
            {
                _logger.LogError("zip is empty or contains more than one entry. Incorrect zip");
                return false;
            }

            zip[0].Extract(targetFile.Directory.FullName, ExtractExistingFileAction.OverwriteSilently);

            if (targetFile.Length != distributionFile.Size)
            {
                _logger.LogError($"Decompressed file {targetFile.Name} has wrong size");
                targetFile.DeleteIfExists();
                return false;
            }
            
            compressedFile.DeleteIfExists();
            return true;
        }
    }
}