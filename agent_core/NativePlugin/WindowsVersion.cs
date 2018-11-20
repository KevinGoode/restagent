using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

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
                    public class WindowsVersion : IEquatable<WindowsVersion>
                    {
                        public WindowsVersion(string name, uint major, uint minor, bool server)
                        {
                            windowsName = name;
                            majorVersion = major;
                            minorVersion = minor;
                            windowsServer = server;
                        }
                        public WindowsVersion(uint major, uint minor, bool server)
                        {
                            windowsName = "";
                            majorVersion = major;
                            minorVersion = minor;
                            windowsServer = server;
                        }
                        public bool Equals(WindowsVersion other)
                        {
                            bool equals = false;
                            if (other.majorVersion == majorVersion &&
                                other.minorVersion == minorVersion &&
                                other.windowsServer == windowsServer)
                            {
                                equals = true;
                            }
                            return equals;
                        }
                        public string Name { get { return windowsName;} }

                        private string windowsName;
                        private uint majorVersion;
                        private uint minorVersion;
                        private bool windowsServer;
                    }
                    public static class WindowsInfo
                    {
                        public static string RetrieveWindowsVersion()
                        {
                            string windowsVersion = "Windows (Unknown)";
                            List<WindowsVersion> known_versions = new List<WindowsVersion>();
                            known_versions.Add(new WindowsVersion("Windows Server 2016", 10, 0, true));
                            known_versions.Add(new WindowsVersion("Windows 10", 10, 0, false));
                            known_versions.Add(new WindowsVersion("Windows 8.1", 6, 3, false));
                            known_versions.Add(new WindowsVersion("Windows Server 2012 R2", 6, 3, true));
                            known_versions.Add(new WindowsVersion("Windows 8", 6, 2, false));
                            known_versions.Add(new WindowsVersion("Windows Server 2012", 6, 2, true));
                            known_versions.Add(new WindowsVersion("Windows 7", 6, 1, false));
                            known_versions.Add(new WindowsVersion("Windows Server 2008 R2", 6, 1, true));
                            known_versions.Add(new WindowsVersion("Windows Server 2008", 6, 0, true));
                            known_versions.Add(new WindowsVersion("Windows Vista", 6, 0, false));
                            known_versions.Add(new WindowsVersion("Windows Server 2003", 5, 2, true));
                            known_versions.Add(new WindowsVersion("Windows XP 64-Bit Edition", 5, 2, false));
                            known_versions.Add(new WindowsVersion("Windows XP", 5, 1, false));
                            known_versions.Add(new WindowsVersion("Windows 2000", 5, 0, false));

                            WindowsVersion current_version = new WindowsVersion(WindowsInfo.WinMajorVersion, WindowsInfo.WinMinorVersion, WindowsInfo.IsServer);
                            WindowsVersion version = known_versions.Find(current_version.Equals);
                            if (version != null)
                            {
                                windowsVersion = version.Name;
                            }
                            return windowsVersion;
                        }
                        public static uint WinMajorVersion
                        {
                            get
                            {
                                dynamic major;

                                if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out major))
                                {
                                    return (uint)major;
                                }


                                dynamic version;
                                if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                                    return 0;

                                var versionParts = ((string)version).Split('.');
                                if (versionParts.Length != 2) return 0;
                                uint majorAsUInt;
                                return uint.TryParse(versionParts[0], out majorAsUInt) ? majorAsUInt : 0;
                            }
                        }
                        public static uint WinMinorVersion
                        {
                            get
                            {
                                dynamic minor;
                                if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber",
                                    out minor))
                                {
                                    return (uint)minor;
                                }

                                dynamic version;
                                if (!TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out version))
                                    return 0;

                                var versionParts = ((string)version).Split('.');
                                if (versionParts.Length != 2) return 0;
                                uint minorAsUInt;
                                return uint.TryParse(versionParts[1], out minorAsUInt) ? minorAsUInt : 0;
                            }
                        }
                        public static bool IsServer
                        {
                            get
                            {
                                dynamic installationType;
                                if (TryGeRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType",
                                    out installationType))
                                {
                                    return (bool)(installationType.Equals("Client") ? false : true);
                                }

                                return false;
                            }
                        }
                        private static bool TryGeRegistryKey(string path, string key, out dynamic value)
                        {
                            value = null;
                            try
                            {
                                var rk = Registry.LocalMachine.OpenSubKey(path);
                                if (rk == null) return false;
                                value = rk.GetValue(key);
                                return value != null;
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
    }
}