using gov.sandia.sld.common.data.wmi;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.diskusage
{
    public class DiskUsageSerializeShould : IClassFixture<DiskUsageFixture>
    {
        private readonly DiskUsageFixture _fixture;

        public DiskUsageSerializeShould(DiskUsageFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void SerializeToStringProperly()
        {
            DiskUsage c = new DiskUsage() { CapacityNum = _fixture.CCapacity, FreeNum = _fixture.CFree };
            DiskUsage e = new DiskUsage() { CapacityNum = _fixture.ECapacity, FreeNum = _fixture.EFree };
            Dictionary<string, DiskUsage> m = new Dictionary<string, DiskUsage>();
            m["C:"] = c;
            m["E:"] = e;
            var value = new { Value = m };

            string serialized = JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });

            Assert.Equal(_fixture.Value, serialized);
        }
    }
}
