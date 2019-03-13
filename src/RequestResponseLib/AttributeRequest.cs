namespace gov.sandia.sld.common.requestresponse
{
    public class AttributeRequest : Request
    {
        public bool Get { get; private set; }
        //public bool Set { get { return !Get; } }
        public string Path { get; private set; }
        public string Value { get; set; }

        public AttributeRequest(string path, bool get)
            :base("AttributeRequest: " + path)
        {
            Path = path;
            Get = get;
        }
    }
}
