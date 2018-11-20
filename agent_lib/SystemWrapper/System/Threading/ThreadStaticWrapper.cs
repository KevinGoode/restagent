using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
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
                    namespace Threading
                    {
                        [ExcludeFromCodeCoverage]
                        public class ThreadStaticWrapper
                        {
                            public virtual void Sleep(int millisecondsTimeout)
                            {
                                Thread.Sleep(millisecondsTimeout);
                            }

                            public virtual void Sleep(TimeSpan timeout)
                            {
                                Thread.Sleep(timeout);
                            }
                        }
                    }
                }
            }
        }
    }
}
