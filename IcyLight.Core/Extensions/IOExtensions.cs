using System.IO;

namespace IcyLight.Core
{
    // ReSharper disable once InconsistentNaming
    public static class IOExtensions
    {
        public static long GetAvailableFreeSpace(this DirectoryInfo directory)
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == directory.Root.FullName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
        
        public static void DeleteIfExists(this FileSystemInfo file)
        {
            if(file.Exists)
                file.Delete();
        }
    }
}