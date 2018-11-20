/*
 * (C) Copyright 2018 Kge
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
            /// SimpleLogger logs messages to log file. Class is thread safe. Logging can be paused and resumed via the 'PauseLogging' and 'ResumeLogging'
            /// APIS. These APIS shall typically be used during support ticket generation or log file rotation. The ProcessLogger shall only log a message
            /// if the loglevel of the message is greater than or equal to the 'CurrentLevel' . The 'CurrentLevel' can be set at runtime to give more detailed
            /// or less detail in logging as required.
            /// 
            /// The ProcessLogger uses the singleton design pattern
            /// </summary>
            public class SimpleLogger
            {
                /// <summary>
                /// SingleInstance of clss
                /// </summary>
                static private SimpleLogger SingleInstance = null;

                /// <summary>
                /// Underlying native logger
                /// </summary>
            

                /// <summary>
                /// Creates single instance of this class
                /// </summary>
                /// <param name="filePath">Fully qualified path of logfile</param>
                static public void CreateInstance(string fileName, string filePath, uint numFiles, uint maxSize)
                {
                    if (SingleInstance == null)
                    {
                        SingleInstance = new SimpleLogger( fileName,  filePath, numFiles, maxSize);
                    }
                }
                /// <summary>
                /// Returns the one and only single instance of this class
                /// </summary>
                /// <returns>Single instance of this class</returns>
                static public SimpleLogger GetInstance()
                {
                    return SingleInstance;
                }
                /// <summary>
                /// Returns path of logfile
                /// </summary>
                /// <returns>Fully qualified path of log file</returns>
                public string GetFilePath()
                {
                    return "simple.log";
                }
                /// <summary>
                /// Returns prefix of logfile
                /// </summary>
                /// <returns>Fully qualified prefix of log file</returns>
                public string GetFilePrefix()
                {
                    return "simple";
                }
                /// <summary>
                /// PauseLogging pauses logging until the 'ResumeLogging' API is called. PauseLogging only returns when the current log write has finished
                /// and the logger is in a paused. 
                /// </summary>
                public void PauseLogging() 
                {
                    //DONOTHING
                }
                /// <summary>
                /// ResumeLogging allows logging to resume .ResumeLogging should be called ONLY after a successful call to PauseLogging
                /// </summary>
                public void ResumeLogging()
                {
                    //DONOTHING
                }
               
                /// <summary>
                /// Sets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                /// </summary>
                /// <param name="level">Value of level. Set to 'ERROR' for minimal logging. Set to 'DEBUG' for maximum logging</param>
                public void SetLogLevel(ProcessLogLevel level)
                {
                    processLogLevel = level;
                }
                 /// <summary>
                 /// Gets the CurrentLevel of the ProcessLogger. By default the CurrentLevel shall be 'INFO'
                 /// </summary>
                 /// <returns>Value of level.</returns>
                public ProcessLogLevel GetLogLevel()
                 {
                     return processLogLevel;
                 }
               
                /// <summary>
                /// Private constructor definition that precludes external code from instantiating this class. Not used
                /// </summary>
                private SimpleLogger()
                {

                }
                /// <summary>
                /// Private constructor definition that precludes external code from instantiating this class
                /// </summary>
                private SimpleLogger(string fileName, string filePath,uint numFiles,uint MaxSize)
                {
                    //DONOTHING
                }
               
                /// <summary>
                /// Writes message
                /// </summary>
                /// <param name="level">Level associated with message. Log message shall only be written if level >= CurrentLevel </param>
                /// <param name="message">Log message</param>
                /// <param name="method">Method name where log message originated. </param>
                /// <param name="lineNumber">Line number where log message originated. </param>
                public void Write(ProcessLogLevel level, string message, string method, int lineNumber)
                {
                    if(level <=processLogLevel)
                    {
                        lock(syncLock)
                        {
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(GetFilePath(),true))
                            {
                                file.WriteLine( method + ":" + lineNumber.ToString()+ ":"+ message);
                            }
                        }
                    }
                }
               
                private static object syncLock = new object();
                private ProcessLogLevel processLogLevel=ProcessLogLevel.Info;
            }
        }
    }
}

