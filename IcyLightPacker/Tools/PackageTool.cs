using Force.Crc32;

namespace IcyLightPacker.Tools
{
    public static class PackageTool
    {
        public static uint ComputeHash(in byte[] data)
        {
            return Crc32CAlgorithm.Compute(data);
        }
    }
}