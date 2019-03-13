using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace gov.sandia.sld.common.db
{
    public static class SystemConfigurationStore
    {
        public static void Set(SystemConfiguration config, DateTimeOffset timestamp, Database db)
        {
            try
            {
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    HandleConfiguration(config.configuration, timestamp, conn);
                    HandleDevices(config.devices, timestamp, conn);
                    HandleGroups(config.groups, conn);
                    StoreMonitoredDrives(config.devices, conn);

                    Attribute attr = new Attribute();
                    attr.Set("configuration.last_update", timestamp.ToString("o"), conn);
                }

                Obfuscate(config);
                SaveConfiguration(config, db);

                // When the configuration changes, we want to do an immediate ping so we can determine if any
                // new devices are online/offline
                CollectPingNow(db);
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(SystemConfigurationStore));
                log.Error("SetSystemConfiguration");
                log.Error(ex);
            }
        }

        private static void CollectPingNow(Database db)
        {
            int ping_id = -1;
            string sql = $"SELECT CollectorID FROM Collectors WHERE CollectorType = {(int)ECollectorType.Ping};";

            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();

                try
                {
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            ping_id = reader.GetInt32(0);
                    }
                }
                catch (Exception ex)
                {
                    ILog log = LogManager.GetLogger(typeof(SystemConfigurationStore));
                    log.Error("Initialize: " + sql);
                    log.Error(ex);
                }

                if (ping_id >= 0)
                {
                    CollectionTime ct = new CollectionTime(conn);
                    ct.CollectNow(ping_id);
                }
            }
        }

        private static void SaveConfiguration(SystemConfiguration config, Database db)
        {
            int system_configuration_id = -1;
            string sql = "SELECT CollectorID FROM Collectors WHERE Name = 'System.Configuration' AND IsEnabled = 1;";

            try
            {
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            system_configuration_id = reader.GetInt32(0);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(SystemConfigurationStore));
                log.Error("Initialize: " + sql);
                log.Error(ex);
            }

            if (system_configuration_id > 0)
            {
                DataCollectorContext dcc = new DataCollectorContext(new CollectorID(system_configuration_id, "System.Configuration"), ECollectorType.Configuration);
                Data d = new Data(dcc)
                {
                    Value = JsonConvert.SerializeObject(config),
                };
                DataStorage ds = new DataStorage();
                ds.SaveData(new CollectedData(dcc, true, d), db);
            }
            else
            {
                ILog log = LogManager.GetLogger(typeof(SystemConfigurationStore));
                log.Error("Error obtaining ID for System.Configuration collector");
            }
        }

        private static void Obfuscate(SystemConfiguration config)
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            // Before we write the configuration data to the database, let's make sure we obfuscate
            // the appropriate things. That would be the database connection strings, and the
            // device's passwords.
            foreach (ConfigurationData cd in config.configuration.Values)
            {
                try
                {
                    if (cd.path.EndsWith("connection_string", StringComparison.InvariantCultureIgnoreCase))
                        cd.value = sed.Encrypt(cd.value);
                }
                catch (Exception)
                {
                }
            }
            foreach (DeviceInfo di in config.devices)
            {
                try
                {
                    di.password = sed.Encrypt(di.password);
                }
                catch (Exception)
                {
                }
            }
        }

        public static SystemConfiguration Get(bool obfuscate, SQLiteConnection conn)
        {
            SystemConfiguration config = new SystemConfiguration();
            string sql = string.Empty;
            ILog log = LogManager.GetLogger(typeof(SystemConfigurationStore));

            try
            {
                Dictionary<string, string> monitoredDrives = null;
                SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

                sql = "SELECT ConfigurationID, Path, Value FROM Configuration WHERE IsValid = 1;";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string path = reader.GetString(1);
                        string value = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        if (path.EndsWith("connection_string", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (obfuscate)
                                value = "*****";
                            else
                                value = sed.Decrypt(value);
                        }

                        config.configuration[path] = new ConfigurationData()
                        {
                            configID = reader.GetInt32(0),
                            path = path,
                            value = value
                        };

                        if (string.Compare(path, "languages", true) == 0)
                        {
                            Languages lang = new Languages();
                            lang.EnableFromDelimitedString(value);
                            foreach (Language l in lang.All)
                                config.languages.Add(new LanguageConfiguration() { languageCode = l.GetDescription(), language = l.ToString(), isEnabled = lang.IsEnabled(l) });
                        }
                    }
                }

                Attribute attr = new Attribute();
                config.softwareVersion = attr.Get("software.version", conn);
                if(config.softwareVersion == null)
                    config.softwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                monitoredDrives = attr.GetMultiple("all.drives.descriptions", conn);
                List<string> device_names = monitoredDrives.Keys.ToList();
                foreach (string name in device_names)
                {
                    string[] n = name.Split('.');
                    if (n.Length > 0)
                        monitoredDrives.ChangeKey(name, n[0]);
                }

                sql = "SELECT GroupID, Name FROM DeviceGroups";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Group g = new Group() { id = reader.GetInt32(0), name = reader.GetString(1) };
                        config.groups.Add(g);
                    }
                }

                config.devices = new Database().GetDevices(conn);

                if (monitoredDrives != null)
                {
                    foreach (DeviceInfo dev in config.devices)
                    {
                        string drive_info;
                        if (monitoredDrives.TryGetValue(dev.name, out drive_info))
                        {
                            try
                            {
                                dev.driveNames = JsonConvert.DeserializeObject<Dictionary<string, string>>(drive_info);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                LoadMonitoredDrives(config.devices, conn);

                // Get the most recent data that has been recorded
                sql = "SELECT Timestamp FROM Data ORDER BY Timestamp DESC LIMIT 1;";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        try
                        {
                            DateTimeOffset ts = DateTimeOffset.Parse(reader.GetString(0));
                            config.mostRecentData = ts;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetSystemConfiguration: " + sql);
                log.Error(ex);
            }

            log.Debug("Configuration: \n" + JsonConvert.SerializeObject(config.devices, Formatting.Indented));

            return config;
        }

        private static void HandleDevices(List<DeviceInfo> devices, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            foreach (DeviceInfo device in devices)
            {
                long id = device.id;
                if(id < 0)
                {
                    try
                    {
                        string sql = $"SELECT DeviceID FROM Devices WHERE Name = '{device.name}';";
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read() && reader.IsDBNull(0) == false)
                                id = reader.GetInt64(0);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (id >= 0)
                    UpdateDevice(device, timestamp, conn);
                else
                    // It's a new device
                    AddDevice(device, timestamp, conn);
            }
        }

        /// <summary>
        /// Used when a new device is being added to the system
        /// </summary>
        /// <param name="di">The device being added</param>
        /// <param name="conn">The DB connection to use</param>
        private static void AddDevice(DeviceInfo di, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            Inserter inserter = new Inserter("Devices", conn);
            if (di.id >= 0)
                inserter.Set("DeviceID", di.id);
            inserter.Set("Name", di.name, true);
            inserter.Set("Type", (int)di.type);
            inserter.Set("IPAddress", di.ipAddress, true);
            inserter.Set("Username", di.username, true);
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            inserter.Set("Password", sed.Encrypt(di.password), true);
            inserter.Set("DateActivated", timestamp);
            if (di.groupID < 0)
                inserter.SetNull("GroupID");
            else
                inserter.Set("GroupID", di.groupID);

            inserter.Execute();

            long device_id = conn.LastInsertRowId;
            Database.AddCollectors(di, device_id, timestamp, conn);
        }

        private static void UpdateDevice(DeviceInfo device, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            if (device.deleted)
            {
                RemoveDevice(device, conn);
            }
            else
            {
                // Grab the original device name
                string original_device_name = string.Empty;
                string original_ip_address = string.Empty;
                bool do_insert = false;
                string sql = string.Format("SELECT Name, IPAddress FROM Devices WHERE DeviceID = {0}", device.id);
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.IsDBNull(0) == false)
                            original_device_name = reader.GetString(0);

                        if (reader.IsDBNull(1) == false)
                        {
                            original_ip_address = reader.GetString(1);

                            // If the IP address hasn't changed, no need to remove it from the NetworkStatus
                            if (original_ip_address == device.ipAddress)
                                original_ip_address = string.Empty;
                        }
                    }
                    else
                        do_insert = true;
                }

                if (do_insert == false)
                {
                    // Just update everything
                    SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
                    Updater update = new Updater("Devices", string.Format("DeviceID = {0}", device.id), conn);
                    update.Set("Name", device.name, true);
                    update.Set("IPAddress", device.ipAddress, true);
                    update.Set("Username", device.username, true);
                    update.Set("Password", sed.Encrypt(device.password), false);
                    if (device.groupID < 0)
                        update.SetNull("GroupID");
                    else
                        update.Set("GroupID", device.groupID);
                    update.Execute();

                    Database.UpdateCollectors(device.id, device.name, original_device_name, device.collectors, timestamp, conn);
                }
                else
                {
                    // The device didn't exist, so just add it.
                    AddDevice(device, timestamp, conn);
                }

                if (string.IsNullOrEmpty(original_device_name) == false)
                {
                    // Make sure if the name changed that the NetworkStatus table is updated properly
                    Updater network_updater = new Updater("NetworkStatus", string.Format("Name = '{0}'", original_device_name), conn);
                    network_updater.Set("Name", device.name, false);
                    network_updater.Execute();
                }

                if (string.IsNullOrEmpty(original_ip_address) == false)
                {
                    // In case the IP address was changed, delete the original IP address and let the new one fill it in.
                    Deleter deleter = new Deleter("NetworkStatus", string.Format("IPAddress = '{0}'", original_ip_address), conn);
                    deleter.Execute();
                }
            }
        }

        /// <summary>
        /// Used when a device is being removed from a syatem. Makes sure everything related to the device
        /// is removed or marked as disabled.
        /// </summary>
        /// <param name="info">The device being removed</param>
        /// <param name="conn">The DB connection to use</param>
        private static void RemoveDevice(DeviceInfo info, SQLiteConnection conn)
        {
            // Don't allow the system or servers to be removed
            if (info == null || info.type == EDeviceType.System || info.type == EDeviceType.Server)
                return;

            // And we're keying off of the device's DB-assigned ID, so if we don't have that we need to stop
            if (info.id < 0)
                return;

            string where = string.Format("DeviceID = {0}", info.id);

            Updater invalidate = new Updater("DeviceStatus", where, conn);
            invalidate.Set("IsValid", 0);
            invalidate.Execute();

            Deleter deleter = new Deleter("NetworkStatus", string.Format("IPAddress = '{0}'", info.ipAddress), conn);
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

        private static void HandleConfiguration(Dictionary<string, ConfigurationData> configuration, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            foreach (KeyValuePair<string, ConfigurationData> kvp in configuration)
            {
                ConfigurationData config_data = kvp.Value;

                if (config_data.path.EndsWith("connection_string", StringComparison.InvariantCultureIgnoreCase))
                    config_data.value = sed.Encrypt(config_data.value);

                Configuration.SetValue(config_data.path, config_data.value, true, timestamp, conn);
            }
        }

        private static void UpdateConfiguration(ConfigurationData config_data, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            if (config_data.deleted)
            {
                Configuration.Clear(config_data.path, conn);
            }
            else
            {
                if (config_data.path.EndsWith("connection_string", StringComparison.InvariantCultureIgnoreCase))
                {
                    SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
                    config_data.value = sed.Encrypt(config_data.value);
                }

                Configuration.SetValue(config_data.path, config_data.value, true, timestamp, conn);
            }
        }

        private static void HandleGroups(List<Group> groups, SQLiteConnection conn)
        {
            foreach (Group g in groups)
            {
                string sql = string.Format("SELECT GroupID, Name FROM DeviceGroups WHERE GroupID = {0}", g.id);
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.GetString(1) != g.name)
                        {
                            Updater updater = new Updater("DeviceGroups", string.Format("GroupID = {0}", g.id), conn);
                            updater.Set("Name", g.name.Trim(), true);
                            updater.Execute();
                        }
                    }
                    else
                    {
                        Inserter insert = new Inserter("DeviceGroups", conn);
                        insert.Set("GroupID", g.id);
                        insert.Set("Name", g.name.Trim(), true);
                        insert.Execute();
                    }
                }
            }
        }

        private static void StoreMonitoredDrives(List<DeviceInfo> devices, SQLiteConnection conn)
        {
            foreach(DeviceInfo device in devices)
            {
                bool to_delete = device.monitoredDrives.driveMap.Count == 0;
                string path = device.name + ".monitored_drives";

                if (device.deleted || to_delete)
                    Configuration.Clear(path, conn);
                else
                    Configuration.SetValue(path, JsonConvert.SerializeObject(device.monitoredDrives, Newtonsoft.Json.Formatting.None,
                        new Newtonsoft.Json.JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }), false, DateTimeOffset.Now, conn);
            }
        }

        private static void LoadMonitoredDrives(List<DeviceInfo> devices, SQLiteConnection conn)
        {
            foreach(DeviceInfo device in devices)
            {
                string path = device.name + ".monitored_drives";
                string value = Configuration.GetValue(path, conn);
                if(string.IsNullOrEmpty(value) == false)
                {
                    try
                    {
                        MonitoredDriveManager manager = JsonConvert.DeserializeObject<MonitoredDriveManager>(value);
                        if(manager != null)
                            device.monitoredDrives = manager;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
