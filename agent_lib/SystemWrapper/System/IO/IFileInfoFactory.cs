using Kge.Agent.Library.SystemWrapper.System.IO;
using System;

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
                    namespace IO
                    {
                        public interface IFileInfoFactory
                        {
                            IFileInfoWrapper Create(string path);
                        }
                    }
                }
            }
        }
    }
}