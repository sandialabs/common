using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace gov.sandia.sld.common.db
{
    public class DataStorage
    {
        public DataStorage()
        {
            m_interpreters = new List<IDataInterpreter>();
        }

        public void AddInterpreter(IDataInterpreter i)
        {
            m_interpreters.Add(i);
        }

        public void RemoveInterpreter(IDataInterpreter i)
        {
            m_interpreters.Remove(i);
        }

        public class DataRecord
        {
            public DataCollectorContext DCContext { get; set; }
            public string Value { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public Data D { get; set; }

            public DataRecord(DataCollectorContext context, Data d)
            {
                DCContext = context;
                Value = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.None,
                    new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                Timestamp = d.CollectedAt;

                D = d;
            }

            public DataRecord(DataCollectorContext context, string value, DateTimeOffset timestamp)
            {
                DCContext = context;
                Value = value;
                Timestamp = timestamp;

                D = DataFactory.Create(context, value);
            }

            public long InsertData(SQLiteConnection conn)
            {
                Inserter insert = new Inserter("Data", conn);
                insert.Set("CollectorID", DCContext.ID.ID);
                insert.Set("Value", Value, false);
                insert.Set("TimeStamp", Timestamp);
                insert.Execute();

                UpdateMostRecentData(conn.LastInsertRowId, conn);

                return conn.LastInsertRowId;
            }

            private void UpdateMostRecentData(long data_id, SQLiteConnection conn)
            {
                Changer change = new Deleter("MostRecentDataPerCollector", $"CollectorID = {DCContext.ID.ID}", conn);
                change.Execute();

                change = new Inserter("MostRecentDataPerCollector", conn);
                change.Set("CollectorID", DCContext.ID.ID);
                change.Set("DataID", data_id);
                change.Execute();
            }
        }

        public bool SaveData(CollectedData data)
        {
            Database db = new Database();
            return SaveData(data, db);
        }

        /// <summary>
        /// Used to store a chunk of collected Data. Determines which Collector collected
        /// the data based on d.Name, then inserts it into the Data table, and updates
        /// the MostRecentDataPerCollector table. It then sends the Data off to each
        /// of the interpreters so they can interpret the new data however they wish.
        /// </summary>
        /// <param name="data">A collection of Data objects to store in the DB</param>
        /// <param name="db">The database to store the data in</param>
        /// <returns></returns>
        public bool SaveData(CollectedData data, Database db)
        {
            if (data == null)
                return false;

            bool success = false;
            ILog log = LogManager.GetLogger(typeof(DataStorage));
            //ApplicationEventLog elog = new ApplicationEventLog();

            try
            {
                //string msg = $"SaveData -- data is collected: {data.DataIsCollected}, with {data.D.Count} items";
                //log.Debug(msg);
                //elog.LogInformation(msg);

                // Record the last time a collection attempt was made for this collector, regardless of whether
                // it was successfully collected. If it wasn't collected, we'll bail out shortly.
                if (data.Context.ID.ID >= 0)
                {
                    using (SQLiteConnection conn = db.Connection)
                    {
                        conn.Open();

                        Updater updater = new Updater("Collectors", $"CollectorID = {data.Context.ID.ID}", conn);
                        if (data.DataIsCollected)
                            updater.Set("LastCollectedAt", DateTimeOffset.Now);
                        updater.Set("SuccessfullyCollected", data.DataIsCollected);
                        updater.Execute();
                    }
                }

                if (data.DataIsCollected == false)
                    return true;

                Stopwatch watch = Stopwatch.StartNew();

                List<DataRecord> to_insert = new List<DataRecord>();
                foreach(Data d in data.D)
                {
                    try
                    {
                        DataCollectorContext dc_context = d.Context;
                        if (dc_context.ID.ID >= 0)
                        {
                            DataRecord dr = new DataRecord(dc_context, d);
                            to_insert.Add(dr);
                        }
                        else
                        {
                            log.Error($"Unknown Data ({d.ID}, {d.Name})");
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("SaveData[A]: ");
                        log.Error(e);
                    }
                }

                success = Insert(to_insert, db);

                log.Debug($"Total insertion took {watch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                log.Error("SaveData[B]: ");
                log.Error(e);
            }

            return success;
        }

        public bool Insert(DataRecord record, Database db)
        {
            return Insert(new DataStorage.DataRecord[] { record }, db);
        }

        public bool Insert(IEnumerable<DataRecord> records, Database db)
        {
            foreach (DataRecord dr in records)
            {
                try
                {
                    using (SQLiteConnection conn = db.Connection)
                    {
                        conn.Open();
                        Insert(dr, conn);
                    }
                }
                catch (Exception e)
                {
                    ApplicationEventLog log = new ApplicationEventLog();
                    log.LogError("DataStorage.Insert error");
                    log.Log(e);
                }
            }

            return true;
        }

        public long Insert(DataRecord dr, SQLiteConnection conn)
        {
            long data_id = dr.InsertData(conn);
            if (dr.D != null)
            {
                m_interpreters.ForEach(i =>
                {
                    try
                    {
                        i.Interpret(dr.D, conn);
                    }
                    catch (Exception e)
                    {
                        ApplicationEventLog log = new ApplicationEventLog();
                        log.LogError($"DataStorage.Insert: interpretation error ({dr.D.Name})");
                        log.Log(e);
                    }
                });
            }
            return data_id;
        }

        // List of the different interpreters that the Data objects are passed to.
        private List<IDataInterpreter> m_interpreters;
    }
}
