using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
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
                    public interface IPlugin
                    {
                        System.Reflection.Assembly Assembly { get; }
                        APluginImplementation InstantiatePlugin(QueryParametersContainer queryParameters);
                        string Name { get; }
                        System.Collections.Generic.List<PluginAPI> PluginAPIs { get; }
                        Type Type { get; }
                    }
                }
            }
        }
    }
}
