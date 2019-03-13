namespace gov.sandia.sld.common.requestresponse
{
    public abstract class Request
    {
        public string Name { get; private set; }
        public bool IsHandled { get { return HandledCount > 0; } }
        public uint HandledCount { get; private set; }

        public Request(string name)
        {
            Name = name;
            HandledCount = 0;
        }

        public void Handled()
        {
            ++HandledCount;
        }
    }
}
