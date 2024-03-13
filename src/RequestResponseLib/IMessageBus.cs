namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// The IMessageBus is an interface for a Bus that allows one-way
    /// communication--sending a message to whoever might be interested.
    /// </summary>
    public interface IMessageBus
    {
        void Subscribe(IReceiver receiver);
        void Unsubscribe(IReceiver receiver);
        void SendMessage(IMessage message);
    }
}
