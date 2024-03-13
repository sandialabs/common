namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// There's a single instance of the SystemBus. There are two forms:
    /// 
    /// 1) Requests are put on the bus, and then passed on to each of the Responders.
    /// If a Responder can handle the Request, he updates the Request object as necessary.
    /// 2) Messages are put on the bus, and they are passed on to each of the Receivers.
    /// These are one-way: the Message object shouldn't be changed in any way.
    ///
    /// Both Responder, Request, Message, and Receiver are abstract since there's no way to know what
    /// the actual request/response/message/receiver will be.
    /// 
    /// Subscribe as a Responder/Receiver to be notified when a Request/Message is put on the bus.
    /// Unsubscribe when you no longer want to be called.
    /// </summary>
    public class SystemBus : IRequestResponderBus, IMessageBus
    {
        private readonly RequestResponderBus m_requestResponderBus;
        private readonly MessageBus m_messageBus;

        private static SystemBus c_instance;

        public static SystemBus Instance
        {
            get
            {
                if (c_instance == null)
                    c_instance = new SystemBus();
                return c_instance;
            }
        }

        public SystemBus()
        {
            m_requestResponderBus = new RequestResponderBus();
            m_messageBus = new MessageBus();
        }

        public void Subscribe(IResponder responder)
        {
            m_requestResponderBus.Subscribe(responder);
        }

        public void Unsubscribe(IResponder responder)
        {
            m_requestResponderBus.Unsubscribe(responder);
        }

        public void MakeRequest(IRequest request)
        {
            m_requestResponderBus.MakeRequest(request);
        }

        public void Subscribe(IReceiver receiver)
        {
            m_messageBus.Subscribe(receiver);
        }

        public void Unsubscribe(IReceiver receiver)
        {
            m_messageBus.Unsubscribe(receiver);
        }

        public void SendMessage(IMessage message)
        {
            m_messageBus.SendMessage(message);
        }
    }
}
