/*
 * (C) Copyright 2016 Kge
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;


namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            
            /// <summary>
            /// ProcessLogger logs messages to log file. Class is thread safe. Logging can be paused and resumed via the 'PauseLogging' and 'ResumeLogging'
            /// APIS. These APIS shall typically be used during support ticket generation or log file rotation. The ProcessLogger shall only log a message
            /// if the loglevel of the message is greater than or equal to the 'CurrentLevel' . The 'CurrentLevel' can be set at runtime to give more detailed
            /// or less detail in logging as required.
            /// 
            /// The ProcessLogger uses the singleton design pattern
            /// </summary>
            public class ProcessLogger : IProcessLogger
            {
                /// <summary>
                /// SingleInstance of clss
                /// </summary>
                static private IProcessLogger SingleInstance = null;

                /// <summary>
                /// Underlying simple logger
                /// </summary>
                private SimpleLogger LoggerNetInstance = null;
                /// <summary>
                /// Creates single instance of this class
                /// </summary>
                /// <param name="filePath">Fully qualified path of logfile</param>
                static public void CreateInstance(string fileName, string filePath, uint numFiles, uint maxSize)
                {
                    if (SingleInstance == null)
                    {
                        SingleInstance = new ProcessLogger( fileName,  filePath, numFiles, maxSize);
                    }
                }
                /// <summary>
                /// Returns the one and only single instance of this class
                /// </summary>
                /// <returns>Single instance of this class</returns>
                static public IProcessLogger GetInstance()
                {
                    return SingleInstance;
                }
                /// <summary>
                /// Returns path of logfile
                /// </summary>
                /// <returns>Fully qualified path of log file</returns>
                public string GetFilePath()
                {
                    return LoggerNetInstance.GetFilePath();
                }
                /// <summary>
                /// Returns prefix of logfile
                /// </summary>
                /// <returns>Fully qualified prefix of log file</returns>
                public string GetFilePrefix()
                {
                    return LoggerNetInstance.GetFilePrefix();
                }
                /// <summary>
                /// PauseLogging pauses logging until the 'ResumeLogging' API is called. PauseLogging only returns when the current log write has finished
                /// and the logger is in a paused. 
                /// </summary>
                public void PauseLogging() 
                {
                    LoggerNetInstance.PauseLogging();
                }
                /// <summary>
                /// ResumeLogging allows logging to resume .ResumeLogging should be called ONLY after a successful call to PauseLogging
                /// </summary>
                public void ResumeLogging()
                {
                    LoggerNetInstance.ResumeLogging();
                }
               
                /// <summary>
                /// Sets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                /// </summary>
                /// <param name="level">Value of level. Set to 'ERROR' for minimal logging. Set to 'DEBUG' for maximum logging</param>
                public void SetLogLevel(ProcessLogLevel level)
                {
                    
                    LoggerNetInstance.SetLogLevel(level);
                }
                 /// <summary>
                 /// Gets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                 /// </summary>
                 /// <returns>Value of level.</returns>
                public ProcessLogLevel GetLogLevel()
                 {
                     return (ProcessLogLevel) LoggerNetInstance.GetLogLevel();
                 }
                /// <summary>
                /// Writes a log message to file
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="message">Log message</param>
                /// <param name="method">Method name where log message originated.This is set automatically. </param>
                /// <param name="lineNumber">Line number where log message originated.This is set automatically. </param>
                public void Log(ProcessLogLevel level, string message, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0)
                { 
                    Write(level, message,  method , lineNumber);
                }
                /// <summary>
                /// Writes a formatted log message to file
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="formattedMessage">Message containing tokens for parameters eg "Value of XXX is {0} .Value of YYY is {1}" where {0} and {1} are first and second values in 'args' array00</param>
                /// <param name="args">Array of arguments to write to log file</param>
                /// <param name="method">Method name where log message originated.This is set automatically. </param>
                /// <param name="lineNumber">Line number where log message originated.This is set automatically. </param>
                public void Log(ProcessLogLevel level, string formattedMessage, object[] args, [CallerMemberNameAttribute] string method = "", [CallerLineNumber] int lineNumber = 0) 
                { 
                    if (args !=null)
                    {
                        StringWriter sw = new StringWriter();
                        sw.Write(formattedMessage, args);
                        Write(level,sw.ToString(), method, lineNumber);
                    }
                    else
                    {
                        Write(level, formattedMessage, method, lineNumber);
                    }
                }
                /// <summary>
                /// Private constructor definition that precludes external code from instantiating this class. Not used
                /// </summary>
                private ProcessLogger()
                {

                }
                /// <summary>
                /// Private constructor definition that precludes external code from instantiating this class
                /// </summary>
                private ProcessLogger(string fileName, string filePath,uint numFiles,uint MaxSize)
                {

                    SimpleLogger.CreateInstance(fileName, filePath, numFiles, MaxSize);
                    LoggerNetInstance = SimpleLogger.GetInstance();

                }
               
                /// <summary>
                /// Writes message
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="message">Log message</param>
                /// <param name="method">Method name where log message originated. </param>
                /// <param name="lineNumber">Line number where log message originated. </param>
                private void Write(ProcessLogLevel level, string message, string method, int lineNumber)
                {

                    LoggerNetInstance.Write(level,  message,  method,  lineNumber);
                }
               
               

            }
        }
    }
}

