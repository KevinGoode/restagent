using Kge.Agent.Library.SystemWrapper.System.IO.Compression;
using Kge.Agent.Rest.Library;
using Kge.Agent.Rest.Library.Plugin;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Appliance;
using Kge.Agent.Rest.Library.Plugin.DataContracts.Login;
using Kge.Agent.Rest.Library.Plugin.Providers;
using Kge.Agent.Rest.Server.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.IO;
using Kge.Agent.Library.SystemWrapper.System.IO;
using Kge.Agent.Lang;
using System.Net;
using System.IO.Compression;
using Kge.Agent.Library;
using io=System.IO;
namespace Kge
{
    namespace Agent
    {
        namespace Rest
        {
            namespace Server
            {
                namespace NativePlugin
                {
                    [ExposedPlugin]
                    public class ReportPlugin : APluginImplementation
                    {
                        //[Dependency]
                        private IDetachedReportFilesProvider SupplementalReportProvider { get; set; }

                        private static Object SharedLock = new Object();

                        public ReportPlugin(QueryParametersContainer queryParameters)
                            : base(queryParameters)
                        {
                            SupplementalReportProvider = RestServerIoc.Resolve<IDetachedReportFilesProvider>();
                        }

                        public ReportPlugin()
                            : base(null)
                        {
                            SupplementalReportProvider = RestServerIoc.Resolve<IDetachedReportFilesProvider>();
                        }

                        public override PluginDetails GetPluginDetails()
                        {
                            return new PluginDetails()
                            {
                                name = "ReportPlugin",
                                version = "1.0.0.0",
                                capabilities = this.GetCapabilities()
                            };
                        }

                        public override Task<ReportFiles> ProvideReportFiles()
                        {
                            return SupplementalReportProvider.ProvideReportFiles();
                        }

                        [RouteDescription(@"/report", "GET", RestrictionLevel.Admin, true)]
                        public ReportResponse Report()
                        {
                            lock (SharedLock)
                            {
                                CommonRegistry reg = new CommonRegistry();
                                var resp = new ReportResponse();

                                var plugins = RootService.APIManager.GetPluginList();
                                var instanciatedPlugins = plugins.Select(x => x.InstantiatePlugin(null));

                                IFileStaticWrapper fileWrapper = RestServerIoc.Container.Resolve<IFileStaticWrapper>();
                                IZipArchiveFactory factory = RestServerIoc.Container.Resolve<IZipArchiveFactory>();

                                string timestamp = DateTime.UtcNow.ToString("dd-MM-yyyy-HHmmss");
                                int count = 1;
                                string rootFileName = SystemDriveHelper.SystemDrive + reg.ReportFilePath + Environment.MachineName + "-" + timestamp;
                                string archivePath = rootFileName + ".zip";
                                while (io.File.Exists(archivePath))
                                {
                                    count++;
                                    archivePath = rootFileName + "-" + count.ToString() + ".zip";
                                }
                                try
                                {
                                    ReportFiles[] reports = Task.WhenAll<ReportFiles>(instanciatedPlugins.Select(x => x.ProvideReportFiles())).Result;
                                    reports = reports.Where(x => x != null).ToArray();

                                    ReportFiles[] coreReports = { GetCoreReportFiles() };
                                    reports = reports.Concat(coreReports).ToArray();

                                    using (IZipArchiveWrapper archive = factory.Create(fileWrapper.OpenForWriting(archivePath), ZipArchiveMode.Create))
                                    {
                                        foreach (ReportFiles report in reports)
                                        {
                                            foreach (ReportFile reportFile in report)
                                            {
                                                IZipArchiveEntryWrapper entry = archive.CreateEntry(reportFile.Info.Name);
                                                Stream archiveFileStream = entry.Open();

                                                reportFile.Stream.CopyTo(archiveFileStream);
                                                reportFile.Stream.Close();

                                                archiveFileStream.Flush();
                                                archiveFileStream.Close();

                                                if (reportFile.DeleteAfterRead)
                                                {
                                                    reportFile.Info.Delete();
                                                }
                                            }
                                        }

                                        resp.FilePath = archivePath;
                                    }
                                }
                                catch (AggregateException aggException)
                                {
                                    aggException.Handle(innerException =>
                                    {
                                        Log.Error(innerException.ToString());
                                        return true;
                                    });

                                    throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.InternalServerError,
                                        Helper.SetAppTypeInErrorMessage(  500,
                                        LanguageHelper.Resolve("CORE_ERROR_FAILED_TO_RETURN_FILE"))),
                                        HttpStatusCode.InternalServerError);
                                }
                                catch
                                {
                                    throw new WebResponseException(new HttpsErrorResponse(HttpStatusCode.InternalServerError,
                                        Helper.SetAppTypeInErrorMessage(  500,
                                        LanguageHelper.Resolve("CORE_ERROR_FAILED_TO_ZIP_REPORT"))),
                                        HttpStatusCode.InternalServerError);
                                }

                                return resp;
                            }
                        }
                       private string getPath(string fullpath){
                            string path = "./";
                            int index = fullpath.LastIndexOf("/");
                            if(index >=0){
                              path = fullpath.Substring(0,index);
                            }
                            return path;
                       }
                       private string getPattern(string fullpath){
                            int index = fullpath.LastIndexOf("/");
                            if(index >=0){
                              index=0;
                            }
                            return fullpath.Substring(index + 1) + "*.*";
                       }
                        private ReportFiles GetCoreReportFiles()
                        {
                            ReportFiles reportFiles = new ReportFiles();
                            IFileStaticWrapper fileWrapper = RestServerIoc.Container.Resolve<IFileStaticWrapper>();
                            IFileInfoFactory fileInfoFactory = RestServerIoc.Container.Resolve<IFileInfoFactory>();
                            IProcessLogger logger = ProcessLogger.GetInstance();
                            String fullprefix = "";
                            if (logger != null)
                            {
                                fullprefix = logger.GetFilePrefix();
                                String path = getPath(fullprefix);
                                String pattern = getPattern(fullprefix);
                                if (Directory.Exists(path))
                                {
                                    DirectoryInfo dir = new DirectoryInfo(path);
                                    FileInfo[] files = dir.GetFiles(pattern);
                                    foreach (FileInfo file in files)
                                    {
                                        var stream = fileWrapper.OpenForReading(file.FullName);
                                        IFileInfoWrapper info = fileInfoFactory.Create(file.FullName);

                                        reportFiles.Add(new ReportFile(info, stream));
                                    }
                                }
                            }
                            return reportFiles;
                        }
                    }
                }
            }
        }
    }
}