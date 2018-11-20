using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        public interface IReportFilesProvider
                        {
                            Task<ReportFiles> ProvideReportFiles();
                        }
                    }
                }
            }
        }
    }
}