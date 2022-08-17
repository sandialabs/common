using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    public interface IRequestResponderBus
    {
        void Subscribe(IResponder responder);
        void Unsubscribe(IResponder responder);
        void MakeRequest(IRequest request);
    }

    public interface IMessageBus
    {
        void Subscribe(IReceiver receiver);
        void Unsubscribe(IReceiver receiver);
        void SendMessage(IMessage message);
    }

    public class RequestResponderBus : IRequestResponderBus
    {
        private List<IResponder> m_responders => new List<IResponder>();

        public void Subscribe(IResponder responder)
        {
            lock (m_responders)
                m_responders.Add(responder);
        }

        public void Unsubscribe(IResponder responder)
        {
            lock (m_responders)
                m_responders.Remove(responder);
        }

        public void MakeRequest(IRequest request)
        {
            // Make a local copy of the responders so we don't hold the lock for very long
            List<IResponder> responders;
            lock (m_responders)
                responders = new List<IResponder>(m_responders);

            responders.ForEach(r =>
            {
                try
                {
                    r.HandleRequest(request);
                }
                catch (Exception e)
                {
                    ApplicationEventLog log = new ApplicationEventLog();
                    log.Log(e);
                };
            });
        }
    }

    public class MessageBus : IMessageBus
    {
        private List<IReceiver> m_receivers => new List<IReceiver>();

        public void Subscribe(IReceiver receiver)
        {
            lock (m_receivers)
                m_receivers.Add(receiver);
        }

        public void Unsubscribe(IReceiver receiver)
        {
            lock (m_receivers)
                m_receivers.Remove(receiver);
        }

        public void SendMessage(IMessage message)
        {
            // Make a local copy of the receivers so we don't hold the lock for very long
            List<IReceiver> receivers;
            lock (m_receivers)
                receivers = new List<IReceiver>(m_receivers);

            receivers.ForEach(r =>
            {
                try
                {
                    r.OnMessage(message);
                }
                catch (Exception e)
                {
                    ApplicationEventLog log = new ApplicationEventLog();
                    log.Log(e);
                };
            });
        }
    }

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
        private RequestResponderBus m_requestResponderBus => new RequestResponderBus();
        private MessageBus m_messageBus => new MessageBus();

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
