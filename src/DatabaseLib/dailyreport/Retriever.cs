using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.dailyreport
{
    public interface IDailyReportDataStore
    {
        List<DailyReport> GetIncompleteDailyReports(bool mark_when_complete, SQLiteConnection conn);
    }

    public class Retriever : IDailyReportDataStore
    {
        public Retriever()
        {
        }

        public List<DailyReport> GetIncompleteDailyReports(bool mark_when_complete, SQLiteConnection conn)
        {
            List<DailyReport> reports = new List<DailyReport>();
            List<DateTimeOffset> days_to_report = GetDaysWithIncompleteDailyReports(conn);

            do
            {
                DateTimeOffset day = days_to_report[0];
                days_to_report.RemoveAt(0);

                reports.Add(GetDailyReport(day, conn));

                // The days are sorted, so if there's at least 1 day beyond the day we're
                // working on then we can close out this day.
                if (mark_when_complete && days_to_report.Count > 0)
                {
                    try
                    {
                        Updater update = new Updater("Data",
                            string.Format("TimeStamp BETWEEN '{0}' AND '{1}'", day.DayBeginAs8601(), day.DayEndAs8601()),
                            conn);
                        update.Set("WrittenToDailyFile", 1);
                        update.Execute();
                    }
                    catch (Exception ex)
                    {
                        ILog log = LogManager.GetLogger(typeof(Retriever));
                        log.Error("GetIncompleteDailyReports");
                        log.Error(ex.Message);
                    }
                }
            }
            while (days_to_report.Count > 0);

            return reports;
        }

        private DailyReport GetDailyReport(DateTimeOffset day, SQLiteConnection conn)
        {
            Attribute attr = new Attribute();
            string software_version = attr.Get("software.version", conn);
            SiteName site_name = new SiteName();
            CountryCode country_code = new CountryCode();
            DailyReport report = new DailyReport(country_code.GetValue(conn), site_name.GetValue(conn), day, software_version);

            string sql = string.Format("SELECT C.Name, C.CollectorType, D.Value, D.Timestamp FROM Data D INNER JOIN Collectors C ON D.CollectorID = C.CollectorID WHERE D.Timestamp BETWEEN '{0}' AND '{1}' ORDER BY D.TimeStamp ASC;",
                day.DayBeginAs8601(), day.DayEndAs8601());

            string first_of_month = string.Format("{0:D4}-{1:D2}-01T00:00:00.000", day.Year, day.Month);

            try
            {
                if (day.DayBeginAs8601() == first_of_month)
                {
                    DailyReport.Record config_record = GetConfigurationRecord(day, conn);
                    if (config_record != null) report.records.Add(config_record);
                }

                SQLiteCommand command = new SQLiteCommand(sql, conn);
                using (command)
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DailyReport.Record record = new DailyReport.Record()
                        {
                            collector = reader.GetString(0),
                            type = (ECollectorType)reader.GetInt32(1),
                            value = reader.GetString(2),
                            timestamp = DateTimeOffset.Parse(reader.GetString(3))
                        };
                        report.records.Add(record);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Retriever));
                log.Error("GetDailyReport: " + sql);
                log.Error(ex);
            }

            return report;
        }

        private List<DateTimeOffset> GetDaysWithIncompleteDailyReports(SQLiteConnection conn)
        {
            List<DateTimeOffset> reports = new List<DateTimeOffset>();
            string sql = "SELECT TimeStamp FROM Data WHERE WrittenToDailyFile = 0;";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    HashSet<DateTimeOffset> incomplete_days = new HashSet<DateTimeOffset>();

                    while (reader.Read())
                    {
                        DateTimeOffset timestamp = DateTimeOffset.Parse(reader.GetString(0));
                        DateTimeOffset day = new DateTimeOffset(timestamp.Year, timestamp.Month, timestamp.Day, 0, 0, 0, TimeSpan.FromMinutes(0));

                        if (incomplete_days.Contains(day) == false)
                        {
                            incomplete_days.Add(day);
                        }
                    }

                    foreach (DateTimeOffset dt in incomplete_days)
                        reports.Add(dt);
                    reports.Sort();
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Retriever));
                log.Error("GetDaysWithIncompleteDailyReports: " + sql);
                log.Error(ex);
            }

            return reports;
        }

        /// <summary>
        /// Used to get the most recent System Configuration for a given day
        /// </summary>
        /// <param name="day">The day for query against for the configuration</param>
        /// <param name="conn">The DB connection to use</param>
        private DailyReport.Record GetConfigurationRecord(DateTimeOffset day, SQLiteConnection conn)
        {
            DailyReport.Record configurationRecord = null;

            string configuration_query = string.Format("SELECT C.Name, C.CollectorType, D.Value, D.Timestamp " +
                                                       "FROM Data D INNER JOIN Collectors C ON D.CollectorID = C.CollectorID " +
                                                       "WHERE C.CollectorType = '{0}' AND D.Timestamp < '{1}' " +
                                                       "ORDER BY D.TimeStamp DESC LIMIT 1;",

                    (int)ECollectorType.Configuration, day);

            SQLiteCommand configuration_command = new SQLiteCommand(configuration_query, conn);
            using (SQLiteDataReader reader = configuration_command.ExecuteReader())
            {
                while (reader.Read())
                {
                    configurationRecord = new DailyReport.Record()
                    {
                        collector = reader.GetString(0),
                        type = (ECollectorType)reader.GetInt32(1),
                        value = reader.GetString(2),
                        timestamp = DateTimeOffset.Parse(reader.GetString(3))
                    };
                }
                reader.Close();
            }
            return configurationRecord;
        }
    }
}
