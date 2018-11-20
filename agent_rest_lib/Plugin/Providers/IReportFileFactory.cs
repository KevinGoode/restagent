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
                        public interface IReportFileFactory
                        {
                            ReportFile Create(string filePath);
                            ReportFiles Create(IEnumerable<string> filePaths);
                        }
                    }
                }
            }
        }
    }
}
