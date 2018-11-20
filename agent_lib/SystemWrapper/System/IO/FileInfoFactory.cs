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
                    namespace IO
                    {
                        [ExcludeFromCodeCoverage]
                        public class FileInfoFactory : IFileInfoFactory
                        {
                            public IFileInfoWrapper Create(string path)
                            {
                                return new FileInfoWrapper(path);
                            }
                        }
                    }
                }
            }
        }
    }
}
