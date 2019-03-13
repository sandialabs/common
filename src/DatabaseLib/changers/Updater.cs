using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.changers
{
    public class Updater : Changer
    {
        public string Where { get; private set; }

        public override string Statement
        {
            get
            {
                string sets = m_changes.JoinStrings(c => $"{c.Column} = @{c.Column}", ", ");
                string where = string.IsNullOrEmpty(Where) ? "" : "WHERE " + Where;
                string statement = $"UPDATE {Table} SET {sets} {where};";

                return statement;
            }
        }

        public Updater(string table, string where, SQLiteConnection conn)
            : base(table, conn)
        {
            Where = where;
        }

        public Updater(string table, SQLiteConnection conn)
            : base(table, conn)
        {
            Where = null;
        }

        public override void Execute()
        {
            ExecuteWithParameters();
        }
    }
}
