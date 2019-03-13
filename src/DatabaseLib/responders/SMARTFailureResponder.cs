using gov.sandia.sld.common.requestresponse;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.responders
{
    // This was testing code to simulate a SMART failure. We obviously
    // don't want this in normal operation.
    //public class SMARTFailureResponder : Responder
    //{
    //    public override void HandleRequest(Request request)
    //    {
    //        if(request is SMARTFailureRequest)
    //        {
    //            Database db = new Database();
    //            Attribute a = new Attribute();
    //            using (SQLiteConnection conn = db.Connection)
    //            {
    //                conn.Open();
    //                string value = a.Get("debug.smart.failure", conn);
    //                if (string.IsNullOrEmpty(value) == false)
    //                {
    //                    (request as SMARTFailureRequest).FailureIsPredicted = value == "1";
    //                    request.Handled();
    //                }
    //            }
    //        }
    //    }
    //}
}
