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
                        public class DirectoryStaticWrapper : IDirectoryStaticWrapper
                        {
                            public string[] GetFiles(string path)
                            {
                                return Directory.GetFiles(path);
                            }

                            public string[] GetFiles(string path, string searchPattern)
                            {
                                return Directory.GetFiles(path, searchPattern);
                            }

                            public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
                            {
                                return Directory.GetFiles(path, searchPattern, searchOption);
                            }


                            public IEnumerable<string> EnumerateFiles(string path)
                            {
                                return Directory.EnumerateFiles(path);
                            }

                            public IEnumerable<string> EnumerateFiles(string path, string pattern)
                            {
                                return Directory.EnumerateFiles(path, pattern);
                            }

                            public string[] GetSubdirectories(string path)
                            {
                                return Directory.GetDirectories(path);
                            }

                            public bool Exists(string name)
                            {
                                return Directory.Exists(name);
                            }

                            public void CreateDirectory(string path)
                            {
                                Directory.CreateDirectory(path);
                            }

                            public void Delete(string path)
                            {
                                Directory.Delete(path);
                            }
                        }
                    }
                }
            }
        }
    }
}