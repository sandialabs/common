using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace UnitTest
{
    public class DirectoryCleanerShould
    {
        [Fact]
        public void CleanProperly()
        {
            using (DirectoryDeleter dd = new DirectoryDeleter(Extensions.GetTempDirectory()))
            {
                // Create 100 temporary files, and change their creation date/time so there's
                // one for each of the last 100 days. Then set a cleaner that deletes
                // files older than 50 days.
                int count = 100;
                DateTime now = DateTime.Now;
                for(int i = 0; i < count; ++i)
                {
                    FileInfo fi = new FileInfo(dd.Di.FullName + "\\" + Guid.NewGuid().ToString() + ".txt");
                    using (StreamWriter sw = fi.CreateText())
                    {
                    }
                    fi.CreationTime = fi.LastWriteTime = now - TimeSpan.FromDays(i);
                }

                List<FileInfo> files = new List<FileInfo>(dd.Di.GetFiles());
                Assert.Equal(count, files.Count);

                DateTime fifty_days_ago = now - TimeSpan.FromDays(50);
                DirectoryCleaner cleaner = new DirectoryCleaner(dd.Di.FullName, "*.txt");
                cleaner.Clean(r => r.LastWriteTime < fifty_days_ago);

                files = new List<FileInfo>(dd.Di.GetFiles());
                Assert.Equal(count / 2 + 1, files.Count);
            }
        }
    }
}
