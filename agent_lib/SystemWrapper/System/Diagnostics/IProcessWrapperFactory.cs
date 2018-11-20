using System;
using System.Collections.Generic;
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
                    namespace Diagnostics
                    {
                        public interface IProcessWrapperFactory
                        {
                            IProcessWrapper MakeProcess(string executablePath, string parameters);
                        }
                    }
                }
            }
        }
    }
}
