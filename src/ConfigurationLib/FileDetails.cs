using System;

namespace gov.sandia.sld.common.configuration
{
    /// <summary>
    /// Used to get info about a file, such as it's size, modification date,
    /// and SHA hash. Is used to determine if a file's changed.
    /// </summary>
    public class FileDetails
    {
        public string Name { get; set; }
        public UInt64 Size { get; set; }
        public DateTimeOffset Modification { get; set; }
        public string SHA256 { get; set; }

        public FileDetails()
        {
            Name = SHA256 = string.Empty;
            Size = 0;
            Modification = DateTimeOffset.MinValue;
        }
    }
}
