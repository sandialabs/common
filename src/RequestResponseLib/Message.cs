namespace gov.sandia.sld.common.requestresponse
{
    public interface IMessage
    {
        bool IsHandled { get; }
        uint HandledCount { get; }

        void Handled();
    }

    public abstract class Message : IMessage
    {
        public bool IsHandled { get { return HandledCount > 0; } }
        public uint HandledCount { get; private set; }

        public Message()
        {
            HandledCount = 0;
        }

        public void Handled()
        {
            ++HandledCount;
        }
    }
}
