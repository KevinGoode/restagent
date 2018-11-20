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
                    namespace Providers
                    {
                        [DataContract]
                        public class PluginDetails
                        {
                            [DataMember(IsRequired = true)]
                            public string name { get; set; }

                            [DataMember(IsRequired = true)]
                            public string version { get; set; }

                            [DataMember(IsRequired = true)]
                            public Capability[] capabilities { get; set; }
                        }
                    }
                }
            }
        }
    }
}