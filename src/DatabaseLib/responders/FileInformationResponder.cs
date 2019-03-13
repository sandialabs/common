using gov.sandia.sld.common.db.files;
using gov.sandia.sld.common.requestresponse;

namespace gov.sandia.sld.common.db.responders
{
    public class FileInformationResponder : Responder
    {
        public override void HandleRequest(Request request)
        {
            if(request is FileInformationRequest)
            {
                FileInformationRequest fi_request = request as FileInformationRequest;
                FileRetriever retriever = new FileRetriever(fi_request.Type, fi_request.Name);
                FileRecord frecord = retriever.Get();
                if (frecord != null)
                {
                    fi_request.ID = frecord.ID;
                    fi_request.Details = frecord.Details;
                }

                request.Handled();
            }
        }
    }
}
