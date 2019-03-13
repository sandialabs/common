using gov.sandia.sld.common.db;
using Xunit;

namespace UnitTest
{
    public class StartStopTimeShould
    {
        [Fact]
        public void ConstructWithNullsOK()
        {
            StartStopTime sst = new StartStopTime("null", "null");
            Assert.Null(sst.Start);
            Assert.Null(sst.End);

            sst = new StartStopTime(null, null);
            Assert.Null(sst.Start);
            Assert.Null(sst.End);
        }

        [Fact]
        public void ConstructWithProperDateTimes()
        {
            StartStopTime sst = new StartStopTime("2018-08-01T06:00:00.000Z", "2018-09-01T06:00:00.000Z");
            Assert.NotNull(sst.Start);
            Assert.NotNull(sst.End);
            Assert.Equal(2018, sst.Start.Value.Year);
            Assert.Equal(8, sst.Start.Value.Month);
            Assert.Equal(1, sst.Start.Value.Day);
            Assert.Equal(2018, sst.End.Value.Year);
            Assert.Equal(9, sst.End.Value.Month);
            Assert.Equal(1, sst.End.Value.Day);
        }
    }
}
