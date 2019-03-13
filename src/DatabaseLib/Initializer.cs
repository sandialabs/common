using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Called to make sure the database structure is set up properly. Should only be called once, probably
    /// by the service, as the service starts. Makes sure the *Types tables are populated with any
    /// new types that come along, and makes sure new columns/indexes/etc. are added.
    /// </summary>
    public class Initializer
    {
        public enum EOptions
        {
            SkipSystemCreation
        }

        public List<EOptions> Options { get; private set; }

        public Initializer(IEnumerable<EOptions> options)
        {
            if (options != null)
                Options = new List<EOptions>(options);
            else
                Options = new List<EOptions>();
        }

        public void Initialize(Database db)
        {
            ILog log = LogManager.GetLogger(typeof(Initializer));
            log.Debug("Initializing DB");

            try
            {
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    CreateTables(conn);
                    InitializeTypeTables(conn);
                    AddMissingColumns(conn);

                    // Don't create the indices until after the missing columns have been added since
                    // some of those new columns want indices.
                    CreateIndices(conn);

                    if (Options.Contains(EOptions.SkipSystemCreation) == false)
                    {
                        // Make sure there's always a System device with a Configuration collector. This is so we can
                        // always collect the configuration data when it comes in.
                        DeviceInfo system = db.GetDevice("System", conn);
                        if (system == null)
                        {
                            // No System device exists, so we want to add it and add a Configuration collector
                            system = new DeviceInfo(EDeviceType.System) { name = "System" };
                            db.AddDevice(system, DateTimeOffset.Now, conn);
                        }
                        else
                        {
                            // One exists...see if the Configuration collector is there
                            CollectorInfo config_info = system.collectors.FirstOrDefault(c => c.collectorType == ECollectorType.Configuration);
                            if (config_info == null)
                            {
                                // Nope. Add it.
                                config_info = new CollectorInfo(ECollectorType.Configuration);
                                system.collectors.Add(config_info);
                                Database.UpdateCollectors(system.id, "System", string.Empty, system.collectors, DateTimeOffset.Now, conn);
                            }
                        }

                        // And make sure there's a server device is there as well
                        List<DeviceInfo> servers = db.GetDevicesFromType(EDeviceType.Server, conn);
                        if (servers.Count == 0)
                        {
                            DeviceInfo server = new DeviceInfo(EDeviceType.Server) { name = "Server", ipAddress = "localhost" };
                            db.AddDevice(server, DateTimeOffset.Now, conn);
                        }
                    }

                    // We changed the "monitored.drives.descriptions" attributes so they are now "all.drives.descriptions" since
                    // all of the drives descriptions are kept there instead of just the few that are being monitored. So, let's rename
                    // these attributes.
                    Attribute attr = new Attribute();
                    Dictionary<string, string> d = attr.GetMultiple("monitored.drives.descriptions", conn);
                    foreach (string path in d.Keys)
                    {
                        attr.Clear(path, conn);
                        string path2 = path.Replace("monitored.", "all.");
                        attr.Set(path2, d[path], conn);
                    }

                    // Initialize the last configuration update time if it doesn't exist
                    DateTimeOffset? first_config = db.GetLastConfigurationUpdateAttribute(conn);
                    if (first_config == null)
                    {
                        first_config = DateTimeOffset.Now;

                        attr = new Attribute();
                        attr.Set("configuration.last_update", first_config.Value, conn);
                    }

                    // Doing this will ensure that the default configurations will be put in the Configuration table
                    foreach (Configuration c in Configuration.Configs)
                    {
                        string v = c.GetValue(conn);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error in Database Initializer");
                log.Error(e);
            }
        }

        private void CreateTables(SQLiteConnection conn)
        {
            ILog log = LogManager.GetLogger(typeof(Initializer));
            log.Debug("Initializing tables");

            List<string> creation_statements = GetTableCreationStatements();
            foreach(string statement in creation_statements)
            {
                log.Info($"Executing '{statement}'");
                conn.ExecuteNonQuery(statement);
            }
        }

        private void CreateIndices(SQLiteConnection conn)
        {
            ILog log = LogManager.GetLogger(typeof(Initializer));
            log.Debug("Initializing indices");

            List<string> creation_statements = GetIndexCreationStatements();
            foreach (string statement in creation_statements)
            {
                log.Info($"Executing '{statement}'");
                conn.ExecuteNonQuery(statement);
            }
        }

        private void InitializeTypeTables(SQLiteConnection conn)
        {
            Dictionary<int, string> device_types = new Dictionary<int, string>();
            foreach (EDeviceType type in Enum.GetValues(typeof(EDeviceType)))
                device_types[(int)type] = type.GetDescription();
            conn.PopulateTypesTable(device_types, "DeviceTypes", "DeviceType");

            Dictionary<int, string> collector_types = new Dictionary<int, string>();
            foreach (ECollectorType type in Enum.GetValues(typeof(ECollectorType)))
                collector_types[(int)type] = type.GetDescription();
            conn.PopulateTypesTable(collector_types, "CollectorTypes", "CollectorType");

            Dictionary<int, string> status_types = new Dictionary<int, string>();
            foreach (EStatusType type in Enum.GetValues(typeof(EStatusType)))
                status_types[(int)type] = type.GetDescription();
            conn.PopulateTypesTable(status_types, "StatusTypes", "StatusType");

            Dictionary<int, string> file_types = new Dictionary<int, string>();
            foreach (EFileType type in Enum.GetValues(typeof(EFileType)))
                file_types[(int)type] = type.GetDescription();
            conn.PopulateTypesTable(file_types, "FileTypes", "FileType");
        }

        private void AddMissingColumns(SQLiteConnection conn)
        {
            ILog log = LogManager.GetLogger(typeof(Initializer));
            log.Info("Checking for column existence in DeviceStatus table");

            //Add new columns/tables here
            if (conn.DoesColumnExist("DeviceStatus", "IsValid") == false)
            {
                log.Info("Adding Message and IsValid columns to DeviceStatus table");

                string[] queries =
                {
                    "ALTER TABLE DeviceStatus ADD COLUMN Message TEXT;",
                    "ALTER TABLE DeviceStatus ADD COLUMN IsValid INTEGER NOT NULL DEFAULT 0;",
                    "CREATE INDEX DeviceStatusIsValid ON DeviceStatus(IsValid);"
                };

                foreach(string query in queries)
                {
                    log.Info(query);
                    conn.ExecuteNonQuery(query);
                }

                // Make sure the existing statuses are valid. The default is 0, for good reason, but we want the
                // existing ones to default to valid.
                Updater updater = new Updater("DeviceStatus", "IsValid = 0", conn);
                updater.Set("IsValid", 1);
                updater.Execute();
            }

            log.Info("Checking for column existence in Collectors table");

            if (conn.DoesColumnExist("Collectors", "NextCollectionTime") == false)
            {
                log.Info("Adding LastCollectionAttempt, LastCollectedAt, NextCollectionTime, and CurrentlyBeingCollected columns to Collectors table");

                string[] queries =
                {
                    "ALTER TABLE Collectors ADD COLUMN LastCollectionAttempt TEXT NULL DEFAULT NULL;",
                    "ALTER TABLE Collectors ADD COLUMN LastCollectedAt TEXT NULL DEFAULT NULL;",
                    "ALTER TABLE Collectors ADD COLUMN NextCollectionTime TEXT NULL DEFAULT NULL;",
                    // Make the default 1 so we don't report false negatives. I think it's better to report when a
                    // collection succeeds or fails than to assume it failed and report when it succeeds.
                    "ALTER TABLE Collectors ADD COLUMN SuccessfullyCollected INTEGER NOT NULL DEFAULT 1;",
                    "ALTER TABLE Collectors ADD COLUMN CurrentlyBeingCollected INTEGER NOT NULL DEFAULT 0;"
                };

                foreach (string query in queries)
                {
                    log.Info(query);
                    conn.ExecuteNonQuery(query);
                }

                // Make sure the existing SuccessfullyCollected are valid. The default is 1.
                Updater updater = new Updater("Collectors", conn);
                updater.Set("SuccessfullyCollected", 1);
                updater.Execute();

                updater = new Updater("Collectors", conn);
                updater.Set("CurrentlyBeingCollected", 0);
                updater.Execute();
            }

            // Unfortunately, we can't add a foreign key from the Devices.GroupID column to the DeviceGroups.GroupID column
            // because SQLite doesn't allow adding a foreign key after the fact. And I don't want to make a different database
            // that was created vs. upgraded.
            if(conn.DoesColumnExist("Devices", "GroupID") == false)
            {
                conn.ExecuteNonQuery("ALTER TABLE Devices ADD COLUMN GroupID INTEGER NULL DEFAULT NULL;");
            }
        }

        /// <summary>
        /// Get a map of the table names, and the queries needed to create those
        /// tables. Will be used to make sure all tables exist.
        /// 
        /// This only does the initial table creation. If a table is modified later,
        /// like if a new column is added to an existing table, this isn't used
        /// to add the column. The query should add the new column so if a new
        /// database is built the column will be added during table creation.
        /// </summary>
        /// <returns></returns>
        private static List<string> GetTableCreationStatements()
        {
            string[] raw_creation_statements =
            {
                @"Attributes(
                    Path TEXT NOT NULL,
                    Value TEXT,
                    PRIMARY KEY(Path));",
                @"CollectorTypes(
                    CollectorType INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    PRIMARY KEY(CollectorType));",
                @"DeviceTypes(
                    DeviceType INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    PRIMARY KEY(DeviceType));",
                @"StatusTypes(
                    StatusType INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    PRIMARY KEY(StatusType));",
                @"Configuration (
                    ConfigurationID INTEGER NOT NULL PRIMARY KEY,
                    Path TEXT NOT NULL,
                    Value TEXT,
                    DateAdded TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    IsValid INTEGER NOT NULL DEFAULT 0);",
                @"NetworkStatus (
                    IPAddress TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    SuccessfulPing INTEGER NOT NULL,
                    DateSuccessfulPingOccurred TEXT NOT NULL,
                    DatePingAttempted TEXT NOT NULL,
                    PRIMARY KEY(IPAddress));",
                @"Devices (
                    DeviceID INTEGER NOT NULL PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Type INTEGER NOT NULL,
                    IPAddress TEXT,
                    Username TEXT,
                    Password TEXT,
                    DateActivated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    DateDisabled TEXT,
                    FOREIGN KEY(Type) REFERENCES DeviceTypes(DeviceType));",
                @"DeviceStatus (
                    DeviceID INTEGER NOT NULL,
                    StatusType INTEGER NOT NULL,
                    IsAlarm INTEGER NOT NULL,
                    Date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    Message TEXT,
                    IsValid INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
                    FOREIGN KEY(StatusType) REFERENCES StatusTypes(StatusType));",
                @"DeviceGroups (
                    GroupID INTEGER NOT NULL PRIMARY KEY,
                    Name TEXT NOT NULL);",
                @"Collectors (
                    CollectorID INTEGER NOT NULL PRIMARY KEY,
                    Name TEXT NOT NULL,
                    DeviceID INTEGER NOT NULL,
                    CollectorType INTEGER NOT NULL,
                    IsEnabled INTEGER NOT NULL DEFAULT 1,
                    FrequencyInMinutes INTEGER NOT NULL DEFAULT 360,
                    LastCollectionAttempt TEXT NULL DEFAULT NULL,
                    LastCollectedAt TEXT NULL DEFAULT NULL,
                    NextCollectionTime TEXT NULL DEFAULT NULL,
                    SuccessfullyCollected INTEGER NOT NULL DEFAULT 1,
                    CurrentlyBeingCollected INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
                    FOREIGN KEY(CollectorType) REFERENCES CollectorTypes(CollectorType));",
                @"Data (
                    DataID INTEGER NOT NULL PRIMARY KEY,
                    CollectorID INTEGER NOT NULL,
                    Value TEXT NOT NULL,
                    TimeStamp TEXT NOT NULL,
                    WrittenToDailyFile INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID));",
                @"MostRecentDataPerCollector (
                    CollectorID INTEGER NOT NULL,
                    DataID INTEGER NOT NULL,
                    PRIMARY KEY(CollectorID, DataID)
                    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID),
                    FOREIGN KEY(DataID) REFERENCES Data(DataID));",
                @"FileTypes (
	                FileType INTEGER NOT NULL,
	                Description TEXT NOT NULL,
	                PRIMARY KEY(FileType));",
                @"Files (
	                FileID INTEGER NOT NULL PRIMARY KEY,
	                FileType INTEGER NOT NULL,
	                Name TEXT NOT NULL,
	                Size INTEGER NOT NULL,
	                ModificationTime TEXT NOT NULL,
	                SHA256 TEXT NOT NULL,
	                FOREIGN KEY(FileType) REFERENCES FileTypes(FileType));",
            };

            List<string> creation_statements = new List<string>();
            foreach(string table_statement in raw_creation_statements)
                creation_statements.Add("CREATE TABLE IF NOT EXISTS " + table_statement);
            return creation_statements;
        }

        private static List<string> GetIndexCreationStatements()
        {
            string[] raw_creation_statements =
            {
                "Configuration_IsValid ON Configuration (IsValid);",
                "Devices_Name ON Devices(Name);",
                "DeviceStatusIsValid ON DeviceStatus(IsValid);",
                "Collectors_DeviceID_CollectorType ON Collectors(DeviceID, CollectorType);",
                "Data_CollectorID ON Data(CollectorID);",
                "Data_CollectorID_Timestamp ON Data(CollectorID, Timestamp);",
                "Data_TimeStamp ON Data (TimeStamp);",
                "Data_WrittenToDailyFile ON Data (WrittenToDailyFile);",
                "MostRecentDataPerCollector_CollectorID ON MostRecentDataPerCollector(CollectorID);",
                "MostRecentDataPerCollector_DataID ON MostRecentDataPerCollector(DataID);",
                "Files_FileType_Name ON Files(FileType, Name);",
            };

            List<string> creation_statements = new List<string>();
            foreach(string index_statment in raw_creation_statements)
                creation_statements.Add("CREATE INDEX IF NOT EXISTS " + index_statment);
            return creation_statements;
        }
    }
}
