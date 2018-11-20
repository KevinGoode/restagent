/*
 * (C) Copyright 2016 Kge
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
namespace Kge
{
    namespace Agent
    {
        namespace Library
        {

            public enum ProcessLogLevel
            {
                None = 0,
                Critical = 1,
                Error = 2,
                Warning = 3,
                Info = 4,
                Trace = 5,
                Debug = 6,

            };
            /// <summary>
            /// IProcessLogger defines the public API of the ProcessLogger service
            /// </summary>
            public interface IProcessLogger
            {
                /// <summary>
                /// Returns path of logfile
                /// </summary>
                /// <returns>Fully qualified path of log file</returns>
                string GetFilePath();
                /// <summary>
                /// Returns prefix of logfile
                /// </summary>
                /// <returns>Fully qualified prefix of log file</returns>
                string GetFilePrefix();
                /// <summary>
                /// PauseLogging pauses logging until the 'ResumeLogging' API is called. PauseLogging only returns when the current log write has finished
                /// and the logger is in a paused. 
                /// </summary>
                 void PauseLogging();
                /// <summary>
                /// ResumeLogging allows logging to resume .ResumeLogging should be called ONLY after a successful call to PauseLogging
                /// </summary>
                 void ResumeLogging();
                /// <summary>
                /// Sets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                /// </summary>
                /// <param name="level">Value of level. Set to 'ERROR' for minimal logging. Set to 'DEBUG' for maximum logging</param>
                 void SetLogLevel(ProcessLogLevel level);
                 /// <summary>
                 /// Gets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                 /// </summary>
                 /// <returns>Value of level.</returns>
                 ProcessLogLevel GetLogLevel();
                /// <summary>
                /// Writes a log message to file
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="message">Log message</param>
                /// <param name="method">Method name where log message originated.This is set automatically.</param>
                /// <param name="lineNumber">Line number where log message originated.This is set automatically.</param>
                 void Log(ProcessLogLevel level, string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0);
                /// <summary>
                /// Writes a formatted log message to file
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="formattedMessage">Message containing tokens for parameters eg "Value of XXX is {0} .Value of YYY is {1}" where {0} and {1} are first and second values in 'args' array00</param>
                /// <param name="args">Array of arguments to write to log file</param>
                /// <param name="method">Method name where log message originated.This is set automatically.</param>
                /// <param name="lineNumber">Line number where log message originated.This is set automatically.</param>
                 void Log(ProcessLogLevel level, string formattedMessage, object[] args, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0);
            }
        }
    }
}

