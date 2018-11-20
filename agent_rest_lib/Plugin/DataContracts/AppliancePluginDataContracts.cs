using Kge.Agent.Rest.Library.Plugin.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Library
            {
                namespace Plugin
                {
                    namespace DataContracts
                    {
                        namespace Appliance
                        {
                            [DataContract]
                            public class ApplianceResponse : IGenericResponse
                            {
                                [DataMember(IsRequired = true)]
                                public ApplianceDetails applianceDetails { get; set; }
                            }

                            [DataContract]
                            public class ApplianceDetails
                            {
                                [DataMember(IsRequired = true)]
                                public string applianceId { get; set; }

                                [DataMember(IsRequired = true)]
                                public string hostName { get; set; }

                                [DataMember(IsRequired = true)]
                                public string domainName { get; set; }

                                [DataMember(IsRequired = true)]
                                public Capability[] capabilities { get; set; }

                                [DataMember(IsRequired = true)]
                                public string windowsVersion { get; set; }

                                [DataMember(IsRequired = true)]
                                public string productVersion { get; set; }
                            }

                            [DataContract]
                            public class PluginsResponse : IGenericResponse
                            {
                                [DataMember(IsRequired = true)]
                                public PluginDetails[] plugins { get; set; }
                            }

                            [DataContract]
                            public class PingResponse : IGenericResponse
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}
