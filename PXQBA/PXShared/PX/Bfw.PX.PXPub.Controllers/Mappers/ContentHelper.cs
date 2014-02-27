using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
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
        /// <param name="resourceMapActions">The resource map actions.</param>
        public ContentHelper(IBusinessContext context, INavigationActions navActions, IContentActions contActions,
            IAssignmentActions assignmentActions, IGradeActions gradeActions, IResourceMapActions resourceMapActions, INoteActions noteActions,
            IUserActivitiesActions userActivitiesActions)
        {
            NavigationActions = navActions;
            ContentActions = contActions;
            AssignmentActions = assignmentActions;
            GradeActions = gradeActions;            
            Context = context;
            ResourceMapActions = resourceMapActions;
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
        /// <param name="module">The module.</param>
        /// <param name="courseId">The course id.</param>
        public void StoreModule(PxUnit module, string courseId)
        {
            BizDC.ContentItem ci;
            if (module.IsBeingEdited)
            {
                var mod = ContentActions.GetContent(courseId, module.Id).ToPxUnit(ContentActions, isLoadChildren: false);
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
                ContentActions.StoreContent(dItem);                                
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
            InvalidateCachedPageDefinitionsForDerivedCourses(courseId, Context.Course.CourseStartPage);
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
                ci.Type = ci.AssignmentSettings != null && ci.AssignmentSettings.DueDate.Year != DateTime.MinValue.Year
                    ? "Assignment"
                    : "Resource";

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

                var sw = new System.IO.StreamWriter(rez.GetStream());
                sw.Write(System.Web.HttpUtility.HtmlDecode(h.Body));
                sw.Flush();

                ci.Resources = new List<BizDC.Resource> { rez };
                ContentActions.StoreContent(ci);
            }
              
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
        public void StoreRssLink(RssLink article, string courseId, string parentId = "", string toc ="syllabusfilter")
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

            var category = ConfigurationManager.AppSettings["MyMaterials"];
            var itemParentId = category + "_" + Context.CurrentUser.Id;
            using (Context.Tracer.DoTrace("[ContentItem].AddCategoryToItem(category={0}, itemParentId={1})",
                                            category, itemParentId))
            {
                ci.AddCategoryToItem(category, itemParentId, null);
            }

            //These can be added directly from the start page, so need to set a default toc on it.
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                ci.SetSyllabusFilterCategory(parentId, toc);
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
        public PxUnit LoadUnit(string unitId, string toc)
        {
            if (!string.IsNullOrEmpty(unitId))
            {
                var unit = ContentActions.GetContent(Context.EntityId, unitId);
                if (unit != null)
                {
                    return unit.ToPxUnit(ContentActions, toc);
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
        public ContentView LoadContentView(string id, ContentViewMode mode, string toc = "syllabusfilter")
        {
            return LoadContentView(id, mode, false, toc);
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
            }

            if (ci is Assignment && !string.IsNullOrEmpty(Context.EnrollmentId))
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

            ContentViewMode allowed;
            ci.ReadOnly = false;

            if (ci.Type.ToLower() == "assignment" || ci.Type.Equals("source", StringComparison.OrdinalIgnoreCase))
            {
                ci.ReadOnly = mode == ContentViewMode.ReadOnly;
            }

            if (Context.AccessLevel == AccessLevel.Instructor)
            {
                switch (ci.Type.ToLower())
                {
                    case "assignment":
                        if (ci is Dropbox)
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                      ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources;
                            if (ci.IsAssigned)
                            {
                                allowed |= ContentViewMode.Results;
                            }
                            ci.ReadOnly = false;
                        }
                        else
                        {
                            allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion | ContentViewMode.Results |
                                      ContentViewMode.Edit | ContentViewMode.Settings |
                                      ContentViewMode.ReadOnly | ContentViewMode.MoreResources;
                        }

                        break;
                    case "discussion":
                    case "rssfeed":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion | ContentViewMode.Results |
                                  ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources ;
                        ci.ReadOnly = false;
                        break;
                    case "rsslink":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                  ContentViewMode.Settings ;
                        ci.ReadOnly = false;
                        break;
                    case "pxunit":
                        allowed = ContentViewMode.Preview | ContentViewMode.Discussion | ContentViewMode.Edit |
                                  ContentViewMode.Settings | ContentViewMode.ReadOnly;
                        break;
                    case "htmlquiz":
                    case "epage":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings | ContentViewMode.Results;
                        ci.ReadOnly = true; //TODO: Do we want this?
                        break;
                    case "externalcontent":
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                 ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources |
                                 ContentViewMode.Grading;
                         ci.ReadOnly = false;
                        if (ci.AgilixType != DlapItemType.Resource)
                        { //Resources cannot have any results/item analysis
                            allowed |= ContentViewMode.Results;
                        }
                        break;
                    default:
                        allowed = ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Discussion |
                                  ContentViewMode.Edit | ContentViewMode.Settings | ContentViewMode.MoreResources |
                                  ContentViewMode.Results | ContentViewMode.Grading;
                        ci.ReadOnly = false;
                        break;
                }

                if (ci.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString() && !(ci.Type.ToLower() == "rssfeed" || ci.Type.ToLower() == "rsslink" || ci.Type.ToLower() == "discussion"))
                {
                    allowed = allowed | ContentViewMode.Results | ContentViewMode.MoreResources;
                }
                if (Context.Course.IsSandboxCourse)
                    allowed = allowed | ContentViewMode.Metadata;
            }
            else if (Context.AccessLevel == AccessLevel.Student)
            {
                if (mode == ContentViewMode.Results && ci.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString() && !(ci.Type.ToLower() == "rssfeed" || ci.Type.ToLower() == "rsslink" || ci.Type.ToLower() == "discussion"))
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

            if (ci.Type.ToLower() == "link")
            {
                if (!String.IsNullOrEmpty(ci.Url) && !Uri.IsWellFormedUriString(ci.Url, UriKind.Absolute))
                {
                    ci.Url = string.Empty;
                }
            }
            else if (String.IsNullOrEmpty(ci.Url))
            {
                ci.Url = string.Format("{0}/Component/ActivityPlayer?EnrollmentId={1}&ItemId={2}&Extra=autostart^true", Context.ProxyUrl, Context.EnrollmentId, ci.Id);
            }

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
            
            ci.EnvironmentUrl = Context.EnvironmentUrl;
            ci.CourseInfo = Context.Course.ToCourse();
            string enrollmentId = Context.EnrollmentId;

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
        public ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, string toc = "syllabusfilter", bool getChildrenGrades = false)
        {
            return LoadContentView(id, mode, loadToc, true, toc, getChildrenGrades);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, string extEntityId, ContentViewMode mode, string toc = "syllabusfilter")
        {
            return LoadContentView(id, mode, false, true, toc, false, null, extEntityId);
        }

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        public ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, bool loadSubmissions,
            string toc = "syllabusfilter", bool getChildrenGrades = false, string categoryid = null, string extEntityId = null)
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
                    entityId = Context.EntityId;
                }

                if (!extEntityId.IsNullOrEmpty())
                {
                    entityId = extEntityId;
                }

                var item = ContentActions.GetContent(entityId, id, true, categoryid);
                if (getChildrenGrades)
                {//if item isn't a lesson, get it's grades
                    ContentActions.GetGradesPerItem(new List<BizDC.ContentItem>() { item }, Context.CourseId);
                }
                ci = CheckItem(id, item, mode, toc);
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
        public ContentView LoadContentView(BizDC.ContentItem item, ContentViewMode mode, bool loadToc, bool loadSubmissions,
            string toc = "syllabusfilter", string categoryid = null, string extEntityId = null)
        {
            ContentItem ci = new ContentItem(); ;

            if(item != null)
                ci = CheckItem(item.Id, item, mode, toc);
            

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
            return (Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor == Context.AccessLevel);
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
        public List<BizDC.ContentItem> GetParentHeirachy(string contentId, TreeCategoryType category, string toc, string entityId = "")
        {
            var parents = new List<BizDC.ContentItem>();
            GetAllParent(contentId, category, parents, toc, entityId);
            return parents;
        }

        /// <summary>
        /// Gets all parent.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <param name="parents">The parents.</param>
        /// <param name="toc"></param>
        /// <param name="entityId"></param>
        public void GetAllParent(string itemId, TreeCategoryType category, List<BizDC.ContentItem> parents, string toc, string entityId = "")
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
                case TreeCategoryType.ManagementCard:
                    parentId = ci.GetSyllabusFilterFromCategory(toc);
                    break;
                default:
                    parentId = string.Empty;
                    break;
            }

            bool istopParent = string.IsNullOrEmpty(parentId) || parentId == "PX_MANIFEST" || parentId == "PX_MULTIPART_LESSONS"
                || ((category == TreeCategoryType.TOC) ? (parentId.ToLower() == "px_toc") : ci.Type.ToLower() == "assignmentcenterfiltersection");

            parents.Add(ci);
            if (!istopParent)
            {
                GetAllParent(parentId, category, parents, toc, entityId);
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
            return ci.ToRelatedContent(ContentActions, true);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="mode"></param>
        /// <param name="getChildrenGrades"></param>
        /// <param name="toc"></param>
        /// <returns></returns>
        private ContentItem CheckItem(string id, BizDC.ContentItem item, ContentViewMode mode, string toc)
        {
            ContentItem ci;

            if (item != null)
            {
                item.ParentId = item.GetSyllabusFilterFromCategory(toc);

                if (item.Type.ToLower() == "folder" || item.Type.ToLower() == "none") 
                    item.CourseId = Context.EntityId;
                
				bool loadChildren = true;
                if (Context.AccessLevel == AccessLevel.Student && HasQuestions(item))  
					loadChildren = false; 
                //Explicity don't load questions for students, since we don't display them
                
                ci = item.ToContentItem(ContentActions, loadChildren, null);

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

        public void InvalidateCachedPageDefinitionsForDerivedCourses(string courseId, string pageName)
        {
            var links = ContentActions.GetItemLinks(courseId).ToList();
            if (links != null && links.Count > 0)
            {
                links.ForEach(l => Context.CacheProvider.InvalidatePageDefinition(l.EntityId, pageName));
            }
        }
    }
}