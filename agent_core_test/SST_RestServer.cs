using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Kge.Agent.Rest.Server;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Login;
using Newtonsoft.Json.Linq;
using Kge.Agent.Library;
using Kge.Agent.Rest.Test.TestPlugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Appliance;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                public enum HttpVerb
                {
                    GET = 0, POST = 1, PUT = 2, DELETE = 3
                }

                [TestClass]
                public class SST_RestServer
                {
                    private static WebApplicationFactory<Startup> factory;
                    private static Program myServ = null;
                    private static HttpClient httpClient = null;
                    private HttpResponseMessage response = null;
                    private string responseContents = null;
                    private string token = "";
                    private static HttpClientHandler myhandler = null;
                    [TestInitialize]
                    public void setup()
                    {
                       
                    }

                    [TestCleanup]
                    public void teardown()
                    {
                       
                    }

                    [ClassInitialize]
                    public static void suite_setup(TestContext context)
                    {
                        //Start rest server before all tests are run
                        string[] args = { };
                        //Program.Main(args);
                        //Thread.Sleep(5000); //Wait for web server to start
                        factory = new WebApplicationFactory<Startup>();
                        httpClient =  factory.CreateClient();
                        myhandler = new HttpClientHandler();
                        myhandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                        myhandler.ServerCertificateCustomValidationCallback = 
                        (httpRequestMessage, cert, cetChain, policyErrors) =>{return true;};
                    }

                    [ClassCleanup]
                    public static void suite_teardown()
                    {
                        //Stop rest server after all tests have finished
                     

                    }


                    [TestMethod]
                    public void TestLoginSuccess()
                    {
                        Login();
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestLoginFailure()
                    {
                        Login("somuser", "");
                        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestGetPing()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "ping");
                        //Check response
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestGetPingUnauthorized()
                    {
                        token = "";
                        SendHttpRequest(HttpVerb.GET, null, "ping");
                        //Check response
                        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
                    }

                   
                

                    [TestMethod]
                    public void TestUnknownRequest()
                    {
                        Login();

                        SendHttpRequest(HttpVerb.GET, null, "testUnknownPath");
                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestExceptionUnsupportedMediaType()
                    {
                        Send(HttpVerb.POST, null, @"ping", "application/notJson");

                        Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);

                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        JObject obj = JObject.Parse(jsonContent);
                        var error = (JObject)obj.GetValue("Error");

                        Assert.AreEqual("415", error.GetValue("code").ToString());
                        Assert.IsTrue(error.GetValue("message").ToString()
                            .Contains(@"Unsupported media type. Content-Type request header must be set to application/json."));
                    }
                    
                    [TestMethod]
                    public void TestExceptionURINotFound()
                    {
                        Login();

                        Send(HttpVerb.POST, null, @"doesntExist");

                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        JObject obj = JObject.Parse(jsonContent);
                        var error = (JObject)obj.GetValue("Error");
                        string message = error.GetValue("message").ToString();
                        string errorCode = error.GetValue("code").ToString();
                        Assert.AreEqual("404", errorCode);
                        Assert.IsTrue(message.Contains(@"URI /doesntExist not found."));
                    }

                    [TestMethod]
                    public void TestExceptionMissingParameter()
                    {
                        var req = new FakeRequest() { testData = "testTestTEST" };

                        Send(HttpVerb.POST, req, @"login");

                        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        JObject obj = JObject.Parse(jsonContent);
                        var error = (JObject)obj.GetValue("Error");

                        Assert.AreEqual("400", error.GetValue("code").ToString());
                        Assert.IsTrue(error.GetValue("message").ToString()
                            .Contains(@"Invalid request packet. Missing expected parameter : 'loginDetails' from object 'LoginRequest'"));
                    }

                    [TestMethod]
                    public void TestExceptionUnexpectedParameter()
                    {
                        var req = new FakeRequestLogin()
                        {
                            loginDetails = new FakeRequestLoginDetails()
                            {
                                user = "dev-user",
                                password = "somepass",
                                weather = "IT RAINS AND RAINS AGAIN"
                            }
                        };

                        Send(HttpVerb.POST, req, @"login");

                        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        JObject obj = JObject.Parse(jsonContent);
                        var error = (JObject)obj.GetValue("Error");

                        Assert.AreEqual("400", error.GetValue("code").ToString());
                        Assert.IsTrue(error.GetValue("message").ToString()
                            .Contains(@"Invalid request packet. Found unexpected parameter : 'weather' in object 'loginDetails'"));
                    }

                    

                   

                    [TestMethod]
                    public void TestExceptionGetResourceNotFound()
                    {
                        Login();
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        SendHttpRequest(HttpVerb.GET, null, "appliance/2");
                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        JObject obj = JObject.Parse(jsonContent);
                        var error = (JObject)obj.GetValue("Error");

                        Assert.AreEqual("404", error.GetValue("code").ToString());
                    }

                   
                    [TestMethod]
                    public void TestPluginCreated()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/alive");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        var resp = GetResponse<TestResponsePing>();

                        Assert.AreEqual(true, resp.Alive);
                    }

                    [TestMethod]
                    public void TestPluginQueryParameters()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/queryParameters?test1=test1Result&test2=test2Result");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                        var resp = GetResponse<TestResponseQueryParameters>();

                        Assert.AreEqual(2, resp.QueryParameters.Count);

                        Assert.IsTrue(resp.QueryParameters.ContainsKey("test1"));
                        Assert.IsTrue(resp.QueryParameters.ContainsKey("test2"));

                        Assert.AreEqual("test1Result", resp.QueryParameters["test1"]);
                        Assert.AreEqual("test2Result", resp.QueryParameters["test2"]);
                    }

                    [TestMethod]
                    public void TestPluginEmptyQueryParameters()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/queryParameters");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var resp = GetResponse<TestResponseQueryParameters>();

                        Assert.AreEqual(0, resp.QueryParameters.Count);
                    }

                    [TestMethod]
                    public void TestPluginResourceId()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/getId/5");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var resp = GetResponse<TestResponseGetId>();

                        Assert.AreEqual(5, resp.Id);
                    }

                    [TestMethod]
                    public void TestPluginSecurity()
                    {
                        // Should be rejected
                        token = "";
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/securityCheck");
                        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

                        // Should be accepted
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/securityCheck");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var resp = GetResponse<TestResponsePing>();

                        Assert.AreEqual(true, resp.Alive);
                    }

                    [TestMethod]
                    public void TestPluginSecurityAsync()
                    {
                        // Should be rejected
                        token = "";
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/securityCheckAsync");
                        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

                        // Should be accepted
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/securityCheckAsync");
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestPluginSecurityAsyncException()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "testPlugin/securityCheckAsyncException");
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        var resp = GetResponse<TaskResponse>();

                        string taskUri = resp.task.taskUri;
                        taskUri = taskUri.Substring(taskUri.IndexOf("task/"));
                        while (resp.task.isFinished == false)
                        {
                            SendHttpRequest(HttpVerb.GET, null, taskUri);
                            resp = GetResponse<TaskResponse>();

                            Thread.Sleep(500);
                        }
                    
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        string taskResultUri = resp.task.taskResultUri;
                        taskResultUri = taskResultUri.Substring(taskResultUri.IndexOf("task/"));

                        SendHttpRequest(HttpVerb.GET, null, taskResultUri);

                        { // Should be an exception
                            var errorResp = GetResponse<HttpsErrorResponse>();

                            Assert.AreEqual(HttpStatusCode.InternalServerError, errorResp.Error.code);
                            Assert.IsTrue(errorResp.Error.message.Contains("testMessage"));
                        }

                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        SendHttpRequest(HttpVerb.DELETE, null, taskUri);
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


                        SendHttpRequest(HttpVerb.DELETE, null, taskUri);
                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

                        SendHttpRequest(HttpVerb.GET, null, taskUri);
                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

                        SendHttpRequest(HttpVerb.GET, null, taskResultUri);
                        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestPluginPostDataAsync()
                    {
                        var req = new TestRequestData() { Data = "dataExample" };

                        SendHttpRequest(HttpVerb.POST, req, "testPlugin/data");
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        var resp = GetResponse<TaskResponse>();

                        while (resp.task.isFinished == false)
                        {
                            string taskUri = resp.task.taskUri;
                            taskUri = taskUri.Substring(taskUri.IndexOf("task/"));

                            SendHttpRequest(HttpVerb.GET, null, taskUri);
                            resp = GetResponse<TaskResponse>();

                            Thread.Sleep(500);
                        }
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        string taskResultUri = resp.task.taskResultUri;
                        taskResultUri = taskResultUri.Substring(taskResultUri.IndexOf("task/"));

                        SendHttpRequest(HttpVerb.GET, null, taskResultUri);

                        var asyncResp = GetResponse<TestResponseData>();
                    }

                    [TestMethod]
                    public void TestPluginPostDataWithId()
                    {
                        var req = new TestRequestData() { Data = "dataExample" };

                        SendHttpRequest(HttpVerb.POST, req, "testPlugin/data/3");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var resp = GetResponse<TestResponseData>();

                        Assert.AreEqual(req.Data + "3", resp.Data);
                    }

                    [TestMethod]
                    public void TestPluginPostDataWithIdAndQueries()
                    {
                        var req = new TestRequestData() { Data = "dataExample" };

                        SendHttpRequest(HttpVerb.POST, req, "testPlugin/data/18/queries?test3=test3Result&test4=test4Result");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var resp = GetResponse<TestResponseData>();

                        var dic = new Dictionary<string, string>();
                        dic.Add("test3", "test3Result");
                        dic.Add("test4", "test4Result");
                        string dicToJson = JsonConvert.SerializeObject(dic);

                        Assert.AreEqual(req.Data + "18" + dicToJson, resp.Data);
                    }

                    [TestMethod]
                    public void TestAppliancePluginGet()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "appliance");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    }

                    [TestMethod]
                    public void TestApplianceGetPlugins()
                    {
                        Login();
                        SendHttpRequest(HttpVerb.GET, null, "plugins");
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                        var obj = JsonConvert.DeserializeObject<PluginsResponse>(response.Content.ReadAsStringAsync().Result);

                        Assert.IsTrue(obj.plugins.Any(x => x.name.Equals("agent_test_plugin")));
                    }

                    [TestMethod]
                    public void TestReport()
                    {
                        Login();

                        SendHttpRequest(HttpVerb.GET, null, "report");
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        var resp = GetResponse<TaskResponse>();

                        while (resp.task.isFinished == false)
                        {
                            string taskUri = resp.task.taskUri;
                            taskUri = taskUri.Substring(taskUri.IndexOf("task/"));

                            SendHttpRequest(HttpVerb.GET, null, taskUri);
                            resp = GetResponse<TaskResponse>();

                            Thread.Sleep(100);
                        }
                        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);

                        string taskResultUri = resp.task.taskResultUri;
                        taskResultUri = taskResultUri.Substring(taskResultUri.IndexOf("task/"));

                        SendHttpRequest(HttpVerb.GET, null, taskResultUri);

                        HttpContent requestContent = response.Content;
                        Assert.IsNotNull(requestContent.Headers.GetValues("Content-Type"));
                        Assert.AreEqual<string>(requestContent.Headers.GetValues("Content-Type").First(), "application/zip");
                       
                    }

                    private T GetResponse<T>()
                    {
                        HttpContent requestContent = response.Content;
                        string jsonContent = requestContent.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(jsonContent);
                    }

                    private void Login(string user = "Administrator", string password = "nopass")
                    {
                        //Send command
                        SendLoginRequest(user, password);
                        //Check response

                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        var serializer = new JsonWCFSerializer();
                        LoginResponse loginResponse = (LoginResponse)serializer.Deserialize(responseContents, typeof(LoginResponse));

                        Assert.AreNotEqual(null, loginResponse.accessToken);
                        Assert.AreNotEqual(null, loginResponse.accessToken.expiredTime);
                        Assert.AreNotEqual(null, loginResponse.accessToken.issuedTime);
                        Assert.AreNotEqual(null, loginResponse.accessToken.id);
                        //Set authentication token for later use.
                        token = loginResponse.accessToken.id;
                    }

                    private void SendLoginRequest(string user, string password)
                    {
                        LoginRequest request = new LoginRequest();
                        request.loginDetails = new LoginDetails();
                        request.loginDetails.user = user;
                        request.loginDetails.password = password;

                        SendHttpRequest(HttpVerb.POST, request, "login");
                    }

                   

                    private void SendHttpRequest(HttpVerb verb, Object request, string resource)
                    {
                        Send(verb, request, resource);
                        ReadResponse();
                    }
                    private void Send(HttpVerb verb, Object request, string resource, string contentType = "application/json")
                    {
                       // HttpClient httpClient = new HttpClient(myhandler);
                        Task task = null;
                        string postBody = JsonConvert.SerializeObject(request);
                        Uri fullPath = new Uri("https://localhost:5001/agent" + WebEndPoints.GENERAL_ENDPOINT + resource);
                        AddToken();
                        
                        switch (verb)
                        {
                            case HttpVerb.GET:
                                {
                                    task = Task.Run(async () => { response = await httpClient.GetAsync(fullPath); });
                                    break;
                                }
                            case HttpVerb.POST:
                                {
                                    task = Task.Run(async () => { response = await httpClient.PostAsync(fullPath, new StringContent(postBody, Encoding.UTF8, contentType)); });
                                    break;
                                }
                            case HttpVerb.PUT:
                                {
                                    task = Task.Run(async () => { response = await httpClient.PutAsync(fullPath, new StringContent(postBody, Encoding.UTF8, contentType)); });
                                    break;
                                }
                            case HttpVerb.DELETE:
                                {
                                    task = Task.Run(async () => { response = await httpClient.DeleteAsync(fullPath); });
                                    break;
                                }
                        }
                        //Wait for async task to return
                        task.Wait();

                    }
                    private void ReadResponse()
                    {
                        Assert.AreNotEqual(response, null);
                        Task task = Task.Run(async () => { responseContents = await response.Content.ReadAsStringAsync(); });

                        //Wait for async task to return
                        task.Wait();
                    }
                    private void AddToken()
                    {
                        httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
                        httpClient.DefaultRequestHeaders.Add("X-Auth-Token", token); 
                    }
                }
            }
        }
    }
}