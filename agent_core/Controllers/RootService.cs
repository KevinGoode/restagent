using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using file=System.IO.File;
using System.Net;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.Reflection;
using Kge.Agent.Library;
using Kge.Agent.Rest.Server.API;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using Kge.Agent.Lang;
using Kge.Agent.Rest.Server.NativePlugin;
using System.Net.Http;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Appliance;
using System.Security.Cryptography.X509Certificates;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                     
                    [Route("{*url}")]
                    [ApiController]
                    public class RootService : ControllerBase
                    {
                       public static IAPIManager APIManager = null;
                       public static RightsManager RightsManager = null;

                        [HttpGet, HttpDelete] 
                        public Stream NoParamsRequest()
                        {
                            var jsonSerializer = new JsonWCFSerializer();
                            try
                            {
                                // Has to be done before everything.
                                SetThreadWebOperationContext();

                                OperationContextFacsimile context =OperationContextHelper.RetrieveContext();
                                IGenericResponse res = APIManager.CallGetAPI(new WCFUriTemplateResolver().ResolveUriTemplate(),
                                    context.IncomingMessageRequest.Method);

                                if (res is ReportResponse) // Duct tape, should be changed later
                                {
                                    try
                                    {
                                        ReportResponse concreteResp = res as ReportResponse;

                                        string headerInfo = "attachment; filename=" + Path.GetFileName(concreteResp.FilePath);
                                        HttpContext.Response.Headers["Content-Disposition"] = headerInfo;
                                        HttpContext.Response.Headers.Add("Content-Type", "application/zip");
                                        return file.OpenRead(concreteResp.FilePath);
                                    }
                                    catch
                                    {
                                        throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.InternalServerError,
                                            Helper.SetAppTypeInErrorMessage(  500,
                                            LanguageHelper.Resolve("CORE_ERROR_FAILED_TO_RETURN_FILE"))),
                                            HttpStatusCode.InternalServerError);
                                    }
                                }
                                else
                                {
                                    byte[] resultBytes = Encoding.UTF8.GetBytes(jsonSerializer.Serialize(res));

                                    HttpContext.Response.Headers.Add("Content-Type", "application/json");

                                    return new MemoryStream(resultBytes);
                                }
                            }
                            catch (WebResponseException excep)
                            {
                                HttpContext.Response.Headers.Add("Content-Type", "application/json");
                                HttpContext.Response.StatusCode = (int)excep.responseCode;
                                return new MemoryStream(Encoding.UTF8.GetBytes(jsonSerializer.Serialize(excep.ErrorMessage)));
                            }
                        }

                        [HttpPost, HttpPut]
                        public Stream OneParamRequest()
                        {
                            var jsonSerializer = new JsonWCFSerializer();
                            try
                            {
                                StreamReader reader = new StreamReader(HttpContext.Request.Body);
                                string request = reader.ReadToEnd();
                                // Has to be done before everything.
                                SetThreadWebOperationContext();

                                VerifyContentType();

                                

                                IGenericResponse res = APIManager.CallPostAPI(new WCFUriTemplateResolver().ResolveUriTemplate(),
                                    OperationContextHelper.RetrieveContext().IncomingMessageRequest.Method.ToString(),
                                    request.ToString(),
                                    jsonSerializer);

                                byte[] resultBytes = Encoding.UTF8.GetBytes(jsonSerializer.Serialize(res));

                                HttpContext.Response.Headers.Add("Content-Type", "application/json");

                                return new MemoryStream(resultBytes);
                            }
                            catch (WebResponseException excep)
                            {
                                HttpContext.Response.Headers.Add("Content-Type", "application/json");
                                HttpContext.Response.StatusCode = (int)excep.responseCode;
                                return new MemoryStream(Encoding.UTF8.GetBytes(jsonSerializer.Serialize(excep.ErrorMessage)));
                            }
                        }

                        
                    
                        public void SetThreadWebOperationContext()
                        {
                            // WebOperationContext setting on thread
                            OperationContextHelper.SaveContext(this.HttpContext);
                        }
                    protected void VerifyContentType()
                    {
                        string json = "application/json";
                        var contentType = OperationContextHelper.RetrieveContext().IncomingMessageRequest.ContentType;

                        if (contentType == null ||
                            !contentType.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(t => t.Equals(json, StringComparison.OrdinalIgnoreCase)))
                        {
                            throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.UnsupportedMediaType,
                                Helper.SetAppTypeInErrorMessage(  415,
                                LanguageHelper.Resolve("CORE_ERROR_WRONG_CONTENT_TYPE"))),
                                HttpStatusCode.UnsupportedMediaType);
                        }
                    }
                        public static void Initialise()
                        {
                            //TODO Get process logger config
                            ProcessLogger.CreateInstance("","",0,0);
                            APIManager = new APIManager(new APIRouter(), new PluginLoader());

                            // Adding manually native plugins
                            APIManager.AddPlugin(new Plugin(typeof(LoginPlugin), new LoginPlugin().GetPluginMethodsMetaData()));
                            APIManager.AddPlugin(new Plugin(typeof(AppliancePlugin), new AppliancePlugin().GetPluginMethodsMetaData()));
                            APIManager.AddPlugin(new Plugin(typeof(TaskPlugin), new TaskPlugin().GetPluginMethodsMetaData()));
                            APIManager.AddPlugin(new Plugin(typeof(ReportPlugin), new ReportPlugin().GetPluginMethodsMetaData()));
                            

                            
                            string uriPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            string localPath = new Uri(uriPath).LocalPath;
                            APIManager.DiscoverAPIDirectory(localPath);
                            

                            RightsManager = new RightsManager(APIManager.APIRouter, new WebAuthentication());
                        }
                    
                }
            }
        }
    }
}
