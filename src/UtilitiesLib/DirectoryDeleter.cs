using System;
using System.Collections.Generic;
using System.IO;

namespace gov.sandia.sld.common.utilities
{
    public class DirectoryDeleter : IDisposable
    {
        public DirectoryInfo Di { get; }

        public DirectoryDeleter(DirectoryInfo di)
        {
            Di = di;
        }

        public void Dispose()
        {
            new List<FileInfo>(Di.GetFiles()).ForEach(f => f.Delete());
            Di.Delete();
        }
    }
}
