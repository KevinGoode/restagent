using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
                    namespace Diagnostics
                    {
                        [ExcludeFromCodeCoverage]
                        public class ProcessWrapper : IProcessWrapper
                        {
                            private Process WrappedProcess { get; set; }
                            private bool Started { get; set; }

                            public bool Running
                            {
                                get { return Started && !WrappedProcess.HasExited; }
                            }

                            protected List<string> OutputCollection { get; private set; }
                            public IReadOnlyList<string> Output
                            {
                                get { return OutputCollection; }
                            }

                            public ProcessStartInfo StartInfo
                            {
                                get { return WrappedProcess.StartInfo; }
                            }

                            public ProcessWrapper(string executablePath, string parameters)
                            {
                                OutputCollection = new List<string>();
                                Started = false;

                                WrappedProcess = new Process();

                                WrappedProcess.StartInfo.FileName = executablePath;
                                WrappedProcess.StartInfo.CreateNoWindow = true;
                                WrappedProcess.StartInfo.UseShellExecute = false;
                                WrappedProcess.StartInfo.RedirectStandardOutput = true;
                                WrappedProcess.StartInfo.Arguments = parameters;

                                WrappedProcess.EnableRaisingEvents = true;

                                WrappedProcess.OutputDataReceived += HandleOutput;
                                WrappedProcess.ErrorDataReceived += HandleError;
                            }

                            public void Start()
                            {
                                WrappedProcess.Start();
                                WrappedProcess.BeginOutputReadLine();
                                Started = true;
                            }

                            private void HandleOutput(Object Sender, DataReceivedEventArgs Args)
                            {
                                if (Args.Data != null)
                                {
                                    string line = Args.Data;
                                    try
                                    {
                                        OutputCollection.Add(line);
                                    }
                                    catch (Exception Ex)
                                    {
                                        Log.Error(Ex.ToString());
                                    }
                                }
                            }

                            private void HandleError(Object Sender, DataReceivedEventArgs Args)
                            {
                                if (Args.Data != null)
                                {
                                    Log.Error(string.Format("{0}: {1}", WrappedProcess.ProcessName, Args.Data));
                                }
                            }

                            public bool WaitForExit(TimeSpan maximumDuration)
                            {
                                return WrappedProcess.WaitForExit((int) maximumDuration.TotalMilliseconds);
                            }

                            public void Dispose()
                            {
                                Dispose(true);
                                GC.SuppressFinalize(this);
                            }

                            protected virtual void Dispose(bool disposeManaged)
                            {
                                if (disposeManaged)
                                {
                                    WrappedProcess.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
