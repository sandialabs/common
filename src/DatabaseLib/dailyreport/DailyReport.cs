using gov.sandia.sld.common.configuration;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.dailyreport
{
    public class Day
    {
        public DateTimeOffset day { get; protected set; }
    }

    public class DailyReport : Day
    {
        public string version { get; private set; }
        public string countryCode { get; private set; }
        public string siteName { get; private set; }
        public List<Record> records { get; private set; }

        public class Record
        {
            public string collector { get; set; }
            public ECollectorType type { get; set; }
            public string value { get; set; }
            public DateTimeOffset timestamp { get; set; }
        }

        public DailyReport(string country_code, string site_name, DateTimeOffset day, string version)
        {
            this.version = version;
            countryCode = country_code;
            siteName = site_name;
            this.day = day;
            records = new List<Record>();
        }
    }
}
