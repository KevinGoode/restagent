using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace Test
                {
                    [TestClass]
                    public class RestServerIocUnitTests
                    {
                        [TestMethod]
                        public void TestRestSrvr_Ioc_Consistency()
                        {
                            Assert.IsNotNull(RestServerIoc.Container);
                            Assert.AreEqual<IContainer>(RestServerIoc.Container, RestServerIoc.Container);
                        }

                       
                    }
                }
            }
        }
    }
}
