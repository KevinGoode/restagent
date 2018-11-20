using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
                        public interface IProcessWrapper : IDisposable
                        {
                            bool Running { get; }
                            IReadOnlyList<string> Output { get; }

                            ProcessStartInfo StartInfo { get; }

                            void Start();
                            bool WaitForExit(TimeSpan maximumDuration);
                        }
                    }
                }
            }
        }
    }
}
