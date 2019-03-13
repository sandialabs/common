using gov.sandia.sld.common.utilities;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.changers
{
    public class Inserter : Changer
    {
        public override string Statement
        {
            get
            {
                string columns = m_changes.JoinStrings(c => c.Column, ", ");
                string values = m_changes.JoinStrings(c => $"@{c.Column}", ", ");
                string statement = $"INSERT INTO {Table} ({columns}) VALUES ({values});";

                return statement;
            }
        }

        public Inserter(string table, SQLiteConnection conn)
            : base(table, conn)
        {
        }

        public override void Execute()
        {
            ExecuteWithParameters();
        }
    }
}
