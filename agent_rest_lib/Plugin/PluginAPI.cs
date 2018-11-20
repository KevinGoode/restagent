using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
                    /// <summary>
                    /// Represents an API from a Plugin
                    /// </summary>
                    public class PluginAPI
                    {
                        public MethodInfo MethodInfo { get; private set; }
                        public string UriTemplate { get; private set; }
                        public string QueryMethod { get; private set; }
                        public Type RequestType { get; private set; }
                        public Type ResponseType { get; private set; }
                        public RestrictionLevel Restriction { get; private set; }
                        public bool Asynchronous { get; private set; }

                        public PluginAPI(MethodInfo methodInfo, string uriTemplate, string queryMethod, RestrictionLevel restriction, bool asynchronous)
                        {
                            MethodInfo = methodInfo;
                            UriTemplate = uriTemplate;
                            QueryMethod = queryMethod;

                            ResponseType = MethodInfo.ReturnType;

                            if (MethodInfo.GetParameters().Count() > 0)
                                RequestType = MethodInfo.GetParameters().First().ParameterType;
                            else
                                RequestType = null;

                            Restriction = restriction;
                            Asynchronous = asynchronous;
                        }

                        public PluginAPI(MethodInfo methodInfo, string uriTemplate, string queryMethod, RestrictionLevel restriction)
                        {
                            MethodInfo = methodInfo;
                            UriTemplate = uriTemplate;
                            QueryMethod = queryMethod;

                            ResponseType = MethodInfo.ReturnType;

                            if (MethodInfo.GetParameters().Count() > 0)
                                RequestType = MethodInfo.GetParameters().First().ParameterType;
                            else
                                RequestType = null;

                            Restriction = restriction;
                            Asynchronous = false;
                        }

                        public PluginAPI(MethodInfo methodInfo, string uriTemplate, string queryMethod)
                        {
                            MethodInfo = methodInfo;
                            UriTemplate = uriTemplate;
                            QueryMethod = queryMethod;

                            ResponseType = MethodInfo.ReturnType;

                            if (MethodInfo.GetParameters().Count() > 0)
                                RequestType = MethodInfo.GetParameters().First().ParameterType;
                            else
                                RequestType = null;

                            Restriction = RestrictionLevel.Admin;
                            Asynchronous = false;
                        }
                    }
                }
            }
        }
    }
}