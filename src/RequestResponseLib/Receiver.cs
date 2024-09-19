namespace gov.sandia.sld.common.requestresponse
{
    public interface IReceiver
    {
        void OnMessage(IMessage message);
    }
}
