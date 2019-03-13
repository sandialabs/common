using gov.sandia.sld.common.utilities;
using Xunit;

namespace UnitTest
{
    public class ThresholdShould
    {
        [Fact]
        public void ThresholdConstructsProperly()
        {
            Threshold t = new Threshold(50);
            Assert.Equal(50.0, t.Transition, 1);
        }

        [Fact]
        public void Threshold2ConstructsProperly()
        {
            Threshold2 t = new Threshold2(40, 50);
            Assert.Equal(40.0, t.Transition, 1);
            Assert.Equal(50.0, t.Transition2, 1);
        }

        [Fact]
        public void Threshold2HandlesBadOrderProperly()
        {
            // Make sure the thresholds are properly swapped
            Threshold2 t = new Threshold2(-200, -400);

            Assert.Equal(-400.0, t.Transition, 1);
            Assert.Equal(-200.0, t.Transition2, 1);
        }

        [Fact]
        public void PercentThresholdConstructsProperly()
        {
            PercentThreshold t = new PercentThreshold(80.0);

            Assert.Equal(80.0, t.Transition, 1);
        }

        [Fact]
        public void PercentThresholdHandlesMaxThresholdsProperly()
        {
            PercentThreshold t = new PercentThreshold(-1.0);
            Assert.Equal(0.0, t.Transition, 1);

            t = new PercentThreshold(101.0);
            Assert.Equal(100.0, t.Transition, 1);
        }

        [Fact]
        public void PercentThreshold2HandlesBadOrderProperly()
        {
            // Make sure the thresholds are properly swapped
            PercentThreshold2 t = new PercentThreshold2(90.0, 80.0);

            Assert.Equal(80.0, t.Transition, 1);
            Assert.Equal(90.0, t.Transition2, 1);
        }

        [Fact]
        public void PercentThreshold2HandlesMaxThresholdsProperly()
        {
            PercentThreshold2 mem_status = new PercentThreshold2(-1.0, 101.0);

            Assert.Equal(0.0, mem_status.Transition, 1);
            Assert.Equal(100.0, mem_status.Transition2, 1);
        }
    }
}
