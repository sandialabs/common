using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.database;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.database
{
    internal class MockedDBCollector : IDatabaseCollector
    {
        public Tuple<Dictionary<string, ulong>, bool> GetData(string connection_string)
        {
            Dictionary<string, ulong> data = new Dictionary<string, ulong>()
            {
                ["Everest"] = 29_029,
                ["K2"] = 28_251,
                ["Kangchenjunga"] = 28_169,
                ["Lhotse"] = 27_940,
                ["Makalu"] = 27_838
            };
            return Tuple.Create(data, true);
        }
    }

    internal class MockedDatabaseCollectorFactory : IDatabaseCollectorFactory
    {
        public IDatabaseCollector Create(EDatabaseType type)
        {
            // Ignore the type parameter
            return new MockedDBCollector();
        }
    }

    public class DatabaseSizeCollectorShould
    {
        [Fact]
        public void ReturnReasonableData()
        {
            CollectorID id = new CollectorID(1234, "Himalaya");
            DatabaseSizeCollector coll = new DatabaseSizeCollector(id, false, new MockedDatabaseCollectorFactory());
            CollectedData data = coll.OnAcquire();

            Assert.NotNull(data);
            Assert.True(data.DataIsCollected);
            Assert.NotNull(data.D);
            Assert.Single(data.D);
            Assert.IsType<ListData<Dictionary<string, string>>>(data.D[0]);
            ListData<Dictionary<string, string>> ld = (ListData<Dictionary<string, string>>)data.D[0];
            Assert.Equal(5, ld.Data.Count);
            Dictionary<string, string> everest = ld.Data[0];
            Assert.True(everest.ContainsKey("Name"));
            Assert.Equal("Everest", everest["Name"]);
            Assert.Equal("29029", everest["Size"]);
        }
    }
}
