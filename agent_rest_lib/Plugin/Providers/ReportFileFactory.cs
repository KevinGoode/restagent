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
                        public class ReportFileFactory : IReportFileFactory
                        {
                            private IFileInfoFactory FileFactory { get; set; }
                            private IFileStaticWrapper FileStatic { get; set; }

                            public ReportFileFactory(IFileInfoFactory infoFactory, IFileStaticWrapper fileWrapper)
                            {
                                FileFactory = infoFactory;
                                FileStatic = fileWrapper;
                            }

                            public ReportFile Create(string filePath)
                            {
                                var info = FileFactory.Create(filePath);
                                var stream = FileStatic.OpenForReading(filePath);
                                return new ReportFile(info, stream);
                            }

                            public ReportFiles Create(IEnumerable<string> filePaths)
                            {
                                var reportCollection = new ReportFiles();

                                foreach (var path in filePaths)
                                {
                                    reportCollection.Add(Create(path));
                                }

                                return reportCollection;
                            }
                        }
                    }
                }
            }
        }
    }
}
