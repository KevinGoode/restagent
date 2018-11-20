using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

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
                    public class advapi32Wrapper
                    {
                        public enum SID_NAME_USE
                        {
                            Invalid = 0,
                            SidTypeUser = 1,
                            SidTypeGroup,
                            SidTypeDomain,
                            SidTypeAlias,
                            SidTypeWellKnownGroup,
                            SidTypeDeletedAccount,
                            SidTypeInvalid,
                            SidTypeUnknown,
                            SidTypeComputer
                        }

                        [Flags]
                        internal enum LogonFlags
                        {
                            None = 0,
                            WithProfile = 1,
                            NetCredentialsOnly = 2,
                        }

                        [Flags]
                        internal enum CreationFlags
                        {
                            DefaultErrorMode = 0x04000000,
                            NewConsole = 0x00000010,
                            NewProcessGroup = 0x00000200,
                            SeparateWOWVDM = 0x00000800,
                            Suspended = 0x00000004,
                            UnicodeEnvironment = 0x00000400,
                            ExtendedStartupInfoPresent = 0x00080000
                        }

                        [Flags]
                        internal enum StartupInfoFlags
                        {
                            STARTF_FORCEONFEEDBACK = 0x00000040,
                            STARTF_FORCEOFFFEEDBACK = 0x00000080,
                            STARTF_PREVENTPINNING = 0x00002000,
                            STARTF_RUNFULLSCREEN = 0x00000020,
                            STARTF_TITLEISAPPID = 0x00001000,
                            STARTF_TITLEISLINKNAME = 0x00000800,
                            STARTF_UNTRUSTEDSOURCE = 0x00008000,
                            STARTF_USECOUNTCHARS = 0x00000008,
                            STARTF_USEFILLATTRIBUTE = 0x00000010,
                            STARTF_USEHOTKEY = 0x00000200,
                            STARTF_USEPOSITION = 0x00000004,
                            STARTF_USESHOWWINDOW = 0x00000001,
                            STARTF_USESIZE = 0x00000002,
                            STARTF_USESTDHANDLES = 0x00000100
                        }

                        internal enum LogonType
                        {
                            LocalSystem = 0,
                            Interactive = 2,
                            Network = 3,
                            Batch = 4,
                            Service = 5,
                            Proxy = 6,
                            Unlock = 7,
                            NetworkCleartext = 8,
                            NewCredentials = 9,
                            RemoteInteractive = 10,
                            CachedInteractive = 11,
                            CachedRemoteInteractive = 12,
                            CachedUnlock = 13
                        };

                        internal enum LogonProvider
                        {
                            LOGON32_PROVIDER_DEFAULT = 0
                        };

                        [StructLayout(LayoutKind.Sequential)]
                        internal struct PROCESS_INFORMATION
                        {
                            public IntPtr hProcess;
                            public IntPtr hThread;
                            public Int32 dwProcessId;
                            public Int32 dwThreadId;
                        }

                        [StructLayout(LayoutKind.Sequential)]
                        internal struct STARTUPINFO
                        {
                            public UInt32 cb;
                            public IntPtr lpReserved;
                            public IntPtr lpDesktop;
                            public IntPtr lpTitle;
                            public UInt32 dwX;
                            public UInt32 dwY;
                            public UInt32 dwXSize;
                            public UInt32 dwYSize;
                            public UInt32 dwXCountChars;
                            public UInt32 dwYCountChars;
                            public UInt32 dwFillAttribute;
                            public UInt32 dwFlags;
                            public UInt16 wShowWindow;
                            public UInt16 cbReserved2;
                            public IntPtr lpReserved2;
                            public SafeFileHandle hStdInput;
                            public SafeFileHandle hStdOutput;
                            public SafeFileHandle hStdError;
                        }

                        internal static class ExportWrapper
                        {
                            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool LookupAccountName(
                                string machineName,
                                string accountName,
                                byte[] sid,
                                ref int sidLen,
                                StringBuilder domainName,
                                ref int domainNameLen,
                                out SID_NAME_USE peUse);

                            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool LogonUserW(
                                String lpszUsername,
                                String lpszDomain,
                                String lpszPassword,
                                int dwLogonType,
                                int dwLogonProvider,
                                out SafeTokenHandle phToken);

                            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool LogonUserW(
                                String lpszUsername,
                                String lpszDomain,
                                IntPtr lpszPassword,
                                int dwLogonType,
                                int dwLogonProvider,
                                out SafeTokenHandle phToken);

                            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            internal static extern bool CreateProcessAsUserW(
                                SafeHandle hToken,
                                string lpApplicationName,
                                string lpCommandLine,
                                ref kernel32Wrapper.SECURITY_ATTRIBUTES lpProcessAttributes,
                                ref kernel32Wrapper.SECURITY_ATTRIBUTES lpThreadAttributes,
                                [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
                                UInt32 dwCreationFlags,
                                IntPtr lpEnvironment,
                                string lpCurrentDirectory,
                                [In] ref STARTUPINFO lpStartupInfo,
                                out PROCESS_INFORMATION lpProcessInformation);
                        }

                        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
                        {
                            private SafeTokenHandle()
                                : base(true)
                            {
                            }

                            [DllImport("kernel32.dll")]
                            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                            [SuppressUnmanagedCodeSecurity]
                            [return: MarshalAs(UnmanagedType.Bool)]
                            private static extern bool CloseHandle(IntPtr handle);

                            protected override bool ReleaseHandle()
                            {
                                return CloseHandle(handle);
                            }
                        }

                        public virtual bool LookupAccountName(
                            string machineName,
                            string accountName,
                            byte[] sid,
                            ref int sidLen,
                            StringBuilder domainName,
                            ref int domainNameLen,
                            out DllImport.advapi32Wrapper.SID_NAME_USE peUse)
                        {
                            return ExportWrapper.LookupAccountName(machineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse);
                        }

                        public virtual bool LogonUserW(
                            String lpszUsername,
                            String lpszDomain,
                            String lpszPassword,
                            int dwLogonType,
                            int dwLogonProvider,
                            out SafeTokenHandle phToken)
                        {
                            return ExportWrapper.LogonUserW(lpszUsername, lpszDomain, lpszPassword, dwLogonType, dwLogonProvider, out phToken);
                        }

                        internal virtual bool LogonUserW(
                            string lpszUsername,
                            string lpszDomain,
                            SecureString lpszPassword,
                            int dwLogonType,
                            int dwLogonProvider,
                            out SafeTokenHandle phToken)
                        {
                            IntPtr password = IntPtr.Zero;
                            try
                            {
                                password = Marshal.SecureStringToCoTaskMemUnicode(lpszPassword);

                                return ExportWrapper.LogonUserW(
                                    lpszUsername,
                                    lpszDomain,
                                    password,
                                    dwLogonType,
                                    dwLogonProvider,
                                    out phToken);
                            }
                            finally
                            {
                                if (password != IntPtr.Zero)
                                {
                                    Marshal.ZeroFreeCoTaskMemUnicode(password);
                                }
                            }
                        }

                        internal virtual bool CreateProcessAsUserW(
                            SafeHandle hToken,
                            string lpApplicationName,
                            string lpCommandLine,
                            ref kernel32Wrapper.SECURITY_ATTRIBUTES lpProcessAttributes,
                            ref kernel32Wrapper.SECURITY_ATTRIBUTES lpThreadAttributes,
                            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
                            UInt32 dwCreationFlags,
                            IntPtr lpEnvironment,
                            string lpCurrentDirectory,
                            [In] ref STARTUPINFO lpStartupInfo,
                            out PROCESS_INFORMATION lpProcessInformation)
                        {
                            return ExportWrapper.CreateProcessAsUserW(
                                hToken,
                                lpApplicationName,
                                lpCommandLine,
                                ref lpProcessAttributes,
                                ref lpThreadAttributes,
                                bInheritHandles,
                                dwCreationFlags,
                                lpEnvironment,
                                lpCurrentDirectory,
                                ref lpStartupInfo,
                                out lpProcessInformation);
                        }
                    }
                }
            }
        }
    }
}
