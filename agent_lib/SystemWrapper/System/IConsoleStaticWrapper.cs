using System;
using System.Text;

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
                    public interface IConsoleStaticWrapper
                    {
                        void WriteLine(string value);
                        Encoding OutputEncoding { get; set; }
                    }
                }
            }
        }
    }
}