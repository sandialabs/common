namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// The IRequestResponderBus is an interface for a Bus that allows
    /// someone to make a request for information, and get a response back.
    /// </summary>
    public interface IRequestResponderBus
    {
        void Subscribe(IResponder responder);
        void Unsubscribe(IResponder responder);
        void MakeRequest(IRequest request);
    }
}
