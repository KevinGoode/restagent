using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Rest.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Kge.Agent.Library.SystemWrapper.System.IO;
using System.Net;
using Kge.Agent.Library;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace NativePlugin
                {
                    internal interface IDetachedReportFilesProvider : IReportFilesProvider
                    { }
                }
            }
        }
    }
}
