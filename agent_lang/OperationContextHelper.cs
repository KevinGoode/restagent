using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            public static class OperationContextHelper
            {
                public static void SaveContext(HttpContext context) 
                {
                    SaveContext(new OperationContextFacsimile(context));
                }

                public static void SaveContext(OperationContextFacsimile context)
                {
                    Thread.SetData(Thread.GetNamedDataSlot("HttpContext"), context);
                }

                public static OperationContextFacsimile RetrieveContext()
                {
                    return Thread.GetData(Thread.GetNamedDataSlot("HttpContext")) as OperationContextFacsimile;
                }
            }
        }
    }
}