using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    public class Services
    {
        public List<string> services { get; set; }
        public DateTimeOffset? timestamp { get; set; }

        public Services()
        {
            services = new List<string>();
            timestamp = null;
        }

        public Services(List<string> services)
        {
            this.services = services;
            timestamp = null;
        }
    }
}
