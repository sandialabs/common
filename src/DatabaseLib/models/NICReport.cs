using System;

namespace gov.sandia.sld.common.db.models
{
    public class NICReport : CurrentPeakReportBase
    {
        public int bps { get; set; }
        public int peakBps { get; set; }

        public NICReport() : base()
        {
            bps = peakBps = -1;
        }

        public void Insert(int percent_used, DateTimeOffset timestamp, int bps)
        {
            // The data is coming back in time order, so the most recent will be the last one.
            // Just update current and the last one will be the most recent.
            currentPercentUsed = percent_used;
            this.bps = bps;

            // Do >= here so we will show the most recent peak if there are multiples of the same value
            if (bps >= peakBps)
            {
                peakPercentUsed = percent_used;
                peakBps = bps;
                peakTimestamp = timestamp;
            }
        }
    }
}
