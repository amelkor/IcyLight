using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace IcyLight.Core
{
    [Serializable]
    public class DistributionPackage
    {
        public string ApplicationId { get; set; }
        public string ApplicationTitle { get; set; }
        public string ApplicationDescription { get; set; }
        public string ApplicationVersion { get; set; }

        public long PackageSize { get; set; }

        public PackageType PackageType { get; set; }
        public bool OnePiece { get; set; }
        public string OnePieceArchieveName { get; set; }
        public uint OnePieceArchieveHash { get; set; }
        public long OnePieceArchieveSize { get; set; }
        public List<DistributionFile> DistributionFiles { get; set; } = new();

        private const string EXT = "dist";

        public void SaveToFile(string path)
        {
            var json = JsonTool.Serialize(this);
            File.WriteAllText($"{path}\\{ApplicationId}-{ApplicationVersion}.{EXT}", json, Encoding.UTF8);
        }

        public static DistributionPackage FindInDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
                throw new ArgumentException("Specified directory not exists", nameof(directory));

            try
            {
                var distFiles = directory.EnumerateFiles($"*.{EXT}", SearchOption.TopDirectoryOnly);
                var dist = distFiles.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

                if (dist == null)
                    return null;

                var json = File.ReadAllText(dist.FullName);
                return JsonSerializer.Deserialize<DistributionPackage>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}