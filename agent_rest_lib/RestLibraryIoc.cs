using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library;
using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Autofac;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                public static class RestLibraryIoc
                {
                    static RestLibraryIoc()
                    {
                        Container = Initialise(LibraryIoc.Container);
                    }

                    public static ContainerBuilder Container { get; private set; }

                    public static void Reset()
                    {
                    
                        Container = Initialise(LibraryIoc.Container);
                    }

                    private static ContainerBuilder Initialise(ContainerBuilder Container)
                    {
                        Container.RegisterType<WebAuthentication>().As<IWebAuthentication>();

                        Container.RegisterType<ReportFileFactory>().As<IReportFileFactory>();
                        

                        return Container;
                    }
                }
            }
        }
    }
}
