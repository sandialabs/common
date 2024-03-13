using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// The bus for one-way messages. Allows users to subscribe
    /// as a receiver for a specific type of message. When a message
    /// is sent through SendMessage(), it'll be passed off to each
    /// receiver. If a given receiver is interested in the message
    /// it can do whatever is appropriate.
    /// </summary>
    public class MessageBus : IMessageBus
    {
        private readonly List<IReceiver> m_receivers;

        public MessageBus()
        {
            m_receivers = new List<IReceiver>();
        }

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
}
