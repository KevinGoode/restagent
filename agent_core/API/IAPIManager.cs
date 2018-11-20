using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace API
                {
                    public interface IAPIManager
                    {
                        IAPIRouter APIRouter { get; set; }
                        void AddPlugin(IPlugin plugin);
                        IGenericResponse CallGetAPI(string uriTemplate, string method);
                        IGenericResponse CallPostAPI(string uriTemplate, string method, string jsonRequest, IWCFSerializer serializer);
                        void DiscoverAPI(string path);
                        void DiscoverAPIDirectory(string path);
                        System.Collections.Generic.List<IPlugin> GetPluginList();
                        void RemovePlugin(IPlugin plugin);
                    }
                }
            }
        }
    }
}