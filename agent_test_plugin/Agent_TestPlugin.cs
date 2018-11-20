using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using Kge.Agent.Rest.Library.Plugin.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                namespace TestPlugin
                {
                    [DataContract]
                    public class TestResponseQueryParameters : IGenericResponse
                    {
                        [DataMember]
                        public Dictionary<string, string> QueryParameters { get; set; }
                    }

                    [DataContract]
                    public class TestResponsePing : IGenericResponse
                    {
                        [DataMember]
                        public bool Alive { get; set; }
                    }

                    [DataContract]
                    public class TestResponseGetId : IGenericResponse
                    {
                        [DataMember]
                        public int Id { get; set; }
                    }

                    public class TestRequestData : IGenericRequest
                    {
                        [DataMember]
                        public string Data { get; set; }
                    }

                    public class TestResponseData : IGenericResponse
                    {
                        [DataMember]
                        public string Data { get; set; }
                    }

                    [ExposedPlugin]
                    public class agent_test_plugin : APluginImplementation
                    {
                        public override Capability[] GetCapabilities()
                        {
                            return new Capability[]
                            {
                                new Capability() { name = "SomeDeps", version = "1.0.0" },
                            };
                        }


                        public agent_test_plugin(QueryParametersContainer queryContainer) : base(queryContainer) { }
                        public agent_test_plugin() : base(null) { }

                        [RouteDescription(@"/testPlugin/alive", "GET", RestrictionLevel.None)]
                        public TestResponsePing GetIsAlive()
                        {
                            return new TestResponsePing() { Alive = true };
                        }

                        [RouteDescription(@"/testPlugin/queryParameters", "GET", RestrictionLevel.None)]
                        public TestResponseQueryParameters GetGivenQueryParameters()
                        {
                            var dic = new Dictionary<string, string>(this.QueryParameters);

                            return new TestResponseQueryParameters() { QueryParameters = dic };
                        }

                        [RouteDescription(@"/testPlugin/getId/{id}", "GET", RestrictionLevel.None)]
                        public TestResponseGetId GetId(int id)
                        {
                            return new TestResponseGetId() { Id = id };
                        }

                        [RouteDescription(@"/testPlugin/securityCheck", "GET")]
                        public TestResponsePing GetSecurityCheck()
                        {
                            return new TestResponsePing() { Alive = true };
                        }

                        [RouteDescription(@"/testPlugin/securityCheckAsync", "GET", RestrictionLevel.Admin, true)]
                        public TestResponsePing GetSecurityCheckAsync()
                        {
                            return new TestResponsePing() { Alive = true };
                        }

                        [RouteDescription(@"/testPlugin/securityCheckAsyncException", "GET", RestrictionLevel.None, true)]
                        public TestResponsePing GetSecurityCheckAsyncException()
                        {
                            throw new WebResponseException(new HttpsErrorResponse(System.Net.HttpStatusCode.InternalServerError,
                                Helper.SetAppTypeInErrorMessage(500, "testMessage")),
                                System.Net.HttpStatusCode.InternalServerError);
                        }

                        [RouteDescription(@"/testPlugin/data", "POST", RestrictionLevel.None, true)]
                        public TestResponseData PostDataAsync(TestRequestData req)
                        {
                            return new TestResponseData() { Data = req.Data };
                        }

                        [RouteDescription(@"/testPlugin/data/{id}", "POST", RestrictionLevel.None)]
                        public TestResponseData PostDataId(TestRequestData req, int id)
                        {
                            return new TestResponseData() { Data = req.Data + id.ToString() };
                        }

                        [RouteDescription(@"/testPlugin/data/{id}/queries", "POST", RestrictionLevel.None)]
                        public TestResponseData PostDataIdQueryParameters(TestRequestData req, int id)
                        {
                            var serializer = new JsonWCFSerializer();

                            return new TestResponseData() { Data = req.Data + id.ToString() + serializer.Serialize(QueryParameters) };
                        }
                    }
                }
            }
        }
    }
}
