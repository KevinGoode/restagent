using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            
            /// <summary>
            /// Convenience methods to wrap Process Logger. Provides API with backward compatibility with legacy RM code
            /// Also provides additional functionality to write to windows event log

            /// </summary>
            public static class Log
            {
                public static void Info(string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0)
                {
                    ProcessLog(ProcessLogLevel.Info, message, method, lineNumber);
                }
                public static void Debug(string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0)
                {
                    ProcessLog(ProcessLogLevel.Debug, message, method, lineNumber);
                }
                public static void Warning(string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0)
                {
                    ProcessLog(ProcessLogLevel.Warning, message, method, lineNumber);
                }
                public static void Error(string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0)
                {
                    ProcessLog(ProcessLogLevel.Error, message, method, lineNumber);
                }
             
                private static void ProcessLog(ProcessLogLevel level, string message, string method, int lineNumber)
                {
                    IProcessLogger LoggerInstance = ProcessLogger.GetInstance();
                    if (LoggerInstance != null)
                    {
                        LoggerInstance.Log(level, message, method, lineNumber);
                    }
                }
               
            } 
        }

    }
}
