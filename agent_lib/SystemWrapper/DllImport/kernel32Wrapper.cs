using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            namespace SystemWrapper
            {
                namespace DllImport
                {
                    [ExcludeFromCodeCoverage]
                    public class kernel32Wrapper
                    {
                        public const int STD_INPUT_HANDLE = -10;
                        public const int STD_OUTPUT_HANDLE = -11;
                        public const int STD_ERROR_HANDLE = -12;

                        public const int DUPLICATE_CLOSE_SOURCE = 1;
                        public const int DUPLICATE_SAME_ACCESS = 2;

                        [StructLayout(LayoutKind.Sequential)]
                        internal struct SECURITY_ATTRIBUTES
                        {
                            public UInt32 nLength;
                            public IntPtr lpSecurityDescriptor;
                            public bool bInheritHandle;
                        }

                        internal static class ExportWrapper
                        {
                            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal extern static bool CloseHandle(IntPtr handle);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool TerminateProcess(
                                SafeProcessHandle hProcess,
                                UInt32 uExitCode);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            internal static extern IntPtr GetStdHandle(int handleId);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool CreatePipe(
                                out SafeFileHandle hReadPipe,
                                out SafeFileHandle hWritePipe,
                                ref SECURITY_ATTRIBUTES lpPipeAttributes,
                                int nSize);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool DuplicateHandle(
                                HandleRef hSourceProcessHandle,
                                SafeFileHandle hSourceHandle,
                                HandleRef hTargetProcess,
                                out SafeFileHandle targetHandle,
                                int dwDesiredAccess,
                                [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                                int dwOptions);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool DuplicateHandle(
                                HandleRef hSourceProcessHandle,
                                SafeProcessHandle hSourceHandle,
                                HandleRef hTargetProcess,
                                out SafeWaitHandle targetHandle,
                                UInt32 dwDesiredAccess,
                                [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                                UInt32 dwOptions);

                            [DllImport("kernel32.dll", SetLastError = true)]
                            internal static extern IntPtr GetCurrentProcess();
                        }

                        public virtual bool CloseHandle(IntPtr handle)
                        {
                            return ExportWrapper.CloseHandle(handle);
                        }

                        public virtual bool TerminateProcess(
                            SafeProcessHandle hProcess,
                            UInt32 uExitCode)
                        {
                            return ExportWrapper.TerminateProcess(
                                hProcess,
                                uExitCode);
                        }

                        public virtual IntPtr GetStdHandle(int handleId)
                        {
                            return ExportWrapper.GetStdHandle(handleId);
                        }

                        internal virtual bool CreatePipe(
                            out SafeFileHandle hReadPipe,
                            out SafeFileHandle hWritePipe,
                            ref SECURITY_ATTRIBUTES lpPipeAttributes,
                            int nSize)
                        {
                            return ExportWrapper.CreatePipe(
                                out hReadPipe,
                                out hWritePipe,
                                ref lpPipeAttributes,
                                nSize);
                        }

                        public virtual bool DuplicateHandle(
                            HandleRef hSourceProcessHandle,
                            SafeFileHandle hSourceHandle,
                            HandleRef hTargetProcess,
                            out SafeFileHandle targetHandle,
                            int dwDesiredAccess,
                            bool bInheritHandle,
                            int dwOptions)
                        {
                            return ExportWrapper.DuplicateHandle(
                                hSourceProcessHandle,
                                hSourceHandle,
                                hTargetProcess,
                                out targetHandle,
                                dwDesiredAccess,
                                bInheritHandle,
                                dwOptions);
                        }

                        public virtual bool DuplicateHandle(
                            HandleRef hSourceProcessHandle,
                            SafeProcessHandle hSourceHandle,
                            HandleRef hTargetProcess,
                            out SafeWaitHandle targetHandle,
                            UInt32 dwDesiredAccess,
                            bool bInheritHandle,
                            UInt32 dwOptions)
                        {
                            return ExportWrapper.DuplicateHandle(
                                hSourceProcessHandle,
                                hSourceHandle,
                                hTargetProcess,
                                out targetHandle,
                                dwDesiredAccess,
                                bInheritHandle,
                                dwOptions);
                        }

                        public virtual IntPtr CurrentProcess
                        {
                            get
                            {
                                return ExportWrapper.GetCurrentProcess();
                            }
                        }
                    }
                }
            }
        }
    }
}