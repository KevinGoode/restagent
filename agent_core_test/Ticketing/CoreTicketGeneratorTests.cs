using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kge.Agent.Rest.Server;
using Kge.Agent.Rest.Server.NativePlugin;
using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Kge.Agent.Library.SystemWrapper.System.IO;
using Kge.Agent.Library.SystemWrapper.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                [TestClass]
                public class CoreTicketGeneratorTests
                {
                    private RestServerMocking.ProcessSpec ProcessSpec;
                    private RestServerMocking.FileSpec FileSpec;
                    private RestServerMocking.DirectoryStaticSpec DirectorySpec;
                    private RestServerMocking.RegistrySpec RegistrySpec;

                    private CoreTicketGenerator TicketGenerator;

                    [TestInitialize]
                    public void Setup()
                    {
                        //RestServerIoc.Reset();

                        ProcessSpec = new RestServerMocking.ProcessSpec()
                        {
                            Running = false,
                            Output = new List<string>()
                            {
                                "** Gathering CPU information",
                                "** Gathering disk information",
                                "** Gathering SCSI information"
                            }
                        };
                        FileSpec = new RestServerMocking.FileSpec()
                        {
                            FilesExist = true
                        };
                        DirectorySpec = new RestServerMocking.DirectoryStaticSpec()
                        {
                            DirectoriesExist = true,
                            Files = new List<string>()
                            {
                                "cpu.htm",
                                "disks.htm",
                                "scsi.htm"
                            }
                        };
                        RegistrySpec = new RestServerMocking.RegistrySpec()
                        {
                            Exists = true,
                            InstallPath = @"C:\Program Files"
                        };

                        var processFactorySpec = new RestServerMocking.ProcessFactorySpec()
                        {
                            Process = RestServerMocking.CreateMockProcess(ProcessSpec)
                        };
                        var mockProcessFactory = RestServerMocking.CreateMockProcessFactory(processFactorySpec);
                        var mockFileStatic = RestServerMocking.CreateMockFileStatic(FileSpec);
                        var mockFileFactory = RestServerMocking.CreateMockFileFactory(FileSpec);
                        var mockDirectoryStatic = RestServerMocking.CreateMockDirectoryStatic(DirectorySpec);
                        var mockRegistry = RestServerMocking.CreateMockRegistry(RegistrySpec);
                        var staticAssemblyWrapper = new AssemblyStaticWrapper();

                        TicketGenerator = new CoreTicketGenerator(
                            mockProcessFactory,
                            mockRegistry,
                            mockFileFactory,
                            mockFileStatic,
                            mockDirectoryStatic,
                            staticAssemblyWrapper);
                    }

                    [TestMethod]
                    public void TestRestSrvr_TicketGenerator_HappyPath()
                    {
                        var reportCollection = TicketGenerator.GenerateReportFiles();

                        Assert.IsNotNull(reportCollection);

                        var reportSet = new HashSet<string>();

                        foreach (var report in reportCollection)
                        {
                            Assert.IsTrue(DirectorySpec.Files.Contains(report.Info.Name));
                            reportSet.Add(report.Info.Name);
                        }

                        foreach (var file in DirectorySpec.Files)
                        {
                            Assert.IsTrue(reportSet.Contains(file));
                        }
                    }

                    [TestMethod]
                    public void TestRestSrvr_TicketGenerator_NoRegistry()
                    {
                        RegistrySpec.Exists = false;
                        var reportCollection = TicketGenerator.GenerateReportFiles();

                        Assert.IsNotNull(reportCollection);

                        var reportSet = new HashSet<string>();

                        foreach (var report in reportCollection)
                        {
                            Assert.IsTrue(DirectorySpec.Files.Contains(report.Info.Name));
                            reportSet.Add(report.Info.Name);
                        }

                        foreach (var file in DirectorySpec.Files)
                        {
                            Assert.IsTrue(reportSet.Contains(file));
                        }
                    }

                    [TestMethod]
                    public void TestRestSrvr_TicketGenerator_Timeout()
                    {
                        ProcessSpec.Running = true;

                        var reportCollection = TicketGenerator.GenerateReportFiles();
                        var reportList = new List<ReportFile>(reportCollection);

                        Assert.AreEqual<int>(0, reportList.Count);
                    }

                    [TestMethod]
                    public void TestRestSrvr_TicketGenerator_MissingScript()
                    {
                        FileSpec.FilesExist = false;

                        var reportCollection = TicketGenerator.GenerateReportFiles();
                        var reportList = new List<ReportFile>(reportCollection);

                        Assert.AreEqual<int>(0, reportList.Count);
                    }
                }
            }
        }
    }
}
