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
                    public class SynchronousProgress<T> : IProgress<T>
                    {
                        protected Action<T> Handler { get; set; }

                        public SynchronousProgress(Action<T> handler)
                        {
                            Handler = handler;
                        }

                        public void Report(T value)
                        {
                            Handler.Invoke(value);
                        }
                    }
                }
            }
        }
    }
}