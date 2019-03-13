using System;

namespace gov.sandia.sld.common.db.models
{
    public abstract class CurrentPeakReportBase
    {
        public int currentPercentUsed { get; set; }
        public int peakPercentUsed { get; set; }
        public DateTimeOffset? peakTimestamp { get; set; }

        public CurrentPeakReportBase()
        {
            currentPercentUsed = peakPercentUsed = -1;
            peakTimestamp = null;
        }

        public void Insert(int percent_used, DateTimeOffset timestamp)
        {
            // The data is coming back in time order, so the most recent will be the last one.
            // Just update current and the last one will be the most recent.
            currentPercentUsed = percent_used;

            // Do >= here so we will show the most recent peak if there are multiples of the same value
            if (percent_used >= peakPercentUsed)
            {
                peakPercentUsed = percent_used;
                peakTimestamp = timestamp;
            }
        }
    }
}
