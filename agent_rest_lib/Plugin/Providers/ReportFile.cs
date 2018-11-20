using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kge.Agent.Library.SystemWrapper.System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                namespace Plugin
                {
                    namespace Providers
                    {
                        public class ReportFiles : IEnumerable<ReportFile>
                        {
                            private List<ReportFile> FilesList { get; set; }

                            public ReportFiles()
                            {
                                FilesList = new List<ReportFile>();
                            }

                            public void Add(ReportFile file)
                            {
                                FilesList.Add(file);
                            }

                            [ExcludeFromCodeCoverage]
                            public IEnumerator<ReportFile> GetEnumerator()
                            {
                                return FilesList.GetEnumerator();
                            }

                            [ExcludeFromCodeCoverage]
                            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                            {
                                return GetEnumerator();
                            }
                        }

                        public class ReportFile
                        {
                            public IFileInfoWrapper Info { get; private set; }
                            public Stream Stream { get; private set; }
                            public bool DeleteAfterRead { get; set; }

                            public ReportFile(IFileInfoWrapper info, Stream stream)
                            {
                                Info = info;
                                Stream = stream;
                                DeleteAfterRead = false;
                            }
                        }
                    }
                }
            }
        }
    }
}