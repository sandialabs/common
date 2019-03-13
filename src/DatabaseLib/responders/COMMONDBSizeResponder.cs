using gov.sandia.sld.common.requestresponse;
using System;

namespace gov.sandia.sld.common.db.responders
{
    public class COMMONDBSizeResponder : Responder
    {
        public override void HandleRequest(Request request)
        {
            if (request is COMMONDBSizeRequest)
            {
                COMMONDBSizeRequest db_request = request as COMMONDBSizeRequest;
                Database db = new Database();
                COMMONDatabaseInfo dbinfo = new COMMONDatabaseInfo(db);

                Tuple<string, uint> info = dbinfo.GetDatabaseInformation();
                if(info != null)
                {
                    db_request.Filename = info.Item1;
                    db_request.SizeInMB = info.Item2;

                    request.Handled();
                }
            }
        }
    }
}
