using Kge.Agent.Rest.Library.Plugin.DataContracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
                public class JsonWCFSerializer : IWCFSerializer
                {
                    public string Serialize(object obj)
                    {
                        if (obj == null)
                            return String.Empty;
                        return JsonConvert.SerializeObject(obj);
                    }

                    public object Deserialize(string json, Type toType)
                    {
                        object deserializedObject = null;

                        var settings = new JsonSerializerSettings();
                        settings.MissingMemberHandling = MissingMemberHandling.Error;
                        settings.NullValueHandling = NullValueHandling.Ignore;

                        List<WCFSerializerExceptions> exceptions = new List<WCFSerializerExceptions>();

                        settings.Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                        {
                            args.ErrorContext.Handled = true;
                            if (exceptions.Where(x => x is MemberTypeException).Any(x => x.MemberName == args.ErrorContext.Member.ToString()))
                                return;

                            exceptions.Add(SearchAppropriateException(args));
                        };

                        deserializedObject = JsonConvert.DeserializeObject(json, toType, settings);

                        if (exceptions.Count > 0)
                            throw exceptions.OrderBy(x => x.Priority).First();

                        return deserializedObject;
                    }

                    public object DeserializeObject(string json)
                    {
                        return JsonConvert.DeserializeObject(json);
                    }

                    protected WCFSerializerExceptions SearchAppropriateException(Newtonsoft.Json.Serialization.ErrorEventArgs args)
                    {
                        if (args.ErrorContext.Error.Message.StartsWith("Required property"))
                            return new MissingMemberException(args);
                        else if (args.ErrorContext.Error.Message.StartsWith("Could not find member"))
                            return new ExtraMemberException(args);
                        else if (args.ErrorContext.Error.Message.IndexOf("convert", StringComparison.OrdinalIgnoreCase) >= 0)
                            return new MemberTypeException(args);
                        return new JsonSyntaxException(args);
                    }
                }
            }
        }
    }
}