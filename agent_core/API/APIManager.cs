using Kge.Agent.Lang;
using Kge.Agent.Library;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
                    public class APIManager : IAPIManager
                    {
                        public IAPIRouter APIRouter { get; set; }
                        protected IPluginLoader PluginLoader { get; set; }

                        protected List<IPlugin> Plugins { get; set; }
                        protected ResourceIdResolver ResourceResolver { get; set; }

                        public APIManager(IAPIRouter apiRouter, IPluginLoader pluginLoader)
                        {
                            APIRouter = apiRouter;
                            PluginLoader = pluginLoader;
                            Plugins = new List<IPlugin>();
                            ResourceResolver = new ResourceIdResolver();
                        }

                        /// <summary>
                        /// Imports all the dll contained in the targeted directory as plugins and
                        /// add their APIs to the routing system.
                        /// </summary>
                        /// <param name="path">Path to target directory</param>
                        public void DiscoverAPIDirectory(string path)
                        {
                            foreach (IPlugin plugin in PluginLoader.Discover(path))
                            {
                                AddPlugin(plugin);
                            }
                        }

                        /// <summary>
                        /// Imports a specified dll as a plugin and its APIs to the routing system.
                        /// </summary>
                        /// <param name="path">Path to a dll file</param>
                        public void DiscoverAPI(string path)
                        {
                            IPlugin plugin = PluginLoader.DiscoverPlugin(path);

                            AddPlugin(plugin);
                        }

                        /// <summary>
                        /// Adds a plugin and add its APIs to the routing system.
                        /// </summary>
                        /// <param name="plugin"></param>
                        public void AddPlugin(IPlugin plugin)
                        {
                            if (Plugins.Any(x => x.Name == plugin.Name))
                            {
                                throw new Exception("Plugin " + plugin.Name + " already registered");
                            }
                            else
                            {
                                Plugins.Add(plugin);
                                APIRouter.AddPlugin(plugin);
                            }
                        }

                        /// <summary>
                        /// Removes a plugin and remove its APIs from the routing system.
                        /// </summary>
                        /// <param name="plugin"></param>
                        public void RemovePlugin(IPlugin plugin)
                        {
                            if (Plugins.Any(x => x.Name == plugin.Name) == false)
                            {
                                throw new Exception("Plugin " + plugin.Name + " not registered");
                            }
                            else
                            {
                                Plugins.Remove(plugin);
                                APIRouter.RemovePlugin(plugin);
                            }
                        }

                        /// <summary>
                        /// Retrieves the registered plugins.
                        /// </summary>
                        /// <returns>List of registered plugins</returns>
                        public List<IPlugin> GetPluginList()
                        {
                            return Plugins.ToList();
                        }

                        /// <summary>
                        /// Logs the fact that a request has been made at a logging level determined by the HTTP method of the request.
                        /// </summary>
                        /// <param name="uri"></param>
                        /// <param name="httpMethod"></param>
                        private void LogCall(ResolvedRoute route, string uri, string httpMethod)
                        {
                            ProcessLogLevel messageLogLevel = ProcessLogLevel.Info;
                            if (route.Method.IsDefined(typeof(SuppressRequestResourceLoggingAttribute), true))
                            {
                                messageLogLevel = ProcessLogLevel.Debug;
                            }
                            var callMessage = string.Format("Received request: {0} {1}", httpMethod.ToUpper(), uri);
                            ProcessLogger.GetInstance().Log(messageLogLevel, callMessage);
                        }

                        /// <summary>
                        /// Logs the request body so long as the method furnishing the request has not been tagged with
                        /// the attribute that indicates request body logging should be disabled.
                        /// </summary>
                        /// <param name="route">Route that resolved the </param>
                        /// <param name="jsonBody"></param>
                        private void LogRequestBody(ResolvedRoute route, string jsonBody)
                        {
                            if (!route.Method.IsDefined(typeof(SuppressRequestBodyLoggingAttribute), true))
                            {
                                Log.Debug(string.Format("Request parameters: {0}", jsonBody));
                            }
                        }

                        /// <summary>
                        /// Tries to communicate with one of the registered API expecting a post method.
                        /// </summary>
                        /// <param name="uriTemplate">It is the targetted REST url</param>
                        /// <param name="jsonRequest">The request that'll be converted and given to the api as parameter</param>
                        /// <param name="serializer">The serializer that'll be used to unserialize the request</param>
                        /// <returns>The API response to the request</returns>
                        public IGenericResponse CallPostAPI(string uriTemplate, string method, string jsonRequest, IWCFSerializer serializer)
                        {
                            var resolvedRoute = APIRouter.GetRoute(uriTemplate, method);

                            LogCall(resolvedRoute, uriTemplate, method);
                            LogRequestBody(resolvedRoute, jsonRequest);

                            object request = DeserializeRequest(jsonRequest, resolvedRoute.Method.GetParameters().First().ParameterType, serializer);

                            object[] parameters = { request };

                            var queryParams = ResourceResolver.GetParamsForRequestMethod(resolvedRoute.Method, resolvedRoute.ResourceIds).ToArray();
                            parameters = parameters.Concat(queryParams).ToArray();

                            IPlugin p = (IPlugin)resolvedRoute.Plugin;

                            try
                            {
                                return (IGenericResponse)p.InstantiatePlugin(resolvedRoute.QueryParameters).Call(resolvedRoute.Method, parameters);
                            }
                            catch (Exception e)
                            {
                                if (e.InnerException != null)
                                    throw e.InnerException;
                                else
                                    throw;
                            }
                        }

                        /// <summary>
                        /// Handle all the different exceptions thrown while deserializing
                        /// </summary>
                        /// <param name="json"></param>
                        /// <param name="type"></param>
                        /// <param name="serializer"></param>
                        /// <returns></returns>
                        protected object DeserializeRequest(string json, Type type, IWCFSerializer serializer)
                        {
                            try
                            {
                                object request = serializer.Deserialize(json, type);
                                return request;
                            }
                            catch (Kge.Agent.Rest.Library.MissingMemberException e)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                    Helper.SetAppTypeInErrorMessage(  400,
                                    LanguageHelper.Resolve("CORE_ERROR_JSON_MISSING_MEMBER", e.MemberName, e.Path, e.Line, e.Position))),
                                    HttpStatusCode.BadRequest);
                            }
                            catch (ExtraMemberException e)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                    Helper.SetAppTypeInErrorMessage(  400,
                                    LanguageHelper.Resolve("CORE_ERROR_JSON_EXTRA_MEMBER", e.MemberName, e.Path, e.Line, e.Position))),
                                     HttpStatusCode.BadRequest);
                            }
                            catch (MemberTypeException e)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                    Helper.SetAppTypeInErrorMessage(  400,
                                    LanguageHelper.Resolve("CORE_ERROR_JSON_MEMBER_TYPE", e.MemberName, e.Path, e.Line, e.Position))),
                                    HttpStatusCode.BadRequest);
                            }
                            catch (JsonSyntaxException e)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                    Helper.SetAppTypeInErrorMessage(  400,
                                     LanguageHelper.Resolve("CORE_ERROR_JSON_SYNTAX", e.Line, e.Position))),
                                    HttpStatusCode.BadRequest);
                            }
                            catch
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                    Helper.SetAppTypeInErrorMessage(  400,
                                    LanguageHelper.Resolve("CORE_ERROR_JSON_UNREGISTERED"))),
                                    HttpStatusCode.BadRequest);
                            }
                        }

                        /// <summary>
                        /// Tries to communicate with one of the registered API expecting a method without request.
                        /// </summary>
                        /// <param name="uriTemplate">>It is the targetted REST url</param>
                        /// <returns>The API response to the request</returns>
                        public IGenericResponse CallGetAPI(string uriTemplate, string method)
                        {
                            var resolvedRoute = APIRouter.GetRoute(uriTemplate, method);

                            LogCall(resolvedRoute, uriTemplate, method);

                            object[] parameters = { };

                            var resourceParams = ResourceResolver.GetParamsForMethod(resolvedRoute.Method, resolvedRoute.ResourceIds).ToArray();
                            parameters = resourceParams.Concat(parameters).ToArray();

                            IPlugin p = (IPlugin)resolvedRoute.Plugin;

                            try
                            {
                                return (IGenericResponse)p.InstantiatePlugin(resolvedRoute.QueryParameters).Call(resolvedRoute.Method, parameters);
                            }
                            catch (Exception e)
                            {
                                if (e.InnerException != null)
                                    
                                    throw e.InnerException;
                                else
                                    throw;
                            }
                        }
                    }
                }
            }
        }
    }
}