namespace gov.sandia.sld.common.requestresponse
{
    public class COMMONDBSizeRequest : Request
    {
        public uint SizeInMB { get; set; }
        public string Filename { get; set; }

        public COMMONDBSizeRequest()
            : base("COMMON DB Size")
        {
        }
    }
}
