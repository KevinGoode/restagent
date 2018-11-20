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
                        namespace Compression
                        {
                            public interface IZipArchiveEntryWrapper
                            {
                                string Name { get; }
                                IZipArchiveWrapper Archive { get; }

                                Stream Open();
                            }
                        }
                    }
                }
            }
        }
    }
}