using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Diagnostics.CodeAnalysis;

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
                            public class ZipArchiveWrapper : IZipArchiveWrapper
                            {
                                protected ZipArchive Archive { get; set; }

                                public ZipArchiveWrapper(ZipArchive archive)
                                {
                                    Archive = archive;
                                }

                                public IZipArchiveEntryWrapper CreateEntry(string entryName)
                                {
                                    return new ZipArchiveEntryWrapper(Archive.CreateEntry(entryName));
                                }

                                public void Dispose()
                                {
                                    Dispose(true);
                                }

                                protected virtual void Dispose(bool disposing)
                                {
                                    if (disposing)
                                    {
                                        Archive.Dispose();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}