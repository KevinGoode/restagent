using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Appliance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Kge.Agent.Lang;
using Kge.Agent.Library;
using System.Runtime.InteropServices;
using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Rest.Server.NativePlugin;
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
                    public class AppliancePlugin : APluginImplementation
                    {
                        public AppliancePlugin()
                            : base(null) { }

                        public AppliancePlugin(QueryParametersContainer queryParameters)
                            : base(queryParameters) { }

                        internal static class SIDHelper
                        {
                            internal enum SID_NAME_USE
                            {
                                SidTypeUser = 1,
                                SidTypeGroup,
                                SidTypeDomain,
                                SidTypeAlias,
                                SidTypeWellKnownGroup,
                                SidTypeDeletedAccount,
                                SidTypeInvalid,
                                SidTypeUnknown,
                                SidTypeComputer
                            }

                            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                            private static extern bool LookupAccountName(
                                string machineName,
                                string accountName,
                                byte[] sid,
                                ref int sidLen,
                                StringBuilder domainName,
                                ref int domainNameLen,
                                out SID_NAME_USE peUse);

                            public static SecurityIdentifier LookupAccountName(
                                string systemName,
                                string accountName)
                            {
                                int sidLen = 0x400;
                                int domainLen = 0x400;
                                byte[] sid = new byte[sidLen];
                                StringBuilder domain = new StringBuilder(domainLen);
                                SID_NAME_USE use;

                                if (LookupAccountName(systemName, accountName, sid, ref sidLen,
                                    domain, ref domainLen, out use))
                                {
                                    return new SecurityIdentifier(sid, 0);
                                }

                                return null;
                            }
                        }

                        [RouteDescription(@"/appliance", "GET")]
                        public ApplianceResponse Appliance()
                        {
                            var reg = RestServerIoc.Resolve<ICommonRegistry>();

                            try
                            {
                                string localApplianceId = null;
                                try{
                                    var sid = SIDHelper.LookupAccountName(null, Environment.MachineName);
                                    localApplianceId = sid.Value;
                                }
                                catch{
                                    Log.Error("Error getting SID");
                                }
                                
                                var appliance = new ApplianceDetails()
                                {
                                    applianceId = localApplianceId,
                                    hostName = Environment.MachineName,
                                    domainName = "Domain-TODO",
                                    //windowsVersion = WindowsInfo.RetrieveWindowsVersion(),
                                    windowsVersion = "Not windows!",
                                    productVersion = reg.Version
                                };

                                List<Capability> capabilities = new List<Capability>();
                                var plugins = RootService.APIManager.GetPluginList();
                                foreach (var plugin in plugins)
                                {
                                    var pluginInstance = plugin.InstantiatePlugin(null);

                                    if (pluginInstance.Exposed)
                                    {
                                        var caps = pluginInstance.GetCapabilities();

                                        if (caps != null)
                                            capabilities.AddRange(caps);
                                    }
                                }


                                appliance.capabilities = capabilities.GroupBy(x => x.name).Select(group => group.First()).ToArray();

                                return new ApplianceResponse() { applianceDetails = appliance };
                            }
                            catch
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.InternalServerError,
                                    Helper.SetAppTypeInErrorMessage(  500,
                                    LanguageHelper.Resolve("CORE_ERROR_GET_SID_FAILED"))),
                                    HttpStatusCode.InternalServerError);
                            }
                        }

                        [RouteDescription(@"/plugins", "GET")]
                        public PluginsResponse Plugins()
                        {
                            List<PluginDetails> details = new List<PluginDetails>();
                            var plugins = RootService.APIManager.GetPluginList();
                            foreach (var plugin in plugins)
                            {
                                var pluginInstance = plugin.InstantiatePlugin(null);
                                if (pluginInstance.Exposed)
                                {
                                    details.Add(pluginInstance.GetPluginDetails());
                                }
                            }

                            details = details.Where(x => x != null).ToList();

                            return new PluginsResponse() { plugins = details.ToArray() };
                        }

                        [RouteDescription(@"/ping", "GET")]
                        public PingResponse Ping()
                        {
                            return new PingResponse();
                        }
                    }
                }
            }
        }
    }
}
