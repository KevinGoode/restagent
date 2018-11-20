using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            /* Config parameters Web service */
            public class ConfigParameters
            {
                
                public  string InstallPath = "./";
                public  string LogPath ="./Log";
                public  string Port = "50002";
                public  string Version = "6.0";
                public  string TraceLevel="4";
                public  string Timeout= "120";
                public  string RestoreTimeout="10";
                public  string RootNamespace="agent";
                public  string ReportFilePath="./";
            }

            public class CommonRegistry : ICommonRegistry
            {
                public bool Exists
                {               
                    get 
                    { 
                        return File.Exists(REG_PATH);
                    }
                }
                 public string ReportFilePath
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.ReportFilePath;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.ReportFilePath = value;
                       writeParams(configParameters);
                    }
                }
                public string RootNamespace
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.RootNamespace;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.RootNamespace = value;
                       writeParams(configParameters);
                    }
                }
                public string InstallPath
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.InstallPath;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.InstallPath = value;
                       writeParams(configParameters);
                    }
                }

                public string LogPath
                {
                     get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.LogPath;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.LogPath = value;
                       writeParams(configParameters);
                    }
                }

                public string ServicePort
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.Port;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.Port = value;
                       writeParams(configParameters);
                    }
                }

                public string LogLevel
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.TraceLevel;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.TraceLevel = value;
                       writeParams(configParameters);
                    }
                }

                public string TimeOut
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.Timeout;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.Timeout = value;
                       writeParams(configParameters);
                    }
                }

                public string RestoreTimeOut
                {
                    get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.RestoreTimeout;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.RestoreTimeout = value;
                       writeParams(configParameters);
                    }
                }

                public string Version
                {
                     get
                    {
                        ConfigParameters configParameters  = readParams();
                        return configParameters.Version;
                    }
                    set
                    {
                       ConfigParameters configParameters  = readParams();
                       configParameters.Version = value;
                       writeParams(configParameters);
                    }
                }
                private ConfigParameters readParams()
                {
                    ConfigParameters configParams =  new ConfigParameters();
                    if(Exists) configParams = JsonConvert.DeserializeObject<ConfigParameters>(File.ReadAllText(REG_PATH));
                    return configParams;
                }
                private void writeParams(ConfigParameters configParameters)
                {
                    File.WriteAllText(REG_PATH, JsonConvert.SerializeObject(configParameters));
                }
                private  const string REG_PATH = "./config.json";
            }
        }
    }
}