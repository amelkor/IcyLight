using System.IO;
using NUnit.Framework;

namespace IcyLight.Core.Tests
{
    // ReSharper disable once InconsistentNaming
    public class IOExtensionsTests
    {
        [Test]
        public void GetAvailableSpaceByRandomPath_ShouldReturnPositiveValue()
        {
            var path = Path.GetRandomFileName();
            var space = new DirectoryInfo(path).GetAvailableFreeSpace();

            Assert.Greater(space, -1);
        }
    }
}