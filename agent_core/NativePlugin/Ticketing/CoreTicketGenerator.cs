using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Rest.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kge.Agent.Library.SystemWrapper.Reflection;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Kge.Agent.Library.SystemWrapper.System.IO;
using System.Net;
using Kge.Agent.Library;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace NativePlugin
                {
                    public class CoreTicketGenerator : IDetachedReportFilesProvider
                    {
                        private const string REPORT_DIR = @"goodies\report";
                        private const string SCRIPT_FILE = @"goodies\core-ticket.cmd";

                        private IProcessWrapperFactory ProcessFactory { get; set; }
                        private ICommonRegistry RegistryValues { get; set; }
                        private IFileInfoFactory FileFactory { get; set; }
                        private IFileStaticWrapper FileStatic { get; set; }
                        private IDirectoryStaticWrapper DirectoryStatic { get; set; }
                        private IAssemblyStaticWrapper AssemblyStatic { get; set; }

                        public CoreTicketGenerator(
                            IProcessWrapperFactory procFactory,
                            ICommonRegistry registryInterface,
                            IFileInfoFactory fileInfoFactory,
                            IFileStaticWrapper fileWrapper,
                            IDirectoryStaticWrapper directoryWrapper,
                            IAssemblyStaticWrapper assemblyWrapper)
                        {
                            ProcessFactory = procFactory;
                            RegistryValues = registryInterface;
                            FileFactory = fileInfoFactory;
                            FileStatic = fileWrapper;
                            DirectoryStatic = directoryWrapper;
                            AssemblyStatic = assemblyWrapper;
                        }

                        private IEnumerable<string> IdentifyInstallLocations()
                        {
                            var possibleInstallDirectories = new HashSet<string>();

                            if (RegistryValues.Exists && (RegistryValues.InstallPath != null))
                            {
                                possibleInstallDirectories.Add(RegistryValues.InstallPath);
                            }

                            var assemblyDirectory = Path.GetDirectoryName(AssemblyStatic.GetExecutingAssembly().Location);
                            if (!possibleInstallDirectories.Contains(assemblyDirectory))
                            {
                                possibleInstallDirectories.Add(assemblyDirectory);
                            }

                            return possibleInstallDirectories;
                        }

                        private IEnumerable<string> LocateReportDirectories(string reportRoot)
                        {
                            var searchQueue = new Queue<string>();
                            searchQueue.Enqueue(reportRoot);
                            var reportDirectories = new List<string>();

                            while (searchQueue.Count > 0)
                            {
                                var searchDirectory = searchQueue.Dequeue();

                                if (DirectoryStatic.Exists(searchDirectory))
                                {
                                    reportDirectories.Add(searchDirectory);
                                    foreach (var subdirectory in DirectoryStatic.GetSubdirectories(searchDirectory))
                                    {
                                        searchQueue.Enqueue(subdirectory);
                                    }
                                }
                            }

                            return reportDirectories;
                        }

                        private ReportFile MarshalReportFile(string reportDirectoryPath, string filename)
                        {
                            // Retry every 5 seconds, 5 times.
                            return Retry.Do<ReportFile>(() =>
                            {
                                Log.Info(string.Format("Trying to gather report file {0} ...", filename));

                                string reportFilePath = Path.Combine(reportDirectoryPath, filename);

                                IFileInfoWrapper fileInfo = FileFactory.Create(reportFilePath);
                                Stream fileStream = FileStatic.OpenForReading(reportFilePath);

                                return new ReportFile(fileInfo, fileStream)
                                {
                                    DeleteAfterRead = true
                                };

                            }, TimeSpan.FromSeconds(5), 5);
                        }

                        public ReportFiles GenerateReportFiles()
                        {
                            ReportFiles reportFileCollection = new ReportFiles();

                            string reportScriptPath;
                            string foundInstallPath;

                            var possibleInstallEnumerator = IdentifyInstallLocations().GetEnumerator();
                            do
                            {
                                possibleInstallEnumerator.MoveNext();
                                foundInstallPath = possibleInstallEnumerator.Current;
                                if (foundInstallPath == null)
                                {
                                    Log.Error(string.Format("Could not find core ticketing script file"));
                                    return reportFileCollection;
                                }

                                reportScriptPath = Path.Combine(foundInstallPath, SCRIPT_FILE);
                            } while (!FileStatic.Exists(reportScriptPath));

                            string cmdParameters = string.Format("/C \"{0}\"", reportScriptPath);

                            using (var scriptProcess = ProcessFactory.MakeProcess("cmd.exe", cmdParameters))
                            {
                                scriptProcess.Start();

                                Log.Info("Started core ticket generation");

                                if (scriptProcess.WaitForExit(new TimeSpan(0, 15, 0)))
                                {
                                    string reportRoot = Path.Combine(foundInstallPath, REPORT_DIR);
                                    foreach (var reportDirectory in LocateReportDirectories(reportRoot))
                                    {
                                        foreach (string filename in DirectoryStatic.GetFiles(reportDirectory))
                                        {
                                            Log.Info(string.Format("Gathering report file {0}", filename));

                                            try
                                            {
                                                reportFileCollection.Add(MarshalReportFile(reportDirectory, filename));

                                                Log.Info(string.Format("Successfully gathered report file {0}", filename));
                                            }
                                            catch (IOException)
                                            {
                                                Log.Error(string.Format("Failed to read {0}, continuing ...", filename));
                                            }
                                        }
                                    }
                                }

                                foreach (string outputLine in scriptProcess.Output)
                                {
                                    Log.Info(outputLine);
                                }

                                Log.Info("Finished core ticket generation");
                            }
                            return reportFileCollection;
                        }

                        public Task<ReportFiles> ProvideReportFiles()
                        {
                            return Task.Run<ReportFiles>(() => GenerateReportFiles());
                        }
                    }
                }
            }
        }
    }
}
