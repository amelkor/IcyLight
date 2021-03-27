using System.IO;
using IcyLight.Core;

namespace IcyLight.Distribution.Services
{
    public interface IVerifier
    {
        bool VerifyFile(DistributionFile file, FileInfo fileInfo);
    }
}