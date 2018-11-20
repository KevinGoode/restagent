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
                        public interface IDirectoryStaticWrapper
                        {
                            string[] GetFiles(string path);
                            string[] GetFiles(string path, string searchPattern);
                            string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
                            string[] GetSubdirectories(string path);
                            IEnumerable<string> EnumerateFiles(string path);
                            IEnumerable<string> EnumerateFiles(string path, string pattern);
                            bool Exists(string name);
                            void CreateDirectory(string path);
                            void Delete(string path);
                        }
                    }
                }
            }
        }
    }
}