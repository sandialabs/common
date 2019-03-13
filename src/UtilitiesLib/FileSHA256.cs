using System;
using System.IO;
using System.Security.Cryptography;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Calculate a SHA256 hash of the specified file
    /// </summary>
    public class FileSHA256
    {
        public FileInfo File { get; set; }
        public string SHA256
        {
            get
            {
                string sha256 = string.Empty;

                try
                {
                    using (FileStream fs = System.IO.File.OpenRead(File.FullName))
                    {
                        SHA256Managed sha = new SHA256Managed();
                        byte[] hash = sha.ComputeHash(fs);
                        sha256 = BitConverter.ToString(hash).Replace("-", string.Empty);
                    }
                }
                catch (Exception)
                {
                }
                return sha256;
            }
        }

        public FileSHA256(FileInfo file)
        {
            File = file;
        }
    }
}
