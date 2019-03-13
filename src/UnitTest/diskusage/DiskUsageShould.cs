using gov.sandia.sld.common.data.wmi;
using Xunit;

namespace UnitTest.diskusage
{
    public class DiskUsageShould : IClassFixture<DiskUsageFixture>
    {
        private readonly DiskUsageFixture _fixture;

        public DiskUsageShould(DiskUsageFixture fixture)
        {
            this._fixture = fixture;
        }

        /// <summary>
        /// Make sure we properly read the original JSON into the new DiskUsage class
        /// </summary>
        [Fact]
        public void ConstructFromStringsProperly()
        {
            DiskUsage c = new DiskUsage() { Capacity = _fixture.CCapacity.ToString(), Free = _fixture.CFree.ToString() };
            DiskUsage e = new DiskUsage() { Capacity = _fixture.ECapacity.ToString(), Free = _fixture.EFree.ToString() };

            _fixture.TestC(c);
            _fixture.TestE(e);
        }

        [Fact]
        public void ConstructFromUInt64Properly()
        {
            DiskUsage c = new DiskUsage() { CapacityNum = _fixture.CCapacity, FreeNum = _fixture.CFree };
            DiskUsage e = new DiskUsage() { CapacityNum = _fixture.ECapacity, FreeNum = _fixture.EFree };

            _fixture.TestC(c);
            _fixture.TestE(e);
        }
    }
}
