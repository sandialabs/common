using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// The bus for requests/responses. Allows users to subscribe
    /// as a responder for a specific type of request. When a request
    /// is sent through MakeRequest(), it'll be passed off to each
    /// responder. If a given responder can handle the request,
    /// it will do whatever is appropriate, updating the IRequest
    /// object as necessary.
    /// </summary>
    public class RequestResponderBus : IRequestResponderBus
    {
        private readonly List<IResponder> m_responders;

        public RequestResponderBus()
        {
            m_responders = new List<IResponder>();
        }

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
}
