using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.wmi;
using Newtonsoft.Json;
using Xunit;

namespace UnitTest
{
    public class MemoryUsageShould
    {
        [Fact]
        void LookLikeTheOldStructure()
        {
            // We used to just put a Dictionary<string, string> in there, but for better OO principles I changed it to an
            // object. Unfortunately, I broke it when I did that so it stored an array of the objects and it didn't deserialize
            // properly any more. Let's make sure when a Data object with the new object is used that it matches the old format.

            string old_format = "{\"Value\":{\"Memory Capacity\":\"17098178560\",\"Free Memory\":\"15014670336\",\"Memory Used\":\"2083508224\"}}";
            DataCollectorContext context = new DataCollectorContext(new CollectorID(-1, "Memory"), ECollectorType.Memory);
            MemoryUsage usage = new MemoryUsage() { CapacityNum = 17098178560, FreeNum = 15014670336 };
            Data d = new GenericData<MemoryUsage>(context, usage);
            string new_format = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.None,
                    new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });

            Assert.Equal(old_format, new_format);
        }
    }
}
