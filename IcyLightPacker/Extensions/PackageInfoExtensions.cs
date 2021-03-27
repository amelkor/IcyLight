using System;
using System.IO;

namespace IcyLightPacker.Extensions
{
    public static class PackageInfoExtensions
    {
        private static bool EnsurePathCreated(string path)
        {
            try
            {
                var dir = Directory.CreateDirectory(path);
                return dir.Exists;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}