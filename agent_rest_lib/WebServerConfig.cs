/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: WebServerConfig.cs

  DESCRIPTION: 
  ------------
  This file contains Web server configuration class

  USAGE INSTRUCTIONS:
  -------------------
  new WebServerConfig();
  
  CHANGE HISTORY:
  --------------- 
  07/05/2015 – New file
 
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Runtime.Serialization;


namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /* Class having all configuration parameter of REST server */
                public class WebServerConfig
                {
                    //Defaults

                    private string configPort = "50002";
                    private string configInstallPath = "C:\\temp";
                    private string configLogPath = "C:\\temp\\log";
                    private string configlogLevel = "5";
                    private string configTimeOut = "120";
                    private string configProtocol = WebConstants.LOCAL_HOST_URL_INSECURE;

                    /* Service intilization parameters */
                    [DataMember(Order = 1)]
                    public string host { get; set; }
                    [DataMember(Order = 2)]
                    public string name { get; set; }
                    [DataMember(Order = 3)]
                    public string description { get; set; }
                    [DataMember(Order = 4)]
                    public string version { get; set; }
                    [DataMember(Order = 5)]
                    public string port { get { return configPort; } set { configPort = value; } }
                    [DataMember(Order = 6)]
                    public string installPath { get { return configInstallPath; } set { configInstallPath = value; } }
                    [DataMember(Order = 6)]
                    public string logPath { get { return configLogPath; } set { configLogPath = value; } }
                    [DataMember(Order = 8)]
                    public string logLevel { get { return configlogLevel; } set { configlogLevel = value; } }
                    [DataMember(Order = 9)]
                    public string timeOut { get { return configTimeOut; } set { configTimeOut = value; } }
                    [DataMember(Order = 9)]
                    public string protocol { get { return configProtocol; } set { configProtocol = value; } }
                }
            }
        }
    }
}
