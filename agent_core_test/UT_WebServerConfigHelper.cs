using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library;
using Kge.Agent.Rest.Library;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_WebServerConfigHelper
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
                    public void Test_SetLogLevel_HappyPath()
                    {
                        WebServiceConfigHelper.SetLogLevel("4");
                        Assert.AreEqual(
                            ProcessLogLevel.Info,
                            ProcessLogger.GetInstance().GetLogLevel());
                    }

                    [TestMethod]
                    public void Test_SetLogLevel_OutOfRange()
                    {
                        WebServiceConfigHelper.SetLogLevel("10");
                        Assert.AreEqual(
                            ProcessLogLevel.None,
                            ProcessLogger.GetInstance().GetLogLevel());
                    }

                    [TestMethod]
                    public void Test_SetLogLevel_Unrepresentable()
                    {
                        WebServiceConfigHelper.SetLogLevel("10000000000"); // 10^10 > 2^32
                        Assert.AreEqual(
                            ProcessLogLevel.None,
                            ProcessLogger.GetInstance().GetLogLevel());
                    }

                    [TestMethod]
                    public void Test_SetLogLevel_Text()
                    {
                        WebServiceConfigHelper.SetLogLevel("non-numeric");
                        Assert.AreEqual(
                            ProcessLogLevel.None,
                            ProcessLogger.GetInstance().GetLogLevel());
                    }

                    [TestMethod]
                    public void Test_SetLogLevel_Whitespace()
                    {
                        WebServiceConfigHelper.SetLogLevel(" 4 ");
                        Assert.AreEqual(
                            ProcessLogLevel.Info,
                            ProcessLogger.GetInstance().GetLogLevel());
                    }
                }
            }
        }
    }
}
