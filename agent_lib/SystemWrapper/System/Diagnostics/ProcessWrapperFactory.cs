using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library.SystemWrapper.System.IO;

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
                    namespace Diagnostics
                    {
                        public class ProcessWrapperFactory : IProcessWrapperFactory
                        {
                            public IProcessWrapper MakeProcess(string executablePath, string parameters)
                            {
                                return new ProcessWrapper(executablePath, parameters);
                            }
                        }
                    }
                }
            }
        }
    }
}
