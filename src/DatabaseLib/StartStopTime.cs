using System;

namespace gov.sandia.sld.common.db
{
    public class StartStopTime : IStartEndTime
    {
        private DateTimeOffset? _Start;
        private DateTimeOffset? _End;

        public DateTimeOffset? Start { get { return _Start; } }
        public DateTimeOffset? End { get { return _End; } }

        public StartStopTime()
        {
            _Start = _End = null;
        }

        public StartStopTime(string start, string end)
        {
            DateTimeOffset temp = DateTimeOffset.MinValue;
            if (DateTimeOffset.TryParse(start, out temp))
                _Start = temp;
            temp = DateTimeOffset.MinValue;
            if (DateTimeOffset.TryParse(end, out temp))
                _End = temp;
        }
    }
}
