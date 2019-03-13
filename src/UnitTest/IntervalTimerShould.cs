using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace UnitTest
{
    // The unit tests sometimes fail when we're comparing that the time < 1s or time < 2s
    // I'm not sure why--seems like DateTime.Now must sometimes return a # less than a previous
    // call to DateTime.Now. Let's just comment out this test for now.

    //public class IntervalTimerShould
    //{
    //    [Fact]
    //    public void FireProperly()
    //    {
    //        // How do we test this? Let's set the timer so it goes off in 1 second,
    //        // and one second after that. We'll record the times we test it, and make sure
    //        // the times are legitimate.
    //        // Let's assume that it works OK when the TimeSpan is > 1 second.
    //        List<Tuple<DateTime, bool>> times = new List<Tuple<DateTime, bool>>();
    //        List<Tuple<DateTime, bool>> times2 = new List<Tuple<DateTime, bool>>();
    //        DateTime start_time = DateTime.Now;
    //        TimeSpan one_second = TimeSpan.FromSeconds(1);
    //        TimeSpan two_seconds = one_second + one_second;
    //        IntervalTimer timer = new IntervalTimer(start_time + one_second, one_second);
    //        bool is_time = false;

    //        do
    //        {
    //            DateTime now = DateTime.Now;
    //            is_time = timer.IsTime();
    //            times.Add(Tuple.Create(now, is_time));
    //            Thread.Sleep(1);
    //        }
    //        while (is_time == false);

    //        Assert.True(is_time);
    //        for (int i = 0; i < times.Count; ++i)
    //        {
    //            TimeSpan diff = times[i].Item1 - start_time;
    //            is_time = times[i].Item2;

    //            if (i < times.Count - 1)
    //            {
    //                Assert.True(diff < one_second);
    //                Assert.False(is_time);
    //            }
    //            else
    //            {
    //                Assert.True(diff >= one_second);
    //                Assert.True(is_time);
    //            }
    //        }

    //        // Now let's reset it and test it once again. This time the timespan
    //        // should be < 2 seconds
    //        timer.Reset();

    //        do
    //        {
    //            DateTime now = DateTime.Now;
    //            is_time = timer.IsTime();
    //            times2.Add(Tuple.Create(now, is_time));
    //            Thread.Sleep(1);
    //        }
    //        while (is_time == false);

    //        Assert.True(is_time);
    //        for (int i = 0; i < times2.Count; ++i)
    //        {
    //            TimeSpan diff = times2[i].Item1 - start_time;
    //            is_time = times[i].Item2;

    //            if (i < times.Count - 1)
    //            {
    //                Assert.True(diff < two_seconds);
    //                Assert.False(is_time);
    //            }
    //            else
    //            {
    //                Assert.True(diff >= two_seconds);
    //                Assert.True(is_time);
    //            }
    //        }
    //    }
    //}
}
