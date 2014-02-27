using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;

using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.IO;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common;
using System.Web.Configuration;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Handles downloading of files.
    /// </summary>
    [PerfTraceFilter]
    public class DownloadController : Controller
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// The IDocumentConverter implementation to use.
        /// </summary>
        protected IDocumentConverter DocumentConverter { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadController"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="ca">The ca.</param>
        /// <param name="ga">The ga.</param>
        public DownloadController(IBusinessContext ctx, IContentActions ca, IGradeActions ga, IDocumentConverter docConverter, IEnrollmentActions enrollmentActions)
        {
            Context = ctx;
            ContentActions = ca;
            GradeActions = ga;
            DocumentConverter = docConverter;
            EnrollmentActions = enrollmentActions;
        }

        /// <summary>
        /// Loads the content item with the given Id along with its resource.
        /// The resource is streamed to the client
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public FileStreamResult Document(string id, string name, string docId, string applicableEnrollmentId)
        {
            FileStreamResult result = null;

            if (!string.IsNullOrEmpty(id))
            {
                var biz = ContentActions.GetContent(Context.EntityId, id, true);

                if (biz == null || biz.Resources == null || biz.Resources.FirstOrDefault().GetStream().Length == 0)
                {                    
                    if (Context.AccessLevel == AccessLevel.Student)
                    {
                        var instructorEnrollmentId = EnrollmentActions.GetEntityEnrollmentsAsAdmin(Context.EntityId).Where(i => !i.Flags.Contains("Participate") && !i.Flags.Contains("DeleteCourse")).Min(i => i.Id);
                        biz = ContentActions.GetContent(instructorEnrollmentId, id, true);
                    }
                    else
                    {
                         biz = ContentActions.GetContent(Context.EnrollmentId, id, true);
                    }
                }

                if (biz != null)
                {
                    var model = biz.ToDocument(ContentActions);

                    var fileName = model.FileName;
                    if (string.IsNullOrEmpty(model.ContentType))
                    {
                        model.ContentType = "text/html";
                        fileName = String.Format("{0}.html", biz.Title);
                    }

                    result = File(biz.Resources.FirstOrDefault().GetStream(), model.ContentType, fileName);
                }
                else if(Context.AccessLevel  == AccessLevel.Student)
                {
                    var resource = ContentActions.GetResource(Context.EnrollmentId, string.Format("Assets/{0}", name));
                    if (resource.ContentType == null && resource.GetStream().Length == 0 )
                    {
                        resource = ContentActions.GetResource(Context.EnrollmentId, string.Format("Templates/Data/{0}/index.html", docId));
                    }
                    result = File(resource.GetStream(), resource.ContentType, name);
                }
                else if (Context.AccessLevel == AccessLevel.Instructor && !string.IsNullOrEmpty(applicableEnrollmentId))
                {
                    var resource = ContentActions.GetResource(applicableEnrollmentId, string.Format("Templates/Data/{0}/index.html", docId));
                    result = File(resource.GetStream(), resource.ContentType, name);
                }

            }           
            return result;
        }


        /// <summary>
        /// Loads the content item with the given Id along with its resource.
        /// The resource is streamed to the client
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns></returns>
        public FileStreamResult DropboxDocument(string id,string name)
        {
            FileStreamResult result = null;
            if (!string.IsNullOrEmpty(name))
            {
                if (Context.AccessLevel == AccessLevel.Student)
                {
                    string format = name.Substring(name.IndexOf("."));
                    var outputType = GetDocumentOutputFormat(format);
                    string filename = string.Empty;
                    var fStream = GradeActions.GetDropboxSubmissionsStream(Context.EntityId, Context.EnrollmentId, id, outputType);
                    result = File(fStream, outputType.ToString(), name);
                }
            }
            return result;
        }

        /// <summary>
        /// Loads the content item with the given Id along with its resource.
        /// The resource is streamed to the client
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns></returns>
        public FileStreamResult DropboxTeacherDocument(string id, string name)
        {
            FileStreamResult result = null;
            var outputType = GetDocumentOutputFormat(name.Substring(name.IndexOf(".")));
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {
                if (Context.AccessLevel == AccessLevel.Student)
                {
                    var resource = GradeActions.GetTeacherResponse(Context.EnrollmentId, id);
                    result = File(resource.ResourceStream, outputType.ToString(), name);
                }
            }


            return result;
        }


        
        /// <summary>
        /// Downloads any type of file based on the agilix resource path provided.
        /// The resource is streamed to the client.
        /// </summary>
        /// <param name="fileName">name of the file to be download</param>
        /// <returns></returns>
        public FileStreamResult DownloadFile(string resourcePath)
        {
            return DownloadEnrollmentFile(resourcePath, Context.CourseId);
        }

        /// <summary>
        /// Downloads any type of file based on the agilix resource path provided.
        /// The resource is streamed to the client.
        /// </summary>
        /// <param name="fileName">name of the file to be download</param>
        /// <param name="entityId">entity id different from courseid (for example enrollment id)</param>
        /// <returns></returns>
        public FileStreamResult DownloadEnrollmentFile(string resourcePath, string entityId)
        {
            FileStreamResult result = null;
            var resource = ContentActions.GetResource(entityId, resourcePath);

            if (String.IsNullOrEmpty(resource.ContentType))
            {
                resource.ContentType = string.Format("application/{0}", resource.Extension);
            }

            result = File(resource.GetStream(), resource.ContentType, resourcePath);

            return result;
        }


        /// <summary>
        /// Banner Image 
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult FolderBannerImage(string id)
        { 
            string body = string.Empty;
            string applicableEnrollmentId = Request.Params["applicableEnrollmentId"];
            if (string.IsNullOrEmpty(applicableEnrollmentId))
            {
                applicableEnrollmentId = Context.EnrollmentId;
            }
            var resource = ContentActions.GetResource(applicableEnrollmentId, string.Format("Templates/Data/XmlResources/Documents/BannerImage/{0}", id));
            var desc = resource.GetStream();
            FileStreamResult fsResult = new FileStreamResult(desc, "image/png");
            fsResult.FileDownloadName = "image.png";
            return fsResult;
        }

        /// <summary>
        /// HTMLs the document.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult HtmlDocument(string id, string applicableEnrollmentId)
        {
            var content = new HtmlDocument();           
            
            var biz = ContentActions.GetContent(Context.EntityId, id, true);

            if (biz != null && !biz.Properties.ContainsKey("DocId"))
            {
                var model = biz.ToHtmlDocument();

                var contentText = model.Type == "HtmlDocument" ? "content" : "directions";

                string addSomeContent = String.Format("<i>You have not provided any {0}. Click \"Edit\" and choose \"Basic Info\" to add {0}.</i><br>", contentText);
                model.Body = string.IsNullOrEmpty(model.Body) ? "<div>" + addSomeContent + "</div>" : model.Body;
                content.Body = model.Body;
            }
            else
            {   
                if (string.IsNullOrEmpty(applicableEnrollmentId))
                {
                    applicableEnrollmentId = Context.EnrollmentId;
                }
               
                biz = ContentActions.GetContent(Context.EntityId,id,true, applicableEnrollmentId);
                if (biz != null)
                {
                    string body = string.Empty;
                    var resource = ContentActions.GetResource(applicableEnrollmentId, string.Format("Templates/Data/{0}/index.html", biz.Properties["DocId"].Value));
                    var desc = resource.GetStream();
                    using (var sw = new System.IO.StreamReader(desc))
                    {
                        body = sw.ReadToEnd();
                    }
                    content.Body = body;
                }
            }

            return View("HtmlContent", content);
        }

        /// <summary>
        /// This method can be used to download the documents from resource.
        /// </summary>
        /// <param name="id">Id of the item in which resources associated to</param>
        /// <param name="zipFileName">Zip file name if its more than one document</param>
        /// <param name="format">Format for the download (doc, docx, ppt)</param>
        /// <param name="documentIds">The document ids.</param>
        /// <returns></returns>
        public FileStreamResult ResourceDocuments(string id, string zipFileName, string format, string documentIds, string downloadtype)
        {
            var docIds = GetCollectionFromDelimitString(id);
            var enrollmentId = downloadtype.Equals("CM", StringComparison.CurrentCultureIgnoreCase) ? Context.EntityId : Context.EnrollmentId;
            var outputType = GetDocumentOutputFormat(format);
            var fileName = "";
            Stream downloadStream=null;

            
            if(string.IsNullOrEmpty(downloadtype))
            {
                downloadStream = GradeActions.GetDocumentsStreamFromResource(docIds, enrollmentId, outputType, out fileName);
            }
            else 
            {
                var content = ContentActions.GetContent(Context.EntityId, docIds[0]);
                var resources = ContentActions.ListResources(content.Href, Context.EntityId);
                //If can't find the resource using entityid, try to use current enrollmentid
                if(resources.Count() == 0)
                    resources = ContentActions.ListResources(content.Href, Context.EnrollmentId);
                //Try to use instructor enrollmentId
                if (resources.Count() == 0)
                {
                    var instructorEnrollmentId = EnrollmentActions.GetEntityEnrollmentsAsAdmin(Context.EntityId).Where(i => !i.Flags.Contains("Participate") && !i.Flags.Contains("DeleteCourse")).Min(i => i.Id);
                    resources = ContentActions.ListResources(content.Href, instructorEnrollmentId);
                }
                
                var docConversions = resources.Select(resource => new DocumentConversion
                {
                    DataStream = UpdateStreamImageSources(resource),
                    FileName = resource.Name,
                    OutputType = outputType
                }).ToList();

                fileName = content.Title;

                downloadStream = DocumentConverter.ConvertDocuments(docConversions);
            }

            downloadStream.Seek(0, 0);

            if (docIds.Count > 1)
            {
                fileName = string.IsNullOrEmpty(zipFileName) ? Guid.NewGuid().ToString("N") : zipFileName;
                format = "zip";
                outputType = DocumentOutputType.Zip;
            }
            return File(downloadStream, outputType.GetDescription(), fileName + "." + format);
        }

        /// <summary>
        /// This method can be used to download the student submissions.
        /// </summary>
        /// <param name="id">Id of the item to which submission made by student</param>
        /// <param name="zipFileName">Zip file name if its more than one submissions</param>
        /// <param name="format">Format for the download (doc, docx, ppt)</param>
        /// <param name="documentIds">The document ids.</param>
        /// <returns></returns>
        public FileStreamResult Submissions(string id, string zipFileName, string format, string documentIds)
        {
            if (!string.IsNullOrEmpty(zipFileName))
            {
                zipFileName = zipFileName.Replace(" ", "_");
            }

            var enrollmentIdColl = GetCollectionFromDelimitString(documentIds);
            var outputType = GetDocumentOutputFormat(format);
            var fileName = "";
            var downloadStream = GradeActions.GetSubmissionsStream(Context.EntityId,enrollmentIdColl,id,outputType,out fileName);

            downloadStream.Seek(0, 0);

            if (enrollmentIdColl.Count > 1) {
                fileName = string.IsNullOrEmpty(zipFileName) ? Guid.NewGuid().ToString("N") : zipFileName;
                format = "zip";
                outputType = DocumentOutputType.Zip;
            }

            return File(downloadStream, outputType.GetDescription(), fileName + "." + format);
        }

        /// <summary>
        /// Gets the document output format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private DocumentOutputType GetDocumentOutputFormat(string format)
        {
            if (format.StartsWith("."))
            {
                format = format.Substring(1, format.Length - 1);
            }

            DocumentOutputType outputType;
            switch (format)
            {
                case "doc":
                    outputType = DocumentOutputType.Doc;
                    break;
                case "docx":
                    outputType = DocumentOutputType.Docx;
                    break;
                case "pdf":
                    outputType = DocumentOutputType.Pdf;
                    break;
                case "jpeg":
                case "jpg":
                case "gif":
                case "tiff":
                case "bmp":
                case "png":
                    outputType = DocumentOutputType.Image;
                    break;
                default:
                    outputType = DocumentOutputType.Doc;
                    break;
            }

            return outputType;
        }

        /// <summary>
        /// Gets the collection from delimit string.
        /// </summary>
        /// <param name="Ids">The ids.</param>
        /// <returns></returns>
        private List<string> GetCollectionFromDelimitString(string Ids) {

            if (Ids.StartsWith(","))
            {
                Ids = Ids.Substring(1, Ids.Length - 1);
            }

            return Ids.Split(',').Distinct().ToList();
        }

        private Stream UpdateStreamImageSources(Resource resource)
        {
            MemoryStream stream = null;
            using (StreamReader sr = new StreamReader(resource.GetStream(), Encoding.UTF8))
            {
                var c = sr.ReadToEnd();
                if (!c.Contains("<html>") && !c.Contains("</html>"))
                    c = "<html>" + c + "</html>";
                if (c.Contains("<img "))
                {
                    var brainhoneyUrl = Context.BrainHoneyUrl;
                    var rootDomain = brainhoneyUrl.Substring(0, brainhoneyUrl.IndexOf(".com") + 4);
                    var index = 0;
                    var startIndex = 0;
                    while (index != -1)
                    {
                        index = c.IndexOf("src=\"", startIndex);

                        if (index != -1)
                        {
                            //Do not modify data image. Data image doesn't need a URL
                            bool isDataImage = c.Substring(index, 30).Contains("data:image");
                            if (isDataImage)
                            {
                                startIndex = index + 30;
                                continue;
                            }
                            c = c.Insert(index + 5, rootDomain);
                            startIndex = index + 1;
                        }
                    }
                }
                stream = new MemoryStream(Encoding.UTF8.GetBytes(c));
                stream.Seek(0, 0);
            }
            return stream;
        }
    }
}
