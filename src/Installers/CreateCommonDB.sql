CREATE TABLE Configuration (
    ConfigurationID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Path TEXT NOT NULL,
    Value TEXT,
    DateAdded TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsValid INTEGER NOT NULL DEFAULT 0
);
CREATE TABLE DeviceTypes (
    DeviceType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(DeviceType)
);
CREATE TABLE CollectorTypes (
    CollectorType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(CollectorType)
);
CREATE TABLE Devices (
    DeviceID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Type INTEGER NOT NULL,
    IPAddress TEXT,
    Username TEXT,
    Password TEXT,
    DateActivated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DateDisabled TEXT,
    FOREIGN KEY(Type) REFERENCES DeviceTypes(DeviceType)
);
CREATE TABLE Collectors (
    CollectorID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    DeviceID INTEGER NOT NULL,
    CollectorType INTEGER NOT NULL,
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    FrequencyInMinutes INTEGER NOT NULL DEFAULT 360,
    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
    FOREIGN KEY(CollectorType) REFERENCES CollectorTypes(CollectorType)
);
CREATE TABLE Data (
    DataID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    CollectorID INTEGER NOT NULL,
    Value TEXT NOT NULL,
    TimeStamp TEXT NOT NULL,
    WrittenToDailyFile INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID)
);
CREATE TABLE MostRecentDataPerCollector (
    CollectorID INTEGER NOT NULL,
    DataID INTEGER NOT NULL,
    PRIMARY KEY(CollectorID, DataID)
    FOREIGN KEY(CollectorID) REFERENCES Collectors(CollectorID),
    FOREIGN KEY(DataID) REFERENCES Data(DataID)
);
CREATE TABLE NetworkStatus (
    IPAddress TEXT NOT NULL,
    Name TEXT NOT NULL,
    SuccessfulPing INTEGER NOT NULL,
    DateSuccessfulPingOccurred TEXT NOT NULL,
    DatePingAttempted TEXT NOT NULL,
    PRIMARY KEY(IPAddress)
);
CREATE TABLE StatusTypes (
    StatusType INTEGER NOT NULL,
    Description TEXT NOT NULL,
    PRIMARY KEY(StatusType)
);
CREATE TABLE DeviceStatus (
    DeviceID INTEGER NOT NULL,
    StatusType INTEGER NOT NULL,
    IsAlarm INTEGER NOT NULL,
    Date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(DeviceID) REFERENCES Devices(DeviceID),
    FOREIGN KEY(StatusType) REFERENCES StatusTypes(StatusType)
);
CREATE TABLE Attributes (
    Path TEXT NOT NULL,
    Value TEXT,
    PRIMARY KEY(Path)
);

CREATE INDEX Data_WrittenToDailyFile ON Data (WrittenToDailyFile);
CREATE INDEX Data_TimeStamp ON Data (TimeStamp);
CREATE INDEX Data_CollectorID ON Data(CollectorID);
CREATE INDEX Configuration_IsValid ON Configuration (IsValid);
CREATE INDEX Collectors_DeviceID_CollectorType ON Collectors(DeviceID, CollectorType);
CREATE INDEX MostRecentDataPerCollector_CollectorID ON MostRecentDataPerCollector(CollectorID);
CREATE INDEX MostRecentDataPerCollector_DataID ON MostRecentDataPerCollector(DataID);

INSERT INTO DeviceTypes (DeviceType, Description) VALUES
(0, 'Server'),
(1, 'Workstation'),
(2, 'Camera'),
(3, 'RPM'),
(4, 'System');

INSERT INTO CollectorTypes (CollectorType, Description) VALUES
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
-- (10, 'Database'),
(11, 'SystemErrors'),
(12, 'ApplicationErrors'),
(13, 'DatabaseSize'),
(14, 'UPS'),
(15, 'DiskSpeed');

INSERT INTO Configuration (Path, Value, IsValid) VALUES
('site.name', 'Initial Site Name', 1),
-- ('contact.name', 'Justin Weaver', 1),
('contact.email', 'jweaver@sandia.gov', 1);

INSERT INTO Devices (Name, Type) VALUES ('System', 4);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'System.Ping', S.seq, 7, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

-- The SELECT S.seq FROM sqlite_sequence AS S WHERE S.name = 'Devices'
-- is used to get the most-recent identity value assigned in the Devices table. The
-- sqlite_sequence table is automatically generated when one of the tables has an identity
-- column, and SQLite uses it to maintain the value of the most-recently-assigned identity
-- value for each table. If we had used last_insert_rowid(), for example when inserting into
-- Collectors, the second insert would've returned the value assigned to the Collectors table,
-- not the one we want from the Devices table.
-- We could've created a temporary table to hold identity values, then assigned it using
-- last_insert_rowid(), then selected it, but doing it the way done below is slightly less work.

INSERT INTO Devices (Name, Type) VALUES ('Server', 0);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.NICUsage', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.SystemErrors', S.seq, 11, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Server.ApplicationErrors', S.seq, 12, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

-- For testing purposes
/*
INSERT INTO Devices (Name, Type) VALUES ('Lane 1 Workstation', 1);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.SystemErrors', S.seq, 11, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Workstation.ApplicationErrors', S.seq, 12, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

INSERT INTO Devices (Name, Type) VALUES ('Lane 2 Workstation', 1);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Workstation.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

INSERT INTO Devices (Name, Type) VALUES ('Lane 1 Camera', 2);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 Camera.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

INSERT INTO Devices (Name, Type) VALUES ('Lane 2 Camera', 2);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 Camera.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

INSERT INTO Devices (Name, Type) VALUES ('Lane 1 RPM', 3);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 1 RPM.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';

INSERT INTO Devices (Name, Type) VALUES ('Lane 2 RPM', 3);
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.Memory', S.seq, 0, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.Disk', S.seq, 1, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.CPUUsage', S.seq, 2, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.NICBytesPerSecond', S.seq, 3, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.Uptime', S.seq, 4, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.LastBootTime', S.seq, 5, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
INSERT INTO Collectors (Name, DeviceID, CollectorType, IsEnabled, FrequencyInMinutes) SELECT 'Lane 2 RPM.Processes', S.seq, 6, 1, 360 FROM sqlite_sequence AS S WHERE S.name = 'Devices';
*/