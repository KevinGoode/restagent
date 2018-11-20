using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Net.Http;
using Kge.Agent.Rest.Server.API;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
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
                public class UT_APIManager
                {
                    [ClassInitialize]
                    public static void Suite_Setup(TestContext context)
                    {
                        var serverConfig = new WebServerConfig();
                        ProcessLogger.CreateInstance("agent", serverConfig.logPath, 5, 1024);
                    }

                    [TestInitialize]
                    public void Setup()
                    {
                        ProcessLogger.GetInstance().SetLogLevel(ProcessLogLevel.None);
                    }

                    [TestMethod]
                    public void TestDiscoveryByDirectory()
                    {
                        // Arrange
                        var mockedPluginLoader = new Mock<IPluginLoader>();
                        var mockedAPIRouter = new Mock<IAPIRouter>();

                        var listPlugins = new List<IPlugin>();
                        listPlugins.Add(new Plugin(typeof(TestClass1), new List<PluginAPI>()));
                        listPlugins.Add(new Plugin(typeof(TestClass2), new List<PluginAPI>()));

                        mockedPluginLoader.Setup(x => x.Discover(It.IsAny<string>())).Returns(() => listPlugins);

                        var listPluginsSentToRouter = new List<IPlugin>();
                        mockedAPIRouter.Setup(x => x.AddPlugin(It.IsAny<IPlugin>())).Callback<IPlugin>((paramPlugin) => listPluginsSentToRouter.Add(paramPlugin));

                        string givenPath = "/somepath/away";


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, mockedPluginLoader.Object);
                        apiManager.DiscoverAPIDirectory(givenPath);
                        var actualList = apiManager.GetPluginList();


                        // Assert
                        mockedPluginLoader.Verify(x => x.Discover(givenPath), Times.Once());
                        mockedAPIRouter.Verify(x => x.AddPlugin(It.IsAny<IPlugin>()), Times.Exactly(listPlugins.Count));
                        Assert.AreEqual(listPlugins.Count, actualList.Count);
                        Assert.IsTrue(Enumerable.SequenceEqual(actualList, listPluginsSentToRouter));
                    }

                    [TestMethod]
                    public void TestDiscoverySinglePlugin()
                    {
                        // Arrange
                        var mockedPluginLoader = new Mock<IPluginLoader>();
                        var mockedAPIRouter = new Mock<IAPIRouter>();

                        var plugin = new Plugin(typeof(TestClass1), new List<PluginAPI>());
                        mockedPluginLoader.Setup(x => x.DiscoverPlugin(It.IsAny<string>())).Returns(() => plugin);

                        string givenPath = "/somepath/somewhere";


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, mockedPluginLoader.Object);
                        apiManager.DiscoverAPI(givenPath);
                        var actualList = apiManager.GetPluginList();


                        // Assert
                        mockedPluginLoader.Verify(x => x.DiscoverPlugin(givenPath), Times.Once());
                        mockedAPIRouter.Verify(x => x.AddPlugin(plugin), Times.Once());
                        Assert.AreEqual(1, actualList.Count);
                        Assert.AreSame(plugin, actualList.First());
                    }

                    [TestMethod]
                    public void TestAddPluginSucceed()
                    {
                        // Arrange
                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        IPlugin plugin = new Plugin(typeof(TestClass1), new List<PluginAPI>());

                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        apiManager.AddPlugin(plugin);
                        var actualList = apiManager.GetPluginList();

                        // Assert
                        Assert.AreEqual(1, actualList.Count);
                        Assert.AreSame(plugin, actualList.First());
                        mockedAPIRouter.Verify(x => x.AddPlugin(plugin), Times.Once());
                    }

                    [TestMethod]
                    public void TestAddPluginFailed()
                    {
                        // Arrange
                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        IPlugin plugin = new Plugin(typeof(TestClass1), new List<PluginAPI>());

                        // Act & Assert
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        apiManager.AddPlugin(plugin);

                        try
                        {
                            apiManager.AddPlugin(plugin);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestRemovePluginSucceed()
                    {
                        // Arrange
                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        IPlugin plugin1 = new Plugin(typeof(TestClass1), new List<PluginAPI>());
                        IPlugin plugin2 = new Plugin(typeof(TestClass2), new List<PluginAPI>());


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        apiManager.AddPlugin(plugin1);
                        apiManager.AddPlugin(plugin2);

                        apiManager.RemovePlugin(apiManager.GetPluginList().First());

                        var actualList = apiManager.GetPluginList();


                        // Assert
                        Assert.AreEqual(1, actualList.Count);
                        Assert.AreSame(plugin2, actualList.First());
                        mockedAPIRouter.Verify(x => x.RemovePlugin(plugin1), Times.Once());
                    }

                    [TestMethod]
                    public void TestRemovePluginFailed()
                    {
                        // Arrange
                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        IPlugin plugin1 = new Plugin(typeof(TestClass1), new List<PluginAPI>());


                        // Act & Assert
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        apiManager.AddPlugin(plugin1);
                        apiManager.RemovePlugin(plugin1);

                        try
                        {
                            apiManager.RemovePlugin(plugin1);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestCallPostAPI()
                    {
                        // Arrange
                        var listPlugins = new List<IPlugin>();
                        var listAPI = new List<PluginAPI>();
                        listAPI.Add(new PluginAPI(typeof(TestClass1).GetMethod("postTest"), "1", "POST"));
                        listPlugins.Add(new Plugin(typeof(TestClass1), listAPI));

                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        var mockedWCFSerializer = new Mock<IWCFSerializer>();

                        var tuple = new Tuple<IPlugin, MethodInfo>(listPlugins[0], listPlugins[0].PluginAPIs[0].MethodInfo);
                        var dic = new Dictionary<string, string>();
                        var tupleDic = new Tuple<Tuple<IPlugin, MethodInfo>, Dictionary<string, string>>(tuple, dic);


                        var resolvedRoute = new ResolvedRoute(tuple.Item1, tuple.Item2, null, null);
                        mockedAPIRouter.Setup(x => x.GetRoute(It.IsAny<string>(), It.IsAny<string>())).Returns(resolvedRoute);

                        var request = new FakeRequest() { testData = "testData" };
                        mockedWCFSerializer.Setup(x => x.Deserialize(It.IsAny<string>(), It.IsAny<Type>())).Returns(request);


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        var ret = (FakeResponse)apiManager.CallPostAPI("1", "POST", "test", mockedWCFSerializer.Object);


                        // Assert
                        Assert.AreEqual(request.testData, ret.testData);
                        mockedAPIRouter.Verify(x => x.GetRoute(listAPI[0].UriTemplate, listAPI[0].QueryMethod), Times.Once());
                        mockedWCFSerializer.Verify(x => x.Deserialize("test", typeof(FakeRequest)), Times.Once());

                        // Assert - Check query parameters
                        Assert.AreEqual(0, TestClass1.SavedQueryParametersContainer.Count);
                    }

                    [TestMethod]
                    public void TestCallPostAPIQueryParameters()
                    {
                        // Arrange
                        var listPlugins = new List<IPlugin>();
                        var listAPI = new List<PluginAPI>();
                        listAPI.Add(new PluginAPI(typeof(TestClass1).GetMethod("postTest"), "1", "POST"));
                        listPlugins.Add(new Plugin(typeof(TestClass1), listAPI));

                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        var mockedWCFSerializer = new Mock<IWCFSerializer>();

                        var tuple = new Tuple<IPlugin, MethodInfo>(listPlugins[0], listPlugins[0].PluginAPIs[0].MethodInfo);
                        var dic = new Dictionary<string, string>();
                        var tupleDic = new Tuple<Tuple<IPlugin, MethodInfo>, Dictionary<string, string>>(tuple, dic);

                        var queryParameters = new QueryParametersContainer();
                        queryParameters.Add("test", "testString");
                        var resolvedRoute = new ResolvedRoute(tuple.Item1, tuple.Item2, null, queryParameters);
                        mockedAPIRouter.Setup(x => x.GetRoute(It.IsAny<string>(), It.IsAny<string>())).Returns(resolvedRoute);

                        var request = new FakeRequest() { testData = "testData" };
                        mockedWCFSerializer.Setup(x => x.Deserialize(It.IsAny<string>(), It.IsAny<Type>())).Returns(request);


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        var ret = (FakeResponse)apiManager.CallPostAPI("1", "POST", "test", mockedWCFSerializer.Object);


                        // Assert
                        Assert.AreEqual(request.testData, ret.testData);
                        mockedAPIRouter.Verify(x => x.GetRoute(listAPI[0].UriTemplate, listAPI[0].QueryMethod), Times.Once());
                        mockedWCFSerializer.Verify(x => x.Deserialize("test", typeof(FakeRequest)), Times.Once());

                        // Assert - Check query parameters
                        Assert.AreEqual(TestClass1.SavedQueryParametersContainer, queryParameters);
                    }

                    [TestMethod]
                    public void TestCallGetAPI()
                    {
                        // Arrange
                        var listPlugins = new List<IPlugin>();
                        var listAPI = new List<PluginAPI>();
                        listAPI.Add(new PluginAPI(typeof(TestClass1).GetMethod("getTest"), "1", "GET"));
                        listPlugins.Add(new Plugin(typeof(TestClass1), listAPI));

                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        var tuple = new Tuple<IPlugin, MethodInfo>(listPlugins[0], listPlugins[0].PluginAPIs[0].MethodInfo);
                        var dic = new Dictionary<string, string>();
                        var tupleDic = new Tuple<Tuple<IPlugin, MethodInfo>, Dictionary<string, string>>(tuple, dic);

                        var resolvedRoute = new ResolvedRoute(tuple.Item1, tuple.Item2, null, null);
                        mockedAPIRouter.Setup(x => x.GetRoute(It.IsAny<string>(), It.IsAny<string>())).Returns(resolvedRoute);


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        var ret = (FakeResponse)apiManager.CallGetAPI("1", "GET");


                        // Assert
                        // Assert - Check query parameters
                        Assert.AreEqual(0, TestClass1.SavedQueryParametersContainer.Count);

                        Assert.AreEqual(new TestClass1().getTest().testData, ret.testData);
                        mockedAPIRouter.Verify(x => x.GetRoute(listAPI[0].UriTemplate, listAPI[0].QueryMethod), Times.Once());
                    }

                    [TestMethod]
                    public void TestCallGetAPIQueryParameters()
                    {
                        // Arrange
                        var listPlugins = new List<IPlugin>();
                        var listAPI = new List<PluginAPI>();
                        listAPI.Add(new PluginAPI(typeof(TestClass1).GetMethod("getTest"), "1", "GET"));
                        listPlugins.Add(new Plugin(typeof(TestClass1), listAPI));

                        var mockedAPIRouter = new Mock<IAPIRouter>();
                        var tuple = new Tuple<IPlugin, MethodInfo>(listPlugins[0], listPlugins[0].PluginAPIs[0].MethodInfo);
                        var dic = new Dictionary<string, string>();
                        var tupleDic = new Tuple<Tuple<IPlugin, MethodInfo>, Dictionary<string, string>>(tuple, dic);

                        var queryParameters = new QueryParametersContainer();
                        queryParameters.Add("test", "testString");
                        var resolvedRoute = new ResolvedRoute(tuple.Item1, tuple.Item2, null, queryParameters);
                        mockedAPIRouter.Setup(x => x.GetRoute(It.IsAny<string>(), It.IsAny<string>())).Returns(resolvedRoute);


                        // Act
                        var apiManager = new APIManager(mockedAPIRouter.Object, new PluginLoader());
                        var ret = (FakeResponse)apiManager.CallGetAPI("1", "GET");


                        // Assert
                        
                        // Check query parameters
                        Assert.AreEqual(TestClass1.SavedQueryParametersContainer, queryParameters);

                        Assert.AreEqual(new TestClass1().getTest().testData, ret.testData);
                        mockedAPIRouter.Verify(x => x.GetRoute(listAPI[0].UriTemplate, listAPI[0].QueryMethod), Times.Once());
                    }
                }
            }
        }
    }
}