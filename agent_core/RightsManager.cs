using Kge.Agent.Lang;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
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
                public class RightsManager
                {
                    protected IWebAuthentication WebAuth { get; set; }

                    public RightsManager(IAPIRouter apiRouter, IWebAuthentication webAuth)
                    {
                        WebAuth = webAuth;

                        SubscribeRouter(apiRouter);
                    }

                    /// <summary>
                    /// Listen to the API router to check if the use of a route is allowed by the auth token.
                    /// </summary>
                    /// <param name="apiRouter"></param>
                    protected void SubscribeRouter(IAPIRouter apiRouter)
                    {
                        apiRouter.OnRouteRetrieved += new RouteHandler(delegate(IAPIRouter o, IPlugin p, MethodInfo m)
                            {
                                PluginAPI pApi = p.PluginAPIs.FirstOrDefault(x => x.MethodInfo.Name == m.Name);
                                IsUserAllowedToUse(pApi);
                            });
                    }

                    /// <summary>
                    /// Verify if the current user has the right to execute that plugin api
                    /// </summary>
                    /// <param name="pApi"></param>
                    public void IsUserAllowedToUse(PluginAPI pApi)
                    {
                        if (pApi != null && pApi.Restriction == RestrictionLevel.Admin)
                        {
                            VerifyUserToken();
                        }
                    }

                    /// <summary>
                    /// Verify if the user respect a certain RestrictionLevel
                    /// </summary>
                    /// <param name="level"></param>
                    public void HasUserAccess(RestrictionLevel level)
                    {
                        if (level == RestrictionLevel.Admin)
                        {
                            VerifyUserToken();
                        }
                    }

                    private void VerifyUserToken()
                    {
                        try
                        {
                            WebAuth.ValidateToken(WebAuth.GetToken());
                        }
                        catch (AuthenticationException ex)
                        {
                            throw new WebResponseException(new HttpsErrorResponse(System.Net.HttpStatusCode.Unauthorized,
                                                            Helper.SetAppTypeInErrorMessage(  ex.errorCode,
                                                            LanguageHelper.Resolve("CORE_ERROR_X_AUTH_TOKEN"))),
                                                            System.Net.HttpStatusCode.Unauthorized);
                        }
                        catch (SystemAuthenticationException ex)
                        {
                            throw new WebResponseException(new HttpsErrorResponse(System.Net.HttpStatusCode.InternalServerError,
                                Helper.SetAppTypeInErrorMessage(  ex.errorCode, ex.Message)),
                                System.Net.HttpStatusCode.InternalServerError);
                        }
                    }
                }
            }
        }
    }
}