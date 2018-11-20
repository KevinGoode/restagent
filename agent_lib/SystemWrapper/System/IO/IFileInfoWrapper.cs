using System;

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
                        public interface IFileInfoWrapper
                        {
                            long Length { get; }
                            string Name { get; }

                            void Delete();
                        }
                    }
                }
            }
        }
    }
}