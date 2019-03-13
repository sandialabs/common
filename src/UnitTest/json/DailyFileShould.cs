using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace UnitTest.json
{
    /// <summary>
    /// This test class is to test the code written by Hai Le
    /// Record2 and CommonData2 are like the classes he wrote
    /// so we're making sure they read the file in OK.
    /// </summary>
    public class DailyFileShould
    {
        public class Record2
        {
            [JsonProperty("collector")]
            public string Collector { get; set; }

            [JsonProperty("type")]
            public ECollectorType Type { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("timestamp")]
            public DateTimeOffset Timestamp { get; set; }
        }

        public class CommonData2
        {
            /// <summary>
            /// Country Code used in generating the Daily File names
            /// </summary>
            [JsonProperty("countryCode")]
            public string CountryCode { get; set; }

            /// <summary>
            /// Site Name used in generating the Daily File names
            /// </summary>
            [JsonProperty("siteName")]
            public string SiteName { get; set; }

            /// <summary>
            /// Day for which this daily file contains data
            /// </summary>
            [JsonProperty("day")]
            public string Day { get; set; }

            /// <summary>
            /// Items of interest that COMMON collects and maintains data.
            /// Each record contains the “collector” name (label), type, the values collected, and the time stamp.
            /// </summary>
            [JsonProperty("records")]
            public List<Record2> Records { get; set; }
        }

        [Fact]
        public void ProperlyDeserializeFileToObject()
        {
            string text = File.ReadAllText(@"dj_doraleh container terminal_2016-07-01.json");
            CommonData2 cData = JsonConvert.DeserializeObject<CommonData2>(text);
            string val = cData.Records[0].Timestamp.ToString("o");
            Assert.Equal("2016-07-01T00:17:17.5123804+03:00", val);
        }
    }
}
