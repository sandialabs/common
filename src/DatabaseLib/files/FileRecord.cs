using gov.sandia.sld.common.configuration;
using System;

namespace gov.sandia.sld.common.db.files
{
    public class FileRecord
    {
        public Int64 ID { get; set; }
        public EFileType Type { get; set; }
        public FileDetails Details { get; set; }

        public FileRecord()
        {
            ID = -1;
            Type = EFileType.Unknown;
        }
    }
}
