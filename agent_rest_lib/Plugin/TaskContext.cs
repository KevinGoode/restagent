using Kge.Agent.Lang;
using Kge.Agent.Library.SystemWrapper.DllImport;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library;


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
                    public sealed class TaskContext : IDisposable
                    {
                        public DateTime TaskCreationTime { get; private set; }
                        public DateTime TaskCompletionTime { get; private set; }

                        public APluginImplementation PluginInstance { get; private set; }
                        public Guid TaskGuid { get; private set; }
                        public TaskResponse TaskStatusResponse { get; private set; }
                        public RestrictionLevel TaskRestriction { get; private set; }

                        private IGenericResponse _taskResult;
                        public IGenericResponse TaskResult
                        {
                            get
                            {
                                if (_taskResult != null)
                                {
                                    lock (_taskResult)
                                    {
                                        return _taskResult;
                                    }
                                }
                                return _taskResult;
                            }

                            set
                            {
                                if (_taskResult != null)
                                {
                                    lock (_taskResult)
                                    {
                                        _taskResult = value;
                                    }
                                }
                                _taskResult = value;
                            }
                        }

                        private Task Task { get; set; }

                        public TaskContext(APluginImplementation pluginInstance, Guid taskGuid, TaskResponse taskStatusResponse, RestrictionLevel taskRestriction)
                        {
                            PluginInstance = pluginInstance;
                            TaskGuid = taskGuid;
                            TaskStatusResponse = taskStatusResponse;
                            TaskRestriction = taskRestriction;
                            TaskCreationTime = DateTime.Now;
                        }

                        /// <summary>
                        /// Create a Task object to execute asynhronously the task, the Task is stored in the TaskContext so it doesn't get Disposed
                        /// </summary>
                        /// <param name="target"></param>
                        /// <param name="parameters"></param>
                        public void Start(MethodInfo target, params object[] parameters)
                        {
                            // Passing the web operation context to the new thread.
                            // Setting the variable as a local one, so the thread can access it.
                            var context = OperationContextHelper.RetrieveContext();
                            var webContext = OperationContextHelper.RetrieveContext();

                            Task = new Task(
                                delegate
                                {
                                    // Passing the web operation context to the new thread.
                                    OperationContextHelper.SaveContext(context);
                                    OperationContextHelper.SaveContext(webContext);

                                    RunTask(target, parameters);
                                });
                            Task.Start();
                        }

                        /// <summary>
                        /// Runs the actual plugin api code, and fill accordingly the TaskResult and TaskStatusResponse
                        /// </summary>
                        /// <param name="target"></param>
                        /// <param name="parameters"></param>
                        private void RunTask(MethodInfo target, params object[] parameters)
                        {
                            try
                            {
                                    //TODO Removed impersonation
                                    TaskResult = target.Invoke(PluginInstance, parameters) as IGenericResponse;
                                
                            }
                            catch (Exception e)
                            {
                                Log.Error("Received exception from Plugin's Async API: " + e.ExtractFullTrace());
                                if (e.InnerException != null && e.InnerException is WebResponseException)
                                {
                                    var webException = (WebResponseException)e.InnerException;
                                    TaskResult = webException.ErrorMessage as IGenericResponse;
                                }
                                else // Unexpected exception
                                {
                                    var webException = new WebResponseException(new HttpsErrorResponse(HttpStatusCode.NotFound,
                                        Helper.SetAppTypeInErrorMessage(500,
                                        LanguageHelper.Resolve("CORE_ERROR_TASK_UNEXPECTED_ERROR", TaskGuid))),
                                        HttpStatusCode.InternalServerError);

                                    TaskResult = webException.ErrorMessage as IGenericResponse;
                                }
                            }

                            lock (TaskStatusResponse)
                            {
                                TaskStatusResponse.task.TaskCompletionTime = DateTime.Now;
                                TaskStatusResponse.task.taskResultUri = WebTask.GenerateTaskResultUri(TaskGuid.ToString());
                                TaskStatusResponse.task.isFinished = true;
                            }
                        }

                        public void Dispose()
                        {
                            Task.Dispose();
                        }
                    }
                }
            }
        }
    }
}
