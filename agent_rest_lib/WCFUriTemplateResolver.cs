using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Lang;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public class WCFUriTemplateResolver
                {
                    /// <summary>
                    /// Resolves an url to get its current target.
                    /// <para>(Example: 127.0.0.1:80/test, this'll return /test)</para>
                    /// </summary>
                    /// <returns></returns>
                    public virtual string ResolveUriTemplate()
                    {
                        string path = OperationContextHelper.RetrieveContext().IncomingMessageRequest.Path;
                        string query = OperationContextHelper.RetrieveContext().IncomingMessageRequest.QueryString.Value;
                        string uriTemplate = path + query;
                        // Make sure there is a slash
                        if (uriTemplate.First() != '/')
                            uriTemplate = uriTemplate.Insert(0, "/");

                        return uriTemplate;
                    }
                }
            }
        }
    }
}
