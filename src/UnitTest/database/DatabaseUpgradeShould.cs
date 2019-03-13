using gov.sandia.sld.common.db;
using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using System.Data.SQLite;
using Xunit;

namespace UnitTest.database
{
    public class DatabaseUpgradeShould
    {
        [Fact]
        public void WorkFrom131ToCurrent()
        {
            // The queries that created the DB as it existed for COMMON 1.3.1
            List<string> sql = new List<string>(new string[] {
@"CREATE TABLE Configuration (
    ConfigurationID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Path TEXT NOT NULL,
    Value TEXT,
    DateAdded TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsValid INTEGER NOT NULL DEFAULT 0
);",
@"CREATE TABLE DeviceTypes(
    DeviceType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(DeviceType)
);",
@"CREATE TABLE CollectorTypes(
    CollectorType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(CollectorType)
);",
@"CREATE TABLE Devices(
    DeviceID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Type INTEGER NOT NULL,
    IPAddress TEXT,
    Username TEXT,
    Password TEXT,
    DateActivated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DateDisabled TEXT,
    FOREIGN KEY(Type) REFERENCES DeviceTypes(DeviceType)
);",
@"CREATE TABLE Collectors(
    CollectorID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    DeviceID INTEGER NOT NULL,
    CollectorType INTEGER NOT NULL,
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    FrequencyInMinutes INTEGER NOT NULL DEFAULT 360,
    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
    FOREIGN KEY(CollectorType) REFERENCES CollectorTypes(CollectorType)
);",
@"CREATE TABLE Data(
    DataID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    CollectorID INTEGER NOT NULL,
    Value TEXT NOT NULL,
    TimeStamp TEXT NOT NULL,
    WrittenToDailyFile INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID)
);",
@"CREATE TABLE MostRecentDataPerCollector(
    CollectorID INTEGER NOT NULL,
    DataID INTEGER NOT NULL,
    PRIMARY KEY(CollectorID, DataID)
            
    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID),
    FOREIGN KEY(DataID) REFERENCES Data(DataID)
);",
@"CREATE TABLE NetworkStatus(
    IPAddress TEXT NOT NULL,
    Name TEXT NOT NULL,
    SuccessfulPing INTEGER NOT NULL,
    DateSuccessfulPingOccurred TEXT NOT NULL,
    DatePingAttempted TEXT NOT NULL,
    PRIMARY KEY(IPAddress)
);",
@"CREATE TABLE StatusTypes(
    StatusType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(StatusType)
);",
@"CREATE TABLE DeviceStatus(
    DeviceID INTEGER NOT NULL,
    StatusType INTEGER NOT NULL,
    IsAlarm INTEGER NOT NULL,
    Date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
    FOREIGN KEY(StatusType) REFERENCES StatusTypes(StatusType)
);",
@"CREATE TABLE Attributes(
    Path TEXT NOT NULL,
    Value TEXT,
    PRIMARY KEY(Path)
);",
@"CREATE INDEX Data_WrittenToDailyFile ON Data(WrittenToDailyFile);",
@"CREATE INDEX Data_TimeStamp ON Data(TimeStamp);",
@"CREATE INDEX Data_CollectorID ON Data(CollectorID);",
@"CREATE INDEX Configuration_IsValid ON Configuration(IsValid);",
@"CREATE INDEX Collectors_DeviceID_CollectorType ON Collectors(DeviceID, CollectorType);",
@"CREATE INDEX MostRecentDataPerCollector_CollectorID ON MostRecentDataPerCollector(CollectorID);",
@"CREATE INDEX MostRecentDataPerCollector_DataID ON MostRecentDataPerCollector(DataID);",
@"INSERT INTO DeviceTypes(DeviceType, Description) VALUES
(0, 'Server'),
(1, 'Workstation'),
(2, 'Camera'),
(3, 'RPM'),
(4, 'System');",
@"INSERT INTO CollectorTypes(CollectorType, Description) VALUES
(-1, 'Unknown'),
(0, 'Memory'),
(1, 'Disk'),
(2, 'CPUUsage'),
(3, 'NICUsage'),
(4, 'Uptime'),
(5, 'LastBootTime'),
(6, 'Processes'),
(7, 'Ping'),
(8, 'InstalledApplications'),
(9, 'Services'),
(11, 'SystemErrors'),
(12, 'ApplicationErrors'),
(13, 'DatabaseSize'),
(14, 'UPS'),
(15, 'DiskSpeed');",
@"INSERT INTO Configuration(Path, Value, IsValid) VALUES
('site.name', 'Test Site Name', 1);",
@"INSERT INTO Devices(Name, Type) VALUES('System', 4);",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'System.Ping', S.seq, 7, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Devices(Name, Type) VALUES('Server', 0);",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.NICUsage', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.SystemErrors', S.seq, 11, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
@"INSERT INTO Collectors(Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.ApplicationErrors', S.seq, 12, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';",
            });

            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    // Create the DB as it existed at 1.3.1, then make sure the tables are all there
                    sql.ForEach(s => conn.ExecuteNonQuery(s));

                    Assert.True(conn.DoesTableExist("Configuration"));
                    Assert.True(conn.DoesTableExist("DeviceTypes"));
                    Assert.True(conn.DoesTableExist("CollectorTypes"));
                    Assert.True(conn.DoesTableExist("Devices"));
                    Assert.True(conn.DoesTableExist("Collectors"));
                    Assert.True(conn.DoesTableExist("Data"));
                    Assert.True(conn.DoesTableExist("MostRecentDataPerCollector"));
                    Assert.True(conn.DoesTableExist("NetworkStatus"));
                    Assert.True(conn.DoesTableExist("StatusTypes"));
                    Assert.True(conn.DoesTableExist("DeviceStatus"));
                    Assert.True(conn.DoesTableExist("Attributes"));

                    // Create an initializer and initialize the DB. Then we can check that the new
                    // columns and tables exist.
                    Initializer init = new Initializer(null);
                    init.Initialize(db);

                    Assert.True(conn.DoesColumnExist("Collectors", "LastCollectionAttempt"));
                    Assert.True(conn.DoesColumnExist("Collectors", "LastCollectedAt"));
                    Assert.True(conn.DoesColumnExist("Collectors", "NextCollectionTime"));
                    Assert.True(conn.DoesColumnExist("Collectors", "SuccessfullyCollected"));
                    Assert.True(conn.DoesColumnExist("Collectors", "CurrentlyBeingCollected"));

                    Assert.True(conn.DoesTableExist("DeviceGroups"));

                    Assert.True(conn.DoesColumnExist("DeviceStatus", "Message"));
                    Assert.True(conn.DoesColumnExist("DeviceStatus", "IsValid"));

                    Assert.True(conn.DoesColumnExist("Devices", "GroupID"));

                    Assert.True(conn.DoesTableExist("FileTypes"));
                    Assert.True(conn.DoesTableExist("Files"));
                }
            }
        }
    }
}
