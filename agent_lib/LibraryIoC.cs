using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library.SystemWrapper.DllImport;
using Kge.Agent.Library.SystemWrapper.Reflection;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Kge.Agent.Library.SystemWrapper.System.IO;
using Kge.Agent.Library.SystemWrapper.System.Net;
using Kge.Agent.Library.SystemWrapper.System.Threading;
using Kge.Agent.Library.SystemWrapper.System;

using Autofac;
using System.Net.Http;
using Kge.Agent.Library.SystemWrapper.System.IO.Compression;


namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public static class LibraryIoc
            {
                static LibraryIoc()
                {
                    Container = Initialise(new ContainerBuilder());
                }

                public static ContainerBuilder Container { get; private set; }

                public static void Reset()
                {
                    
                    Container = Initialise(new ContainerBuilder());
                }

                private static ContainerBuilder Initialise(ContainerBuilder container)
                {
                    {// LibraryWrapper
/* 
                        { // DllImport
                            container.RegisterType<advapi32Wrapper>();
                            container.RegisterType<kernel32Wrapper>();
                        }

            */           

                        { // Reflection
                            container.RegisterType<AssemblyStaticWrapper>().As<IAssemblyStaticWrapper>();
                        }

                        { // System

                            { // Diagnostics
                                container.RegisterType<ProcessWrapperFactory>().As<IProcessWrapperFactory>();
                            }

                            { // IO

                                { // Compression
                                    container.RegisterType<ZipArchiveFactory>().As<IZipArchiveFactory>();
                                }

                                container.RegisterType<FileInfoFactory>().As<IFileInfoFactory>();
                                container.RegisterType<DirectoryStaticWrapper>().As<IDirectoryStaticWrapper>();
                                container.RegisterType<FileStaticWrapper>().As<IFileStaticWrapper>();
                            }

                            
                           
                            { // Threading
                                container.RegisterType<ThreadStaticWrapper>();
                            }

                            container.RegisterType<ConsoleStaticWrapper>().As<IConsoleStaticWrapper>();
                            container.RegisterType<EnvironmentStaticWrapper>().As<IEnvironmentStaticWrapper>();
                        }
                    }

                    

                    container.RegisterType<CommonRegistry>().As<ICommonRegistry>();
                    
                    return container;
                }
            }
        }
    }
}
