using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Rest.Server.API;
using Kge.Agent.Lang;
using Kge.Agent.Library;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace API
                {
                    public class APIRouter : IAPIRouter
                    {
                        #region Events

                        public event RouteHandler OnRouteRetrieved;

                        #endregion

                        public int RouteCount { get { return routesDictionnary.Count; } }
                        public int PluginCount { get { return routesDictionnary.GroupBy(x => x.Value.Item1).Select(x => x.First()).Count(); } }

                        protected Dictionary<APIRouteDescription, Tuple<IPlugin, MethodInfo>> routesDictionnary { get; set; }
                        protected ResourceIdMatcher resourceMatcher { get; set; }

                        public APIRouter()
                        {
                            routesDictionnary = new Dictionary<APIRouteDescription, Tuple<IPlugin, MethodInfo>>();
                            resourceMatcher = new ResourceIdMatcher();
                        }

                        /// <summary>
                        /// Registers all the APIs contained in the plugin in the route table.
                        /// </summary>
                        /// <param name="plugin"></param>
                        public void AddPlugin(IPlugin plugin)
                        {
                            foreach (PluginAPI pluginAPI in plugin.PluginAPIs)
                            {
                                var key = new APIRouteDescription(
                                    pluginAPI.UriTemplate,
                                    pluginAPI.QueryMethod);

                                if (ContainsAPIRouteDescription(key))
                                {
                                    Debug.WriteLine("Error same key found in the route dictionnary");
                                }
                                else
                                {
                                    var valueTuple = new Tuple<IPlugin, MethodInfo>(plugin, pluginAPI.MethodInfo);
                                    routesDictionnary.Add(key, valueTuple);
                                }
                            }
                        }

                        /// <summary>
                        ///  Unregisters all the APIs contained into the plugin from the route table.
                        /// </summary>
                        /// <param name="plugin"></param>
                        public void RemovePlugin(IPlugin plugin)
                        {
                            var keys = routesDictionnary.Where(x => x.Value.Item1 == plugin).Select(x => x.Key).ToList();

                            if (keys == null || keys.Count() == 0)
                            {
                                throw new Exception("Plugin not found");
                            }

                            foreach (var key in keys)
                            {
                                routesDictionnary.Remove(key);
                            }
                        }

                        /// <summary>
                        /// Checks if a plugin is registered.
                        /// </summary>
                        /// <param name="plugin"></param>
                        /// <returns></returns>
                        public bool ContainsPlugin(IPlugin plugin)
                        {
                            return routesDictionnary.Any(x => x.Value.Item1 == plugin);
                        }

                        /// <summary>
                        /// Removes all the plugins and routes from the router.
                        /// </summary>
                        public void Clear()
                        {
                            routesDictionnary.Clear();
                        }

                        /// <summary>
                        /// Manually adds a new route into the router.
                        /// </summary>
                        /// <param name="ard">This is the key</param>
                        /// <param name="instance">The plugin instance</param>
                        /// <param name="methodInfo">The associated method from the plugin</param>
                        public void AddRoute(APIRouteDescription ard, IPlugin instance, MethodInfo methodInfo)
                        {
                            if (ContainsAPIRouteDescription(ard))
                            {
                                throw new Exception("Route already exists");
                            }

                            var valueTuple = new Tuple<IPlugin, MethodInfo>(instance, methodInfo);

                            routesDictionnary.Add(ard, valueTuple);
                        }

                        /// <summary>
                        /// Manually removes a new route into the router.
                        /// </summary>
                        /// <param name="ard">This is the key</param>
                        /// <param name="instance">The plugin instance</param>
                        /// <param name="methodInfo">The associated method from the plugin</param>
                        public void RemoveRoute(APIRouteDescription ard)
                        {
                            if (ContainsAPIRouteDescription(ard) == false)
                            {
                                throw new Exception("Route not found");
                            }

                            var keys = routesDictionnary.Where(x => x.Key.Equals(ard)).Select(x => x.Key).ToList();
                            foreach (var key in keys)
                                routesDictionnary.Remove(key);
                        }

                        /// <summary>
                        /// Manually checks if a route is present in the router.
                        /// </summary>
                        /// <param name="ard"></param>
                        /// <returns></returns>
                        public bool ContainsRoute(APIRouteDescription ard)
                        {
                            return ContainsAPIRouteDescription(ard);
                        }

                        /// <summary>
                        /// Find an API that can be identified to the URI and http method
                        /// </summary>
                        /// <param name="uriTemplate"></param>
                        /// <param name="queryMethod"></param>
                        /// <returns></returns>
                        public ResolvedRoute GetRoute(string uri, string queryMethod)
                        {
                            
                            var queryHandler = new QueryParametersHandler();
                            var queryParameters = queryHandler.ConsumeQueryParameters(ref uri);
                    
                            APIRouteDescription ard = removeAPIRoot(uri, queryMethod);
                            string uriTemplate  = ard.UriTemplate;
                            if (ContainsAPIRouteDescription(ard) == false)
                            {
                                throw getNotFound(uriTemplate);
                            }

                            {
                                // Fires event
                                var handler = OnRouteRetrieved;
                                if (handler != null)
                                {
                                    var binder = Find(ard);
                                    handler(this, binder.Value.Item1, binder.Value.Item2);
                                }
                            }

                            var ret = Find(uriTemplate, queryMethod);
                            var resourcesIds = resourceMatcher.ExtractResourceIds(ret.Key.UriTemplate, uriTemplate);

                            return new ResolvedRoute(ret.Value.Item1, ret.Value.Item2, resourcesIds, queryParameters);
                        }

                        protected KeyValuePair<APIRouteDescription, Tuple<IPlugin, MethodInfo>> Find(APIRouteDescription ard)
                        {
                            if (ContainsAPIRouteDescription(ard) == false)
                                return new KeyValuePair<APIRouteDescription, Tuple<IPlugin, MethodInfo>>(null, null);
                            var ret = routesDictionnary.SingleOrDefault(x => x.Key.QueryMethod == ard.QueryMethod && resourceMatcher.Match(x.Key.UriTemplate, ard.UriTemplate));
                            return ret;
                        }

                        protected KeyValuePair<APIRouteDescription, Tuple<IPlugin, MethodInfo>> Find(string uriTemplate, string queryMethod)
                        {
                            if (ContainsAPIRouteDescription(uriTemplate, queryMethod) == false)
                                return new KeyValuePair<APIRouteDescription, Tuple<IPlugin, MethodInfo>>(null, null);
                            var ret = routesDictionnary.SingleOrDefault(x => x.Key.QueryMethod == queryMethod && resourceMatcher.Match(x.Key.UriTemplate, uriTemplate));
                            return ret;
                        }

                        protected bool ContainsAPIRouteDescription(APIRouteDescription ard)
                        {
                            return routesDictionnary.Any(x => x.Key.QueryMethod == ard.QueryMethod && resourceMatcher.Match(x.Key.UriTemplate, ard.UriTemplate));
                        }

                        protected bool ContainsAPIRouteDescription(string uriTemplate, string queryMethod)
                        {
                            return routesDictionnary.Any(x => x.Key.QueryMethod == queryMethod && resourceMatcher.Match(x.Key.UriTemplate, uriTemplate));
                        }
                        protected WebResponseException getNotFound(string uriTemplate){

                            return new WebResponseException(new HttpsErrorResponse(HttpStatusCode.NotFound,
                                                            Helper.SetAppTypeInErrorMessage(  404,
                                                            LanguageHelper.Resolve("CORE_ERROR_URI_NOT_FOUND", uriTemplate))),
                                                            HttpStatusCode.NotFound);
                        }
                        protected APIRouteDescription removeAPIRoot(string uri,  string queryMethod){
                            CommonRegistry registry = new CommonRegistry();
                            string rootNameSpace="/" + registry.RootNamespace;
                            if(!uri.StartsWith(rootNameSpace))
                            {
                                 throw getNotFound(uri);
                            }
                            string trimmedUri=uri.Replace(rootNameSpace,"");
                            return new APIRouteDescription(trimmedUri, queryMethod);
                        }
                    }
                }
            }
        }
    }
}
