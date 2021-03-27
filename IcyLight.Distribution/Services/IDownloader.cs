using IcyLight.Core;

namespace IcyLight.Distribution.Services
{
    public interface IDownloader
    {
        bool TryDownload(DistributionFile file, string destinationPath);
    }
}