using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{

    /// <summary>
    /// This generates an alert when the average CPU usage over the last x readings is > y.
    /// 
    /// x is the number of readings to average over, and y is the threshold.
    /// 
    /// We do this so that a temporary spike isn't alerted, but long-term heavy usage is.
    /// </summary>
    public class CPUUsageInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type == ECollectorType.CPUUsage)
            {
                CPUUsageAlert cpu_alert_level = new CPUUsageAlert();
                double cpu_alert = cpu_alert_level.GetValueAsDouble(conn) ?? 80.0;
                CPUUsageAlertCounts alert_counts = new CPUUsageAlertCounts();
                // Make sure we get at least 1
                int a_counts = Math.Max(alert_counts.GetValueAsInt(conn) ?? 1, 1);

                // The interpretation should be done after the insertion, so the value that was just collected should be in the database already

                // Because we're averaging over the last x readings, we can't just use the single Data record d and have to do a query
                // to get the last y readings

                long device_id = GetDeviceID(d, conn);
                if (device_id >= 0)
                {
                    List<int> all_cpu = new List<int>();

                    // To see how the TimeStamp clause works, check out:
                    // https://www.sqlite.org/lang_datefunc.html
                    //string sql = string.Format("SELECT D.Value FROM Data AS D INNER JOIN Collectors AS C ON D.CollectorID = C.CollectorID WHERE C.DeviceID = {0} AND C.CollectorType = {1} AND TimeStamp >= date('now', '-1 day');",
                    //    device_id, (int)CollectorType.CPUUsage);
                    string sql = $"SELECT D.Value FROM Data AS D INNER JOIN Collectors AS C ON D.CollectorID = C.CollectorID WHERE C.DeviceID = {device_id} AND C.CollectorType = {(int)ECollectorType.CPUUsage} ORDER BY TimeStamp DESC LIMIT {a_counts};";
                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //var definition = new { Value = string.Empty };
                            //var val = JsonConvert.DeserializeAnonymousType(reader.GetString(0), definition);
                            var val = JsonConvert.DeserializeObject<ValueClass<string>>(reader.GetString(0));
                            if (int.TryParse(val.Value, out int cpu_usage))
                                all_cpu.Add(cpu_usage);
                        }
                    }

                    // When all_cpu doesn't have anything in it, Average() throws an exception, so we
                    // need to make sure there's something in it. And, we don't want to generate an alert
                    // unless we've gotten all the readings we should have. Given both of those, the
                    // average shouldn't even be calculated unless we have everything we want.
                    if(all_cpu.Count == a_counts)
                    {
                        CPUStatus cpu_status = new CPUStatus(cpu_alert);
                        EStatusType alert_status = cpu_status.GetStatus(all_cpu);
                        string message = $"{(int)(cpu_status.Average + 0.5)} %";

                        SetDeviceStatus(device_id, alert_status, CPUStatus.Types, message, conn);
                    }
                }
            }
        }

        private class ValueClass<T>
        {
            public T Value { get; set; }
        }
    }
}
