using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http; 
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Lang
        {
            public static class LanguageHelper
            {
                public static string Resolve(string msgId, params object[] args)
                {
                    // Gets HttpContext from the Thread itself (in case of async task)
                    OperationContextFacsimile context = OperationContextHelper.RetrieveContext();

                    if (context == null || context.RequestHeaders== null)
                        return Messages.GetMessage(msgId, args);

                    var ret = context.RequestHeaders["Accept-Language"];
                    if (String.IsNullOrWhiteSpace(ret))
                        return Messages.GetMessage(msgId, args);

                    return Messages.GetLocalisedMessage(new LocalMessage(ret, msgId), args);
                }
            }
        }
    }
}
