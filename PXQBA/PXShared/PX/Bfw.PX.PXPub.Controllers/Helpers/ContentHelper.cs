using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
using EPortfolio = Bfw.PX.PXPub.Models.EPortfolio;
using NavigationItem = Bfw.PX.PXPub.Models.NavigationItem;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public class ContentHelper : IContentHelper
    {
        private const string RELATED_RESOURCE_PREFIX = "RELATED_CONTENT_";


        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected IEnrollmentActions EnrollmentActions
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEnrollmentActions>();
            }
        }


        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        /// <value>
        /// The assignment actions.
        /// </value>
        protected IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Access to an IGradeActions implementation.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Access to an INoteActions implementation.
        /// </summary>
        /// <value>
        /// The Note Actions.
        /// </value>
        protected INoteActions NoteActions { get; set; }

        /// <summary>
        /// Access to an IBookmarkActions implementation.
        /// </summary>
        /// <value>
        /// The bookmark actions.
        /// </value>
        protected IBookmarkActions BookmarkActions { get; set; }

        /// <summary>
        /// Access to an IEPortfolioActions implementation.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected IEPortfolioActions EPortfolioActions { get; set; }

        /// <summary>
        /// Access to an IResourceMapActions implementation.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected IResourceMapActions ResourceMapActions { get; set; }

        /// <summary>
        /// Access to an IUserActivitiesActions implementation.
        /// </summary>
        /// <value>
        /// The user activities actions.
        /// </value>
        protected IUserActivitiesActions UserActivitiesActions { get; set; }

        /// <summary>
        /// Gets or sets the next items.
        /// </summary>
        /// <value>
        /// The next items.
        /// </value>
        private List<ContentItem> NextItems { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has access level as instructor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has access level as instructor; otherwise, <c>false</c>.
        /// </value>
        public bool HasAccessLevelAsInstructor
        {
            get
            {
                return Context.AccessLevel == AccessLevel.Instructor;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has access level as student.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has access level as student; otherwise, <c>false</c>.
        /// </value>
        public bool HasAccessLevelAsStudent
        {
            get
            {
                return Context.AccessLevel == AccessLevel.Student;
            }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ContentHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navActions">The nav actions.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="assignmentActions">The assignment actions.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="bookmarkActions">The bookmark actions.</param>
        /// <param name="resourceMapActions">The resource map actions.</param>
        public ContentHelper(IBusinessContext context, INavigationActions navActions, IContentActions contActions,
            IAssignmentActions assignmentActions, IGradeActions gradeActions, IBookmarkActions bookmarkActions,
            IResourceMapActions resourceMapActions, IEPortfolioActions ePortfolioActions, INoteActions noteActions,
            IUserActivitiesActions userActivitiesActions)
        {
            NavigationActions = navActions;
            ContentActions = contActions;
            AssignmentActions = assignmentActions;
            GradeActions = gradeActions;
            BookmarkActions = bookmarkActions;
            Context = context;
            ResourceMapActions = resourceMapActions;
            EPortfolioActions = ePortfolioActions;
            NoteActions = noteActions;
            UserActivitiesActions = userActivitiesActions;
        }

        /// <summary>
        /// Stores the link collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreLinkCollection(LinkCollection collection, string title, string url, string courseId)
        {
            BizDC.ContentItem ci;

            if (collection.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, collection.Id);
                ci.UpdateContentItemWithEdits(collection.ToContentItem(courseId), this.ContentActions);
            }
            else
            {
                if (string.IsNullOrEmpty(collection.Id))
                {
                    collection.Id = Guid.NewGuid().ToString("N");
                }
                ci = collection.ToContentItem(courseId);
                SetDefaultParent(collection, ci);
            }
            ContentActions.StoreContent(ci);

            if (collection.Links != null)
            {
                foreach (var lnk in collection.Links)
                    StoreLink(collection.Id, lnk.Title, lnk.Url, courseId);
            }
            if (!title.IsNullOrEmpty() && !url.IsNullOrEmpty())
            {
                StoreLink(collection.Id, title, url, courseId);
            }
        }

        /// <summary>
        /// Stores the link.
        /// </summary>
        /// <param name="lnk">The LNK.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreLink(Link lnk, string courseId)
        {
            BizDC.ContentItem ci;

            if (lnk.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, lnk.Id);
                ci.UpdateContentItemWithEdits(lnk.ToContentItem(courseId), this.ContentActions);
                ci.Href = lnk.Url;
            }
            else
            {
                if (string.IsNullOrEmpty(lnk.Id))
                {
                    lnk.Id = Guid.NewGuid().ToString("N");
                }
                ci = lnk.ToContentItem(courseId);
                SetDefaultParent(lnk, ci);
            }
            ci.Href = lnk.Url;
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the link.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="link">The link.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreLink(string collectionId, string link, string url, string courseId)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var d = new Link();
                d.Id = Guid.NewGuid().ToString("N");
                d.ParentId = collectionId;
                d.Title = link;
                d.Url = url;

                var dItem = d.ToContentItem(courseId);
                ContentActions.StoreContent(dItem);
            }
        }

        /// <summary>
        /// Stores the widget configuration.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreWidgetConfiguration(WidgetConfiguration model, string courseId)
        {
            // Create and save the Folder
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var ci = model.ToContentItem(courseId);

            SetDefaultParent(model, ci);
            ContentActions.StoreContent(ci);
        }


        /// <summary>
        /// Stores Media Content Assignment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreMediaContentAssignment(MediaContentAssignment model, string courseId)
        {
            // Create and save the Folder
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var ci = model.ToContentItem(courseId);

            SetDefaultParent(model, ci);
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores Media Content.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreMediaContent(MediaContent model, string mediaContentAssignmentId)
        {

            // Create and save the Folder
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var ci = model.ToContentItem(Context.EntityId);

            SetDefaultParent(model, ci);
            ContentActions.StoreContent(ci);

            //var mediaContentAssignment = ContentActions.GetContent (Context.EntityId, mediaContentAssignmentId).ToMediaContentAssignment(ContentActions);
            ////mediaContentAssignment.AddMediaContent (model );
            //StoreMediaContentAssignment(mediaContentAssignment, Context.EntityId);
        }

        /// <summary>
        /// Stores CompetencyRubric
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreCompetencyRubric(CompetencyRubric model)
        {
            // Create and save the Folder
            if (string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            var ci = model.ToContentItem();

            SetDefaultParent(model, ci);
            ContentActions.StoreContent(ci);
        }



        /// <summary>
        /// Saves a document collection along with any initial file posted.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="docTitle">The doc title.</param>
        /// <param name="docFile">The doc file.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreDocumentCollection(DocumentCollection doc, string docTitle, System.Web.HttpPostedFileBase docFile, string courseId)
        {
            BizDC.ContentItem ci;
            if (doc.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, doc.Id);
                ci.UpdateContentItemWithEdits(doc.ToContentItem(courseId), this.ContentActions);
            }
            else
            {
                if (string.IsNullOrEmpty(doc.Id))
                {
                    doc.Id = Guid.NewGuid().ToString("N");
                }
                ci = doc.ToContentItem(courseId);
                SetDefaultParent(doc, ci);

                var documentCreatedDate = new BizDC.MetadataValue() { Value = Context.Course.UtcRelativeAdjust(DateTime.Now) };
                ci.Metadata.Add("DOCUMENT_CREATED_DATE", documentCreatedDate);

                BizDC.PropertyValue propDate = new BizDC.PropertyValue();
                propDate.Value = Context.Course.UtcRelativeAdjust(DateTime.Now).ToString();
                ci.Properties.Add("CreationDate", propDate);

                if (docFile != null)
                {
                    BizDC.PropertyValue propName = new BizDC.PropertyValue();
                    propName.Value = docFile.FileName;
                    ci.Properties.Add("FileName", propName);
                    BizDC.PropertyValue propSize = new BizDC.PropertyValue();
                    propSize.Value = docFile.ContentLength;
                    ci.Properties.Add("FileSize", propSize);
                }
                BizDC.PropertyValue propDocId = new BizDC.PropertyValue();
                propDocId.Value = doc.Id;
                ci.Properties.Add("DocId", propDocId);
            }
            ContentActions.StoreContent(ci, courseId);
            if (docFile != null)
            {
                StoreDocument(doc.Id, docTitle, docFile, courseId);
            }
        }

        public string StoreDocumentCollection(DocumentCollection doc, string docTitle, System.Web.HttpPostedFileBase docFile, string courseId, bool hasDisplay)
        {
            string parentId = "";

            var firstOrDefault = doc.Documents.FirstOrDefault();
            if (firstOrDefault != null)
            {
                parentId = firstOrDefault.ParentId;
            }
            try
            {
                var resourceId = GradeActions.UploadDocument(Context.EnrollmentId,
                                                             parentId ?? "",
                                                             docTitle,
                                                             docFile.InputStream,
                                                             BizDC.DocumentOutputType.Html,
                                                             ResourceMapActions);
                StoreDocumentCollection(doc, docTitle, docFile, courseId);
                return resourceId;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public DocumentCollection ToDocumentCollection(Upload uploadModel)
        {
            List<Document> docList = new List<Document>();
            Document doc = new Document
                               {
                                   ContentType = uploadModel.UploadFile.ContentType,
                                   FileName = uploadModel.UploadTitle,
                                   Size = uploadModel.UploadFile.ContentLength,
                                   Title = uploadModel.UploadTitle,
                                   Uploaded = Context.Course.UtcRelativeAdjust(DateTime.Now),
                                   ParentId = uploadModel.ParentId

                               };
            docList.Add(doc);
            DocumentCollection docCollection = new DocumentCollection { Documents = docList };
            return docCollection;
        }
        /// <summary>
        /// Stores the module.
        /// </summary>
        /// <param name="pxunit">The module.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreModule(PxUnit module, string courseId)
        {
            BizDC.ContentItem ci;
            if (module.IsBeingEdited)
            {
                var mod = ContentActions.GetContent(courseId, module.Id).ToPxUnit(ContentActions);
                mod.Title = module.Title;
                mod.Description = module.Description;
                mod.Thumbnail = module.Thumbnail;

                if (string.IsNullOrEmpty(mod.Thumbnail))
                {
                    var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
                    mod.Thumbnail = urlHelper.Action("Index", "Style", new { path = ConfigurationManager.AppSettings["DefaultImage"] });
                }

                ci = mod.ToContentItem(courseId);
            }
            else
            {
                if (string.IsNullOrEmpty(module.Id))
                {
                    module.Id = Guid.NewGuid().ToString("N");
                }
                if (module.Id.Contains("MODULE_") == false)
                {
                    module.Id = "MODULE_" + module.Id;
                }
                if (string.IsNullOrEmpty(module.Thumbnail))
                {
                    var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
                    module.Thumbnail = urlHelper.Action("Index", "Style", new { path = ConfigurationManager.AppSettings["DefaultImage"] });
                }
                ci = module.ToContentItem(courseId);
                if (ci.DefaultCategoryParentId.IsNullOrEmpty())
                {
                    ci.DefaultCategoryParentId = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
                }
                else
                {
                    ci.DefaultCategoryParentId = module.DefaultCategoryParentId;
                }

                ci.AssignmentSettings.Category = module.GradeBookWeightCategoryId;
                SetDefaultParent(module, ci);
            }

            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the multi part lesson.
        /// </summary>
        /// <param name="pxunit">The module.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreMultiPartLesson(MultiPartLesson module, string courseId)
        {
            BizDC.ContentItem ci;
            if (module.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, module.Id);
                ci.Title = module.Title;
                ci.Description = module.Description;
            }
            else
            {
                if (string.IsNullOrEmpty(module.Id))
                {
                    module.Id = Guid.NewGuid().ToString("N");
                }
                if (module.Id.Contains("MODULE_") == false && module.Type != "MultiPartLesson")
                {
                    module.Id = "MODULE_" + module.Id;
                }
                if (string.IsNullOrEmpty(module.Thumbnail))
                {
                    var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
                    module.Thumbnail = urlHelper.Action("Index", "Style", new { path = ConfigurationManager.AppSettings["DefaultImage"] });
                }
                ci = module.ToContentItem(courseId);
                SetDefaultParent(module, ci);
            }

            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Saves a new document to an existing document collection.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="docTitle">The doc title.</param>
        /// <param name="docFile">The doc file.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public string StoreDocument(string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, string courseId)
        {
            var docId = Guid.NewGuid().ToString("N");
            StoreDocument(collectionId, docTitle, docFile, courseId, docId);
            return docId;
        }

        /// <summary>
        /// Saves a new document to an existing document collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="docTitle"></param>
        /// <param name="docFile"></param>
        /// <param name="courseId"></param>
        /// <param name="id"></param>
        public void StoreDocument(string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, string courseId, string id)
        {
            if (!string.IsNullOrEmpty(docTitle) && null != docFile)
            {
                var d = new Models.Document
                        {
                            Id = id ?? Guid.NewGuid().ToString("N"),
                            ParentId = collectionId,
                            Title = docTitle
                        };

                var dItem = d.ToContentItem(courseId, docFile);
                if (Context.Course.CourseType != CourseType.PersonalEportfolioPresentation.ToString())
                {
                    ContentActions.StoreContent(dItem);
                }
                else
                {
                    var eportfolioCourseActions = ServiceLocator.Current.GetInstance<IEportfolioCourseActions>();
                    var dashboardCourseId = eportfolioCourseActions.GetPersonalEportfolio(Context.CurrentUser.ReferenceId, Context.Course.Domain.Id);
                    ContentActions.StoreContents(new List<Bfw.PX.Biz.DataContracts.ContentItem> { dItem }, dashboardCourseId);
                }
            }
        }

        public void CopyDocumentResource(string collectionId, Document document, string courseId)
        {
            var resource = ContentActions.GetContent(courseId, document.Id, true).Resources.FirstOrDefault();

            document.Id = Guid.NewGuid().ToString("N");
            document.ParentId = collectionId;

            Stream inputStream = null;
            if (resource != null)
            {
                inputStream = resource.GetStream();
            }

            var dItem = document.ToContentItem(courseId, inputStream, document.FileName, document.Size, document.ContentType);
            ContentActions.StoreContent(dItem);
        }

        /// <summary>
        /// Stores the assignment.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreAssignment(Assignment a, string courseId)
        {
            BizDC.ContentItem ci;
            if (a.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, a.Id);
                ci.Title = a.Title;
                ci.SubTitle = a.SubTitle;
                ci.Description = a.Description;
            }
            else
            {
                if (string.IsNullOrEmpty(a.Id))
                {
                    a.Id = Guid.NewGuid().ToString("N");
                }

                ci = a.ToContentItem(courseId);

                SetDefaultParent(a, ci);
            }

            if (ci.CreatedDate.ToShortDateString() == DateTime.MinValue.ToShortDateString())
            {
                ci.CreatedDate = Context.Course.UtcRelativeAdjust(DateTime.Now);
            }

            if (ci.AvailableDate.ToShortDateString() == DateTime.MinValue.ToShortDateString())
            {
                ci.AvailableDate = Context.Course.UtcRelativeAdjust(DateTime.Now);
            }

            ContentActions.StoreContent(ci);

            //Store the item as instructor eportfolio item
            //EPortfolioActions.StoreEPortfolioItem(ci);
        }

        /// <summary>
        /// Saves a new document to an existing assignment document collection
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="collectionId"></param>
        /// <param name="docTitle"></param>
        /// <param name="docFile"></param>
        /// <param name="courseId"></param>
        public void StoreItemDocument(string assignmentId, string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, string courseId)
        {
            var contentItems = new List<BizDC.ContentItem>();

            if (string.IsNullOrEmpty(docTitle) || null == docFile)
            {
                return;
            }

            if (string.IsNullOrEmpty(collectionId))
            {
                var docsCollection = new DocumentCollection
                {
                    Title = "Document Collection",
                    Id = assignmentId + "_DOCS",
                    ParentId = assignmentId
                };

                var docsCi = docsCollection.ToContentItem(courseId);
                docsCi.Hidden = true;
                contentItems.Add(docsCi);
                collectionId = docsCollection.Id;
            }

            var document = new Models.Document { Id = Guid.NewGuid().ToString("N"), ParentId = collectionId, Title = docTitle };
            var documentItem = document.ToContentItem(courseId, docFile);
            documentItem.Hidden = true;
            contentItems.Add(documentItem);
            ContentActions.StoreContents(contentItems);
        }

        /// <summary>
        /// Stores the assignment link.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="link">The link.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreAssignmentLink(string assignmentId, string collectionId, string link, string url, string courseId)
        {
            var contentItems = new List<BizDC.ContentItem>();

            if (!string.IsNullOrEmpty(url))
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    var linkCollection = new LinkCollection
                    {
                        Title = "Link Collection",
                        Id = assignmentId + "_LINKS",
                        ParentId = assignmentId
                    };

                    var linkCollCi = linkCollection.ToContentItem(courseId);
                    linkCollCi.Hidden = true;
                    contentItems.Add(linkCollCi);
                    collectionId = linkCollection.Id;
                }
            }

            var d = new Link { Id = Guid.NewGuid().ToString("N"), ParentId = collectionId, Title = link, Url = url };
            var dItem = d.ToContentItem(courseId);
            contentItems.Add(dItem);
            ContentActions.StoreContents(contentItems);
        }

        /// <summary>
        /// Stores the folder.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreFolder(Folder f, string courseId)
        {
            BizDC.ContentItem ci;
            if (f.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, f.Id);
                ci.Title = f.Title;
                ci.Description = f.Description;
            }
            else
            {
                if (string.IsNullOrEmpty(f.Id))
                {
                    f.Id = Guid.NewGuid().ToString("N");
                }
                ci = f.ToContentItem(courseId);
                SetDefaultParent(f, ci);
            }
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the custom widget.
        /// </summary>
        /// <param name="cw">The Custom Widget.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreCustomWidget(CustomWidget cw, string courseId)
        {
            BizDC.ContentItem ci;
            if (cw.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, cw.Id);
                ci.Title = cw.Title;
                ci.Description = cw.Description;
                var rez = new BizDC.Resource
                {
                    ContentType = "text/html",
                    Extension = "html",
                    Status = Biz.DataContracts.ResourceStatus.Normal,
                    Url = ci.Href,
                    EntityId = courseId
                };

                var sw = new System.IO.StreamWriter(rez.GetStream());
                sw.Write(System.Web.HttpUtility.HtmlDecode(cw.WidgetContents));
                sw.Flush();

                ci.Resources = new List<BizDC.Resource> { rez };
            }
            else
            {
                BizDC.ContentItem ciTemp = ContentActions.GetContent(courseId, cw.Id);
                if (string.IsNullOrEmpty(cw.Id))
                {
                    cw.Id = Guid.NewGuid().ToString("N");
                }
                ci = cw.ToContentItem(courseId);
                ci.Properties = ciTemp.Properties;
                SetDefaultParent(cw, ci);
            }
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the navigation item.
        /// </summary>
        /// <param name="h">The Html document.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreHtmlDocument(HtmlDocument h, string courseId)
        {
            BizDC.ContentItem ci;
            if (h.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, h.Id);

                //As far as I can tell, we never get into this method without IsBeingEdited set to true.  Therefore we need to set these properties
                //on the content item (else new documents created off a template (all of them) will all use the same resource). 
                ci.Type = "Resource";
                ci.Subtype = "HtmlDocument";
                ci.Href = string.Format("Templates/Data/{0}/index.html", h.Id);


                ci.Title = h.Title;
                ci.SubTitle = h.SubTitle;
                ci.Description = h.Description;
                var rez = new BizDC.Resource
                {
                    ContentType = "text/html",
                    Extension = "html",
                    Status = Biz.DataContracts.ResourceStatus.Normal,
                    Url = ci.Href,
                    EntityId = courseId
                };
                //In launchpad, and lms we store html documents under the entitiy id so that students can see them
                //eportfolio has student upload their own documents

                if (IsEportfolioCourse())
                {
                    var cat = h.Categories.Filter(c => c.ItemParentId != null && c.Id == "bfw_toc_contents");
                    if (cat != null && cat.Count() > 0)
                    {
                        rez.EntityId = Context.EnrollmentId;
                    }
                }

                var sw = new System.IO.StreamWriter(rez.GetStream());
                sw.Write(System.Web.HttpUtility.HtmlDecode(h.Body));
                sw.Flush();

                ci.Resources = new List<BizDC.Resource> { rez };
            }
                //TODO: Remove this if it never gets called.  I dont think it ever does
            else
            {
                if (string.IsNullOrEmpty(h.Id))
                {
                    h.Id = Guid.NewGuid().ToString("N");
                }
                ci = h.ToContentItem(courseId);
                if (h.ExtendedProperties.Count > 0)
                {
                    ci.Metadata = ExtendedPropertiesToMetaData(h.ExtendedProperties);
                    //ci.Metadata.Add(h.ExtendedProperties.Keys[0]);
                }
                SetDefaultParent(h, ci);
            }
            
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Checks to see if the passed in course is an eportfolio course
        /// </summary>
        /// <param name="course">course object</param>
        /// <returns>true if the course type is an eportfolio type, false other wise</returns>
        public bool IsEportfolioCourse()
        {
            string currentCourseType = Context.Course.CourseType;
            return
                currentCourseType == CourseType.Eportfolio.ToString() ||
                currentCourseType == CourseType.PersonalEportfolioPresentation.ToString() ||
                currentCourseType == CourseType.PersonalEportfolioDashboard.ToString() ||
                currentCourseType == CourseType.EportfolioTemplate.ToString() ||
                currentCourseType == CourseType.PersonalEportfolioProductMaster.ToString() ||
                currentCourseType == CourseType.ProgramDashboard.ToString() ||
                currentCourseType == CourseType.EportfolioDashboard.ToString();
        }

        public Dictionary<string, BizDC.MetadataValue> ExtendedPropertiesToMetaData(Hashtable extendedProperty)
        {
            IDictionaryEnumerator hashtableEnumerator = extendedProperty.GetEnumerator();
            Dictionary<string, BizDC.MetadataValue> metadata = new Dictionary<string, BizDC.MetadataValue>();
            try
            {
                while (hashtableEnumerator.MoveNext())
                {
                    metadata[hashtableEnumerator.Key.ToString()] = new BizDC.MetadataValue()
                                                                       {
                                                                           Type = BizDC.PropertyType.String,
                                                                           Value = hashtableEnumerator.Value
                                                                       };
                    //metadata.Add( hashtableEnumerator.Key.ToString(), new BizDC.MetadataValue() { Type = PropertyType.String, Value = hashtableEnumerator.Value } );
                }
                return metadata;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Stores the rubric.
        /// </summary>
        /// <param name="r">The rubric model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreRubric(Rubric r, string courseId, string instructorDashboardId)
        {
            BizDC.ContentItem contentItem = null;

            if (r.IsBeingEdited)
            {
                // gets Rubric for editing
                contentItem = ContentActions.GetContent(courseId, r.Id);
            }

            bool actionSaveAs = Context.Course.ProductCourseId == courseId ||
                               (null != contentItem &&  // logged in as an instructor and editing a product manager rubric
                                  !Context.Course.CourseType.Equals(CourseType.ProgramDashboard.ToString()) &&
                                  CourseType.ProgramDashboard.ToString().Equals(contentItem.RubricCourseType)) ||
                               (null != contentItem &&  // publisher rubrics should always be a save as
                                  String.IsNullOrEmpty(contentItem.RubricCourseType));

            // when a user wanted to edit a Rubric that is out side of their scope
            // that Rubric performs a Save As functionality to the users scope
            if (actionSaveAs)
            {
                // priming values to be set later on in method
                r.Id = null;
                contentItem.RubricOwner = null;
                contentItem.RubricCourseType = null;
                contentItem = null;
            }

            // generate an ID for any Rubric that came in with out one
            if (string.IsNullOrEmpty(r.Id))
                r.Id = Guid.NewGuid().ToString("N");

            r.RubricPath = string.Format("Templates/Data/{0}/_rubric.xml", r.Id);

            // create a new Content Item if one does not exist
            if (null == contentItem)
            {
                contentItem = r.ToContentItem(courseId);
                SetDefaultParent(r, contentItem);
                if (String.IsNullOrEmpty(contentItem.ParentId))
                {
                    contentItem.ParentId = "PX_RUBRICS";
                }
            }

            // set the Rubric CourseType if its not the product course rubric or if it has already been set
            if (!Context.Course.ProductCourseId.Equals(courseId) &&
                String.IsNullOrEmpty(contentItem.RubricCourseType))
                contentItem.RubricCourseType = Context.Course.CourseType.ToString();

            // update elements
            contentItem.Title = r.Title;
            contentItem.Description = r.Description;

            // if the assignment settings was not already created, create it
            contentItem.AssignmentSettings = contentItem.AssignmentSettings ?? new BizDC.AssignmentSettings();
            contentItem.AssignmentSettings.Rubric = r.RubricPath;

            // set the Owner Id
            if (String.IsNullOrEmpty(contentItem.RubricOwner))
                contentItem.RubricOwner = Context.CurrentUser.Id;

            // if the rubric was created by an instrutor, make the rubric active by default
            if (!r.IsBeingEdited && !Context.Course.CourseType.Equals(CourseType.ProgramDashboard.ToString()))
                contentItem.isActiveRubric = true;

            // store learning objectives
            if (!r.Objectives.IsNullOrEmpty())
            {
                string[] lstLOGuids = r.Objectives.Split('|');
                List<BizDC.LearningObjective> loItems = new List<BizDC.LearningObjective>();

                if (!lstLOGuids.IsNullOrEmpty())
                {
                    foreach (string loGuid in lstLOGuids)
                    {
                        if (!loGuid.IsNullOrEmpty())
                        {
                            BizDC.LearningObjective loItem = new BizDC.LearningObjective();
                            loItem.Guid = loGuid;
                            loItems.Add(loItem);
                        }
                    }
                }
                contentItem.LearningObjectives = loItems;
            }

            // store rubric and associate it to current course
            StoreRubricData(r, courseId);
            ContentActions.StoreContent(contentItem);

            // if dashboard id passed, save rubric in dashboard course.
            if (!String.IsNullOrEmpty(instructorDashboardId))
            {
                StoreRubricData(r, instructorDashboardId);
                contentItem.CourseId = instructorDashboardId;
                contentItem.isActiveRubric = false;
                ContentActions.StoreContent(contentItem, instructorDashboardId);
            }
        }



        /// <summary>
        /// Stores the Rubric and its data
        /// </summary>
        /// <param name="r">The rubric model.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreRubricData(Rubric r, string courseId)
        {
            var resource = new BizDC.Resource()
            {
                EntityId = courseId,
                ContentType = "text/xml",
                Status = BizDC.ResourceStatus.Normal,
                Extension = "xml",
                Url = r.RubricPath
            };

            if (r.Rubric == null)
            {
                r.Rubric = new RubricData1();
            }

            var rubricData = PxXmlSerializer.Serialize(r.Rubric, "");
            var data = rubricData.ToString();
            var rstream = resource.GetStream();
            var sw = new StreamWriter(rstream);
            sw.Write(data);
            sw.Flush();

            ContentActions.StoreResources(new List<BizDC.Resource> { resource });
        }

        /// <summary>
        /// Stores the navigation item.
        /// </summary>
        /// <param name="navItem">The nav item.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreNavigationItem(NavigationItem navItem, string courseId)
        {
            // Create and save the Folder
            if (string.IsNullOrEmpty(navItem.Id))
            {
                navItem.Id = Guid.NewGuid().ToString("N");
            }

            var ci = navItem.ToContentItem(courseId);
            SetDefaultParent(navItem, ci);
            ContentActions.StoreContent(ci);

            foreach (var subItem in navItem.Items)
            {
                subItem.ParentId = navItem.Id;
                StoreNavigationItem(subItem, courseId);
            }
        }

        /// <summary>
        /// Stores the assignment center filter section.
        /// </summary>
        /// <param name="filterSection">The filter section.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreAssignmentCenterFilterSection(AssignmentCenterFilterSection filterSection, string courseId)
        {
            if (string.IsNullOrEmpty(filterSection.Id))
            {
                filterSection.Id = "PX_ASSIGNMENT_CENTER_SYLLABUS_" + Guid.NewGuid().ToString("N");
            }

            filterSection.TocId = GetTOCId();
            var ci = filterSection.ToContentItem(courseId);
            SetDefaultParent(filterSection, ci);
            ContentActions.StoreContent(ci);

            foreach (var subItem in filterSection.ChildrenFilterSections)
            {
                if (subItem.ParentId != "PX_DELETED")
                {
                    subItem.ParentId = filterSection.Id;
                }

                StoreAssignmentCenterFilterSection(subItem, courseId);
            }
        }

        /// <summary>
        /// Stores the quiz.
        /// </summary>
        /// <param name="q">The q.</param>
        public void StoreQuiz(Quiz q)
        {
            BizDC.ContentItem ci;
            if (q.IsBeingEdited)
            {
                ci = ContentActions.GetContent(Context.EntityId, q.Id);
                ci.Title = q.Title;
                ci.SubTitle = q.SubTitle;
                ci.Description = q.Description;


                if (!string.IsNullOrEmpty(ci.Subtype) && ci.Subtype.ToLowerInvariant() == "learningcurveactivity")
                {
                    var learningCurve = (LearningCurveActivity)q;
                    ci.SetPropertyValue("bfw_whoopsright", learningCurve.WhoopsRight);
                    ci.SetPropertyValue("bfw_whoopswrong", learningCurve.WhoopsWrong);
                    ci.SetPropertyValue("bfw_EbookReferenceDescription", learningCurve.EbookReferenceDescription);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(q.Id))
                {
                    q.Id = Guid.NewGuid().ToString("N");
                    ci = q.ToContentItem(Context.EntityId);
                }
                else
                {
                    ci = ContentActions.GetContent(Context.EntityId, q.Id);
                    ci.Title = q.Title;
                    ci.SubTitle = q.SubTitle;
                    ci.Description = q.Description;
                    ci.Visibility = "<bfw_visibility><instructor /><student /></bfw_visibility>";


                    if (!ci.Properties.Keys.Contains("bfw_syllabusfilter"))
                    {
                        ci.Properties.Add("bfw_syllabusfilter", new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = q.SyllabusFilter });
                    }

                    //fix for setting correct parentId in case of FacePlate or Xbook.
                    if (q.HostMode == HostMode.FacePlate || q.HostMode == HostMode.XBook)
                    {
                        ci.ParentId = q.ParentId;
                        ci.DefaultCategoryParentId = q.DefaultCategoryParentId;
                    }
                }

                SetDefaultParent(q, ci);
            }
            ContentActions.StoreContent(ci);
        }


        /// <summary>
        /// Stores the content.
        /// </summary>
        /// <param name="item">The item.</param>
        public void StoreContent(ContentItem item, BizDC.ContentItem biz)
        {
            string type = item.GetType().Name.ToLowerInvariant();

            bool isNew = string.IsNullOrEmpty(item.Id);

            if (type == "quiz")
            {
                StoreQuiz((Quiz)item);
            }
            else if (type == "folder")
            {
                StoreFolder((Folder)item, Context.EntityId);
            }
            else if (type == "pxunit")
            {
                StoreModule((PxUnit)item, Context.EntityId);
            }
            else if (type == "assignment")
            {
                StoreAssignment((Assignment)item, Context.EntityId);
            }
            else if (type == "htmldocument")
            {
                StoreHtmlDocument((HtmlDocument)item, Context.EntityId);
            }
            else if (type == "discussion")
            {
                StoreDiscussion((Discussion)item, Context.EntityId);
            }
            else if (type == "documentcollection")
            {
                StoreDocumentCollection((DocumentCollection)item, "", null, Context.EntityId);
            }
            else if (type == "rssfeed")
            {
                StoreRssFeed((RssFeed)item, Context.EntityId);
            }
            else if (type == "linkcollection")
            {
                StoreLinkCollection((LinkCollection)item, "", "", Context.EntityId);
            }
            else if (type == "wiki")
            {
                StoreWiki((Wiki)item, Context.EntityId);
            }
            else if (type == "navigationitem")
            {
                StoreNavigationItem((NavigationItem)item, Context.EntityId);
            }
            else if (type == "link")
            {
                StoreLink((Link)item, Context.EntityId);
            }
            else if (type == "widgetconfiguration")
            {
                StoreWidgetConfiguration((WidgetConfiguration)item, Context.EntityId);
            }
            else if (biz != null)
            {
                ContentActions.StoreContent(biz);
            }

            if (isNew)
            {
                SetVisibility(item.Visibility);
            }
        }

        /// <summary>
        /// Stores the discussion.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreDiscussion(Discussion d, string courseId)
        {
            BizDC.ContentItem ci;
            if (d.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, d.Id);
                ci.Title = d.Title;
                ci.SubTitle = d.SubTitle;
                ci.Description = d.Description;
            }
            else
            {
                if (string.IsNullOrEmpty(d.Id))
                {
                    d.Id = Guid.NewGuid().ToString("N");
                }
                ci = d.ToContentItem(courseId);
                SetDefaultParent(d, ci);
            }
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the wiki.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreWiki(Wiki w, string courseId)
        {
            BizDC.ContentItem ci;
            if (w.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, w.Id);
                ci.Title = w.Title;
                ci.Description = w.Description;
            }
            else
            {
                if (string.IsNullOrEmpty(w.Id))
                {
                    w.Id = Guid.NewGuid().ToString("N");
                }
                ci = w.ToContentItem(courseId);
                SetDefaultParent(w, ci);
            }
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Removes the RSS Feed.
        /// </summary>
        /// <param name="r">The RssFeed.</param>
        /// <param name="courseId">The course id.</param>
        public void RemoveRssFeed(string rssFeedId, string courseId)
        {
            using (Context.Tracer.StartTrace(String.Format("ContentHelper.RemoveRssFeed(rssArticleId = {0})", rssFeedId)))
            {
                ContentActions.RemoveContent(Context.EntityId, rssFeedId);
            }
        }

        /// <summary>
        /// Removes the RSS Link.
        /// </summary>
        /// <param name="rssArticleId"></param>
        /// <param name="courseId">The course id.</param>
        public void RemoveRssLink(string rssArticleId, string courseId)
        {
            using (Context.Tracer.StartTrace(String.Format("ContentHelper.RemoveRssLink(rssArticleId = {0})", rssArticleId)))
            {
                ContentActions.RemoveContent(Context.EntityId, rssArticleId);
            }
        }

        /// <summary>
        /// Stores the RSS feed.
        /// </summary>
        /// <param name="r">The RssFeed.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreRssFeed(RssFeed article, string courseId)
        {
            BizDC.ContentItem ci;
            if (article.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, article.Id);
                ci.Title = article.Title;
                ci.Description = article.LinkDescription;
                ci.Href = article.RssUrl;
            }
            else
            {
                if (string.IsNullOrEmpty(article.Id))
                {
                    article.Id = Guid.NewGuid().ToString("N");
                }
                ci = article.ToContentItem(courseId);
            }
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Stores the RSS Link.
        /// </summary>
        /// <param name="r">The RssFeed.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreRssLink(RssLink article, string courseId, string parentId = "")
        {
            BizDC.ContentItem ci;
            if (article.IsBeingEdited)
            {
                ci = ContentActions.GetContent(courseId, article.Id);
                ci.Title = article.LinkTitle;
                ci.Description = article.LinkDescription;
                ci.Href = article.Link;
                ci.Sequence = article.Sequence;
            }
            else
            {
                if (string.IsNullOrEmpty(article.Id))
                {
                    article.Id = Guid.NewGuid().ToString("N");
                }
                ci = article.ToContentItem(courseId);
            }
            ContentActions.AddCategoryToItem(ci, System.Configuration.ConfigurationManager.AppSettings["MyMaterials"], null);
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                ci.SetSyllabusFilterCategory(parentId);
            }

            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Sets the default parent.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        public void SetDefaultParent(ContentItem model, BizDC.ContentItem biz)
        {
            if (model.Categories.IsNullOrEmpty())
            {
                biz.DefaultCategoryParentId = model.DefaultCategoryParentId;
                biz.DefaultCategorySequence = model.Sequence;
            }
        }

        /// <summary>
        /// Loads the unit specified.
        /// </summary>
        /// <param name="unitId">The unit ID.</param>
        /// <returns></returns>
        public PxUnit LoadUnit(string unitId)
        {
            if (!string.IsNullOrEmpty(unitId))
            {
                var unit = ContentActions.GetContent(Context.EntityId, unitId);
                if (unit != null)
                {
                    return unit.ToPxUnit(ContentActions);
                }
            }

            return null;
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, ContentViewMode mode)
        {
            return LoadContentView(id, mode, false);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(ContentItem ci, ContentViewMode mode, bool loadToc)
        {
            return LoadContentView(ci, mode, loadToc, true);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(ContentItem ci, ContentViewMode mode, bool loadToc, bool loadSubmissions)
        {
            bool canAddContent = false;
            ci.UserAccess = Context.AccessLevel;
            var q = ci as Quiz;

            //Quiz logic
            if (q != null)
            {
                if (!Context.IsAnonymous)
                {
                    q.Display = (Context.AccessLevel == AccessLevel.Instructor) ? Quiz.DisplayType.Instructor : Quiz.DisplayType.Student;
                    if (loadSubmissions || (q.Display == Quiz.DisplayType.Student && q.QuizType == QuizType.Assessment))
                    {
                        q.Submissions = GradeActions.GetSubmissions(Context.EnrollmentId, ci.Id).Map(s => s.ToSubmission()).ToList();
                    }
                    if (q.Display == Quiz.DisplayType.Student && q.QuizType == QuizType.Homework)
                    {
                        var studentSub = GradeActions.GetStudentSubmission(Context.EnrollmentId, q.Id).ToSubmission();
                        q.QuestionAttempts = studentSub.QuestionAttempts;
                        q.SubmissionAttempts = studentSub.SubmissionAttempts;
                    }
                    if (q.Display == Quiz.DisplayType.Student && q.QuizType == QuizType.Assessment && q.AllowSaveAndContinue)
                    {
                        var path = ci.Id + "/attempt.xml";
                        var itemId = "(agx-wip)";
                        try
                        {
                            q.IsQuizSubmissionSaved = GradeActions.HasSavedDocument(Context.EnrollmentId, itemId, path);
                        }
                        catch
                        {
                            q.IsQuizSubmissionSaved = false;
                        }

                    }
                }
                else
                {
                    q.Display = Quiz.DisplayType.Anonymous;
                }

                q.IsProductCourse = Context.CourseIsProductCourse;
                q.QuestionTypes = Context.GetQuestionTypes();

                //for getting the question analysis.
                //var analysis = ContentActions.GetQuestionAnalysis(Context.EntityId, ci.Id);

                //if (!q.Questions.IsNullOrEmpty())
                //{
                //    foreach (var question in q.Questions)
                //    {
                //        var matched = (from c in analysis where c.QuestionId == question.Id select c);

                //        if (null != matched && matched.Count() > 0)
                //        {
                //            question.Analysis = matched.OrderByDescending(o => o.Version).First().ToQuestionAnalysis();
                //        }
                //    }
                //}
            }

            if (!(ci is EPortfolio) && ci is Assignment && !string.IsNullOrEmpty(Context.EnrollmentId) && Context.Course.CourseType != CourseType.PersonalEportfolioPresentation.ToString())
            {
                var assignment = ((Assignment)ci);
                var submission = GradeActions.GetSubmissions(Context.EnrollmentId, ci.Id).OrderByDescending(x => x.Version).FirstOrDefault();
                if (submission != null)
                {
                    assignment.Submission = submission.ToSubmission();

                    var grade = GradeActions.GetTeacherResponse(Context.EnrollmentId, assignment.Id, assignment.Submission.Version);
                    if (grade != null)
                    {
                        if (assignment.Submission.Version > 0)
                        {
                            assignment.AssignmentStatus = AssignmentStatus.Submitted;
                            if (grade.Status == BizDC.GradeStatus.AllowResubmission)
                            {
                                assignment.AssignmentStatus = AssignmentStatus.Unsubmitted;
                            }
                            else
                            {
                                assignment.PossibleScore = grade.PointsPossible;
                                assignment.AssignedScore = grade.PointsAssigned;
                                if (grade.ScoredVersion == submission.Version)
                                {
                                    assignment.AssignmentStatus = AssignmentStatus.Graded;
                                }
                            }
                        }
                    }
                }
            }
            else if ((ci is Bfw.PX.PXPub.Models.EPortfolio) && !string.IsNullOrEmpty(Context.EnrollmentId) && !((EPortfolio)ci).IsStudentCreatedFolder)
            {
                //if (loadSubmissions)
                //{
                var assignment = ((EPortfolio)ci);
                var submission = GradeActions.GetSubmissions(Context.EnrollmentId, ci.Id).OrderByDescending(x => x.Version).FirstOrDefault();
                if (submission != null)
                {
                    assignment.Submission = submission.ToSubmission();

                    var grade = GradeActions.GetTeacherResponse(Context.EnrollmentId, assignment.Id, assignment.Submission.Version);
                    if (grade != null)
                    {
                        if (assignment.Submission.Version > 0)
                        {
                            assignment.AssignmentStatus = AssignmentStatus.Submitted;
                            if (grade.Status == BizDC.GradeStatus.AllowResubmission)
                            {
                                assignment.AssignmentStatus = AssignmentStatus.Unsubmitted;
                            }
                            else
                            {
                                assignment.PossibleScore = grade.PointsPossible;
                                assignment.AssignedScore = grade.PointsAssigned;
                                if (grade.ScoredVersion == submission.Version)
                                {
                                    assignment.AssignmentStatus = AssignmentStatus.Graded;
                                }
                            }
                        }
                    }
                }
                //}
            }

            ContentViewMode allowed;
            ci.ReadOnly = false;

            if (ci.Type.ToLower() == "assignment" || ci.Type.Equals("source", StringComparison.OrdinalIgnoreCase))
            {
                ci.ReadOnly = mode == ContentViewMode.ReadOnly;
            }

            if (Context.AccessLevel == AccessLevel.Instructor && Context.ProductType != "bcs")
            {
                switch (ci.Type.ToLower())
                {
                    case "assignment":
                        if (ci is PeerReview)
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign |
                                      ContentViewMode.Discussion | ContentViewMode.Edit | ContentViewMode.Settings |
                                      ContentViewMode.ReadOnly | ContentViewMode.MoreResources | ContentViewMode.Rubrics;
                            canAddContent = true;
                        }
                        else if (ci is EPortfolio)
                        {
                            //mishka removed Settings tab and added rubric tab on ePortfolio content view
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Rubrics |
                                ContentViewMode.ReadOnly;
                            ((EPortfolio)ci).IsEditable = Context.AccessLevel == AccessLevel.Instructor;
                            ((EPortfolio)ci).AccessLevel = Context.AccessLevel;
                            var assignedItem = ((EPortfolio)ci).ToContentItem(Context.CourseId).ToAssignedItem();
                            ((EPortfolio)ci).IsAssigned = assignedItem != null && assignedItem.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString();
                        }
                        else if (ci is ReflectionAssignment &&
                            // checking to see if the course type is an eportfolio course (eportfolio, eportfoliotemplate, personaleportfoliopresentation)
                            Context.Course.CourseType.ToLowerInvariant().Contains(BizDC.CourseType.Eportfolio.ToString().ToLowerInvariant()))
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Rubrics |
                                      ContentViewMode.ReadOnly;
                            canAddContent = true;
                        }
                        else if (ci is Dropbox)
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                      ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources |
                                      ContentViewMode.Rubrics | ContentViewMode.Results;
                            ci.ReadOnly = false;
                            canAddContent = true;
                        }
                        else if (ci is MediaContentAssignment || ci is WritingAssignment)
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings |  ContentViewMode.Results |
                                      ContentViewMode.Edit | ContentViewMode.Rubrics | ContentViewMode.ReadOnly;
                            canAddContent = true;
                        }
                        else
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion | ContentViewMode.Results |
                                      ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.Rubrics |
                                      ContentViewMode.ReadOnly | ContentViewMode.MoreResources;
                            canAddContent = true;
                        }

                        break;
                    case "wiki":
                    case "discussion":
                    case "rssfeed":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion | ContentViewMode.Results |
                                  ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources | ContentViewMode.Rubrics;
                        ci.ReadOnly = false;
                        canAddContent = true;
                        break;
                    case "rsslink":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                  ContentViewMode.Settings | ContentViewMode.Rubrics;
                        ci.ReadOnly = false;
                        canAddContent = true;
                        break;
                    case "pxunit":
                        allowed = ContentViewMode.Preview | ContentViewMode.Discussion | ContentViewMode.Edit |
                                  ContentViewMode.Settings | ContentViewMode.ReadOnly | ContentViewMode.Rubrics;
                        canAddContent = true;
                        break;
                    case "rubric":
                        allowed = ContentViewMode.Preview | ContentViewMode.Edit | ContentViewMode.Rubrics;
                        canAddContent = true;
                        break;
                    case "source":
                        allowed = ContentViewMode.Preview | ContentViewMode.Edit | ContentViewMode.ReadOnly;
                        canAddContent = true;
                        break;
                    case "htmlquiz":
                    case "epage":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings | ContentViewMode.Results;
                        canAddContent = false;
                        ci.ReadOnly = true; //TODO: Do we want this?
                        break;
                    default:
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                  ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources |
                                  ContentViewMode.Rubrics | ContentViewMode.Results | ContentViewMode.Grading;
                        ci.ReadOnly = false;
                        canAddContent = true;
                        break;
                }

                if (ci.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString() && !(ci.Type.ToLower() == "rssfeed" || ci.Type.ToLower() == "rsslink" || ci.Type.ToLower() == "discussion" || ci.Type.ToLower() == "wiki"))
                {
                    allowed = allowed | ContentViewMode.Results | ContentViewMode.MoreResources;
                }
                if (Context.Course.IsSandboxCourse)
                    allowed = allowed | ContentViewMode.Metadata;
            }
            else if (Context.AccessLevel == AccessLevel.Student)
            {
                if (mode == ContentViewMode.Results && ci.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString() && !(ci.Type.ToLower() == "rssfeed" || ci.Type.ToLower() == "rsslink" || ci.Type.ToLower() == "discussion" || ci.Type.ToLower() == "wiki"))
                {
                    allowed = ContentViewMode.Results | ContentViewMode.ReadOnly;
                }
                else if (mode == ContentViewMode.Preview && ci is LearningCurveActivity)
                {
                    allowed = ContentViewMode.Results | ContentViewMode.ReadOnly;
                }
                else
                {
                    allowed = ContentViewMode.ReadOnly;
                }
            }
            else
            {
                allowed = ContentViewMode.None;
            }

            if (ci.Type.ToLower() != "rsslink")
            {
                var isMoreResourcesAllowed = false;
                if (Context.Course != null)
                {
                    if (!Boolean.TryParse(Context.Course.MoreResources, out isMoreResourcesAllowed))
                    {
                        allowed = allowed | ContentViewMode.MoreResourcesStatic;
                        allowed = allowed & ~ContentViewMode.MoreResources;
                    }
                    else
                    {
                        if (!isMoreResourcesAllowed)
                        {
                            allowed = allowed & ~ContentViewMode.MoreResources;
                        }
                    }
                }
            }
            if (!(ci is Models.ExternalContent))
            {
                allowed = allowed & ~ContentViewMode.MoreResources;
                allowed = allowed & ~ContentViewMode.MoreResourcesStatic;
            }

            if (ci is MediaContentAssignment)
            {
                allowed = allowed | ContentViewMode.Results;
            }

            if (String.IsNullOrEmpty(ci.Url) && ci.Type.ToLower() != "link")
            {
                ci.Url = string.Format("{0}/Component/ActivityPlayer?EnrollmentId={1}&ItemId={2}&Extra=autostart^true", Context.ProxyUrl, Context.EnrollmentId, ci.Id);
            }

            if (canAddContent && Context.ProductType == "lms")
            {
                allowed = allowed | ContentViewMode.Create;
            }

            if (!Context.Course.bfw_tab_settings.view_tab.show_rubrics && !Context.Course.bfw_tab_settings.view_tab.show_learning_objectives)
            {
                allowed = allowed & ~ContentViewMode.Rubrics;
            }

            //var isSettingsAllowed = (ci is Models.Quiz) || (!string.IsNullOrEmpty(ci.SubType) && ci.SubType == "homework");
            //if (!isSettingsAllowed)
            //{
            //    allowed = allowed & ~ContentViewMode.Settings;
            //}

            allowed = allowed & ~ContentViewMode.MoreResources;
            allowed = allowed & ~ContentViewMode.MoreResourcesStatic;

            if (ci is Folder)
            {
                allowed = allowed & ~ContentViewMode.Results;
            }

            if ((allowed & mode) != mode)
            {
                mode = ContentViewMode.Preview;
            }

            var model = new ContentView
            {
                ActiveMode = mode,
                AllowedModes = allowed,
                Content = ci
            };

            ci.IsBookmarked = BookmarkActions.IsBookmarked(ci.Id);
            ci.EnvironmentUrl = Context.EnvironmentUrl;
            ci.CourseInfo = Context.Course.ToCourse();
            string enrollmentId = Context.EnrollmentId;

            if (Context.Course.CourseType == CourseType.PersonalEportfolioPresentation.ToString() && ci is HtmlDocument)
            {
                enrollmentId = ContentActions.GetPersonalEportfolioDashboardEnrollmentId();
            }

            ci.EnrollmentId = enrollmentId;
            ci.Status = ContentStatus.Existing;
            ci.UserAccess = Context.AccessLevel;

            if (loadToc)
            {
                model.TableOfContents = LoadToc(Context.EntityId, ci.Id);
            }

            if (!Context.Course.Categories.IsNullOrEmpty())
            {
                model.Categories = Context.Course.Categories.Map(c => c.ToTocCategory());
            }

            if (!Context.CourseIsProductCourse && !Context.IsAnonymous && Context.Course.CourseType != "Eportfolio" && Context.Course.CourseType != CourseType.PersonalEportfolioPresentation.ToString())
            {
                model.AllowBookmarks = true;
            }
            else
            {
                model.AllowBookmarks = false;

                // no dot set this to fasle, because we need this in documentViewer to make decision whether
                // it should go through external proxy page or not. it will be set to false for product course in DocumentViewer
                // model.Content.AllowComments = false;
            }

            if (Context.CourseIsProductCourse)
            {
                model.Content.ReadOnly = true;
            }

            var items = new List<ContentItem>();

            items.Add(model.Content);

            model.ContentItems = items;

            return model;
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, bool getChildrenGrades = false)
        {
            return LoadContentView(id, mode, loadToc, true, getChildrenGrades);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, string extEntityId, ContentViewMode mode)
        {
            return LoadContentView(id, mode, false, true, false, null, extEntityId);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, bool loadSubmissions, bool getChildrenGrades = false, string categoryid = null, string extEntityId = null)
        {
            var ci = new ContentItem();

            if (!string.IsNullOrEmpty(id))
            {
                var entityId = !String.IsNullOrEmpty(Context.EnrollmentId) ? Context.EnrollmentId : Context.EntityId;

                if (Context.IsPublicView)
                {
                    entityId = EnrollmentActions.GetUserEnrollmentId(Context.Course.CourseOwner, Context.EntityId);
                    Context.EnrollmentId = entityId;
                    Context.AccessLevel = AccessLevel.Student;
                    if (categoryid.IsNullOrEmpty())
                    {
                        categoryid = entityId;
                    }
                }

                //Apparently you can still get content using an enrollmentid, which is a derivative of the course specific to a user.  But when editing
                //you need to pull the item using the course entity id
                if (mode == ContentViewMode.Edit)
                {
                    if (Context.Course.CourseType == "Eportfolio" && Context.AccessLevel == AccessLevel.Student)
                    {

                    }
                    else
                    {
                        entityId = Context.EntityId;
                    }
                }

                if (!extEntityId.IsNullOrEmpty())
                {
                    entityId = extEntityId;
                }

                var item = ContentActions.GetContent(entityId, id, true, categoryid);
                if (getChildrenGrades && !(ci is PxUnit))
                {//if item isn't a lesson, get it's grades
                    ContentActions.GetGradesPerItem(new List<BizDC.ContentItem>() { item }, Context.CourseId);
                }
                ci = CheckItem(id, item, mode);
              
            }

            return LoadContentView(ci, mode, loadToc, loadSubmissions);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(BizDC.ContentItem item, ContentViewMode mode, bool loadToc, bool loadSubmissions, bool getChildrenGrades = false, string categoryid = null, string extEntityId = null)
        {
            ContentItem ci = new ContentItem(); ;

            if(item != null)
                ci = CheckItem(item.Id, item, mode, getChildrenGrades);
            

            return LoadContentView(ci, mode, loadToc, loadSubmissions);
        }

        /// <summary>
        /// Checks whether an item is a type that has questions.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if it has a type that should be treated as an assessment.</returns>
        private static bool HasQuestions(BizDC.ContentItem item)
        {
            return new string[] { "assessment", "homework" }.Contains(item.Type.ToLower());
        }

        public bool HasMyMaterials(string category, IEnumerable<TocItem> items)
        {
            bool hasMyMaterials = false;
            using (Context.Tracer.DoTrace("HasMyMaterials", category))
            {
                if (string.IsNullOrEmpty(category) || (category.ToLowerInvariant() != System.Configuration.ConfigurationManager.AppSettings["MyMaterials"].ToLowerInvariant()))
                {
                    hasMyMaterials = false;
                }
                else
                {

                    var excludeWithParents = new string[] { ContentActions.TemplateFolder, ContentActions.TemporaryFolder };
                    var filtered = items.Filter(c => (c != null) && !excludeWithParents.Contains(c.ParentId));

                    hasMyMaterials = filtered.Count() > 0;
                }
            }

            return hasMyMaterials;
        }

        /// <summary>
        /// Loads the eportfolio result tab details 
        /// </summary>
        /// <param name="itemId">item id</param>
        /// <param name="enrollmentId">enrollmentId</param>
        /// <returns>content view</returns>
        public ContentView LoadResults(string itemId, string enrollmentId)
        {
            var items = EPortfolioActions.LoadResults(itemId, enrollmentId);
            var results = new List<EportfolioResult>();
            if (items != null)
            {
                foreach (var student in items)
                {
                    var result = new EportfolioResult
                        {
                            EnrollmentId = student.EnrollmentId,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            StudentAvatar = student.Avatar,
                            UserId = student.UserId
                        };

                    var submissions = GradeActions.GetStudentSubmissionInfoList(Context.EntityId, student.EnrollmentId,
                                                                                student.ItemsIds);
                    var comments = NoteActions.GetAllNoteCounts(new BizDC.EportfolioNotesSearch
                        {
                            NoteSearch =
                                student.ItemsIds.Map(i => new BizDC.NoteSearch
                                    {
                                        ItemId = i,
                                        CourseId = Context.EntityId,
                                        UserId =
                                            "",
                                        EnrollmentId = student.EnrollmentId,
                                        HighlightType = BizDC.PxHighlightType.WritingAssignment
                                    }).ToList()
                        });

                    result.ResultItem = GetResultItem(result, student.ResultItems, comments, submissions);

                    results.Add(result);
                }
            }
            return new ContentView { EportfolioResults = results };
        }

        /// <summary>
        /// Loads the results for eportfolio export service
        /// </summary>
        /// <param name="itemId">item id</param>
        /// <param name="enrollmentId">enrollmentId</param>
        /// <returns>content view</returns>
        public ContentView LoadEportfolioExportResults(string itemId, string enrollmentId)
        {
            var items = EPortfolioActions.LoadEportfolioExportResults(itemId, enrollmentId);
            var results = new List<EportfolioResult>();

            foreach (var student in items)
            {
                var result = new EportfolioResult
                {
                    EnrollmentId = student.EnrollmentId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    StudentAvatar = student.Avatar
                };

                var submissions = GradeActions.GetStudentSubmissionInfoList(Context.EntityId, student.EnrollmentId, student.ItemsIds);

                var comments = NoteActions.GetAllNoteCounts(new BizDC.EportfolioNotesSearch
                {
                    NoteSearch =
                        student.ItemsIds.Map(i => new BizDC.NoteSearch
                        {
                            ItemId = i,
                            CourseId = Context.EntityId,
                            //UserId = Context.CurrentUser.Id,
                            EnrollmentId = student.EnrollmentId,
                            HighlightType = BizDC.PxHighlightType.WritingAssignment
                        }).ToList()
                });

                result.ResultItem = GetResultItem(result, student.ResultItems, comments, submissions);

                results.Add(result);
            }

            return new ContentView { EportfolioResults = results };
        }

        /// <summary>
        /// Recursive function to load the eportfolio result item
        /// </summary>
        /// <param name="result"></param>
        /// <param name="bizItem"></param>
        /// <param name="comments"></param>
        /// <param name="submissions"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private EportfolioResultItem GetResultItem(EportfolioResult result, BizDC.EportfolioResultItem bizItem, IEnumerable<BizDC.ItemsNoteCountDetail> comments, IEnumerable<BizDC.Submission> submissions, int level = 0)
        {
            var item = new EportfolioResultItem
            {
                Id = bizItem.Id,
                Name = bizItem.Name,
                DueDate = bizItem.DueDate,
                Hidden = bizItem.Hidden,
                Sequence = bizItem.Sequence,
                Type = bizItem.Type,
                Level = level,
                IsGradable = bizItem.IsGradable
            };

            item.CommentCount = comments.Where(c => c.ItemId == item.Id).Sum(c => int.Parse(c.NoteCount));

            if (item.DueDate != null || item.Type == "ReflectionAssignment")
            {
                item.AssignmentStatus = AssignmentStatus.Unsubmitted;

                var submission = submissions.Where(s => s.ItemId == bizItem.Id).OrderByDescending(s => s.Version).FirstOrDefault();
                if (submission != null)
                {
                    result.SubmittedCount++;
                    item.SubmittedDate = submission.SubmittedDate;
                    item.AssignmentStatus = AssignmentStatus.Submitted;

                    var grade = submission.Grade;

                    if (grade != null && submission.SubmissionStatus == BizDC.SubmissionStatus.Graded)
                    {
                        if (grade.SubmittedVersion == grade.ScoredVersion)
                        {
                            result.GradedCount++;
                            item.AssignmentStatus = AssignmentStatus.Graded;
                            item.Grade = double.IsNaN(grade.Achieved) ? 0 : grade.Achieved;
                            if (bizItem.Rubric != null)
                            {
                                var response = GradeActions.GetTeacherResponse(result.EnrollmentId, bizItem.Id);

                                item.RawPointsAvailable = response.PointsPossible;
                                item.RawPointsScored = response.PointsComputed;
                            }
                            result.TotalGrade += item.Grade.Value;
                        }
                    }
                }
            }

            if (item.Type == "ReflectionAssignment" && item.AssignmentStatus == AssignmentStatus.Unsubmitted)
            {
                var composed = ResourceMapActions.GetResourcesForItem(item.Id, result.EnrollmentId).FirstOrDefault();
                if (composed != null)
                {
                    item.ComposedDate = composed.ModifiedDate;
                }
            }

            foreach (var child in bizItem.Children)
            {
                var childItem = GetResultItem(result, child, comments, submissions, level + 1);
                item.Children.Add(childItem);
            }
            item.CommentCount += item.Children.Sum(e => e.CommentCount);

            return item;
        }

        /// <summary>
        /// Loads the Toc.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public IEnumerable<TocItem> LoadToc(string entityId, string itemId)
        {
            return LoadToc(entityId, itemId, string.Empty);
        }

        /// <summary>
        /// Loads the Toc.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public IEnumerable<TocItem> LoadToc(string entityId, string itemId, string category)
        {
            var toc = new List<TocItem>();
            var child = NavigationActions.LoadNavigation(entityId, itemId, category);
            var tocRoot = LoadTocTree(entityId, "PX_TOC", child);

            toc.Add(tocRoot.ToTocItem(itemId, ShowTocControls(), GetTreeItems(category, itemId), GetItemUpdates(category), Context));

            return toc;
        }

        /// <summary>
        /// Loads the Toc with all the childs.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public IEnumerable<TocItem> LoadTocWithAllChild(string entityId, string itemId, string category)
        {
            var toc = new List<TocItem>();
            var child = NavigationActions.LoadNavigation(entityId, itemId, category);
            child = LoadAllChilds(child, entityId, category);

            var tocRoot = LoadTocTree(entityId, "PX_TOC", child);

            toc.Add(tocRoot.ToTocItem(itemId, ShowTocControls(), GetTreeItems(category, itemId), GetItemUpdates(category), Context));

            return toc;
        }

        private BizDC.NavigationItem LoadAllChilds(BizDC.NavigationItem child, string entityId, string category)
        {
            BizDC.NavigationItem result = child;
            for (int index = 0; index < child.Children.Count; index++)
            {
                var item = child.Children[index];
                child.Children[index] = NavigationActions.LoadNavigation(entityId, item.Id, category, true);
                child.Children[index] = LoadAllChilds(child.Children[index], entityId, category);
            }
            return result;
        }

        ///// <summary>
        ///// Given a list of toc items and active itemid, this method sets the active flag to true on the corresponding toc item
        ///// </summary>
        ///// <param name="selectedItemId"></param>
        ///// <param name="tocs"></param>
        ///// <returns></returns>
        //public void SetActiveTocItem(string selectedItemId, IList<TocItem> tocs)
        //{
        //    //if no selected item to look for in the toc
        //    if (string.IsNullOrEmpty(selectedItemId)) {
        //        return;
        //    }

        //    foreach (var toc in tocs)
        //    {
        //        if (selectedItemId.ToLowerInvariant() == toc.Id.ToLowerInvariant())
        //        {
        //            toc.IsActive = true;
        //            return;
        //        }

        //        if (toc.Children.Any())
        //        {
        //            SetActiveTocItem(selectedItemId, toc.Children );
        //        }
        //    }

        //}

        /// <summary>
        /// Gets the current tree item and its descendents
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IEnumerable<BizDC.ContentItem> GetTreeItems(string categoryId, string itemId)
        {
            if (string.IsNullOrEmpty(categoryId) || categoryId.IndexOf("enrollment_") == -1)
            {
                return null;
            }
            return ContentActions.ListDescendentsAndSelf(categoryId.Substring(11), itemId);
        }

        /// <summary>
        /// gets the list of student updates 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<BizDC.ItemUpdate> GetItemUpdates(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId) || categoryId.IndexOf("enrollment_") == -1)
            {
                return null;
            }
            return UserActivitiesActions.GetItemUpdates(categoryId.Substring(11));
        }

        /// <summary>
        /// Loads the Toc tree.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="rootId">The root id.</param>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        public BizDC.NavigationItem LoadTocTree(string entityId, string rootId, BizDC.NavigationItem child)
        {
            BizDC.NavigationItem root = null;

            try
            {
                if (!string.IsNullOrEmpty(child.ParentId) && child.ParentId != rootId && !child.ParentId.StartsWith("PX_") && !child.ParentId.ToLower().Equals("root"))
                {
                    var pNav = NavigationActions.LoadNavigation(entityId, child.ParentId);
                    var current = pNav.Children.Find(c => c.Id.ToLowerInvariant() == child.Id.ToLowerInvariant());

                    if (current != null && child != null)
                    {
                        current.Children = child.Children;
                    }

                    root = LoadTocTree(entityId, rootId, pNav);
                }
                else
                {
                    root = child;
                }
            }
            catch { }

            return root;
        }

        /// <summary>
        /// Shows the toc controls.
        /// </summary>
        /// <returns></returns>
        public bool ShowTocControls()
        {
            return (Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor == Context.AccessLevel && Context.ProductType != "bcs");
        }

        /// <summary>
        /// Gets the widget configuration collection.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="errorValue">The error value.</param>
        /// <returns></returns>
        public WidgetConfigurationCollection GetWidgetConfigurationCollection(string parentId, string errorValue)
        {
            var item = new WidgetConfigurationCollection();
            item.ParentId = parentId;

            var itemId = item.ParentId + "_ALLOWEDWIDGETLIST";
            string htmlBody = errorValue;
            var c = ContentActions.GetContent(Context.EntityId, itemId, true);

            if (c != null)
            {
                string configData = errorValue;
                if (!c.Properties.ContainsKey("bfw_allowed_widgets"))
                {
                    BizDC.ContentItem ci = new BizDC.ContentItem();

                    string parentCourseId = Context.ProductCourseId ?? Context.EntityId;
                    ci.CourseId = Context.ProductCourseId;
                    ci.ParentId = "PX_ALLOWED_WIDGET_LIST";
                    ci.Id = itemId;
                    ci.Type = "Custom";
                    ci.Subtype = "bfw_homepageconfig";
                    ci.Properties["bfw_allowed_widgets"] = new BizDC.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = configData };
                    ContentActions.StoreContent(ci);
                }
                else
                {
                    htmlBody = c.Properties["bfw_allowed_widgets"].As<string>();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(htmlBody.Replace("and", "&amp;"));
            item.AllowedWidgetsMasterList = doc;
            item.CurrentWidgetList = "|";
            foreach (var widget in NavigationActions.GetWidgets(item.ParentId))
            {
                var webConfig = widget.ToWebConfiguration();
                // hide (skip) this widget if it doesn't contain links
                if (webConfig.Controller == "ContentLinkWidget")
                {
                    var contentLinks = ContentActions.GetContent(Context.EntityId, "PX_LOCATION_ZONE1_MENU_1");
                    var links = ContentActions.ListChildren(Context.EntityId, contentLinks.Id);
                    try
                    {
                        if (links.Count() < 1)
                        {
                            if (Context.AccessLevel.ToString().Equals("Instructor"))
                            {
                                webConfig.IsVisible = false;
                                webConfig.UserAccess = Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    catch { continue; }
                }

                if (!item.Widgets.Exists(i => i.Title == webConfig.Title))
                {
                    item.Widgets.Add(webConfig);
                    item.CurrentWidgetList += webConfig.Id + "|";

                    if (webConfig.Id.Contains("custom_widget"))
                    {
                        doc = new XmlDocument();
                        doc.LoadXml(item.AllowedWidgetsMasterList.InnerXml);
                        var list = doc.SelectSingleNode("WidgetList");

                        XmlNode customNode = doc.CreateNode(XmlNodeType.Element, "Widget", "");

                        XmlAttribute attribName = doc.CreateAttribute("name");
                        attribName.Value = webConfig.Title;
                        customNode.Attributes.Append(attribName);

                        XmlAttribute attribId = doc.CreateAttribute("id");
                        attribId.Value = webConfig.Id;
                        customNode.Attributes.Append(attribId);

                        list.AppendChild(customNode);
                        item.AllowedWidgetsMasterList = doc;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        public void SetVisibility(XElement visibility)
        {
            SetVisibility(visibility, true, "student|instructor");
        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="roles">The roles.</param>
        public void SetVisibility(XElement visibility, bool isVisible, string roles)
        {
            if (!string.IsNullOrEmpty(roles))
            {
                foreach (var role in roles.Split('|'))
                {
                    if (role != "")
                    {
                        if (isVisible)
                        {
                            if (visibility.Element(role) == null) visibility.Add(new XElement(role));
                        }
                        else
                        {
                            try
                            { // in case it's already hidden we catch the removal exception
                                visibility.Element(role).Remove();
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the product visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <param name="product">The product.</param>
        /// <param name="role">The role.</param>
        public void SetProductVisibility(XElement visibility, string product, string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                SetVisibility(visibility, true, role);
            }
            else
            {
                var p = visibility.Elements(product).FirstOrDefault();
                if (p == null)
                {
                    visibility.Add(new XElement(product));
                    p = visibility.Elements(product).FirstOrDefault();
                }

                var r = p.Elements(role).FirstOrDefault();
                if (r == null)
                {
                    p.Add(new XElement(role));
                }
            }
        }

        /// <summary>
        /// Returns a list of item ids
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public List<ContentItem> ListChildren(string id)
        {
            if (NextItems == null)
                NextItems = new List<ContentItem>();

            if (NextItems.Count < 3)
            {
                //var bizNavigationItem = ContentActions.GetContent(Context.EntityId, id);
                var content = ContentActions.GetContent(Context.EntityId, id, true);
                if (content != null)
                {
                    var contentItem = content.ToContentItem(ContentActions, true);

                    if ((contentItem.Type.ToLower() != "folder") || (NextItems.Count == 0))
                        NextItems.Add(contentItem);

                    var items = ContentActions.ListChildren(Context.EntityId, id);

                    if (!items.IsNullOrEmpty())
                    {
                        foreach (var item in items)
                        {
                            if (NextItems.Count < 3)
                            {
                                ListChildren(item.Id);
                            }
                            else
                                break;
                        }
                    }
                }
            }

            return NextItems;
        }

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="includeLead">if set to <c>true</c> [include lead].</param>
        /// <returns></returns>
        public List<ContentItem> ListChildren(ContentItem ci, Boolean includeLead)
        {
            string excludeTypes = "htmldocument,externalcontent";
            List<String> excludes = excludeTypes.Split(',').ToList();

            if (NextItems == null)
            {
                NextItems = new List<ContentItem>();
            }

            if (NextItems.Count < 3)
            {
                if (ci != null)
                {
                    if (!ci.Hidden && includeLead)
                    {
                        if ((excludes.Contains(ci.Type.ToLower())) || (NextItems.Count == 0))
                        {
                            NextItems.Add(ci);
                        }
                    }

                    var items = ContentActions.ListChildren(Context.EntityId, ci.Id);

                    if (!items.IsNullOrEmpty())
                    {
                        foreach (var item in items)
                        {
                            if (NextItems.Count < 3)
                            {
                                ListChildren(item.ToContentItem(ContentActions), true);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return NextItems;
        }

        /// <summary>
        /// Gets the TOC id.
        /// </summary>
        /// <returns></returns>
        public string GetTOCId()
        {
            string tocId = "";
            var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, "PX_TOC");
            var navToc = bizNavigationItem.ToNavigationItem(ContentActions);

            if (navToc != null && navToc.Children.Count > 0)
            {
                tocId = navToc.Children.First().Id;
            }

            return tocId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="menu"></param>
        public void ApplyRulesToMenuItem(List<Bfw.PX.PXPub.Models.MenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                if (!string.IsNullOrEmpty(menuItem.BfwMenuCreatedby) && menuItem.BfwMenuCreatedby.ToLowerInvariant().Contains("eportfolio"))
                {
                    var items = ContentActions.ListChildren(Context.EntityId, ConfigurationManager.AppSettings["eportfolioRootFolder"]);
                    if (items.Count() == 0)
                    {
                        menuItem.Title = "[Inactive] " + menuItem.Title;

                        if (Context.AccessLevel == AccessLevel.Student)
                        {
                            menuItem.WidgetDisplayOptions.DisplayOptions.RemoveAll(i => i == BizDC.DisplayOption.Student);
                            menuItem.VisibleByStudent = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a item for instructor review if that item has been modified by the student
        /// </summary>
        /// <param name="eportfolio"></param>
        public void AddItemForReview(string itemId)
        {
            if (Context.AccessLevel == AccessLevel.Student)
            {
                UserActivitiesActions.MarkItemAsUpdated(new BizDC.ItemUpdate { CourseId = Context.CourseId, EnrollmentId = Context.EnrollmentId, ItemId = itemId });
            }
        }

        /// <summary>
        /// Removes the item from updated items list
        /// </summary>
        /// <param name="itemId"></param>
        public void RemoveItemFromReview(string itemId)
        {
            UserActivitiesActions.DeleteItemUpdateData(new BizDC.ItemUpdate { CourseId = Context.CourseId, EnrollmentId = Context.EnrollmentId, ItemId = itemId });
        }

        /// <summary>
        /// marks the student updated item as reviewed by instructor
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="enrollmentId"></param>
        public void FlagItemAsReviewed(string itemId, string enrollmentId)
        {
            UserActivitiesActions.UnmarkItemAsUpdated(new BizDC.ItemUpdate { CourseId = Context.CourseId, EnrollmentId = enrollmentId, ItemId = itemId });
        }


        /// <summary>
        /// Gets the parent heirachy.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public List<BizDC.ContentItem> GetParentHeirachy(string contentId, TreeCategoryType category, string entityId = "")
        {
            var parents = new List<BizDC.ContentItem>();
            GetAllParent(contentId, category, ref parents, entityId);
            return parents;
        }

        /// <summary>
        /// Gets all parent.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <param name="parents">The parents.</param>
        public void GetAllParent(string itemId, TreeCategoryType category, ref List<BizDC.ContentItem> parents, string entityId = "")
        {
            if (entityId.IsNullOrEmpty())
            {
                entityId = Context.EntityId;
            }

            var ci = ContentActions.GetContent(entityId, itemId);
            if (ci == null || ci.Categories == null || ci.Categories.Count < 0)
            {
                return;
            }
            string parentId = string.Empty;
            BizDC.TocCategory cat;
            switch (category)
            {
                case TreeCategoryType.TOC:
                    cat = ci.Categories.FirstOrDefault(c => c.Id == "bfw_toc_contents");
                    if (cat != null)
                    {
                        parentId = cat.ItemParentId;
                    }
                    break;
                case TreeCategoryType.Assignment:
                    parentId = ci.GetSyllabusFilterFromCategory();
                    break;
                case TreeCategoryType.ManagementCard:
                    parentId = ci.GetSyllabusFilterFromCategory();
                    break;
                default:
                    parentId = string.Empty;
                    break;
            }



            bool istopParent = string.IsNullOrEmpty(parentId) || parentId == "PX_MANIFEST";

            if (!istopParent)
            {
                istopParent = (category == TreeCategoryType.TOC) ? (parentId.ToLower() == "px_toc") : ci.Type.ToLower() == "assignmentcenterfiltersection";
            }

            if (!istopParent)
            {
                parents.Add(ci);
                GetAllParent(parentId, category, ref parents, entityId);
            }
        }


        /// <summary>
        /// Creates the new content of the related.
        /// </summary>
        /// <param name="relatedContentId">The related content id.</param>
        /// <param name="itemIdToAdd">The item id to add.</param>
        /// <returns></returns>
        public RelatedContent CreateNewRelatedContent(string relatedContentId, string itemIdToAdd)
        {
            RelatedContent model = new RelatedContent();
            model.Id = relatedContentId;
            model.EntityId = Context.EntityId;
            if (!string.IsNullOrEmpty(itemIdToAdd))
            {
                model.RelatedContents = new List<ContentItem>();
                model.RelatedContents.Add(new ContentItem() { Id = itemIdToAdd });
            }
            var ci = model.ToContentItem();
            ContentActions.StoreContent(ci);
            return ci.ToRelatedContent(true);
        }

        /// <summary>
        /// Gets the related content ID.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public string GetRelatedContentID(string itemId)
        {
            string relatedContentId = string.Empty;
            if (!string.IsNullOrEmpty(itemId))
            {
                relatedContentId = string.Concat(RELATED_RESOURCE_PREFIX, itemId);
            }
            return relatedContentId;
        }

        #region Implementation
        private ContentItem CheckItem(string id, BizDC.ContentItem item, ContentViewMode mode)
        {
            ContentItem ci;
            item.ParentId = item.GetSyllabusFilterFromCategory();

            //PLATX -- 4934

            //lock the item if it is a readonly course Ex: Publisher Template or Program Manager template accessed by non-owner
            if (ContentActions.Context.IsCourseReadOnly)
            {
                item.IsItemLocked = true;
            }

            if (item != null)
            {
                if (item.Type.ToLower() == "folder" || item.Type.ToLower() == "none") item.CourseId = Context.EntityId;
                bool loadChildren = HasQuestions(item) || (item.ModelType().ToLowerInvariant() == "pxunit");
                ci = item.ToContentItem(Context, loadChildren, null);

                if (ci is PxUnit)
                {
                    ((PxUnit)ci).IsReadOnly = (mode == ContentViewMode.ReadOnly);
                }
            }
            else
            {
                ci = new Content404()
                {
                    Id = id
                };
            }
            return ci;
        }
        #endregion Implementation
    }
}