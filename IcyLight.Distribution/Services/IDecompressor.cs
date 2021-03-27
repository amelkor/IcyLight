using System.IO;
using IcyLight.Core;

namespace IcyLight.Distribution.Services
{
    public interface IDecompressor
    {
        bool TryDecompress(DistributionFile distributionFile, FileInfo compressedFile, FileInfo targetFile);
    }
}