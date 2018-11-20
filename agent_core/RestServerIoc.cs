using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Kge.Agent.Rest.Library;
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
                public static class RestServerIoc
                {
                    static RestServerIoc()
                    {
                        Reset();
                    }

                    public static IContainer Container { get; private set; }

                    public static void Reset()
                    {
                        Container = Initialise(RestLibraryIoc.Container);
                    }

                    public static T Resolve<T>()
                    {
                        return Container.Resolve<T>();
                    }

                    private static IContainer Initialise(ContainerBuilder initialisingContainer)
                    {
                        initialisingContainer.RegisterType<CoreTicketGenerator>().As<IDetachedReportFilesProvider>().PropertiesAutowired();
                        
                        //Nothing else should depend on this component (it is an executable) so can now build
                        return initialisingContainer.Build();
                    }
                }
            }
        }
    }
}
