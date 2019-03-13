using gov.sandia.sld.common.configuration;
using Xunit;

namespace UnitTest.statustype
{
    public class TimeStatusShould
    {
        [Fact]
        public void HandleRecentRebootProperly()
        {
            TimeStatus ts = new TimeStatus(2, 365);

            EStatusType? status = ts.GetStatus(0);
            Assert.NotNull(status);
            Assert.Equal(EStatusType.RecentlyRebooted, status.Value);
            status = ts.GetStatus(1);
            Assert.NotNull(status);
            Assert.Equal(EStatusType.RecentlyRebooted, status.Value);
            status = ts.GetStatus(2);
            Assert.Null(status);
        }

        [Fact]
        public void HandleLongRebootProperly()
        {
            TimeStatus ts = new TimeStatus(2, 365);

            EStatusType? status = ts.GetStatus(363);
            Assert.Null(status);
            status = ts.GetStatus(364);
            Assert.Null(status);
            status = ts.GetStatus(365);
            Assert.NotNull(status);
            Assert.Equal(EStatusType.ExcessiveUptime, status.Value);
            status = ts.GetStatus(366);
            Assert.NotNull(status);
            Assert.Equal(EStatusType.ExcessiveUptime, status.Value);
        }
    }
}
