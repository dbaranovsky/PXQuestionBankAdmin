using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.ServiceProcess;
using System.Data.SqlClient;
using System.Data;
using Mono.Options;
using UploadResources.DataContainer;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using UploadResources.DataAccess;
using UploadResources.DataAccessLogic;
using UploadResources.Exception;
namespace UploadResources
{
    class Program
    {


        static readonly object _locker = new object();

        static void Main(string[] args)
        {

                // running as console app
                Start();

                Console.Write("press any key to exit...");
                Console.ReadKey();

                Stop();

        }

        /// <summary>
        /// Uses the connection to DLAP to upload resource from the local sourcePath to the specified entity in the specified environment using
        /// targetPath as the base resource path.
        /// </summary>
        /// <param name="connection">Connection to DLAP.</param>
        /// <param name="environment">Environment to direct commands against.</param>
        /// <param name="entityId">ID of the entity to upload resources to.</param>
        /// <param name="targetPath">Base resource path.</param>
        /// <param name="sourcePath">Local directory containing resources to upload.</param>
        private static void UploadResources(ISessionManager connection, TitleDC titleDC, bool async, int threads)
        {
            var files = new List<string>();
            long totalFileSize = 0;
            var timer = Stopwatch.StartNew();
            var tasks = new List<System.Threading.Tasks.Task>();
            var fileCount = 0;

            if (titleDC.IsFileList == 0)
            {
                FindAllFiles(titleDC.SourcePath, files, ref totalFileSize, true);
                fileCount = files.Count;
                SaveDisplayMessage("Starting upload of " + titleDC.SourcePath + " to " + titleDC.Environment + ". " + fileCount.ToString() + " files to process.");
                

                foreach (var file in files)
                {
                    var fixedPath = FixPath(file, titleDC.SourcePath, titleDC.TargetPath);
                    tasks.Add(UploadResource(connection, titleDC, fixedPath, file, async));

                    //SaveDisplayMessage(fixedPath);
                    if (async && (threads > 0) && (tasks.Count >= threads))
                    {
                        System.Threading.Tasks.Task.WaitAny(tasks.ToArray());
                        tasks.RemoveAll(t => t.IsCompleted);
                    }
                }
            }
            else
            {
                TitleFileListDC titleFileListDC = new TitleFileListDC();
                titleFileListDC.TitleID = titleDC.TitleID;

                TransactionFactory.ConstructToken(titleFileListDC);
                DBServices.GetFileListByTitleID(titleFileListDC);
                TransactionFactory.Dispose(titleFileListDC);
                if (titleFileListDC.TitleFileListDataSet.Tables.Count > 0 && titleFileListDC.TitleFileListDataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable filesTable = titleFileListDC.TitleFileListDataSet.Tables[0];
                    fileCount = filesTable.Rows.Count;

                    SaveDisplayMessage("Starting upload of provided file list files  to " + titleDC.Environment + ". " + fileCount.ToString() + " files to process.");
                    
                    for (int i = 0; i < filesTable.Rows.Count; i++)
                    {
                        try
                        {
                            var file = new System.IO.FileInfo(filesTable.Rows[i]["SourceFilePath"].ToString());
                            totalFileSize = totalFileSize + file.Length;
                        }
                        catch{}

                        tasks.Add(UploadResource(connection, titleDC, filesTable.Rows[i]["TargetFilePath"].ToString(), filesTable.Rows[i]["SourceFilePath"].ToString(), async));

                        //SaveDisplayMessage(fixedPath);
                        if (async && (threads > 0) && (tasks.Count >= threads))
                        {
                            System.Threading.Tasks.Task.WaitAny(tasks.ToArray());
                            tasks.RemoveAll(t => t.IsCompleted);
                        }
                        
                    }

                }
    
            }

            
            SaveDisplayMessage("waiting for all uploads to complete...");
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            titleDC.ProcessingEndTime = DateTime.Now;
            titleDC.FileCount = fileCount;
            titleDC.FileSizeString = FormatFileSize(totalFileSize);
            TransactionFactory.ConstructToken(titleDC);
            DBServices.UpdateTitleUploadStatus(titleDC);
            if(titleDC.ProcessingErrors != null && titleDC.ProcessingErrors.Count > 0)
                DBServices.LogUploadError(titleDC);

            TransactionFactory.Dispose(titleDC);

            timer.Stop();
            SaveDisplayMessage(string.Format("Processed {0} files ({1}) in {2}h:{3}m:{4}s:{5}ms", titleDC.FileCount, titleDC.FileSizeString, timer.Elapsed.Hours, timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds));
        }

        /// <summary>
        /// Uploads a local file specified by sourcePath to the given entityId at targetPath in the given environment.
        /// </summary>
        /// <param name="connection">Connection to DLAP.</param>
        /// <param name="environment">Environment to direct commands against.</param>
        /// <param name="entityId">ID of the entity to upload resources to.</param>
        /// <param name="targetPath">Exact resource path to store resource to.</param>
        /// <param name="sourcePath">Local file path of source resource.</param>
        private static System.Threading.Tasks.Task UploadResource(ISessionManager connection,TitleDC titleDC, string targetPath, string sourcePath, bool async = false)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var cmd = new PutResource
                {
                    Resource = new Resource
                    {
                        EntityId = titleDC.TargetEntityId,
                        Url = targetPath
                    },
                    RunAsync = false
                };

                

                int Attempts = 3;
                while (true)
                {
                    try
                    {
                        Attempts = Attempts - 1;
                        File.OpenRead(sourcePath).CopyTo(cmd.Resource.GetStream());
                        connection.CurrentSession.ExecuteAsAdmin(titleDC.Environment, cmd);
                        break;
                    }
                    catch (System.IO.FileNotFoundException exception)
                    {
                        lock (_locker)
                        {
                            titleDC.ProcessingErrors.Add(new UploadErrorsDC { Message = exception.Message, SourcePath = sourcePath, TargetPath = cmd.Resource.Url });
                            SaveDisplayMessage(string.Format("FAILURE:{0}\r\nSOURCE:{1}\r\nDESTINATION:{2}", exception.Message, sourcePath, cmd.Resource.Url));
                        }
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        if (Attempts > 0)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        else
                        {
                            lock (_locker)
                            {
                                titleDC.ProcessingErrors.Add(new UploadErrorsDC { Message = ex.Message, SourcePath = sourcePath, TargetPath = cmd.Resource.Url });
                                SaveDisplayMessage(string.Format("FAILURE:{0}\r\nSOURCE:{1}\r\nDESTINATION:{2}", ex.Message, sourcePath, cmd.Resource.Url));
                            }
                            break;
                        }
                    }
                }

            });

            if (!async)
            {
                // if not running in async mode, then force the task to wait until completion
                task.Wait();
            }

            return task;
        }        

        #region Helper Methods

        /// <summary>
        /// Returns a formated string containing the appropriate file size suffix.
        /// </summary>
        /// <param name="lengthInBytes">Size of a file in bytes</param>
        /// <returns>formated string containing the appropriate file size suffix.</returns>
        private static string FormatFileSize(long lengthInBytes)
        {
            var size = string.Empty;
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (lengthInBytes == 0)
                return "0" + suf[0];

            long bytes = Math.Abs(lengthInBytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            size = (Math.Sign(lengthInBytes) * num).ToString() + suf[place];

            return size;
        }

        /// <summary>
        /// Recursively builds a list of all files in a directory.
        /// </summary>
        /// <param name="sourcePath">directory to start at.</param>
        /// <param name="files">accumulated list of files, may not be null.</param>
        private static void FindAllFiles(string sourcePath, List<string> files, ref long totalFileSize, bool isRoot)
        {
            if (files == null)
            {
                totalFileSize = 0;
                throw new ArgumentNullException("files");
            }
            if (isRoot)
            {
                var filesInRoot = Directory.GetFiles(sourcePath);

                //calculate the total size of the files in the directory
                totalFileSize = filesInRoot.Map(f => new FileInfo(f)).Map(fi => fi.Length).Reduce((s, t) => t += s, totalFileSize);

                files.AddRange(filesInRoot);
            }

            foreach (var dir in Directory.GetDirectories(sourcePath))
            {
                var filesInDir = Directory.GetFiles(dir);

                //calculate the total size of the files in the directory
                totalFileSize = filesInDir.Map(f => new FileInfo(f)).Map(fi => fi.Length).Reduce((s, t) => t += s, totalFileSize);

                files.AddRange(filesInDir);
                FindAllFiles(dir, files, ref totalFileSize,false);
            }
        }

        /// <summary>
        /// Fixes the given filePath so that it becomes a valid Resource path.
        /// </summary>
        /// <param name="filePath">Path to a local file.</param>
        /// <param name="sourcePath">Base path the file was found starting at.</param>
        /// <param name="baseResourcePath">Base resource path the file should exist under.</param>
        /// <returns></returns>
        private static string FixPath(string filePath, string sourcePath, string baseResourcePath)
        {
            var fixedPath = filePath.Remove(0, sourcePath.Length);
            fixedPath = fixedPath.TrimStart('\\');
            fixedPath = string.Format("{0}/{1}", baseResourcePath, fixedPath);
            fixedPath = fixedPath.Replace("\\", "/");

            return fixedPath;
        }



        /// <summary>
        /// Creates a session manager with an active connection to DLAP.
        /// </summary>
        /// <returns>SessionManager to communicate with DLAP.</returns>
        private static ISessionManager EstablishConnection()
        {
            var sm = new ThreadSessionManager(new Bfw.Common.Logging.NullLogger(), new Bfw.Common.Logging.NullTraceManager());

            sm.CurrentSession = sm.StartAnnonymousSession();

            return sm;
        }

        #endregion

        private static void Start()
        {


            try
            {
                var sm = EstablishConnection();
                int ThreadLimit = 10;
                bool Async = true;                              

                while (true)
                {

                    TitleDC titleDC = new TitleDC();
                    TransactionFactory.ConstructToken(titleDC);
                    DBServices.GetTitleToUpload(titleDC);                    
                    TransactionFactory.Dispose(titleDC);
                    if (titleDC.TitleDataSet.Tables.Count > 0 && titleDC.TitleDataSet.Tables[0].Rows.Count > 0)
                    {
                        titleDC.Environment = titleDC.TitleDataSet.Tables[0].Rows[0]["Environment"].ToString();
                        if (titleDC.TitleDataSet.Tables[0].Rows[0]["IsFileList"] == null)
                            titleDC.IsFileList = 0;
                        else
                            titleDC.IsFileList = (int)titleDC.TitleDataSet.Tables[0].Rows[0]["IsFileList"];

                        titleDC.TargetEntityId = titleDC.TitleDataSet.Tables[0].Rows[0]["TargetEntityId"].ToString();
                        titleDC.TargetPath = titleDC.TitleDataSet.Tables[0].Rows[0]["TargetPath"].ToString();
                        titleDC.SourcePath = titleDC.TitleDataSet.Tables[0].Rows[0]["SourcePath"].ToString();
                        titleDC.TitleID = (int)titleDC.TitleDataSet.Tables[0].Rows[0]["TitleID"];
                        UploadResources(sm, titleDC, Async, ThreadLimit);
                    }
                    else
                    {
                        SaveDisplayMessage("Nothing to do...taking a break for 30 mins. Break start time - " + DateTime.Now.ToString());
                        Thread.Sleep(new TimeSpan(0, 30, 0));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogException.Publish(ex);

            }
        }

        private static void Stop()
        {
           
        }

        private static void SaveDisplayMessage(string Message)
        {
            if (Environment.UserInteractive)
                Console.WriteLine(Message);
        }
    }
}
