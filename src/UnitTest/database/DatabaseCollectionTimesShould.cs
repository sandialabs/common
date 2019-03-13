using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Xunit;

namespace UnitTest.database
{
    public class DatabaseCollectionTimesShould
    {
        [Fact]
        public void IndicateNullsProperly()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();
                    Initializer init = new Initializer(null);
                    init.Initialize(db);

                    Tuple<List<DeviceInfo>, DateTimeOffset> devices = LoadDevices(db, conn);

                    string sql = "SELECT Name, LastCollectionAttempt, LastCollectedAt, NextCollectionTime FROM Collectors WHERE IsEnabled = 1";
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Assert.False(reader.IsDBNull(0));
                            Assert.True(reader.IsDBNull(1));
                            Assert.True(reader.IsDBNull(2));
                            Assert.True(reader.IsDBNull(3));
                        }
                    }
                }
            }
        }

        [Fact]
        public void UseDBCollectionTimeRetrieverProperly()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();
                    Initializer init = new Initializer(null);
                    init.Initialize(db);

                    Tuple<List<DeviceInfo>, DateTimeOffset> devices = LoadDevices(db, conn);

                    int count = -1;
                    string sql = "SELECT COUNT(*) FROM Collectors WHERE IsEnabled = 1 AND FrequencyInMinutes > 0";
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            count = reader.GetInt32(0);
                    }

                    DBCollectionTimeRetriever retriever = new DBCollectionTimeRetriever(conn);
                    Assert.Equal(count, retriever.AllIDs.Count);
                }
            }
        }

        [Fact]
        public void UpdateBeingCollectedProperly()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();
                    Initializer init = new Initializer(null);
                    init.Initialize(db);

                    Tuple<List<DeviceInfo>, DateTimeOffset> devices = LoadDevices(db, conn);

                    TimeSpan one_second = TimeSpan.FromSeconds(1);
                    DBCollectionTimeRetriever retriever = new DBCollectionTimeRetriever(conn);

                    foreach (long collector_id in retriever.AllIDs)
                    {
                        string sql = $"SELECT LastCollectionAttempt, LastCollectedAt, NextCollectionTime, CurrentlyBeingCollected, FrequencyInMinutes FROM Collectors WHERE CollectorID = {collector_id}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Before being collected, the CurrentlyBeingCollected column should have 0
                                Assert.True(reader.IsDBNull(0));
                                Assert.True(reader.IsDBNull(1));
                                Assert.True(reader.IsDBNull(2));
                                Assert.False(reader.IsDBNull(3));
                                Assert.Equal(0, reader.GetInt32(3));
                                Assert.False(reader.IsDBNull(4));
                            }
                        }

                        using (BeingCollected bc = new BeingCollected(collector_id, conn))
                        {
                            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Now that it's being collected, the LastCollectionAttempt and NextCollectionTime columns
                                    // should have something there, and the CurrentlyBeingCollected column should have 1
                                    Assert.False(reader.IsDBNull(0));
                                    Assert.True(reader.IsDBNull(1));
                                    Assert.False(reader.IsDBNull(2));
                                    Assert.False(reader.IsDBNull(3));
                                    Assert.Equal(1, reader.GetInt32(3));

                                    if (DateTimeOffset.TryParse(reader.GetString(0), out DateTimeOffset lca))
                                    {
                                        // Make sure it just happened...let's say within 1 second of now
                                        DateTimeOffset now = DateTimeOffset.Now;
                                        DateTimeOffset lower = now - one_second;
                                        Assert.InRange(lca, lower, now);

                                        // And make sure NextCollectionTime is correct--lets just see if the time is
                                        // in the future.
                                        int frequency = reader.GetInt32(4);
                                        if (frequency > 0 && DateTimeOffset.TryParse(reader.GetString(2), out DateTimeOffset next))
                                            Assert.True(next >= now);
                                    }
                                    else
                                        Assert.True(false);
                                }
                            }
                        }

                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // And after being collected, the CurrentlyBeingCollected column should have 0 again
                                Assert.False(reader.IsDBNull(0));
                                Assert.True(reader.IsDBNull(1));
                                Assert.False(reader.IsDBNull(2));
                                Assert.False(reader.IsDBNull(3));
                                Assert.Equal(0, reader.GetInt32(3));
                            }
                        }

                        // The LastCollectedAt column should remain NULL because it's set when the data is stored and
                        // we didn't store anything here.
                    }

                    // Now make sure none of them show as being collected
                    foreach (long collector_id in retriever.AllIDs)
                    {
                        string sql = $"SELECT CurrentlyBeingCollected FROM Collectors WHERE CollectorID = {collector_id}";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                Assert.Equal(0, reader.GetInt32(0));
                        }
                    }

                    // And make sure that if an exception occurs while marking it as being collected that it shows as
                    // no longer being collected
                    foreach (long collector_id in retriever.AllIDs)
                    {
                        string sql = $"SELECT CurrentlyBeingCollected FROM Collectors WHERE CollectorID = {collector_id}";
                        try
                        {
                            using (BeingCollected bc = new BeingCollected(collector_id, conn))
                            {
                                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                                using (SQLiteDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                        Assert.Equal(1, reader.GetInt32(0));
                                }
                                throw new Exception($"Exception with collector_id {collector_id}");
                            }
                        }
                        catch (Exception)
                        {
                        }

                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                Assert.Equal(0, reader.GetInt32(0));
                        }
                    }
                }
            }
        }

        [Fact]
        public void CollectNowProperly()
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();
                    Initializer init = new Initializer(null);
                    init.Initialize(db);

                    Tuple<List<DeviceInfo>, DateTimeOffset> devices = LoadDevices(db, conn);
                    DBCollectionTimeRetriever retriever = new DBCollectionTimeRetriever(conn);
                    List<long> collector_ids = retriever.AllIDs;

                    // At first, all of the collectors will have a NULL value, so we need to
                    // get them to a non-NULL value.
                    collector_ids.ForEach(c => new BeingCollected(c, conn).Dispose());

                    // Now lets go through each of them and make sure that when we collect now
                    // the next time changes to NULL, and re-appears 
                    CollectionTime ct = new CollectionTime(conn);
                    foreach(long collector_id in collector_ids)
                    {
                        retriever = new DBCollectionTimeRetriever(conn);
                        Assert.DoesNotContain(collector_id, retriever.AllIDs);

                        CollectorInfo collector_info = ct.CollectNow(collector_id);
                        Assert.Equal(collector_id, collector_info.id);

                        retriever = new DBCollectionTimeRetriever(conn);
                        Assert.Contains(collector_id, retriever.AllIDs);

                        Assert.Null(collector_info.nextCollectionTime);
                    }
                }
            }
        }

        private static Tuple<List<DeviceInfo>, DateTimeOffset> LoadDevices(Database db, SQLiteConnection conn)
        {
            // Insert some Devices and Collectors
            List<EDeviceType> device_types = new List<EDeviceType>( new EDeviceType[] {
                EDeviceType.Server,
                EDeviceType.System,
                EDeviceType.Workstation,
                EDeviceType.Generic,
                EDeviceType.Workstation,
                EDeviceType.Generic,
            });
            List<DeviceInfo> devices = new List<DeviceInfo>();
            DateTimeOffset dt = new DateTimeOffset(2018, 10, 11, 8, 9, 10, TimeSpan.FromHours(-6));
            for(int i = 0; i < device_types.Count; ++i)
            {
                EDeviceType type = device_types[i];
                DeviceInfo di = new DeviceInfo(type) { DID = new DeviceID(i, $"Device{i:00}"), ipAddress = $"11.22.33.{i}" };
                db.AddDevice(di, dt, conn);
                devices.Add(di);
            }

            return Tuple.Create(devices, dt);
        }
    }
}
