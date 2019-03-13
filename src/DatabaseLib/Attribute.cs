using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Attributes are more loosy-goosy than configuration, so here let's just
    /// get and set attributes with a string path.
    /// </summary>
    public class Attribute
    {
        public Attribute()
        {
        }

        public void Set(string path, string value, SQLiteConnection conn)
        {
            ILog log = LogManager.GetLogger(typeof(Attribute));

            try
            {
                log.DebugFormat("Setting attribute '{0}' to '{1}'", path, value);

                using (SQLiteTransaction t = conn.BeginTransaction())
                {
                    Changer change = new Deleter("Attributes", $"Path = '{path}'", conn);
                    change.Execute();

                    change = new Inserter("Attributes", conn);
                    change.Set("Path", path, true);
                    change.Set("Value", value, false);
                    change.Execute();

                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void Set(string path, DateTimeOffset date, SQLiteConnection conn)
        {
            Set(path, date.ToString("o"), conn);
        }

        public void Clear(string path, SQLiteConnection conn)
        {
            ILog log = LogManager.GetLogger(typeof(Attribute));

            try
            {
                log.DebugFormat("Clearing attribute '{0}'", path);

                Changer change = new Deleter("Attributes", $"Path = '{path}'", conn);
                change.Execute();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public string Get(string path, SQLiteConnection conn)
        {
            string attribute = null;
            string sql = $"SELECT Value FROM Attributes WHERE Path = '{path}';";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read() && reader.IsDBNull(0) == false)
                        attribute = reader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Attribute));
                log.Error("Get: " + sql);
                log.Error(ex);
            }

            return attribute;
        }

        public Dictionary<string, string> GetMultiple(string path, SQLiteConnection conn)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            string sql = $"SELECT Path, Value FROM Attributes WHERE Path LIKE '%{path}';";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read() && reader.IsDBNull(1) == false)
                    {
                        string dbpath = reader.GetString(0);
                        string value = reader.GetString(1);

                        attributes[dbpath] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(Attribute));
                log.Error("GetMultiple: " + sql);
                log.Error(ex);
            }

            return attributes;
        }
    }
}
