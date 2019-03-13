using gov.sandia.sld.common.data;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db
{
    public interface IDataInterpreter
    {
        void Interpret(Data d, SQLiteConnection conn);
    }
}
