using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

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
                    public delegate void RouteHandler(IAPIRouter sender, IPlugin plugin, MethodInfo method);

                    public interface IAPIRouter
                    {
                        #region Events

                        event RouteHandler OnRouteRetrieved;

                        #endregion

                        void AddPlugin(IPlugin plugin);
                        void AddRoute(APIRouteDescription ard, IPlugin instance, MethodInfo methodInfo);
                        void Clear();
                        bool ContainsPlugin(IPlugin plugin);
                        bool ContainsRoute(APIRouteDescription ard);
                        ResolvedRoute GetRoute(string uriTemplate, string queryMethod);
                        int PluginCount { get; }
                        void RemovePlugin(IPlugin plugin);
                        void RemoveRoute(APIRouteDescription ard);
                        int RouteCount { get; }
                    }
                }
            }
        }
    }
}