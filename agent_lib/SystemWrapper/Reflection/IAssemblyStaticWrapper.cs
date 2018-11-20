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
                    public interface IAssemblyStaticWrapper
                    {
                        Assembly LoadFile(string path);

                        Assembly LoadFrom(string assemblyFile);

                        Assembly LoadFrom(string assemblyFile, byte[] hashValue, assemblies.AssemblyHashAlgorithm hashAlgorithm);

                        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
                        Assembly GetExecutingAssembly();
                    }
                }
            }
        }
    }
}