using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
                namespace System
                {
                    [ExcludeFromCodeCoverage]
                    public class EnvironmentStaticWrapper : IEnvironmentStaticWrapper
                    {
                        public string MachineName { get { return Environment.MachineName; } }
                    }
                }
            }
        }
    }
}