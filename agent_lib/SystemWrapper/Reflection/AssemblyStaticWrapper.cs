using System;
using System.Collections.Generic;
using assemblies=System.Configuration.Assemblies;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            namespace SystemWrapper
            {
                namespace Reflection
                {
                    [ExcludeFromCodeCoverage]
                    [SecuritySafeCritical]
                    public class AssemblyStaticWrapper : IAssemblyStaticWrapper
                    {
                        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
                        public Assembly LoadFile(string path)
                        {
                            return Assembly.LoadFile(path);
                        }

                        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
                        public Assembly LoadFrom(string assemblyFile)
                        {
                            return Assembly.LoadFrom(assemblyFile);
                        }

                        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
                        public Assembly LoadFrom(string assemblyFile, byte[] hashValue, assemblies.AssemblyHashAlgorithm hashAlgorithm)
                        {
                            return Assembly.LoadFrom(assemblyFile, hashValue, hashAlgorithm);
                        }

                        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.GetExecutingAssembly")]
                        public Assembly GetExecutingAssembly()
                        {
                            return Assembly.GetExecutingAssembly();
                        }
                    }
                }
            }
        }
    }
}