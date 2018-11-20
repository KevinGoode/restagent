using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                    public class Plugin : IPlugin
                    {
                        public Assembly Assembly { get { return assembly; } }
                        public Type Type { get { return type; } }
                        public List<PluginAPI> PluginAPIs { get { return pluginAPIs; } }
                        public string Name { get { return name; } }

                        protected Assembly assembly;
                        protected Type type;
                        protected List<PluginAPI> pluginAPIs;
                        protected string name;

                        public Plugin(Assembly assembly, Type type, List<PluginAPI> pluginAPIs)
                        {
                            this.assembly = assembly;
                            this.type = type;
                            this.pluginAPIs = pluginAPIs;

                            this.name = type.Name;
                        }

                        public Plugin(Type type, List<PluginAPI> pluginAPIs)
                        {
                            this.type = type;
                            this.pluginAPIs = pluginAPIs;

                            this.name = type.Name;
                        }

                        /// <summary>
                        /// Instantiates and returns the plugin execution class.
                        /// </summary>
                        /// <returns></returns>
                        public APluginImplementation InstantiatePlugin(QueryParametersContainer queryParameters)
                        {
                            return (APluginImplementation)Activator.CreateInstance(Type, queryParameters);
                        }

                    }
                }
            }
        }
    }
}