using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Library
        {
            namespace SystemWrapper
            {
                namespace Newtonsoft
                {
                    namespace Json
                    {
                        [ExcludeFromCodeCoverage]
                        public class JsonConvertStaticWrapper
                        {
                            public virtual string SerializeObject(object value)
                            {
                                return JsonConvert.SerializeObject(value);
                            }

                            public virtual object DeserializeObject(string value, Type type)
                            {
                                return JsonConvert.DeserializeObject(value, type);
                            }
                        }
                    }
                }
            }
        }
    }
}