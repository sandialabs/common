using System;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Specify a DateTime the timer should "fire", and how often
    /// it should repeat. Then, check IsTime() to see if it is
    /// time, and once it is, call Reset() to set it up for the
    /// next time.
    /// </summary>
    public class IntervalTimer
    {
        public DateTime NextTime { get; private set; }
        public TimeSpan Interval { get; private set; }

        //public IntervalTimer(TimeSpan interval)
        //{
        //    NextTime = DateTime.MinValue;
        //    Interval = interval;
        //}

        public IntervalTimer(DateTime next_time, TimeSpan interval)
        {
            NextTime = next_time;
            Interval = interval;
        }

        public bool IsTime()
        {
            return DateTime.Now >= NextTime;
        }

        public void Reset()
        {
            //if (NextTime == DateTime.MinValue)
            //    NextTime = DateTime.Now;

            NextTime += Interval;
        }
    }
}
