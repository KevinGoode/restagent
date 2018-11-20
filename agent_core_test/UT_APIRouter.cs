using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kge.Agent.Rest.Server.API;
using System.Collections.Generic;
using System.Net.Http;
using Kge.Agent.Library;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_APIRouter
                {
                    [ClassInitialize]
                    public static void Suite_Setup(TestContext context)
                    {
                        root = "/" + new CommonRegistry().RootNamespace;
                        
                    }
                    [TestMethod]
                    public void TestAddPlugin()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddPlugin(plugin);


                        // Assert
                        Assert.AreEqual(1, apiRouter.PluginCount);
                        Assert.AreEqual(2, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestRemovePluginSucceed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddPlugin(plugin);
                        apiRouter.RemovePlugin(plugin);


                        // Assert
                        Assert.AreEqual(0, apiRouter.PluginCount);
                        Assert.AreEqual(0, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestRemovePluginFailed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act & Assert
                        var apiRouter = new APIRouter();

                        try
                        {
                            apiRouter.RemovePlugin(plugin);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestContainsPluginTrue()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddPlugin(plugin);


                        // Assert
                        Assert.IsTrue(apiRouter.ContainsPlugin(plugin));
                    }

                    [TestMethod]
                    public void TestContainsPluginFalse()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var apiRouter = new APIRouter();


                        // Assert
                        Assert.IsFalse(apiRouter.ContainsPlugin(plugin));
                    }

                    [TestMethod]
                    public void TestClear()
                    {
                        // Arrange
                        IPlugin plugin1 = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        IPlugin plugin2 = new Plugin(typeof(TestClass2), new TestClass2().GetPluginMethodsMetaData());


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddPlugin(plugin1);
                        apiRouter.AddPlugin(plugin2);
                        apiRouter.Clear();


                        // Assert
                        Assert.AreEqual(0, apiRouter.PluginCount);
                        Assert.AreEqual(0, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestAddRouteSucceed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);


                        // Assert
                        Assert.AreEqual(1, apiRouter.PluginCount);
                        Assert.AreEqual(1, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestAddRouteFailed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];


                        // Act & Assert
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);

                        try
                        {
                            apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestRemoveRouteSucceed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                        apiRouter.RemoveRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod));

                        // Assert
                        Assert.AreEqual(0, apiRouter.PluginCount);
                        Assert.AreEqual(0, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestRemoveRouteFailed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        // Act & Assert
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                        apiRouter.RemoveRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod));

                        try
                        {
                            apiRouter.RemoveRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod));
                            Assert.Fail();
                        }
                        catch { }

                    }

                    [TestMethod]
                    public void TestContainsRoute()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);

                        // Assert
                        Assert.IsTrue(apiRouter.ContainsRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod)));
                        Assert.AreEqual(1, apiRouter.PluginCount);
                        Assert.AreEqual(1, apiRouter.RouteCount);
                    }

                    [TestMethod]
                    public void TestGetRouteSucceed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);

                        var route = apiRouter.GetRoute(root + pApi.UriTemplate, pApi.QueryMethod);

                        // Assert
                        Assert.AreEqual(plugin, route.Plugin);
                        Assert.AreEqual(pApi.MethodInfo, route.Method);
                    }

                    [TestMethod]
                    public void TestGetRouteFailed()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        // Act & Assert
                        var apiRouter = new APIRouter();

                        try
                        {
                            var route = apiRouter.GetRoute(root + pApi.UriTemplate, pApi.QueryMethod);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestGetRouteQueryParameters()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        string uri = pApi.UriTemplate + "?test=testString";


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                        var route = apiRouter.GetRoute(root + uri, pApi.QueryMethod);


                        // Assert
                        Assert.AreEqual(plugin, route.Plugin);

                        Assert.AreEqual(1, route.QueryParameters.Count);

                        Assert.IsTrue(route.QueryParameters.ContainsKey("test"));
                        Assert.AreEqual("testString", route.QueryParameters["test"]);
                    }

                    [TestMethod]
                    public void TestGetRouteMultipleQueryParameters()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        string uri = pApi.UriTemplate + "?test=testString&test2=testString2&test3=testString3";


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                        var route = apiRouter.GetRoute(root + uri, pApi.QueryMethod);


                        // Assert
                        Assert.AreEqual(plugin, route.Plugin);

                        Assert.AreEqual(3, route.QueryParameters.Count);

                        Assert.IsTrue(route.QueryParameters.ContainsKey("test"));
                        Assert.AreEqual("testString", route.QueryParameters["test"]);

                        Assert.IsTrue(route.QueryParameters.ContainsKey("test2"));
                        Assert.AreEqual( "testString2", route.QueryParameters["test2"]);

                        Assert.IsTrue(route.QueryParameters.ContainsKey("test2"));
                        Assert.AreEqual( "testString3", route.QueryParameters["test3"]);
                    }

                    [TestMethod]
                    public void TestGetRouteNoQueryParameters()
                    {
                        // Arrange
                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());
                        var pApi = plugin.PluginAPIs[0];

                        string uri = pApi.UriTemplate;


                        // Act
                        var apiRouter = new APIRouter();
                        apiRouter.AddRoute(new APIRouteDescription(pApi.UriTemplate, pApi.QueryMethod), plugin, pApi.MethodInfo);
                        var route = apiRouter.GetRoute(root + uri, pApi.QueryMethod);


                        // Assert
                        Assert.AreEqual(plugin, route.Plugin);

                        Assert.AreEqual(0, route.QueryParameters.Count);
                    }
                    static private string root;
                }
            }
        }
    }
}