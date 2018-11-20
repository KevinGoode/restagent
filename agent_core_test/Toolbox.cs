using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                #region Stub Classes

                public interface ITestClass
                {
                    FakeResponse getTest();
                    FakeResponse postTest(FakeRequest request);
                }

                public class TestClass1 : APluginImplementation, ITestClass
                {
                    public static QueryParametersContainer SavedQueryParametersContainer;

                    public TestClass1(QueryParametersContainer queryParameters = null)
                        : base(queryParameters)
                    {
                        SavedQueryParametersContainer = QueryParameters;
                    }

                    public virtual FakeResponse getTest()
                    {
                        return new FakeResponse() { testData = "nahlah" };
                    }

                    public virtual FakeResponse postTest(FakeRequest request)
                    {
                        return new FakeResponse() { testData = request.testData };
                    }

                    public override List<PluginAPI> GetPluginMethodsMetaData()
                    {
                        return new List<PluginAPI>()
                {
                    new PluginAPI(typeof(TestClass1).GetMethod("getTest"), @"/get1", "GET", RestrictionLevel.Admin),
                    new PluginAPI(typeof(TestClass1).GetMethod("postTest"), @"/post1", "POST", RestrictionLevel.None),
                };
                    }
                }

                public class TestClass2 : APluginImplementation, ITestClass
                {
                    public TestClass2(QueryParametersContainer queryParameters = null)
                        : base(queryParameters) { }

                    public virtual FakeResponse getTest()
                    {
                        return new FakeResponse() { testData = "nahlah" };
                    }

                    public virtual FakeResponse postTest(FakeRequest request)
                    {
                        return new FakeResponse() { testData = request.testData };
                    }

                    public override List<PluginAPI> GetPluginMethodsMetaData()
                    {
                        return new List<PluginAPI>()
                {
                    new PluginAPI(typeof(TestClass2).GetMethod("getTest"), @"/get2", "GET"),
                    new PluginAPI(typeof(TestClass2).GetMethod("postTest"), @"/post2", "POST"),
                };
                    }
                }

                public class FakeRequest : IGenericRequest
                {
                    public object testData;
                }

                public class FakeRequestLogin : IGenericRequest
                {
                    public FakeRequestLoginDetails loginDetails { get; set; }
                }

                public class FakeRequestLoginDetails
                {
                    public string user { get; set; }
                    public string password { get; set; }
                    public string weather { get; set; }
                }

                public class FakeResponse : IGenericResponse
                {
                    public object testData;
                }

                #endregion
            }
        }
    }
}
