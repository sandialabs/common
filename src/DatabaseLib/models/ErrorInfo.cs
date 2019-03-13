using System;

namespace gov.sandia.sld.common.db.models
{
    public class ErrorInfo
    {
        public string message { get; set; }
        public DateTimeOffset timestamp { get; set; }

        public int count { get; set; }
        public DateTimeOffset? firstTimestamp { get; set; }
        public DateTimeOffset? lastTimestamp { get; set; }

        public ErrorInfo()
        {
            message = string.Empty;
            timestamp = DateTimeOffset.MinValue;

            count = 1;
            firstTimestamp = lastTimestamp = null;
        }

        public void IncrementCount(DateTimeOffset timestamp)
        {
            ++count;
            if (firstTimestamp.HasValue == false)
                firstTimestamp = timestamp;
            if (lastTimestamp.HasValue == false)
                lastTimestamp = timestamp;

            if (timestamp.CompareTo(firstTimestamp.Value) < 0)
                firstTimestamp = timestamp;
            if (timestamp.CompareTo(lastTimestamp.Value) > 0)
                lastTimestamp = timestamp;
        }
    }
}
