/*******************************************************************
 (C) Copyright 2015 Kge.
  All Rights Reserved.

  FILE NAME: GlobalConstants.cs

  DESCRIPTION:
  ------------
  This file contains all common global constants used in diff projects 

  USAGE INSTRUCTIONS:
 -------------------
  N/A

  CHANGE HISTORY:
  ---------------
  07/05/2015 – New file

  AUTHOR: Kge.
  DATE LAST MODIFIED: July, 2015
********************************************************************/

using System.ComponentModel;

/* Name space having all global constants used in web service code */
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                

                /* Enum of all command types supported Web service code */
                public enum CmdTypes
                {
                    [Description("analyze")]
                    analyze = 1,
                    [Description("backup")]
                    backup,
                    [Description("create")]
                    create,
                    [Description("deletebackup")]
                    deletebackup,
                    [Description("display")]
                    display,
                    [Description("inservparam")]
                    inservparam,
                    [Description("license")]
                    license,
                    [Description("listbackup")]
                    listbackup,
                    [Description("listhost")]
                    listhost,
                    [Description("mail")]
                    mail,
                    [Description("mount")]
                    mount,
                    [Description("policy")]
                    policy,
                    [Description("portconfig")]
                    portconfig,
                    [Description("remove")]
                    remove,
                    [Description("report")]
                    report,
                    [Description("repository")]
                    repository,
                    [Description("restore")]
                    restore,
                    [Description("rescan")]
                    rescan,
                    [Description("setvc")]
                    setvc,
                    [Description("timeconfig")]
                    timeconfig,
                    [Description("update")]
                    update,
                    [Description("unmount")]
                    unmount
                }
                /* Mapping of REST parameters name, used for command line */
                public static class ParamsLookup
                {
                    public const string appHost = @"appHost";
                    public const string backupServer = @"backupServer";
                    public const string appServer = @"appServer";
                    public const string appServers = @"appServers";
                    public const string targetServer = @"targetServer";
                    public const string masterServer = @"masterServer";
                    public const string mediaServer = @"mediaServer";
                    public const string appInstance = @"appInstance";
                    public const string appDatabase = @"appDatabase";
                    public const string policy = @"policy";
                    public const string schedule = @"schedule";
                    public const string syncFlag = @"syncFlag";
                    public const string forceFlag = @"forceFlag";
                    public const string timeStamp = @"timeStamp";
                    public const string retainFlag = @"retainFlag";
                    public const string datalist = @"datalist";
                    public const string backupExecFlag = @"backupExecFlag";
                    public const string device = @"device";
                    public const string no_email = @"no_email";
                    public const string waitTime = @"waitTime";
                    public const string expireTime = @"expireTime";
                    public const string license = @"license";
                    public const string mountPoint = @"mountPoint";
                    public const string attachDatabase = @"attachDatabase";
                    public const string queryStatus = @"queryStatus";
                    public const string retainTime = @"retainTime";
                    public const string retentionTime = @"retentionTime";
                    public const string queryStatFlag = @"queryStatFlag";
                    public const string instanceVCLimit = @"instanceVCLimit";
                    public const string databaseVCLimit = @"databaseVCLimit";
                    public const string port = @"port";
                    public const string directory = @"directory";
                    public const string altDestination = @"altDestination";
                    public const string dpFlag = @"dpFlag";
                    public const string cloneDatabase = @"cloneDatabase";
                    public const string alterLocation = @"alterLocation";
                    public const string snapFlag = @"snapFlag";
                    public const string norecoveryFlag = @"norecoveryFlag";
                    public const string promoteTime = @"promoteTime";
                    public const string backupTime = @"backupTime";
                    public const string analyzeInterval = @"analyzeInterval";
                    public const string cleanFlag = @"cleanFlag";
                    public const string detachFlag = @"detachFlag";
                    public const string noMailFlag = @"noMailFlag";
                    public const string pointOfFailureFlag = @"pointOfFailureFlag";
                    public const string soFlag = @"soFlag";
                    public const string backupId = @"backupId";
                    public const string backupName = @"backupName";
                    public const string incrementalFlag = @"incrementalFlag";
                    public const string recoverysetId = @"recoverysetId";
                    public const string instanceStorOnceBkpLimit = @"instanceStorOnceBkpLimit";
                    public const string databaseStorOnceBkpLimit = @"databaseStorOnceBkpLimit";
                    public const string retainOldestStorOnceBkpFlag = @"retainOldestStorOnceBkpFlag";
                    public const string instanceStorOnceBkpPolicyId = @"instanceStorOnceBkpPolicyId";
                    public const string databaseStorOnceBkpPolicyId = @"databaseStorOnceBkpPolicyId";
                    public const string storageSystemId = @"storageSystemId";
                    public const string volumeNames = @"volumeNames";
                    public const string windowsHost = @"windowsHost";
                    public const string outputFolder = @"outputFolder";
                    public const string targetStorageSystemId = @"targetStorageSystemId";
                    public const string instanceCatalystCopyPolicyId = @"instanceCatalystCopyPolicyId";
                    public const string databaseCatalystCopyPolicyId = @"databaseCatalystCopyPolicyId";
                    public const string instanceCatalystCopyCounts = @"instanceCatalystCopyCounts";
                    public const string databaseCatalystCopyCounts = @"databaseCatalystCopyCounts";
                    public const string retainOldDatabaseCatalystCopyFlags = @"retainOldDatabaseCatalystCopyFlags";
                    public const string retainOldInstanceCatalystCopyFlags = @"retainOldInstanceCatalystCopyFlags";
                    public const string deleteInstance = @"deleteInstance";
                }

                /* Web service constants */
                public static class WebConstants
                {
                    public const string SERVICE_NAME = @"KGE Agent Web Extension";
                    public const string SERVICE_DESC = @"KGE Agent Web Extension";
                    public const string SERVICE_BASE_URL = @"/agent";
                    public const string LOCAL_HOST_URL = @"https://localhost:";
                    public const string LOCAL_HOST_URL_INSECURE = @"http://localhost:";
                    public const string LOCAL_HOST_STR = @"localhost";
                }

                /* Web service end points */
                public static class WebEndPoints
                {
                    public const string GENERAL_ENDPOINT = @"/";
                }

                public static class WebTask
                {
                    public const string TASK_URI = "task/{task_guid}";

                    public static string GenerateTaskUri(string task_guid)
                    {
                        return TASK_URI.Replace("{task_guid}", task_guid);
                    }

                    public static string GenerateTaskResultUri(string task_guid)
                    {
                        return TASK_URI.Replace("{task_guid}", task_guid) + "/result";
                    }
                }

                
                public static class AgentConstants
                {
                   
                    public const string STR_APP_TYPE_NAME = @"Agent";

                }
            }
        }
    }
}

