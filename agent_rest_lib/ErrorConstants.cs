/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.
 
  FILE NAME: ErrorConstants.cs

  DESCRIPTION: 
  ------------
  This file contains all Error constants used in diff projects 

  USAGE INSTRUCTIONS:
 -------------------
  N/A
  
  CHANGE HISTORY:
  --------------- 
  07/05/2015 – New file
  
  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System;
/*Name space for all common and application error constants */

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                /* All Common error constants defined here */
                public static class CommonErrors
                {
                    public const UInt32 ERROR_SUCCESS = 0;
                    public const UInt32 ERROR_UNDEFINED = 1100;
                    public const UInt32 ERROR_SYSTEM = 1101;
                    public const UInt32 ERROR_WEB_CONFIG = 1102;
                    public const UInt32 ERROR_UNAUTH_ACCESS = 1103;
                    public const UInt32 ERROR_INVALID_USR_CREDENTIAL = 1104;
                    public const UInt32 ERROR_ACCESS_TOKEN = 1105;
                    public const UInt32 ERROR_EXPIRED_TOKEN = 1106;
                    public const UInt32 ERROR_COMMAND_INIT = 1107;
                    public const UInt32 ERROR_PARAMS_VALIDATION = 1108;
                    public const UInt32 ERROR_IN_APP_REGISTRY = 1109;
                    public const UInt32 ERROR_IN_APP_INSTALLATION = 1110;
                    public const UInt32 ERROR_CMD_EXEC_PROCESS = 1111;
                    public const UInt32 ERROR_TASK_TRACKER = 1112;
                    
                }
               
            }
        }
    }
}