using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using static gov.sandia.sld.common.db.ConfigStrings;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// We have well-defined configuration settings, so let's define a series of classes that
    /// let you work with them directly.
    /// </summary>
    public abstract class Configuration
    {
        public string Path { get; private set; }

        public static List<Configuration> Configs { get; private set; }

        protected Configuration(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Sets the IsValid field in the database to 0
        /// </summary>
        public void Disable(SQLiteConnection conn)
        {
            Clear(Path, conn);
        }

        public static string GetValue(string path, SQLiteConnection conn)
        {
            string str = null;
            string sql = $"SELECT Value FROM Configuration WHERE Path = '{path}' AND IsValid = 1;";
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read() && reader.IsDBNull(0) == false)
                        str = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Configuration));
                log.Error("GetValue: " + sql);
                log.Error(ex);
            }

            return str;
        }

        public virtual string GetValue(SQLiteConnection conn)
        {
            return GetValue(Path, conn);
        }

        public int? GetValueAsInt(SQLiteConnection conn)
        {
            string value = GetValue(conn);
            if (int.TryParse(value, out int i))
                return i;
            else
                return null;
        }

        public double? GetValueAsDouble(SQLiteConnection conn)
        {
            string value = GetValue(conn);
            if (double.TryParse(value, out double d))
                return d;
            else
                return null;
        }

        public bool? GetValueAsBoolean(SQLiteConnection conn)
        {
            int? i = GetValueAsInt(conn);
            if (i.HasValue)
                return i != 0;
            else
                return null;
        }

        public static string GetValue(string path, string default_value, SQLiteConnection conn)
        {
            string str = GetValue(path, conn);

            if (string.IsNullOrEmpty(str))
            {
                SetValue(path, default_value, true, DateTimeOffset.Now, conn);
                str = default_value;
            }

            return str;
        }

        public static void SetValue(string path, string value, bool normalize, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            if (string.IsNullOrEmpty(path))
                return;

            string existing_value = GetValue(path, conn);

            if (value != existing_value)
            {
                Clear(path, conn);

                Inserter insert = new Inserter("Configuration", conn);
                insert.Set("Path", path, true);
                insert.Set("Value", value ?? string.Empty, normalize);
                insert.Set("DateAdded", timestamp);
                insert.Set("IsValid", 1);
                insert.Execute();
            }
        }

        public virtual void SetValue(string value, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SetValue(Path, value, false, timestamp, conn);
        }

        public virtual void SetValue(int value, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SetValue(value.ToString(), timestamp, conn);
        }

        public virtual void SetValue(double value, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SetValue(value.ToString(), timestamp, conn);
        }

        public virtual void SetValue(bool value, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SetValue((int)(value ? 1 : 0), timestamp, conn);
        }

        public static void Clear(string path, SQLiteConnection conn)
        {
            Updater update = new Updater("Configuration", $"Path = '{path}' AND IsValid = 1", conn);
            update.Set("IsValid", 0);
            update.Execute();
        }

        static Configuration()
        {
            Configs = new List<Configuration>(
                new Configuration[]
                {
                    new CountryCode(),
                    new SiteName(),
                    new CPUUsageAlert(),
                    new CPUUsageAlertCounts(),
                    new DailyFileLocation(@"c:\COMMON\DailyFiles"),
                    new DeleteDays(),
                    new DiskSpaceLowAlert(),
                    new DiskSpaceCriticallyLowAlert(),
                    new MemoryLowAlert(),
                    new MemoryCriticallyLowAlert(),
                    new RecentRebootAlert(),
                    new LongRebootAlert(),
                    new PingExtras(),
                    new PingSubnets(),
                    new PingNumPingers(),
                    new CompressDailyFile(),
                    new DeleteDailyFileAfterCompression(),
                    new AlertsEmailSMTP(),
                    new AlertsEmailFrom(),
                    new AlertsEmailTo(),
                });
        }
    }

    public abstract class EncryptedConfiguration : Configuration
    {
        protected EncryptedConfiguration(string path) : base(path)
        {
        }

        public override string GetValue(SQLiteConnection conn)
        {
            string value = base.GetValue(conn);
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            return sed.Decrypt(value);
        }

        public override void SetValue(string value, DateTimeOffset timestamp, SQLiteConnection conn)
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();
            string enc_value = sed.Encrypt(value);
            Configuration.SetValue(Path, enc_value, false, timestamp, conn);
        }
    }

    public abstract class ConfigurationWithDefault : Configuration
    {
        public string DefaultValue { get; private set; }

        protected ConfigurationWithDefault(string path, string default_value)
            : base(path)
        {
            DefaultValue = default_value;
        }

        protected ConfigurationWithDefault(string path, int default_value)
            : base(path)
        {
            DefaultValue = default_value.ToString();
        }

        public override string GetValue(SQLiteConnection conn)
        {
            string s = base.GetValue(conn);
            if (s == null)
            {
                s = DefaultValue;
                SetValue(s, DateTimeOffset.Now, conn);
            }
            return s;
        }
    }

    public class CountryCode : ConfigurationWithDefault
    {
        public CountryCode()
            : base(countryCode, xx)
        {
        }
    }

    public class SiteName : ConfigurationWithDefault
    {
        public SiteName()
            : base(siteName, unknown)
        {
        }
    }

    public class CPUUsageAlert : ConfigurationWithDefault
    {
        public CPUUsageAlert()
            : base(cpuUsageAlert, 75)
        {
        }
    }

    public class CPUUsageAlertCounts : ConfigurationWithDefault
    {
        public CPUUsageAlertCounts()
            : base(cpuUsageAlertCounts, 5)
        {
        }
    }

    public class DiskSpaceLowAlert : ConfigurationWithDefault
    {
        public DiskSpaceLowAlert()
            : base(diskLowAlert, 80)
        {
        }
    }

    public class DiskSpaceCriticallyLowAlert : ConfigurationWithDefault
    {
        public DiskSpaceCriticallyLowAlert()
            : base(diskLowCriticalAlert, 90)
        {
        }
    }

    public class MemoryLowAlert : ConfigurationWithDefault
    {
        public MemoryLowAlert()
            : base(memoryLowAlert, 80)
        {
        }
    }

    public class MemoryCriticallyLowAlert : ConfigurationWithDefault
    {
        public MemoryCriticallyLowAlert()
            : base(memoryLowCriticalAlert, 90)
        {
        }
    }

    public class RecentRebootAlert : ConfigurationWithDefault
    {
        public RecentRebootAlert()
            : base(rebootRecentAlert, 1)
        {
        }
    }

    public class LongRebootAlert : ConfigurationWithDefault
    {
        public LongRebootAlert()
            : base(rebootLongAlert, 365)
        {
        }
    }

    public class DatabaseConnectionString : EncryptedConfiguration
    {
        public string DeviceName { get; private set; }

        public DatabaseConnectionString(string device_name)
            : base(device_name + databaseConnectionString)
        {
            DeviceName = device_name;
        }

        public static string Default = "data source=(local)\\SCADAREPORTS;user id=commonuser;password=commonuser_pass;";
    }

    public class DatabaseType : Configuration
    {
        public string DeviceName { get; private set; }

        public DatabaseType(string device_name)
            : base(device_name + databaseType)
        {
            DeviceName = device_name;
        }

        public EDatabaseType GetAsDatabaseType(SQLiteConnection conn)
        {
            string value = GetValue(Path, conn);
            return value.GetDatabaseType();
        }

        public static string Default = "SqlServer";
    }

    public class DailyFileLocation : ConfigurationWithDefault
    {
        public DailyFileLocation(string default_location)
            : base(dailyfileLocation, default_location)
        {
        }
    }

    public class DeleteDays : ConfigurationWithDefault
    {
        public DeleteDays()
            : base(deleteDays, 180)
        {
        }
    }

    public class PingExtras : Configuration
    {
        public PingExtras()
            : base(pingExtras)
        {
        }
    }

    public class PingSubnets : Configuration
    {
        public PingSubnets()
            : base(pingSubnets)
        {
        }
    }

    public class PingNumPingers : ConfigurationWithDefault
    {
        public PingNumPingers()
            : base(pingNumPingers, 8)
        {
        }
    }

    public class CompressDailyFile : ConfigurationWithDefault
    {
        public CompressDailyFile()
            : base(compressDailyFiles, 0)
        {
        }
    }

    public class DeleteDailyFileAfterCompression : ConfigurationWithDefault
    {
        public DeleteDailyFileAfterCompression()
            : base(deleteDailyFileAfterCompression, 0)
        {
        }
    }

    public class AlertsEmailSMTP : ConfigurationWithDefault
    {
        public AlertsEmailSMTP()
            : base(alertsEmailSMTP, "")
        {
        }
    }

    public class AlertsEmailFrom : ConfigurationWithDefault
    {
        public AlertsEmailFrom()
            : base(alertsEmailFrom, "")
        {
        }
    }

    public class AlertsEmailTo : ConfigurationWithDefault
    {
        public AlertsEmailTo()
            : base(alertsEmailTo, "")
        {
        }
    }

    [Localizable(false)]
    public static class ConfigStrings
    {
        public static string countryCode = "country.code";
        public static string siteName = "site.name";
        public static string cpuUsageAlert = "cpu.usage.alert";
        public static string cpuUsageAlertCounts = "cpu.usage.alert.counts";
        public static string diskLowAlert = "disk.low.alert";
        public static string diskLowCriticalAlert = "disk.low.critical.alert";
        public static string memoryLowAlert = "memory.low.alert";
        public static string memoryLowCriticalAlert = "memory.low.critical.alert";
        public static string rebootRecentAlert = "reboot.recent.alert";
        public static string rebootLongAlert = "reboot.long.alert";
        public static string databaseConnectionString = ".database.connection_string";
        public static string databaseType = ".database.type";
        public static string dailyfileLocation = "dailyfile.location";
        public static string deleteDays = "delete.days";
        public static string unknown = "unknown";
        public static string xx = "xx";
        public static string pingExtras = "ping.extras";
        public static string pingSubnets = "ping.subnets";
        public static string pingNumPingers = "ping.num_pingers";
        public static string compressDailyFiles = "dailyfile.compress";
        public static string deleteDailyFileAfterCompression = "dailyfile.compress.delete_after_compression";
        public static string alertsEmailSMTP = "alerts.email.smtp";
        public static string alertsEmailFrom = "alerts.email.from";
        public static string alertsEmailTo = "alerts.email.to";

        public static string[] DBStrings =
            {
                countryCode,
                siteName,
                cpuUsageAlert,
                cpuUsageAlertCounts,
                diskLowAlert,
                diskLowCriticalAlert,
                memoryLowAlert,
                memoryLowCriticalAlert,
                rebootRecentAlert,
                rebootLongAlert,
                databaseConnectionString,
                databaseType,
                dailyfileLocation,
                deleteDays,
                unknown,
                xx,
                pingExtras,
                pingSubnets,
                pingNumPingers,
                compressDailyFiles,
                deleteDailyFileAfterCompression,
                alertsEmailSMTP,
                alertsEmailFrom,
                alertsEmailTo,
            };
    }
}
