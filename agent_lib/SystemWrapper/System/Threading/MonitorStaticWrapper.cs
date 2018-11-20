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
                        public class MonitorStaticWrapper
                        {
                            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Exit")]
                            public virtual void Exit(object obj)
                            {
                                Monitor.Exit(obj);
                            }

                            public virtual bool TryEnter(object obj)
                            {
                                return Monitor.TryEnter(obj);
                            }
                        }
                    }
                }
            }
        }
    }
}