using System.ComponentModel;

namespace gov.sandia.sld.common.configuration
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa394173(v=vs.85).aspx
    public enum EDriveType
    {
        Unknown,
        [Description("No root rirectory")]
        NoRootDirectory,
        [Description("Removable disk")]
        RemovableDisk,
        [Description("Local disk")]
        LocalDisk,
        [Description("Network drive")]
        NetworkDrive,
        [Description("Compact disc")]
        CompactDisc,
        [Description("RAM disk")]
        RAMDisk,
    }
}
