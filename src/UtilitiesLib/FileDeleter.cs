using System;
using System.IO;

namespace gov.sandia.sld.common.utilities
{
    public class FileDeleter : IDisposable
    {
        public FileInfo Fi { get; }

        public FileDeleter(FileInfo fi)
        {
            Fi = fi;
        }

        public void Dispose()
        {
            Fi.Delete();
        }
    }
}
