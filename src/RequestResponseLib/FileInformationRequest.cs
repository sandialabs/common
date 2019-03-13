using gov.sandia.sld.common.configuration;
using System;

namespace gov.sandia.sld.common.requestresponse
{
    public class FileInformationRequest : Request
    {
        public EFileType Type { get; private set; }
        public string Filename { get; private set; }
        public Int64 ID { get; set; }
        public FileDetails Details { get; set; }

        public FileInformationRequest(EFileType type, string filename)
            : base("FileInformationRequest: " + filename)
        {
            Type = type;
            Filename = filename;
            ID = -1;
        }
    }
}
