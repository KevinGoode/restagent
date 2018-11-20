using System;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            public interface ICommonRegistry
            {
                bool Exists { get; }
                string InstallPath { get; set; }
                string LogLevel { get; set; }
                string LogPath { get; set; }
                string ServicePort { get; set; }
                string TimeOut { get; set; }
                string RestoreTimeOut { get; set; }
                string Version { get; set; }
            }
        }
    }
}