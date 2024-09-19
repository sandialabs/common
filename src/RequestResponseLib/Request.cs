namespace gov.sandia.sld.common.requestresponse
{
    public interface IRequest
    {
        string Name { get; }
        bool IsHandled { get; }
        uint HandledCount { get; }

        void Handled();
    }

    public abstract class Request : IRequest
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
