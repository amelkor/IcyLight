using System.IO;

namespace IcyLightPacker.Extensions
{
    internal static class FileInfoExtensions
    {
        public static string GetSubPathDirectory(this FileInfo fileInfo, string rootPointPath)
        {
            if (fileInfo.Directory == null)
                return string.Empty;

            var split = fileInfo.Directory.FullName.Split(rootPointPath);
            
            return split.Length < 2 ? string.Empty : $".{split[1]}";
        }
    }
}