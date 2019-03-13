using gov.sandia.sld.common.data.wmi;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.diskusage
{
    public class DiskUsageDeserializeShould : IClassFixture<DiskUsageFixture>
    {
        private readonly DiskUsageFixture _fixture;

        public DiskUsageDeserializeShould(DiskUsageFixture fixture)
        {
            this._fixture = fixture;
        }

        /// <summary>
        /// Make sure we properly read the original JSON into the new DiskUsage class
        /// </summary>
        [Fact]
        public void ProperlyReadCAndEDrives()
        {
            var definition = new { Value = new Dictionary<string, DiskUsage>() };
            var value = JsonConvert.DeserializeAnonymousType(_fixture.Value, definition);
            Dictionary<string, DiskUsage> map = value.Value;

            Assert.Equal(2, map.Count);
            DiskUsage c = null;
            Assert.True(map.TryGetValue("C:", out c));
            DiskUsage e = null;
            Assert.True(map.TryGetValue("E:", out e));
            Assert.NotNull(c);
            Assert.NotNull(e);
        }

        [Fact]
        public void ProperlySeeNoFDrive()
        {
            var definition = new { Value = new Dictionary<string, DiskUsage>() };
            var value = JsonConvert.DeserializeAnonymousType(_fixture.Value, definition);
            Dictionary<string, DiskUsage> map = value.Value;

            Assert.Equal(2, map.Count);
            DiskUsage f = null;
            Assert.False(map.TryGetValue("F:", out f));
            Assert.Null(f);
        }

        [Fact]
        public void SeeCorrectValuesInCAndEDrives()
        {
            var definition = new { Value = new Dictionary<string, DiskUsage>() };
            var value = JsonConvert.DeserializeAnonymousType(_fixture.Value, definition);
            Dictionary<string, DiskUsage> map = value.Value;

            DiskUsage c = null;
            Assert.True(map.TryGetValue("C:", out c));
            DiskUsage e = null;
            Assert.True(map.TryGetValue("E:", out e));

            _fixture.TestC(c);
            _fixture.TestE(e);
        }
    }
}
