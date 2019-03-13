using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    /// <summary>
    /// There's a single instance of the RequestBus. Requests are put on the bus,
    /// and then passed on to each of the Responders. If a Responder can handle the
    /// Request, he updates the Request as necessary.
    /// 
    /// Both Responder and Request are abstract since there's no way to know what
    /// the actual request/response will be.
    /// </summary>
    public class RequestBus
    {
        public static RequestBus Instance
        {
            get
            {
                if (c_instance == null)
                    c_instance = new RequestBus();
                return c_instance;
            }
        }

        public void Subscribe(Responder responder)
        {
            lock (m_lock)
                m_responders.Add(responder);
        }

        public void Unsubscribe(Responder responder)
        {
            lock (m_lock)
                m_responders.Remove(responder);
        }

        public void MakeRequest(Request request)
        {
            // Make a local copy of the responders so we don't hold the lock for very long
            List<Responder> responders;
            lock (m_lock)
                responders = new List<Responder>(m_responders);

            responders.ForEach(r =>
                {
                    try
                    {
                        r.HandleRequest(request);
                    }
                    catch(Exception e)
                    {
                        ApplicationEventLog log = new ApplicationEventLog();
                        log.Log(e);
                    };
                }
            );
        }

        public RequestBus()
        {
            m_lock = new object();
            m_responders = new List<Responder>();
        }

        private object m_lock;
        private List<Responder> m_responders;

        private static RequestBus c_instance;
    }
}
