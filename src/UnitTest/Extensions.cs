using System;
using System.IO;

namespace UnitTest
{
    public static class Extensions
    {
        public static FileInfo GetTempDBFile()
        {
            return new FileInfo(Path.GetTempPath() + Guid.NewGuid().ToString() + ".sqlite");
        }

        public static DirectoryInfo GetTempDirectory()
        {
            string path = Path.GetTempPath() + Guid.NewGuid().ToString();
            return Directory.CreateDirectory(path);
        }
    }
}
