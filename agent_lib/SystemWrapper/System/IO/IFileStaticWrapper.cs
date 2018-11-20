using System;
using System.IO;

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
                        public interface IFileStaticWrapper
                        {
                            FileStream OpenForReading(string path);
                            FileStream OpenForWriting(string path);
                            bool Exists(string name);
                            string ReadAllText(string path);
                            void WriteAllText(string path, string contents);
                            void Move(string sourcePath, string destinationPath);
                            void Copy(string sourcePath, string destinationPath);
                            void Delete(string path);
                        }
                    }
                }
            }
        }
    }
}
