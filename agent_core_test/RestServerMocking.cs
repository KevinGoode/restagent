using Kge.Agent.Library;
using Kge.Agent.Library.SystemWrapper.System.Diagnostics;
using Kge.Agent.Library.SystemWrapper.System.IO;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Test
            {
                internal static class RestServerMocking
                {
                    public class ProcessSpec
                    {
                        public List<string> Output { get; set; }
                        public bool Running { get; set; }
                    }

                    public static IProcessWrapper CreateMockProcess(ProcessSpec spec)
                    {
                        var mockProc = new Mock<IProcessWrapper>();

                        mockProc.Setup(u => u.Running).Returns(() => spec.Running);
                        mockProc.Setup(u => u.Output).Returns(() => spec.Output);
                        mockProc.Setup(u => u.WaitForExit(It.IsAny<TimeSpan>())).Returns(() => !spec.Running);

                        return mockProc.Object;
                    }

                    public class ProcessFactorySpec
                    {
                        public IProcessWrapper Process { get; set; }
                    }

                    public static IProcessWrapperFactory CreateMockProcessFactory(ProcessFactorySpec spec)
                    {
                        var mockFact = new Mock<IProcessWrapperFactory>();

                        mockFact.Setup(
                            u => u.MakeProcess(
                                It.IsAny<string>(),
                                It.IsAny<string>())).Returns(() => spec.Process);

                        return mockFact.Object;
                    }

                    public class FileSpec
                    {
                        public bool FilesExist { get; set; }
                    }

                    public static IFileStaticWrapper CreateMockFileStatic(FileSpec spec)
                    {
                        var mockFile = new Mock<IFileStaticWrapper>();

                        mockFile.Setup(u => u.Exists(It.IsAny<string>())).Returns(() => spec.FilesExist);

                        return mockFile.Object;
                    }

                    public static IFileInfoWrapper CreateMockFileInfo(string path)
                    {
                        var filename = Path.GetFileName(path);

                        var mockFile = new Mock<IFileInfoWrapper>();

                        mockFile.Setup(u => u.Name).Returns(filename);

                        return mockFile.Object;
                    }

                    public static IFileInfoFactory CreateMockFileFactory(FileSpec spec)
                    {
                        var mockFact = new Mock<IFileInfoFactory>();

                        mockFact.Setup(
                            u => u.Create(
                                It.IsAny<string>())).Returns<string>(path => CreateMockFileInfo(path));

                        return mockFact.Object;
                    }

                    public class DirectoryStaticSpec
                    {
                        public bool DirectoriesExist { get; set; }
                        public ICollection<string> Files { get; set; }
                    }

                    public static IDirectoryStaticWrapper CreateMockDirectoryStatic(DirectoryStaticSpec spec)
                    {
                        var mockDir = new Mock<IDirectoryStaticWrapper>();

                        mockDir.Setup(u => u.Exists(It.IsAny<string>())).Returns(() => spec.DirectoriesExist);
                        mockDir.Setup(u => u.GetFiles(It.IsAny<string>())).Returns(() => spec.Files.ToArray());
                        mockDir.Setup(u => u.GetSubdirectories(It.IsAny<string>())).Returns(new string[0]);

                        return mockDir.Object;
                    }

                    public class RegistrySpec
                    {
                        public bool Exists { get; set; }
                        public string InstallPath { get; set; }
                    }

                    public static ICommonRegistry CreateMockRegistry(RegistrySpec spec)
                    {
                        var mockRegistry = new Mock<ICommonRegistry>();

                        mockRegistry.Setup(u => u.Exists).Returns(() => spec.Exists);
                        mockRegistry.Setup(u => u.InstallPath).Returns(() => spec.InstallPath);

                        return mockRegistry.Object;
                    }
                }
            }
        }
    }
}
