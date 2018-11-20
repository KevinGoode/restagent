using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Library;
using Kge.Agent.Rest.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class RestLibraryIocUnitTests
                {
                    [TestMethod]
                    public void TestRestLib_Ioc_Consistency()
                    {
                        Assert.IsNotNull(RestLibraryIoc.Container);
                        Assert.AreEqual<ContainerBuilder>(RestLibraryIoc.Container, RestLibraryIoc.Container);
                    }

                    
                }
            }
        }
    }
}
