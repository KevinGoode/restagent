using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Kge.Agent.Rest.Server.API;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_ResourceIdMatcher
                {
                     [TestMethod]
                    public void TestMatchSimpleSuccess()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}";
                        string queryString = @"/rooturl/15";

                        Assert.IsTrue(queryMatcher.Match(uriTemplate, queryString));
                        var ret = queryMatcher.ExtractResourceIds(uriTemplate, queryString);

                        ret = new Dictionary<string, string>(ret, StringComparer.OrdinalIgnoreCase);

                        Assert.IsTrue(ret.ContainsKey("id"));
                        Assert.AreEqual("15", ret["id"]);

                        
                    }
                    [TestMethod]
                    public void TestMatchSuccess()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}/{blahblah}";
                        string queryString = @"/rooturl/15/18";

                        Assert.IsTrue(queryMatcher.Match(uriTemplate, queryString));
                        var ret = queryMatcher.ExtractResourceIds(uriTemplate, queryString);

                        ret = new Dictionary<string, string>(ret, StringComparer.OrdinalIgnoreCase);

                        Assert.IsTrue(ret.ContainsKey("id"));
                        Assert.AreEqual("15", ret["id"]);

                        Assert.IsTrue(ret.ContainsKey("blahblah"));
                        Assert.AreEqual("18", ret["blahblah"]);
                    }

                    [TestMethod]
                    public void TestMatchSuccessMultipleParamsSameArea()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}&{blahblah}";
                        string queryString = @"/rooturl/15&weshwesh";

                        Assert.IsTrue(queryMatcher.Match(uriTemplate, queryString));

                        var ret = queryMatcher.ExtractResourceIds(uriTemplate, queryString);
                        ret = new Dictionary<string, string>(ret, StringComparer.OrdinalIgnoreCase);

                        Assert.IsTrue(ret.ContainsKey("id"));
                        Assert.AreEqual("15", ret["id"]);

                        Assert.IsTrue(ret.ContainsKey("blahblah"));
                        Assert.AreEqual("weshwesh", ret["blahblah"]);
                    }

                    [TestMethod]
                    public void TestMatchSuccessOnlyOneParamWithSlash()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/{id}";
                        string queryString = @"/15";

                        Assert.IsTrue(queryMatcher.Match(uriTemplate, queryString));
                        var ret = queryMatcher.ExtractResourceIds(uriTemplate, queryString);
                        ret = new Dictionary<string, string>(ret, StringComparer.OrdinalIgnoreCase);

                        Assert.IsTrue(ret.ContainsKey("id"));
                        Assert.AreEqual("15", ret["id"]);
                    }

                    [TestMethod]
                    public void TestMatchFailureNumberOfParams()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}/{test}";
                        string queryString = @"/rooturl/15";

                        Assert.IsFalse(queryMatcher.Match(uriTemplate, queryString));
                    }

                    [TestMethod]
                    public void TestMatchFailureDifferentUrl()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}/{test}";
                        string queryString = @"/test/15/";

                        Assert.IsFalse(queryMatcher.Match(uriTemplate, queryString));
                    }

                    [TestMethod]
                    public void TestMatchFailureNoUriTemplate()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"";
                        string queryString = @"/test/15/";

                        Assert.IsFalse(queryMatcher.Match(uriTemplate, queryString));
                    }

                    [TestMethod]
                    public void TestMatchFailureNoResourceIds()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"/rooturl/{id}/{blahblah}";
                        string queryString = @"";

                        Assert.IsFalse(queryMatcher.Match(uriTemplate, queryString));
                    }

                    [TestMethod]
                    public void TestMatchFailureTotallyDifferent()
                    {
                        var queryMatcher = new ResourceIdMatcher();

                        string uriTemplate = @"awdawop[ida[wkd['aplwd";
                        string queryString = @"awd[poawp[odiapoijwd;";

                        Assert.IsFalse(queryMatcher.Match(uriTemplate, queryString));
                    }
                }
            }
        }
    }
}