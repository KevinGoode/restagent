/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: WebExceptions.cs

  DESCRIPTION: 
  ------------
  This file contains Web Service Exception method

  USAGE INSTRUCTIONS:
  -------------------
  new WebResponseException(<HttpsErrorResponse> message, <System.Net.HttpStatusCode> responsecode)
  
  CHANGE HISTORY:
  --------------- 
  07/05/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using Kge.Agent.Rest.Library.Plugin.DataContracts;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /* User defined exceptions for web exceptions to be thrown to client */
                [Serializable]
                public class WebResponseException : Exception
                {
                    public WebResponseException(HttpsErrorResponse message, System.Net.HttpStatusCode responsecode)
                    :base(message.Error.message)
                    {
                        ErrorMessage = message;
                        responseCode= responsecode;
                    }
                   public  HttpsErrorResponse ErrorMessage{get;}
                   public System.Net.HttpStatusCode responseCode{get;}
                }

                /* Common response body inacase any errors occured */
                [DataContract]
                public class HttpsErrorResponse : IGenericResponse
                {
                    [DataMember]
                    public ErrorDetail Error { get; set; }
                    public HttpsErrorResponse()
                    {
                        
                    }
                    public HttpsErrorResponse(HttpStatusCode code, string message)
                    {
                        Error = new ErrorDetail();
                        Error.code = code;
                        Error.message = message;
                    }

                    public HttpsErrorResponse(HttpStatusCode code, string message, IEnumerable<string> additionalMessages)
                    {
                        Error = new ErrorDetail();
                        Error.code = code;
                        Error.message = message;
                        Error.additionalMessages = additionalMessages;
                    }
                }

                /* Error response details*/
                [DataContract]
                public class ErrorDetail
                {
                    [DataMember(Order = 1)]
                    public HttpStatusCode code { get; set; }

                    [DataMember(Order = 2)]
                    public string message { get; set; }

                    [DataMember(Order = 3)]
                    public IEnumerable<string> additionalMessages { get; set; }
                }
            }
        }
    }
}
