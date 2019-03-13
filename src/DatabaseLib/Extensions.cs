using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db
{
    public static class Extensions
    {
        public static void ExecuteNonQuery(this SQLiteConnection conn, string sql)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Used to make sure the *Type tables are updated
        /// </summary>
        /// <param name="conn">The connection to the database</param>
        /// <param name="values">The key/value pair that needs to exist in the DB. If there's a missing entry, or the description
        /// changed, we'll insert or update as appropriate</param>
        /// <param name="table">The table we're going to be updating</param>
        /// <param name="type_column">The name of the type column.</param>
        public static void PopulateTypesTable(this SQLiteConnection conn, Dictionary<int, string> values, string table, string type_column)
        {
            ILog log = LogManager.GetLogger(typeof(Extensions));
            Dictionary<int, string> db_values = new Dictionary<int, string>();

            string select_query = string.Format("SELECT {0}, Description FROM {1};", type_column, table);
            using (SQLiteCommand command = new SQLiteCommand(select_query, conn))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    db_values[reader.GetInt32(0)] = reader.GetString(1);
            }

            List<int> db_types = new List<int>(db_values.Keys);
            List<int> types = new List<int>(values.Keys);
            db_types.ForEach(t => types.Remove(t));

            // Whatever's left needs to be added
            foreach(int type in types)
            {
                log.Info($"Inserting {type}=>{values[type]} into {table}");

                Inserter inserter = new Inserter(table, conn);
                inserter.Set(type_column, type);
                inserter.Set("Description", values[type], false);
                inserter.Execute();

                db_values[type] = values[type];
            }

            // Now compare the existing descriptions with the descriptions in values
            foreach(int type in values.Keys)
            {
                if(values[type] != db_values[type])
                {
                    log.Info($"Changing {type}=>{db_values[type]} to {values[type]} in {table}");

                    Updater updater = new Updater(table, $"{type_column} = {type}", conn);
                    updater.Set("Description", values[type], false);
                    updater.Execute();
                }
            }
        }

        public static string DayBeginAs8601(this DateTimeOffset dt)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2}T00:00:00.000", dt.Year, dt.Month, dt.Day);
        }

        public static string DayEndAs8601(this DateTimeOffset dt)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2}T23:59:59.999", dt.Year, dt.Month, dt.Day);
        }

        public static DateTimeOffset AsMidnight(this DateTimeOffset dt)
        {
            return new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, dt.Offset);
        }

        public static List<string> GetColumnNames(this SQLiteConnection conn, string table)
        {
            List<string> columns = new List<string>();
            try
            {
                string pragma = string.Format("PRAGMA table_info({0});", table);

                using (SQLiteCommand command = new SQLiteCommand(pragma, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    int name_index = reader.GetOrdinal("Name");

                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(name_index));
                    }
                }
            }
            catch (Exception)
            {
                columns.Clear();
            }

            return columns;
        }

        public static bool DoesColumnExist(this SQLiteConnection conn, string table, string column)
        {
            List<string> columns = conn.GetColumnNames(table);
            return columns.Find(c => string.Compare(c, column, true) == 0) != null;
        }

        public static bool DoesIndexExist(this SQLiteConnection conn, string table, string index)
        {
            bool does_exist = false;

            try
            {
                string pragma = string.Format("PRAGMA index_list({0});", table);

                using (SQLiteCommand command = new SQLiteCommand(pragma, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    int name_index = reader.GetOrdinal("name");
                    while (does_exist == false && reader.Read())
                    {
                        does_exist = string.Compare(reader.GetString(name_index), index, true) == 0;
                    }
                }
            }
            catch (Exception)
            {
            }

            return does_exist;
        }

        public static bool DoesTableExist(this SQLiteConnection conn, string table)
        {
            bool does_exist = false;

            try
            {
                string query = string.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}';", table);
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read() && reader.HasRows)
                    {
                        int value = reader.GetInt32(0);
                        does_exist = value != 0;
                    }
                }
            }
            catch (Exception)
            {
            }

            return does_exist;
        }
    }
}
