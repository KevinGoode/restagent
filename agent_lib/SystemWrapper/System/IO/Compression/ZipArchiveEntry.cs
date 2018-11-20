using System;
using System.Collections.Generic;
using System.IO;
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
                            public class ZipArchiveEntryWrapper : IZipArchiveEntryWrapper
                            {
                                public string Name { get { return Entry.Name; } }
                                public IZipArchiveWrapper Archive { get { return new ZipArchiveWrapper(Entry.Archive); } }

                                protected ZipArchiveEntry Entry { get; set; }

                                public ZipArchiveEntryWrapper(ZipArchiveEntry entry)
                                {
                                    Entry = entry;
                                }

                                public Stream Open()
                                {
                                    return Entry.Open();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}