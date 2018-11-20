/*******************************************************************
 (C) Copyright 2015 Kge..
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
using System.Threading;
using Microsoft.Win32;
using Kge.Agent.Library;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                // All common helper functions goes here
                public class ResourceLockHelper
                {
                    private string resource { get; set; }
                    Mutex mutex { get; set; }

                    //Create lock on resource 
                    public static ResourceLockHelper CreateResourceLock(string resourceName)
                    {
                        ResourceLockHelper resourceLock = new ResourceLockHelper();
                        try
                        {
                            // Try to open existing mutex.
                            resourceLock.mutex = Mutex.OpenExisting(resourceName);
                            Log.Debug("Mutex with name:  " + resourceName + " exist, and opened");
                        }
                        catch (WaitHandleCannotBeOpenedException)
                        {
                            Log.Debug("Mutex with name:  " + resourceName + " does not exist");
                            // If exception occurred, there is no such mutex.
                            resourceLock.mutex = new Mutex(false, resourceName);
                            Log.Debug("Mutex with name:  " + resourceName + " created");
                        }
                        catch (Exception)
                        {
                            Log.Error("Error while creating Mutex: " + resourceName);
                            throw;
                        }
                        resourceLock.resource = resourceName;
                        return (resourceLock);
                    }

                    //Acquires lock on resource 
                    public void GetLockOnResource(int timeOut)
                    {
                        ConfigParameters configParameters = new ConfigParameters();
                        string temp = configParameters.Timeout;
                        if(temp != null)
                        {
                            
                            timeOut = Int32.Parse(temp);
                            timeOut = timeOut * 1000; //Convert into milliseconds
                        }
                        Log.Debug("Waiting for lock on Mutex " + resource);
                        mutex.WaitOne(timeOut);
                        Log.Debug("Acquired lock on Mutex " + resource);
                    }

                    //Acquires lock on resource 
                    public void GetLockOnResource()
                    {
                        GetLockOnResource(60000);
                    }

                    //Releases lock on resource 
                    public void ReleaseLockOnResource()
                    {
                        if (mutex != null)
                        {
                            mutex.ReleaseMutex();
                            Log.Debug("Mutex with name: " + resource + " released");
                        }
                    }

                    //Disposes lock on resource 
                    public void DisposeLockOnResource()
                    {
                        if (mutex != null)
                        {
                            mutex.Dispose();
                            Log.Debug("Mutex with name: " + resource + " disposed");
                        }
                    }

                    //CLoses lock on resource 
                    public void CLoseLockOnResource()
                    {
                        if (mutex != null)
                        {
                            mutex.Close();
                            Log.Debug("Mutex with name: " + resource + " closed");
                        }
                    }
                }
            }
        }
    }
}
