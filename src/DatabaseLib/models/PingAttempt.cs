using System;

namespace gov.sandia.sld.common.db.models
{
    public class PingAttempt
    {
        public bool successful { get; set; }
        public DateTimeOffset timestamp { get; set; }
        public long responseTimeMS { get; set; }

        public PingAttempt()
        {
            successful = false;
            timestamp = DateTimeOffset.MinValue;
            responseTimeMS = 0;
        }
    }

    //public class PingCounts
    //{
    //    public DateTimeOffset timestamp { get; set; }
    //    public int success { get; set; }
    //    public int fail { get; set; }
    //    public long totalResponseTimeMS { get; set; }

    //    public PingCounts(DateTimeOffset timestamp)
    //    {
    //        this.timestamp = timestamp;
    //        success = fail = 0;
    //        totalResponseTimeMS = 0;
    //    }
    //}
}
