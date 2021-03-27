using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace IcyLight.Core
{
    [Serializable]
    public class DistributionPackage
    {
        public string ApplicationId { get; set; } = "3D930DC8-197F-4B4A-BC9E-11F02ABD71BB";
        public string ApplicationTitle { get; set; }
        public string ApplicationDescription { get; set; }
        public string ApplicationVersion { get; set; } = "1.0.0-preview1";

        public long PackageSize { get; set; }

        public PackageType PackageType { get; set; }
        public List<DistributionFile> DistributionFiles { get; set; } = new();


        private const string EXT = "dist";

        public void SaveToFile(string path)
        {
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(Path.Combine(path, $"{ApplicationId}.{ApplicationVersion}.{EXT}"), json);
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