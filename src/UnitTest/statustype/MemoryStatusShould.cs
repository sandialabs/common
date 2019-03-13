using gov.sandia.sld.common.configuration;
using Xunit;

namespace UnitTest.statustype
{
    public class MemoryStatusShould
    {
        [Fact]
        public void GenerateStatusTypesProperly()
        {
            MemoryStatus mem_status = new MemoryStatus(80.0, 90.0);

            EStatusType status = mem_status.GetStatus(70, 100);
            Assert.Equal(EStatusType.AdequateFreeMemory, status);

            status = mem_status.GetStatus(80, 100);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(90, 100);
            Assert.Equal(EStatusType.CriticallyLowFreeMemory, status);

            status = mem_status.GetStatus(99, 100);
            Assert.Equal(EStatusType.CriticallyLowFreeMemory, status);

            status = mem_status.GetStatus(0, 100);
            Assert.Equal(EStatusType.AdequateFreeMemory, status);
        }

        [Fact]
        public void HandleLargeNumbersOK()
        {
            MemoryStatus mem_status = new MemoryStatus(80.0, 90.0);

            // Handle large #s OK
            EStatusType status = mem_status.GetStatus(10_000_000_000_000, 10_500_000_000_000);
            Assert.Equal(EStatusType.CriticallyLowFreeMemory, status);
        }

        [Fact]
        public void HandleMaxThresholdsProperly()
        {
            MemoryStatus mem_status = new MemoryStatus(0.0, 100.0);

            EStatusType status = mem_status.GetStatus(0, 1000);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(1, 1000);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(999, 1000);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(1000, 1000);
            Assert.Equal(EStatusType.CriticallyLowFreeMemory, status);
        }

        [Fact]
        public void HandleThresholdTransitionsProperly()
        {
            MemoryStatus mem_status = new MemoryStatus(80.0, 90.0);

            EStatusType status = mem_status.GetStatus(799, 1000);
            Assert.Equal(EStatusType.AdequateFreeMemory, status);

            status = mem_status.GetStatus(800, 1000);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(899, 1000);
            Assert.Equal(EStatusType.LowFreeMemory, status);

            status = mem_status.GetStatus(900, 1000);
            Assert.Equal(EStatusType.CriticallyLowFreeMemory, status);
        }
    }
}
