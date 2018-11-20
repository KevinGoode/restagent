using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Collections.Generic;
using Kge.Agent.Rest.Server.API;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Server;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_RightsManager
                {
                    [TestMethod]
                    public void TestRestrictionAdminAsAdmin()
                    {
                        // Arrange
                        var mockedApiRouter = new Mock<IAPIRouter>();
                        var mockedWebAuthentication = new Mock<IWebAuthentication>();

                        string expectedToken = "1234";
                        mockedWebAuthentication.Setup(x => x.GetToken()).Returns(expectedToken);

                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var rightsManager = new RightsManager(mockedApiRouter.Object, mockedWebAuthentication.Object);
                        mockedApiRouter.Raise(x => x.OnRouteRetrieved += null, mockedApiRouter.Object, plugin, plugin.PluginAPIs[0].MethodInfo);


                        // Assert
                        mockedWebAuthentication.Verify(x => x.ValidateToken(expectedToken), Times.Once());
                    }

                    [TestMethod]
                    public void TestRestrictionAdminAsNone()
                    {
                        // Arrange
                        var mockedApiRouter = new Mock<IAPIRouter>();
                        var mockedWebAuthentication = new Mock<IWebAuthentication>();

                        mockedWebAuthentication.Setup(x => x.GetToken()).Returns("");

                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act & Assert
                        var rightsManager = new RightsManager(mockedApiRouter.Object, mockedWebAuthentication.Object);

                        try
                        {
                            mockedApiRouter.Raise(x => x.OnRouteRetrieved += null, mockedApiRouter.Object, plugin, plugin.PluginAPIs[0].MethodInfo);
                            Assert.Fail();
                        }
                        catch { }
                    }

                    [TestMethod]
                    public void TestRestrictionNone()
                    {
                        // Arrange
                        var mockedApiRouter = new Mock<IAPIRouter>();
                        var mockedWebAuthentication = new Mock<IWebAuthentication>();

                        IPlugin plugin = new Plugin(typeof(TestClass1), new TestClass1().GetPluginMethodsMetaData());


                        // Act
                        var rightsManager = new RightsManager(mockedApiRouter.Object, mockedWebAuthentication.Object);
                        mockedApiRouter.Raise(x => x.OnRouteRetrieved += null, mockedApiRouter.Object, plugin, plugin.PluginAPIs[1].MethodInfo);


                        // Assert
                        mockedWebAuthentication.Verify(x => x.GetToken(), Times.Never());
                        mockedWebAuthentication.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Never());
                    }
                }
            }
        }
    }
}