using Kge.Agent.Library.SystemWrapper.DllImport;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using Kge.Agent.Rest.Library.Plugin.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kge.Agent.Library;
using Kge.Agent.Lang;
using Microsoft.AspNetCore.Mvc;
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
                    public abstract class APluginImplementation : ICapabilityProvider, IPluginDetailsProvider, IReportFilesProvider 
                    {
                        private static ConcurrentDictionary<Guid, TaskContext> RunningTasks = new ConcurrentDictionary<Guid, TaskContext>();
                        protected ConcurrentDictionary<Guid, TaskContext> _runningTasks { get { return RunningTasks; } }

                        protected APluginImplementation(QueryParametersContainer queryParameters)
                        {
                            QueryParameters = queryParameters;
                            if (QueryParameters == null)
                                QueryParameters = new QueryParametersContainer();
                        }

                        protected QueryParametersContainer QueryParameters { get; private set; }

                        /// <summary>
                        /// Returns all the APIs contained in the plugin implementation
                        /// </summary>
                        /// <returns></returns>
                        public virtual List<PluginAPI> GetPluginMethodsMetaData()
                        {
                            var pluginAPIs = new List<PluginAPI>();

                            List<MethodInfo> apiMethods = this.GetType().
                                GetMethods().
                                Where(x => x.GetCustomAttributes<RouteDescription>(false).Count() > 0).
                                ToList();

                            foreach (MethodInfo apiMethod in apiMethods)
                            {
                                List<RouteDescription> apis = apiMethod.GetCustomAttributes<RouteDescription>().ToList();

                                foreach (var api in apis)
                                {
                                    var pluginApi = new PluginAPI(apiMethod, api.Uri, api.HttpMethod, api.Restriction, api.Asynchronous);
                                    pluginAPIs.Add(pluginApi);
                                }
                            }

                            return pluginAPIs;
                        }

                        /// <summary>
                        /// Decides if we need to synchronously or asynchronously call the plugin api
                        /// </summary>
                        /// <param name="target"></param>
                        /// <param name="parameters"></param>
                        /// <returns></returns>
                        public IGenericResponse Call(MethodInfo target, params object[] parameters)
                        {
                            if (target.GetCustomAttribute<RouteDescription>() != null && target.GetCustomAttribute<RouteDescription>().Asynchronous)
                            {
                                return AsynchronousCall(target, parameters);
                            }
                            else
                            {
                                return SynchronousCall(target, parameters);
                            }
                        }

                        /// <summary>
                        /// Creates a TaskContext which will be executed by another thread to execute the plugin api, stores it, and returns the 202 response
                        /// with a Json TaskResponse object which contains the uri to this newly created specific task
                        /// </summary>
                        /// <param name="target"></param>
                        /// <param name="parameters"></param>
                        /// <returns></returns>
                        private IGenericResponse AsynchronousCall(MethodInfo target, params object[] parameters)
                        {
                            RouteDescription api = target.GetCustomAttribute<RouteDescription>();

                            Guid taskGuid = Guid.NewGuid();
                            string uri = WebTask.GenerateTaskUri(taskGuid.ToString());
                            var taskResp = new TaskResponse()
                            {
                                task = new DataContracts.TaskUriDetails()
                                {
                                    taskUri = uri,
                                    isFinished = false,
                                    TaskCreationTimeDetails = DateTime.Now,
                                },
                            };

                            try
                            {
                                OperationContextHelper.RetrieveContext().OutgoingMessageResponse.StatusCode = (int) System.Net.HttpStatusCode.Accepted;
                            }
                            catch { }

                            var newTask = new TaskContext(this, taskGuid, taskResp, api.Restriction);
                            RunningTasks.GetOrAdd(taskGuid, newTask);

                            newTask.Start(target, parameters);

                            // Created a new Response because of 'resp' being changed before it is returned by the main thread
                            return new TaskResponse()
                            {
                                task = new DataContracts.TaskUriDetails()
                                {
                                    taskUri = uri,
                                    isFinished = false,
                                    TaskCreationTimeDetails = taskResp.task.TaskCreationTimeDetails,
                                }
                            };
                        }

                        /// <summary>
                        /// Simply call the plugin api
                        /// </summary>
                        /// <param name="target"></param>
                        /// <param name="parameters"></param>
                        /// <returns></returns>
                        private IGenericResponse SynchronousCall(MethodInfo target, params object[] parameters)
                        {
                            // TODO Removed impersonation
                           
                                try
                                {
                                    return target.Invoke(this, parameters) as IGenericResponse;
                                }
                                catch (Exception e)
                                {
                                    Log.Error("Received exception from Plugin's API: " + e.ExtractFullTrace());
                                    throw;
                                }
                            
                        }

                        public bool Exposed
                        {
                            get
                            {
                                var implementorType = this.GetType();
                                var ExposedAttributes = implementorType.GetCustomAttributes<ExposedPluginAttribute>(true);
                                return ExposedAttributes.ToList().Count > 0;
                            }
                        }

                        /// <summary>
                        /// Get plugin's details
                        /// </summary>
                        /// <returns></returns>
                        public virtual PluginDetails GetPluginDetails()
                        {
                            var implementorType = this.GetType();
                            var implementorAssemblyName = implementorType.Assembly.GetName();

                            return new PluginDetails()
                            {
                                name = implementorAssemblyName.Name,
                                version = implementorAssemblyName.Version.ToString(),
                                capabilities = this.GetCapabilities()
                            };
                        }

                        /// <summary>
                        /// Get plugin capabilities
                        /// </summary>
                        /// <returns></returns>
                        public virtual Capability[] GetCapabilities()
                        {
                            return new Capability[0];
                        }

                        /// <summary>
                        /// Get report files for file tickets purposes
                        /// </summary>
                        /// <returns></returns>
                        public virtual Task<ReportFiles> ProvideReportFiles()
                        {
                            return Task.FromResult<ReportFiles>(null);
                        }
                    }

                    [AttributeUsage(AttributeTargets.Method)]
                    public sealed class RouteDescription : Attribute
                    {
                        public string Uri { get; private set; }
                        public string HttpMethod { get; private set; }
                        public RestrictionLevel Restriction { get; private set; }
                        public bool Asynchronous { get; private set; }

                        public RouteDescription(string uri, string httpMethod, RestrictionLevel restriction, bool asynchronous)
                        {
                            Uri = uri;
                            HttpMethod = httpMethod;
                            Restriction = restriction;
                            Asynchronous = asynchronous;
                        }

                        public RouteDescription(string uri, string httpMethod, RestrictionLevel restriction)
                        {
                            Uri = uri;
                            HttpMethod = httpMethod;
                            Restriction = restriction;
                            Asynchronous = false;
                        }

                        public RouteDescription(string uri, string httpMethod)
                        {
                            Uri = uri;
                            HttpMethod = httpMethod;
                            Restriction = RestrictionLevel.Admin;
                            Asynchronous = false;
                        }
                    }

                    /// <summary>
                    /// Attribute used to indicate that a plugin should be exposed
                    /// through the plugins REST resource.
                    /// </summary>
                    [AttributeUsage(AttributeTargets.Class)]
                    public sealed class ExposedPluginAttribute : Attribute
                    {
                    }
                }
            }
        }
    }
}