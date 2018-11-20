using Kge.Agent.Lang;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                namespace NativePlugin
                {
                    public class TaskPlugin : APluginImplementation
                    {
                        public TaskPlugin()
                            : base(null) { }

                        public TaskPlugin(QueryParametersContainer queryParameters)
                            : base(queryParameters) { }

                        /// <summary>
                        /// Returns the task status, if the asked task doesn't exist, returns a 404 response
                        /// </summary>
                        /// <param name="task_guid"></param>
                        /// <returns></returns>
                        [SuppressRequestResourceLogging]
                        [RouteDescription(@"/task/{task_guid}", "GET", RestrictionLevel.None, false)]
                        public IGenericResponse GetTask(string task_guid)
                        {
                            var guid = Guid.Parse(task_guid);

                            if (this._runningTasks.ContainsKey(guid))
                            {
                                TaskContext taskContext = this._runningTasks[guid];

                                RootService.RightsManager.HasUserAccess(taskContext.TaskRestriction);

                                if (taskContext.TaskStatusResponse.task.isFinished)
                                {
                                    try
                                    {
                                        OperationContextHelper.RetrieveContext().OutgoingMessageResponse.StatusCode = (int) System.Net.HttpStatusCode.Accepted;
                                    }
                                    catch { }
                                }

                                return taskContext.TaskStatusResponse;
                            }
                            else
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.NotFound,
                                   Helper.SetAppTypeInErrorMessage(  404,
                                   LanguageHelper.Resolve("CORE_ERROR_TASK_NOT_FOUND", task_guid))),
                                   HttpStatusCode.NotFound);
                            }
                        }

                        /// <summary>
                        /// Delete the Task and its result, 404 response if the task doesn't exist
                        /// </summary>
                        /// <param name="task_guid"></param>
                        /// <returns></returns>
                        [SuppressRequestResourceLogging]
                        [RouteDescription(@"/task/{task_guid}", "DELETE", RestrictionLevel.None, false)]
                        public IGenericResponse DeleteTask(string task_guid)
                        {
                            var guid = Guid.Parse(task_guid);

                            if (this._runningTasks.ContainsKey(guid))
                            {
                                TaskContext taskContext = this._runningTasks[guid];

                                RootService.RightsManager.HasUserAccess(taskContext.TaskRestriction);

                                if (taskContext.TaskResult != null)
                                {
                                    TaskContext dummy;
                                    this._runningTasks.TryRemove(guid, out dummy);
                                    return null;
                                }
                                else
                                {
                                    throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                        Helper.SetAppTypeInErrorMessage(  400,
                                        LanguageHelper.Resolve("CORE_ERROR_DELETE_RUNNING_TASK", task_guid))),
                                        HttpStatusCode.BadRequest);
                                }
                            }
                            else
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.NotFound,
                                   Helper.SetAppTypeInErrorMessage(  404,
                                   LanguageHelper.Resolve("CORE_ERROR_TASK_RESULT_NOT_FOUND", task_guid))),
                                   HttpStatusCode.NotFound);
                            }
                        }

                        /// <summary>
                        /// Gets the task result, if the task or the result doesn't exist, returns a 404 response
                        /// </summary>
                        /// <param name="task_guid"></param>
                        /// <returns></returns>
                        [SuppressRequestResourceLogging]
                        [RouteDescription(@"/task/{task_guid}/result", "GET", RestrictionLevel.None, false)]
                        public IGenericResponse GetTaskResult(string task_guid)
                        {
                            var guid = Guid.Parse(task_guid);

                            if (this._runningTasks.ContainsKey(guid) && this._runningTasks[guid].TaskResult != null)
                            {
                                TaskContext taskContext = this._runningTasks[guid];

                                RootService.RightsManager.HasUserAccess(taskContext.TaskRestriction);

                                return taskContext.TaskResult;
                            }
                            else
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.NotFound,
                                   Helper.SetAppTypeInErrorMessage(  404,
                                   LanguageHelper.Resolve("CORE_ERROR_TASK_RESULT_NOT_FOUND", task_guid))),
                                   HttpStatusCode.NotFound);
                            }
                        }
                    }
                }
            }
        }
    }
}
