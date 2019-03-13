using Newtonsoft.Json;
using System;
using Xunit;

namespace UnitTest.json
{
    public class GivenDateTimeOffsetShould
    {
        class TestDTO
        {
            [JsonProperty("timestamp")]
            public DateTimeOffset TimeStamp { get; set; }
        }

        [Fact]
        public void SerializeStringAndDeserializeBackToIdenticalString()
        {
            string dt = "2016-08-17T00:06:26.1212671+03:00";
            DateTimeOffset dto;
            DateTimeOffset.TryParse(dt, out dto);

            TestDTO test = new TestDTO() { TimeStamp = dto };
            string json = JsonConvert.SerializeObject(test);

            var obj = new { timestamp = new DateTimeOffset() };
            var o2 = JsonConvert.DeserializeAnonymousType(json, obj);

            string dt2 = o2.timestamp.ToString("o");

            Assert.Equal(dt, dt2);
        }
    }
}
