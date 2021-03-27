using System;

namespace IcyLight.Core
{
    [Serializable]
    public class DistributionFile
    {
        public string Name { get; set; }
        public string InternalPath { get; set; }

        public string Version { get; set; }
        public string Signature { get; set; }

        public long Size { get; set; }
        public uint Hash { get; set; }

        public long CompressedSize { get; set; }
        public uint CompressedHash { get; set; }
        public string CompressedName { get; set; }

        public FileProcessingAction Action { get; set; }
    }
}