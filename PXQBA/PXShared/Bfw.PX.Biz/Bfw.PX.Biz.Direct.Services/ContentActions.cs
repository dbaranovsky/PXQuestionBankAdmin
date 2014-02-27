using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services.Helper;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using Grade = Bfw.PX.Biz.DataContracts.Grade;


namespace Bfw.PX.Biz.Direct.Services
{
    using System.Text.RegularExpressions;

    public class ContentActions : IContentActions
    {
        #region Properties

        /// <summary>
        /// Extension used by all PX Resource files.
        /// </summary>
        public const string PXRES_EXTENSION = "pxres";

        public string GradableParentId { get { return "PX_MANIFEST"; } }
        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected IDocumentConverter DocumentConverter { get; set; }

        protected IDatabaseManager Db { get; set; }

        /// <summary>
        /// The item search service.
        /// </summary>
        protected IItemQueryActions ItemQueryActions { get; set; }

        /// <summary>
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected IResourceMapActions ResourceMapActions
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IResourceMapActions>();
            }
        }

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


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        public ContentActions(IBusinessContext context, ISessionManager sessionManager, IDocumentConverter documentConverter, IDatabaseManager db, IItemQueryActions itemQueryActions)
        {
            Context = context;
            SessionManager = sessionManager;
            DocumentConverter = documentConverter;
            Db = db;
            ItemQueryActions = itemQueryActions;
        }

        #endregion

        #region IContentActions Members

        /// <summary>
        /// Provides access to the BusinessContext the service is running under.
        /// </summary>
        public IBusinessContext Context { get; protected set; }

        /// <summary>
        /// Returns the ID of a location that can store item templates.
        /// </summary>
        public string TemplateFolder { get { return "PX_TEMPLATES"; } }

        /// <summary>
        /// Returns the ID of a location that can store items temporarily.
        /// </summary>
        public string TemporaryFolder { get { return "PX_TEMP"; } }

        /// <summary>
        /// Retrieves all items that are identified as featured.
        /// </summary>
        /// <param name="entityId">Id of the course from which to load the featured items.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListFeaturedItems(string entityId)
        {
            IEnumerable<Bdc.ContentItem> featured = new List<Bdc.ContentItem>();

            using (Context.Tracer.StartTrace("ContentActions.ListFeaturedItems"))
            {
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Type = DlapItemType.Shortcut,
                    ItemId = "PX_FEATURED",
                    Depth = 1
                }, false);

                featured = itemList.Map(f => f.ToContentItem(Context));
            }

            return featured;
        }

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="loadChild">The flag if loading of chidren is required</param>
        /// <returns></returns>
        public IEnumerable<Agilix.DataContracts.Item> ListChildren(string entityId, string parentId, string categoryId, string userId, bool loadChild = false)
        {
            List<Adc.Item> result = null;
            var batch = new Batch();
            var tempCategory = categoryId;

            if (!categoryId.IsNullOrEmpty())
            {
                if (categoryId.IndexOf("enrollment_") > -1)
                {
                    tempCategory = "";
                }
            }

            var childrenCmd = new GetItems()
            {
                SearchParameters = ItemQueryActions.BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId)
            };

            var parentCmd = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = parentId
                }
            };

            batch.Add(parentCmd);
            batch.Add(childrenCmd);

            SessionManager.CurrentSession.ExecuteAsAdmin(batch);

            if (!parentCmd.Items.IsNullOrEmpty())
            {
                var parent = parentCmd.Items.First();
                parent.Children = childrenCmd.Items;
                result = new List<Adc.Item>() { parent };
                result.AddRange(childrenCmd.Items);
            }
            else
            {//Parent doesn't exist (fake parent), use children items
                result = new List<Adc.Item>();
                result.AddRange(childrenCmd.Items);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                if (result == null)
                {
                    result = new List<Adc.Item>();
                }

                if (!categoryId.IsNullOrEmpty())
                {
                    if (categoryId.IndexOf("enrollment_") > -1)
                    {
                        tempCategory = categoryId;
                    }
                }

                childrenCmd.SearchParameters = ItemQueryActions.BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId);

                if (!categoryId.IsNullOrEmpty())
                {
                    if (categoryId.IndexOf("enrollment_") > -1)
                    {
                        ApplyStudentItems(result, categoryId.Substring(11), loadChild, childrenCmd.SearchParameters);
                    }
                    else
                    {
                        ApplyStudentItems(result, string.Empty, loadChild, childrenCmd.SearchParameters);
                    }
                }
                else
                {
                    ApplyStudentItems(result, string.Empty, loadChild, childrenCmd.SearchParameters);
                }
            }

            return result;
        }

        /// <summary>
        /// List the descendents including student items
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">item id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public IEnumerable<Agilix.DataContracts.Item> ListDescendents(string entityId, string itemId, string userId)
        {
            List<Adc.Item> result = null;

            var parentCmd = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = itemId
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(parentCmd);

            if (!parentCmd.Items.IsNullOrEmpty())
            {
                var parent = parentCmd.Items.First();
                result = new List<Adc.Item>() { parent };

                var studentItems = GetStudentItems(userId, string.Empty).ToList();
                ListDescendents(entityId, userId, parent, result, studentItems);
            }
            else
            {
                var studentItems = GetStudentItems(userId, string.Empty).ToList();
                var item = studentItems.FirstOrDefault(i => i.Id == itemId);

                if (item != null)
                {
                    result = new List<Adc.Item>() { item };
                    ListDescendents(entityId, userId, item, result, studentItems);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the student items.
        /// </summary>
        /// <param name="enrollmentId">enrollment id</param>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        private IEnumerable<Agilix.DataContracts.Item> GetStudentItems(string enrollmentId, string categoryId)
        {
            var studentResourceDoc = GetResource(enrollmentId, GetStudentResourceId(enrollmentId));
            List<Adc.Item> allStudentItems = new List<Adc.Item>();

            if (!categoryId.IsNullOrEmpty())
            {
                studentResourceDoc = GetResource(categoryId, GetStudentResourceId(categoryId));
            }

            if (studentResourceDoc != null)
            {
                var stream = studentResourceDoc.GetStream();
                XDocument doc;

                try
                {
                    doc = stream != null && stream.Length > 0
                                        ? XDocument.Load(stream)
                                        : GetEmptyStudentDoc();
                }
                catch
                {
                    doc = GetEmptyStudentDoc();
                }

                foreach (var itemElement in doc.Root.Elements("item"))
                {
                    var item = new Adc.Item();
                    item.ParseEntity(itemElement);
                    allStudentItems.Add(item);
                }
            }

            return allStudentItems;
        }

        /// <summary>
        /// Items that are in the TOC can be considered featured if they have a bfw_property for 'is_featured'
        /// This method will return a list of featured items from with in the course
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="featuredState">
        /// Three states are possible [true, false, null]
        /// true: only returns featured content that has an attribute of true
        /// false: only return featured content that has an attribute of false or empty
        /// null: returns any items that have the bfw_property 'is_featured'
        /// </param>
        /// <returns>List of Featured Content Items</returns>
        public IEnumerable<Bdc.ContentItem> ListItemsThatAreFeatured(string entityId, bool? featuredState = null)
        {
            IEnumerable<Bdc.ContentItem> featured = new List<Bdc.ContentItem>();

            using (Context.Tracer.StartTrace("ContentActions.ListItemsThatAreFeatured"))
            {
                // gets all items that have a flag is_featured in the bfw_property name attribute
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = String.Format("/bfw_properties/bfw_property@name='{0}'", "is_featured")
                }, false);

                featured = itemList.Map(f => f.ToContentItem(Context));

                if (featuredState.HasValue)
                {
                    // we only care about the items that have a TRUE value and disregard the false values
                    bool value;
                    featured = featured.Where(x => bool.TryParse((String)x.Properties["is_featured"].Value, out value) && value == featuredState.Value);
                }
            }

            return featured;
        }

        /// <summary>
        /// Retrieves the item specified, without loading associated resources.
        /// </summary>
        /// <param name="entityId">Entity from which to load the content.</param>
        /// <param name="id">Id of the item to load.</param>
        /// <returns></returns>
        public Bdc.ContentItem GetContent(string entityId, string id)
        {
            return GetContent(entityId, id, false);
        }

        /// <summary>
        /// Retrieves the item specified. If loadResources is true then any resource
        /// pointed to by the Href property (if it is NOT a fully qualified URL) will also
        /// be loaded.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load content.</param>
        /// <param name="id">Id of the item to load.</param>
        /// <param name="loadResources">True if the resources associated to the item should be loaded, false otherwise.</param>
        /// <returns></returns>
        public Bdc.ContentItem GetContent(string entityId, string id, bool loadResources)
        {
            return GetContent(entityId, null, id, loadResources);
        }

        /// <summary>
        /// Retrieves the item specified. Personal Presentation Course resources use the dashboard enrollment Id. 
        /// If loadResources is true then any resource pointed to by the Href property (if it is NOT a fully qualified URL) will also
        /// be loaded.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load content.</param>
        /// <param name="dashboardEnrollmentId">Student dashboard enrollment Id.</param>
        /// <param name="id">Id of the item to load.</param>
        /// <param name="loadResources">True if the resources associated to the item should be loaded, false otherwise.</param>
        /// <returns></returns>
        public Bdc.ContentItem GetContent(string entityId, string dashboardEnrollmentId, string id, bool loadResources)
        {
            Bdc.ContentItem ci = null;

            using (Context.Tracer.StartTrace("ContentActions.GetContent"))
            {
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = id
                });

                if (!itemList.IsNullOrEmpty())
                {
                    ci = itemList.First().ToContentItem(Context);

                    if (ci.AssignmentSettings.IsSendReminder)
                    {
                        ci.AssignmentSettings.ReminderEmail = GetReminderMailDetails(entityId: entityId, itemId: ci.Id, dueDate: ci.AssignmentSettings.DueDate);
                    }

                    if (loadResources)
                    {
                        if (!string.IsNullOrEmpty(ci.AssignmentSettings.Rubric))
                        {
                            ci.Rubric = GetResource(Context.EntityId, ci.AssignmentSettings.Rubric);
                        }

                        if (!string.IsNullOrEmpty(ci.Href) && !string.Equals(ci.Href, "comingsoon.html"))
                        {
                            var targetEntityId = String.IsNullOrEmpty(dashboardEnrollmentId) ? entityId : dashboardEnrollmentId;
                            LoadResource(ci, targetEntityId, ci.Href);
                        }
                    }
                }
            }
            return ci;
        }

        /// <summary>
        /// Retrieves the item specified. If loadResources is true then any resource
        /// pointed to by the Href property (if it is NOT a fully qualified URL) will also
        /// be loaded.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load content.</param>
        /// <param name="id">Id of the item to load.</param>
        /// <param name="loadResources">True if the resources associated to the item should be loaded, false otherwise.</param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Bdc.ContentItem GetContent(string entityId, string id, bool loadResources, string categoryId)
        {
            Bdc.ContentItem ci = null;
            var eid = entityId;

            if (string.IsNullOrEmpty(Context.EntityId))
            {
                eid = Context.CourseId;
            }

            using (Context.Tracer.DoTrace("ContentActions.GetContent(entityId={0}, id={1}, loadResources={2}, categoryId={3})", entityId, id, loadResources, categoryId))
            {
                if (categoryId.IsNullOrEmpty())
                    categoryId = Context.EnrollmentId;
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = id
                }, true, categoryId);

                if (!itemList.IsNullOrEmpty())
                {
                    ci = itemList.First().ToContentItem(Context);

                    if (ci.AssignmentSettings.IsSendReminder)
                    {
                        ci.AssignmentSettings.ReminderEmail = GetReminderMailDetails(entityId: entityId, itemId: ci.Id, dueDate: ci.AssignmentSettings.DueDate);
                    }

                    if (loadResources)
                    {
                        if (!string.IsNullOrEmpty(ci.AssignmentSettings.Rubric))
                        {
                            ci.Rubric = GetResource(Context.EntityId, ci.AssignmentSettings.Rubric);
                        }

                        if (!string.IsNullOrEmpty(ci.Href) && !string.Equals(ci.Href, "comingsoon.html"))
                        {
                            LoadResource(ci, entityId, ci.Href);
                        }
                    }
                }
            }
            return ci;
        }



        /// <summary>
        /// Gets the details of remainder email to be sent based on the item id and was set by instructor
        /// </summary>
        /// <param name="entityId">Entity Id from which Reminder details to be fetched</param>
        /// <param name="itemId">Entity Id details</param>
        /// <param name="dueDate">Due Date</param>
        /// <returns>Reminder Email object with Body and Subject text</returns>
        public Bdc.ReminderEmail GetReminderMailDetails(string entityId, string itemId, DateTime dueDate)
        {
            var email = new Bdc.ReminderEmail();

            using (Context.Tracer.DoTrace("ContentActions.GetReminderMailDetails(EntityId = {0}, itemId={1}, dueDate={2}) - db(GetEmailTrackingInfo)", entityId, itemId, dueDate))
            {
                Db.ConfigureConnection("PXData");

                try
                {
                    Db.StartSession();
                    var records = Db.Query("GetEmailTrackingInfo @0, @1, @2", itemId,string.Empty, entityId);

                    if (records != null && records.Count() > 0)
                    {
                        var record = records.First();
                        var sendOnDate = dueDate - record.DateTime("SendOnDate");

                        CalculateDurationTypeAndDaysBefore(sendOnDate, email);

                        email.AssignmentId = itemId;
                        email.Body = record.String("EmailBody"); //message.XPathSelectElement("//body").Value;
                        email.Subject = record.String("EmailSubject"); //message.XPathSelectElement("//subject").Value;
                    }
                }
                finally
                {
                    Db.EndSession();
                }
            }

            return email;
        }

        /// <summary>
        /// This method is used to get the duration type and days before for an assignment
        /// </summary>
        /// <param name="sendOnDate">Difference between Assignment "Due date" and Assignment "Send on date"</param>
        /// <param name="email">Reminder email with updated Duration Type and Days Before data</param>
        public void CalculateDurationTypeAndDaysBefore(TimeSpan sendOnDate, ReminderEmail email)
        {
            int days = sendOnDate.Days;
            int hour = sendOnDate.Hours;
            int daysHourMin = (int)sendOnDate.TotalMinutes;

            if (daysHourMin % (7 * 24 * 60) == 0)
            {
                email.DurationType = "week";
                email.DaysBefore = (days / 7);
            }
            else if (daysHourMin % (24 * 60) == 0)
            {
                email.DurationType = "day";
                email.DaysBefore = days;
            }
            else if (daysHourMin % 60 == 0)
            {
                email.DurationType = "hour";
                email.DaysBefore = (days * 24) + hour;
            }
            else
            {
                email.DurationType = "minute";
                email.DaysBefore = daysHourMin;
            }
        }

        /// <summary>
        /// Returns the matching resource.
        /// </summary>
        /// <param name="entityId">Id of the entity to load resource from.</param>
        /// <param name="resourceUri">Uri to the resource.</param>
        /// <returns></returns>
        public Bdc.Resource GetResource(string entityId, string resourceUri)
        {
            Bdc.Resource result = null;

            using (Context.Tracer.DoTrace("ContentActions.GetResource(entityId={0}, resourceUri={1})", entityId, resourceUri))
            {
                result = LoadResource(entityId, resourceUri);
            }

            return result;
        }


        public Stream GetResourceStream(IEnumerable<Bdc.Resource> resources, out string fileName)
        {
            using (Context.Tracer.DoTrace("ContentActions.GetResourceStream(resources, out fileName)"))
            {
                var docConversions = resources.Select(resource => new DocumentConversion
                {
                    DataStream = resource.GetStream(),
                    FileName = resource.Name,
                    OutputType = DocumentOutputType.Image
                }).ToList();

                // Set fileName if its only one doc, if > 1 then zip file name should be set by caller.
                fileName = docConversions.Count == 1 ? docConversions.First().FileName : "";
                return DocumentConverter.ConvertDocuments(docConversions);
            }
        }



        /// <summary>
        /// Returns the matching resources.
        /// </summary>
        /// <param name="entityId">Id of the entity to load resources from.</param>
        /// <param name="resourceUri">Uri to the resources to load.</param>
        /// <param name="xQuery">Optional query to match resources against.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Resource> ListResources(string entityId, string resourceUri, string xQuery)
        {
            IEnumerable<Bdc.Resource> result = null;

            using (Context.Tracer.DoTrace("ContentActions.ListResources(entityId={0}, resourceUri={1}, xQuery)", entityId, resourceUri))
            {
                result = LoadResources(entityId, resourceUri, xQuery);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of resource info data (no stream)
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="resourceUri"></param>
        /// <param name="xQuery"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.Resource> ListResourcesInfo(string entityId, string resourceUri, string xQuery)
        {
            IEnumerable<Bdc.Resource> result = null;

            using (Context.Tracer.DoTrace("ContentActions.ListResourcesInfo(entityId={0}, resourceUri={1}, xQuery)", entityId, resourceUri))
            {
                result = LoadResourcesInfo(entityId, resourceUri, xQuery);
            }

            return result;
        }

        /// <summary>
        /// Returns the matching resources.
        /// </summary>
        /// <param name="resourceIds">Comma delimited list of resource ids.</param>
        /// <param name="enrollmentId">Id of the enrollment to load resources from.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Resource> ListResources(string resourceIds, string enrollmentId)
        {
            using (Context.Tracer.DoTrace("ContentActions.ListResources(resourceUri={0}, enrollmentId={1})", resourceIds, enrollmentId))
            {
                var itemIds = resourceIds.Split(',').ToList();
                return ListResources(itemIds, enrollmentId);
            }
        }

        /// <summary>
        /// Returns the matching resources.
        /// </summary>
        /// <param name="resourceIds">List of resource Ids.</param>
        /// <param name="enrollmentId">Id of the enrollment that contains the resources.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Resource> ListResources(IEnumerable<string> resourceIds, string enrollmentId)
        {
            var resourceList = new List<Bdc.Resource>();

            using (Context.Tracer.DoTrace("ContentActions.ListResources(resourceIds,enrollmentId={0})", enrollmentId))
            {
                foreach (var s in resourceIds)
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    var path = '*' + s + '*';
                    var resList = ListResources(enrollmentId, path, "");

                    if (resList.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var resource = resList.First();
                    resource.ExtendedId = s;

                    resourceList.Add(resource);
                }
            }

            return resourceList;
        }

        /// <summary>
        /// Gets the list of item and its descendents based on item id.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> ListDescendentsAndSelf(string entityId, string itemId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListDescendentsAndSelf(entityId={0}, itemId={1}", entityId, itemId))
            {
                var itemList = ListDescendents(Context.CourseId, itemId, entityId);
                if (!itemList.IsNullOrEmpty())
                {
                    result = itemList.Map(c => c.ToContentItem(Context));
                }
                else
                {
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the immediate children of the specified parent item, if any.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load items.</param>
        /// <param name="parentId">Id of the parent item whose children are to be loaded.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListChildren(string entityId, string parentId)
        {
            return ListChildren(entityId, parentId, 1, string.Empty);
        }

        /// <summary>
        /// Gets the children of the specified parent item for a specified number of levels, if any.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load items.</param>
        /// <param name="parentId">Id of the parentItem whose children are to be loaded.</param>
        /// <param name="depth">Level to which to load children.</param>
        /// <param name="categoryId">Id of the category the children must belong to.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListChildren(string entityId, string parentId, int depth, string categoryId, bool ignoreParent = true)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListChildren(entityId={0}, parentId={1}, depth={2}, categoryId={3}", entityId, parentId, depth, categoryId))
            {
                string currentUserId = string.Empty;

                if (Context.IsPublicView && (Context.CurrentUser == null || (Context.CurrentUser != null && Context.CurrentUser.Id != Context.Course.CourseOwner)))
                {
                    currentUserId = Context.Course.CourseOwner;
                    Context.CurrentUser = new UserInfo();
                    Context.CurrentUser.Id = currentUserId;
                    var cmdUser = new GetUsers()
                    {
                        SearchParameters = new Adc.UserSearch()
                        {
                            Id = currentUserId
                        }
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdUser);

                    if (!cmdUser.Users.IsNullOrEmpty())
                    {
                        Context.CurrentUser.Email = cmdUser.Users.FirstOrDefault().Email;
                        Context.CurrentUser.ReferenceId = cmdUser.Users.FirstOrDefault().Reference;
                        Context.CurrentUser.Username = cmdUser.Users.FirstOrDefault().UserName;
                        Context.CurrentUser.PasswordQuestion = "#DynamicRunTimePasswordQuestion#";
                    }
                    Context.EnrollmentId = EnrollmentActions.GetUserEnrollmentId(Context.Course.CourseOwner, entityId);
                }
                else if (Context.CurrentUser != null)
                {
                    currentUserId = Context.CurrentUser.Id;
                }
                var children = ListChildren(entityId, parentId, categoryId, currentUserId);

                if (!children.IsNullOrEmpty() && ignoreParent && !parentId.IsNullOrEmpty()) //skip first item because parent is also returned
                {
                    children = children.Where(e => !e.Id.IsNullOrEmpty() && !e.Id.Trim().Equals(parentId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                }

                if (!children.IsNullOrEmpty())
                {
                    var tempChildren = children.ToList();
                    tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                    children = tempChildren.ToList();
                }

                result = children.OrderBy(i => i.Sequence).Map(i => i.ToContentItem(Context));
            }

            return result;
        }

        /// <summary>
        /// Performs search for items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="queryParams">Query to use for search.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="op">The operation parameter.</param>
        /// <returns></returns>
        public List<Agilix.DataContracts.Item> DoItemSearch(string entityId, Dictionary<string, string> queryParams, string userId, string op = "OR")
        {
            var queryCmd = new GetItems()
            {
                SearchParameters = ItemQueryActions.BuildItemSearchQuery(entityId, queryParams, userId, op)
            };

            var batch = new Batch();
            batch.Add(queryCmd);

            SessionManager.CurrentSession.ExecuteAsAdmin(batch);

            var items = queryCmd.Items;

            if (!queryCmd.Items.IsNullOrEmpty())
            {
                var tempChildren = queryCmd.Items.ToList();
                tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                items = tempChildren.ToList();
            }

            return items;
        }

        /// <summary>
        /// Loads all items for a specific container and/or subcontainer
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="containerId"></param>
        /// <param name="subcontainerId"></param>
        /// <param name="extraQueryFilter"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetContainerItems(string entityId, string containerId, string subcontainerId, string toc = "", string extraQueryFilter = "")
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            //Dictionary<string, string> queryDeprecated = new Dictionary<string, string>();
            List<Bdc.ContentItem> results = null;
            //List<Bdc.ContentItem> deprecatedResults = null;
            if (!toc.IsNullOrEmpty())
            {
                if (containerId != null)
                {
                    query.Add("meta-containers/meta-container", containerId);
                }
                if (subcontainerId != null)
                {
                    query.Add("meta-subcontainers/meta-subcontainerid", subcontainerId);
                }
            }

            if (query.Count > 0)
            {
                if (!extraQueryFilter.IsNullOrEmpty())
                {
                    query.Add("freequery", extraQueryFilter);
                }

                results = DoItemsSearch(entityId, query, "AND").ToList()
                       .Filter(i =>  //ensure that contains and subcontainers match up with the toc parameter since we have no way to search for attributes of the same element
                       i.Containers.Where(container => container.Toc.ToLower().Equals(toc)).Any(c => c.Value.ToLower() == containerId.ToLower())  //find the "toc" container and check its value == containerId
                       && (
                       (subcontainerId == null) || //if subcontainerid is empty we don't need to check the subcontainerId
                       i.SubContainerIds.Where(container => container.Toc.ToLower().Equals(toc)).Any(c => c.Value.ToLower() == subcontainerId.ToLower())
                       )).ToList(); // check the "toc" subcontainerId value == subcontainerId
            }

            if (results != null)
            {
                //TODO: Temporary processing to fix shortcut items
                results.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                //filter out the list of deprecated results by removing all items that also exists in results
                //deprecatedResults = deprecatedResults.Filter(d => !results.Contains(d, (result, dep) => result.Id == dep.Id)).ToList();
                //add remaining deprecated results to results
                //results.AddRange(deprecatedResults);
            }
            return results;
        }

        /// <summary>
        /// Get items for a specific container under a specific parent item (folder) in this container
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="containerId"></param>
        /// <param name="subcontainerId"></param>
        /// <param name="toc"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetContainerItemsForParent(string entityId, string containerId,
                                                                       string subcontainerId, string parentId,
                                                                       string toc = "")
        {
            var containerItems = GetContainerItems(entityId, containerId, subcontainerId, toc);
            var parent = GetContent(entityId, parentId);
            if (containerItems != null || containerItems.Count() != 0)
            {
                return GetAssociatedChildren(containerItems, parent, toc);

            }
            return null;
        }
        //recursively get all the children of the parent in this container
        private List<ContentItem> GetAssociatedChildren(IEnumerable<ContentItem> source, ContentItem parent, string toc)
        {
            List<ContentItem> children = new List<ContentItem>();
            if (parent.Type.ToLower() == "pxunit" || (!string.IsNullOrEmpty(parent.Subtype) && parent.Subtype.ToLower() == "pxunit"))
            {
                children.AddRange(source.Where(i => i.Categories.Any(c => c.Id == toc && c.ItemParentId == parent.Id)));

                var subchildren = new List<ContentItem>();
                foreach (var child in children)
                {
                    subchildren.AddRange(GetAssociatedChildren(source, child, toc));
                }
                children.AddRange(subchildren);
            }
            return children;
        }
        

        /// <summary>
        /// Gets all the related content items for an item
        /// Requires MetaData on dlap items
        /// /meta-related-to-list/meta-related-to
        /// </summary>
        /// <param name="entityId">Course Id</param>
        /// <param name="itemId">Item Id</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetRelatedItems(string entityId, string itemId)
        {
            using (Context.Tracer.DoTrace("ContentActions.GetRelatedItems(entityId={0}, itemId={1})", entityId, itemId))
            {
                // getting the list of related content items from dlap
                string query = string.Format("/meta-related-to-list/meta-related-to='{0}'", itemId);
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                // removing duplicutes and converting the unique list to content items
                return itemList.Distinct((a, b) => a.Id == b.Id).Map(i => i.ToContentItem(Context));
            }
        }

        /// <summary>
        /// Gets the children of the items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemIds">The item ids.</param>
        /// <param name="tocDefinition">The name of the node element that defines the TOC</param>
        /// <param name="includingShortCuts">removed all shortcut items if false</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetChildItems(string entityId, List<string> itemIds, String tocDefinition, bool includingShortCuts = false)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();
            var items = GetItemsByQuery(entityId, itemIds, tocDefinition, includingShortCuts);

            if (!items.IsNullOrEmpty())
            {
                result = items.OrderBy(i => i.Sequence).Map(i => i.ToContentItem(Context));
            }

            return result;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemIds">The item ids.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetItems(string entityId, List<string> itemIds, bool includingShortCuts = false)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();
            string currentUserId = string.Empty;
            
            if (Context.CurrentUser != null)
            {
                currentUserId = Context.CurrentUser.Id;
            }

            var items = GetItemsByIdList(entityId, itemIds, currentUserId, includingShortCuts);
            
            if (!items.IsNullOrEmpty())
            {
                result = items.OrderBy(i => i.Sequence).Map(i => i.ToContentItem(Context));
            }

            return result;
        }

        public IEnumerable<Bdc.ContentItem> DoItemsSearch(string entityId, Dictionary<string, string> queryParams, string op = "OR")
        {
            // replacing single quote and double quotes by '' and "" respectively to make it acceptable for dlap
            if (queryParams != null)
            {
                Dictionary<string, string> updatedPair = new Dictionary<string, string>();
                foreach (var query in queryParams)
                {
                    if (query.Key != "freequery")
                    {
                        if (!query.Value.IsNullOrEmpty() && (query.Value.Contains("'") || query.Value.Contains("\"")))
                        {
                            updatedPair[query.Key] = query.Value.Replace("'", "''");
                            updatedPair[query.Key] = updatedPair[query.Key].Replace("\"", "\"\"");
                        }
                    }
                }
                foreach (var pair in updatedPair)
                {
                    queryParams[pair.Key] = pair.Value;
                }
            }
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();
            string currentUserId = string.Empty;
            if (Context.CurrentUser != null)
            {
                currentUserId = Context.CurrentUser.Id;
            }
            if (Context.AccessLevel == AccessLevel.Student && !string.IsNullOrEmpty(Context.EnrollmentId) && entityId == Context.EntityId)  
            {
                entityId = Context.EnrollmentId;
            }
            var items = DoItemSearch(entityId, queryParams, currentUserId, op);
            if (!items.IsNullOrEmpty())
            {
                result = items.OrderBy(i => i.Sequence).Map(i => i.ToContentItem(Context));
            }
            return result;
        }


        /// <summary>
        /// Retrieves all the content in the entity.
        /// </summary>
        /// <param name="entityId">Id of the entity to load content from.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListContent(string entityId)
        {
            IEnumerable<Bdc.ContentItem> content = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContent(entityId={0})", entityId))
            {
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId
                });

                content = itemList.Map(c => c.ToContentItem(Context));
            }

            return content;
        }

        /// <summary>
        /// Retrieves the content of specific subtype in the entity.
        /// </summary>
        /// <param name="entityId">Id of the entity from which to load content.</param>
        /// <param name="subType">Type of content to load.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListContent(string entityId, string subType)
        {
            IEnumerable<Bdc.ContentItem> content = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContent(entityId={0}, subType={1})", entityId, subType))
            {
                string query = "";
                if (!string.IsNullOrEmpty(subType))
                {
                    query = string.Format("/bfw_type='{0}'", subType);
                }

                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                content = itemList.Map(c => c.ToContentItem(Context));
            }

            return content;
        }

        /// <summary>
        /// Retrieves all the content in the entity.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="listStudentItems">List Student Content</param>
        /// <returns></returns>
        public IEnumerable<ContentItem> ListContent(string entityId, string subType, bool listStudentItems)
        {
            IEnumerable<Bdc.ContentItem> content = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContent(entityId={0}, subType={1})", entityId, subType))
            {
                string query = "";
                if (!string.IsNullOrEmpty(subType))
                {
                    query = string.Format("/bfw_type='{0}'", subType);
                }

                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query,
                    ExcludeStudentItem = !listStudentItems
                });

                content = itemList.Map(c => c.ToContentItem(Context));
            }

            return content;
        }

        /// <summary>
        /// Retrieves all content that has a due date assigned.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="containerId">The container id (for example launchpad).</param>
        /// <param name="subcontainerId">The chapter.</param>
        /// <returns></returns>
        public IEnumerable<ContentItem> ListContentWithDueDates(string entityId, string containerId, string subcontainerId, string toc)
        {
            DateTime now = DateTime.Now.GetCourseDateTime(Context);
            string date = now.Year + "/" + now.Month + "/" + now.Day;

            return GetContainerItems(entityId, containerId, subcontainerId, toc, string.Format("(/DUEDATE>='{0}' OR /ALLOWLATESUBMISSION=true)", date));
        }

        /// <summary>
        /// Retrieves all content that has a due date assigned.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="rootFolder">The root folder.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListContentWithDueDates(string entityId, string parentId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContentWithDueDates(entityId={0}, parentId={1})", entityId, parentId))
            {
                DateTime now = DateTime.Now.GetCourseDateTime(Context);
                string date = now.Year + "/" + now.Month + "/" + now.Day;

                var query = "";
                if (!string.IsNullOrEmpty(parentId))
                {
                    query = string.Format(@"/bfw_tocs/{0}@parentid='{1}'", parentId, parentId);
                    query += string.Format("AND /PARENT='PX_MANIFEST' AND (/DUEDATE>='{0}' OR /ALLOWLATESUBMISSION=true)", date);
                }
                else
                {
                    query = string.Format("/PARENT='PX_MANIFEST' AND (/DUEDATE>='{0}' OR /ALLOWLATESUBMISSION=true)", date);
                }

                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                result = itemList.Map(c => c.ToContentItem(Context));
            }

            return result;
        }

        /// <summary>
        /// Retrieves all content that has a due date assigned.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="fromDate">From Date</param>
        /// <param name="toDate">To Date</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListContentWithDueDatesWithinRange(string entityId, string fromDate, string toDate)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContentWithDueDatesWithinRange(entityId={0}, fromDate={1}, toDate={2})", entityId, fromDate, toDate))
            {
                DateTime now = DateTime.Now.GetCourseDateTime(Context);
                var query = string.Format("/PARENT='PX_MANIFEST' AND /DUEDATE>='{0}' AND /DUEDATE<='{1}'", fromDate, toDate);

                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                result = itemList.Map(c => c.ToContentItem(Context));
            }

            return result;
        }



        /// <summary>
        /// Gets the template items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetTemplateItems(string entityId, string itemId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.GetTemplateItems(entityId={0}, itemId={1})", entityId, itemId))
            {
                var item = GetContent(entityId, itemId);

                string query = string.Format("/bfw_template='{0}' AND /parent<>'{1}' AND /parent<>'{2}'", item.Template, TemplateFolder, TemporaryFolder);
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                result = itemList.Map(i => i.ToContentItem(Context)).Distinct((a, b) => a.Id == b.Id);
            }

            return result;
        }


        /// <summary>
        /// Gets the items associated with derived (user created) templates.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetDerivedTemplateItems(string entityId, string itemId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.GetDerivedTemplateItems(entityId={0}, itemId={1})", entityId, itemId))
            {
                var item = GetContent(entityId, itemId);

                string query = string.Format("/bfw_template='{0}' AND /parent<>'{1}' AND /parent<>'{2}'", item.Template, TemplateFolder, TemporaryFolder);
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });

                result = itemList.Map(i => i.ToContentItem(Context)).Distinct((a, b) => a.Id == b.Id);
            }

            return result;
        }

        /// <summary>
        /// Stores the specified piece of content and associated resources
        /// </summary>
        /// <param name="content">The content to store.</param>
        public void StoreContent(Bdc.ContentItem content, string entityId = "")
        {
            if (entityId.IsNullOrEmpty())
            {
                entityId = Context.EntityId;
            }

            StoreContents(new List<Bdc.ContentItem> { content }, entityId);
        }

        /// <summary>
        /// Stores the specified piece of content and associated resources
        /// </summary>
        /// <param name="content">The content to store.</param>
        public void StoreContent(Bdc.ContentItem content, string EntityId, string ignoreCategory)
        {
            StoreContents(new List<Bdc.ContentItem> { content }, EntityId, ignoreCategory: ignoreCategory);
        }

        /// <summary>
        /// Stores the specified piece of content and associated resources
        /// </summary>
        /// <param name="content">The content to store.</param>
        /// <param name="storeLocked">bool flag to indicate whether to store a flagged item.</param>
        public void StoreContent(Bdc.ContentItem content, bool storeLocked)
        {
            StoreContents(new List<Bdc.ContentItem> { content }, Context.EntityId, storeLocked);
        }

        /// <summary>
        /// Stores the specified piece of content and associated resources
        /// </summary>
        /// <param name="content">The content to store.</param>
        /// <param name="storeLocked">bool flag to indicate whether to store a flagged item.</param>
        public void StoreContent(Bdc.ContentItem content, string entityId, bool storeLocked)
        {
            var applicableEntityId = string.IsNullOrEmpty(entityId) ? Context.EntityId : entityId;
            StoreContents(new List<Bdc.ContentItem> { content }, applicableEntityId, storeLocked);
        }

        /// <summary>
        /// Copy content item to other entity
        /// </summary>
        /// <param name="content"></param>
        /// <param name="entityId"></param>
        /// <param name="targetEntityid"></param>
        /// <param name="lockedCourseType"></param>
        public void CopyItemToAnotherEntity(Bdc.ContentItem content, string entityId, string targetEntityid, string lockedCourseType)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyItemToAnotherEntity(content, entityId={0}, targetentityid={1}) content.Id={2}, lockedCourseType={3}", entityId, targetEntityid, content.Id, lockedCourseType ?? ""))
            {
                var from = GetRawItem(entityId, content.Id);
                if (from != null)
                {
                    HtmlXmlHelper.SwitchAttributeName(from.Root, "id", "itemid", content.Id);
                    HtmlXmlHelper.SwitchAttributeName(from.Root, "actualentityid", "entityid", targetEntityid);

                    if (lockedCourseType != null)
                    {
                        AddDataElement(from.Root, "bfw_locked", lockedCourseType);
                    }

                    var putCmd = new PutRawItem()
                    {
                        ItemDoc = from
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(putCmd);
                }
                else
                {
                    throw new Exception("CopyItemToAnotherEntity: Could not find item to copy.");
                }
            }
        }

        /// <summary>
        /// Copy content resource to other entity
        /// </summary>
        /// <param name="content"></param>
        /// <param name="targetEntityid"></param>
        public void CopyResourceToAnotherEntity(Bdc.ContentItem content, string targetEntityid)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyResourceToAnotherEntity(content, targetentityid={0}) content.Id={1}", targetEntityid, content.Id))
            {
                if (content.Resources != null)
                {
                    foreach (Bdc.Resource res in content.Resources)
                    {
                        res.EntityId = targetEntityid;
                        StoreResources(new List<Bdc.Resource> { res });
                    }
                }
            }
        }

        /// <summary>
        /// Copy content resource to other entity
        /// </summary>
        /// <param name="content"></param>
        /// <param name="targetEntityid"></param>
        public void CopyResourceToAnotherDomain(string destinationDomainid, string destinationPath, string sourcePath, string sourceDomain)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyResourceToAnotherEntity(destinationDomainid={0}, destinationPath={1}, sourcePath={2}, sourceDomain={3})", destinationDomainid, destinationPath, sourcePath, sourceDomain))
            {
                if (destinationPath.IsNullOrEmpty())
                {
                    destinationPath = "";
                }
                if (destinationDomainid != null && sourcePath != null != null && sourceDomain != null)
                {
                    var cmdCopyResource = new CopyResources()
                    {
                        SourceEntityId = sourceDomain,
                        DestEntityId = destinationDomainid,
                        SourcePath = sourcePath,
                        DestPath = destinationPath

                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdCopyResource);
                }
            }
        }



        /// <summary>
        /// Copy content item to other entity
        /// </summary>
        /// <param name="content"></param>
        /// <param name="entityId"></param>
        /// <param name="targetEntityid"></param>
        public void CopyItemToAnotherEntity(ContentItem content, string entityId, string targetEntityid)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyItemToAnotherEntity(content, entityId={0}, targetentityid={1}) content.Id={2}", entityId, targetEntityid, content.Id))
            {
                CopyItemToAnotherEntity(content, entityId, targetEntityid, null);
            }
        }

        /// <summary>
        /// Create and save a new content item that is identical to the first, but which has a new ID
        /// This is necessary so that we can create copies or overwrite existing items, but without
        /// regenerating the content
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="fromItemId">From item ID.</param>
        /// <param name="toItemId">To item id.</param>
        public void CopyItem(string entityId, string fromItemId, string toItemId)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyItem(entityid={0}, fromItemId={1}, toItemId={2})", entityId, fromItemId, toItemId))
            {
                var item = CopyItemXml(entityId, fromItemId, toItemId);

                var putCmd = new PutRawItem()
                {
                    ItemDoc = item
                };

                SessionManager.CurrentSession.Execute(putCmd);
            }
        }

        /// <summary>
        /// Copies an existing item and adds it to the speicified categories and parentid.
        /// </summary>
        /// <param name="entityId">Id of the entity to which the item should belong.</param>
        /// <param name="fromItemId">Id of the item to copy.</param>
        /// <param name="toItemId">Id of the new item.</param>
        /// <param name="categories">Categories to add the item to.</param>
        /// <param name="parentId">Default category parent item id.</param>
        /// <param name="removeDesc">Flag to remove Description of item during copy. default is false</param>
        /// <param name="hiddenFromStudent">Flag enabling/disabling visibility to students for template</param>
        public Bdc.ContentItem CopyItem(string entityId, string fromItemId, string toItemId, string parentId, IEnumerable<Bdc.TocCategory> categories,
            bool removeDesc = false, string title = null, string subTitle = null, string description = null, bool? hiddenFromStudent = null, bool? includePoints = null)
        {
            Bdc.ContentItem item = null;
            using (Context.Tracer.DoTrace("ContentActions.CopyItem(entityid={0}, fromItemId={1}, toItemId={2}, parentId={3}, categories)", entityId, fromItemId, toItemId, parentId))
            {
                var itemXml = CopyItemXml(entityId, fromItemId, toItemId, removeDesc);
                var data = itemXml.Root.Element(ElStrings.Data);

                if (data != null)
                {
                    var parentElem = data.Element(ElStrings.parent);

                    if (parentElem == null)
                    {
                        parentElem = new XElement(ElStrings.parent);
                        data.Add(parentElem);
                    }

                    parentElem.Value = parentId;

                    if (includePoints.HasValue && !includePoints.Value)
                    {
                        var maxpoints = data.Element(ElStrings.maxpoints);

                        if (maxpoints != null)
                        {
                            data.Element(ElStrings.maxpoints).Remove();
                        }
                        var gradable = data.Element(ElStrings.gradable);
                        if (gradable != null)
                        {
                            data.Element(ElStrings.gradable).Remove();

                        }
                    }

                    if (hiddenFromStudent.HasValue && !hiddenFromStudent.Value)
                    {
                        var propertiesElem = data.Element(ElStrings.bfw_properties);

                        if (propertiesElem == null)
                        {
                            propertiesElem = new XElement(ElStrings.bfw_properties);
                            data.Add(propertiesElem);
                        }

                        var visibilityElem = propertiesElem.XPathSelectElement("./bfw_property[@name='bfw_visibility']");

                        if (visibilityElem == null)
                        {
                            visibilityElem = new XElement(ElStrings.Bfw_Property);
                            visibilityElem.Name = "bfw_visibility";
                            visibilityElem.SetAttributeValue("type", "string");

                            propertiesElem.Add(visibilityElem);
                        }

                        visibilityElem.Value = "<roles><instructor /><student /></roles>";
                    }

                    if (title != null)
                    {
                        var titleElem = data.Element(ElStrings.title);

                        if (titleElem == null)
                        {
                            titleElem = new XElement(ElStrings.title);
                            data.Add(titleElem);
                        }

                        titleElem.Value = title;
                    }

                    if (subTitle != null)
                    {
                        var subTitleElem = data.Element(ElStrings.subtitle);

                        if (subTitleElem == null)
                        {
                            subTitleElem = new XElement(ElStrings.subtitle);
                            data.Add(subTitleElem);
                        }

                        subTitleElem.Value = subTitle;
                    }

                    if (description != null)
                    {
                        var descriptionElem = data.Element(ElStrings.description);

                        if (descriptionElem == null)
                        {
                            descriptionElem = new XElement(ElStrings.description);
                            data.Add(descriptionElem);
                        }

                        descriptionElem.Value = description;
                    }

                    BizEntityExtensions.SetupDefaultParent(data, parentId, string.Empty);

                    if (!categories.IsNullOrEmpty())
                    {
                        BizEntityExtensions.StoreTocCategories(data, categories);
                    }

                    XElement metaDataXml = data.XPathSelectElement("./bfw_metadata_admin");

                    if (metaDataXml != null)
                    {
                        metaDataXml.Remove();
                    }
                }

                var cmd = new PutRawItem()
                {
                    ItemDoc = itemXml
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                var agx = new Adc.Item();
                agx.ParseEntity(itemXml.Root);
                item = agx.ToContentItem(Context);
            }

            return item;
        }

        /// <summary>
        /// Given two existing items, copy everything from the 'from' item to the 'to' item,
        /// excluding all attributes PX is aware of.  I.e., copy only Agilix-specific attributes.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="fromId">From ID.</param>
        /// <param name="toId">To ID.</param>
        public void CopyItemSettings(string entityId, string fromId, string toId)
        {
            using (Context.Tracer.DoTrace("ContentActions.CopyItemSettings(entityId={0}, fromId={1}, toId={2})", entityId, fromId, toId))
            {
                var fromCmd = new GetRawItem()
                {
                    ItemId = fromId,
                    EntityId = entityId
                };
                var toCmd = new GetRawItem()
                {
                    ItemId = toId,
                    EntityId = entityId
                };
                // This is ugly, but we need to delete the item, because stuff won't go away in DLAP just by
                // removing the elements when you do the PutItems command
                var deleteCmd = new DeleteItems()
                {
                    Items = new List<Adc.Item>() 
                    { 
                        new Adc.Item() 
                        { 
                            Id=toId,
                            EntityId = entityId
                        }
                    }
                };

                var batch = new Batch();
                batch.Add(fromCmd);
                batch.Add(toCmd);
                batch.Add(deleteCmd);

                SessionManager.CurrentSession.Execute(batch);

                var from = fromCmd.ItemDocument;
                if (from == null)
                {
                    throw new Exception("Can't copy settings from an item that does not exist");
                }

                var to = toCmd.ItemDocument;
                if (to == null)
                {
                    throw new Exception("Can't copy settings to an item that does not exist");
                }

                // Get the list of elements that we want to keep (the whitelist) when copying settings.  These are the things
                // that we manage ourselves, and we get the list from the element strings (ElStrings) object on the Item class.
                IEnumerable<string> protectedElements = Adc.ElStrings.SettingsWhiteList;

                // copy all whitelisted items in to, into from, so as to overwrite them
                var toData = to.Root.Element("data");
                var fromData = from.Root.Element("data");
                foreach (var element in protectedElements)
                {
                    var elementPath = string.Format("//{0}", element);
                    var inTo = toData.XPathSelectElement(elementPath);
                    var inFrom = toData.XPathSelectElement(elementPath);

                    if (inTo != null)
                    {
                        inFrom.TryRemove();
                        fromData.Add(inTo);
                    }
                }

                HtmlXmlHelper.SwitchAttributeName(from.Root, "id", "itemid", toId);
                HtmlXmlHelper.SwitchAttributeName(from.Root, "actualentityid", "entityid", entityId);
                var putCmd = new PutRawItem()
                {
                    ItemDoc = from
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(putCmd);
            }
        }

        /// <summary>
        /// Stores the collection of contents and associated resources. Note that each item is 
        /// first loaded to ensure that all necessary properties are being persisted. This should
        /// be refactored so that properties not load is required.
        /// </summary>
        /// <param name="contents">The content items to store.</param>
        public void StoreContents(IEnumerable<Bdc.ContentItem> contents)
        {
            StoreContents(contents, Context.EntityId);
        }

        /// <summary>
        /// Stores the collection of contents and associated resources. Note that each item is 
        /// first loaded to ensure that all necessary properties are being persisted. This should
        /// be refactored so that properties not load is required.
        /// </summary>
        /// <param name="contents">The content items to store.</param>
        public void StoreContents(IEnumerable<Bdc.ContentItem> contents, string entityId)
        {
            StoreContents(contents, entityId, true);
        }

        /// <summary>
        /// Stores the collection of contents and associated resources. Note that each item is 
        /// first loaded to ensure that all necessary properties are being persisted. This should
        /// be refactored so that properties not load is required.
        /// </summary>
        /// <param name="contents">The content items to store.</param>
        public void StoreContents(IEnumerable<Bdc.ContentItem> contents, string entityId, string ignoreCategory)
        {
            StoreContents(contents, entityId, true, ignoreCategory);
        }

        /// <summary>
        /// Stores the collection of contents and associated resources. Note that each item is 
        /// first loaded to ensure that all necessary properties are being persisted. This should
        /// be refactored so that properties not load is required.
        /// </summary>
        /// <param name="contents">the contents ti store</param>
        /// <param name="entityId">entity id</param>
        /// <param name="saveLocked">flag to allow storing locked items</param>
        private void StoreContents(IEnumerable<Bdc.ContentItem> contents, string entityId, bool saveLocked, 
            string ignoreCategory = "")
        {
            var resources = new List<Bdc.Resource>();
            var items = new List<Adc.Item>();

            using (Context.Tracer.DoTrace("ContentActions.StoreContents(contents, entityId={0})", entityId))
            {
                var batch = new Batch();

                foreach (var content in contents)
                {
                    batch.Add(content.Id, new GetItems()
                    {
                        SearchParameters = new Adc.ItemSearch()
                        {
                            EntityId = entityId,
                            ItemId = content.Id
                        }
                    });
                }

                SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                using (Context.Tracer.DoTrace("Loading Items To Store"))
                {
                    foreach (var content in contents)
                    {
                        if (content.Resources != null)
                        {
                            resources.AddRange(content.Resources);
                        }

                        var currentCmd = batch.CommandAs<GetItems>(content.Id);

                        Bdc.ContentItem current = null;
                        if (currentCmd != null && !currentCmd.Items.IsNullOrEmpty())
                        {
                            current = currentCmd.Items.First().ToContentItem(Context);

                            if (current != null)
                            {
                            	if (!current.Categories.IsNullOrEmpty())
	                            {
	                                string category = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
	                                if (!ignoreCategory.Equals(category) && current.Categories.ToList().Find(i => i.Id == category) != null)
	                                {
                                        content.AddCategoryToItem(category, null, null);
	                                }
	                            }
                                PersistProperties(content, current);
                                PersistContainers(content, current);
                            }
                        }

                        //modify due dates to comply with timezone
                        if (content.AssignmentSettings != null && content.AssignmentSettings.DueDate.Year > DateTime.MinValue.Year)
                        {
                            if (TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone) != TimeZoneInfo.Local)
                            {
                                content.AssignmentSettings.DueDateTZ = new Bfw.Common.DateTimeWithZone(content.AssignmentSettings.DueDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                                content.AssignmentSettings.StartDateTZ = new Bfw.Common.DateTimeWithZone(content.AssignmentSettings.StartDate, TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone), false);
                                content.AssignmentSettings.DueDate = content.AssignmentSettings.DueDateTZ.UniversalTime;
                                content.AssignmentSettings.StartDate = content.AssignmentSettings.StartDateTZ.UniversalTime;
                            }
                        }
                        var item = content.ToItem();
                        item.EntityId = entityId;

                        items.Add(item);
                    }
                }

                using (Context.Tracer.DoTrace("Storing Resources"))
                {
                    StoreResources(resources);
                }

                using (Context.Tracer.DoTrace("Storing Items"))
                {
                    if (!items.IsNullOrEmpty())
                    {
                        items.AddRange(AdjustGroups(entityId, items));

                        var cmd = new PutItems();
                        cmd.Add(items);
                        cmd.RunAsync = true;

                        SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                    }
                }
            }
            Context.CacheProvider.InvalidateLaunchPadData(Context.CourseId);
        }

        /// <summary>
        /// Adjusts items for groups/individuals with distinct duedates
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private List<Item> AdjustGroups(string entityId, List<Item> items)
        {
            var originalItems = new List<Item>();

            items.ForEach(i =>
            {
                if (i.EntityId != Context.CourseId)
                {
                    var originalItem = GetContent(Context.CourseId, i.Id).ToItem();

                    if (originalItem != null)
                    {
                        if (originalItem.DueDate == i.DueDate)
                        {
                            if (originalItem.AdjustedGroups.Contains(entityId))
                            {
                                i.AdjustedGroups.Remove(entityId);
                                originalItem.AdjustedGroups.Remove(entityId);
                                originalItems.Add(originalItem);
                            }
                        }
                        else
                        {
                            if (!originalItem.AdjustedGroups.Contains(entityId))
                            {
                                originalItem.AdjustedGroups.Add(entityId);
                                originalItems.Add(originalItem);
                            }
                        }
                    }
                }
            });

            return originalItems;
        }

        /// <summary>
        /// Copies the container collection from one content item to another.
        /// </summary>
        /// <param name="contentfromModel">The contentfrom model.</param>
        /// <param name="content">The content.</param>
        private void PersistContainers(ContentItem content, ContentItem current)
        {
            if (content.Containers == null)
            { // init the containers obj
                content.Containers = new List<Bdc.Container>();
            }

            if (!current.Containers.IsNullOrEmpty())
            {
                if (content.Containers.IsNullOrEmpty())
                    // just add the containers from current because we know there will not be any conflicks
                    content.Containers.AddRange(current.Containers);
                else
                {
                    var copiedContainers = new List<Bdc.Container>();

                    // make sure the Toc does not already exist and add all other containers
                    foreach (var curContainer in current.Containers)
                    {
                        if (!content.Containers.Exists(o => o.Toc.Equals(curContainer.Toc)))
                        {
                            copiedContainers.Add(curContainer);
                        }
                    }

                    content.Containers.AddRange(copiedContainers);
                }
            }
        }

        /// <summary>
        /// Stores a collection of resources into the system.
        /// </summary>
        /// <param name="resources">The resources to store.</param>
        public void StoreResources(IEnumerable<Bdc.Resource> resources)
        {
            using (Context.Tracer.DoTrace("ContentActions.StoreResources(resources)"))
            {
                foreach (var resource in resources.Map(r => r.ToResource()))
                {
                    var cmd = new PutResource()
                    {
                        Resource = resource
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                    Context.CacheProvider.InvalidateResource(resource.EntityId, resource.Url);
                }
            }
        }

        /// <summary>
        /// Removes the specified resource from the entity
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="resourcePath">The resource path.</param>
        public void RemoveResource(string entityId, string resourcePath)
        {
            RemoveResources(new List<Bdc.Resource>() { new Bdc.Resource() { EntityId = entityId, Url = resourcePath } });
        }

        /// <summary>
        /// Removes a list of resources for the same entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="resourcePathList">List of resource paths.</param>
        public void RemoveResources(string entityId, string[] resourcePathList)
        {
            if (resourcePathList != null)
            {
                var list = new List<Bdc.Resource>();
                foreach (var uri in resourcePathList)
                {
                    list.Add(new Bdc.Resource() { EntityId = entityId, Url = uri });
                }
                RemoveResources(list);
            }
        }

        /// <summary>
        /// Removes the specified resources from the system.
        /// </summary>
        /// <param name="resources">The resources to remove.</param>
        public void RemoveResources(IList<Bdc.Resource> resources)
        {
            using (Context.Tracer.StartTrace("ContentActions.RemoveResources(resources)"))
            {
                var cmd = new DeleteResources()
                {
                    ResourcesToDelete = resources.Map(r => r.ToResource()).ToList()
                };

                SessionManager.CurrentSession.Execute(cmd);
            }
        }

        /// <summary>
        /// Removes the resources with given IDs.
        /// </summary>
        /// <param name="resourceIds">The resource IDs.</param>
        public void RemoveResources(IEnumerable<string> resourceIds)
        {
            using (Context.Tracer.StartTrace("ContentActions.RemoveResources(resourceIds)"))
            {
                if (!resourceIds.IsNullOrEmpty())
                {
                    var resourceList = ListResources(resourceIds, Context.EnrollmentId).ToList();

                    if (resourceList.Any())
                    {
                        RemoveResources(resourceList);
                    }
                }
            }
        }

   




        /// <summary>
        /// Removes the specified content from the system.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        [Obsolete("Depends on IResourceMap", false)]
        public void RemoveContent(string entityId, string itemId)
        {
            RemoveContents(entityId, new List<string>() { itemId });
        }

        /// <summary>
        /// Removes items with the specified IDs from the course/entity.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        [Obsolete("Depends on IResourceMap", false)]
        public void RemoveContents(string entityId, IList<string> itemId)
        {
            using (Context.Tracer.DoTrace("ContentActions.RemoveContents(entityId={0}, itemId)", entityId))
            {
                var cmd = new DeleteItems()
                {
                    Items = itemId.Map(i => { ResourceMapActions.DeleteMap(i); return i; }).Map(i => new Adc.Item() { EntityId = entityId, Id = i }).ToList()
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        /// <summary>
        /// Marks the content as read by logging activity against it in Agilix.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        public void MarkContentAsRead(string itemId)
        {
            using (Context.Tracer.DoTrace("ContentActions.MarkContentAsRead(itemId={0})", itemId))
            {
                if (!string.IsNullOrEmpty(Context.EnrollmentId))
                {
                    //var cmd = new PutItemActivity()
                    //{
                    //    ItemId = itemId,
                    //    EnrollmentId = Context.EnrollmentId,
                    //    Seconds = 10, 
                    //    Attempts = 1,
                    //    StartTime = DateTime.UtcNow
                    //};

                    //SessionManager.CurrentSession.Execute(cmd);

                    var submission = new Bdc.Submission
                    {
                        ItemId = itemId,
                        Body = "",
                        SubmissionType = Bdc.SubmissionType.Assignment,
                        SubmittedDate = DateTime.Today,
                        EnrollmentId = Context.EnrollmentId
                    };

                    using (Context.Tracer.DoTrace("AddStudentSubmission(entityId={0}, submission={1})", Context.EntityId, submission))
                    {
                        var cmd2 = new PutStudentSubmission()
                        {
                            EntityId = Context.EntityId,
                            Submission = submission.ToSubmission()
                        };
                        cmd2.Submission.EnrollmentId = Context.EnrollmentId;
                        try
                        {
                            SessionManager.CurrentSession.Execute(cmd2);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tracks time student spent on the item
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        public void StoreContentDuration(string itemId, string enrollmentId, int durationInSeconds, DateTime? startDate)
        {
            using (Context.Tracer.DoTrace("ContentActions.StoreContentDuration(itemId={0}, enrollmentId={1}, durationInSeconds={2}, startDate={3})", itemId, enrollmentId, durationInSeconds, startDate))
            {
                if (!string.IsNullOrEmpty(Context.EnrollmentId))
                {
                    var cmd = new PutItemActivity()
                    {
                        ItemId = itemId,
                        EnrollmentId = enrollmentId,
                        Seconds = durationInSeconds,
                        StartTime = startDate
                    };

                    SessionManager.CurrentSession.Execute(cmd);
                }
            }
        }

        /// <summary>
        /// Lists the content read by the active user, sorted by date.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListContentReadByDate()
        {
            IEnumerable<Bdc.ContentItem> result = null;

            using (Context.Tracer.DoTrace("ContentActions.ListContentReadByDate"))
            {
                if (!string.IsNullOrEmpty(Context.EnrollmentId))
                {
                    var activityCmd = new GetEnrollmentActivity()
                    {
                        SearchParameter = new Adc.EnrollmentActivitySearch()
                        {
                            EnrollmentId = Context.EnrollmentId
                        }
                    };

                    SessionManager.CurrentSession.Execute(activityCmd);

                    var activity = activityCmd.Activity.OrderByDescending(a => a.StartTime);
                    var distinct = activity.Distinct((x, y) => x.ItemId == y.ItemId).ToList();
                    var readDates = new Dictionary<string, DateTime>();

                    var batch = new Batch();
                    foreach (var act in distinct)
                    {
                        if (!readDates.ContainsKey(act.ItemId))
                        {
                            var cmd = new GetItems()
                            {
                                SearchParameters = new Adc.ItemSearch()
                                {
                                    EntityId = Context.EntityId,
                                    ItemId = act.ItemId
                                }
                            };

                            batch.Add(act.ItemId, cmd);
                            readDates.Add(act.ItemId, act.StartTime);
                        }
                    }

                    using (Context.Tracer.DoTrace("Get Items"))
                    {
                        if (!batch.Commands.IsNullOrEmpty())
                        {
                            SessionManager.CurrentSession.Execute(batch);

                            result = batch.Commands.Map(cmd => cmd as GetItems).Map(cmd =>
                            {
                                var item = cmd.Items.First().ToContentItem(Context);
                                item.ReadDate = readDates[item.Id];
                                return item;
                            });
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the collection of supported template items for the given course/entityid.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetAllTemplates()
        {
            IEnumerable<Bdc.ContentItem> result = null;

            result =
                Context.CacheProvider.FetchCourseItem(Context.CourseId, "GetAllTemplates") as
                IEnumerable<Bdc.ContentItem>;

            if (result == null)
            {
                using (Context.Tracer.DoTrace("ContentActions.GetAllTemplates"))
                {
                    result = ListChildren(Context.EntityId, TemplateFolder.ToLowerInvariant(), 1, Constants.USE_AGILIX_PARENT);
                }
                Context.CacheProvider.StoreCourseItem("GetAllTemplates", Context.CourseId, result.ToList());
            }
            return result;
        }

        /// <summary>
        /// Add all templates with types and subtypes that are the same as the given item.
        /// ALSO: if the item is a quiz, homeworks are related, and vice-versa.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> FindRelatedTemplates(string itemId)
        {
            var relatedTemplates = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.FindRelatedTemplates(itemId={0})", itemId))
            {
                var item = GetContent(Context.EntityId, itemId);
                if (item == null)
                    return new List<Bdc.ContentItem>();

                var allTemplates = GetAllTemplates();

                foreach (var template in allTemplates)
                {
                    if (template.Type.Equals("homework", StringComparison.CurrentCultureIgnoreCase) || template.Type.Equals("assessment", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (item.Type.Equals("homework", StringComparison.CurrentCultureIgnoreCase) || item.Type.Equals("assessment", StringComparison.CurrentCultureIgnoreCase))
                        {
                            relatedTemplates.Add(template);
                        }
                    }
                    else if (template.Type == item.Type && template.Subtype == item.Subtype)
                    {
                        relatedTemplates.Add(template);
                        break;
                    }
                }
            }

            return relatedTemplates;
        }

        /// <summary>
        /// Add all templates with types and subtypes that are the same as the given item.
        /// ALSO: if the item is a quiz, home works are related, and vice-versa.
        /// </summary>
        /// <param name="contentType"> </param>
        /// <returns></returns>
        public Bdc.ContentItem FindTemplateForType(string contentType)
        {

            using (Context.Tracer.DoTrace("ContentActions.FindTemplateForType(contentType={0})", contentType))
            {
                var allTemplates = GetAllTemplates();

                foreach (var local in
                         from template in allTemplates
                         where template.Title != null && template.Title.ToLowerInvariant().Contains(contentType.ToLowerInvariant())
                         select GetContent(Context.EntityId, template.Id))
                {
                    return local;
                }
            }

            return null;
        }

        /// <summary>
        /// Lists all items related to itemId by taxonomy.
        /// </summary>
        /// <param name="itemId">ID of the item to find related items for.</param>
        /// <param name="entityId">ID of the entity the taxonomy relationship should exist in.</param>
        /// <returns>
        /// Enumeration of all relationships to itemId by taxonomy.
        /// </returns>
        public IEnumerable<Bdc.TaxonomyRelationship> FindRelatedItems(string itemId, string entityId)
        {
            var relationships = new List<Bdc.TaxonomyRelationship>();

            using (Context.Tracer.DoTrace("ContentActions.FindRelatedItems(itemId={0}, entityId={1})", itemId, entityId))
            {
                Db.ConfigureConnection("PXTaxonomy");

                try
                {
                    Db.StartSession();
                    var records = Db.Query("LoadRelatedItems @0, @1", itemId, entityId);

                    if (!records.IsNullOrEmpty())
                    {
                        relationships = records.Map(r => new Bdc.TaxonomyRelationship()
                        {
                            ItemId = r.String("ItemId"),
                            ItemTitle = r.String("ItemTitle"),
                            RelatedItemId = r.String("RelatedItemId"),
                            TaxonomyId = r.String("TaxonomyId"),
                            TaxonomyTitle = r.String("TaxonomyTitle"),
                            ScopeId = r.String("ScopeId")
                        }).ToList();
                    }
                }
                finally
                {
                    Db.EndSession();
                }
            }

            return relationships;
        }

        /// <summary>
        /// Gets gradebook detail for the specified user.
        /// </summary>
        /// <param name="userId">ID of the user for which to get grades.</param>
        /// <param name="entityId">Optional entity ID by which to filter the returned data.</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Grade> GetGradesPerItem(IList<Bdc.ContentItem> items, string entityId)
        {
            List<DataContracts.Grade> grades = new List<Grade>();

            using (Context.Tracer.DoTrace("GetGradesPerItem(items, entityId={0})", entityId))
            {
                if (!items.IsNullOrEmpty())
                {
                    var itemIds = items.Map(i => i.Id).ToArray<string>();
                    string itemPipeList = string.Join("|", itemIds);

                    var cmd = new GetGrades();

                    if (Context.AccessLevel == AccessLevel.Instructor)
                    {
                        cmd = new GetGrades
                        {
                            SearchParameters = new Adc.GradeSearch
                            {
                                ItemIds = itemIds,
                                EntityId = entityId
                            }
                        };
                    }
                    else if (Context.AccessLevel == AccessLevel.Student)
                    {
                        cmd = new GetGrades
                        {
                            SearchParameters = new Adc.GradeSearch
                            {
                                ItemIds = itemIds,
                                EnrollmentId = Context.EnrollmentId
                            }
                        };
                    }
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                    if (!cmd.Enrollments.IsNullOrEmpty())
                    {
                        var gradeCollection = cmd.Enrollments.SelectMany(e => e.ItemGrades).Map(g => g.ToGrade());
                        var currentUserGrades = cmd.Enrollments.Where(e => e.User.Id == Context.CurrentUser.Id).SelectMany(e => e.ItemGrades).Map(g => g.ToGrade()); ;

                        foreach (Bdc.ContentItem item in items)
                        {
                            var itemGrades = gradeCollection.Where(g => g.GradedItem.Id == item.Id);

                            if (!itemGrades.IsNullOrEmpty())
                            {
                                Bdc.GradebookInfo bookInfo = new Bdc.GradebookInfo()
                                {
                                    AverageScore = 0.0,
                                    TotalSubmissions = 0,
                                    TotalGrades = 0,
                                    ItemId = item.Id,
                                    LastScore = 0.0,
                                    IsUserSubmitted = false,
                                    IsUserGraded = false
                                };

                                var hasBeenSubmittedorGraded = itemGrades.Where(g => (g.SubmittedDate.HasValue && g.SubmittedDate.Value.Year > DateTime.MinValue.Year) || (g.ScoredDate.HasValue && g.ScoredDate.Value.Year > DateTime.MinValue.Year));
                                var hasBeenGraded = hasBeenSubmittedorGraded.Where(g => g.ScoredDate.HasValue && g.ScoredDate.Value.Year > DateTime.MinValue.Year && g.ScoredVersion == g.SubmittedVersion);

                                if (!hasBeenGraded.IsNullOrEmpty())
                                {
                                    bookInfo.AverageScore = hasBeenGraded.Average(g => g.Achieved);
                                    bookInfo.TotalGrades = hasBeenGraded.Count();
                                }

                                if (!hasBeenSubmittedorGraded.IsNullOrEmpty())
                                {
                                    bookInfo.TotalSubmissions = hasBeenSubmittedorGraded.Count();
                                }

                                if (currentUserGrades.Any(g => g.GradedItem.Id == item.Id && (g.Status & Bdc.GradeStatus.Completed) == Bdc.GradeStatus.Completed))
                                { //only mark as submitted if item has been completed
                                    bookInfo.IsUserSubmitted = true;
                                    //special case for learning curve:
                                    if (item.FacetMetadata.ContainsKey("meta-content-type") &&
                                        item.FacetMetadata["meta-content-type"].Contains("LearningCurve"))
                                    {
                                        var gradedItem = currentUserGrades.FirstOrDefault(g => g.GradedItem.Id == item.Id);

                                        if (gradedItem != null && gradedItem.Achieved <= 0.0)
                                        {
                                            bookInfo.IsUserSubmitted = false;
                                        }
                                    }
                                }
                                bool hasReleasedGrade = currentUserGrades.Any(g =>
                                {
                                    if (g.GradedItem.Id == item.Id &&
                                        (g.Status & Bdc.GradeStatus.ShowScore) == Bdc.GradeStatus.ShowScore)
                                    {
                                        return ((g.Status & Bdc.GradeStatus.Released) == Bdc.GradeStatus.Released) ||
                                               item.AssignmentSettings.GradeReleaseDate.ToUniversalTime() <= DateTime.UtcNow;
                                    }
                                    return false;

                                });

                                if (hasReleasedGrade)
                                { //set score to achieved if the score is visible
                                    var gradedItem = currentUserGrades.Where(g => g.GradedItem.Id == item.Id && g.ScoredDate.HasValue && g.ScoredDate.Value.Year > DateTime.MinValue.Year);

                                    if (!gradedItem.IsNullOrEmpty())
                                    {
                                        bookInfo.IsUserGraded = true;
                                        bookInfo.LastScore = Math.Floor(gradedItem.First().Achieved * 100) / 100;
                                        grades.AddRange(gradedItem);
                                    }
                                }
                                else
                                {//set score to -1 if the score isn't visible
                                    bookInfo.LastScore = -1;
                                    bookInfo.IsUserGraded = false;
                                }

                                item.GradebookInfo = bookInfo;

                            }
                        }
                    }
                }
            }

            return grades;
        }

        public List<ContentItem> ListContentForCourseMaterials(string entityId)
        {

            List<ContentItem> results = new List<ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContentForCourseMaterials(entityId={0})", entityId))
            {
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='PX_COURSE_MATERIALS'"
                });

                using (Context.Tracer.DoTrace("Map Results"))
                {
                    results.AddRange(itemList.Map(item => item.ToContentItem(Context)));
                }
            }

            return results;

        }

        /// <summary>
        /// List Content for Dropbopx
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ContentItem> ListContentForDropBox(string entityId, string itemid, string type)
        {

            List<ContentItem> results = new List<ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContentForCourseMaterials(entityId={0})", entityId))
            {
                var itemList = FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='" + itemid + "'AND/bfw_type='" + type + "'"
                });

                using (Context.Tracer.DoTrace("Map Results"))
                {
                    results.AddRange(itemList.Map(item => item.ToContentItem()));
                }
            }

            return results;

        }


        /// <summary>
        /// Returns assest items corresponding to the resource items
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public List<ContentItem> ListAssestsForCourseMaterials(string entityID, List<ContentItem> results)
        {

            //get the asset items for all the uploaded documents
            List<ContentItem> uploadResults = new List<ContentItem>();
            if (results.Count == 0)
                return uploadResults;

            using (Context.Tracer.DoTrace("ContentActions.ListAssestsForCourseMaterials(entityID={0}, results)", entityID))
            {
                Batch batch = new Batch();
                foreach (var r in results)
                {
                    batch.Add(new GetItems()
                    {
                        SearchParameters = new ItemSearch
                        {
                            EntityId = entityID,
                            Query = String.Format("/parent='{0}'", r.Id)
                        }
                    });
                }

                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                for (int i = 0; i < results.Count; i++)
                {
                    uploadResults.AddRange(batch.CommandAs<GetItems>(i).Items.Map(item => item.ToContentItem(Context)));
                }
            }

            return uploadResults;
        }

        /// <summary>
        /// Retrieves all content that should be loaded into the assignment center's left hand side.
        /// </summary>
        /// <param name="entityId">Id of the entity the assignment center is located in.</param>
        /// <returns>All assignment center content</returns>
        public IEnumerable<Biz.DataContracts.ContentItem> ListContentForAssignmentCenter(string entityId)
        {
            var results = new List<Biz.DataContracts.ContentItem>();

            using (Context.Tracer.DoTrace("ContentActions.ListContentForAssignmentCenter(entityId={0})", entityId))
            {
                var itemList = FindItems(new ItemSearch
                {
                    EntityId = entityId,
                    Query = "/bfw_in_assignment_center=true OR /meta-bfw_assigned=true"
                });

                using (Context.Tracer.DoTrace("Map Results"))
                {
                    results.AddRange(itemList.Map(item => item.ToContentItem(Context)));
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="items"></param>
        public void UnAssignAssignmentCenterItems(string categoryId,
            IEnumerable<Biz.DataContracts.AssignmentCenterItem> items, string toc, bool keepInGradebook)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateAssignmentCenterItems(categoryId={0}, items)", categoryId))
            {
                if (!items.IsNullOrEmpty())
                {
                    var getBatch = new Batch();
                    var putItems = new PutItems();
                    putItems.RunAsync = true;

                    using (Context.Tracer.DoTrace("Load Content Items"))
                    {
                        foreach (var item in items)
                        {
                            getBatch.Add(item.Id, new GetItems()
                            {
                                SearchParameters = new Adc.ItemSearch()
                                {
                                    EntityId = Context.EntityId,
                                    ItemId = item.Id
                                }
                            });
                        }

                        SessionManager.CurrentSession.ExecuteAsAdmin(getBatch);
                    }

                    using (Context.Tracer.StartTrace("Modify Content Items"))
                    {
                        Bdc.ContentItem current = null;
                        foreach (var item in items)
                        {
                            if (getBatch.CommandAs<GetItems>(item.Id).Items.Count == 0)
                            {
                                //item not found in DLAP
                                continue;
                            }

                            current = getBatch.CommandAs<GetItems>(item.Id).Items.First().ToContentItem(Context);

                            // make sure the item is part of the correct syllabus filter
                            if (string.IsNullOrEmpty(item.ParentId))
                            {
                                item.ParentId = categoryId;
                            }

                            current.Properties["bfw_syllabusfilter"] = new Bdc.PropertyValue
                            {
                                Type = Bfw.PX.Biz.DataContracts.PropertyType.String,
                                Value = item.ParentId
                            };

                            if (string.IsNullOrWhiteSpace(item.PreviousParentId))
                            {
                                item.PreviousParentId = item.ParentId;
                            }

                            // update the appropriate category and sequence in case it has changed
                            current.SetSyllabusFilterCategory(item.ParentId, toc, item.Sequence);


                            // create assignment settings to add points, even if not assigned date.
                            if (current.AssignmentSettings == null)
                            {
                                current.AssignmentSettings = new Bdc.AssignmentSettings()
                                {
                                    StartDate = DateTime.MinValue,
                                    DueDate = DateTime.MinValue,
                                    meta_bfw_Assigned = false,
                                    Points = 0,
                                    Category = "-1",
                                    CategorySequence = string.Empty,
                                    IsAssignable = false
                                };
                            }

                            current.ParentId = "PX_MANIFEST";

                            current.AssignmentSettings.meta_bfw_Assigned = false;
                            current.Properties["bfw_startdate"] = new PropertyValue()
                            {
                                Type = PropertyType.DateTime,
                                Value = DateTime.MinValue
                            };
                            current.AssignmentSettings.StartDate = DateTime.MinValue;
                            current.AssignmentSettings.DueDate = DateTime.MinValue;

                            if (keepInGradebook)
                            {
                                current.AssignmentSettings.Points = item.Points;
                            }
                            else
                            {
                                current.AssignmentSettings.IsAssignable = false;
                                current.AssignmentSettings.Points = 0;
                                current.AssignmentSettings.Category = "-1";
                                current.AssignmentSettings.CategorySequence = string.Empty;
                            }

                            current.AssignmentSettings.AllowLateSubmission = false;
                            current.Properties.Remove("bfw_SendReminder");
                            current.Properties.Remove("bfw_highlightlatesubmission");
                            current.Properties.Remove("bfw_allowlatesubmissiongrace");
                            current.Properties.Remove("bfw_latesubmissiongraceduration");
                            current.Properties.Remove("bfw_latesubmissiongracedurationtype");

                            current.InAssignmentCenter = true;
                            current.WasDueDateManuallySet = false;

                            putItems.Add(current.ToItem());

                            item.StartDate = DateTime.MinValue;
                            item.EndDate = DateTime.MinValue;
                        }
                    }

                    using (Context.Tracer.StartTrace("Store Content Items"))
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(putItems);
                    }
                    Context.CacheProvider.InvalidateLaunchPadData(Context.CourseId);
                }
            }
        }

        /// <summary>
        /// Updates the given items in assignment center to reflect their current state.
        /// </summary>
        /// <param name="categoryId">id of the category the items belong to</param>
        /// <param name="items">Items to update</param>
        /// <param name="toc"></param>
        /// <param name="entityid">entity id the item belongs to</param>
        public void UpdateAssignmentCenterItems(string categoryId, IEnumerable<Biz.DataContracts.AssignmentCenterItem> items, string toc, string entityid = "")
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateAssignmentCenterItems(categoryId={0}, items)", categoryId))
            {
                if (entityid.IsNullOrEmpty())
                {
                    entityid = Context.EntityId;
                }

                if (!items.IsNullOrEmpty())
                {
                    var getBatch = new Batch();
                    getBatch.RunAsync = true;
                    var putItems = new PutItems();

                    using (Context.Tracer.DoTrace("Load Content Items"))
                    {
                        foreach (var item in items)
                        {
                            getBatch.Add(item.Id, new GetItems()
                            {
                                SearchParameters = new Adc.ItemSearch()
                                {
                                    EntityId = entityid,
                                    ItemId = item.Id
                                }
                            });
                        }

                        SessionManager.CurrentSession.ExecuteAsAdmin(getBatch);
                    }

                    using (Context.Tracer.StartTrace("Modify Content Items"))
                    {
                        Bdc.ContentItem current = null;
                        foreach (var item in items)
                        {
                            if (getBatch.CommandAs<GetItems>(item.Id).Items.Count == 0)
                            {
                                //Item not found in DLAP;
                                continue;
                            }

                            current = getBatch.CommandAs<GetItems>(item.Id).Items.First().ToContentItem(Context);

                            // make sure the item is part of the correct syllabus filter
                            if (string.IsNullOrEmpty(item.ParentId))
                            {
                                item.ParentId = categoryId;
                            }

                            current.Properties["bfw_syllabusfilter"] = new Bdc.PropertyValue
                            {
                                Type = Bfw.PX.Biz.DataContracts.PropertyType.String,
                                Value = item.ParentId
                            };

                            if (string.IsNullOrWhiteSpace(item.PreviousParentId))
                            {
                                item.PreviousParentId = item.ParentId;
                            }
                            
                            // update the appropriate category and sequence in case it has changed
                            current.SetSyllabusFilterCategory(item.ParentId, toc, item.Sequence);

                            SetContainer(current, item, toc); //only update the container if it's set
                            SetSubContainer(current, item, toc);

                            // create assignment settings to add points, even if not assigned date.
                            if (current.AssignmentSettings == null)
                            {
                                current.AssignmentSettings = new Bdc.AssignmentSettings()
                                {
                                    StartDate = DateTime.MinValue,
                                    DueDate = DateTime.MinValue,
                                    meta_bfw_Assigned = false,
                                    Points = 0,
                                    IsAssignable = false,
                                    CompletionTrigger = Bfw.PX.Biz.DataContracts.CompletionTrigger.Submission
                                };
                            }

                            //current.AssignmentSettings.Points = (item.Points > 0) ? item.Points : current.AssignmentSettings.Points;
                            current.AssignmentSettings.Points = item.Points;

                            if (item.GradebookCategory != null)
                            {
                                current.AssignmentSettings.Category = item.GradebookCategory;
                            }

                            // if the item is assigned a date, then update the necessary values
                            if (item.IsAssigned)
                            {
                                if (current.Subtype.ToLowerInvariant() != "pxunit")
                                {
                                    current.ParentId = "PX_MANIFEST"; 
                                }
                                current.Properties["bfw_startdate"] = new Bdc.PropertyValue()
                                {
                                    Type = Bdc.PropertyType.DateTime,
                                    Value = item.StartDate
                                };

                                var currentStartDate = current.AssignmentSettings.StartDate;
                                var currentDueDate = current.AssignmentSettings.DueDate;

                                var context = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Bfw.PX.Biz.ServiceContracts.IBusinessContext>();

                                if (!String.IsNullOrWhiteSpace(context.Course.CourseTimeZone)
                                    && TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone) != TimeZoneInfo.Local
                                    && (item.EndDateTZ == null || item.EndDateTZ.TimeZone != TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone)))
                                {
                                    item.EndDateTZ = new Bfw.Common.DateTimeWithZone(item.EndDate, TimeZoneInfo.Local, true);
                                    item.StartDateTZ = new Bfw.Common.DateTimeWithZone(item.StartDate, TimeZoneInfo.Local, true);
                                    item.EndDate = item.EndDateTZ.UniversalTime;
                                    item.StartDate = item.StartDateTZ.UniversalTime;
                                }

                                current.AssignmentSettings.StartDate = item.StartDate;
                                current.AssignmentSettings.DueDate = item.EndDate;
                                current.AssignmentSettings.StartDateTZ = item.StartDateTZ;
                                current.AssignmentSettings.DueDateTZ = item.EndDateTZ;
                                current.AssignmentSettings.meta_bfw_Assigned = true;
                                current.AssignmentSettings.CategorySequence = item.CategorySequence;
                                if (current.Subtype.ToLowerInvariant() != "pxunit")
                                {
                                    current.AssignmentSettings.IsAssignable = true;
                                }

                                if (item.SubmissionGradeAction == Bdc.SubmissionGradeAction.NotSet)
                                {
                                    if (current.Subtype.ToLowerInvariant() == "assessment" ||
                                        current.Subtype.ToLowerInvariant() == "customactivity" ||
                                        current.Subtype.ToLowerInvariant() == "homework" ||
                                        current.Subtype.ToLowerInvariant() == "quiz" ||
                                        current.Sco)
                                    {
                                        current.AssignmentSettings.SubmissionGradeAction = Bdc.SubmissionGradeAction.Default;
                                    }
                                    else if (current.Subtype.ToLowerInvariant() == "assignment" ||
                                        current.Subtype.ToLowerInvariant() == "reflectionassignment" ||
                                        current.Subtype.ToLowerInvariant() == "dropbox")
                                    {
                                        current.AssignmentSettings.SubmissionGradeAction = Bdc.SubmissionGradeAction.Manual;
                                    }
                                    else if (current.Subtype.ToLowerInvariant() == "externalcontent")
                                    {
                                        current.AssignmentSettings.SubmissionGradeAction = Bdc.SubmissionGradeAction.FullCredit;
                                    }
                                    
                                }
                                else
                                {
                                    current.AssignmentSettings.SubmissionGradeAction = item.SubmissionGradeAction;
                                }
                                if (current.Type == "Resource" || current.Type == "AssetLink")
                                {
                                    current.Type = "Assignment";
                                }
                            }
                            else
                            {
                                current.AssignmentSettings.StartDate = DateTime.MinValue;
                                current.AssignmentSettings.DueDate = DateTime.MinValue;
                                current.AssignmentSettings.meta_bfw_Assigned = false;
                                if (current.Type == "Assignment")
                                {//convert resources to assignments
                                    current.Type = "Resource";
                                }
                            }

                            current.InAssignmentCenter = true;
                            current.WasDueDateManuallySet = item.WasDueDateManuallySet;
                            current.CustomFields = item.CustomFields;

                            Item dlapItem = current.ToItem();

                            putItems.Add(dlapItem);
                        }
                    }

                    putItems.Items.AddRange(AdjustGroups(entityid, putItems.Items));

                    var curSession = SessionManager.CurrentSession;

                    Context.CacheProvider.InvalidateLaunchPadData(Context.CourseId);
                    //var task = System.Threading.Tasks.Task.Factory.StartNew<object>(() =>
                    //{
                    using (Context.Tracer.StartTrace("Store Content Items"))
                    {
                        putItems.RunAsync = true;
                        curSession.ExecuteAsAdmin(putItems);
                    }
                    //    return null;
                    //});

                }
            }
        }

        /// <summary>
        /// Updates the given assignment center category.
        /// </summary>
        /// <param name="entityId">id of the entity in which the category exists.</param>
        /// <param name="category">category to update.</param>
        public void UpdateAssignmentCenterCategory(string entityId, Bdc.AssignmentCenterCategory category)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateAssignmentCenterCategory(entityId={0}, category)", entityId))
            {
                var getItem = new GetItems();
                var putItems = new PutItems();
                Bdc.ContentItem item = null;

                using (Context.Tracer.DoTrace("Load Content Item"))
                {
                    getItem.SearchParameters = new ItemSearch
                    {
                        EntityId = entityId,
                        ItemId = category.Id
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(getItem);
                }

                if (getItem.Items.IsNullOrEmpty())
                {
                    return;
                }

                using (Context.Tracer.StartTrace("Modify Content Item"))
                {
                    item = getItem.Items.First().ToContentItem(Context);
                    item.Properties["bfw_filter_start_date"] = new Bdc.PropertyValue()
                    {
                        Type = Bdc.PropertyType.DateTime,
                        Value = category.StartDate
                    };

                    item.Properties["bfw_filter_end_date"] = new Bdc.PropertyValue()
                    {
                        Type = Bdc.PropertyType.DateTime,
                        Value = category.EndDate
                    };
                }

                using (Context.Tracer.StartTrace("Store Content Item"))
                {
                    putItems.Add(item.ToItem());

                    SessionManager.CurrentSession.ExecuteAsAdmin(putItems);
                }
            }
        }

        /// <summary>
        /// Gets the list of assignment folders in a course
        /// </summary>
        public IEnumerable<Bdc.ContentItem> GetAssignmentFolders()
        {
            var assignmentFolders = new List<Bdc.ContentItem>();
            var cmd = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = Context.EntityId,
                    Query = string.Format(@"/meta-xbook-assignment='{0}'", "y")
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Items.IsNullOrEmpty())
            {
                assignmentFolders = cmd.Items.Map(item => item.ToContentItem()).ToList();
            }

            return assignmentFolders;
        }

        #endregion

        #region Methods

        

        /// <summary>
        /// Searches for a set of items using the Search item service.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="type">Set of item types to search for.</param>
        /// <param name="parent">The parent id.</param>
        /// <returns></returns>
        private IEnumerable<Adc.Item> GetItemsByType(string entityId, List<string> type, string parent)
        {
            using (Context.Tracer.DoTrace("ContentActions.GetItemsByType(entityId={0}, type, parent={1})", entityId, parent))
            {
                var query = string.Format("( {0} ) AND /parent='{1}'", type.Map(t => string.Format("/type='{0}'", t)).Fold(" OR "), parent);
                return FindItems(new ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                });
            }
        }

        /// <summary>
        /// Loads the resource and attaches to the given content item.
        /// </summary>
        /// <param name="ci">The content item.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="resourceUri">The resource URI.</param>
        private void LoadResource(Bdc.ContentItem ci, string entityId, string resourceUri)
        {
            using (Context.Tracer.DoTrace("ContentActions.LoadResource(ci, entityId={0}, resourceUri={1})", entityId, resourceUri))
            {
                var resource = LoadResource(entityId, resourceUri);

                if (null != resource)
                {
                    if (ci.Properties.ContainsKey("mimetype"))
                    {
                        resource.ContentType = ci.Properties["mimetype"].As<string>();
                    }

                    if (ci.Resources == null)
                    {
                        ci.Resources = new List<Bdc.Resource>() { resource };
                    }
                    else
                    {
                        var list = new List<Bdc.Resource>();
                        list.AddRange(ci.Resources);
                        list.Add(resource);
                        ci.Resources = list;
                    }
                }
            }
        }        

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="resourceUri">The resource URI.</param>
        /// <returns></returns>
        private Bdc.Resource LoadResource(string entityId, string resourceUri, string query = "")
        {
            Bdc.Resource resource = null;
            Adc.Resource res = null;

            using (Context.Tracer.DoTrace("ContentActions.LoadResource(entityId={0}, resourceUri={1}, query={2})", entityId, resourceUri, query))
            {
                if (IsResourceUri(resourceUri))
                {
                    resource = Context.CacheProvider.FetchResource(entityId, resourceUri + query);
                    if (resource == null)
                    {
                        var cmd = new GetResource() { EntityId = entityId, ResourcePath = resourceUri };
                        try
                        {
                            SessionManager.CurrentSession.Execute(cmd);
                            res = cmd.Resource;
                        }
                        catch (DlapException)
                        {
                            res = null;
                        }
                        if (null != res)
                        {
                            resource = res.ToResource();
                            if (resource.Extension == PXRES_EXTENSION)
                            {
                                Bdc.XmlResource xmlRes = res.ToXmlResource();
                                return xmlRes;
                            }
                        }
                        if (null != resource)
                            Context.CacheProvider.StoreResource(resource, entityId, resourceUri + query);
                    }
                }
            }
            return resource;
        }

        /// <summary>
        /// Loads the resources by first getting the list of matching resources, then loads the content of each
        /// resource individually. Optionally resources can be matched against query. Only those resources that match
        /// will be included in the result list.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private IEnumerable<Bdc.Resource> LoadResources(string entityId, string resourceUri, string query = "")
        {
            List<Bdc.Resource> result = new List<Bdc.Resource>();

            using (Context.Tracer.DoTrace("ContentActions.LoadResources(entityId={0}, resourceUri={1}, query={2})", entityId, resourceUri, query))
            {
                if (IsResourceUri(resourceUri))
                {
                    var resourceList = new GetResourceList()
                    {
                        EntityId = entityId,
                        ResourcePath = resourceUri
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(resourceList);

                    if (!resourceList.Resources.IsNullOrEmpty())
                    {
                        foreach (var resource in resourceList.Resources)
                        {
                            var bizResource = LoadResource(entityId, resource.Url, query);
                            if (bizResource != null)
                            {
                                bizResource.CreationDate = resource.CreationDate;
                                bizResource.ModifiedDate = resource.ModifiedDate;
                                result.Add(bizResource);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private IEnumerable<Bdc.Resource> LoadResourcesInfo(string entityId, string resourceUri, string query = "")
        {
            List<Bdc.Resource> result = new List<Bdc.Resource>();
            using (Context.Tracer.DoTrace("ContentActions.LoadResourcesInfo(entityId={0}, resourceUri={1}, query={2})", entityId, resourceUri, query))
            {
                if (IsResourceUri(resourceUri))
                {
                    var resourceList = new GetResourceList()
                    {
                        EntityId = entityId,
                        ResourcePath = resourceUri
                    };
                    SessionManager.CurrentSession.ExecuteAsAdmin(resourceList);
                    if (!resourceList.Resources.IsNullOrEmpty())
                    {
                        foreach (var resource in resourceList.Resources)
                        {
                            var bizResource = new Bdc.Resource();
                            bizResource.Url = resource.Url;
                            bizResource.EntityId = resource.EntityId;
                            bizResource.CreationDate = resource.CreationDate;
                            bizResource.ModifiedDate = resource.ModifiedDate;
                            bizResource.ContentType = resource.ContentType;
                            result.Add(bizResource);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether [is resource URI] [the specified resource URI].
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <returns>
        ///   <c>true</c> if [is resource URI] [the specified resource URI]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsResourceUri(string resourceUri)
        {
            bool result = false;
            Uri uri = null;

            result = Uri.TryCreate(resourceUri, UriKind.RelativeOrAbsolute, out uri) && !uri.IsAbsoluteUri;

            return result;
        }

        /// <summary>
        /// Parses the px resource file.
        /// </summary>
        /// <param name="resource">The resource.</param>
        private bool ParsePxResourceFile(Bdc.Resource resource, string query = "")
        {
            bool resourceMatchesQuery = true;

            using (Context.Tracer.DoTrace("ContentActions.ParsePxResourceFile(resource, query={0})", query))
            {
                var dest = resource.GetStream();
                var sr = new StreamReader(dest);

                string xmlDoc = sr.ReadToEnd();
                dest.Flush();
                dest.Seek(0, SeekOrigin.Begin);

                XDocument xDoc = new XDocument();
                if (!string.IsNullOrEmpty(xmlDoc))
                {
                    xDoc = XDocument.Parse(xmlDoc);

                    if (!string.IsNullOrEmpty(query))
                    {
                        var result = (IEnumerable<object>)xDoc.XPathEvaluate(query);

                        if (result.IsNullOrEmpty())
                        {
                            resourceMatchesQuery = false;
                        }
                    }
                }

                XElement xElement = xDoc.XPathSelectElement("//body");

                if (xElement != null)
                {
                    string body = xElement.Value;
                    dest.SetLength(0);
                    var sw = new StreamWriter(dest);

                    sw.Write(System.Web.HttpUtility.HtmlDecode(body));
                    sw.Flush();
                }

                xElement = xDoc.XPathSelectElement("//title");
                if (xElement != null) resource.Name = xElement.Value ?? "Document";

                xElement = xDoc.XPathSelectElement("//ExtendedProperties");
                if (xElement != null)
                {
                    var extendedProperties = xElement.Elements();
                    if (!extendedProperties.IsNullOrEmpty())
                    {
                        foreach (XElement prop in extendedProperties)
                        {
                            resource.ExtendedProperties.Add(prop.Name.ToString(), prop.Value);

                        }
                    }
                }
            }

            return resourceMatchesQuery;
        }
        
        /// <summary>
        /// Copies the properties collection from one content item to another.
        /// </summary>
        /// <param name="contentfromModel">The contentfrom model.</param>
        /// <param name="content">The content.</param>
        private void PersistProperties(Bdc.ContentItem contentfromModel, Bdc.ContentItem content)
        {
            using (Context.Tracer.DoTrace("ContentActions.PersistProperties(contentfromModel, content)"))
            {
                if (!contentfromModel.Properties.IsNullOrEmpty())
                {
                    Bdc.PropertyValue cat = null;
                    Bdc.PropertyValue catModel = null;
                    content.Properties.TryGetValue("templateparent", out cat);
                    contentfromModel.Properties.TryGetValue("templateparent", out catModel);
                    if (cat != null && catModel == null)
                    {
                        contentfromModel.Properties.Add("templateparent", cat);
                    }

                    // determines of 
                    Bdc.PropertyValue sci = null, sciModel = null;
                    content.Properties.TryGetValue("bfw_social_commenting_integration", out sci);
                    contentfromModel.Properties.TryGetValue("bfw_social_commenting_integration", out sciModel);
                    if (sci != null && sciModel == null)
                        contentfromModel.Properties.Add("bfw_social_commenting_integration", sci);
                }
                else
                {
                    contentfromModel.Properties = content.Properties;
                }
            }
        }

        /// <summary>
        /// Gets the raw item.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public XDocument GetRawItem(string entityId, string itemId)
        {
            XDocument doc = null;

            using (Context.Tracer.DoTrace("ContentActions.GetRawItem(entityId={0}, itemId={1})", entityId, itemId))
            {
                var getCmd = new GetRawItem()
                {
                    ItemId = itemId,
                    EntityId = entityId
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(getCmd);

                doc = getCmd.ItemDocument;
            }

            return doc;
        }

        /// <summary>
        /// Add a new element to data
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementName"></param>
        /// <param name="newVal"></param>
        private static void AddDataElement(XElement element, string elementName, string newVal)
        {
            var data = element.Element("data");
            var locked = new XElement(elementName);
            locked.Value = newVal;
            data.Add(locked);
        }

        /// <summary>
        /// Copies the XML from one item to another.
        /// </summary>
        /// <param name="entityId">Entity in which the item exists.</param>
        /// <param name="fromItemId">Id of the item to copy.</param>
        /// <param name="toItemId">Id of the new item.</param>
        /// <param name="removeDesc">Flag to remove Description of item during copy. default is false</param>
        /// <returns>XML document representing the copy.</returns>        
        private XDocument CopyItemXml(string entityId, string fromItemId, string toItemId, bool removeDesc = false)
        {
            XDocument itemXml = null;
            using (Context.Tracer.DoTrace("ContentActions.CopyItemXml(entityid={0}, fromItemId={1}, toItemId={2}, removeDesc={3})", entityId, fromItemId, toItemId, removeDesc))
            {
                var from = GetRawItem(entityId, fromItemId);

                if (from != null)
                {
                    // Need to turn 'id' in the root node into 'itemid', and 'actualentityid' into 'entityid'.
                    // Also replace the id with the new id, and switch the entityid to the course that was chosen.

                    if (from.XPathSelectElement("//data/bfw_template") != null && from.XPathSelectElement("//data/bfw_template").Value == "PxUnit")
                    {
                        toItemId = "MODULE_" + toItemId;
                    }

                    HtmlXmlHelper.SwitchAttributeName(from.Root, "id", "itemid", toItemId);
                    HtmlXmlHelper.SwitchAttributeName(from.Root, "actualentityid", "entityid", entityId);
                    from.XPathSelectElement("//data/parent").TryRemove();
                    from.XPathSelectElement("//data/bfw_tocs").TryRemove();
                    if (removeDesc)
                        from.XPathSelectElement("//data/description").TryRemove();

                    itemXml = from;
                }
                else
                {
                    throw new Exception("CopyItem: Could not find item to copy.");
                }
            }

            return itemXml;
        }

        public IEnumerable<Item> FindItems(ItemSearch search, bool asAdmin = true, string categoryId = null)
        {
            var cmd = new GetItems()
            {
                SearchParameters = search
            };

            using (Context.Tracer.DoTrace("ContentActions.FindItems(search, asAdmin={0}, categoryId={1})", asAdmin, categoryId))
            {
                if (asAdmin)
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
                else
                {
                    SessionManager.CurrentSession.Execute(cmd);
                }

                // If appropriate, check the student's resource file to see if there are any student-specific items that need
                // to overwrite items in the list we are about to return.
                //var lookForStudentResources = true || !search.EntityId.IsNullOrEmpty() && Context.EnrollmentId == search.EntityId;
                //if (lookForStudentResources)
                if (!search.ExcludeStudentItem && !string.IsNullOrEmpty(Context.EnrollmentId))
                {
                    ApplyStudentItems(cmd.Items, categoryId, false, search);
                }
            }
            return cmd.Items ?? new List<Item>();
        }

        /// <summary>
        /// Find List of ContentItems based on specified ItemsSearch Query
        /// </summary>
        /// <param name="search"></param>
        /// <param name="asAdmin"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<ContentItem> FindContentItems(ItemSearch search, bool asAdmin = true, string categoryId = null)
        {
            Batch batch = new Batch();
            var cmd = new GetItems()
            {
                SearchParameters = search
            };
            batch.Add(cmd);
            using (Context.Tracer.DoTrace("ContentActions.FindItems(search, asAdmin={0}, categoryId={1}, searchQuery{2})", asAdmin, categoryId, search.Query))
            {
                if (asAdmin)
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                }
                else
                {
                    SessionManager.CurrentSession.Execute(batch);
                }

                // If appropriate, check the student's resource file to see if there are any student-specific items that need
                // to overwrite items in the list we are about to return.
                //var lookForStudentResources = true || !search.EntityId.IsNullOrEmpty() && Context.EnrollmentId == search.EntityId;
                //if (lookForStudentResources)

                cmd = batch.CommandAs<GetItems>(0);

                if (categoryId != null)
                {
                    if (!string.IsNullOrEmpty(Context.EnrollmentId)) ApplyStudentItems(cmd.Items, categoryId, false, search);
                }
            }

            List<ContentItem> contentItems = new List<ContentItem>();

            if (cmd.Items != null) contentItems = cmd.Items.Map(c => c.ToContentItem(Context)).ToList();

            return contentItems;
        }

        public IEnumerable<ContentItem> FindItemsBatch(List<ItemSearch> search, bool asAdmin = true)
        {
            List<ContentItem> contentItems = new List<ContentItem>();
            Batch batch = new Batch();
            batch.RunAsync = true;
            foreach (var s in search)
            {
                var cmd = new GetItems()
                {
                    SearchParameters = s
                };
                batch.Add(cmd);
            }
            if (!batch.Commands.IsNullOrEmpty())
            {
                using (Context.Tracer.DoTrace("ContentActions.FindItemsBatch()"))
                {
                    if (asAdmin)
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                    }
                    else
                    {
                        SessionManager.CurrentSession.Execute(batch);
                    }
                }

                for (int i = 0; i < batch.Commands.Count(); i++)
                {
                    var cmd = batch.CommandAs<GetItems>(i);
                    contentItems.AddRange(cmd.Items.Map(c => c.ToContentItem()));
                }
            }
            return contentItems;
        }

        private void ApplyStudentItems(List<Item> items, string categoryId, bool loadchild, params ItemSearch[] searches)
        {
            if (ItemSearch.ApplyStudentItems && (searches.IsNullOrEmpty() || searches.Any(s => !s.ExcludeStudentItem)))
            {
                using (
                    Context.Tracer.DoTrace(
                        "ContentActions.ApplyStudentItems(items, categoryId={0}, loadChild={1}, search)", categoryId,
                        loadchild))
                {

                    // Can't apply student items to a blank list.
                    List<string> r = new List<string>();


                    if (items == null)
                    {
                        throw new ArgumentNullException("items");
                    }

                    if (Context.IsPublicView &&
                        (Context.CurrentUser == null ||
                         (Context.CurrentUser != null && Context.CurrentUser.Id != Context.Course.CourseOwner)))
                    {
                        string currentUserId = Context.Course.CourseOwner;
                        Context.CurrentUser = new UserInfo();
                        Context.CurrentUser.Id = currentUserId;
                        var cmdUser = new GetUsers()
                        {
                            SearchParameters = new Adc.UserSearch()
                            {
                                Id = currentUserId
                            }
                        };

                        SessionManager.CurrentSession.ExecuteAsAdmin(cmdUser);

                        if (!cmdUser.Users.IsNullOrEmpty())
                        {
                            Context.CurrentUser.Email = cmdUser.Users.FirstOrDefault().Email;
                        Context.CurrentUser.Username = cmdUser.Users.FirstOrDefault().UserName;
                            Context.CurrentUser.ReferenceId = cmdUser.Users.FirstOrDefault().Reference;
                            Context.CurrentUser.PasswordQuestion = "#DynamicRunTimePasswordQuestion#";
                        }

                        Context.EnrollmentId = EnrollmentActions.GetUserEnrollmentId(Context.Course.CourseOwner,
                            Context.EntityId);
                    }

		    var studentItems = GetStudentItems(Context.EnrollmentId, categoryId);                    

                    foreach (var item in studentItems)
                    {
                        item.Folder += "__(*studentfolder*)";
                        item.IsStudentCreatedFolder = true;
                    }

                    // See if there are any student items that need to be added by the item search. We
                    // are going to only work with ID searches and ParentID searches.
                    foreach (var search in searches)
                    {
                        foreach (var studentItem in studentItems)
                        {
                            if (studentItem.Id == search.ItemId)
                            {
                                items.Add(studentItem);
                            }

                            if (!String.IsNullOrEmpty(search.Query) && search.Query.ToLower().Contains("parentid"))
                            {
                                var searchParentId = search.Query.Split('@')[1].Split('=')[1].Trim('\'');
                                if (studentItem.ParentId == searchParentId)
                                {
                                    //items.Add(studentItem); // Commented out because, changed the style of loading toc: ref: PLATX-8118
                                }
                            }
                        }
                    }

                    // Look at each student item found and see if it needs to replace an exisitng natural item.
                    foreach (var studentItem in studentItems)
                    {
                        // Student items need to replace original items in the same location in the list.
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].Id == studentItem.Id)
                            {
                                studentItem.Children = items[i].Children;
                                items[i] = ApplyStudentData(items[i], studentItem);
                            }
                        }
                    }

                    // Apply student items to all children of items in this list.
                    foreach (var item in items)
                    {
                        if (!item.Children.IsNullOrEmpty())
                        {
                            ApplyStudentItems(item.Children, categoryId, loadchild);
                        }
                    }

                    // See if any of the student items need to be made children of the items.
                    foreach (var studentItem in studentItems)
                    {
                        foreach (var item in items)
                        {
                            if (item.Id == studentItem.ParentId)
                            {
                                if (item.Children == null)
                                {
                                    item.Children = new List<Item>();
                                }
                                else if (item.Children.Contains(studentItem, (a, b) => a.Id == b.Id))
                                {
                                    continue;
                                }
                                // student alterations to a instructor item have already been merged down to the instructor item,
                                // it does not make sense to be added as a child to the same parent item
                                if (studentItem.Id != item.Id &&
                                    (loadchild || item.Id == "PX_COURSE_EPORTFOLIO_ROOT_ITEM"))
                                {
                                    item.Children.Add(studentItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Item> RemoveDueDate(IEnumerable<Item> studentItems)
        {
            using (Context.Tracer.DoTrace("ContentActions.RemoveDuedate(studentItems)"))
            {
                List<ContentItem> ciItems = new List<ContentItem>();
                foreach (Item item in studentItems)
                {
                    ContentItem ci = item.ToContentItem(Context);
                    if (ci.AssignmentSettings != null)
                    {
                        ci.AssignmentSettings.DueDate = DateTime.MinValue;
                    }
                    ciItems.Add(ci);
                }
                return ciItems.Map(i => i.ToItem());
            }
        }


        public IEnumerable<ContentItem> GetAllStudentItems(string categoryId)
        {
            using (Context.Tracer.DoTrace("ContentActions.GetA;;StudentItems(categoryId={0})", categoryId))
            {
                if (categoryId.IsNullOrEmpty())
                {
                    return GetStudentItems(Context.EnrollmentId, categoryId).Map(i => i.ToContentItem(Context));
                }
                else
                {
                    return GetStudentItems(categoryId, categoryId).Map(i => i.ToContentItem(Context));
                }
            }
        }

        public string GetStudentResourceId(string enrollmentId)
        {
            return String.Format("enrollment_{0}", enrollmentId);
        }

        public XDocument GetEmptyStudentDoc()
        {
            XDocument doc = new XDocument();
            doc.AddFirst(new XElement("items"));
            return doc;
        }

        public IEnumerable<Bdc.ContentItem> UpdateStudentItems(string id, string parentId, string sequence, string categoryId)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateStudentItems(id={0}, parentId={1}, sequence={2}, categoryId={3})", id, parentId, sequence, categoryId))
            {
                var studentItems = GetStudentItems(Context.EnrollmentId, categoryId.Substring(11));

                IEnumerable<ContentItem> ciItems = studentItems.Map(ci => ci.ToContentItem(Context));


                foreach (ContentItem item in ciItems)
                {
                    if (item.Id == id)
                    {
                        item.Sequence = sequence;
                        item.DefaultCategorySequence = sequence;
                        item.ParentId = parentId;
                        item.DefaultCategoryParentId = parentId;
                    }
                }
                return ciItems;
            }
        }

        /// <summary>
        /// Update item from student resource file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="sequence"></param>
        /// <param name="studentEnrollmentId"></param>
        public ContentItem UpdateStudentItemsForStudents(string id, string parentId, string sequence, string categoryId)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateStudentItemsForStudents(id={0}, parentId={1}, sequence={2}, categoryId={3})", id, parentId, sequence, categoryId))
            {
                var studentItems = GetStudentItems(categoryId.Substring(11), categoryId.Substring(11));

                IEnumerable<ContentItem> ciItems = studentItems.Map(ci => ci.ToContentItem(Context));


                foreach (ContentItem item in ciItems)
                {
                    if (item.Id == id)
                    {
                        item.Sequence = sequence;
                        item.DefaultCategorySequence = sequence;
                        item.ParentId = parentId;
                        item.DefaultCategoryParentId = parentId;
                        return item;
                    }
                }
                return null;
            }
        }

        public IEnumerable<Bdc.ItemLink> GetItemLinks(string entityId)
        {
            var cmd = new GetItemLinks() {
                EntityId = entityId
            };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.ItemLinks.Map(i => i.ToItemLink());
        }

        /// <summary>
        /// Update item from personal eportfolio dashboard student resource file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="sequence"></param>
        /// <param name="studentEnrollmentId"></param>
        public ContentItem UpdatePresentationCourseStudentItemsForStudents(string id, string parentId, string sequence, string categoryId)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdatePresentationCourseStudentItemsForStudents(id={0}, parentId={1}, sequence={2}, categoryId={3})", id, parentId, sequence, categoryId))
            {
                var studentItems = GetStudentItems(categoryId.Substring(11), categoryId.Substring(11));

                IEnumerable<ContentItem> ciItems = studentItems.Map(ci => ci.ToContentItem(Context));


                foreach (ContentItem item in ciItems)
                {
                    if (item.Id == id)
                    {
                        foreach (TocCategory cat in item.Categories)
                        {
                            if (cat.Id == string.Format("ep_course_{0}", Context.EntityId))
                            {
                                cat.ItemParentId = parentId;
                                cat.Sequence = sequence;
                            }
                        }
                        return item;
                    }
                }
                return null;
            }
        }


        private Item ApplyStudentData(Item instructorItem, Item studentItem)
        {
            using (Context.Tracer.DoTrace("ContentActions.ApplyStudentData(instructorItem, studentItem)"))
            {
                ContentItem ciInsTemp = instructorItem.ToContentItem(Context);
                ContentItem ciStuTemp = studentItem.ToContentItem(Context);

                // Cover title
                string coverTitle = string.Empty;
                if (ciStuTemp.Properties.ContainsKey("CoverTitle"))
                {
                    coverTitle = ciStuTemp.Properties["CoverTitle"].Value.ToString();
                }
                if (!ciInsTemp.Properties.ContainsKey("CoverTitle"))
                {
                    PropertyValue propVal = new PropertyValue();
                    propVal.Type = PropertyType.String;
                    propVal.Value = coverTitle;
                    ciInsTemp.Properties.Add("CoverTitle", propVal);
                    if (!coverTitle.IsNullOrEmpty())
                    {
                        ciInsTemp.Title = coverTitle;
                    }
                }
                else
                {
                    ciInsTemp.Properties["CoverTitle"].Value = coverTitle;
                    if (!coverTitle.IsNullOrEmpty())
                    {
                        ciInsTemp.Title = coverTitle;
                    }
                }

                // Student Instruction
                string studentDescription = string.Empty;
                if (ciStuTemp.Properties.ContainsKey("StudentDescription"))
                {
                    studentDescription = ciStuTemp.Properties["StudentDescription"].Value.ToString();
                }
                if (!ciInsTemp.Properties.ContainsKey("StudentDescription"))
                {
                    PropertyValue propVal = new PropertyValue();
                    propVal.Type = PropertyType.String;
                    propVal.Value = studentDescription;
                    ciInsTemp.Properties.Add("StudentDescription", propVal);
                }
                else
                {
                    ciInsTemp.Properties["StudentDescription"].Value = studentDescription;
                }

                //Selected Theme
                string selectedTheme = string.Empty;
                if (ciStuTemp.Properties.ContainsKey("SelectedTheme"))
                {
                    selectedTheme = ciStuTemp.Properties["SelectedTheme"].Value.ToString();
                }
                if (!ciInsTemp.Properties.ContainsKey("SelectedTheme"))
                {
                    PropertyValue propVal = new PropertyValue();
                    propVal.Type = PropertyType.String;
                    propVal.Value = selectedTheme;
                    ciInsTemp.Properties.Add("SelectedTheme", propVal);
                }
                else
                {
                    ciInsTemp.Properties["SelectedTheme"].Value = selectedTheme;
                }

                //Banner Image
                string bannerImage = string.Empty;
                if (ciStuTemp.Properties.ContainsKey("BannerImage"))
                {
                    bannerImage = ciStuTemp.Properties["BannerImage"].Value.ToString();
                }
                if (!ciInsTemp.Properties.ContainsKey("BannerImage"))
                {
                    PropertyValue propVal = new PropertyValue();
                    propVal.Type = PropertyType.String;
                    propVal.Value = bannerImage;
                    ciInsTemp.Properties.Add("BannerImage", propVal);
                }
                else
                {
                    ciInsTemp.Properties["BannerImage"].Value = bannerImage;
                }

                Item tempItem = ciInsTemp.ToItem();
                tempItem.Children = instructorItem.Children;
                instructorItem = tempItem;
                return instructorItem;
            }
        }

        //////////////////////////////////////
        /// <summary>
        /// Saves Row Item xml
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemDataXml"> </param>
        /// <param name="contentItemId"> </param>
        public void SaveRowItem(string entityId, XElement itemDataXml, string contentItemId)
        {
            //Save ContentItem
            using (Context.Tracer.DoTrace("ContentActions.SaveRowItem(entityid={0}, ItemId={1})", entityId, contentItemId))
            {
                XDocument xItemDoc = GetRawItem(entityId, contentItemId);
                HtmlXmlHelper.SwitchAttributeName(xItemDoc.Root, "id", "itemid", contentItemId);
                HtmlXmlHelper.SwitchAttributeName(xItemDoc.Root, "actualentityid", "entityid", entityId);

                xItemDoc.XPathSelectElement("./item/data").ReplaceWith(itemDataXml);
                //if (xItemDoc.Document != null && xItemDoc.XPathSelectElement("./item/data") != null)
                //{
                //    el = xItemDoc.XPathSelectElement("./item/data");
                //    el = itemDataXml;
                //}

                var putCmd = new PutRawItem()
                {
                    ItemDoc = xItemDoc
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(putCmd);
            }
        }
        
        #region Gradebook Category

        public bool SetGradebookCategoryFor(string itemId, string categoryId, string entityId)
        {
            var contentItem = GetContent(entityId, itemId);
            if (contentItem == null)
            {
                return false;
            }

            contentItem.UnitGradebookCategory = categoryId;

            StoreContent(contentItem, entityId);

            return true;
        }

        #endregion

        #region Containers and SubContainers

        /// <summary>
        /// Assigns the containers to Bdc.ContentItem
        /// </summary>
        /// <param name="contentItem">The contentitem.</param>
        /// <param name="assignmentItem">The assignment item.</param>
        /// <param name="toc">The toc.</param>
        private static void SetContainer(Bdc.ContentItem contentItem, Bdc.AssignmentCenterItem assignmentItem, string toc = "syllabusfilter")
        {
            var container = assignmentItem.Containers.FirstOrDefault(c => c.Toc == toc) ??
                            contentItem.Containers.FirstOrDefault(c => c.Toc == toc);
            
            if (container != null)
            {
                contentItem.SetContainer(container.Value, toc);
            }
        }

        /// <summary>
        /// Assigns the sub-containers to Bdc.ContentItem
        /// </summary>
        /// <param name="contentItem">The item.</param>
        /// <param name="assignmentItem">The assignment item.</param>
        /// <param name="toc">The toc.</param>
        private static void SetSubContainer(Bdc.ContentItem contentItem, Bdc.AssignmentCenterItem assignmentItem, string toc = "syllabusfilter")
        {
            var subcontainer = assignmentItem.SubContainerIds.FirstOrDefault(c => c.Toc == toc) ??
                            contentItem.SubContainerIds.FirstOrDefault(c => c.Toc == toc);

            if (subcontainer != null)
            {
                contentItem.SetSubContainer(subcontainer.Value, toc);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Recursive function used by the ListDescendents parent method
        /// </summary>
        /// <param name="entityId">entity id</param>
        /// <param name="userId">user id</param>
        /// <param name="item"></param>
        /// <param name="result"></param>
        /// <param name="studentItems"></param>
        private void ListDescendents(string entityId, string userId, Adc.Item item, List<Adc.Item> result, List<Adc.Item> studentItems)
        {
            var childrenCmd = new GetItems()
            {
                SearchParameters = ItemQueryActions.BuildListChildrenQuery(entityId, item.Id, 1, string.Empty, userId)
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(childrenCmd);

            if (!childrenCmd.Items.IsNullOrEmpty())
            {
                item.Children = childrenCmd.Items;
                result.AddRange(childrenCmd.Items);
            }

            foreach (var studentItem in studentItems)
            {
                if (!String.IsNullOrEmpty(childrenCmd.SearchParameters.Query) && childrenCmd.SearchParameters.Query.ToLower().Contains("parentid"))
                {
                    var searchParentId = childrenCmd.SearchParameters.Query.Split('@')[1].Split('=')[1].Trim('\'');

                    if (studentItem.ParentId == searchParentId && result.Count(i => i.Id == studentItem.Id) == 0)
                    {
                        item.Children.Add(studentItem);
                        result.Add(studentItem);
                    }
                }
            }

            if (!item.Children.IsNullOrEmpty())
            {
                foreach (var childItem in item.Children)
                {
                    ListDescendents(entityId, userId, childItem, result, studentItems);
                }
            }
        }
       
        /// <summary>
        /// Gets list of child items based on TOC definition.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemIds">The list of item's ids.</param>
        /// <param name="tocDefinition">The TOC definition</param>
        /// <param name="includingShortCuts">The flag if shortcuts must be included.</param>
        /// <returns></returns>
        private List<Agilix.DataContracts.Item> GetItemsByQuery(string entityId, List<string> itemIds, string tocDefinition, bool includingShortCuts = false)
        {
            var batch = new Batch();

            foreach (string id in itemIds)
            {
                var queryCmd = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = entityId,
                        Query = String.Format("/bfw_tocs/{0}@parentid='{1}'", tocDefinition, id)
                    }
                };

                batch.Add(queryCmd);
            }

            if (batch.Commands.Any())
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }

            List<Item> result = new List<Item>();

            foreach (GetItems cmd in batch.Commands)
            {
                var items = cmd.Items;

                if (!cmd.Items.IsNullOrEmpty())
                {
                    var tempChildren = cmd.Items.ToList();
                    if (!includingShortCuts) tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                    items = tempChildren.ToList();
                }

                result.AddRange(items);
            }

            return result;
        }

        /// <summary>
        /// Gets list of child items based on user id.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemIds">The list of item's ids.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="includingShortCuts">The flag if shortcuts must be included.</param>
        /// <returns></returns>
        private List<Agilix.DataContracts.Item> GetItemsByIdList(string entityId, List<string> itemIds, string userId, bool includingShortCuts = false)
        {
            var batch = new Batch();

            foreach (string id in itemIds)
            {
                var queryCmd = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = entityId,
                        ItemId = id
                    }
                };

                batch.Add(queryCmd);
            }

            if (batch.Commands.Any())
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }

            List<Item> result = new List<Item>();

            foreach (GetItems cmd in batch.Commands)
            {
                var items = cmd.Items;

                if (!cmd.Items.IsNullOrEmpty())
                {
                    var tempChildren = cmd.Items.ToList();
                    if (!includingShortCuts) tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                    items = tempChildren.ToList();
                }

                result.AddRange(items);
            }

            return result;
        }              
    }
}
