using gov.sandia.sld.common.requestresponse;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.responders
{
    public class AttributeResponder : Responder
    {
        public override void HandleRequest(Request request)
        {
            if(request is AttributeRequest)
            {
                AttributeRequest attr_request = request as AttributeRequest;
                Database db = new Database();
                Attribute attr = new Attribute();

                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    if (attr_request.Get)
                    {
                        string value = attr.Get(attr_request.Path, conn);
                        if (string.IsNullOrEmpty(value) == false)
                        {
                            attr_request.Value = value;
                            request.Handled();
                        }
                    }
                    else
                    {
                        attr.Set(attr_request.Path, attr_request.Value, conn);
                        request.Handled();
                    }
                }
            }
        }
    }
}
