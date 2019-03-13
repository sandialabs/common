using gov.sandia.sld.common.requestresponse;
using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.responders
{
    /// <summary>
    /// Get DB type and DB connection string for the device specified in the request.
    /// </summary>
    public class DatabaseInfoResponder : Responder
    {
        public override void HandleRequest(Request request)
        {
            if(request is DatabaseTypeRequest)
            {
                DatabaseTypeRequest db_request = request as DatabaseTypeRequest;

                DatabaseConnectionString db_conn_string = new DatabaseConnectionString(db_request.DeviceName);
                DatabaseType db_type = new DatabaseType(db_request.DeviceName);

                try
                {
                    Database db = new Database();
                    using (SQLiteConnection conn = db.Connection)
                    {
                        conn.Open();

                        db_request.ConnectionString = db_conn_string.GetValue(conn);
                        db_request.DatabaseType = db_type.GetAsDatabaseType(conn);

                        db_request.Handled();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
