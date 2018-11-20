using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using Kge.Agent.Library.SystemWrapper.DllImport;

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
                        internal class AsyncLineReader : IDisposable
                        {
                            private static char[] LineTerminators = { '\r', '\n' };

                            private StringBuilder LineBuffer { get; set; }

                            char[] CharacterBuffer { get; set; }
                            byte[] ByteBuffer { get; set; }

                            private Stream SourceStream { get; set; }
                            private Decoder StreamDecoder { get; set; }

                            private Action<string> HandlerCallback { get; set; }
                            private Action StreamEndCallback { get; set; }

                            private bool Disposed { get; set; }

                            private object QueueSynchronisation { get; set; }

                            public AsyncLineReader(Stream stream, Action<string> lineHandler, Action eofHandler)
                            {
                                var encoding = Console.OutputEncoding;

                                LineBuffer = new StringBuilder();
                                SourceStream = stream;
                                HandlerCallback = lineHandler;
                                StreamEndCallback = eofHandler;
                                StreamDecoder = encoding.GetDecoder();
                                ByteBuffer = new byte[1024];
                                CharacterBuffer = new char[encoding.GetMaxCharCount(ByteBuffer.Length)];
                                QueueSynchronisation = new object();

                                QueueRead();
                            }

                            private void QueueRead()
                            {
                                Log.Debug("Queuing async read");
                                var asyncRead = SourceStream.ReadAsync(ByteBuffer, 0, ByteBuffer.Length);
                                asyncRead.ContinueWith((read) => ReadBuffer(read.Result));
                            }

                            private void ReadBuffer(int byteCount)
                            {
                                if (byteCount == 0)
                                {
                                    Log.Debug("Hit EOF");
                                    lock (LineBuffer)
                                    {
                                        if (LineBuffer.Length > 0)
                                        {
                                            HandlerCallback(LineBuffer.ToString().Trim(LineTerminators));
                                            LineBuffer.Clear();
                                        }
                                    }
                                    if (StreamEndCallback != null)
                                    {
                                        StreamEndCallback();
                                    }
                                }
                                else
                                {
                                    Log.Debug(string.Format("Reading {0} bytes", byteCount));
                                    var characterCount = StreamDecoder.GetChars(ByteBuffer, 0, byteCount, CharacterBuffer, 0);
                                    lock (LineBuffer)
                                    {
                                        LineBuffer.Append(CharacterBuffer, 0, characterCount);
                                        string line = StripLine(LineBuffer);
                                        while (line != null)
                                        {
                                            HandlerCallback(line);
                                            line = StripLine(LineBuffer);
                                        }
                                    }
                                    QueueRead();
                                }
                            }

                            private static string StripLine(StringBuilder buffer)
                            {
                                // Windows and Linux both agree on \n being the final character in a newline
                                var breakpoint = buffer.ToString().IndexOf('\n');
                                if (breakpoint < 0)
                                {
                                    return null;
                                }
                                else
                                {
                                    var linePrefix = buffer.ToString().Substring(0, breakpoint).Trim(LineTerminators);
                                    buffer.Remove(0, breakpoint + 1);
                                    return linePrefix;
                                }
                            }


                            public void Dispose()
                            {
                                Dispose(true);
                                GC.SuppressFinalize(this);
                            }

                            protected virtual void Dispose(bool disposeManaged)
                            {
                                if (Disposed)
                                {
                                    return;
                                }
                                lock (this)
                                {
                                    if (Disposed)
                                    {
                                        return;
                                    }
                                    if (disposeManaged)
                                    {
                                        SourceStream.Dispose();
                                    }
                                    SourceStream = null;
                                    ByteBuffer = null;
                                    CharacterBuffer = null;
                                    StreamDecoder = null;
                                    Disposed = true;
                                }
                            }
                        }

                        [ExcludeFromCodeCoverage]
                        internal class ServiceProcessWaitandle : WaitHandle
                        {
                            private kernel32Wrapper Kernel { get; set; }

                            internal ServiceProcessWaitandle(kernel32Wrapper kernelWrapper, SafeProcessHandle processHandle)
                                : base()
                            {
                                Kernel = kernelWrapper;
                                this.SafeWaitHandle = MakeSafeWaitHandle(processHandle);
                            }

                            private SafeWaitHandle MakeSafeWaitHandle(SafeProcessHandle processHandle)
                            {
                                SafeWaitHandle waitHandle;
                                var retVal = Kernel.DuplicateHandle(
                                    new HandleRef(this, Kernel.CurrentProcess),
                                    processHandle,
                                    new HandleRef(this, Kernel.CurrentProcess),
                                    out waitHandle,
                                    0,
                                    false,
                                    kernel32Wrapper.DUPLICATE_SAME_ACCESS);

                                if (!retVal)
                                {
                                    Int32 errorCode = Marshal.GetLastWin32Error();
                                    Log.Error(string.Format("Win32Error: {0}", errorCode));
                                    throw new Win32Exception(errorCode);
                                }
                                else
                                {
                                    Log.Debug("Created wait handle");
                                }
                                return waitHandle;
                            }
                        }

                        /// <summary>
                        /// Due to certain quirks in the way Windows handles the LocalSystem account, the Process
                        /// class cannot be used to run processes under another user. To support this, we need
                        /// to wrap the CreateProcessWithTokenW API manually as the LocalSystem account does
                        /// not have the privileges needed for other means of spawning child processes.
                        /// </summary>
                        [ExcludeFromCodeCoverage]
                        public class ServiceProcessWrapper : IProcessWrapper
                        {
                            private static TimeSpan WaitForEofDuration = TimeSpan.FromSeconds(3);

                            private bool Started { get; set; }
                            private bool Finished { get; set; }
                            private bool StdOutEof { get; set; }
                            private bool StdErrEof { get; set; }

                            private bool IOCompleted
                            {
                                get { return StdOutEof && StdErrEof; }
                            }

                            private bool ValidProcess
                            {
                                get { return ProcessHandle != null && !ProcessHandle.IsInvalid; }
                            }

                            public bool Running
                            {
                                get { return Started && ValidProcess && !(Finished || Disposed); }
                            }
                            private bool Disposed { get; set; }

                            protected bool RedirectingIO { get; private set; }

                            private List<string> mOutput { get; set; }
                            public IReadOnlyList<string> Output
                            {
                                get { return mOutput; }
                            }

                            private ProcessStartInfo mStartInfo { get; set; }
                            public ProcessStartInfo StartInfo
                            {
                                get
                                {
                                    if (mStartInfo == null)
                                    {
                                        mStartInfo = new ProcessStartInfo();
                                    }
                                    return mStartInfo;
                                }
                            }

                            private SafeFileHandle StdOutHandle { get; set; }
                            private SafeFileHandle StdErrHandle { get; set; }

                            private SafeProcessHandle ProcessHandle { get; set; }
                            private ServiceProcessWaitandle ProcessWaitHandle { get; set; }
                            private RegisteredWaitHandle RegisteredWaitHandle { get; set; }

                            private ICollection<IDisposable> Disposables { get; set; }

                            private kernel32Wrapper Kernel { get; set; }
                            private advapi32Wrapper AdvApi { get; set; }

                            public ServiceProcessWrapper(kernel32Wrapper kernelWrapper, advapi32Wrapper advApiWrapper)
                            {
                                Kernel = kernelWrapper;
                                AdvApi = advApiWrapper;

                                StartInfo.CreateNoWindow = true;
                                StartInfo.UseShellExecute = false;
                                StartInfo.RedirectStandardOutput = true;
                                StartInfo.RedirectStandardError = true;

                                CopyEnvironmentVariables(Environment.GetEnvironmentVariables() as IDictionary<string,string>);

                                mOutput = new List<string>();
                                Disposables = new List<IDisposable>();
                            }

                            private void CopyEnvironmentVariables(IDictionary<string,string> source)
                            {
                                if (source == null)
                                {
                                    return;
                                }
                                foreach (var pair in source)
                                {
                                    StartInfo.EnvironmentVariables[pair.Key] = pair.Value;
                                }
                            }

                            public void Start()
                            {
                                if (Started || Disposed)
                                {
                                    return;
                                }

                                lock (this)
                                {
                                    if (Started || Disposed)
                                    {
                                        return;
                                    }

                                    Log.Debug(string.Format("Starting '{0}' with parameters '{1}'", StartInfo.FileName, StartInfo.Arguments));

                                    advapi32Wrapper.STARTUPINFO startupInfo = new advapi32Wrapper.STARTUPINFO()
                                    {
                                        dwFlags = (UInt32)DetermineStartupInfoFlags(),
                                        wShowWindow = 0,
                                        lpDesktop = IntPtr.Zero,
                                        lpTitle = IntPtr.Zero
                                    };

                                    startupInfo.cb = (uint) Marshal.SizeOf(startupInfo);

                                    advapi32Wrapper.SafeTokenHandle tokenHandle = null;
                                    advapi32Wrapper.PROCESS_INFORMATION processInfo;

                                    if (RedirectingIO)
                                    {
                                        InitialiseStartupInfoIoHandles(ref startupInfo);
                                    }

                                    try
                                    {
                                        LogonForTokenHandle(out tokenHandle);
                                        StartProcess(tokenHandle, ref startupInfo, out processInfo);

                                        ProcessHandle = new SafeProcessHandle(processInfo.hProcess, true);
                                        var threadHandle = new SafeProcessHandle(processInfo.hThread, true);
                                        if (!threadHandle.IsInvalid)
                                        {
                                            threadHandle.Close();
                                        }
                                        SetupWaitForExit();

                                        if (RedirectingIO)
                                        {
                                            InitialiseAsyncHandlers();
                                        }
                                    }
                                    finally
                                    {
                                        if (tokenHandle != null)
                                        {
                                            tokenHandle.Close();
                                        }
                                    }

                                    Started = true;
                                }
                            }

                            private void SetupWaitForExit()
                            {
                                ProcessWaitHandle = new ServiceProcessWaitandle(Kernel, ProcessHandle);
                                RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                                    ProcessWaitHandle,
                                    new WaitOrTimerCallback(ProcessExitCallback),
                                    null,
                                    -1,
                                    true);
                            }

                            private void ProcessExitCallback(object context, bool signal)
                            {
                                if (Finished)
                                {
                                    return;
                                }
                                lock (this)
                                {
                                    if (Finished)
                                    {
                                        return;
                                    }
                                    SpinWait.SpinUntil(
                                        () => IOCompleted,
                                        WaitForEofDuration);
                                    RegisteredWaitHandle.Unregister(null);
                                    ProcessWaitHandle.Close();
                                    ProcessWaitHandle = null;
                                    RegisteredWaitHandle = null;
                                    Finished = true;
                                }
                            }

                            private advapi32Wrapper.StartupInfoFlags DetermineStartupInfoFlags()
                            {
                                advapi32Wrapper.StartupInfoFlags dwFlags = 0;

                                if (StartInfo.CreateNoWindow)
                                {
                                    dwFlags = dwFlags | advapi32Wrapper.StartupInfoFlags.STARTF_USESHOWWINDOW;
                                }

                                if (StartInfo.RedirectStandardError || StartInfo.RedirectStandardOutput)
                                {
                                    dwFlags = dwFlags | advapi32Wrapper.StartupInfoFlags.STARTF_USESTDHANDLES;
                                    RedirectingIO = true;
                                }

                                Log.Debug(string.Format("dwflags: {0}", dwFlags));

                                return dwFlags;
                            }

                            private void LogonForTokenHandle(out advapi32Wrapper.SafeTokenHandle handle)
                            {
                                bool retVal = AdvApi.LogonUserW(
                                    StartInfo.UserName,
                                    StartInfo.Domain,
                                    StartInfo.Password,
                                    (int)advapi32Wrapper.LogonType.Batch,
                                    (int)advapi32Wrapper.LogonProvider.LOGON32_PROVIDER_DEFAULT,
                                    out handle);

                                if (!retVal)
                                {
                                    Int32 errorCode = Marshal.GetLastWin32Error();
                                    Log.Error(string.Format("Win32Error: {0}", errorCode));
                                    throw new Win32Exception(errorCode);
                                }
                                else
                                {
                                    Log.Debug(string.Format("Logged on as {0}\\{1}", StartInfo.Domain, StartInfo.UserName));
                                }
                            }

                            private advapi32Wrapper.CreationFlags DetermineCreationFlags()
                            {
                                advapi32Wrapper.CreationFlags dwFlags = 0;

                                dwFlags |= advapi32Wrapper.CreationFlags.DefaultErrorMode;
                                dwFlags |= advapi32Wrapper.CreationFlags.NewConsole;
                                dwFlags |= advapi32Wrapper.CreationFlags.NewProcessGroup;
                                dwFlags |= advapi32Wrapper.CreationFlags.UnicodeEnvironment;

                                return dwFlags;
                            }

                            private IntPtr DuplicateEnvironment(ref GCHandle garbageCollectorHandle)
                            {
                                if (StartInfo.EnvironmentVariables != null)
                                {
                                    StringBuilder environmentBuffer = new StringBuilder();

                                    foreach (var pair in StartInfo.Environment)
                                    {
                                        environmentBuffer.Append(pair.Key);
                                        environmentBuffer.Append('=');
                                        environmentBuffer.Append(pair.Value);
                                        environmentBuffer.Append('\0');
                                    }

                                    byte[] environmentBytes = Encoding.Unicode.GetBytes(environmentBuffer.ToString());
                                    garbageCollectorHandle = GCHandle.Alloc(environmentBytes, GCHandleType.Pinned);
                                    return garbageCollectorHandle.AddrOfPinnedObject();
                                }
                                else
                                {
                                    return IntPtr.Zero;
                                }
                            }

                            private string CreateCommandLine()
                            {
                                StringBuilder commandLineBuilder = new StringBuilder();
                                bool isQuoted = StartInfo.FileName.StartsWith("\"", StringComparison.Ordinal);
                                if (!isQuoted)
                                {
                                    commandLineBuilder.Append("\"");
                                }
                                commandLineBuilder.Append(StartInfo.FileName);
                                if (!isQuoted)
                                {
                                    commandLineBuilder.Append("\"");
                                }

                                if (!string.IsNullOrWhiteSpace(StartInfo.Arguments))
                                {
                                    commandLineBuilder.Append(" ");
                                    commandLineBuilder.Append(StartInfo.Arguments);
                                }

                                return commandLineBuilder.ToString();
                            }

                            private void StartProcess(
                                advapi32Wrapper.SafeTokenHandle tokenHandle,
                                ref advapi32Wrapper.STARTUPINFO startupInfo,
                                out advapi32Wrapper.PROCESS_INFORMATION processInfo)
                            {
                                GCHandle garbageCollectorHandle = new GCHandle();
                                string lpCurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(StartInfo.FileName));
                                kernel32Wrapper.SECURITY_ATTRIBUTES blankSecAttributes = new kernel32Wrapper.SECURITY_ATTRIBUTES();

                                try
                                {
                                    bool retVal = AdvApi.CreateProcessAsUserW(
                                        tokenHandle,
                                        null,
                                        CreateCommandLine(),
                                        ref blankSecAttributes,
                                        ref blankSecAttributes,
                                        true,
                                        (UInt32) DetermineCreationFlags(),
                                        DuplicateEnvironment(ref garbageCollectorHandle),
                                        lpCurrentDirectory,
                                        ref startupInfo,
                                        out processInfo);

                                    if (!retVal)
                                    {
                                        Int32 errorCode = Marshal.GetLastWin32Error();
                                        Log.Error(string.Format("Win32Error: {0}", errorCode));
                                        throw new Win32Exception(errorCode);
                                    }
                                    else
                                    {
                                        Log.Debug(string.Format("Started '{0}'", StartInfo.FileName));
                                    }
                                }
                                finally
                                {
                                    if (garbageCollectorHandle.IsAllocated)
                                    {
                                        garbageCollectorHandle.Free();
                                    }
                                }
                            }

                            private void InitialiseStartupInfoIoHandles(ref advapi32Wrapper.STARTUPINFO startupInfo)
                            {
                                startupInfo.hStdInput = new SafeFileHandle(Kernel.GetStdHandle(kernel32Wrapper.STD_INPUT_HANDLE), true);

                                if (StartInfo.RedirectStandardOutput)
                                {
                                    SafeFileHandle parentOutputHandle = null;
                                    CreatePipe(out parentOutputHandle, out startupInfo.hStdOutput);
                                    if (parentOutputHandle != null)
                                    {
                                        StdOutHandle = parentOutputHandle;
                                    }
                                }
                                else
                                {
                                    startupInfo.hStdOutput = new SafeFileHandle(Kernel.GetStdHandle(kernel32Wrapper.STD_OUTPUT_HANDLE), false);
                                }

                                if (StartInfo.RedirectStandardError)
                                {
                                    SafeFileHandle parentErrHandle = null;
                                    CreatePipe(out parentErrHandle, out startupInfo.hStdError);
                                    if (parentErrHandle != null)
                                    {
                                        StdErrHandle = parentErrHandle;
                                    }
                                }
                                else
                                {
                                    startupInfo.hStdError = new SafeFileHandle(Kernel.GetStdHandle(kernel32Wrapper.STD_ERROR_HANDLE), false);
                                }
                            }

                            private void InitialiseAsyncHandlers()
                            {
                                if (StartInfo.RedirectStandardOutput)
                                {
                                    if (StdOutHandle.IsInvalid || StdOutHandle.IsClosed)
                                    {
                                        var reason = StdOutHandle.IsInvalid ? "invalid" : "closed";
                                        Log.Error(string.Format("STDOUT handle is {0}", reason));
                                        StdOutEof = true;
                                    }
                                    else
                                    {
                                        Log.Debug("Initialising STDOUT async reader");
                                        var stdOutReader = new AsyncLineReader(
                                            new FileStream(StdOutHandle, FileAccess.Read, 4096, false),
                                            (line) => mOutput.Add(line),
                                            () => StdOutEof = true);
                                        Disposables.Add(stdOutReader);
                                    }
                                }
                                else
                                {
                                    StdOutEof = true;
                                }

                                if (StartInfo.RedirectStandardError)
                                {
                                    if (StdErrHandle.IsInvalid || StdErrHandle.IsClosed)
                                    {
                                        var reason = StdOutHandle.IsInvalid ? "invalid" : "closed";
                                        Log.Error(string.Format("STDERR handle is {0}", reason));
                                        StdErrEof = true;
                                    }
                                    else
                                    {
                                        Log.Debug("Initialising STDERR async reader");
                                        var stdErrReader = new AsyncLineReader(
                                            new FileStream(StdErrHandle, FileAccess.Read, 4096, false),
                                            (line) => Log.Error(string.Format("{0} ERR: {1}", Path.GetFileName(StartInfo.FileName), line)),
                                            () => StdErrEof = true);
                                        Disposables.Add(stdErrReader);
                                    }
                                }
                                else
                                {
                                    StdErrEof = true;
                                }
                            }

                            public void CreatePipe(out SafeFileHandle parentHandle, out SafeFileHandle childHandle)
                            {
                                SafeFileHandle temporaryHandle = null;

                                kernel32Wrapper.SECURITY_ATTRIBUTES secAttributes = new kernel32Wrapper.SECURITY_ATTRIBUTES()
                                {
                                    lpSecurityDescriptor = IntPtr.Zero,
                                    bInheritHandle = true
                                };

                                secAttributes.nLength = (UInt32)Marshal.SizeOf(secAttributes);

                                try
                                {
                                    var retVal = Kernel.CreatePipe(out temporaryHandle, out childHandle, ref secAttributes, 0);
                                    if (!retVal)
                                    {
                                        Int32 errorCode = Marshal.GetLastWin32Error();
                                        Log.Error(string.Format("Win32Error: {0}", errorCode));
                                        throw new Win32Exception(errorCode);
                                    }

                                    retVal = Kernel.DuplicateHandle(
                                        new HandleRef(this, Kernel.CurrentProcess),
                                        temporaryHandle,
                                        new HandleRef(this, Kernel.CurrentProcess),
                                        out parentHandle,
                                        0,
                                        false,
                                        kernel32Wrapper.DUPLICATE_SAME_ACCESS);
                                    if (!retVal)
                                    {
                                        Int32 errorCode = Marshal.GetLastWin32Error();
                                        Log.Error(string.Format("Win32Error: {0}", errorCode));
                                        throw new Win32Exception(errorCode);
                                    }
                                }
                                finally
                                {
                                    if (temporaryHandle != null && !temporaryHandle.IsInvalid)
                                    {
                                        temporaryHandle.Close();
                                    }
                                }
                            }

                            public bool WaitForExit(TimeSpan maximumDuration)
                            {
                                if (Running)
                                {
                                    SpinWait.SpinUntil(
                                        () => Finished,
                                        maximumDuration);
                                }
                                return Finished;
                            }

                            public void Dispose()
                            {
                                Dispose(true);
                                GC.SuppressFinalize(this);
                            }

                            protected virtual void Dispose(bool disposeManaged)
                            {
                                if (!Disposed)
                                {
                                    if (disposeManaged)
                                    {
                                        if (Running)
                                        {
                                            Kernel.TerminateProcess(
                                               ProcessHandle,
                                               0);
                                        }
                                        foreach (var disposable in Disposables)
                                        {
                                            disposable.Dispose();
                                        }
                                        if (StdOutHandle != null && !StdOutHandle.IsInvalid)
                                        {
                                            StdOutHandle.Close();
                                        }
                                        if (StdErrHandle != null && !StdErrHandle.IsInvalid)
                                        {
                                            StdErrHandle.Close();
                                        }
                                        if (ProcessHandle != null && !ProcessHandle.IsInvalid)
                                        {
                                            ProcessHandle.Close();
                                        }
                                        if (RegisteredWaitHandle != null)
                                        {
                                            RegisteredWaitHandle.Unregister(null);
                                        }
                                        if (ProcessWaitHandle != null)
                                        {
                                            ProcessWaitHandle.Close();
                                        }
                                    }
                                    Finished = true;
                                    StdOutEof = true;
                                    StdErrEof = true;

                                    ProcessWaitHandle = null;
                                    RegisteredWaitHandle = null;
                                    Disposables = null;
                                    StdOutHandle = null;
                                    StdErrHandle = null;
                                    ProcessHandle = null;
                                    Disposed = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
