using Kge.Agent.Rest.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
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
                    public class ResourceIdResolver
                    {
                        /// <summary>
                        /// Parameters resolution/matching between a resource id dictionnary and the specified method.
                        /// </summary>
                        /// <param name="methodInfo"></param>
                        /// <param name="resourceIdsDic"></param>
                        /// <returns></returns>
                        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
                        public List<object> GetParamsForMethod(MethodInfo methodInfo, Dictionary<string, string> resourceIdsDic)
                        {
                            List<object> retList = new List<object>();

                            // Ignoring cases
                            resourceIdsDic = new Dictionary<string, string>(resourceIdsDic, StringComparer.InvariantCultureIgnoreCase);

                            foreach (var param in methodInfo.GetParameters())
                            {
                                if (resourceIdsDic.ContainsKey(param.Name) == false)
                                    ThrowExpectedResourceIdMissing(param.Name);
                                try
                                {
                                    object obj = Convert.ChangeType(resourceIdsDic[param.Name], param.ParameterType);
                                    if (obj == null)
                                        ThrowExpectedResourceIdMissing(param.Name);
                                    retList.Add(obj);
                                }
                                catch
                                {
                                    ThrowExpectedResourceIdMissing(param.Name);
                                }
                            }

                            if (resourceIdsDic.Count > methodInfo.GetParameters().Length)
                            {
                                string unexpectedResourceId = resourceIdsDic.First(x => retList.Contains(x.Key) == false).Key;
                                ThrowUnexpectedResourceId(unexpectedResourceId);
                            }

                            return retList;
                        }

                        /// <summary>
                        /// Parameters resolution/matching between a resource id dictionnary and the specified method,
                        /// except that here we expect an additionnal request object as first parameter.
                        /// </summary>
                        /// <param name="methodInfo"></param>
                        /// <param name="resourceIdsDic"></param>
                        /// <returns></returns>
                        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
                        public List<object> GetParamsForRequestMethod(MethodInfo methodInfo, Dictionary<string, string> resourceIdsDic)
                        {
                            if (resourceIdsDic == null)
                                return new List<object>();

                            List<object> retList = new List<object>();

                            // Ignoring cases
                            resourceIdsDic = new Dictionary<string, string>(resourceIdsDic, StringComparer.InvariantCultureIgnoreCase);

                            // Skip the first element because it is the request parameter object
                            foreach (var param in methodInfo.GetParameters().Skip(1))
                            {
                                if (resourceIdsDic.ContainsKey(param.Name) == false)
                                    ThrowExpectedResourceIdMissing(param.Name);
                                try
                                {
                                    object obj = Convert.ChangeType(resourceIdsDic[param.Name], param.ParameterType);
                                    if (obj == null)
                                        ThrowResourceIdConversionFailed(param.Name, resourceIdsDic[param.Name], param.ParameterType);
                                    retList.Add(obj);
                                }
                                catch
                                {
                                    ThrowResourceIdConversionFailed(param.Name, resourceIdsDic[param.Name], param.ParameterType);
                                }
                            }

                            if (resourceIdsDic.Count > methodInfo.GetParameters().Length - 1)
                            {
                                string unexpectedResourceId = resourceIdsDic.First(x => retList.Contains(x.Key) == false).Key;
                                ThrowUnexpectedResourceId(unexpectedResourceId);
                            }

                            return retList;
                        }

                        protected void ThrowExpectedResourceIdMissing(string resourceIdName)
                        {
                            throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                Helper.SetAppTypeInErrorMessage(  400,
                                "Couldn't find the resource id:" + resourceIdName + ", unexpected error.")),
                                HttpStatusCode.BadRequest);
                        }

                        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
                        protected void ThrowResourceIdConversionFailed(string resourceIdName, string resourceIdValue, Type expectedType)
                        {
                            throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                Helper.SetAppTypeInErrorMessage(  400,
                                "Couldn't convert resource id \'" + resourceIdName + "\' with value \'" + resourceIdValue + "\' to expected type \'" + expectedType.Name + "\'.")),
                                HttpStatusCode.BadRequest);
                        }

                        protected void ThrowUnexpectedResourceId(string resourceIdName)
                        {
                            throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                Helper.SetAppTypeInErrorMessage(  400,
                                "Unexpected resource id:" + resourceIdName + ", unexpected error.")),
                                HttpStatusCode.BadRequest);
                        }
                    }
                }
            }
        }
    }
}
