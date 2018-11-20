using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    public class PluginLoader : IPluginLoader
                    {
                        /// <summary>
                        /// Discovers a whole directory of dynamic libraries to retrieve plugins
                        /// </summary>
                        /// <param name="path">Path to the directory</param>
                        /// <returns>List of successfully loaded plugins</returns>
                        public List<IPlugin> Discover(string path)
                        {
                            var listPlugins = new List<IPlugin>();
                            var files = Directory.GetFiles(path, "*.dll");

                            foreach (string dll in files)
                            {
                                IPlugin newPlugin = DiscoverPlugin(dll);

                                if (newPlugin != null)
                                    listPlugins.Add(newPlugin);
                            }

                            return listPlugins;
                        }

                        /// <summary>
                        /// Discovers only one specific dynamic library
                        /// </summary>
                        /// <param name="dll">Path to the library file</param>
                        /// <returns>Return the successfully loaded plugin, if not returns null</returns>
                        public IPlugin DiscoverPlugin(string dll)
                        {
                            try
                            {
                                var assembly = Assembly.LoadFile(dll);
                                string assemblyName  = assembly.GetName().Name;
                                var exportedTypes = assembly.ExportedTypes;
                                Type pluginType = exportedTypes.FirstOrDefault(x => x.Name.Equals(assemblyName));
                                if (pluginType != null)
                                {
                                    APluginImplementation pluginDll = (APluginImplementation)Activator.CreateInstance(pluginType);

                                    var list = pluginDll.GetPluginMethodsMetaData();
                                    Log.Info("Loaded plugin: " + assembly.GetName().Name);
                                    return new Plugin(assembly, pluginType, list);
                                }
                                else
                                {
                                    Log.Debug("Did not recognise dll as plugin: " + dll);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("Failed to load dll: " + dll +  " because " + ex.Message);
                            }

                            return null;
                        }
                    }
                }
            }
        }
    }
}