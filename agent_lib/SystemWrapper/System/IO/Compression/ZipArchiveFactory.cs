using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
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
                        namespace Compression
                        {
                            [ExcludeFromCodeCoverage]
                            public class ZipArchiveFactory : IZipArchiveFactory
                            {
                                public ZipArchiveFactory() { }

                                public IZipArchiveWrapper Create(Stream stream)
                                {
                                    return new ZipArchiveWrapper(new ZipArchive(stream));
                                }

                                public IZipArchiveWrapper Create(Stream stream, ZipArchiveMode mode)
                                {
                                    return new ZipArchiveWrapper(new ZipArchive(stream, mode));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}