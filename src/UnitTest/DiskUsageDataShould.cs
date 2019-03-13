using gov.sandia.sld.common.db.models;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTest
{
    public class DiskUsageDataShould
    {
        [Fact]
        public void ConstructProperly()
        {
            DiskUsageData data = new DiskUsageData()
            {
                dataID = 222,
                collectorID = 333,
                timeStamp = DateTimeOffset.Parse("2018-05-01T10:34:45.777-06:00"),
                capacity = 33445566,
                free = 11223344
            };

            Assert.Equal(222, data.dataID);
            Assert.Equal(333, data.collectorID);
            Assert.Equal(2018, data.timeStamp.Year);
            Assert.Equal(5, data.timeStamp.Month);
            Assert.Equal(1, data.timeStamp.Day);
            Assert.Equal(10, data.timeStamp.Hour);
            Assert.Equal(34, data.timeStamp.Minute);
            Assert.Equal(45, data.timeStamp.Second);
            Assert.Equal(777, data.timeStamp.Millisecond);
            Assert.Equal(TimeSpan.FromHours(-6), data.timeStamp.Offset);
            Assert.Equal((ulong)33445566, data.capacity);
            Assert.Equal((ulong)11223344, data.free);
            Assert.Equal((ulong)(33445566 - 11223344), data.used);
        }

        [Fact]
        public void BuildFromOldJSONProperly()
        {
            string old_json = "{\"Value\": { \"Disk Capacity\":\"999\",\"Disk Free\":\"444\",\"Disk Name\":\"C:\",\"Disk Used\":\"555\"}}";
            List<Tuple<string, DiskUsageData>> list = DiskUsageData.FromJSON(old_json);

            Assert.Single(list);
            Tuple<string, DiskUsageData> tuple = list[0];
            Assert.NotNull(tuple);
            Assert.False(string.IsNullOrEmpty(tuple.Item1));
            Assert.NotNull(tuple.Item2);
            Assert.Equal("C:", tuple.Item1);
            Assert.Equal((ulong)999, tuple.Item2.capacity);
            Assert.Equal((ulong)444, tuple.Item2.free);
            Assert.Equal((ulong)555, tuple.Item2.used);
        }

        /// <summary>
        /// It appears that at some point we serialized the data as numbers, but we currently serialize them as strings.
        /// Make sure either one will work OK.
        /// </summary>
        /// <param name="json"></param>
        [Theory]
        [InlineData("{\"Value\":{\"C:\":{\"Capacity\":449956540416,\"Free\":362764394496,\"Used\":87192145920},\"D:\":{\"Capacity\":3150316564480,\"Free\":3070029946880,\"Used\":80286617600},\"E:\":{\"Capacity\":450062446592,\"Free\":321103839232,\"Used\":128958607360},\"F:\":{\"Capacity\":450062446592,\"Free\":419051577344,\"Used\":31010869248},\"G:\":{\"Capacity\":899993825280,\"Free\":895661461504,\"Used\":4332363776}}}")]
        [InlineData("{\"Value\":{\"C:\":{\"Capacity\":\"449956540416\",\"Free\":\"362764394496\",\"Used\":\"87192145920\"},\"D:\":{\"Capacity\":\"3150316564480\",\"Free\":\"3070029946880\",\"Used\":\"80286617600\"},\"E:\":{\"Capacity\":\"450062446592\",\"Free\":\"321103839232\",\"Used\":\"128958607360\"},\"F:\":{\"Capacity\":\"450062446592\",\"Free\":\"419051577344\",\"Used\":\"31010869248\"},\"G:\":{\"Capacity\":\"899993825280\",\"Free\":\"895661461504\",\"Used\":\"4332363776\"}}}")]
        public void BuildFromNewJSONProperly(string json)
        {
            List<Tuple<string, DiskUsageData>> list = DiskUsageData.FromJSON(json);

            Assert.Equal(5, list.Count);

            for(int i = 0; i < 5; ++i)
            {
                Tuple<string, DiskUsageData> tuple = list[i];

                Assert.NotNull(tuple);
                Assert.False(string.IsNullOrEmpty(tuple.Item1));
                Assert.NotNull(tuple.Item2);

                switch(tuple.Item1)
                {
                    case "C:":
                        Assert.Equal((ulong)449956540416, tuple.Item2.capacity);
                        Assert.Equal((ulong)362764394496, tuple.Item2.free);
                        Assert.Equal((ulong)87192145920, tuple.Item2.used);
                        Assert.Equal((ulong)(449956540416 - 362764394496), tuple.Item2.used);
                        break;
                    case "D:":
                        Assert.Equal((ulong)3150316564480, tuple.Item2.capacity);
                        Assert.Equal((ulong)3070029946880, tuple.Item2.free);
                        Assert.Equal((ulong)80286617600, tuple.Item2.used);
                        break;
                    case "E:":
                        Assert.Equal((ulong)450062446592, tuple.Item2.capacity);
                        Assert.Equal((ulong)321103839232, tuple.Item2.free);
                        Assert.Equal((ulong)128958607360, tuple.Item2.used);
                        break;
                    case "F:":
                        Assert.Equal((ulong)450062446592, tuple.Item2.capacity);
                        Assert.Equal((ulong)419051577344, tuple.Item2.free);
                        Assert.Equal((ulong)31010869248, tuple.Item2.used);
                        break;
                    case "G:":
                        Assert.Equal((ulong)899993825280, tuple.Item2.capacity);
                        Assert.Equal((ulong)895661461504, tuple.Item2.free);
                        Assert.Equal((ulong)4332363776, tuple.Item2.used);
                        break;
                    default:
                        Assert.Equal("", tuple.Item1);
                        Assert.True(false);
                        break;
                }
            }
        }
    }
}
