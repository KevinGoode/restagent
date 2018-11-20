using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http; 
using System.Text;
using System.Threading.Tasks;


namespace Kge
{
    namespace Agent
    {
        namespace Lang
        {
            /// <summary>
            /// This class is a limited copy of the contents of an HTTPContext
            /// object so that they may be stored beyond the lifespan of the original
            /// HTTPContext object.
            /// </summary>
            public class OperationContextFacsimile
            {
                public HttpRequest IncomingMessageRequest { get; protected set; } 
                public HttpResponse OutgoingMessageResponse { get; protected set; } 
                public IHeaderDictionary RequestHeaders { 
                    get { 
                        IHeaderDictionary headers = IncomingMessageRequest!=null ? IncomingMessageRequest.Headers: null; 
                        return headers;
                    }
                }
                public OperationContextFacsimile()
                {
                    IncomingMessageRequest=null;
                }

                public OperationContextFacsimile(HttpContext original)
                {
                    IncomingMessageRequest = original.Request;
                    OutgoingMessageResponse = original.Response;
                }
            }
        }
    }
}
