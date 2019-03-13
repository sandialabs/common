using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.db.models;

namespace gov.sandia.sld.common.db
{
    public class Database : IDataStore
    {
        public string Filename { get; private set; }
        public string ConnectionString { get { return "Data Source=" + Filename + ";Version=3;"; } }
        public SQLiteConnection Connection { get { return new SQLiteConnection(ConnectionString); } }

        public Database()
        {
            Context config = Context.LoadConfigFromFile();
            Filename = config.DBPath;
        }

        public Database(Context dbconfig)
        {
            Filename = dbconfig.DBPath;
        }

        /// <summary>
        /// This is called as the device is being updated. It checks the device's collectors and makes sure they're updated as well.
        /// </summary>
        /// <param name="device_id"></param>
        /// <param name="device_name"></param>
        /// <param name="original_device_name"></param>
        /// <param name="collectors"></param>
        /// <param name="conn"></param>
        public static void UpdateCollectors(long device_id, string device_name, string original_device_name, List<CollectorInfo> collectors, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            foreach(CollectorInfo collector in collectors)
            {
                Changer changer = null;
                if(collector.id >= 0)
                {
                    changer = new Updater("Collectors", $"CollectorID = {collector.id}", conn);
                }
                else
                {
                    changer = new Inserter("Collectors", conn);
                    changer.Set("DeviceID", device_id);
                    changer.Set("CollectorType", (int)collector.collectorType);
                }

                changer.Set("Name", device_name + "." + collector.collectorType.ToString(), false);
                changer.Set("IsEnabled", collector.isEnabled ? 1 : 0);
                changer.Set("FrequencyInMinutes", collector.frequencyInMinutes);

                // Remove or change the Configuration table settings that hold the database configurations
                if(collector.collectorType == ECollectorType.DatabaseSize)
                {
                    DatabaseType db_type = new DatabaseType(original_device_name);
                    DatabaseConnectionString db_conn_str = new DatabaseConnectionString(original_device_name);

                    if(collector.isEnabled == false)
                    {
                        db_type.Disable(conn);
                        db_conn_str.Disable(conn);
                    }
                    else
                    {
                        // Make sure that if the device name changed, that the original value from the original path gets
                        // moved over to the new path. And if there was no original device name, the clear won't do anything
                        // but the set will add the new one. This would happen if the device's DB collector was disabled, then
                        // enabled.

                        string type = db_type.GetValue(conn);
                        string conn_str = db_conn_str.GetValue(conn);

                        if (original_device_name != device_name ||
                            type == null ||
                            conn_str == null)
                        {
                            db_type.Disable(conn);
                            db_type = new DatabaseType(device_name);
                            db_type.SetValue(type ?? DatabaseType.Default, timestamp, conn);

                            db_conn_str.Disable(conn);
                            db_conn_str = new DatabaseConnectionString(device_name);
                            db_conn_str.SetValue(conn_str ?? DatabaseConnectionString.Default, timestamp, conn);
                        }
                    }
                }

                changer.Execute();
            }
        }

        /// <summary>
        /// Gets the information about the active devices.
        /// </summary>
        /// <returns></returns>
        public List<DeviceInfo> GetDevices(SQLiteConnection conn)
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();
            string sql = "SELECT DeviceID, Name, Type, IPAddress, Username, Password, GroupID FROM Devices WHERE DateDisabled IS NULL;";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

                    while (reader.Read())
                    {
                        DeviceInfo info = new DeviceInfo() { DID = new DeviceID(reader.GetInt64(0), reader.GetString(1)), type = (EDeviceType)reader.GetInt32(2) };
                        if (reader.IsDBNull(3) == false)
                            info.ipAddress = reader.GetString(3);
                        if (reader.IsDBNull(4) == false)
                            info.username = reader.GetString(4);
                        if (reader.IsDBNull(5) == false)
                            info.password = sed.Decrypt(reader.GetString(5));
                        if (reader.IsDBNull(6) == false)
                            info.groupID = reader.GetInt32(6);

                        info.collectors = GetCollectors(info.id, info.type, conn);

                        devices.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDevices[A]: " + sql);
                log.Error(ex);
            }

            return devices;
        }

        /// <summary>
        /// Gets the information about the active devices.
        /// </summary>
        /// <returns></returns>
        public List<DeviceInfo> GetDevices()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    devices = GetDevices(conn);
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDevices[B]");
                log.Error(ex);
            }

            return devices;
        }

        public List<CollectorInfo> GetAllCollectors()
        {
            List<CollectorInfo> collectors = new List<CollectorInfo>();

            try
            {
                string sql = "SELECT DeviceID, Type FROM Devices WHERE DateDisabled IS NULL;";
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            long deviceID = reader.GetInt64(0);
                            EDeviceType deviceType = (EDeviceType)reader.GetInt32(1);

                            collectors.AddRange(GetCollectors(deviceID, deviceType, conn));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetAllCollectors()");
                log.Error(ex);
            }

            return collectors;
        }

        private List<CollectorInfo> GetCollectors(long device_id, EDeviceType d, SQLiteConnection conn)
        {
            List<CollectorInfo> collectors = new List<CollectorInfo>();

            string sql = $"SELECT CollectorID, Name, CollectorType, IsEnabled, FrequencyInMinutes, LastCollectionAttempt, LastCollectedAt, NextCollectionTime, SuccessfullyCollected, CurrentlyBeingCollected FROM Collectors WHERE DeviceID = {device_id};";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    CollectorInfo c = new CollectorInfo((ECollectorType)reader.GetInt32(2));
                    c.DID = new DeviceID(device_id, string.Empty);
                    c.CID = new CollectorID(reader.GetInt64(0), reader.GetString(1));
                    c.isEnabled = reader.GetInt32(3) != 0;
                    c.frequencyInMinutes = reader.GetInt32(4);

                    if(reader.IsDBNull(5) == false)
                        c.lastCollectionAttempt = DateTimeOffset.Parse(reader.GetString(5));
                    if (reader.IsDBNull(6) == false)
                        c.lastCollectedAt = DateTimeOffset.Parse(reader.GetString(6));
                    if (reader.IsDBNull(7) == false)
                        c.nextCollectionTime = DateTimeOffset.Parse(reader.GetString(7));
                    c.successfullyCollected = reader.GetInt32(8) != 0;
                    c.isBeingCollected = reader.GetInt32(9) == 1;

                    collectors.Add(c);
                }
            }

            // Now fill in the collectors that exist, but that aren't in the Collectors table. This will ensure
            // the web page configuration page gets everything, and it can be enabled.
            foreach (ECollectorType type in Enum.GetValues(typeof(ECollectorType)))
            {
                if (type == ECollectorType.Unknown)
                    continue;

                if (d.IsValidCollector(type))
                {
                    CollectorInfo info = collectors.Find(c => c.collectorType == type);
                    if (info == null)
                    {
                        CollectorInfo c = new CollectorInfo(type);
                        c.isEnabled = false;
                        collectors.Add(c);
                    }
                }
            }
            return collectors;
        }

        public FullDeviceStatus GetDeviceStatuses(long device_id)
        {
            FullDeviceStatus statuses = new FullDeviceStatus();
            string sql =
@"
SELECT D.Name, ST.Description, DS.IsAlarm, DS.Message, D.DeviceID 
FROM DeviceStatus AS DS 
INNER JOIN Devices AS D ON DS.DeviceID = D.DeviceID 
INNER JOIN StatusTypes AS ST ON DS.StatusType = ST.StatusType 
WHERE DS.IsValid = 1 AND 
D.DateDisabled IS NULL
";
            if (device_id >= 0)
                sql += $" AND D.DeviceID = {device_id}";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string description = reader.GetString(1);
                            EAlertLevel alert_level = (EAlertLevel)reader.GetInt32(2);
                            string message = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            long db_device_id = reader.GetInt32(4);

                            statuses.AddStatus(name, db_device_id, description, alert_level, message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDeviceStatuses: " + sql);
                log.Error(ex);
            }

            return statuses;
        }

        #region Network
        public List<NetworkStatus> GetNetworkStatuses(IStartEndTime start_end)
        {
            List<NetworkStatus> statuses = new List<NetworkStatus>();
            string sql =
@"
SELECT N.Name, N.SuccessfulPing, N.DateSuccessfulPingOccurred, N.DatePingAttempted, N.IPAddress, D.DeviceID 
FROM NetworkStatus AS N 
LEFT OUTER JOIN Devices AS D ON N.Name = D.Name 
WHERE D.DateDisabled IS NULL
";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTimeOffset? successful_ping = null;
                            string dt_string = reader.GetString(2);
                            if (string.IsNullOrEmpty(dt_string) == false)
                                successful_ping = DateTimeOffset.Parse(dt_string);

                            long device_id = -1;
                            if (reader.IsDBNull(5) == false)
                                device_id = reader.GetInt64(5);

                            statuses.Add(new NetworkStatus()
                            {
                                name = reader.GetString(0),
                                successfulPing = reader.GetInt32(1) != 0,
                                dateSuccessfulPingOccurred = successful_ping,
                                datePingAttempted = DateTimeOffset.Parse(reader.GetString(3)),
                                ipAddress = reader.GetString(4),
                                deviceID = device_id,
                                hasBeenPinged = successful_ping.HasValue,
                            });
                        }
                    }

                    Dictionary<string, NetworkStatus> status_map = new Dictionary<string, NetworkStatus>();
                    statuses.ForEach(ns => status_map[ns.ipAddress.ToString()] = ns);

                    GetPingHistory(status_map, start_end, conn);
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetNetworkStatuses: " + sql);
                log.Error(ex);
            }

            IPAddressComparer comparer = new IPAddressComparer();
            statuses.Sort((a, b) =>
            {
                IPAddress ip_a = IPAddress.Parse(a.ipAddress);
                IPAddress ip_b = IPAddress.Parse(b.ipAddress);
                return comparer.Compare(ip_a, ip_b);
            });

            return statuses;
        }

        private void GetPingHistory(Dictionary<string, NetworkStatus> statuses, IStartEndTime start_end, SQLiteConnection conn)
        {
            if (start_end.Start == null && start_end.End == null)
                return;

            long collector_id = -1;
            string sql = $"SELECT CollectorID FROM Collectors WHERE CollectorType = {(int)ECollectorType.Ping};";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                    collector_id = reader.GetInt64(0);
            }

            if (collector_id >= 0)
            {
                sql = $"SELECT D.Value, D.Timestamp FROM Data D WHERE D.CollectorID = {collector_id}";
                if (start_end.Start != null)
                    sql += $" AND D.TimeStamp >= '{start_end.Start.Value.ToString("o")}'";
                if (start_end.End != null)
                    sql += $" AND D.TimeStamp < '{start_end.End.Value.ToString("o")}'";
                sql += " ORDER BY D.Timestamp ASC;";

                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTimeOffset timestamp = DateTimeOffset.Parse(reader.GetString(1));
                        var value = JsonConvert.DeserializeObject<ListToValue<PingResult>>(reader.GetString(0));

                        foreach (PingResult p in value.Value)
                        {
                            string ip = p.Address.ToString();
                            if (statuses.TryGetValue(ip, out NetworkStatus ns))
                                ns.AddPingAttempt(new PingAttempt() { successful = p.IsPingable, timestamp = timestamp, responseTimeMS = p.AvgTime });
                        }
                    }
                }

                foreach (NetworkStatus ns in statuses.Values)
                    ns.attempts.Sort((a, b) => a.timestamp.CompareTo(b.timestamp));
            }
        }

        #endregion

        private List<string> GetDeviceNames(SQLiteConnection conn)
        {
            List<string> names = new List<string>();

            using (SQLiteCommand command = new SQLiteCommand("SELECT Name FROM Devices WHERE DateDisabled IS NULL;", conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    names.Add(reader.GetString(0));
            }

            return names;
        }

        /// <summary>
        /// Used when a new device is being added to the system
        /// </summary>
        /// <param name="di">The device being added</param>
        /// <param name="conn">The DB connection to use</param>
        public void AddDevice(DeviceInfo di, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            Inserter inserter = new Inserter("Devices", conn);
            inserter.Set("Name", di.name, true);
            inserter.Set("Type", (int)di.type);
            inserter.Set("IPAddress", di.ipAddress, true);
            inserter.Set("Username", di.username, true);
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            inserter.Set("Password", sed.Encrypt(di.password), true);
            inserter.Set("DateActivated", timestamp);

            inserter.Execute();

            long device_id = conn.LastInsertRowId;
            AddCollectors(di, device_id, timestamp, conn);
        }

        /// <summary>
        /// Used when a device is being removed from a syatem. Makes sure everything related to the device
        /// is removed or marked as disabled.
        /// </summary>
        /// <param name="info">The device being removed</param>
        /// <param name="conn">The DB connection to use</param>
        private void RemoveDevice(DeviceInfo info, SQLiteConnection conn)
        {
            // Don't allow the system or servers to be removed
            if (info == null || info.type == EDeviceType.System || info.type == EDeviceType.Server)
                return;

            // And we're keying off of the device's DB-assigned ID, so if we don't have that we need to stop
            if (info.id < 0)
                return;

            string where = $"DeviceID = '{info.id}'";

            Updater invalidate = new Updater("DeviceStatus", where, conn);
            invalidate.Set("IsValid", 0);
            invalidate.Execute();

            Deleter deleter = new Deleter("NetworkStatus", $"IPAddress = '{info.ipAddress}'", conn);
            deleter.Execute();

            Updater update = new Updater("Devices", where, conn);
            update.Set("DateDisabled", DateTimeOffset.Now);
            update.Execute();

            // Make sure a device's database configuration information is also disabled
            DatabaseType dt = new DatabaseType(info.name);
            dt.Disable(conn);
            DatabaseConnectionString dcs = new DatabaseConnectionString(info.name);
            dcs.Disable(conn);
        }

        public DeviceInfo GetDevice(string name, SQLiteConnection conn)
        {
            DeviceInfo di = null;
            string sql = $"SELECT DeviceID, IPAddress, Username, Password, Type FROM Devices WHERE Name = '{name}' AND DateDisabled IS NULL;";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    di = new DeviceInfo() { DID = new DeviceID(reader.GetInt64(0), name), type = (EDeviceType)reader.GetInt32(4) };

                    if(reader.IsDBNull(1) == false)
                        di.ipAddress = reader.GetString(1);
                    if(reader.IsDBNull(2) == false)
                        di.username = reader.GetString(2);
                    if (reader.IsDBNull(3) == false)
                    {
                        SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
                        di.password = sed.Decrypt(reader.GetString(3));
                    }

                    di.collectors = GetCollectors(di.id, di.type, conn);
                }
            }

            return di;
        }

        public List<DeviceInfo> GetDevicesFromType(EDeviceType type, SQLiteConnection conn)
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            string sql = $"SELECT DeviceID, Name, IPAddress, Username, Password FROM Devices WHERE Type == {(int)type} AND DateDisabled IS NULL;";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    long device_id = reader.GetInt64(0);
                    DeviceInfo di = new DeviceInfo() { DID = new DeviceID(device_id, reader.GetString(1)), type = type };

                    if (reader.IsDBNull(2) == false)
                        di.ipAddress = reader.GetString(2);
                    if (reader.IsDBNull(3) == false)
                        di.username = reader.GetString(3);
                    if (reader.IsDBNull(4) == false)
                    {
                        SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
                        di.password = sed.Decrypt(reader.GetString(4));
                    }

                    di.collectors = GetCollectors(device_id, type, conn);

                    devices.Add(di);
                }
            }

            return devices;
        }

        public static void AddCollectors(DeviceInfo di, long device_id, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            foreach(CollectorInfo ci in di.collectors)
            {
                Inserter inserter = new Inserter("Collectors", conn);
                if (ci.id >= 0)
                    inserter.Set("CollectorID", ci.id);
                inserter.Set("Name", di.name + "." + ci.collectorType.ToString(), false);
                inserter.Set("DeviceID", device_id);
                inserter.Set("CollectorType", (int)ci.collectorType);
                inserter.Set("IsEnabled", ci.isEnabled ? 1 : 0);
                inserter.Set("FrequencyInMinutes", ci.frequencyInMinutes);

                inserter.Execute();

                if(ci.collectorType == ECollectorType.DatabaseSize && ci.isEnabled)
                {
                    // Insert placeholders for the DB type and connection string. Let's default to SQL Server
                    DatabaseType dt = new DatabaseType(di.name);
                    dt.SetValue(DatabaseType.Default, timestamp, conn);

                    DatabaseConnectionString dcs = new DatabaseConnectionString(di.name);
                    dcs.SetValue(DatabaseConnectionString.Default, timestamp, conn);
                }
            }
        }

        private static void Add<T>(Dictionary<string, List<T>> data, string key, T t)
        {
            List<T> list = null;
            if (data.ContainsKey(key))
                list = data[key];
            else
            {
                list = new List<T>();
                data[key] = list;
            }

            list.Add(t);
        }

        /// <summary>
        /// Get info about the disks on the specified device.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="daysToRetrieve"></param>
        /// <returns>A map of the drive letter (e.g. "C:", "E:") to a list of DiskUsage objects
        /// representing how much of the disk was in use at the given time</returns>
        public Dictionary<string, List<DiskUsageData>> GetDiskData(long deviceID, IStartEndTime start_end)
        {
            Dictionary<string, List<DiskUsageData>> disk_data = new Dictionary<string, List<DiskUsageData>>();
            List<DeviceData> dataList = GetDeviceData(deviceID, ECollectorType.Disk, start_end);
            foreach (DeviceData data in dataList)
            {
                List<Tuple<string, DiskUsageData>> data_list = DiskUsageData.FromJSON(data.value);
                foreach(Tuple<string, DiskUsageData> d in data_list)
                {
                    if (d != null &&
                        string.IsNullOrEmpty(d.Item1) == false &&
                        d.Item2 != null)
                    {
                        DiskUsageData du = d.Item2;
                        du.collectorID = data.collectorID;
                        du.dataID = data.dataID;
                        du.timeStamp = data.timeStamp;

                        Add(disk_data, d.Item1, du);
                    }
                }
            }

            return disk_data;
        }

        /// <summary>
        /// There was a bug during development of 1.5 where we accidentally stored a memory report as an array of
        /// a single Dictionary<string, string> when in the past we had just stored a single Dictionary<string, string>
        /// This messed things up, but can be easily fixed on the fly.
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="start_end"></param>
        /// <returns></returns>
        public List<DeviceData> GetMemoryData(long deviceID, IStartEndTime start_end)
        {
            List<DeviceData> memory_data = GetDeviceData(deviceID, ECollectorType.Memory, start_end);

            // Use this so we "remember" what the last successful deserialization was and will attempt to use
            // that same one the next time. This is because all of the wrong ones were clustered together and
            // we'll see a performance boost if we don't rely on try/catch for finding the problematic ones.
            bool use_definition1 = true;
            var definition2 = new { Value = new Dictionary<string, string>[] { new Dictionary<string, string>() } };

            foreach (DeviceData d in memory_data)
            {
                if (use_definition1)
                {
                    try
                    {
                        var value = JsonConvert.DeserializeObject<DictValue<string, string>>(d.value);
                        if (value != null)
                        {
                            // Nothing needs to be done...we're good
                        }
                    }
                    catch (Exception)
                    {
                        use_definition1 = false;
                    }
                }

                if (use_definition1 == false)
                {
                    try
                    {
                        var value = JsonConvert.DeserializeAnonymousType(d.value, definition2);
                        if (value != null)
                        {
                            if(value.Value.Length > 0)
                            {
                                d.value = JsonConvert.SerializeObject(new { Value = value.Value[0] });

                                // Fix the value in the database so we can eventually take this
                                // code out. The bug was discovered and fixed in the testbed, so no
                                // actual buggy code was ever deployed.
                                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                                {
                                    conn.Open();
                                    Updater updater = new Updater("Data", $"DataID = {d.dataID}", conn);
                                    updater.Set("Value", d.value, false);
                                    updater.Execute();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        use_definition1 = true;
                    }

                    if (use_definition1)
                    {
                        try
                        {
                            //var value = JsonConvert.DeserializeAnonymousType(d.value, definition1);
                            var value = JsonConvert.DeserializeObject<DictValue<string, string>>(d.value);
                            if (value != null)
                            {
                                // Nothing needs to be done...we're good
                            }
                        }
                        catch (Exception)
                        {
                            use_definition1 = false;
                        }
                    }
                }
            }

            return memory_data;
        }

        public List<DeviceData> GetDeviceData(long deviceID, ECollectorType collectorType, IStartEndTime start_end)
        {
            var deviceDataList = new List<DeviceData>();
            string sql = $"SELECT CollectorID FROM Collectors WHERE DeviceID = {deviceID} AND CollectorType = {(int)collectorType};";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    long collectorID = -1;
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            collectorID = reader.GetInt64(0);
                    }

                    if (collectorID >= 0)
                    {
                        string start_date = string.Empty;
                        string end_date = string.Empty;
                        if (start_end.Start != null)
                            start_date = $" AND D.TimeStamp >= '{start_end.Start.Value.ToString("o")}'";
                        if (start_end.End != null)
                            end_date = $" AND D.TimeStamp < '{start_end.End.Value.ToString("o")}'";
                        sql = $"SELECT D.DataID, D.Value, D.TimeStamp FROM Data D WHERE D.CollectorID = {collectorID} {start_date} {end_date};";

                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                deviceDataList.Add(new DeviceData { dataID = reader.GetInt64(0), collectorID = collectorID, value = reader.GetString(1), timeStamp = DateTimeOffset.Parse(reader.GetString(2)) });
                        }

                        // Instead of doing an ORDER BY clause, it's faster to sort here
                        deviceDataList.Sort((a, b) => a.timeStamp.CompareTo(b.timeStamp));
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDeviceData: " + sql);
                log.Error(ex);
            }
            return deviceDataList;
        }

        public DateTimeOffset? GetLastConfigurationUpdateAttribute(SQLiteConnection conn)
        {
            DateTimeOffset? first_config = null;
            string sql = "SELECT Value FROM Attributes WHERE Path = 'configuration.last_update';";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTimeOffset dt = DateTimeOffset.MinValue;
                        DateTimeOffset.TryParse(reader.GetString(0), out dt);
                        first_config = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetLastConfigurationUpdateAttribute: " + sql);
                log.Error(ex);
            }

            return first_config;
        }

        public DeviceDetails GetDeviceDetails(int deviceID)
        {
            DeviceDetails details = new DeviceDetails() { deviceID = deviceID };

            try
            {
                ValueInfo val = GetMostRecentValueForDevice(deviceID, ECollectorType.Uptime);
                if(val != null && val.IsValid)
                {
                    var v = new { Value = string.Empty };
                    var value = JsonConvert.DeserializeAnonymousType(val.value, v);
                    details.uptime = value.Value;
                }

                val = GetMostRecentValueForDevice(deviceID, ECollectorType.LastBootTime);
                if (val != null && val.IsValid)
                {
                    var v = new { Value = string.Empty };
                    var value = JsonConvert.DeserializeAnonymousType(val.value, v);
  
                    DateTimeOffset dt = DateTimeOffset.MinValue;
                    if(DateTimeOffset.TryParse(value.Value, out dt))
                        details.lastBootTime = dt;
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDeviceDetails: ");
                log.Error(ex);
            }

            return details;
        }

        public DeviceErrors GetSystemErrors(int deviceID, IStartEndTime start_end)
        {
            return GetErrors(deviceID, start_end, ECollectorType.SystemErrors);
        }

        public DeviceErrors GetApplicationErrors(int deviceID, IStartEndTime start_end)
        {
            return GetErrors(deviceID, start_end, ECollectorType.ApplicationErrors);
        }

        private DeviceErrors GetErrors(int deviceID, IStartEndTime start_end, ECollectorType type)
        {
            DeviceErrors errors = new DeviceErrors() { deviceID = deviceID };
            string sql = string.Empty;

            try
            {
                string start_date = string.Empty;
                string end_date = string.Empty;
                if (start_end.Start != null)
                    start_date = $" AND D.TimeStamp >= '{start_end.Start.Value.ToString("o")}'";
                if (start_end.End != null)
                    end_date = $" AND D.TimeStamp < '{start_end.End.Value.ToString("o")}'";

                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    sql = $"SELECT D.Value FROM Collectors AS C INNER JOIN Data AS D ON C.CollectorID = D.CollectorID WHERE C.DeviceID = {deviceID} AND C.CollectorType = {(int)type} {start_date} {end_date} ORDER BY D.TimeStamp DESC;";

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        Dictionary<string, ErrorInfo> error_dict = new Dictionary<string, ErrorInfo>();

                        while (reader.Read())
                        {
                            var val = JsonConvert.DeserializeObject<ValueClass<ErrorStruct>>(reader.GetString(0));

                            string message = val.Value.Message;
                            DateTimeOffset timestamp = val.Value.TimeGenerated;

                            ErrorInfo existing = null;
                            if (error_dict.TryGetValue(message, out existing))
                                existing.IncrementCount(timestamp);
                            else
                            {
                                ErrorInfo error = new ErrorInfo() { message = message, timestamp = timestamp };
                                error_dict[message] = error;
                                errors.errors.Add(error);
                            }
                        }
                    }
                }

                errors.errors.Sort((a, b) => b.timestamp.CompareTo(a.timestamp));
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetErrors: " + sql);
                log.Error(ex);
            }

            return errors;
        }

        public List<HardDisk> GetSMARTData(int deviceID)
        {
            List<HardDisk> smart = new List<HardDisk>();

            try
            {
                ValueInfo val = GetMostRecentValueForDevice(deviceID, ECollectorType.SMART);
                if (val != null && val.IsValid)
                {
                    var definition = new { Value = new List<HardDisk>() };
                    var value = JsonConvert.DeserializeAnonymousType(val.value, definition);
                    smart.AddRange(value.Value);
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetSMARTData: ");
                log.Error(ex);
            }

            return smart;
        }

        /// <summary>
        /// Gets all of a device's processes over the specified number of days
        /// 
        /// Is used in the process report.
        /// 
        /// Performance isn't great, but it's all bunched into two things: GetDeviceData and
        /// JsonConvert.DeserializeAnonymousType. Turns out DeserializeObject<...> is about
        /// twice as fast as DeserializeAnonymousType so let's switch to that one.
        /// </summary>
        /// <param name="deviceID">The device of interest</param>
        /// <param name="start_end">The date range to get</param>
        /// <returns>A set of strings of the processes</returns>
        public List<string> GetAllDeviceProcesses(int deviceID, IStartEndTime start_end)
        {
            // First, we need to get all of the processes in the date range...
            List<DeviceData> data = GetDeviceData(deviceID, ECollectorType.Processes, start_end);
            HashSet<string> processes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            //Stopwatch watch = Stopwatch.StartNew();

            // Then, for each of those, deserialize to the proper data structure. In this case it's a dictionary<string...>,
            // with the process names as the keys in the dictionary. We put the keys of each dictionary into a HashSet, which
            // will be all of the processes in the date range.
            foreach (DeviceData pData in data)
            {
                try
                {
                    // The data in Value is an array of pairs mapping the process name to its CPU/Memory usage.
                    // In this method we just want the process name so we pull those out and ignore the CPU/Memory
                    //var value = JsonConvert.DeserializeAnonymousType(pData.value, definition);
                    DictToDictValue<string, string, string> value = JsonConvert.DeserializeObject<DictToDictValue<string, string, string>>(pData.value);
                    if(value != null)
                        processes.IntersectWith(value.Value.Keys);
                }
                catch (Exception)
                {
                }
            }
            //watch.Stop();

            List<string> p = processes.ToList();
            p.Sort();
            return p;
        }

        public DeviceApplications GetDeviceApplications(int deviceID)
        {
            DeviceApplications apps = new DeviceApplications() { deviceID = deviceID };

            try
            {
                ValueInfo val = GetMostRecentValueForDevice(deviceID, ECollectorType.InstalledApplications);
                if(val != null && val.IsValid)
                {
                    var definition = new { Value = new List<models.ApplicationInfo>() };
                    var value = JsonConvert.DeserializeAnonymousType(val.value, definition);
                    apps.applications = value.Value.ConvertAll<DeviceApplications.ApplicationInfo>(i => i.AsDeviceApplicationsApplicationInfo());
                    apps.timestamp = val.timestamp;
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDeviceApplications: ");
                log.Error(ex);
            }

            return apps;
        }

        public AllApplicationsHistory GetApplicationChanges(int deviceID, IStartEndTime start_end)
        {
            AllApplicationsHistory history = new AllApplicationsHistory();
            try
            {
                List<DeviceData> data = GetDeviceData(deviceID, ECollectorType.InstalledApplications, start_end);
                Dictionary<string, string> previous_apps = null;

                foreach (DeviceData d in data)
                {
                    if (d == null)
                        continue;

                    string value_str = d.value;
                    var value = JsonConvert.DeserializeObject<ListToValue<models.ApplicationInfo>>(value_str);
                    List<DeviceApplications.ApplicationInfo> apps = value.Value.ConvertAll<DeviceApplications.ApplicationInfo>(i => new DeviceApplications.ApplicationInfo() { name = i.Name, version = i.Version });

                    // Sometimes it looks like when we're messing around with configuration, no apps are recorded from time to time.
                    // Let's assume that if there are none recorded, then we should just skip it and don't assume everything was uninstalled.
                    if (apps.Count == 0)
                        continue;

                    // Record all the current versions
                    Dictionary<string, string> current_apps = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    apps.ForEach(a => current_apps[a.name] = a.version);

                    if(previous_apps != null)
                    {
                        // Go through all the previous ones, and compare the versions to the current ones.
                        // We'll also see if any previous have disappeared, or if any current have appeared
                        // that aren't in the previous.
                        List<string> previous = new List<string>();
                        foreach(var p in previous_apps)
                        {
                            previous.Add(p.Key);

                            string current_version = null;
                            if(current_apps.TryGetValue(p.Key, out current_version) == false)
                            {
                                // It must have been removed between the previous collection and now.
                                // Insert history as a null version
                                history.AddHistory(p.Key, null, d.timeStamp);
                            }
                            else if(p.Value != current_version)
                            {
                                // It has changed. Record the new version.
                                history.AddHistory(p.Key, current_version, d.timeStamp);
                            }
                            // else, it didn't change versions. Don't record anything.
                        }

                        // Now create a hash set of the current keys, then remove the previous keys. Whatever is left must have been
                        // installed between the previous collection and the current collection.
                        HashSet<string> current = new HashSet<string>(current_apps.Keys.ToArray(), StringComparer.InvariantCultureIgnoreCase);
                        List<string> diff = new List<string>(current.Except(previous));
                        diff.ForEach(a => history.AddHistory(a, current_apps[a], d.timeStamp));
                    }
                    else
                    {
                        // The initial apps--put them all in
                        foreach(KeyValuePair<string, string> app in current_apps)
                            history.AddHistory(app.Key, app.Value, d.timeStamp);
                    }

                    previous_apps = current_apps;
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDeviceApplications: ");
                log.Error(ex);
            }

            return history;
        }

        public ApplicationHistory GetApplicationHistory(int deviceID, string app)
        {
            ApplicationHistory history = new ApplicationHistory(app);

            try
            {
                List<DeviceData> data = GetDeviceData(deviceID, ECollectorType.InstalledApplications, new StartStopTime());
                string version = string.Empty;

                foreach (DeviceData d in data)
                {
                    if (d == null)
                        continue;

                    var value = JsonConvert.DeserializeObject<ListToValue<data.wmi.ApplicationInfo>>(d.value);

                    // Sometimes it looks like when we're messing around with configuration, no apps are recorded from time to time.
                    // Let's assume that if there are none recorded, then we should just skip it and don't assume everything was uninstalled.
                    if (value.Value.Count == 0)
                        continue;

                    data.wmi.ApplicationInfo app2 = value.Value.FirstOrDefault(a => string.Compare(app, a.Name, true) == 0);

                    if (app2 != null)
                    {
                        if (string.Compare(version, app2.Version, true) != 0)
                        {
                            history.history.Add(new ApplicationHistory.Snapshot() { timestamp = d.timeStamp, version = app2.Version });
                            version = app2.Version;
                        }
                    }
                    // Don't record 'Removed' as the first entry
                    else if(history.history.Count > 0)
                    {
                        history.history.Add(new ApplicationHistory.Snapshot() { timestamp = d.timeStamp, version = "Removed" });
                        version = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetApplicationHistory: ");
                log.Error(ex);
            }

            return history;
        }

        public models.UPSStatus GetUPSStatus(int deviceID)
        {
            models.UPSStatus ups = new models.UPSStatus();

            try
            {
                ValueInfo val = GetMostRecentValueForDevice(deviceID, ECollectorType.UPS);
                if (val != null && val.IsValid)
                {
                    var v = new { Value = new { BatteryStatus = string.Empty, EstimatedChargeRemaining = 0, EstimatedRunTime = 0, Name = string.Empty, Status = string.Empty } };
                    var value = JsonConvert.DeserializeAnonymousType(val.value, v);
                    ups.deviceID = deviceID;
                    ups.timestamp = val.timestamp;
                    ups.batteryStatus = value.Value.BatteryStatus;
                    ups.estimatedChargeRemainingPercentage = value.Value.EstimatedChargeRemaining;
                    ups.estimatedRunTimeInMinutes = value.Value.EstimatedRunTime;
                    ups.name = value.Value.Name;
                    ups.upsStatus = value.Value.Status;
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetUPSStatus: ");
                log.Error(ex);
            }

            return ups;
        }


        public ValueInfo GetMostRecentValueForDevice(int deviceID, ECollectorType type)
        {
            ValueInfo value = null;
            string sql = $"SELECT D.Value, D.TimeStamp FROM Collectors AS C INNER JOIN MostRecentDataPerCollector MR ON C.CollectorID = MR.CollectorID INNER JOIN Data AS D ON MR.DataID = D.DataID WHERE C.DeviceID = {deviceID} AND C.CollectorType = {(int)type};";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string v = reader.GetString(0);
                            string timestamp = reader.GetString(1);
                            value = new ValueInfo() { deviceID = deviceID, collectorType = type, value = v, timestamp = DateTimeOffset.Parse(timestamp) };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetMostRecentValueForDevice: " + sql);
                log.Error(ex);
            }

            return value;
        }

        public DeviceProcessInfo GetDeviceProcesses(int deviceID)
        {
            DeviceProcessInfo info = new DeviceProcessInfo() { deviceID = deviceID };
            ValueInfo value_info = GetMostRecentValueForDevice(deviceID, ECollectorType.Processes);
            if(value_info != null)
            {
                info.timestamp = value_info.timestamp;

                ProcessInfoBuilder builder = new ProcessInfoBuilder();
                builder.Build(value_info.value);

                foreach (KeyValuePair<string, gov.sandia.sld.common.data.wmi.Process> process in builder.Processes)
                    info.Add(process.Key, process.Value.CPUNum);
            }

            info.Sort();

            return info;
        }

        /// <summary>
        /// Get the history for a specified process on the specified device
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="processName"></param>
        /// <returns></returns>
        public ProcessHistory GetProcessHistory(int deviceID, string processName)
        {
            ProcessHistory history = new ProcessHistory() { deviceID = deviceID, processName = processName };
            string sql = string.Empty;

            try
            {
                // Turns out doing two queries is MUCH, MUCH faster (like 100x faster) than the above query
                sql = $"SELECT CollectorID FROM Collectors WHERE DeviceID = {deviceID} AND CollectorType = {(int)ECollectorType.Processes}";

                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    int collectorID = -1;
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            collectorID = reader.GetInt32(0);
                    }

                    if(collectorID >= 0)
                    {
                        // We want to use ORDER BY ... DESC here so we get the most recent ones, not the earliest ones
                        sql = $"SELECT Value, TimeStamp FROM Data WHERE CollectorID = {collectorID} ORDER BY TimeStamp DESC LIMIT 10000;";

                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            ProcessInfoBuilder builder = new ProcessInfoBuilder();

                            while (reader.Read())
                            {
                                string d_value = reader.GetString(0);
                                DateTimeOffset timestamp = DateTimeOffset.Parse(reader.GetString(1));

                                builder.Build(d_value);

                                if (builder.Processes.TryGetValue(processName, out data.wmi.Process process_data))
                                    history.AddData(timestamp, process_data.CPUNum, process_data.MemoryNum);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetProcessHistory: " + sql);
                log.Error(ex);
            }

            // The query returns the timestamps in reverse order, so let's invert the list here so they'll be in time order.
            history.details.Reverse();

            return history;
        }

        public DatabaseHistory GetDatabaseHistory(int deviceID, string databaseName)
        {
            DatabaseHistory history = new DatabaseHistory() { deviceID = deviceID, databaseName = databaseName };
            string sql = $"SELECT D.Value, D.TimeStamp FROM Collectors AS C INNER JOIN Data AS D ON C.CollectorID = D.CollectorID WHERE C.DeviceID = {deviceID} AND C.CollectorType = {(int)ECollectorType.DatabaseSize} ORDER BY D.TimeStamp DESC LIMIT 1000;";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string d_value = reader.GetString(0);
                            DateTimeOffset timestamp = DateTimeOffset.Parse(reader.GetString(1));
                            var value = JsonConvert.DeserializeObject<ListToValue<Dictionary<string, string>>>(d_value);
                            foreach (Dictionary<string, string> dict in value.Value)
                            {
                                if (dict.ContainsKey("Name") && dict["Name"].ToLower() == databaseName.ToLower())
                                {
                                    int size;
                                    if (int.TryParse(dict["Size"], out size))
                                        history.details.Add(new DatabaseDetail() { sizeInMB = size, timestamp = timestamp });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Database));
                log.Error("GetDatabaseHistory: " + sql);
                log.Error(ex);
            }

            // The query returns the timestamps in reverse order, so let's invert the list here so they'll be in time order.
            history.details.Reverse();

            return history;
        }

        #region HelperClasses

        private class ErrorStruct
        {
            public string Message { get; set; }
            public DateTimeOffset TimeGenerated { get; set; }
            public string Source { get; set; }
            public string RecordNumber { get; set; }
            public string EventCode { get; set; }
        }

        private class ValueClass<T>
        {
            public T Value { get; set; }
        }

        private class DictValue<T, U>
        {
            public Dictionary<T, U> Value { get; set; }
        }

        private class DictToDictValue<T, U, V>
        {
            public Dictionary<T, Dictionary<U, V>> Value { get; set; }
        }

        private class ListToValue<T>
        {
            public List<T> Value { get; set; }
        }

        #endregion
    }
}
