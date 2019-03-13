using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.changers
{
    public abstract class Changer
    {
        public string Table { get; private set; }
        public abstract string Statement { get; }

        public Changer(string table, SQLiteConnection conn)
        {
            Table = table;
            m_changes = new List<Data>();
            m_conn = conn;
            m_log = LogManager.GetLogger(typeof(Changer));
        }

        public virtual void Execute()
        {
            string statement = Statement;

            m_log.Debug("Executing: " + statement);
            m_conn.ExecuteNonQuery(statement);
        }

        public void Set(string column, string value, bool normalize, bool make_empty_null = true)
        {
            if(normalize && string.IsNullOrEmpty(value) == false)
                value = value.Replace("'", "").Replace("\"", "");
            Data d = new Data { Column = column };
            if (value == null || (string.IsNullOrEmpty(value) && make_empty_null))
                d.Value = DBNull.Value;
            else
                d.Value = value;


            m_changes.Add(d);
        }

        public void Set(string column, int value)
        {
            Set(column, (long)value);
        }

        public void Set(string column, long value)
        {
            m_changes.Add(new Data() { Column = column, Value = value });
        }

        public void Set(string column, DateTimeOffset dt)
        {
            Set(column, dt.ToString("o"), false);
        }

        /// <summary>
        /// Set a bool value in the DB
        /// 
        /// It will store true as 1 and false as 0, or true as "true" and false as "false"
        /// depending upon the as_string param
        /// </summary>
        /// <param name="column">The column to store data in</param>
        /// <param name="b">The boolean value</param>
        /// <param name="as_string">true if it should be stored as the strings "true" or "false"; false
        /// if it should be 1 for true or 0 for false</param>
        public void Set(string column, bool b, bool as_string = false)
        {
            if (as_string)
                Set(column, b ? "true" : "false", false);
            else
                Set(column, b ? 1L : 0L);
        }

        public void SetNull(string column)
        {
            Set(column, null, false, true);
        }

        protected void ExecuteWithParameters()
        {
            string statement = Statement;
            m_log.Debug($"Executing: {statement}");

            using (SQLiteCommand command = new SQLiteCommand(statement, m_conn))
            {
                foreach (Data d in m_changes)
                    command.Parameters.AddWithValue($"@{d.Column}", d.Value);
                command.ExecuteNonQuery();
            }
        }

        protected class Data
        {
            public string Column { get; set; }
            public object Value { get; set; }
        }

        protected List<Data> m_changes;
        protected SQLiteConnection m_conn;
        protected ILog m_log;
    }
}
