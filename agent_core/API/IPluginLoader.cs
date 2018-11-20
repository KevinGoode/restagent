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
                    public interface IPluginLoader
                    {
                        System.Collections.Generic.List<IPlugin> Discover(string path);
                        IPlugin DiscoverPlugin(string dll);
                    }
                }
            }
        }
    }
}