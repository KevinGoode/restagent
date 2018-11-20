using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
                        public class FileInfoWrapper : IFileInfoWrapper
                        {
                            public string Name { get { return Info.Name; } }
                            public long Length { get { return Info.Length; } }

                            protected FileInfo Info { get; set; }

                            public FileInfoWrapper(string path)
                            {
                                Info = new FileInfo(path);
                            }

                            public void Delete()
                            {
                                Info.Delete();
                            }
                        }
                    }
                }
            }
        }
    }
}
