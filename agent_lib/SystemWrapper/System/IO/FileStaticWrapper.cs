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
                        public class FileStaticWrapper : IFileStaticWrapper
                        {
                            public FileStream OpenForReading(string path)
                            {
                                return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            }
                            public FileStream OpenForWriting(string path)
                            {
                                return File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                            }
                            public bool Exists(string name)
                            {
                                return File.Exists(name);
                            }

                            public string ReadAllText(string path)
                            {
                                return File.ReadAllText(path);
                            }

                            public void WriteAllText(string path, string contents)
                            {
                                File.WriteAllText(path, contents);
                            }

                            public void Move(string sourcePath, string destinationPath)
                            {
                                File.Move(sourcePath, destinationPath);
                            }

                            public void Copy(string sourcePath, string destinationPath)
                            {
                                File.Copy(sourcePath, destinationPath);
                            }

                            public void Delete(string path)
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }
            }
        }
    }
}