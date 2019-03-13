using System.Data.SQLite;

namespace gov.sandia.sld.common.db.changers
{
    public class Deleter : Changer
    {
        public string Where { get; private set; }

        public override string Statement { get { return $"DELETE FROM {Table} WHERE {Where};"; } }

        public Deleter(string table, string where, SQLiteConnection conn)
            : base(table, conn)
        {
            Where = where;
        }
    }
}
