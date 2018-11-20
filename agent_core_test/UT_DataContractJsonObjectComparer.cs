using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Login;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Appliance;
using Newtonsoft.Json;
using Kge.Agent.Rest.Library.Plugin.DataContracts;
using System.Runtime.Serialization;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class UT_JsonWCFSerializer
                {
                    [DataContract]
                    public class TestRequest : IGenericResponse
                    {
                        [DataMember(IsRequired = true)]
                        string testString { get; set; }

                        [DataMember(IsRequired = true)]
                        int testInteger { get; set; }
                    }

                    [DataContract]
                    public class TestRequest2 : IGenericResponse
                    {
                        [DataMember]
                        public TestRequest testRequest { get; set; }
                    }

                    [TestMethod]
                    public void TestExtraMember()
                    {
                        string json = "{\"loginDetails\":{\"user\":\"dev-user\",\"password\":\"somepass\", \"ipAddress\":\"192.168.0.1\"}}";
                        Type type = typeof(LoginRequest);

                        var jsonSerializer = new JsonWCFSerializer();

                        try
                        {
                            jsonSerializer.Deserialize(json, type);
                        }
                        catch (Kge.Agent.Rest.Library.MissingMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (MemberTypeException)
                        {
                            Assert.Fail();
                        }
                        catch (ExtraMemberException e)
                        {
                            Assert.AreEqual(e.MemberName, "ipAddress");
                            Assert.AreEqual(e.Path, "loginDetails");
                            Assert.AreEqual(e.Line, 1);
                            Assert.AreEqual(e.Position, 70);
                            return;
                        }
                        catch (JsonSyntaxException)
                        {
                            Assert.Fail();
                        }
                        Assert.Fail();
                    }

                    [TestMethod]
                    public void TestMissingMember()
                    {
                        string json = "{\"loginDetails\":{\"user\":\"dev-user\"}}";
                        Type type = typeof(LoginRequest);

                        var jsonSerializer = new JsonWCFSerializer();

                        try
                        {
                            jsonSerializer.Deserialize(json, type);
                        }
                        catch (Kge.Agent.Rest.Library.MissingMemberException e)
                        {
                            Assert.AreEqual(e.MemberName, "password");
                            Assert.AreEqual(e.Path, "loginDetails");
                            Assert.AreEqual(e.Line, 1);
                            Assert.AreEqual(e.Position, 35);
                            return;
                        }
                        catch (MemberTypeException)
                        {
                            Assert.Fail();
                        }
                        catch (ExtraMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (JsonSyntaxException)
                        {
                            Assert.Fail();
                        }
                        Assert.Fail();
                    }

                    

                    [TestMethod]
                    public void TestWrongTypeRoot()
                    {
                        string json = "{\"testString\":\"a string\",\"testInteger\":\"notInteger\"}";

                        Type type = typeof(TestRequest);

                        var jsonSerializer = new JsonWCFSerializer();

                        try
                        {
                            jsonSerializer.Deserialize(json, type);
                        }
                        catch (Kge.Agent.Rest.Library.MissingMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (ExtraMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (MemberTypeException)
                        {
                            return;
                        }
                        catch (JsonSyntaxException)
                        {
                            Assert.Fail();
                        }
                        Assert.Fail();
                    }

                    [TestMethod]
                    public void TestWrongTypeSub()
                    {
                        string json = "{\"testRequest\":{\"testString\":\"a string\",\"testInteger\":\"notInteger\"}}";

                        Type type = typeof(TestRequest2);

                        var jsonSerializer = new JsonWCFSerializer();

                        try
                        {
                            jsonSerializer.Deserialize(json, type);
                        }
                        catch (Kge.Agent.Rest.Library.MissingMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (ExtraMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (MemberTypeException)
                        {
                            return;
                        }
                        catch (JsonSyntaxException)
                        {
                            Assert.Fail();
                        }
                        Assert.Fail();
                    }

                    [TestMethod]
                    public void TestSyntaxError()
                    {
                        string json = "{awdawd\"testString\":\"a string\",\"testInteger\":\"notInteger\"}}";

                        Type type = typeof(TestRequest);

                        var jsonSerializer = new JsonWCFSerializer();

                        try
                        {
                            jsonSerializer.Deserialize(json, type);
                        }
                        catch (Kge.Agent.Rest.Library.MissingMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (ExtraMemberException)
                        {
                            Assert.Fail();
                        }
                        catch (MemberTypeException)
                        {
                            Assert.Fail();
                        }
                        catch (JsonSyntaxException)
                        {
                            return;
                        }
                        Assert.Fail();
                    }
                }
            }
        }
    }
}