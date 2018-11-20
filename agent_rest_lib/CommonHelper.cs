/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: CommonHelper.cs

  DESCRIPTION: 
  ------------
  This file contains all common helper methods 

  USAGE INSTRUCTIONS:
 -------------------
  Helper.GetEnumDescription(Enum value)
  
  CHANGE HISTORY:
  --------------- 
  07/02/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/


using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                public static class Helper
                {
                    /// <summary>
                    /// Gets local host name
                    /// </summary>
                    /// <returns>Returns Hostname on success or null on failure</returns>
                    public static string GetLocalHostName()
                    {
                        string fqdnHostName = null;
                        var iFqdnProp = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                        if (iFqdnProp != null)
                        {
                            fqdnHostName = iFqdnProp.HostName;
                            if (!string.IsNullOrEmpty(iFqdnProp.DomainName))
                            {
                                fqdnHostName = fqdnHostName + "." + iFqdnProp.DomainName;
                            }
                        }
                        return (fqdnHostName);
                    }

                    /// <summary>
                    /// Replaces "Error" header with "Agent" in message
                    /// </summary>
                    /// <param name="errorCode"></param>
                    /// <param name="message"></param>
                    /// <returns></returns>
                    public static string SetAppTypeInErrorMessage(UInt32 errorCode, string message)
                    {
                        string result = message;
                       
                        result = AgentConstants.STR_APP_TYPE_NAME + " " + errorCode + ": " + result;
                        
                        return (result);
                    }
                }
            }

        }
    }
}
