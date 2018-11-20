using Kge.Agent.Lang;
using Kge.Agent.Library;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                namespace NativePlugin
                {
                    public class LoginPlugin : APluginImplementation
                    {
                        public LoginPlugin()
                            : base(null) { }

                        public LoginPlugin(QueryParametersContainer queryParameters)
                            : base(queryParameters) { }

                        [SuppressSensitiveRequestBodyLogging]
                        [RouteDescription(@"/login", "POST", RestrictionLevel.None)]
                        public LoginResponse Login(LoginRequest request)
                        {
                            string urlAbsPath = OperationContextHelper.RetrieveContext().IncomingMessageRequest.Path;
                            Log.Debug("Requested URI:" + urlAbsPath);
                            LoginResponse response = new LoginResponse();
                            CryptographyWrapper registrationObj = new CryptographyWrapper();

                            Log.Debug("REST application request type RMSQL");
                            try
                            {
                                if (string.IsNullOrEmpty(request.loginDetails.user) || string.IsNullOrEmpty(request.loginDetails.password))
                                {
                                    Log.Error("Invalid request parameters found in request body");
                                    throw new RequestValidationException(CommonErrors.ERROR_PARAMS_VALIDATION,
                                                                            LanguageHelper.Resolve("CORE_ERROR_PARAMS_VALIDATION"));
                                }
                                IWebAuthentication authentication = new WebAuthentication();
                                string token = string.Empty;  // initializing token to empty string which will be set if successfully created in tokenAuth module
                                authentication.CreateToken(request.loginDetails.user, request.loginDetails.password, ref token);
                                Log.Debug("encoded token:" + token);
                                StringBuilder encryptedToken = new StringBuilder();
                                Log.Debug("encrypted token: " + token);
                                registrationObj.GenerateEncryptedString(true, token, encryptedToken);
                                response.accessToken = new LoginTokenDetails();
                                response.accessToken.id = encryptedToken.ToString();
                                Log.Debug("response token id: " + response.accessToken.id);
                                response.accessToken.issuedTime = authentication.GetTokenIssueTimeInUTC(token).ToLocalTime().ToString();
                                Log.Debug("response token issue time: " + response.accessToken.issuedTime);
                                response.accessToken.expiredTime = authentication.GetTokenExpireTimeInUTC(token).ToLocalTime().ToString();
                                Log.Debug("response token expire time: " + response.accessToken.expiredTime);
                            }
                            catch (RequestValidationException ex)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.BadRequest,
                                                                Helper.SetAppTypeInErrorMessage(  ex.errorCode, ex.Message)),
                                                                HttpStatusCode.BadRequest);
                            }
                            catch (AuthenticationException ex)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(System.Net.HttpStatusCode.Unauthorized,
                                                                Helper.SetAppTypeInErrorMessage(  ex.errorCode, ex.Message)),
                                                                System.Net.HttpStatusCode.Unauthorized);
                            }
                            catch (SystemAuthenticationException ex)
                            {
                                throw new WebResponseException(new HttpsErrorResponse(System.Net.HttpStatusCode.InternalServerError,
                                                                Helper.SetAppTypeInErrorMessage(  ex.errorCode, ex.Message)),
                                                                System.Net.HttpStatusCode.InternalServerError);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("System Exception: " + ex.Message);
                                throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.InternalServerError,
                                                                Helper.SetAppTypeInErrorMessage(  CommonErrors.ERROR_UNDEFINED,
                                                                LanguageHelper.Resolve("CORE_ERROR_UNDEFINED"))),
                                                                HttpStatusCode.InternalServerError);
                            }
                           
                            return (response);
                        }
                    }
                }
            }
        }
    }
}
