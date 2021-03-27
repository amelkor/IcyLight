using System.IO;
using IcyLight.Core;

namespace IcyLight.Distribution.Services
{
    public interface IPatcher
    {
        bool TryApplyPatch(DistributionFile file, FileInfo originalFile, FileInfo patchile);
    }
}