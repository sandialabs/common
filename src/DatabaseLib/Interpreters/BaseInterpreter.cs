using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.requestresponse;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace gov.sandia.sld.common.db.interpreters
{
    public abstract class BaseInterpreter
    {
        protected static long GetDeviceID(Data d, SQLiteConnection conn)
        {
            long device_id = -1;
            string sql = $"SELECT D.DeviceID FROM Devices D INNER JOIN Collectors C ON D.DeviceID = C.DeviceID WHERE C.CollectorID = {d.ID} AND D.DateDisabled IS NULL;";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                    device_id = reader.GetInt64(0);
            }
            return device_id;
        }

        /// <summary>
        /// Updates a device's status. If the new status is null, the existing status should be invalidated and no new
        /// entry added. If the new status is non-null, see it the existing status matches the new status. If the new
        /// and old status are identical, do nothing. If the new and old status are different somehow (the status itself,
        /// the is-alarm flag or the message are different), invalidate the old status and insert a new status.
        /// 
        /// A single device may have different statuses, so we need to restrict our decisions to a set of types, which
        /// is included in the 3rd parameter. For example, the memory statuses are adequate, low, or critically low. Only
        /// one of these should be 'active' at a given time.
        /// </summary>
        /// <param name="device_id">The ID of the device we're checking</param>
        /// <param name="type">The new type. Can be null to indicate the status should be cleared</param>
        /// <param name="statuses">The full set of statuses we should be considering. Will be a subset of all
        /// the statuses a device can have.</param>
        /// <param name="message">A message regarding the status</param>
        /// <param name="conn">The DB connection to use</param>
        protected void SetDeviceStatus(long device_id, EStatusType? type, List<EStatusType> statuses, string message, SQLiteConnection conn)
        {
            if (type.HasValue)
                Debug.Assert(statuses.Contains(type.Value));

            EAlertLevel alert_level = (type.HasValue ? type.Value.GetAlertLevel() : null) ?? EAlertLevel.Normal;
            string in_clause = statuses.Join();
            string clause = $"DeviceID = {device_id} AND StatusType IN ({in_clause}) AND IsValid = 1";
            string sql = $"SELECT StatusType, IsAlarm, Message FROM DeviceStatus WHERE {clause};";
            bool insert = type.HasValue;
            bool remove = type.HasValue == false;

            if (type.HasValue)
            {
                // We may be inserting a new type. We need to see if the current value for the device/status is the
                // same, or has changed. If it's the same we don't need to do anything. If it's changed, we'll want to
                // mark the old value as invalid, and insert the new value.
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        EStatusType existing_type = (EStatusType)reader.GetInt32(0);
                        EAlertLevel existing_alert_level = (EAlertLevel)reader.GetInt32(1);
                        string existing_message = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);

                        // An existing record exists, so insert the new one and update the old one if something's changed.
                        // If nothing changed, we'll leave the old one alone
                        bool something_changed =
                            (type.Value != existing_type) ||
                            (alert_level != existing_alert_level) ||
                            (string.Compare(message, existing_message) != 0);

                        // If something has changed, we'll want to remove the old version, and insert the new
                        // version. If nothing changed, we don't want to do either.
                        insert = remove = something_changed;
                    }
                    // If it wasn't found, just insert, which is the default
                }
            }
            // In this case there's no status value, so that means were clearing out the old one, if it exists.

            if (insert || remove)
            {
                if (remove)
                {
                    Updater updater = new Updater("DeviceStatus", clause, conn);
                    updater.Set("IsValid", 0);
                    updater.Execute();
                }

                if (insert)
                {
                    Inserter inserter = new Inserter("DeviceStatus", conn);
                    inserter.Set("DeviceID", device_id);
                    inserter.Set("StatusType", (int)type);
                    inserter.Set("IsAlarm", (int)alert_level);
                    inserter.Set("Date", DateTimeOffset.Now);
                    inserter.Set("Message", message, false);
                    inserter.Set("IsValid", 1);
                    inserter.Execute();

                    AlertMessage alert = new AlertMessage(device_id, type.Value, alert_level, message);
                    SystemBus.Instance.SendMessage(alert);
                }
            }
            // else, no change
        }

        protected static void ClearDeviceStatus(long device_id, List<EStatusType> statuses, SQLiteConnection conn)
        {
            string in_clause = statuses.Join();
            string clause = $"DeviceID = {device_id} AND StatusType IN ({in_clause}) AND IsValid = 1";
            Updater updater = new Updater("DeviceStatus", clause, conn);
            updater.Set("IsValid", 0);
            updater.Execute();
        }

        public static List<IDataInterpreter> AllInterpreters()
        {
            return new List<IDataInterpreter>(new IDataInterpreter[] {
                    new PingInterpreter(),
                    new OfflineInterpreter(),
                    new DiskSpaceInterpreter(),
                    new MemoryInterpreter(),
                    new CPUUsageInterpreter(),
                    new TimeInterpreter(),
                    //new DatabaseSizeInterpreter(),
                    new UPSInterpreter(),
                    new SMARTInterpreter(),
                });
        }
    }
}
