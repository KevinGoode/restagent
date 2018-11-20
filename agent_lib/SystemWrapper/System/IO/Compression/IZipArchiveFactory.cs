using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
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
                            public interface IZipArchiveFactory
                            {
                                IZipArchiveWrapper Create(Stream stream);
                                IZipArchiveWrapper Create(Stream stream, ZipArchiveMode mode);
                            }
                        }
                    }
                }
            }
        }
    }
}