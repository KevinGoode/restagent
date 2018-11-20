/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: WebServiceConfigHelper.cs

  DESCRIPTION: 
  ------------
  This file contains methods to initialize Web service configurations

  USAGE INSTRUCTIONS:
  -------------------
  WebServiceConfigHelper.InitDefaultConfiguration();
  
  CHANGE HISTORY:
  --------------- 
  07/04/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: June, 2015
********************************************************************/

using System;
using Microsoft.Win32;
using System.Security.Permissions;
using System.IO;
using Kge.Agent.Lang;
using Kge.Agent.Library;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /* Helper class to initialize the webServerConfiguration parameter class */
                public static class WebServiceConfigHelper
                {
                    private static WebServerConfig mServerConfig = new WebServerConfig();
                    public static WebServerConfig serverConfig
                    {
                        get { return mServerConfig; }
                        internal set { mServerConfig = value; }
                    }

                    /*
                      Method: InitConfiguration
                      Description: Initializes web service configuration parameters
                      Arguments: 
                                None
                      Returns:
                                None
                    */
                    public static void InitConfiguration()
                    {
                        serverConfig.host = Helper.GetLocalHostName();
                        serverConfig.name = WebConstants.SERVICE_NAME;
                        serverConfig.description = WebConstants.SERVICE_DESC;


                        try
                        {
                            var commonRegistry = new CommonRegistry();

                            if (!commonRegistry.Exists || string.IsNullOrWhiteSpace(commonRegistry.ServicePort))
                            {
                                commonRegistry.ServicePort = serverConfig.port;
                                commonRegistry.InstallPath = serverConfig.installPath;
                                commonRegistry.LogPath = serverConfig.logPath;
                                commonRegistry.LogLevel = serverConfig.logLevel;
                                commonRegistry.TimeOut = serverConfig.timeOut;
                            }

                            serverConfig.port = commonRegistry.ServicePort;
                            serverConfig.installPath = commonRegistry.InstallPath;
                            serverConfig.logPath = commonRegistry.LogPath;
                            serverConfig.logLevel = commonRegistry.LogLevel;
                            serverConfig.timeOut = commonRegistry.TimeOut;
                            serverConfig.protocol = WebConstants.LOCAL_HOST_URL;


                            ProcessLogger.CreateInstance("agent", serverConfig.logPath, 5, 1024);
                            SetLogLevel(serverConfig.logLevel);
                            LogServerConfigParameters();
                        }
                        catch (Exception ex)
                        {
                            //TODO no win events
                            //WinEvent.ErrorEvent(WebConstants.SERVICE_NAME, "CORE_ERROR_GENERAL_EXCEPTION", ex.Message);
                            throw;
                        }
                    }
                    
                    public static void SetLogLevel(string logLevel)
                    {
                        try
                        {
                            uint parsedLogLevel = UInt32.Parse(logLevel);
                            if (parsedLogLevel <= (uint)ProcessLogLevel.Debug)
                            {
                                ProcessLogger.GetInstance().SetLogLevel((ProcessLogLevel)parsedLogLevel);
                            }
                            else
                            {
                                Log.Warning(string.Format("Log level in registry is out of range: {0}", serverConfig.logLevel));
                            }
                        }
                        catch (FormatException)
                        {
                            Log.Warning(string.Format("Log level in registry is not a parseable numeral: {0}", serverConfig.logLevel));
                        }
                        catch (OverflowException)
                        {
                            Log.Warning(string.Format("Log level in registry is out of range: {0}", serverConfig.logLevel));
                        }
                    }

                    /*
                      Method: LogServerConfigParameters
                      Description: Logs web server configuration parameters
                      Arguments: 
                                None
                      Returns:
                                None
                    */
                    static void LogServerConfigParameters()
                    {
                        Log.Debug("Printing-: Web server config parameters");
                        Log.Debug("Service Name: " + serverConfig.name);
                        Log.Debug("Service Desciption: " + serverConfig.description);
                        Log.Debug("Service Version: " + serverConfig.version);
                        Log.Debug("Service Port: " + serverConfig.port.ToString());
                        Log.Debug("Service Installation path: " + serverConfig.installPath);
                        Log.Debug("Service Log path: " + serverConfig.logPath);
                        Log.Debug("Service Log level: " + serverConfig.logLevel.ToString());
                    }
                }
            }
        }
    }
}
