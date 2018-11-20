using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kge.Agent.Rest.Server.API;
using System.Collections.Generic;
using Kge.Agent.Rest.Library;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_QueryParametersHandler
                {
                    [TestMethod]
                    public void TestNothingToRetrieve()
                    {
                        string uri = "/someresource/15";

                        QueryParametersHandler handler = new QueryParametersHandler();

                        var dic = handler.ConsumeQueryParameters(ref uri);

                        Assert.AreEqual("/someresource/15", uri);

                        Assert.IsNull(dic);
                    }

                    [TestMethod]
                    public void TestRetrieveSuccess()
                    {
                        string uri = "/someresource/15?name=salut";

                        QueryParametersHandler handler = new QueryParametersHandler();

                        var dic = handler.ConsumeQueryParameters(ref uri);

                        Assert.AreEqual(uri, "/someresource/15");

                        Assert.AreEqual(dic.Count, 1);

                        Assert.IsTrue(dic.ContainsKey("name"));
                        Assert.AreEqual( "salut", dic["name"]);
                    }

                    [TestMethod]
                    public void TestRetrieveTwoSuccess()
                    {
                        string uri = "/someresource/15?name=salut&success=failure";

                        QueryParametersHandler handler = new QueryParametersHandler();

                        var dic = handler.ConsumeQueryParameters(ref uri);

                        Assert.AreEqual(uri, "/someresource/15");

                        Assert.AreEqual(2, dic.Count);

                        Assert.IsTrue(dic.ContainsKey("name"));
                        Assert.AreEqual("salut", dic["name"]);

                        Assert.IsTrue(dic.ContainsKey("success"));
                        Assert.AreEqual("failure", dic["success"]);
                    }

                    [TestMethod]
                    public void TestRetrieveThreeSuccess()
                    {
                        string uri = "/someresource/15?name=salut&success=failure&test=testValue";

                        QueryParametersHandler handler = new QueryParametersHandler();

                        var dic = handler.ConsumeQueryParameters(ref uri);

                        Assert.AreEqual("/someresource/15", uri);

                        Assert.AreEqual(3, dic.Count);

                        Assert.IsTrue(dic.ContainsKey("name"));
                        Assert.AreEqual("salut", dic["name"]);

                        Assert.IsTrue(dic.ContainsKey("success"));
                        Assert.AreEqual("failure", dic["success"]);

                        Assert.IsTrue(dic.ContainsKey("test"));
                        Assert.AreEqual("testValue", dic["test"]);
                    }

                    [TestMethod]
                    public void TestRetrieveFourSuccess()
                    {
                        string uri = "/someresource/15?name=salut&success=failure&test=testValue&super=parameter";

                        QueryParametersHandler handler = new QueryParametersHandler();

                        var dic = handler.ConsumeQueryParameters(ref uri);

                        Assert.AreEqual(uri, "/someresource/15");

                        Assert.AreEqual(dic.Count, 4);

                        Assert.IsTrue(dic.ContainsKey("name"));
                        Assert.AreEqual(dic["name"], "salut");

                        Assert.IsTrue(dic.ContainsKey("success"));
                        Assert.AreEqual(dic["success"], "failure");

                        Assert.IsTrue(dic.ContainsKey("test"));
                        Assert.AreEqual("testValue", dic["test"]);

                        Assert.IsTrue(dic.ContainsKey("super"));
                        Assert.AreEqual(dic["super"], "parameter");
                    }
                }
            }
        }
    }
}
