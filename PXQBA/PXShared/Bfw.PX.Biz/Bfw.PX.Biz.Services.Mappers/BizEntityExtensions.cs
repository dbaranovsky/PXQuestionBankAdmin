using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Agx = Bfw.Agilix.Dlap;
using AgxDC = Bfw.Agilix.DataContracts;
using Attachment = Bfw.Agilix.DataContracts.Attachment;

namespace Bfw.PX.Biz.Services.Mappers
{
    public static class BizEntityExtensions
    {
        /// <summary>
        /// Convert from an agilix object to announcement.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static Announcement ToAnnouncement(this AgxDC.Announcement item)
        {
            Announcement biz = new Announcement();

            biz.Title = item.Title;
            biz.Body = item.Html;
            biz.CreationDate = item.CreationDate;
            biz.Path = item.Path;
            biz.StartDate = item.StartDate;
            biz.EndDate = item.EndDate;
            biz.PrimarySortOrder = item.PrimarySortOrder;
            biz.PinSortOrder = item.PinSortOrder;

            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to RSS Feed.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static ContentItem ToRSSFeed(this AgxDC.Item item)
        {
            ContentItem biz = new ContentItem();
            biz.Href = item.Href;
            return biz;
        }

        /// <summary>
        /// Gets the inner value of an <see cref="XmlNode" />.
        /// </summary>
        /// <param name="node">The xml node.</param>
        /// <param name="defaultValue">Default value to return if null.</param>
        /// <returns></returns>
        public static string NodeValue(this XmlNode node, string defaultValue)
        {
            if (node != null)
                return node.InnerText ?? defaultValue;

            return defaultValue;
        }

        /// <summary>
        /// Convert from an agilix assignment object to assigned item.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns></returns>
        public static AssignedItem ToAssignedItem(this AgxDC.Assignment assignment)
        {
            var item = new AssignedItem();

            item.Title = assignment.Item.Title;
            item.Id = assignment.Item.Id;
            item.DueDate = assignment.DueDate;

            return item;
        }

        /// <summary>
        /// Convert from an agilix object to assigneditem.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static AssignedItem ToAssigneditem(this AgxDC.Item item)
        {
            var biz = new AssignedItem();

            if (null != item.Data)
            {
                var dueDate = item.Data.Attribute("duedate");

                if (null != dueDate)
                {
                    biz.DueDate = DateTime.Parse(dueDate.Value);
                }
                else
                {
                    var dueDateElm = item.Data.Element("duedate");
                    if (null != dueDateElm && !String.IsNullOrEmpty(dueDateElm.Value))
                    {
                        biz.DueDate = DateTime.Parse(dueDateElm.Value);
                    }
                }
            }

            biz.Id = item.Id;
            biz.Title = item.Title;
            biz.MaxPoints = item.MaxPoints;

            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to message.
        /// </summary>
        /// <param name="item">The agilix message object.</param>
        /// <param name="userService">An IUserService implementation.</param>
        /// <returns></returns>
        public static Message ToMessage(this AgxDC.Message item)
        {
            var biz = new Biz.DataContracts.Message();

            if (item.AuthorInfo != null)
            {
                biz.Author = item.AuthorInfo.ToUserInfo();
            }

            biz.Body = item.Body;
            biz.Date = item.Created;

            return biz;
        }

        /// <summary>
        /// Convert from a message to an agilix object.
        /// </summary>
        /// <param name="biz">The agilix item.</param>
        /// <returns></returns>
        public static AgxDC.Message ToMessage(this Message biz)
        {
            var item = new AgxDC.Message();

            item.Author = biz.Author.Id;
            item.Body = biz.Body;
            item.Created = biz.Date;

            return item;
        }

        /// <summary>
        /// Convert from an agilix object to submission.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Submission ToSubmission(this AgxDC.Submission agx)
        {
            var biz = new Submission
            {
                ItemId = agx.ItemId,
                Body = agx.Body,
                Data = agx.Data,
                StreamData = agx.StreamData,
                EnrollmentId = agx.EnrollmentId,
                SubmissionType = (SubmissionType)agx.SubmissionType,
                StudentFirstName = agx.StudentFirstName,
                StudentLastName = agx.StudentLastName,
                StudentFullName = agx.StudentLastName + ", " + agx.StudentFirstName,
                Version = agx.Version,
                SubmittedDate = agx.SubmittedDate,
                Actions = agx.Actions == null ? null : agx.Actions.Map(a => a.ToSubmissionAction()).ToList()

            };

            if (agx.Grade != null)
            {
                biz.Grade = agx.Grade.ToGrade();
            }

            if (agx.QuestionAttempts != null)
            {
                biz.QuestionAttempts = new Dictionary<string, IList<QuestionAttempt>>();

                foreach (var qa in agx.QuestionAttempts)
                {
                    var qaList = qa.Value.Map(q => new QuestionAttempt() { AttemptAnswer = q.AttemptAnswer, QuestionId = q.QuestionId, PartId = q.PartId, PointsComputed = q.PointsComputed, PointsPossible = q.PointsPossible, AttemptVersion = q.AttemptVersion }).ToList();
                    biz.QuestionAttempts.Add(qa.Key, qaList);
                }
            }

            if (agx.SubmissionAttempts != null)
            {
                biz.SubmissionAttempts = new Dictionary<string, SubmissionAttempt>();

                foreach (var sa in agx.SubmissionAttempts)
                {
                    var saList = new SubmissionAttempt()
                    {
                        LastSave = sa.Value.LastSave,
                        StartPage = sa.Value.StartPage,
                        PartId = sa.Value.PartId,
                        QuestionId = sa.Value.QuestionId,
                        SecondsSpent = sa.Value.SecondsSpent,
                        ToContinue = sa.Value.ToContinue
                    };

                    biz.SubmissionAttempts.Add(sa.Key, saList);
                }
            }

            return biz;
        }

        /// <summary>
        /// Converts agilix object to SubmissionAction
        /// </summary>
        /// <param name="agx"></param>
        /// <returns></returns>
        public static SubmissionAction ToSubmissionAction(this AgxDC.SubmissionAction agx)
        {
            var submissionAction = new SubmissionAction()
            {
                Date = agx.Date,
                Location = agx.Location,
                Type = (SubmissionActionType)agx.Type
            };

            return submissionAction;
        }

        /// <summary>
        /// Convert from submission to an agilix object.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.Submission ToSubmission(this Submission biz)
        {
            var item = new AgxDC.Submission
            {
                ItemId = biz.ItemId,
                Body = biz.Body,
                Data = biz.Data,
                StreamData = biz.StreamData,
                EnrollmentId = biz.EnrollmentId,
                SubmissionType = (AgxDC.SubmissionType)biz.SubmissionType,
                StudentFirstName = biz.StudentFirstName,
                StudentLastName = biz.StudentLastName,
                Version = biz.Version,
                SubmittedDate = biz.SubmittedDate,
                Notes = biz.Notes,
                SubmittedFileName = biz.SubmittedFileName
            };
            return item;
        }
        
        /// <summary>
        /// Convert from an agilix object to menu item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static NavigationItem ToMenuItem(this AgxDC.Item item)
        {
            return ToMenuItem(item, null);
        }

        /// <summary>
        /// Converts the agilix item and any of its children to a MenuItem.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static NavigationItem ToMenuItem(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context)
        {
            return ToMenuItem(item, context, string.Empty);
        }

        /// <summary>
        /// Converts the agilix item and any of its children to a MenuItem.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="context">The context.</param>
        /// <param name="categoryId">The category id.</param>
        /// <returns></returns>
        public static NavigationItem ToMenuItem(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context, string categoryId)
        {
            NavigationItem menuItem = ParseItem(item, context, categoryId);
            return menuItem;
        }

        /// <summary>
        /// Parses an agilix item and all it's children and converts to a navigation item structure.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="context"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private static NavigationItem ParseItem(AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context, string categoryId)
        {
            var menuItem = new NavigationItem()
            {
                Name = item.Title,
                Id = item.Id,
                ParentId = item.ParentId,
                Type = DetermineBfwType(item),
                Children = new List<NavigationItem>(),
                Sequence = item.Sequence,
                IsStudentCreated = item.IsStudentCreatedFolder
            };

            menuItem.Properties = ParseContentProperties(item.Data);


            var assigned = item.ToAssigneditem();
            menuItem.DueDate = assigned.DueDate;
            menuItem.MaxPoints = assigned.MaxPoints;

            menuItem.Categories = ParseTocCategories(item.Data);
            bool isCatSet = false;

            if (!menuItem.Categories.IsNullOrEmpty() && !string.IsNullOrEmpty(categoryId))
            {
                var cat = menuItem.Categories.FirstOrDefault(c => c.Id == categoryId);
                if (cat != null)
                {
                    menuItem.Sequence = cat.Sequence;
                    menuItem.ParentId = cat.ItemParentId;
                    isCatSet = true;
                }
            }

            if (!isCatSet && !item.Data.Descendants("bfw_toc_contents").IsNullOrEmpty())
            {
                var elm = item.Data.Descendants("bfw_toc_contents").FirstOrDefault();
                if (elm != null)
                {
                    menuItem.Sequence = elm.Attribute("sequence").Value;
                    menuItem.ParentId = elm.Attribute("parentid").Value;
                }
            }

            menuItem.Description = GetDescendantAsString(item.Data, "description");

            if (context != null)
            {
                menuItem.Hidden = item.ItemIsHiddenFromStudents(context);
            }

            if (!item.Children.IsNullOrEmpty())
            {
                foreach (var child in item.Children)
                {
                    menuItem.Children.Add(ParseItem(child, context, categoryId));
                }
                menuItem.Children = menuItem.Children.OrderBy(m => m.Sequence).ToList();
            }

            if (!string.IsNullOrEmpty(item.LockedCourseType))
            {
                menuItem.LockedCourseType = item.LockedCourseType;
            }

            return menuItem;
        }

        /// <summary>
        /// Determines the BFW type of the an agilix item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        private static string DetermineBfwType(AgxDC.Item item)
        {
            var type = "None";

            var bizType = (item.Type == Bfw.Agilix.Dlap.DlapItemType.Custom) ? item.CustomType : item.Type.ToString();
            var bizSubtype = "";

            var subtype = item.Data.Descendants("bfw_type").FirstOrDefault();
            if (null != subtype)
            {
                bizSubtype = subtype.Value;
            }
            else if (!item.Data.Descendants("bsi_type").IsNullOrEmpty())
            {
                bizSubtype = item.Data.Descendants("bsi_type").First().Value;
            }

            type = string.IsNullOrEmpty(bizSubtype) ? bizType : bizSubtype;

            if (type.ToLower() == "bfw_toc_document")
                type = "Folder";

            if (type.ToLower() == "bfw_document")
                type = "None";

            if (type.ToLower() == "assessment")
                type = "Quiz";

            if (type.ToLower() == "bfw_qti_document")
                type = "Quiz";

            return type;
        }

        /// <summary>
        /// Builds out a default Url for a MenuItem using the item's type and id.
        /// The resulting Url looks like:
        /// {type}/{id}
        /// For example, "Assignment/123"
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        private static string BuildDefaultUrl(AgxDC.Item item)
        {
            var url = string.Empty;
            Agx.DlapItemType itemType = item.Type;

            if (Agx.DlapItemType.CustomActivity == itemType)
                itemType = Agx.DlapItemType.AssetLink;

            if (Agx.DlapItemType.AssetLink == itemType)
            {
                var href = item.Data.Element("href");

                if (null != href)
                    url = "/" + href.Value;
            }

            if (string.IsNullOrEmpty(url))
            {
                var type = (Agx.DlapItemType.Custom == itemType ? item.CustomType : Enum.GetName(typeof(Agx.DlapItemType), itemType));
                url = string.Format("./{0}/{1}", type, item.Id);
            }

            return url;
        }

        /// <summary>
        /// Convert from an agilix object to user info.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static UserInfo ToUserInfo(this AgxDC.AgilixUser user)
        {
            UserInfo bizUser = null;

            if (null != user)
            {
                bizUser = new UserInfo();
                bizUser.Id = user.Id;
                bizUser.FirstName = user.FirstName;
                bizUser.LastName = user.LastName;
                bizUser.Username = user.UserName;
                bizUser.ReferenceId = user.Reference;

                if (user.Credentials != null)
                {
                    if (!string.IsNullOrEmpty(user.Credentials.Username))
                        bizUser.Username = user.Credentials.Username;

                    if (!string.IsNullOrEmpty(user.Credentials.Password))
                        bizUser.Password = user.Credentials.Password;
                }

                //else
                //{
                //    bizUser.Username = "";
                //    bizUser.Password = "";
                //}
                bizUser.Email = user.Email;

                if (user.Domain != null)
                {
                    bizUser.DomainId = user.Domain.Id;
                    bizUser.DomainName = user.Domain.Name;
                }
                else
                {
                    bizUser.DomainId = "";
                    bizUser.DomainName = "";
                }
                bizUser.LastLogin = user.LastLogin;
                bizUser.ReferenceId = user.Reference;
                if (null != user.Data)
                {
                    bizUser.Properties = ParseContentProperties(user.Data);
                }

            }

            return bizUser;
        }

        /// <summary>
        /// Convert from an agilix object to user info.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static AgxDC.AgilixUser ToUserInfo(this UserInfo user)
        {
            AgxDC.AgilixUser agxUser = null;

            if (null != user)
            {
                var credentials = new AgxDC.Credentials() { Username = user.Username };
                var domain = new AgxDC.Domain() { Id = user.DomainId, Name = user.DomainName == string.Empty ? "root" : user.DomainName };

                agxUser = new AgxDC.AgilixUser();
                agxUser.Id = user.Id;
                agxUser.FirstName = user.FirstName;
                agxUser.LastName = user.LastName;
                agxUser.Credentials = credentials;
                agxUser.UserName = user.Username;
                agxUser.Email = user.Email;
                agxUser.Domain = domain;

                agxUser.LastLogin = user.LastLogin;
                agxUser.Reference = user.ReferenceId;
                agxUser.Properties = GetXElementFromProperties(user.Properties);
            }

            return agxUser;
        }

        /// <summary>
        /// Convert from an agilix object to grade.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Grade ToGrade(this AgxDC.Grade agx)
        {
            Grade biz = null;

            if (null != agx)
            {
                biz = new Grade();
                biz.GradedItem = agx.Item.ToContentItem();
                biz.ItemId = biz.GradedItem.Id;
                biz.ItemName = biz.GradedItem.Title;
                biz.Achieved = agx.Achieved;
                biz.Attempts = agx.Attempts;
                biz.Letter = agx.Letter;
                biz.Possible = agx.Possible;
                biz.RawAchieved = agx.RawAchieved;
                biz.RawPossible = agx.RawPossible;
                biz.ScoredDate = agx.ScoredDate;
                biz.ScoredVersion = agx.ScoredVersion;
                biz.SubmittedVersion = agx.SubmittedVersion;
                biz.SubmittedDate = agx.SubmittedDate;
                biz.Status = (GradeStatus)agx.Status;
                biz.Rule = biz.GradedItem.AssignmentSettings.GradeRule;
            }

            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to categorygrade.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static CategoryGrade ToCategoryGrade(this AgxDC.CategoryGrade agx)
        {
            CategoryGrade biz = null;

            if (null != agx)
            {
                biz = new CategoryGrade();
                biz.Id = agx.Id;
                biz.Name = agx.Name;
                biz.Achieved = agx.Achieved;
                biz.Letter = agx.Letter;
                biz.Possible = agx.Possible;
            }

            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to group.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Group ToGroup(this AgxDC.Group agx)
        {
            return new Group()
            {
                Id = agx.Id,
                Name = agx.Title,
                OwnerId = agx.OwnerId,
                Reference = agx.Reference,
                DomainId = agx.DomainId,
                SetId = agx.SetId,
                Members = agx.MemberEnrollments.Map(e => e.ToEnrollment())
            };
        }

        /// <summary>
        /// Convert from an agilix object to course.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Course ToCourse(this AgxDC.Course agx)
        {
            Course biz = null;

            if (null != agx)
            {
                biz = new Course()
                {
                    Id = agx.Id,
                    Title = agx.Title,
                    Domain = agx.Domain != null ? agx.Domain.ToDomain() : null,
                    ParentId = agx.ParentId,
                    ActivatedDate = agx.ActivatedDate,
                    CreationDate = agx.CreationDate
                };

                if (agx.Data != null)
                {
                    if (agx.Data.Element("passingscore") != null)
                    {
                        biz.PassingScore = Double.Parse(agx.Data.Element("passingscore").Value) * 100;
                    }
                    else
                    {
                        biz.PassingScore = 0;
                    }

                    if (agx.Data.Element("isdashboardactive") != null)
                    {
                        biz.IsDashboardActive = bool.Parse(agx.Data.Element("isdashboardactive").Value);

                    }
                    else
                    {
                        biz.IsDashboardActive = false;

                    }
                    XElement cssParams = agx.Data.Element("cssparameters");
                    if (cssParams != null)
                    {
                        foreach (var cssParam in cssParams.Elements())
                        {
                            biz.TitleCSSParameters.Add(new CSSParameter()
                                {
                                    Name = cssParam.Attribute("name").Value,
                                    Value = cssParam.Attribute("value").Value
                                });
                        }
                    }

                    XElement courseDisciplineAbbreviation = agx.Data.Element("CourseDisciplineAbbreviation");
                    if (courseDisciplineAbbreviation != null)
                    {
                        biz.CourseDisciplineAbbreviation = courseDisciplineAbbreviation.Value;
                    }

                    XElement questionBankRepositoryCourse = agx.Data.Element("QuestionBankRepositoryCourse");
                    if (questionBankRepositoryCourse != null)
                    {
                        biz.QuestionBankRepositoryCourse = questionBankRepositoryCourse.Value;
                    }

                    var routeData = System.Web.HttpContext.Current == null ? null : System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));
                    if (routeData != null)
                    {
                        if (routeData.Values["course"] != null)
                        {
                            biz.SubType = routeData.Values["course"] as String;

                        }

                        if (routeData.Values["section"] != null)
                        {
                            biz.CourseSectionType = routeData.Values["section"] as String;

                        }
                    }

                    if (agx.Data.Element("sourceid") != null)
                    {
                        biz.DerivedCourseId = agx.Data.Element("sourceid").Value.ToString();

                    }

                    if (agx.Data.Element("meta-available-question-data") != null)
                    {
                        biz.QuestionFilter = ParseQuestionFilter(agx.Data);
                    }

                    DashboardSettings dashboardSettings = new DashboardSettings();

                    if (agx.Data.Element("bfw_dashboard_settings") != null)
                    {

                        var dashboardData = agx.Data.Element("bfw_dashboard_settings");
                        bool outValue = false;
                        if (dashboardData.Element("instructor_dashboard") != null)
                        {
                            bool.TryParse(dashboardData.Element("instructor_dashboard").Value.ToLowerInvariant(), out outValue);
                            dashboardSettings.IsInstructorDashboardOn = outValue;
                            outValue = false;
                        }

                        if (dashboardData.Element("program_dashboard") != null)
                        {
                            bool.TryParse(dashboardData.Element("program_dashboard").Value.ToLowerInvariant(), out outValue);
                            dashboardSettings.IsProgramDashboardOn = outValue;
                            outValue = false;
                        }
                        if (dashboardData.Element("bfw_dashboard_course_home_page") != null)
                        {
                            dashboardSettings.DashboardHomePageStart = dashboardData.Element("bfw_dashboard_course_home_page").Value;

                        }
                        if (dashboardData.Element("bfw_program_dashboard_course_home_page") != null)
                        {
                            dashboardSettings.ProgramDashboardHomePageStart = dashboardData.Element("bfw_program_dashboard_course_home_page").Value;

                        }

                    }
                    else
                    {
                        dashboardSettings.IsInstructorDashboardOn = false;
                        dashboardSettings.IsProgramDashboardOn = false;
                    }
                    biz.Properties = ParseContentProperties(agx.Data);
                    biz.Metadata = ParseContentMetadata(agx.Data);
                    biz.Categories = ParseTocCategories(agx.Data);
                    biz.GroupSets = ParseGroupSets(agx.Data);
                    biz.SearchSchema = ParseSearchSchema(agx.Data);
                    biz.FacetSearchSchema = ParseFacetSchema(agx.Data);
                    biz.RSSCourseFeeds = ParseAvailableRssFeeds(agx.Data);
                    biz.ContactInformation = ParseAvailableContactInfo(agx.Data);
                    biz.TinyMCE = ParseTinyMCESettings(agx.Data);
                    biz.RubricTypes = ParseRubricTypes(agx.Data);

                    biz.SyllabusType = "Url";
                    biz.Syllabus = string.Empty;
                    biz.DashboardSettings = dashboardSettings;
                    if (agx.Data.Descendants("bfw_syllabus_info") != null && agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault() != null)
                    {
                        biz.SyllabusType = agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault().Value;
                    }

                    if (agx.Data.Descendants("bfw_syllabus_info") != null && agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault() != null)
                    {
                        biz.Syllabus = agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault().Value;
                    }

                    biz.AssessmentConfiguration = ParseAssessmentConfiguration(agx.Data);

                    if (biz.CourseType == CourseType.ProgramDashboard.ToString())
                    {
                        // currently there is no way of coding an update to assessment confiuations
                        // since program managers are the only screen that could change this setting it was placed here
                        biz.AssessmentConfiguration.Objectives.ShowRightColumn = false;
                        biz.AssessmentConfiguration.Rubrics.ShowRightColumn = false;
                    }

                    biz.bfw_tab_settings = ParseTabSettings(agx.Data);
                    biz.AssignTabSettings = ParseAssignTabSettings(agx.Data);

                    //load grade book categories
                    if (agx.Data.Element("categories") != null)
                    {
                        biz.GradeBookCategoryList = ParseGrageBookCategories(agx.Data.Element("categories"));
                        if (agx.Data.Element("categories").Attribute("weighted") != null)
                        {
                            biz.UseWeightedCategories = Boolean.Parse(agx.Data.Element("categories").Attribute("weighted").Value);
                        }
                    }

                    var propertyCourseId = agx.Data.Element("meta-product-course-id");
                    if (propertyCourseId != null)
                    {
                        biz.ProductCourseId = propertyCourseId.Value;

                    }

                    var propertyReadOnly = agx.Data.Element("bfw_read_only");
                    if (propertyReadOnly != null)
                    {
                        biz.ReadOnly = propertyReadOnly.Value;
                    }

                    var allowSampling = agx.Data.Element("bfw_allow_sampling");
                    if (allowSampling != null)
                    {
                        bool samplingOk = false;
                        bool.TryParse(allowSampling.Value, out samplingOk);

                        biz.AllowSampling = samplingOk;
                    }

                    var allowPurchase = agx.Data.Element("bfw_allow_purchase");
                    if (allowPurchase != null)
                    {
                        bool purchaseOk = false;
                        bool.TryParse(allowPurchase.Value, out purchaseOk);

                        biz.AllowPurchase = purchaseOk;
                    }

                    var allowTrialAccess = agx.Data.Element("bfw_allow_trial_access");
                    if (allowTrialAccess != null)
                    {
                        bool allowTrialAccessOk = false;
                        bool.TryParse(allowTrialAccess.Value, out allowTrialAccessOk);

                        biz.AllowTrialAccess = allowTrialAccessOk;
                    }

                    var allowExtraCredit = agx.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_allow_extracredit\"]");
                    if (allowExtraCredit != null)
                    {
                        biz.IsAllowExtraCredit = allowExtraCredit.Value.Equals("true");
                    }

                    var lockedLOs = biz.LockedLearningObjectives;
                    foreach (String nodeName in new string[]
                                                    {
                                                        "bfw_template_learningobjectives",
                                                        "bfw_dashboard_learningobjectives",
                                                        "bfw_eportfolio_learningobjectives",
                                                        "bfw_program_learningobjectives"
                                                    })
                    {
                        var learningObjectiveType = agx.Data.Element(nodeName);
                        if (learningObjectiveType == null)
                            continue; // if learning objective type element could not be found, skip it

                        var lObjectives = learningObjectiveType.Element("learningobjectives");
                        if (lObjectives == null)
                            continue; // if learning objective elements could not be found, skip it

                        var learningObjectives = new List<LearningObjective>();
                        foreach (var objective in lObjectives.Elements("objective"))
                        {
                            var learningObjective = new LearningObjective();

                            learningObjective.Guid = objective.Attribute("guid").Value;
                            learningObjective.Id = objective.Attribute("id").Value;
                            learningObjective.Title = objective.Element("description").Value;
                            if (objective.Attribute("parentid") != null)
                            {
                                learningObjective.ParentId = objective.Attribute("parentid").Value;
                            }
                            if (objective.Attribute("sequence") != null)
                            {
                                learningObjective.Sequence = objective.Attribute("sequence").Value;
                            }
                            if (objective.Element("bfw_description") != null)
                            {
                                learningObjective.Description = objective.Element("bfw_description").Value;
                            }
                            learningObjective.IsLocked = lockedLOs.Contains(learningObjective.Guid);

                            learningObjectives.Add(learningObjective);
                        }

                        if (nodeName == "bfw_template_learningobjectives")
                            biz.PublisherTemplateLearningObjectives = learningObjectives;
                        else if (nodeName == "bfw_dashboard_learningobjectives")
                            biz.DashboardLearningObjectives = learningObjectives;
                        else if (nodeName == "bfw_eportfolio_learningobjectives")
                            biz.EportfolioLearningObjectives = learningObjectives;
                        else if (nodeName == "bfw_program_learningobjectives")
                            biz.ProgramLearningObjectives = learningObjectives;
                    }

                    bool flag = false;
                    string genericCourseId = string.Empty;

                    if (agx.Data.Element("bfw_generic_course") != null)
                    {
                        bool.TryParse(agx.Data.Element("bfw_generic_course").Value, out flag);
                        genericCourseId = (agx.Data.Element("bfw_generic_course").Attribute("id") == null) ? string.Empty : agx.Data.Element("bfw_generic_course").Attribute("id").Value;
                    }
                    biz.GenericCourseSupported = flag;
                    biz.GenericCourseId = genericCourseId;

                    flag = false;
                    if (agx.Data.Element("bfw_enrollment_switch") != null)
                    {
                        bool.TryParse(agx.Data.Element("bfw_enrollment_switch").Value, out flag);
                    }
                    biz.EnrollmentSwitchSupported = flag;

                    flag = false;
                    if (agx.Data.Element("bfw_course_domain_switch") != null)
                    {
                        bool.TryParse(agx.Data.Element("bfw_course_domain_switch").Value, out flag);
                    }
                    biz.DomainSwitchSupported = flag;


                    biz.CourseSubType = string.Empty;
                    if (agx.Data.Element("meta-bfw_course_subtype") != null)
                    {
                        biz.CourseSubType = agx.Data.Element("meta-bfw_course_subtype").Value.ToString();
                    }

                    bool bRequired;
                    if (agx.Data.Element("bfw_lmsid_required") != null)
                    {
                        bool.TryParse(agx.Data.Element("bfw_lmsid_required").Value, out bRequired);
                        biz.LmsIdRequired = bRequired;
                    }

                    biz.LmsIdLabel = string.Empty;
                    if (agx.Data.Element("bfw_lmsid_label") != null)
                    {
                        biz.LmsIdLabel = agx.Data.Element("bfw_lmsid_label").Value;
                    }

                    biz.LmsIdPrompt = string.Empty;
                    if (agx.Data.Element("bfw_lmsid_prompt") != null)
                    {
                        biz.LmsIdPrompt = agx.Data.Element("bfw_lmsid_prompt").Value;
                    }

                    biz.QuestionCardLayout = string.Empty;
                    if (agx.Data.Element("questioncardlayout") != null)
                    {
                        biz.QuestionCardLayout = agx.Data.Element("questioncardlayout").ToString();
                    }

                    if (agx.QuestionCardData != null)
                    {
                        biz.QuestionCardData = agx.QuestionCardData.Map(i => i.ToQuestionCardData()).ToList();
                    }

                    if (agx.Data.Element("enableArgaUrlMapping") != null)
                    {
                        bool enableArgaUrlMapping;
                        bool.TryParse(agx.Data.Element("enableArgaUrlMapping").Value, out enableArgaUrlMapping);
                        biz.EnableArgaUrlMapping = enableArgaUrlMapping;
                    }
                    else
                    {
                        biz.EnableArgaUrlMapping = true;
                    }

                    if (agx.Data.Element("bfw_disable_comments") != null)
                    {
                        bool disableComments;
                        bool.TryParse(agx.Data.Element("bfw_disable_comments").Value, out disableComments);
                        biz.DisableComments = disableComments;
                    }
                }

                if (!agx.LearningObjectives.IsNullOrEmpty())
                {
                    var learningObjectives = new List<LearningObjective>();
                    foreach (var objective in agx.LearningObjectives)
                    {
                        var bizObjective = new LearningObjective()
                        {
                            Id = objective.Id,
                            ParentId = objective.ParentId,
                            Title = objective.Title,
                            Sequence = objective.Sequence,
                            Description = objective.Description,
                            Group = objective.Group,
                            Guid = objective.Guid
                        };

                        learningObjectives.Add(bizObjective);
                    }

                    biz.LearningObjectives = learningObjectives;
                }


                if (biz.QuestionCardData != null && biz.LearningObjectives != null)
                {
                    QuestionCardData qcd = biz.QuestionCardData.Find(dasd => dasd.FriendlyName == "learning objectives");
                    if (qcd != null)
                    {
                        foreach (LearningObjective lo in biz.LearningObjectives)
                        {
                            qcd.QuestionValues.Add(lo.Guid + '|' + lo.Title);
                        }
                    }
                }

                if (!biz.EportfolioLearningObjectives.IsNullOrEmpty())
                {
                    var guids = biz.EportfolioLearningObjectives.Map(x => x.Guid).ToList();
                    var learningObjectives = new List<LearningObjective>();
                    foreach (String guid in guids)
                    {
                        LearningObjective lo = null;
                        if (null == lo && !biz.PublisherTemplateLearningObjectives.IsNullOrEmpty())
                            lo = biz.PublisherTemplateLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.ProgramLearningObjectives.IsNullOrEmpty())
                            lo = biz.ProgramLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.DashboardLearningObjectives.IsNullOrEmpty())
                            lo = biz.DashboardLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.EportfolioLearningObjectives.IsNullOrEmpty())
                            lo = biz.EportfolioLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));

                        learningObjectives.Add(lo);
                    }
                    biz.EportfolioLearningObjectives = learningObjectives;
                }
                if (!biz.LearningObjectives.IsNullOrEmpty())
                {
                    var guids = biz.LearningObjectives.Map(x => x.Guid).ToList();
                    var learningObjectives = new List<LearningObjective>();
                    foreach (String guid in guids)
                    {
                        LearningObjective lo = null;
                        if (null == lo && !biz.PublisherTemplateLearningObjectives.IsNullOrEmpty())
                            lo = biz.PublisherTemplateLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.ProgramLearningObjectives.IsNullOrEmpty())
                            lo = biz.ProgramLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.DashboardLearningObjectives.IsNullOrEmpty())
                            lo = biz.DashboardLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
                        if (null == lo && !biz.LearningObjectives.IsNullOrEmpty())
                            lo = biz.LearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));

                        learningObjectives.Add(lo);
                    }
                    biz.LearningObjectives = learningObjectives;
                }


            }

            return biz;
        }
        private static Biz.DataContracts.QuestionCardData ToQuestionCardData(this AgxDC.QuestionCardData agx)
        {
            QuestionCardData questionCardData = new QuestionCardData();
            if (agx != null)
            {

                questionCardData.Filterable = agx.Filterable;
                questionCardData.FriendlyName = agx.FriendlyName;
                questionCardData.QuestionCardDataName = agx.QuestionCardDataName;

                if (agx.QuestionValues != null)
                {
                    questionCardData.QuestionValues = agx.QuestionValues.Select(v => v.Text).ToList();

                }

            }

            return questionCardData;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        private static List<GradeBookWeightCategory> ParseGrageBookCategories(XElement categories)
        {
            var gbbList = new List<GradeBookWeightCategory>();
            foreach (var category in categories.Elements("category"))
            {
                var newcategory = new GradeBookWeightCategory()
                                    {
                                        Id = category.Attribute("id").Value,
                                        Text = category.Attribute("name").Value,
                                        Weight = category.Attribute("weight").Value,
                                        DropLowest = category.Attribute("droplowest") == null ? "0" : category.Attribute("droplowest").Value,
                                        Sequence = category.Attribute("sequence") == null ? "a" : category.Attribute("sequence").Value,
                                        //ItemWeightTotal = "0",
                                        Percent = "0"
                                    };

                gbbList.Add(newcategory);
            }

            return gbbList;
        }


        /// <summary>
        /// Parses group set data out of XML Returns a list of group sets.
        /// </summary>
        /// <param name="data">The XML data.</param>
        /// <returns></returns>
        private static IEnumerable<GroupSet> ParseGroupSets(XElement data)
        {
            var groupSets = new List<GroupSet>();

            if (null != data)
            {
                var groupsetsEl = data.Element("groupsets");
                if (null != groupsetsEl)
                {
                    foreach (var groupSetEl in groupsetsEl.Elements("groupset"))
                    {
                        int id = Convert.ToInt32(groupSetEl.Attribute("id").Value);
                        string title = groupSetEl.Attribute("title").Value;

                        groupSets.Add(new GroupSet()
                        {
                            Id = id,
                            Name = title
                        });
                    }
                }
            }

            return groupSets;
        }

        /// <summary>
        /// Parses Assign Tab Settings data out of XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static AssignTabSettings ParseAssignTabSettings(XElement data)
        {
            var assignTabSettings = new AssignTabSettings();

            if (null != data)
            {
                var bfwTabSettings = data.Element("bfw_tab_settings");

                if (null != bfwTabSettings)
                {
                    var assignTab = bfwTabSettings.Element("assign_tab");

                    if (null != assignTab)
                    {
                        var showMarkGradeable = assignTab.Element("show_make_gradeable");
                        if (null != showMarkGradeable)
                        {
                            assignTabSettings.ShowMakeGradeable = Convert.ToBoolean(showMarkGradeable.Value);
                        }

                        var showAllowLateSubmissions = assignTab.Element("show_allow_late_submissions");
                        if (null != showAllowLateSubmissions)
                        {
                            assignTabSettings.ShowAllowLateSubmissions = Convert.ToBoolean(showAllowLateSubmissions.Value);
                        }

                        var showScheduleReminder = assignTab.Element("show_schedule_reminder");
                        if (null != showScheduleReminder)
                        {
                            assignTabSettings.ShowScheduleReminder = Convert.ToBoolean(showScheduleReminder.Value);
                        }

                        var showSubcontentCreation = assignTab.Element("show_subcontent_creation");
                        if (null != showSubcontentCreation)
                        {
                            assignTabSettings.ShowScheduleReminder = Convert.ToBoolean(showSubcontentCreation.Value);
                        }

                        var showAssignedSameDay = assignTab.Element("show_assigned_on_same_day");
                        if (null != showAssignedSameDay)
                        {
                            assignTabSettings.ShowAssignedSameDay = Convert.ToBoolean(showAssignedSameDay.Value);
                        }

                        var showGradebookCategory = assignTab.Element("show_gradebook_category");
                        if (null != showGradebookCategory)
                        {
                            assignTabSettings.ShowGradebookCategory = Convert.ToBoolean(showGradebookCategory.Value);
                        }

                        var showCalculationType = assignTab.Element("show_calculation_type");
                        if (null != showCalculationType)
                        {
                            assignTabSettings.ShowCalculationType = Convert.ToBoolean(showCalculationType.Value);
                        }

                        var showMarkAsComplete = assignTab.Element("show_mark_as_complete");
                        if (null != showMarkAsComplete)
                        {
                            assignTabSettings.ShowMarkAsComplete = Convert.ToBoolean(showMarkAsComplete.Value);
                        }


                        var showPointsPossible = assignTab.Element("show_points_possible");
                        if (null != showPointsPossible)
                        {
                            assignTabSettings.ShowPointsPossible = Convert.ToBoolean(showPointsPossible.Value);
                        }

                        var showSendReminder = assignTab.Element("show_send_reminder");
                        if (null != showSendReminder)
                        {
                            assignTabSettings.ShowSendReminder = Convert.ToBoolean(showSendReminder.Value);
                        }

                        var showIncludeScore = assignTab.Element("show_include_score");
                        if (null != showIncludeScore)
                        {
                            assignTabSettings.ShowIncludeScore = Convert.ToBoolean(showIncludeScore.Value);
                        }

                        var showCompletionCategory = assignTab.Element("show_completion_category");
                        if (null != showCompletionCategory)
                        {
                            assignTabSettings.ShowCompletionCategory = Convert.ToBoolean(showCompletionCategory.Value);
                        }
                    }

                }
            }

            return assignTabSettings;

        }

        /// <summary>
        /// Parse Academic Term details out of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static CourseAcademicTerm ParseAcademicTerm(XElement data)
        {
            var academicTerm = new CourseAcademicTerm();

            if (null != data)
            {
                var academicTermDetails = data.Element("bfw_academic_term");

                if (academicTermDetails != null)
                {
                    var termName = academicTermDetails.Element("term_name");
                    if (termName != null)
                    {
                        academicTerm.Name = termName.Value;
                    }

                    var termId = academicTermDetails.Element("term_id");
                    if (termId != null)
                    {
                        academicTerm.Id = termId.Value;
                    }

                    var startDate = academicTermDetails.Element("start_date");
                    if (startDate != null)
                    {
                        academicTerm.StartDate = DateTime.Parse(startDate.Value);
                    }

                    var endDate = academicTermDetails.Element("end_date");
                    if (endDate != null)
                    {
                        academicTerm.EndDate = DateTime.Parse(endDate.Value);
                    }
                }
            }

            return academicTerm;
        }

        /// <summary>
        /// Parses TOC category data out of XML and returns a list of TOC categories.
        /// </summary>
        /// <param name="data">The XML data.</param>
        /// <returns></returns>
        private static IList<TocCategory> ParseTocCategories(XElement data)
        {
            var categories = new List<TocCategory>();

            if (null != data)
            {
                var tocs = data.Descendants("bfw_tocs");

                if (!tocs.IsNullOrEmpty())
                {
                    var tocElms = from t in tocs.First().Elements()
                                  //Assuming that the type will be bfw_toc if it is under bfw_tocs.
                                  //where t.Attribute("type") != null && t.Attribute("type").Value == "bfw_toc"
                                  select t;

                    if (!tocElms.IsNullOrEmpty())
                    {
                        foreach (var toc in tocElms)
                        {
                            var cat = new TocCategory()
                            {
                                Id = toc.Name.ToString(),
                                Text = toc.Value,

                            };

                            if (toc.Attribute("parentid") != null)
                                cat.ItemParentId = toc.Attribute("parentid").Value;

                            if (toc.Attribute("sequence") != null)
                                cat.Sequence = toc.Attribute("sequence").Value;


                            if (toc.Attribute("type") != null)
                                cat.Type = toc.Attribute("type").Value;


                            categories.Add(cat);
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Converts a list of TOC categories to XML structure.
        /// </summary>
        /// <param name="data">The XML structure.</param>
        /// <param name="categories">List of TOC categories.</param>
        public static void StoreTocCategories(XElement data, IEnumerable<TocCategory> categories)
        {
            if (!categories.IsNullOrEmpty())
            {
                var tocs = data.Element("bfw_tocs");

                if (tocs == null)
                {
                    tocs = new XElement("bfw_tocs");
                }

                foreach (var cat in categories)
                {
                    if ((!string.IsNullOrEmpty(cat.Id)) && tocs.Element(cat.Id) == null)
                    {
                        string category = cat.Type.IsNullOrEmpty() ? "bfw_toc" : cat.Type;
                        tocs.Add(new XElement("" + cat.Id.ToString(),
                            new XAttribute("type", category),
                            new XAttribute("parentid", cat.ItemParentId ?? ""),
                            new XAttribute("sequence", cat.Sequence ?? ""),
                            cat.Text));
                    }
                }
                if (data.Element("bfw_tocs") == null)
                    data.Add(tocs);
            }
        }

        /// <summary>
        /// Converts a list of <see cref="AssessmentGroup"/> objects to XML structure.
        /// </summary>
        /// <param name="data">The XML structure.</param>
        /// <param name="assessmentGroups">List of assessment groups.</param>
        private static void StoreAssessmentGroup(XElement data, List<AssessmentGroup> assessmentGroups)
        {
            if (null != data.Element("homeworkgroups"))
                data.Element("homeworkgroups").Remove();

            var homeworkgroups = new XElement("homeworkgroups");

            foreach (var grp in assessmentGroups)
            {
                var groupElement = new XElement("group",
                    new XAttribute("name", grp.Name != null ? grp.Name : ""),
                    new XAttribute("attemptlimit", grp.Attempts != null ? grp.Attempts : "0"),
                    new XAttribute("timelimit", grp.TimeLimit != null ? grp.TimeLimit : "0"),
                    new XAttribute("flags", grp.Review.ToString() != null ? ((int)grp.Review).ToString() : ""),
                    new XAttribute("submissiongradeaction", grp.SubmissionGradeAction.ToString() != null ? ((int)grp.SubmissionGradeAction).ToString() : "")
                );

                if (grp.ReviewSettings != null)
                {
                    groupElement.Add(grp.ReviewSettings.ToAgilixItem());
                }

                homeworkgroups.Add(groupElement);
            }
            data.Add(homeworkgroups);
            StoreHWGroupList(data, assessmentGroups);
        }

        /// <summary>
        /// Converts a list of <see cref="AssessmentGroup"/> objects to XML structure
        /// marking them as homework groups.
        /// TODO: check whether this method is depreicated
        /// </summary>
        /// <param name="data">The XML structure.</param>
        /// <param name="assessmentGroups">The assessment groups.</param>
        private static void StoreHWGroupList(XElement data, List<AssessmentGroup> assessmentGroups)
        {
            if (null != data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"hw_group_info\"]"))
            {
                data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"hw_group_info\"]").Remove();
            }
            if (data.Element("bfw_properties") == null)
            {
                var props = new XElement("bfw_properties");
                data.Add(props);
            }
            data.Element("bfw_properties").Add(new XElement("bfw_property", new XAttribute("name", "hw_group_info"), new XAttribute("type", "String")));

            foreach (var grp in assessmentGroups)
            {
                var hwgroupsinfo = new XElement("group");
                hwgroupsinfo.Add(new XAttribute("name", grp.Name));

                hwgroupsinfo.Add(new XElement("property",
                         new XAttribute("name", "scrambled"),
                         new XAttribute("value", grp.Scrambled != null ? grp.Scrambled : "")
                    )
                );
                hwgroupsinfo.Add(new XElement("property",
                         new XAttribute("name", "hints"),
                         new XAttribute("value", grp.Hints != null ? grp.Hints : "")
                    )
                );
                data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"hw_group_info\"]").Add(hwgroupsinfo);
            }
        }

        /// <summary>
        /// Parses question filter.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static QuestionFilter ParseQuestionFilter(XElement data)
        {
            var questionFilter = new QuestionFilter();
            List<QuestionFilterMetadata> filterList = new List<QuestionFilterMetadata>();
            if (data != null)
            {
                foreach (XElement node in data.Elements("meta-available-question-data").Nodes())
                {
                    var filter = new QuestionFilterMetadata();
                    filter.Name = node.Name.LocalName;
                    if (node.Attribute("filterable") != null)
                    {
                        filter.Filterable = Convert.ToBoolean(node.Attribute("filterable").Value);
                    }
                    else
                    {
                        filter.Filterable = false;
                    }

                    if (node.Attribute("searchterm") != null)
                    {
                        filter.Searchterm = node.Attribute("searchterm").Value;
                    }
                    else
                    {
                        filter.Searchterm = node.Name.LocalName + ":";
                    }

                    if (node.Attribute("friendlyname") != null)
                    {
                        filter.Friendlyname = node.Attribute("friendlyname").Value;
                    }
                    else
                    {
                        filter.Friendlyname = char.ToUpper(node.Name.LocalName[0]) + node.Name.LocalName.Substring(1); ;
                    }
                    filterList.Add(filter);
                }
                questionFilter.FilterMetadata = filterList;
            }
            return questionFilter;
        }
        /// <summary>
        /// Parses search schema data out of XML and returns a SearchSchema object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static SearchSchema ParseSearchSchema(XElement data)
        {
            var schema = new SearchSchema();

            List<SearchCategory> catList = new List<SearchCategory>();

            if (null != data)
            {
                var schemaNode = data.Descendants("bfw_search");

                if (null != schemaNode && !schemaNode.IsNullOrEmpty())
                {
                    var searchCats = from s in schemaNode.First().Descendants("searchcategory") select s;

                    if (!searchCats.IsNullOrEmpty())
                    {
                        foreach (var elm in searchCats)
                        {
                            var cat = new SearchCategory();

                            if (elm.Attribute("title") != null)
                                cat.Title = elm.Attribute("title").Value;

                            if (elm.Attribute("searchable") != null)
                                cat.IsSearchable = Convert.ToBoolean(elm.Attribute("searchable").Value);

                            if (elm.Attribute("metadata") != null)
                            {
                                cat.AssociatedItems = elm.Attribute("metadata").Value.Split(',').ToList();
                                cat.MetaTag = elm.Attribute("metadata").Value;
                            }

                            catList.Add(cat);
                        }

                        schema.Categories = catList;
                    }
                }
            }

            return schema;
        }

        /// <summary>
        /// Parses available RSS feeds
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<RSSCourseFeed> ParseAvailableRssFeeds(XElement data)
        {
            List<RSSCourseFeed> result = new List<RSSCourseFeed>();
            if (data != null)
            {
                var schemaNode = data.Descendants("bfw_rss_feeds");
                if (schemaNode != null && !schemaNode.IsNullOrEmpty())
                {
                    foreach (var xfeed in schemaNode.Descendants("feed"))
                    {
                        RSSCourseFeed feed = new RSSCourseFeed()
                                   {
                                       Name = xfeed.Attribute("name").Value,
                                       Url = xfeed.Attribute("url").Value
                                   };
                        result.Add(feed);

                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses available contact info
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<ContactInfo> ParseAvailableContactInfo(XElement data)
        {
            List<ContactInfo> result = new List<ContactInfo>();

            if (data != null)
            {
                var schemaNode = data.Descendants("bfw_contact_info");

                if (schemaNode != null && !schemaNode.IsNullOrEmpty())
                {
                    foreach (var xfeed in schemaNode.Descendants("info"))
                    {
                        ContactInfo info = new ContactInfo()
                        {
                            ContactType = xfeed.Attribute("ContactType").Value,
                            ContactValue = xfeed.Attribute("ContactValue").Value
                        };

                        result.Add(info);
                    }
                }
            }
            return result;
        }

        private static tinyMCE ParseTinyMCESettings(XElement data)
        {
            tinyMCE result = new tinyMCE();

            if (data != null)
            {
                var schemaNode = data.Descendants("bfw_tinymce");

                if (schemaNode != null && !schemaNode.IsNullOrEmpty())
                {
                    result.EditorOptions = schemaNode.Descendants("options").SingleOrDefault() != null ? schemaNode.Descendants("options").SingleOrDefault().Value : string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse rubric types allowed in the course.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> ParseRubricTypes(XElement data)
        {
            List<string> rubricTypes = new List<string>();
            if (null != data)
            {
                var schemaNode = data.Descendants("bfw_rubric_types");

                if (schemaNode != null && !schemaNode.IsNullOrEmpty())
                {
                    foreach (var xfeed in schemaNode.Descendants("rubric_type"))
                    {
                        rubricTypes.Add(xfeed.Value);
                    }
                }
            }

            return rubricTypes;
        }

        /// <summary>
        /// Parses facet search schema data out of XML and returns a FacetSearchSchema object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static FacetSearchSchema ParseFacetSchema(XElement data)
        {
            var schema = new FacetSearchSchema();

            List<FacetSearchCategory> catList = new List<FacetSearchCategory>();

            if (null != data)
            {
                var schemaNode = data.Descendants("bfw_facets");

                if (null != schemaNode && !schemaNode.IsNullOrEmpty())
                {
                    var facetCats = from s in schemaNode.First().Descendants("facet") select s;

                    if (!facetCats.IsNullOrEmpty())
                    {
                        foreach (var elm in facetCats)
                        {
                            var cat = new FacetSearchCategory();

                            if (elm.Attribute("title") != null)
                                cat.Title = elm.Attribute("title").Value;


                            if (elm.Attribute("metadatakey") != null)
                            {
                                cat.MetaDataKey = elm.Attribute("metadatakey").Value;
                            }


                            if (elm.Attribute("default") != null)
                            {
                                bool def = false;
                                bool.TryParse(elm.Attribute("default").Value, out def);
                                cat.Default = def;
                            }
                            catList.Add(cat);
                        }

                        schema.Categories = catList;
                    }
                }
            }

            return schema;
        }

        /// <summary>
        /// Parses course assessment configuration data out of XML and returns a CourseAssessmentConfiguration object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static CourseAssessmentConfiguration ParseAssessmentConfiguration(XElement data)
        {
            var config = new CourseAssessmentConfiguration();
            var rubricConfig = config.Rubrics;
            var objectiveConfig = config.Objectives;
            var reportConfig = config.Reports;
            if (null != data)
            {
                XElement configNode = null;
                var rubricStyle = data.Descendants("rubric_management_style");
                if (!rubricStyle.IsNullOrEmpty())
                {
                    configNode = rubricStyle.First();
                }

                if (null != configNode)
                {
                    if (configNode.Element("show_left_column") != null)
                        rubricConfig.ShowLeftColumn = Convert.ToBoolean(configNode.Element("show_left_column").Value);

                    if (configNode.Element("show_right_column") != null)
                        rubricConfig.ShowRightColumn = Convert.ToBoolean(configNode.Element("show_right_column").Value);

                    if (configNode.Element("show_edit_on_left") != null)
                        rubricConfig.ShowEditOnLeft = Convert.ToBoolean(configNode.Element("show_edit_on_left").Value);

                    if (configNode.Element("show_edit_on_right") != null)
                        rubricConfig.ShowEditOnRight = Convert.ToBoolean(configNode.Element("show_edit_on_right").Value);

                    if (configNode.Element("show_preview_on_left") != null)
                        rubricConfig.ShowPreviewOnLeft = Convert.ToBoolean(configNode.Element("show_preview_on_left").Value);

                    if (configNode.Element("show_preview_on_right") != null)
                        rubricConfig.ShowPreviewOnRight = Convert.ToBoolean(configNode.Element("show_preview_on_right").Value);

                    if (configNode.Element("show_view_alignments") != null)
                        rubricConfig.ShowViewAlignments = Convert.ToBoolean(configNode.Element("show_view_alignments").Value);

                    if (configNode.Element("show_in_assessment") != null)
                        rubricConfig.ShowInAssessment = Convert.ToBoolean(configNode.Element("show_in_assessment").Value);
                    else
                        rubricConfig.ShowInAssessment = true;

                    if (configNode.Element("show_delete_on_left") != null)
                        rubricConfig.ShowDeleteOnLeft = Convert.ToBoolean(configNode.Element("show_delete_on_left").Value);
                    else
                        rubricConfig.ShowDeleteOnLeft = false;

                    var alignmentNode = configNode.Element("rubric_alignment_views");
                    if (alignmentNode != null)
                    {
                        if (alignmentNode.Element("rubric") != null)
                            rubricConfig.ShowRubricAlignments = Convert.ToBoolean(alignmentNode.Element("rubric").Value);

                        if (alignmentNode.Element("assignment") != null)
                            rubricConfig.ShowAssignmentAlignments = Convert.ToBoolean(alignmentNode.Element("assignment").Value);
                    }
                }

                var objectiveStyle = data.Descendants("objective_management_style");
                if (!objectiveStyle.IsNullOrEmpty())
                {
                    configNode = objectiveStyle.First();
                }

                if (null != configNode)
                {
                    if (configNode.Element("show_left_column") != null)
                        objectiveConfig.ShowLeftColumn = Convert.ToBoolean(configNode.Element("show_left_column").Value);

                    if (configNode.Element("show_right_column") != null)
                        objectiveConfig.ShowRightColumn = Convert.ToBoolean(configNode.Element("show_right_column").Value);

                    if (configNode.Element("show_edit_on_left") != null)
                        objectiveConfig.ShowEditOnLeft = Convert.ToBoolean(configNode.Element("show_edit_on_left").Value);

                    if (configNode.Element("show_edit_on_right") != null)
                        objectiveConfig.ShowEditOnRight = Convert.ToBoolean(configNode.Element("show_edit_on_right").Value);

                    if (configNode.Element("show_view_alignments") != null)
                        objectiveConfig.ShowViewAlignments = Convert.ToBoolean(configNode.Element("show_view_alignments").Value);

                    if (configNode.Element("show_in_assessment") != null)
                        objectiveConfig.ShowInAssessment = Convert.ToBoolean(configNode.Element("show_in_assessment").Value);
                    else
                        objectiveConfig.ShowInAssessment = true;

                    var alignmentNode = configNode.Element("learningobj_alignment_views");
                    if (alignmentNode != null)
                    {
                        if (alignmentNode.Element("objective") != null)
                            objectiveConfig.ShowObjectiveAlignments = Convert.ToBoolean(alignmentNode.Element("objective").Value);

                        if (alignmentNode.Element("assignment") != null)
                            objectiveConfig.ShowAssignmentAlignments = Convert.ToBoolean(alignmentNode.Element("assignment").Value);
                    }
                }

                var reportStyle = data.Descendants("report_management_style");
                if (!reportStyle.IsNullOrEmpty())
                {
                    configNode = reportStyle.First();
                }

                if (null != configNode)
                {
                    if (configNode.Element("show_in_assessment") != null)
                        reportConfig.ShowInAssessment = Convert.ToBoolean(configNode.Element("show_in_assessment").Value);
                    else
                        reportConfig.ShowInAssessment = true;
                }
            }

            return config;
        }

        private static TabSettings ParseTabSettings(XElement data)
        {
            TabSettings tab_settings = new TabSettings();
            if (null != data.Element("bfw_tab_settings"))
            {
                var vt = data.Element("bfw_tab_settings").Element("view_tab");

                if (null != vt)
                {
                    tab_settings.view_tab.show_policies = Convert.ToBoolean(vt.GetDescendantAsString("show_policies"));
                    tab_settings.view_tab.show_assignment_details = Convert.ToBoolean(vt.GetDescendantAsString("show_assignment_details"));
                    tab_settings.view_tab.show_rubrics = Convert.ToBoolean(vt.GetDescendantAsString("show_rubrics"));
                    tab_settings.view_tab.show_learning_objectives = Convert.ToBoolean(vt.GetDescendantAsString("show_learning_objectives"));
                }

            }

            return tab_settings;
        }
        public static AgxDC.DashboardSettings ToDashboardSettings(this DashboardSettings biz)
        {
            var model = new AgxDC.DashboardSettings();
            if (biz.IsInstructorDashboardOn != null)
            {
                model.IsInstructorDashboardOn = biz.IsInstructorDashboardOn;
            }
            else
            {
                model.IsInstructorDashboardOn = false;
            }
            if (biz.ProgramDashboardHomePageStart != null)
            {
                model.ProgramDashboardHomePageStart = biz.ProgramDashboardHomePageStart;
            }
            if (biz.DashboardHomePageStart != null)
            {
                model.DashboardHomePageStart = biz.DashboardHomePageStart;
            }

            if (biz.IsProgramDashboardOn != null)
            {
                model.IsProgramDashboardOn = biz.IsProgramDashboardOn;
            }
            else
            {
                model.IsProgramDashboardOn = false;
            }
            return model;
        }

        /// <summary>
        /// Convert from course to an agilix object.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        public static AgxDC.Course ToCourse(this Course course)
        {
            var agxCourse = new AgxDC.Course();

            if (null != course)
            {

                agxCourse.Id = course.Id;
                agxCourse.Title = course.Title;
                agxCourse.Domain = course.Domain.ToDomain();
                agxCourse.ActivatedDate = course.ActivatedDate;
                agxCourse.CourseSubType = course.CourseSubType;
                agxCourse.PassingScore = AdjustPassingScore(course.PassingScore);
                agxCourse.UseWeightedCategories = course.UseWeightedCategories;
                agxCourse.Data = GetXElementFromProperties(course.Properties, "course");

                var passingScore = new XElement("passingscore");
                passingScore.Value = agxCourse.PassingScore.ToString();
                agxCourse.Data.Add(passingScore);

                //add gradebook categories list
                if (!course.GradeBookCategoryList.IsNullOrEmpty())
                {
                    var gbb_categories = new XElement("categories");

                    XAttribute weighted = new XAttribute("weighted", agxCourse.UseWeightedCategories.ToString().ToLower());
                    gbb_categories.Add(weighted);

                    foreach (var category in course.GradeBookCategoryList)
                    {
                        var newCategory = new XElement("category",
                                                       new XAttribute("id", category.Id),
                                                       new XAttribute("name", category.Text),
                                                       new XAttribute("weight", (category.Weight == null) ? "0" : category.Weight.ToString(CultureInfo.InvariantCulture)),
                                                       new XAttribute("droplowest", (category.DropLowest == null) ? "0" : category.DropLowest),
                                                       new XAttribute("sequence", (category.Sequence == null) ? "0" : category.Sequence));

                        gbb_categories.Add(newCategory);
                    }

                    agxCourse.Data.Add(gbb_categories);
                }

                var contactInfo = new XElement("bfw_contact_info");
                agxCourse.Data.Add(contactInfo);

                if (!course.ContactInformation.IsNullOrEmpty())
                {
                    foreach (var info in course.ContactInformation)
                    {
                        var newInfo = new XElement("info", new XAttribute("ContactType", info.ContactType), new XAttribute("ContactValue", info.ContactValue));
                        contactInfo.Add(newInfo);
                    }
                }

                if (course.Syllabus != null)
                {
                    var syllabusInfo = new XElement("bfw_syllabus_info");
                    agxCourse.Data.Add(syllabusInfo);

                    var type = new XElement("syllabus_type", course.SyllabusType);
                    var value = new XElement("syllabus_value", course.Syllabus);

                    syllabusInfo.Add(type);
                    syllabusInfo.Add(value);
                }

                //agxCourse.Data.Add(course.LmsIdRequired);
                //agxCourse.Data.Add(course.LmsIdPrompt);
                //agxCourse.Data.Add(course.LmsIdLabel);

                var learningobjectives = new XElement("learningobjectives");
                agxCourse.Data.Add(learningobjectives); // allowed to be empty

                if (!course.LearningObjectives.IsNullOrEmpty())
                {
                    foreach (var objective in course.LearningObjectives)
                    {
                        var newObjective = new XElement("objective",
                            new XAttribute("guid", objective.Guid),
                            new XAttribute("id", objective.Id)
                            //new XAttribute("group", objective.Group)
                        );

                        newObjective.Add(new XElement("description", objective.Title));
                        learningobjectives.Add(newObjective);
                    }
                }

                foreach (String nodeName in new string[]
                                                {
                                                    "bfw_template_learningobjectives",
                                                    "bfw_dashboard_learningobjectives",
                                                    "bfw_eportfolio_learningobjectives",
                                                    "bfw_program_learningobjectives"
                                                })
                {
                    IEnumerable<LearningObjective> objectives = null;
                    if (false) ; // done for alignment purposes 
                    else if (nodeName == "bfw_template_learningobjectives") objectives = course.PublisherTemplateLearningObjectives;
                    else if (nodeName == "bfw_dashboard_learningobjectives") objectives = course.DashboardLearningObjectives;
                    else if (nodeName == "bfw_eportfolio_learningobjectives") objectives = course.EportfolioLearningObjectives;
                    else if (nodeName == "bfw_program_learningobjectives") objectives = course.ProgramLearningObjectives;

                    // create header nodes for learning objective type
                    var bfwObjectives = new XElement(nodeName);
                    var learningObjectives = new XElement("learningobjectives");
                    bfwObjectives.Add(learningObjectives);

                    agxCourse.Data.Add(bfwObjectives); // allowed to be converted to empty

                    // skip learning objective type if no objects exist for that type
                    if (objectives.IsNullOrEmpty())
                        continue;

                    foreach (var objective in objectives)
                    {
                        var newObjective = new XElement("objective",
                            new XAttribute("guid", objective.Guid),
                            new XAttribute("id", objective.Id),
                            new XAttribute("parentid", !String.IsNullOrEmpty(objective.ParentId) ? objective.ParentId : ""),
                            new XAttribute("sequence", !String.IsNullOrEmpty(objective.Sequence) ? objective.Sequence : "")
                            //new XAttribute("group", objective.Group)
                        );

                        if (!String.IsNullOrEmpty(objective.Title))
                        {
                            newObjective.Add(new XElement("description", objective.Title));
                        }
                        if (!String.IsNullOrEmpty(objective.Description))
                        {
                            newObjective.Add(new XElement("bfw_description", objective.Description));
                        }


                        learningObjectives.Add(newObjective);
                    }
                }

                if (!string.IsNullOrEmpty(course.ProductCourseId))
                {
                    agxCourse.Data.Add(new XElement("meta-product-course-id", course.ProductCourseId));

                    agxCourse.Data.Add(new XElement("meta-bfw_course_subtype", course.CourseSubType));
                }

                var lmsidRequired = new XElement("bfw_lmsid_required", course.LmsIdRequired);
                agxCourse.Data.Add(lmsidRequired);

                if (course.LmsIdLabel != null)
                {
                    var lmsidLabel = new XElement("bfw_lmsid_label", course.LmsIdLabel);
                    agxCourse.Data.Add(lmsidLabel);
                }

                if (course.LmsIdPrompt != null)
                {
                    var lmsidPrompt = new XElement("bfw_lmsid_prompt", course.LmsIdPrompt);
                    agxCourse.Data.Add(lmsidPrompt);
                }

                /* Updating Flags or Value in case it is Product Course Id */
                if (string.IsNullOrEmpty(course.ProductCourseId) || course.Id == course.ProductCourseId)
                {
                    if (course.GenericCourseSupported)
                    {
                        var genericElement = new XElement("bfw_generic_course", course.GenericCourseSupported);
                        genericElement.Add(new XAttribute("id", course.GenericCourseId));
                        agxCourse.Data.Add(genericElement);
                    }

                    if (course.EnrollmentSwitchSupported)
                    {
                        agxCourse.Data.Add("bfw_enrollment_switch", course.EnrollmentSwitchSupported);
                    }

                    if (course.DomainSwitchSupported)
                    {
                        agxCourse.Data.Add("bfw_course_domain_switch", course.DomainSwitchSupported);
                    }
                }

                var dashboard_settings = new XElement("bfw_dashboard_settings");


                var dashboard_course_home_page = new XElement("bfw_dashboard_course_home_page");
                var instructor_dashboard_on = new XElement("instructor_dashboard");
                var program_dashboard_on = new XElement("program_dashboard");
                var program_dashboard_course_home_page = new XElement("bfw_program_dashboard_course_home_page");

                if (course.DashboardSettings != null)
                {
                    dashboard_course_home_page.Add(course.DashboardSettings.DashboardHomePageStart);
                    instructor_dashboard_on.Add(course.DashboardSettings.IsInstructorDashboardOn);
                    program_dashboard_on.Add(course.DashboardSettings.IsProgramDashboardOn);
                    program_dashboard_course_home_page.Add(course.DashboardSettings.ProgramDashboardHomePageStart);
                }

                dashboard_settings.Add(dashboard_course_home_page);
                dashboard_settings.Add(instructor_dashboard_on);
                dashboard_settings.Add(program_dashboard_on);
                dashboard_settings.Add(program_dashboard_course_home_page);
                
                agxCourse.Data.Add(dashboard_settings);
                DashboardSettings d = new DashboardSettings();
                if (course.DashboardSettings != null)
                {
                    d.DashboardHomePageStart = course.DashboardSettings.DashboardHomePageStart;
                    d.IsInstructorDashboardOn = course.DashboardSettings.IsInstructorDashboardOn;
                    d.IsProgramDashboardOn = course.DashboardSettings.IsProgramDashboardOn;
                    d.ProgramDashboardHomePageStart = course.DashboardSettings.ProgramDashboardHomePageStart;
                }
                agxCourse.DashboardSettings = d.ToDashboardSettings();

                agxCourse.SubType = course.SubType;

                var derivedCourseId = new XElement("sourceid");

                agxCourse.DerivedCourseId = course.DerivedCourseId;
                derivedCourseId.Add(course.DerivedCourseId);
                agxCourse.Data.Add(derivedCourseId);

                var course_subtype = new XElement("meta-bfw_course_subtype");
                course_subtype.Add(course.CourseSubType);
                agxCourse.Data.Add(course_subtype);
            }


            return agxCourse;
        }



        /// <summary>
        /// Convert from domain to an agilix object.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns></returns>
        public static AgxDC.Domain ToDomain(this Domain domain)
        {
            return (domain != null) ?
                new AgxDC.Domain()
                {
                    Id = domain.Id,
                    Name = domain.Name,
                    Reference = domain.Reference,
                    Userspace = domain.Userspace,
                    Data = domain.Data
                } : null;
        }

        /// <summary>
        /// Convert from an agilix object to domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <returns></returns>
        public static Domain ToDomain(this AgxDC.Domain domain)
        {
            Dictionary<string, string> customQuestionUrls = new Dictionary<string, string>();
            try
            {
                if (domain != null && domain.Data != null)
                {
                    var customLocations = domain.Data.XPathSelectElements("//customization/customquestions/customquestion");
                    if (!customLocations.IsNullOrEmpty())
                    {
                        customQuestionUrls = customLocations.ToDictionary(cl => cl.Attribute("name").Value, cl => cl.Attribute("location").Value);
                    }
                }
            }
            catch
            {
                // Just don't set the hts player url if it wasn't found
            }

            return (domain != null) ?
                new Domain()
                {
                    Id = domain.Id,
                    Name = domain.Name,
                    Reference = domain.Reference,
                    Userspace = domain.Userspace,
                    CustomQuestionUrls = customQuestionUrls,
                    Data = domain.Data
                } : null;
        }

        /// <summary>
        /// Convert from an agilix object to enrollment.
        /// </summary>
        /// <param name="agx">The agx.</param>
        /// <returns></returns>
        public static Enrollment ToEnrollment(this AgxDC.Enrollment agx)
        {
            Enrollment biz = null;

            if (null != agx)
            {
                biz = new Enrollment();
                biz.Course = agx.Course.ToCourse();
                biz.CourseId = agx.CourseId;

                if (!agx.ItemGrades.IsNullOrEmpty())
                {
                    biz.ItemGrades = agx.ItemGrades.Map(g => g.ToGrade());
                }

                if (!agx.CategoryGrades.IsNullOrEmpty())
                {
                    biz.CategoryGrades = agx.CategoryGrades.Map(g => g.ToCategoryGrade());
                }

                biz.Id = agx.Id;
                biz.User = agx.User.ToUserInfo();
                biz.PercentGraded = agx.PercentGraded;
                if (agx.OverallGrade != null && agx.OverallGrade.Possible != 0)
                {
                    biz.OverallAchieved = agx.OverallGrade.Achieved;
                    biz.OverallPossible = agx.OverallGrade.Possible;
                    biz.OverallGrade = String.Format("{0:P2}", (agx.OverallGrade.Achieved / agx.OverallGrade.Possible));
                }
                else
                {
                    biz.OverallGrade = "Unknown";
                }
                biz.DomainId = agx.Domain.Id;
                biz.EndDate = agx.EndDate;
                biz.Flags = agx.Flags.ToString();
                biz.Reference = agx.Reference;
                biz.Schema = agx.Schema;
                biz.StartDate = agx.StartDate;
                biz.Status = agx.Status;
                //biz.Bookmarks = ParseBookmarks(agx);
            }

            return biz;
        }

        /// <summary>
        /// Convert from domain to an agilix object.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.Enrollment ToEnrollment(this Enrollment biz)
        {
            var agx = new AgxDC.Enrollment()
            {
                Id = biz.Id,
                Course = biz.Course.ToCourse(),
                User = new AgxDC.AgilixUser() { Id = biz.User.Id },
                PercentGraded = biz.PercentGraded,
                Domain = new AgxDC.Domain() { Id = biz.DomainId },
                StartDate = biz.StartDate,
                EndDate = biz.EndDate,
                Flags = (Bfw.Agilix.Dlap.DlapRights)Enum.Parse(typeof(Bfw.Agilix.Dlap.DlapRights), biz.Flags),
                Reference = biz.Reference,
                Schema = biz.Schema,
                Status = biz.Status
            };

            //agx.Data.Add(StoreBookmarks(biz.Bookmarks));

            return agx;
        }

        /// <summary>
        /// Convert from QuestionChoice to an agilix object.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.QuestionChoice ToQuestionChoice(this QuestionChoice biz)
        {
            var agx = new AgxDC.QuestionChoice();

            if (biz != null)
            {
                agx.Id = biz.Id;
                agx.Text = biz.Text;
                agx.Feedback = biz.Feedback;
                agx.Answer = biz.Answer;
            }

            return agx;
        }

        /// <summary>
        /// Convert the learning curve question settings.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.LearningCurveQuestionSettings ToLearningCurveQuestionSettings(this LearningCurveQuestionSettings biz)
        {
            var agx = new AgxDC.LearningCurveQuestionSettings();
            if (biz != null)
            {
                agx.Id = biz.Id;
                agx.DifficultyLevel = biz.DifficultyLevel;
                agx.NeverScrambleAnswers = biz.NeverScrambleAnswers;
                agx.PrimaryQuestion = biz.PrimaryQuestion;
            }
            return agx;
        }

        /// <summary>
        /// Toes the learning curve question settings.
        /// </summary>
        /// <param name="agx">The agx.</param>
        /// <returns></returns>
        public static LearningCurveQuestionSettings ToLearningCurveQuestionSettings(this AgxDC.LearningCurveQuestionSettings agx)
        {
            return new LearningCurveQuestionSettings()
            {
                Id = agx.Id,
                DifficultyLevel = agx.DifficultyLevel,
                NeverScrambleAnswers = agx.NeverScrambleAnswers,
                PrimaryQuestion = agx.PrimaryQuestion
            };
        }

        /// <summary>
        /// Convert from Question to an agilix object.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.Question ToQuestion(this Question biz)
        {
            var agx = new AgxDC.Question();

            if (null != biz)
            {
                agx.AnswerList = biz.AnswerList;
                agx.Body = biz.Body;
                agx.GeneralFeedback = biz.GeneralFeedback;
                agx.Title = biz.Title;
                agx.CustomUrl = biz.CustomUrl;
                if (!biz.Choices.IsNullOrEmpty())
                {
                    agx.Choices = biz.Choices.Map(c => c.ToQuestionChoice()).ToList();
                }
                agx.Id = biz.Id;
                agx.EntityId = biz.EntityId;
                agx.Score = biz.Points;
                agx.InteractionData = biz.InteractionData;
                agx.Interaction = TypeConversion.ConvertType<QuestionInteraction, AgxDC.QuestionInteraction>(biz.Interaction, null);
                agx.InteractionType = biz.InteractionType.ToString();
                agx.AssessmentGroups = biz.AssessmentGroups;
                agx.SearchableMetaData = biz.SearchableMetaData;
                agx.Title = biz.Title;
                agx.ExcerciseNo = biz.ExcerciseNo;
                agx.QuestionStatus = biz.QuestionStatus;

                agx.Difficulty = biz.Difficulty;
                agx.CognitiveLevel = biz.CongnitiveLevel;
                agx.BloomsDomain = biz.BloomsDomain;
                agx.Guidance = biz.Guidance;
                agx.FreeResponseQuestion = biz.FreeResponseQuestion;
                agx.SuggestedUse = biz.SuggestedUse;
                agx.LearningObjectives = biz.LearningObjectives;
                agx.EbookSectionText = biz.EbookSectionText;
                agx.eBookChapter = biz.EbookSectionText;
                agx.QuestionBank = biz.QuestionBank;
                agx.QuestionBankText = biz.QuestionBankText;
                if (biz.QuestionBankText != null)
                {
                    agx.QuestionBankText = biz.QuestionBankText;
                }

                if (biz.EbookSectionText != null)
                {

                    agx.EbookSectionText = biz.EbookSectionText;
                }
                agx.LearningCurveQuestionSettings = biz.LearningCurveQuestionSettings == null ? null : biz.LearningCurveQuestionSettings.Map(l => l.ToLearningCurveQuestionSettings()).ToList();
            }

            return agx;
        }

        public static Question ToQuestion(this RespondusQuestion respondusQuestion)
        {
            List<QuestionChoice> choices = new List<QuestionChoice>();
            var correctChoices = new List<RespondusQuestionChoice>();

            foreach (var respondusQuestionChoice in respondusQuestion.Choices)
            {
                if (respondusQuestionChoice.IsCorrect)
                {
                    correctChoices.Add(respondusQuestionChoice);
                }

                choices.Add(new QuestionChoice()
                    {
                        Id = respondusQuestionChoice.Id,
                        Text = respondusQuestionChoice.Text
                    });
            }

            List<string> answers = new List<string>();

            foreach (var choice in correctChoices)
            {
                answers.Add(choice.Id);
            }

            Question question = new Question()
                {
                    Id = respondusQuestion.Id,
                    Title = respondusQuestion.Title,
                    GeneralFeedback = respondusQuestion.Feedback,
                    Body = respondusQuestion.Text,
                    Points = respondusQuestion.Points.HasValue ? respondusQuestion.Points.Value : 0,
                    Choices = choices,
                    AnswerList = answers,
                    InteractionType = GetInteractionTypeFromRespondusType(respondusQuestion.Type)
                };

            return question;
        }

        public static Question ToQuestion(this SearchResultDoc doc)
        {
            var type = InteractionType.Choice;
            Enum.TryParse(doc.dlap_q_type, true, out type);
            var question_id = "";
            try
            {
                question_id = doc.dlap_id.Split('|')[2];
            }
            catch (Exception)
            {

                throw new Exception("Question Id not found in search result");
            }

            var title = string.Empty;
            doc.Metadata.TryGetValue("title", out title);
            if (title.IsNullOrEmpty())
            {
                title = doc.dlap_title;
            }
            if (title.IsNullOrEmpty())
            {
                title = doc.dlap_html_text.Split('|').FirstOrDefault().Trim();
            }
            
            
            var question = new Question()
            {
                Id = question_id,
                Body = doc.dlap_html_text.Split('|').FirstOrDefault(),
                Choices = new List<QuestionChoice>(),
                Title = title,
                Points = doc.dlap_q_score,
                InteractionType = type,
                SearchableMetaData = doc.Metadata,
                EntityId =  doc.entityid,
                //LearningObjectives = doc.dlap_objective_text - TODO: figure out this mapping
            };
            foreach (var choice in doc.dlap_html_text.Split('|').Skip(1))
            {
                question.Choices.Add(new QuestionChoice()
                {
                    Text = choice,
                    Id = "unknown"
                });
            }
            return question;
        }

        private static InteractionType GetInteractionTypeFromRespondusType(RespondusType type)
        {
            InteractionType result = InteractionType.Custom;

            switch (type)
            {
                case RespondusType.MultipleChoice:
                    result = InteractionType.Choice;
                    break;
                case RespondusType.FillInBlank:
                    result = InteractionType.Answer;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Copies a business resource entity to an agilix resource entity.
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
        public static AgxDC.Resource ToResource(this Resource biz)
        {
            if (biz is XmlResource)
            {
                return ((XmlResource)biz).ToResource();
            }
            else
            {
                var resource = new AgxDC.Resource();

                if (null != biz)
                {
                    resource.EntityId = biz.EntityId;
                    resource.ContentType = biz.ContentType;
                    resource.Extension = biz.Extension;
                    resource.Status = (Bfw.Agilix.DataContracts.ResourceStatus)Enum.Parse(typeof(Bfw.Agilix.DataContracts.ResourceStatus), biz.Status.ToString());
                    resource.Url = biz.Url;

                    var source = biz.GetStream();
                    var dest = resource.GetStream();
                    source.Copy(dest);

                    dest.Flush();
                    dest.Seek(0, System.IO.SeekOrigin.Begin);
                }

                return resource;
            }
        }

        /// <summary>
        /// Copies a business resource entity to an agilix resource entity.
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
        public static AgxDC.Resource ToResource(this XmlResource biz)
        {
            var rez = new AgxDC.Resource();

            if (null != biz)
            {
                rez.EntityId = biz.EntityId;
                rez.ContentType = biz.ContentType;
                rez.Extension = biz.Extension;
                rez.Status = (Bfw.Agilix.DataContracts.ResourceStatus)Enum.Parse(typeof(Bfw.Agilix.DataContracts.ResourceStatus), biz.Status.ToString());
                rez.Url = biz.Url;

                var xDoc = new XmlDocument();

                xDoc.LoadXml("<xmlResource></xmlResource>"); //Created the Parent Node.

                XmlNode xTitleNode = xDoc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode xBodyNode = xDoc.CreateNode(XmlNodeType.Element, "body", "");

                XmlCDataSection cdata = xDoc.CreateCDataSection(biz.Body);
                xBodyNode.AppendChild(cdata);

                XmlNode xTypeNode = xDoc.CreateNode(XmlNodeType.Element, "ContentType", "");
                XmlNode xExtensionNode = xDoc.CreateNode(XmlNodeType.Element, "Extension", "");
                XmlNode xUrlNode = xDoc.CreateNode(XmlNodeType.Element, "Url", "");
                XmlNode xEntityIdNode = xDoc.CreateNode(XmlNodeType.Element, "EntityId", "");

                xTitleNode.InnerText = biz.Title;
                xTypeNode.InnerText = biz.ContentType;
                xExtensionNode.InnerText = biz.Extension;
                xUrlNode.InnerText = biz.Url;
                xEntityIdNode.InnerText = biz.EntityId;

                xDoc.DocumentElement.AppendChild(xTitleNode);
                xDoc.DocumentElement.AppendChild(xBodyNode);
                xDoc.DocumentElement.AppendChild(xTypeNode);
                xDoc.DocumentElement.AppendChild(xExtensionNode);
                xDoc.DocumentElement.AppendChild(xUrlNode);
                xDoc.DocumentElement.AppendChild(xEntityIdNode);

                XmlNode extendedNode = xDoc.CreateNode(XmlNodeType.Element, "ExtendedProperties", "");
                xDoc.DocumentElement.AppendChild(extendedNode);
                if (biz.ExtendedProperties != null)
                {
                    foreach (var KeyItem in biz.ExtendedProperties)
                    {
                        var childNode = xDoc.CreateNode(XmlNodeType.Element, KeyItem.Key, "");
                        childNode.InnerText = KeyItem.Value;
                        extendedNode.AppendChild(childNode);
                    }

                }
                string outputXml = xDoc.DocumentElement.OuterXml;

                var source = biz.GetStream();
                var dest = rez.GetStream();

                var sw = new System.IO.StreamWriter(dest);
                sw.Write(System.Web.HttpUtility.HtmlDecode(outputXml));
                sw.Flush();

                dest.Flush();
                dest.Seek(0, System.IO.SeekOrigin.Begin);
            }

            return rez;
        }

        /// <summary>
        /// Convert from an agilix object to resource.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Resource ToResource(this AgxDC.Resource agx)
        {
            var resource = new Resource();

            if (null != agx)
            {
                resource.EntityId = agx.EntityId;
                resource.ContentType = agx.ContentType;
                resource.Extension = agx.Extension;
                resource.Status = (Bfw.PX.Biz.DataContracts.ResourceStatus)Enum.Parse(typeof(Bfw.PX.Biz.DataContracts.ResourceStatus), agx.Status.ToString());
                resource.Url = agx.Url;
                resource.CreationDate = agx.CreationDate;
                resource.ModifiedDate = agx.ModifiedDate;

                if (String.IsNullOrEmpty(resource.Extension))
                    resource.Extension = System.IO.Path.GetExtension(resource.Url).Replace(".", "");

                var source = agx.GetStream();
                var dest = resource.GetStream();
                source.Copy(dest);

                dest.Flush();
                dest.Seek(0, System.IO.SeekOrigin.Begin);
            }

            return resource;
        }

        /// <summary>
        /// Convert from an agilix object to XML resource.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static XmlResource ToXmlResource(this AgxDC.Resource agx)
        {
            var resource = new XmlResource();

            if (null != agx)
            {
                resource.EntityId = agx.EntityId;
                resource.ContentType = agx.ContentType;
                resource.Extension = agx.Extension;
                resource.Status = (Bfw.PX.Biz.DataContracts.ResourceStatus)Enum.Parse(typeof(Bfw.PX.Biz.DataContracts.ResourceStatus), agx.Status.ToString());
                resource.Url = agx.Url;
                resource.CreationDate = agx.CreationDate;
                resource.ModifiedDate = agx.ModifiedDate;

                if (String.IsNullOrEmpty(resource.Extension))
                    resource.Extension = System.IO.Path.GetExtension(resource.Url).Replace(".", "");

                var source = agx.GetStream();
                var dest = resource.GetStream();
                source.Copy(dest);

                dest.Flush();
                dest.Seek(0, System.IO.SeekOrigin.Begin);

                var sr = new System.IO.StreamReader(dest);

                string xmlDoc = sr.ReadToEnd();
                dest.Flush();
                dest.Seek(0, SeekOrigin.Begin);

                if (xmlDoc.Contains("&nbsp;"))
                    xmlDoc = xmlDoc.Replace("&nbsp;", " ");

                XDocument xDoc = XDocument.Parse(xmlDoc);

                XElement xElement = xDoc.XPathSelectElement("//body");

                if (xElement != null)
                {
                    string body = xElement.Value;
                    dest.SetLength(0);
                    var sw = new System.IO.StreamWriter(dest);
                    sw.Write(System.Web.HttpUtility.HtmlDecode(body));
                    sw.Flush();
                }

                xElement = xDoc.XPathSelectElement("//title");
                if (xElement != null) resource.Name = xElement.Value ?? "Document";

                xElement = xDoc.XPathSelectElement("//ExtendedProperties");
                if (xElement != null)
                {
                    var extendedProperties = xElement.Elements().ToList<XElement>();
                    if (!extendedProperties.IsNullOrEmpty())
                    {
                        var comment = extendedProperties.FirstOrDefault(x => x.Name.ToString() == "Comment");
                        if (comment != null && !comment.IsEmpty)
                        {
                            resource.ExtendedProperties.Add(comment.Name.ToString(), comment.ToString().Replace("\r\n", ""));
                            extendedProperties.Remove(comment);
                        }

                        foreach (XElement prop in extendedProperties)
                        {
                            resource.ExtendedProperties.Add(prop.Name.ToString(), prop.Value);
                        }
                    }
                }

            }

            return resource;
        }

        /// <summary>
        /// Converts a properties collection to XML.
        /// </summary>
        /// <param name="Properties">The properties collection.</param>
        /// <param name="subType">The sub type property.</param>
        /// <returns></returns>
        public static XElement GetXElementFromProperties(IDictionary<string, PropertyValue> Properties, string subType)
        {
            XElement data = new XElement(subType);
            data.Add(new XElement("bfw_type", subType));

            if (!Properties.IsNullOrEmpty())
            {
                var propsElm = new XElement("bfw_properties");
                foreach (var prop in Properties)
                {
                    if (!prop.Value.Values.IsNullOrEmpty())
                    {
                        foreach (var v in prop.Value.Values)
                        {
                            propsElm.Add(new XElement("bfw_property",
                                new XAttribute("name", prop.Key),
                                new XAttribute("type", prop.Value.Type.ToString()),
                                v));
                        }
                    }
                    else
                    {
                        propsElm.Add(new XElement("bfw_property",
                                new XAttribute("name", prop.Key),
                                new XAttribute("type", prop.Value.Type.ToString()),
                                prop.Value.Value));
                    }
                }
                data.Add(propsElm);
            }

            return data;
        }

        /// <summary>
        /// Converts a properties collection to XML.
        /// </summary>
        /// <param name="Properties">The properties collection.</param>
        /// <param name="subType">The sub type property.</param>
        /// <returns></returns>
        public static XElement GetXElementFromProperties(IDictionary<string, PropertyValue> Properties)
        {
            var propsElm = new XElement("bfw_properties");

            if (!Properties.IsNullOrEmpty())
            {
                foreach (var prop in Properties)
                {
                    if (!prop.Value.Values.IsNullOrEmpty())
                    {
                        foreach (var v in prop.Value.Values)
                        {
                            propsElm.Add(new XElement("bfw_property",
                                new XAttribute("name", prop.Key),
                                new XAttribute("type", prop.Value.Type.ToString()),
                                v));
                        }
                    }
                    else
                    {
                        propsElm.Add(new XElement("bfw_property",
                                new XAttribute("name", prop.Key),
                                new XAttribute("type", prop.Value.Type.ToString()),
                                prop.Value.Value));
                    }
                }
            }

            return propsElm;
        }

        /// <summary>
        /// Convert an Agilix question into business question.
        /// </summary>
        /// <param name="agx"></param>
        /// <returns></returns>
        public static Question ToQuestion(this AgxDC.Question agx)
        {
            var biz = new Question();

            if (null != agx)
            {
                biz.Id = agx.Id;
                biz.EntityId = agx.EntityId;
                biz.Body = agx.Body;
                biz.Title = agx.Title;
                biz.GeneralFeedback = agx.GeneralFeedback;
                biz.CustomUrl = agx.CustomUrl;
                biz.InteractionType = (InteractionType)Enum.Parse(typeof(InteractionType), agx.InteractionType, true);
                biz.InteractionData = agx.InteractionData;
                biz.Interaction = TypeConversion.ConvertType<AgxDC.QuestionInteraction, QuestionInteraction>(agx.Interaction, null);
                biz.Points = agx.Score;
                biz.Choices = agx.Choices== null? null : agx.Choices.Map(c => c.ToQuestionChoice()).ToList();
                biz.AnswerList = agx.AnswerList;
                biz.QuestionXml = agx.QuestionXml;
                biz.QuestionMetaData = agx.QuestionMetaData;
                biz.SearchableMetaData = agx.SearchableMetaData;
                biz.LearningCurveQuestionSettings = agx.LearningCurveQuestionSettings == null ? null : agx.LearningCurveQuestionSettings.Map(c => c.ToLearningCurveQuestionSettings()).ToList();
                biz.AssessmentGroups = agx.AssessmentGroups;
                biz.ExcerciseNo = agx.ExcerciseNo;
                biz.Difficulty = agx.Difficulty;
                biz.CongnitiveLevel = agx.CognitiveLevel;
                biz.BloomsDomain = agx.BloomsDomain;
                biz.Guidance = agx.Guidance;
                biz.FreeResponseQuestion = agx.FreeResponseQuestion;
                biz.UsedIn = agx.UsedIn;
                biz.EbookSectionText = agx.EbookSectionText;
                biz.eBookChapter = agx.eBookChapter;
                biz.QuestionBank = agx.QuestionBank;
                biz.QuestionBankText = agx.QuestionBankText;
                biz.QuestionStatus = agx.QuestionStatus;


                if (agx.SelectedLearningObjectives != null)
                {
                    biz.SelectedLearningObjectives = agx.SelectedLearningObjectives;

                }
                if (agx.SelectedSuggestedUse != null)
                {
                    biz.SelectedSuggestedUse = agx.SelectedSuggestedUse;

                }
                biz.QuestionVersion = string.IsNullOrEmpty(agx.QuestionVersion) == true ? "" : agx.QuestionVersion;
                if (!string.IsNullOrEmpty(agx.QuestionXml))
                {
                    XDocument doc = XDocument.Parse(agx.QuestionXml);
                    if (doc.Element("question") != null)
                    {
                        var d = doc.Element("question").Attribute("modifieddate").Value;
                        if (!string.IsNullOrEmpty(d))
                            biz.ModifiedDate = DateTime.Parse(d);
                    }
                }

            }

            return biz;
        }



        /// <summary>
        /// Convert from an agilix object to question analysis.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static QuestionAnalysis ToQuestionAnalysis(this AgxDC.QuestionAnalysis agx)
        {
            var biz = new QuestionAnalysis();

            if (null != agx)
            {
                if (agx.QuestionId.Contains(':'))
                {
                    var questionId = agx.QuestionId.Split(':');
                    biz.QuestionId = questionId[1];
                }
                else
                {
                    biz.QuestionId = agx.QuestionId;
                }
                biz.Version = agx.Version;
                biz.QuestionNumber = agx.QuestionNumber;
                biz.Enrollments = agx.Enrollments;
                biz.Attempts = agx.Attempts;
                biz.CorrectAnswerCount = agx.CorrectAnswerCount;
                biz.Correlation = agx.Correlation;
                biz.Score = agx.Score;
                biz.AverageTime = agx.AverageTime;
            }

            return biz;
        }

        /// <summary>
        /// Converts a ContentItem business entity into an agilix item.
        /// </summary>
        /// <param name="biz">The ContentItem business entity.</param>
        /// <returns></returns>
        public static AgxDC.Item ToItem(this ContentItem biz)
        {
            //TODO: Throw mapping exception is Type isn't set.  We can do this once we merge release-2013.10.21

            var item = new AgxDC.Item();

            if (null != biz)
            {
                item.Id = biz.Id;
                item.Title = biz.Title;
                item.Description = biz.Description;
                item.Type = (Bfw.Agilix.Dlap.DlapItemType)Enum.Parse(typeof(Bfw.Agilix.Dlap.DlapItemType), biz.Type);
                item.ParentId = biz.ParentId;
                item.SubTitle = biz.SubTitle;
                item.DueDateGrace = biz.DueDateGrace;
                item.IsStudentCreatedFolder = biz.IsStudentCreatedFolder;
                item.Containers = GetContainers(biz.Containers);
                item.SubContainerIds = GetContainers(biz.SubContainerIds);
                item.GradeFlags = (AgxDC.GradeFlags) biz.GradeFlags;
                item.AdjustedGroups = biz.AdjustedGroups;

                if (null != biz.AssignmentSettings)
                {
                    item.IsGradable = biz.AssignmentSettings.IsAssignable;
                    item.DueDate = biz.AssignmentSettings.DueDate;
                    item.meta_bfw_Assigned = biz.AssignmentSettings.meta_bfw_Assigned;
                    item.DropBox = (biz.AssignmentSettings.DropBoxType != DropBoxType.None);
                    item.Category = biz.AssignmentSettings.Category;
                    item.CompletionTrigger = (AgxDC.CompletionTrigger)biz.AssignmentSettings.CompletionTrigger;
                    item.TimeToComplete = biz.AssignmentSettings.TimeToComplete;
                    item.PassingScore = biz.AssignmentSettings.PassingScore;
                    item.MaxPoints = biz.AssignmentSettings.Points;
                    item.Weight = biz.AssignmentSettings.Points;
                    item.CategorySequence = biz.AssignmentSettings.CategorySequence;
                    item.IsMarkAsCompleteChecked = biz.AssignmentSettings.IsMarkAsCompleteChecked;
                    item.IsAllowLateSubmission = biz.AssignmentSettings.AllowLateSubmission;
                    if (biz.AssignmentSettings.IsAllowLateGracePeriod)
                    {
                        item.DueDateGrace = CalculateGraceDurationInMinute(biz.AssignmentSettings.LateGraceDuration,
                            biz.AssignmentSettings.LateGraceDurationType);
                    }
                    else
                    {
                        item.DueDateGrace = 0;
                    }

                    item.SubmissionGradeAction = (AgxDC.SubmissionGradeAction)biz.AssignmentSettings.SubmissionGradeAction;

                    if (item.DropBox)
                    {
                        item.DropBoxType = (AgxDC.DropBoxType)biz.AssignmentSettings.DropBoxType;
                    }
                    if (!string.IsNullOrEmpty(biz.AssignmentSettings.Rubric))
                    {
                        item.HasRubric = true;
                        item.Rubric = biz.AssignmentSettings.Rubric;
                    }
                }

                if (null != biz.AssessmentSettings)
                {
                    item.AttemptLimit = biz.AssessmentSettings.AttemptLimit;

                    item.GradeRule = (AgxDC.GradeRule)biz.AssessmentSettings.GradeRule;
                    item.TimeLimit = biz.AssessmentSettings.TimeLimit;
                    item.QuestionsPerPage = (AgxDC.QuestionDelivery)biz.AssessmentSettings.QuestionDelivery;
                    item.AllowSaveAndContinue = biz.AssessmentSettings.AllowSaveAndContinue;
                    item.AutoSubmitAssessments = biz.AssessmentSettings.AutoSubmitAssessments;
                    item.ShuffleQuestions = biz.AssessmentSettings.RandomizeQuestionOrder;
                    item.ShuffleAnswers = biz.AssessmentSettings.RandomizeAnswerOrder;
                    item.AllowViewHints = biz.AssessmentSettings.AllowViewHints;
                    item.PercentSubstractHint = biz.AssessmentSettings.PercentSubstractHint;
                    item.ShowScoreAfter = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowScoreAfter;
                    item.ShowQuestionsAnswers = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowQuestionsAnswers;
                    item.ShowRightWrong = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowRightWrong;
                    item.ShowAnswers = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowAnswers;
                    item.ShowFeedbackAndRemarks = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowFeedbackAndRemarks;
                    item.ShowSolutions = (AgxDC.ReviewSetting)biz.AssessmentSettings.ShowSolutions;
                    item.StudentsCanEmailInstructors = biz.AssessmentSettings.StudentsCanEmailInstructors;
                }

                item.EntityId = biz.CourseId;
                item.Folder = biz.Folder;
                item.Href = biz.Href;
                item.HrefDisciplineCourseId = biz.HrefDisciplineCourseId;

                item.HiddenFromToc = biz.Hidden;
                item.HiddenFromStudents = biz.HiddenFromStudents;
                item.AvailableDate = biz.AvailableDate;
                item.Sequence = biz.Sequence;
                if (!item.CategorySequence.IsNullOrEmpty())
                {
                    item.Sequence = item.CategorySequence;      //TODO: A bug in Brainhoney's Export Gradebook functionality sorts the export by <sequence>
                    //instead of <categorysequence>. This means when we update the categorysequence, we have to
                    //update the sequence as well. Once this bug is resolved, remove this.
                }
                item.Sco = biz.Sco;
                item.LockedCourseType = biz.LockedCourseType;
                item.Data.Add(new XElement("bfw_type", biz.Subtype));

                if (!biz.Metadata.IsNullOrEmpty())
                {
                    StoreContentMetadata(item.Data, biz.Metadata);
                }

                if (biz.Attachments != null && biz.Attachments.Count > 0)
                {
                    if (item.Attachments == null)
                        item.Attachments = new List<Attachment>();
                    foreach (var attachment in biz.Attachments)
                    {
                        item.Attachments.Add(new Attachment { Href = attachment.Href });
                    }
                }

                if (biz.LearningObjectives != null)
                {
                    if (item.LearningObjectives == null)
                        item.LearningObjectives = new List<AgxDC.ItemLearningObjective>();
                    foreach (var objective in biz.LearningObjectives)
                    {
                        item.LearningObjectives.Add(new AgxDC.ItemLearningObjective { Guid = objective.Guid });
                    }
                }
                if (biz.RelatedContents != null)
                {
                    if (item.RelatedContents == null)
                        item.RelatedContents = new List<AgxDC.ItemRelatedContent>();
                    foreach (var relatedContent in biz.RelatedContents)
                    {
                        item.RelatedContents.Add(new AgxDC.ItemRelatedContent { Id = relatedContent.Id, ParentId = relatedContent.ParentId, Type = relatedContent.Type, Threshold = relatedContent.Threshold });
                    }
                }

                if (!string.IsNullOrEmpty(biz.DefaultCategoryParentId))
                {
                    SetupDefaultParent(item.Data, biz.DefaultCategoryParentId, biz.DefaultCategorySequence);
                }
                else
                {
                    SetupDefaultParent(item.Data, biz.ParentId, biz.Sequence);
                }

                if (!biz.Categories.IsNullOrEmpty())
                {
                    StoreTocCategories(item.Data, biz.Categories);
                }

                if (null != biz.AssessmentGroups && biz.AssessmentGroups.Count > 0)
                {
                    StoreAssessmentGroup(item.Data, biz.AssessmentGroups);
                }

                if (null != biz.AssessmentSettings)
                {
                    // Similar to ShowScoreAfter, AutoSubmitAtDueTime is not able to be persisted in Agilix.
                    biz.Properties["assessment_auto_submit_at_due_time"] = new PropertyValue() { Type = PropertyType.Boolean, Value = biz.AssessmentSettings.AutoSubmitAssessments };
                }
                if (!biz.Properties.IsNullOrEmpty())
                {
                    StoreContentProperties(item.Data, biz.Properties);
                }

                if (!string.IsNullOrEmpty(biz.Template))
                {
                    var templateElm = item.Data.XPathSelectElement("//bfw_template");
                    if (templateElm == null)
                    {
                        templateElm = new XElement("bfw_template");
                        item.Data.Add(templateElm);
                    }

                    templateElm.Value = biz.Template;
                }

                var inAssignmentCenter = item.Data.Element("bfw_in_assignment_center");
                if (inAssignmentCenter == null)
                {
                    inAssignmentCenter = new XElement("bfw_in_assignment_center", biz.InAssignmentCenter);
                    item.Data.Add(inAssignmentCenter);
                }

                inAssignmentCenter.Value = biz.InAssignmentCenter.ToString().ToLower();

                var wasDueDateManuallySet = item.Data.Element("bfw_duedate_manuallyset");
                if (wasDueDateManuallySet == null)
                {
                    wasDueDateManuallySet = new XElement("bfw_duedate_manuallyset", biz.WasDueDateManuallySet);
                    item.Data.Add(wasDueDateManuallySet);
                }


                wasDueDateManuallySet.Value = biz.WasDueDateManuallySet.ToString().ToLower();

                XElement properties = item.Data.Element("bfw_properties");
                if (properties != null)
                {
                    foreach (var elem in properties.Elements())
                    {
                        if (elem.Attribute("name").Value == "bfw_startdate")
                        {
                            elem.Value = biz.AssignmentSettings.StartDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                        }

                        if (elem.Attribute("name").Value == "thumbnail" && biz.Thumbnail != null)
                        {
                            elem.Value = biz.Thumbnail;
                        }

                        if (elem.Attribute("name").Value == "bfw_unit_gradebook_category" && biz.UnitGradebookCategory != null)
                        {
                            elem.Value = biz.UnitGradebookCategory;
                        }
                    }
                }

                if (biz.AssignmentFolderId != null)
                {
                    item.AssignmentFolderId = biz.AssignmentFolderId;
                }


                StoreLearningCurveProperties(biz, item);
                StoreCustomFields(biz, item);

                if (biz.GradeFlags != null)
                {
                    item.GradeFlags = (Agilix.DataContracts.GradeFlags)biz.GradeFlags;
                }
            }

            return item;
        }

        /// <summary>
        /// Calculate grace period duration in minutes
        /// </summary>
        /// <param name="duration">duration</param>
        /// <param name="type">duration unit: minute, hour, day or week</param>
        /// <returns>grace period duration in minutes</returns>
        public static long CalculateGraceDurationInMinute(long duration, string type)
        {
            if (duration == 0 || type.IsNullOrEmpty())
                return 0;

            long durationInMinute = 0;
            switch (type.ToLower())
            {
                case "minute":
                    durationInMinute = duration;
                    break;
                case "hour":
                    durationInMinute = duration * 60;
                    break;
                case "day":
                    durationInMinute = duration * 60 * 24;
                    break;
                case "week":
                    durationInMinute = duration * 60 * 24 * 7;
                    break;
                case "infinite":
                    durationInMinute = -1;
                    break;
            }
            return durationInMinute;
        }
        
        public static void StoreLearningCurveProperties(ContentItem biz, AgxDC.Item item)
        {
            if (!string.IsNullOrEmpty(biz.Subtype) && biz.Subtype.ToLowerInvariant() == "learningcurveactivity")
            {
                var learningCurve = item.Data.Element("bfw_learning_curve");
                if (learningCurve == null)
                {
                    learningCurve = new XElement("bfw_learning_curve");
                    item.Data.Add(learningCurve);
                }

                var targetscore = item.Data.Element("targetscore");
                if (targetscore == null)
                {
                    targetscore = new XElement("targetscore");
                    learningCurve.Add(targetscore);
                }

                targetscore.SetValue(biz.AssessmentSettings.LearningCurveTargetScore ?? "0");

                var autotargetscore = item.Data.Element("autotargetscore");
                if (autotargetscore == null)
                {
                    autotargetscore = new XElement("autotargetscore");
                    learningCurve.Add(autotargetscore);
                }

                autotargetscore.SetValue(biz.AssessmentSettings.AutoTargetScore);

                var autocalibrate = item.Data.Element("autocalibrate");
                if (autocalibrate == null)
                {
                    autocalibrate = new XElement("autocalibrate");
                    learningCurve.Add(autocalibrate);
                }

                autocalibrate.SetValue(biz.AssessmentSettings.AutoCalibrateDifficulty);
            }
            else
            {
                if (item.Data.Element("bfw_learning_curve") != null)
                    item.Data.Element("bfw_learning_curve").Remove();
            }
        }

        /// <summary>
        /// Store custom fields into agilix item
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="item"></param>
        private static void StoreCustomFields(ContentItem biz, AgxDC.Item item)
        {
            
            //If this item uses scrom data, set "duedategrace", "submissiongradeaction" and "customfields" elements in dlap item
            if (item.Sco)
            {
                var graceDueDate = item.Data.Element("duedategrace");
                if (null == graceDueDate)
                {
                    graceDueDate = new XElement("duedategrace", item.DueDateGrace);
                    item.Data.Add(graceDueDate);
                }
                else
                {
                    graceDueDate.Value = item.DueDateGrace.ToString();
                }
                if (biz.CustomFields.ContainsKey("duedategrace"))
                    biz.CustomFields["duedategrace"] = item.DueDateGrace.ToString();
                else
                    biz.CustomFields.Add(new KeyValuePair<string, string>("duedategrace", item.DueDateGrace.ToString()));

                bool isLearningCurve = item.Type == Bfw.Agilix.Dlap.DlapItemType.CustomActivity && item.Href != null &&
                                       item.Href.Contains("learningcurve.bfwpub.com");

                if (!isLearningCurve)
                {
                    if (biz.CustomFields.ContainsKey("submissiongradeaction"))
                        biz.CustomFields["submissiongradeaction"] = item.SubmissionGradeAction.ToString();
                    else
                        biz.CustomFields.Add(new KeyValuePair<string, string>("submissiongradeaction",
                            item.SubmissionGradeAction.ToString()));
                }
            }

            if (biz.CustomFields == null || biz.CustomFields.IsNullOrEmpty())
                return;

            XElement customFieldsElement = item.Data.Element("customfields");
            if (customFieldsElement == null)
            {
                customFieldsElement = new XElement("customfields");
                item.Data.Add(customFieldsElement);
            }

            foreach (KeyValuePair<string, string> mapping in biz.CustomFields)
            {

                var fieldElement = new XElement("field");
                fieldElement.Add(new XAttribute("name", mapping.Key));
                fieldElement.Add(new XAttribute("value", mapping.Value));
                customFieldsElement.Add(fieldElement);

            }
        }

        /// <summary>
        /// Get custom fields from agilix item
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="item"></param>
        private static void GetCustomFields(AgxDC.Item item, ContentItem biz)
        {
            XElement customFieldsElement = item.Data.Element("customfields");
            if (customFieldsElement != null)
            {
                IEnumerable<XElement> fields = customFieldsElement.Elements("field");
                foreach (XElement field in fields)
                {
                    var mapping = new KeyValuePair<string, string>(field.Attribute("name").Value, field.Attribute("value").Value);
                    biz.CustomFields.Add(mapping);

                }
            }
        }

        /// <summary>
        /// Converts a properties collection to XML.
        /// </summary>
        /// <param name="data">The XML element used to store the collection.</param>
        /// <param name="properties">The properties.</param>
        private static void StoreContentProperties(XElement data, IDictionary<string, PropertyValue> properties)
        {
            var propsElm = new XElement("bfw_properties");
            foreach (var prop in properties)
            {
                if (!prop.Value.Values.IsNullOrEmpty())
                {
                    foreach (var v in prop.Value.Values)
                    {
                        propsElm.Add(new XElement("bfw_property",
                            new XAttribute("name", prop.Key),
                            new XAttribute("type", prop.Value.Type.ToString()),
                            v));
                    }
                }
                else
                {
                    propsElm.Add(new XElement("bfw_property",
                            new XAttribute("name", prop.Key),
                            new XAttribute("type", prop.Value.Type.ToString()),
                            prop.Value.Value));
                }
            }
            data.Add(propsElm);
        }

        /// <summary>
        /// Converts a metadata properties collection to XML.
        /// </summary>
        /// <param name="data">The XML element used to store the collection.</param>
        /// <param name="metadata">The metadata collection.</param>
        private static void StoreContentMetadata(XElement data, IDictionary<string, MetadataValue> metadata)
        {
            var propsElm = new XElement("bfw_meta");
            foreach (var prop in metadata)
            {
                if (!prop.Value.Values.IsNullOrEmpty())
                {
                    foreach (var v in prop.Value.Values)
                    {
                        propsElm.Add(new XElement("bfw_metadata",
                            new XAttribute("name", prop.Key),
                            new XAttribute("type", prop.Value.Type.ToString()),
                            v));
                    }
                }
                else
                {
                    propsElm.Add(new XElement("bfw_metadata",
                            new XAttribute("name", prop.Key),
                            new XAttribute("type", prop.Value.Type.ToString()),
                            prop.Value.Value));
                }
            }
            data.Add(propsElm);
        }

        /// <summary>
        /// Sets up the default TOC parent ID in specified XML element.
        /// </summary>
        /// <param name="data">The XML element.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="sequence">The sequence.</param>
        public static void SetupDefaultParent(XElement data, string parentId, string sequence)
        {
            XElement tocs = data.Element("bfw_tocs");

            if (tocs != null)
                tocs.Remove();

            if (string.IsNullOrEmpty(sequence))
                sequence = "a";

            tocs = new XElement("bfw_tocs", new XElement("bfw_toc_contents", new XAttribute("parentid", parentId ?? ""), new XAttribute("sequence", sequence)));
            data.Add(tocs);
        }

        /// <summary>
        /// Determines if the agilix item is marked as hidden.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <returns></returns>
        public static bool ItemIsHidden(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context)
        {
            var hfs = item.ItemIsHiddenFromStudents(context);

            bool hft = item.HiddenFromToc(context);
            bool isAvailable;
            bool isStudent = context.AccessLevel == AccessLevel.Student;

            if (item.AvailableDate.Year == 1 && hfs == true && isStudent)
            {
                isAvailable = true;
            }
            else if (item.AvailableDate.Year != 1 && hfs == false && isStudent)
            {
                isAvailable = item.AvailableDate > DateTime.Now.GetCourseDateTime(context);
            }
            else
            {
                isAvailable = false;
            }

            if (hft)
            {
                return true;
            }

            if (isAvailable)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the item is marked as hidden from students.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <returns></returns>
        public static bool ItemIsHiddenFromStudents(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context)
        {
            var hfs = item.Data.XPathSelectElement("./hiddenfromstudent");
            return hfs != null ? hfs.Value == "true" : false;
        }

        /// <summary>
        /// Determines if the item is marked as hidden from toc.
        /// modified this to reflect new fields in <data></data> rather than <bfw_properties></bfw_properties>
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <returns></returns>
        public static bool HiddenFromToc(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context)
        {
            var hfs = item.Data.XPathSelectElement("./hiddenfromtoc");
            return hfs != null ? hfs.Value == "true" : false;
        }

        /// <summary>
        /// Determines if the item is marked as hidden from TOC.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <returns></returns>
        public static bool ItemIsHiddenFromToc(this AgxDC.Item item, Bfw.PX.Biz.ServiceContracts.IBusinessContext context)
        {
            var hfs = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"HideFromToc\"]");
            return hfs != null ? hfs.Value == "true" : false;
        }

        /// <summary>
        /// Pulls all information out of an agilix item and populates a content item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static ContentItem ToContentItem(this AgxDC.Item item, IBusinessContext context)
        {
            var biz = new ContentItem();

            if (null != item)
            {
                biz.Id = item.Id;
                biz.ActualEntityid = item.ActualEntityid;
                biz.Title = item.Title;
                biz.SubTitle = item.SubTitle;
                biz.Description = System.Web.HttpUtility.HtmlDecode(item.Description);
                biz.Type = (item.Type == Bfw.Agilix.Dlap.DlapItemType.Custom) ? item.CustomType : item.Type.ToString();
                biz.CourseId = item.EntityId;
                biz.ResourceEntityId = item.ResourceEntityId;
                biz.ParentId = item.ParentId;
                biz.BHParentId = item.ParentId;
                biz.Href = item.Href;
                biz.Hidden = item.HiddenFromToc;
                biz.AvailableDate = item.AvailableDate;
                biz.HiddenFromToc = item.HiddenFromToc;
                biz.Folder = item.Folder;
                biz.Sequence = item.Sequence;
                biz.Sco = item.Sco;
                biz.MaxPoints = item.MaxPoints;
                biz.CustomExamType = item.CustomExamType;
                biz.IsStudentCreatedFolder = item.IsStudentCreatedFolder;
                biz.ItemDataXml = item.Data;
                biz.AssignmentSettings = item.ToAssignmentSettings();
                biz.AssessmentSettings = item.ToAssessmentSettings();
                biz.Weight = item.Weight;
                biz.Percent = item.Percent;
                biz.Modified = item.Modified;
                biz.DueDateGrace = item.DueDateGrace;
                biz.AdjustedGroups = item.AdjustedGroups;

                if (item.ProxyConfig != null)
                {
                    biz.ProxyConfig = new ProxyConfig()
                        {
                            AllowComments = item.ProxyConfig.AllowComments
                        };
                }

                if (item.Containers != null)
                {
                    biz.Containers = GetContainers(item.Containers);
                }

                if (item.SubContainerIds != null)
                {
                    biz.SubContainerIds = GetContainers(item.SubContainerIds);
                }

                if (context != null && context.Course != null && biz.AssignmentSettings != null)
                {
                    if (!String.IsNullOrWhiteSpace(context.Course.CourseTimeZone) 
                        && TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone) != TimeZoneInfo.Local
                        && (biz.AssignmentSettings.DueDateTZ == null || biz.AssignmentSettings.DueDateTZ.TimeZone != TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone)))
                    {

                        biz.AssignmentSettings.DueDateTZ = new Bfw.Common.DateTimeWithZone(biz.AssignmentSettings.DueDate.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone), true);
                        biz.AssignmentSettings.StartDateTZ = new Bfw.Common.DateTimeWithZone(biz.AssignmentSettings.StartDate.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone), true);
                        biz.AssignmentSettings.DueDate = biz.AssignmentSettings.DueDateTZ.LocalTime;//new Bfw.Common.DateTimeWithZone(biz.AssignmentSettings.DueDate.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone), true).LocalTime;
                        biz.AssignmentSettings.StartDate = biz.AssignmentSettings.StartDateTZ.LocalTime;//new Bfw.Common.DateTimeWithZone(biz.AssignmentSettings.StartDate.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone), true).LocalTime;
                    }
                }

                if (!string.IsNullOrEmpty(item.LockedCourseType))
                {
                    biz.LockedCourseType = item.LockedCourseType;
                }

                // hack to identify folder between instructor-created and student-created
                if (!item.Folder.IsNullOrEmpty() && item.Folder.EndsWith("__(*studentfolder*)"))
                {
                    biz.IsStudentCreatedFolder = true;
                    item.Folder = item.Folder.Remove(item.Folder.IndexOf("__(*studentfolder*)"));
                }

                biz.Properties = ParseContentProperties(item.Data);
                if (item.RelatedTemplates != null)
                {
                    foreach (var template in item.RelatedTemplates)
                    {

                        biz.RelatedTemplates.Add(template.ToRelatedTemplate());
                    }
                }

                if (item.Type == Bfw.Agilix.Dlap.DlapItemType.Assessment
                    || item.Type == Bfw.Agilix.Dlap.DlapItemType.Homework
                    || (!string.IsNullOrEmpty(item.SubType) && item.SubType.ToLowerInvariant() == "learningcurveactivity"))
                {
                    if (item.Type == Bfw.Agilix.Dlap.DlapItemType.Homework)
                    {
                        item.ShuffleQuestions = false;
                    }
                    biz.AssessmentSettings = item.ToAssessmentSettings();
                    if (biz.AssessmentSettings != null && context != null && context.Course != null)
                    {
                        if (!String.IsNullOrWhiteSpace(context.Course.CourseTimeZone) && TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone) != TimeZoneInfo.Local
                            && (biz.AssessmentSettings.DueDateTZ == null || biz.AssessmentSettings.DueDateTZ.TimeZone != TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone)))
                        {
                            
                            biz.AssessmentSettings.DueDateTZ = new Bfw.Common.DateTimeWithZone(biz.AssessmentSettings.DueDate.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone), true);
                            biz.AssessmentSettings.DueDate = biz.AssessmentSettings.DueDateTZ.ServerTime;
                        }
                    }
                    biz.AssessmentGroups = item.ToAssessmentGroups();

                    // This is a hack to get around the fact that Agilix does not have a 'auto submit at due date' setting.
                    if (biz.Properties.ContainsKey("assessment_auto_submit_at_due_time"))
                    {
                        biz.AssessmentSettings.AutoSubmitAssessments = (bool)biz.Properties["assessment_auto_submit_at_due_time"].Value;
                    }
                }

                if (item.Type == Bfw.Agilix.Dlap.DlapItemType.Resource)
                {
                    if (biz.Properties.ContainsKey("CreationDate"))
                    {
                        biz.CreatedDate = Convert.ToDateTime(biz.Properties["CreationDate"].Value);
                        biz.AvailableDate = Convert.ToDateTime(biz.Properties["CreationDate"].Value);
                    }
                }

                var socialCommentingIntegration = item.Data.Descendants("bfw_social_commenting_integration").FirstOrDefault();
                if (null != socialCommentingIntegration)
                {
                    bool sci = true;
                    bool.TryParse(socialCommentingIntegration.Value, out sci);
                    biz.SocialCommentingIntegration = sci;
                }

                if (!item.Data.Descendants("xbook-assignment-sortindex").IsNullOrEmpty())
                {
                    int hasVal = 0;
                    int.TryParse(item.Data.Descendants("xbook-assignment-sortindex").First().Value, out hasVal);
                    biz.SortIndex = hasVal;
                }

                biz.Subtype = "";
                var subtype = item.Data.Descendants("bfw_type").FirstOrDefault();
                if (null != subtype)
                {
                    biz.Subtype = subtype.Value;
                }
                else if (!item.Data.Descendants("bsi_type").IsNullOrEmpty())
                {
                    biz.Subtype = item.Data.Descendants("bsi_type").First().Value;
                }

                biz.Metadata = ParseContentMetadata(item.Data);
                if (!item.Data.Descendants("bfw_unit_chapter").IsNullOrEmpty())
                {
                    biz.UnitChapter = item.Data.Descendants("bfw_unit_chapter").First().Value;
                }
                biz.FacetMetadata = ParseContentFacetMetadata(item.Data);

                ParseDefaultParent(biz, item);
                biz.Categories = ParseTocCategories(item.Data);

                var templateElm = item.Data.XPathSelectElement("//bfw_template");
                if (templateElm != null)
                {
                    biz.Template = templateElm.Value;
                }

                if (item.Href != null)
                {
                    var hrefsettings = item.Data.XPathSelectElement("//href");
                    if (hrefsettings != null && hrefsettings.HasAttributes)
                    {
                        biz.HrefDisciplineCourseId = hrefsettings.Attribute("entityid").Value ?? "";
                    }
                }

                var bfw_custom_settings = item.Data.XPathSelectElement("//bfw_custom_settings");
                if (bfw_custom_settings != null)
                {
                    var controller = bfw_custom_settings.Attribute("controller") == null ? "" : bfw_custom_settings.Attribute("controller").Value;
                    var action = bfw_custom_settings.Attribute("action") == null ? "" : bfw_custom_settings.Attribute("action").Value;
                    biz.CustomSettings = controller + "|" + action;
                }

                if (item.Attachments != null && item.Attachments.Count > 0)
                {
                    if (biz.Attachments == null)
                        biz.Attachments = new List<DataContracts.Attachment>();
                    foreach (var attachment in item.Attachments)
                    {
                        biz.Attachments.Add(new DataContracts.Attachment { Href = attachment.Href });
                    }
                }

                if (item.LearningObjectives != null)
                {
                    if (biz.LearningObjectives == null)
                        biz.LearningObjectives = new List<DataContracts.LearningObjective>();
                    foreach (var objective in item.LearningObjectives)
                    {
                        biz.LearningObjectives.Add(new DataContracts.LearningObjective { Guid = objective.Guid });
                    }
                }

                if (item.RelatedContents != null)
                {
                    if (biz.RelatedContents == null)
                        biz.RelatedContents = new List<DataContracts.RelatedContent>();
                    foreach (var content in item.RelatedContents)
                    {
                        biz.RelatedContents.Add(new DataContracts.RelatedContent
                        {
                            Id = content.Id,
                            Sequence = content.Sequence,
                            Type = content.Type,
                            ParentId = content.ParentId,
                            Threshold = content.Threshold
                        });
                    }
                }

                var questionsEl = item.Data.Elements("questions");
                if (questionsEl != null)
                {
                    var questionEls = questionsEl.Elements("question");
                    foreach (var questionEl in questionEls)
                    {
                        var entityIdEl = questionEl.Attribute("entityid");
                        var entityId = entityIdEl != null ? entityIdEl.Value : "";
                        var questionIdEl = questionEl.Attribute("id");
                        var countE1 = questionEl.Attribute("count");
                        var count = countE1 != null ? countE1.Value : "";
                        var scoreE1 = questionEl.Attribute("score");
                        var score = scoreE1 != null ? scoreE1.Value : "";
                        var questionId = questionIdEl != null ? questionIdEl.Value : "";
                        var typeEl = questionEl.Attribute("type");
                        var type = typeEl != null ? typeEl.Value : "";

                        biz.QuizQuestions.Add(new QuizQuestion() { EntityId = entityId, QuestionId = questionId, Score = score, Type = type, Count = count });
                    }
                }

                var inAssignmentCenter = item.Data.Element("bfw_in_assignment_center");
                if (inAssignmentCenter != null)
                {
                    bool inAc = false;
                    bool.TryParse(inAssignmentCenter.Value, out inAc);

                    biz.InAssignmentCenter = inAc;
                }

                var wasDueDateManuallySet = item.Data.Element("bfw_duedate_manuallyset");
                if (wasDueDateManuallySet != null)
                {
                    bool hasValue = false;
                    bool.TryParse(wasDueDateManuallySet.Value, out hasValue);

                    biz.WasDueDateManuallySet = hasValue;
                }

                var unitGradebookCategoryElement = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_unit_gradebook_category\"]");
                if (unitGradebookCategoryElement != null && !string.IsNullOrEmpty(unitGradebookCategoryElement.Value))
                {
                    biz.UnitGradebookCategory = unitGradebookCategoryElement.Value;
                }


                var dueDateGrace = item.Data.Element("duedategrace");
                if (dueDateGrace != null)
                {
                    long dueDateGraceValue;
                    long.TryParse(dueDateGrace.Value, out dueDateGraceValue);
                    biz.DueDateGrace = dueDateGraceValue;
                }
                GetCustomFields(item, biz);

                var defaultPointsElement = item.Data.Element("bfw_default_points");
                if (defaultPointsElement != null)
                {
                    int hasValue = 0;
                    int.TryParse(defaultPointsElement.Value, out hasValue);

                    biz.DefaultPoints = hasValue;
                }
                else
                {
                    defaultPointsElement = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_item_default_points\"]");

                    if (defaultPointsElement != null)
                    {
                        int hasValue = 0;
                        int.TryParse(defaultPointsElement.Value, out hasValue);

                        biz.DefaultPoints = hasValue;
                    }
                    else
                    {
                        biz.DefaultPoints = 0;
                    }
                }

                if (context != null && context.Course != null)
                {
                    biz.IsItemLocked = !string.IsNullOrEmpty(item.LockedCourseType) && item.LockedCourseType != context.Course.CourseType;
                }

                if (item.AssignmentFolderId != null)
                {
                    biz.AssignmentFolderId = item.AssignmentFolderId;
                }

                if (item.GradeFlags != null)
                {
                    biz.GradeFlags = (Biz.DataContracts.GradeFlags)item.GradeFlags;
                }

                var overrideDueDateReq = item.Data.Element("overrideDueDateReq");
                if (overrideDueDateReq != null)
                {
                    bool ovr;
                    bool.TryParse(overrideDueDateReq.Value, out ovr);
                    biz.OverrideDueDateReq = ovr;
                }

                var examTemplate = item.Data.Element("examtemplate");
                if (examTemplate != null)
                {
                    biz.ExamTemplate = examTemplate.Value;
                }
            }

            return biz;
        }

        /// <summary>
        /// Converts a Aglix ItemRelatedTemplate to Biz Related Template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static RelatedTemplate ToRelatedTemplate(this AgxDC.ItemRelatedTemplate template)
        {
            RelatedTemplate newRelatedTemplate = new RelatedTemplate();

            newRelatedTemplate.TemplateID = template.Id;
            newRelatedTemplate.DisplayName = template.Name;
            newRelatedTemplate.Message = template.Message;

            return newRelatedTemplate;

        }

        /// <summary>
        /// Pulls all information out of an agilix item and populates a content item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static ContentItem ToContentItem(this AgxDC.Item item)
        {
            return ToContentItem(item, null);
        }

        /// <summary>
        /// Created aglix data contract item from biz data contract WidgetItem
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static AgxDC.Item ToItem(this Widget bizWidget)
        {
            AgxDC.Item item = new AgxDC.Item();

            if (bizWidget == null)
            {
                return item;
            }

            item.Id = bizWidget.Id;
            item.ParentId = bizWidget.ParentId;
            item.EntityId = bizWidget.CourseID;
            item.Title = bizWidget.Title;
            item.Sequence = bizWidget.Sequence;
            //item.GradeRule = 

            item.Type = item.Type = (Bfw.Agilix.Dlap.DlapItemType)Enum.Parse(typeof(Bfw.Agilix.Dlap.DlapItemType), bizWidget.Type);
            if (!bizWidget.Properties.IsNullOrEmpty())
            {
                StoreContentProperties(item.Data, bizWidget.Properties);
            }

            item.Data.Add(new XElement("bfw_type", bizWidget.BfwType));
            item.Data.Add(new XElement("bfw_subtype", bizWidget.BfwSubType));
            item.Data.Add(new XElement("bfw_widget_template", bizWidget.Template));
            item.Data.Add(new XElement("abbreviation", bizWidget.Abbreviation));

            // add call back nodes
            if (bizWidget.Callbacks != null && bizWidget.Callbacks.Count != 0)
            {
                // if bfw_widget_callbacks node does not exist add it
                XElement elementCallback = item.Data.Element("bfw_widget_callbacks");
                if (elementCallback == null)
                {
                    elementCallback = new XElement("bfw_widget_callbacks");
                    item.Data.Add(elementCallback);
                }

                // add method nodes to bfw_widget_callbacks
                foreach (KeyValuePair<string, WidgetCallback> callback in bizWidget.Callbacks)
                {
                    XElement elementMethod = new XElement("method");
                    elementMethod.Add(new XAttribute("name", callback.Value.Name));
                    elementMethod.Add(new XAttribute("controller", callback.Value.Controller));
                    elementMethod.Add(new XAttribute("action", callback.Value.Action));
                    elementMethod.Add(new XAttribute("fne", callback.Value.IsFNE.ToString()));

                    elementCallback.Add(elementMethod);
                }
            }

            // add display options
            if (bizWidget.WidgetDisplayOptions != null && bizWidget.WidgetDisplayOptions.DisplayOptions.Count != 0)
            {
                // if bfw_widget_callbacks node does not exist add it
                XElement elementDisplayFlag = item.Data.Element("bfw_display_flags");
                if (elementDisplayFlag == null)
                {
                    elementDisplayFlag = new XElement("bfw_display_flags");
                    item.Data.Add(elementDisplayFlag);
                }

                foreach (DisplayOption dispOptions in bizWidget.WidgetDisplayOptions.DisplayOptions)
                {
                    XElement elementMethod = new XElement("display", dispOptions);
                    elementDisplayFlag.Add(elementMethod);
                }
            }
            //special widget settings
            //Instructor Console widget
            if (bizWidget.InstructorConsoleSettings != null)
            {
                bizWidget.InstructorConsoleSettings.ToInstructorConsoleSettings(item);
            }

            //Launchpad settings
            if (bizWidget is LaunchPadSettings)
            {
                ((LaunchPadSettings)bizWidget).ToItem(item);
            }

            return item;
        }

        /// <summary>
        /// Takes the item (widget) just created from DLAP and then translates it into the InstructorConsoleSettings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="item"></param>
        public static void ToInstructorConsoleSettings(this InstructorConsoleSettings settings, AgxDC.Item item)
        {
            var xmlSettings = item.Data.Element("instructor_console_settings");
            if (xmlSettings == null)
            {
                item.Data.Add(new XElement("instructor_console_settings"));
                xmlSettings = item.Data.Element("instructor_console_settings");
            }

            // adding all the sub settings
            if (settings.Resources != null)
            {
                foreach (var resource in settings.Resources)
                {
                    XElement xe = new XElement("resource");
                    xe.Add(new XAttribute("name", resource.Name));
                    xe.Add(new XAttribute("type", resource.Type));
                    xe.Add(new XAttribute("value", resource.Value));
                    xmlSettings.Add(xe);
                }
            }

            if (settings.ShowEbook)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showebook"));
                xe.Add(new XAttribute("type", "showebook"));
                xe.Add(new XAttribute("value", "showebook"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowChapters)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showchapters"));
                xe.Add(new XAttribute("type", "showchapters"));
                xe.Add(new XAttribute("value", "showchapters"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowLaunchPad)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showlaunchpad"));
                xe.Add(new XAttribute("type", "showlaunchpad"));
                xe.Add(new XAttribute("value", "showlaunchpad"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowWelcomeReturn)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showwelcomereturn"));
                xe.Add(new XAttribute("type", "showwelcomereturn"));
                xe.Add(new XAttribute("value", "showwelcomereturn"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowBatchUpdater)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showbatchupdater"));
                xe.Add(new XAttribute("type", "showbatchupdater"));
                xe.Add(new XAttribute("value", "showbatchupdater"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowManageAnnouncemets)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showmanageannouncemets"));
                xe.Add(new XAttribute("type", "showmanageannouncemets"));
                xe.Add(new XAttribute("value", "showmanageannouncemets"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowNavigation)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "shownavigation"));
                xe.Add(new XAttribute("type", "shownavigation"));
                xe.Add(new XAttribute("value", "shownavigation"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowTypes)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showtypes"));
                xe.Add(new XAttribute("type", "showtypes"));
                xe.Add(new XAttribute("value", "showtypes"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowGeneral)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showgeneral"));
                xe.Add(new XAttribute("type", "showgeneral"));
                xe.Add(new XAttribute("value", "showgeneral"));
                xmlSettings.Add(xe);
            }
            if (settings.ShowMyResources)
            {
                XElement xe = new XElement("resource");
                xe.Add(new XAttribute("name", "showmyresources"));
                xe.Add(new XAttribute("type", "showmyresources"));
                xe.Add(new XAttribute("value", "showmyresources"));
                xmlSettings.Add(xe);
            }


        }

        /// <summary>
        /// Takes the item (widget) just created from DLAP and then translates it into the LaunchPadSettings
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="item"></param>
        public static void ToItem(this LaunchPadSettings widget, AgxDC.Item item)
        {
            var xmlSettings = item.Data.Element("bfw_properties");
            if (xmlSettings == null)
            {
                item.Data.Add(new XElement("bfw_properties"));
                xmlSettings = item.Data.Element("bfw_properties");
            }
            else
            {
                xmlSettings.RemoveAll();
            }


            if (widget.Properties.ContainsKey("bfw_toggleduelater"))
            {
                widget.Properties["bfw_toggleduelater"].Value = widget.CollapseDueLater;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_toggleduelater", new PropertyValue() { Value = widget.CollapseDueLater.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_togglepastdue"))
            {
                widget.Properties["bfw_togglepastdue"].Value = widget.CollapsePastDue;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_togglepastdue", new PropertyValue() { Value = widget.CollapsePastDue.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_sortbyduedate"))
            {
                widget.Properties["bfw_sortbyduedate"].Value = widget.SortByDueDate;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_sortbyduedate", new PropertyValue() { Value = widget.SortByDueDate.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_launchpadtitle"))
            {
                widget.Properties["bfw_launchpadtitle"].Value = widget.CategoryName;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_launchpadtitle", new PropertyValue() { Value = widget.CategoryName.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_grayoutpastduelater"))
            {
                widget.Properties["bfw_grayoutpastduelater"].Value = widget.GrayoutPastDueLater;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_grayoutpastduelater", new PropertyValue() { Value = widget.GrayoutPastDueLater.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_toggleduelaterdays"))
            {
                widget.Properties["bfw_toggleduelaterdays"].Value = widget.DueLaterDays;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_toggleduelaterdays", new PropertyValue() { Value = widget.DueLaterDays.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_showcollapseunassigned"))
            {
                widget.Properties["bfw_showcollapseunassigned"].Value = widget.ShowCollapseUnassigned;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_showcollapseunassigned", new PropertyValue() { Value = widget.ShowCollapseUnassigned.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_disableediting"))
            {
                widget.Properties["bfw_disableediting"].Value = widget.DisableEditing;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_disableediting", new PropertyValue() { Value = widget.DisableEditing.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_disabledraganddrop"))
            {
                widget.Properties["bfw_disabledraganddrop"].Value = widget.DisableDragAndDrop;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_disabledraganddrop", new PropertyValue() { Value = widget.DisableDragAndDrop.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_showitemsonly"))
            {
                widget.Properties["bfw_showitemsonly"].Value = widget.ShowItemsOnly;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_showitemsonly", new PropertyValue() { Value = widget.ShowItemsOnly.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_collapseunassigned"))
            {
                widget.Properties["bfw_collapseunassigned"].Value = widget.CollapseUnassigned;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_collapseunassigned", new PropertyValue() { Value = widget.CollapseUnassigned.ToString() });
                widget.Properties.Add(kvp);
            }

            if (widget.Properties.ContainsKey("bfw_splitassigned"))
            {
                widget.Properties["bfw_splitassigned"].Value = widget.SplitAssigned;
            }
            else
            {
                KeyValuePair<string, PropertyValue> kvp = new KeyValuePair<string, PropertyValue>("bfw_splitassigned", new PropertyValue() { Value = widget.SplitAssigned.ToString() });
                widget.Properties.Add(kvp);
            }

            foreach (var key in widget.Properties.Keys)
            {
                XElement xel = new XElement("bfw_property");
                xel.Add(new XAttribute("name", key));
                xel.Add(new XAttribute("type", widget.Properties[key].Type));
                xel.Value = Convert.ToString(widget.Properties[key].Value);
                xmlSettings.Add(xel);
            }
        }
        /// <summary>
        /// Pulls all information out of an agilix item and populates a widget item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static Widget ToWidgetItem(this AgxDC.Item item)
        {
            var biz = new Widget();

            if (null != item)
            {
                biz.Id = item.Id;
                biz.CourseID = item.ResourceEntityId;
                biz.ParentId = item.ParentId;
                biz.Type = (item.Type == Bfw.Agilix.Dlap.DlapItemType.Custom) ? item.CustomType : item.Type.ToString();
                biz.Sequence = item.Sequence;
                biz.Title = item.Title;

                var BfwType = item.Data.Descendants("bfw_type").FirstOrDefault();
                if (null != BfwType)
                {
                    biz.BfwType = BfwType.Value;
                }

                var Abbreviation = item.Data.Descendants("abbreviation").FirstOrDefault();
                if (null != Abbreviation)
                {
                    biz.Abbreviation = Abbreviation.Value;
                }

                var BfwSubType = item.Data.Descendants("bfw_subtype").FirstOrDefault();
                if (null != BfwSubType)
                {
                    biz.BfwSubType = BfwSubType.Value;
                }

                var WidgetTemplate = item.Data.Descendants("bfw_widget_template").FirstOrDefault();
                if (null != WidgetTemplate)
                {
                    biz.Template = WidgetTemplate.Value;
                }

                var displaysflags = item.Data.Element("bfw_display_flags");

                List<XElement> displays = new List<XElement>();
                if (displaysflags != null)
                {
                    displays = displaysflags.Elements("display").ToList();
                }

                var widgetDisplayOptions = new WidgetDisplayOptions();

                widgetDisplayOptions.DisplayOptions = (from display in displays
                                                       select (DisplayOption)Enum.Parse(typeof(DisplayOption), display.Value, true)).ToList();

                biz.WidgetDisplayOptions = widgetDisplayOptions;

                var CallbackMethods = item.Data.Descendants("bfw_widget_callbacks").Descendants("method");
                foreach (XElement callbackMethod in CallbackMethods)
                {
                    String key = callbackMethod.Attribute("name").Value;
                    biz.Callbacks.Add(key, new WidgetCallback()
                    {
                        Name = key,
                        Controller = callbackMethod.Attribute("controller").Value,
                        Action = callbackMethod.Attribute("action").Value,
                        IsFNE = Boolean.Parse(callbackMethod.Attribute("fne").Value),
                        IsASync = false // default to false
                    });

                    // allows the async attribute to be missing which is equivalent to FALSE
                    XAttribute async_attr = null;
                    if (null != (async_attr = callbackMethod.Attribute("async")))
                    {
                        bool async = false;
                        if (Boolean.TryParse(async_attr.Value, out async))
                        {
                            biz.Callbacks[key].IsASync = async;
                        }
                    }
                }

                var inputHelpers = item.Data.Descendants("bfw_input_helpers").Descendants("input");
                foreach (var inputHelper in inputHelpers)
                {
                    if (inputHelper.Attribute("name") != null)
                    {
                        var key = inputHelper.Attribute("name").Value;
                        var widgetInputHelper = new WidgetInputHelper();

                        widgetInputHelper.Name = inputHelper.Attribute("name").Value;

                        if (inputHelper.Attribute("selector") != null)
                        {
                            widgetInputHelper.Selector = inputHelper.Attribute("selector").Value;
                        }

                        if (inputHelper.Attribute("default") != null)
                        {
                            widgetInputHelper.DefaultValue = inputHelper.Attribute("default").Value;
                        }

                        if (inputHelper.Attribute("usedefault") != null)
                        {
                            var useDefaultValue = false;
                            if (Boolean.TryParse(inputHelper.Attribute("usedefault").Value, out useDefaultValue))
                            {
                                widgetInputHelper.UseDefaultValue = useDefaultValue;
                            }
                        }
                        biz.InputHelpers.Add(key, widgetInputHelper);
                    }
                }

                biz.Properties = ParseContentProperties(item.Data);
                biz.BHProperties = ParseBHContentProperties(item.Data);

                biz.InstructorConsoleSettings = new InstructorConsoleSettings();
                biz.InstructorConsoleSettings.
                    ParseSettings(item.Data.Element("instructor_console_settings") != null ?
                    XElement.Parse(item.Data.Element("instructor_console_settings").ToString()) :
                    null);
            }

            return biz;
        }

        public static Menu ToMenu(this AgxDC.Item item)
        {
            var biz = new Menu();

            if (null != item)
            {
                biz.Id = item.Id;
                biz.CourseID = item.ResourceEntityId;
                biz.ParentId = item.ParentId;
                biz.Type = (item.Type == Bfw.Agilix.Dlap.DlapItemType.Custom) ? item.CustomType : item.Type.ToString();
                biz.Sequence = item.Sequence;
                biz.Title = item.Title;

                var BfwType = item.Data.Descendants("bfw_type").FirstOrDefault();
                if (null != BfwType)
                {
                    biz.BfwType = BfwType.Value;
                }

                var tocId = item.Data.Descendants("bfw_tocid").FirstOrDefault();
                if (null != tocId)
                {
                    biz.BfwTocId = tocId.Value;
                }

                biz.Properties = ParseContentProperties(item.Data);
            }

            return biz;
        }

        public static MenuItem ToPageMenuItem(this AgxDC.Item item, string defaultparentid = null, string appendtosequence = null, int MenuSeq = -1)
        {
            var biz = new MenuItem();

            if (null != item)
            {
                biz.MenuSequence = MenuSeq;
                biz.Id = item.Id;
                biz.CourseID = item.ResourceEntityId;
                if (string.IsNullOrEmpty(defaultparentid))
                {
                    biz.ParentId = item.ParentId;
                }
                else
                {
                    biz.ParentId = defaultparentid;
                }
                biz.Type = (item.Type == Bfw.Agilix.Dlap.DlapItemType.Custom) ? item.CustomType : item.Type.ToString();
                biz.Sequence = item.Sequence;
                if (string.IsNullOrEmpty(appendtosequence))
                {
                    biz.FlatSequence = item.Sequence;
                }
                else
                {
                    biz.FlatSequence = string.Concat(appendtosequence, item.Sequence);
                }
                biz.Title = item.Title;

                var BfwType = item.Data.Descendants("bfw_type").FirstOrDefault();
                if (null != BfwType)
                {
                    biz.BfwType = BfwType.Value;
                }

                var Abbreviation = item.Data.Descendants("abbreviation").FirstOrDefault();
                if (null != Abbreviation)
                {
                    biz.Abbreviation = Abbreviation.Value;
                }


                biz.IsVisible = item.HiddenFromToc;


                var displayOnProductCourse = item.Data.Descendants("bfw_menu_productcourse_display").FirstOrDefault();
                if (null != displayOnProductCourse)
                {
                    biz.BfwDisplayOnProductCourse = bool.Parse(displayOnProductCourse.Value);
                }

                var selectedByDefault = item.Data.Descendants("bfw_menu_selected_by_default").FirstOrDefault();
                if (null != selectedByDefault)
                {
                    biz.SelectedByDefault = bool.Parse(selectedByDefault.Value);
                }

                var bfwCssClass = item.Data.Descendants("bfw_css_class").FirstOrDefault();
                if (null != bfwCssClass)
                {
                    biz.BfwCssClass = bfwCssClass.Value;
                }

                var createdBy = item.Data.Descendants("bfw_menu_created_by").FirstOrDefault();
                if (null != createdBy)
                {
                    biz.BfwMenuCreatedby = createdBy.Value;
                }

                var displaysflags = item.Data.Element("bfw_display_flags");

                List<XElement> displays = new List<XElement>();
                if (displaysflags != null)
                {
                    displays = displaysflags.Descendants("role").ToList();
                }

                var widgetDisplayOptions = new WidgetDisplayOptions();

                widgetDisplayOptions.DisplayOptions = (from display in displays
                                                       select (DisplayOption)Enum.Parse(typeof(DisplayOption), display.Value, true)).ToList();

                biz.WidgetDisplayOptions = widgetDisplayOptions;

                var CallbackMethods = item.Data.Descendants("bfw_menu_callbacks").Descendants("method");
                foreach (XElement callbackMethod in CallbackMethods)
                {
                    var callBack = new MenuItemCallback()
                    {
                        Name = (string)callbackMethod.Attribute("name") ?? "",
                        Controller = (string)callbackMethod.Attribute("controller") ?? "",
                        Target = (string)callbackMethod.Attribute("target") ?? "",
                        Action = (string)callbackMethod.Attribute("action") ?? "",
                        LinkType = (string)callbackMethod.Attribute("type") ?? "",
                        RouteName = (string)callbackMethod.Attribute("route") ?? "",
                        Url = (string)callbackMethod.Attribute("url") ?? "",
                        StudentOverride = (string)callbackMethod.Attribute("studentoverride") ?? "",
                        InstructorOverride = (string)callbackMethod.Attribute("instructoroverride") ?? ""

                    };

                    var subParams = callbackMethod.Elements("parameter");
                    callBack.Parameters = new Dictionary<string, PropertyValue>();
                    if (!subParams.IsNullOrEmpty())
                    {
                        foreach (var param in subParams)
                        {
                            try
                            {
                                var paramName = param.Attribute("name").Value;
                                var paramValue = param.Attribute("value").Value;
                                callBack.Parameters.Add(paramName, new PropertyValue { Value = paramValue });
                            }
                            catch { }
                        }
                    }

                    biz.Callbacks.Add(callBack.Name, callBack);
                }

                if (biz.Callbacks.IsNullOrEmpty())
                {
                    biz.IsDisabled = true;
                }

                biz.Properties = ParseContentProperties(item.Data);
            }

            return biz;
        }

        /// <summary>
        /// Pulls all information out of an agilix item and populates a ZONE item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static Zone ToZoneItem(this AgxDC.Item item, IPageActions pageActions)
        {
            var biz = new Zone();

            if (null != item)
            {
                biz.Id = item.Id;
                biz.CourseID = item.ResourceEntityId;
                biz.ParentId = item.ParentId;
                biz.Sequence = item.Sequence;
                biz.Title = item.Title;
                biz.IsSupportHide = item.Data.Element("bfw_support_hide") == null ? false : bool.Parse(item.Data.Element("bfw_support_hide").Value);
                var allowedWidgets = item.Data.Element("bfw_allowed_widgets");
                if (allowedWidgets != null)
                {
                    foreach (var widget in allowedWidgets.Elements())
                    {
                        biz.AllowedWidgets.Add(new AllowedWidget() { widgetName = widget.Attribute("displayName").Value, widgetType = widget.Attribute("type").Value });
                    }
                }
                var defaultPageId = item.Data.Element("bfw_default_page") == null
                                      ? null
                                      : item.Data.Element("bfw_default_page").Value;
                if (defaultPageId != null && pageActions != null)
                {
                    biz.DefaultPage = pageActions.LoadPageDefinition(defaultPageId);
                }
            }

            return biz;
        }

        /// <summary>
        /// Pulls all information out of an agilix item and populates a PageDefinition item.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static PageDefinition ToPageDefinition(this AgxDC.Item item)
        {
            var biz = new PageDefinition();

            if (null != item)
            {
                biz.Name = item.Id;
                biz.IsEditable = item.Data.Element("bfw_page_editable") == null ? false : bool.Parse(item.Data.Element("bfw_page_editable").Value);
                biz.CustomDivs = new List<string>();
                var divs = item.Data.Element("bfw_custom_divs");
                if (divs != null)
                {
                    foreach (var div in divs.Elements())
                    {
                        biz.CustomDivs.Add(div.Value);
                    }
                }
            }

            return biz;
        }

        /// <summary>
        /// Copies the default TOC parent ID from an agilix item to a content item.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="item">The item.</param>
        private static void ParseDefaultParent(ContentItem biz, AgxDC.Item item)
        {
            var defaultToc = item.Data.Descendants("bfw_toc_contents").FirstOrDefault();
            if (defaultToc != null)
            {
                biz.DefaultCategoryParentId = defaultToc.Attribute("parentid").Value;
                biz.DefaultCategorySequence = defaultToc.Attribute("sequence").Value;
            }
        }

        /// <summary>
        /// Parses any bfw_property elements from the item's data element.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IDictionary<string, PropertyValue> ParseContentProperties(XElement data)
        {
            return ParseContentProperties(data, "bfw_property");
        }

        /// <summary>
        /// Parses any bfw_property elements from the item's data element.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IDictionary<string, BHProperty> ParseBHContentProperties(XElement data)
        {
            return ParseBHProperties(data, "bfw_bhcomponent_property");
        }

        /// <summary>
        /// Parses any bfw_property elements from the item's data element.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IDictionary<string, BHProperty> ParseBHProperties(XElement data, string descendants)
        {
            var pdict = new Dictionary<string, BHProperty>();
            var properties = data.Descendants(descendants);

            if (!properties.IsNullOrEmpty())
            {
                var props = from prop in properties
                            group prop by prop.Attribute("name") into p
                            select new { Name = p.Key.Value, Elements = p };

                foreach (var prop in props)
                {
                    var seed = new List<object>();
                    PropertyType type = PropertyType.String;
                    var vals = prop.Elements.Map(v => ParsePropertyValue(v)).Reduce((v, vs) => { type = v.Type; vs.Add(v.Value); return vs; }, seed);

                    if (vals.Count() > 1)
                    {
                        pdict[prop.Name] = new BHProperty() { Type = type };
                    }
                    else
                    {
                        pdict[prop.Name] = new BHProperty() { Type = type, Values = vals };
                    }

                    var parameters = ParseParameters(data, "param", prop.Name);

                    pdict[prop.Name].Parameters = parameters.ToDictionary(x => x.Key, x => x.Value);
                }
            }
            return pdict;
        }

        private static IDictionary<string, BHParams> ParseParameters(XElement data, string descendants, string sParent)
        {
            var pdict = new Dictionary<string, BHParams>();
            var properties = data.Descendants(descendants);

            if (!properties.IsNullOrEmpty())
            {
                foreach (var param in properties)
                {
                    String name = param.Attribute("name").Value ?? "";
                    var singleParam = pdict.ContainsKey(name) ? pdict[name] : new BHParams();

                    XAttribute attribute = param.Attribute("alias");
                    if (null != attribute)
                        singleParam.Alias = attribute.Value;
                    attribute = param.Attribute("value");
                    if (null != attribute)
                        singleParam.Value = attribute.Value;
                    attribute = param.Attribute("id");
                    if (null != attribute)
                        singleParam.Id = attribute.Value;
                    attribute = param.Attribute("rel");
                    if (null != attribute)
                        singleParam.Rel = attribute.Value;

                    singleParam.Name = name;

                    pdict[name] = singleParam;
                }
            }

            return pdict;
        }

        /// <summary>
        /// Parses any bfw_property elements from the item's data element.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IDictionary<string, PropertyValue> ParseContentProperties(XElement data, string descendants)
        {
            var pdict = new Dictionary<string, PropertyValue>();

            var properties = data.Descendants(descendants);

            if (!properties.IsNullOrEmpty())
            {
                var props = from prop in properties
                            group prop by prop.Attribute("name") into p
                            select new { Name = p.Key.Value, Elements = p };

                foreach (var prop in props)
                {
                    var seed = new List<object>();
                    PropertyType type = PropertyType.String;
                    var vals = prop.Elements.Map(v => ParsePropertyValue(v)).Reduce((v, vs) => { type = v.Type; vs.Add(v.Value); return vs; }, seed);

                    if (vals.Count() > 1)
                    {
                        pdict[prop.Name] = new PropertyValue() { Type = type, Values = vals };
                    }
                    else
                    {
                        pdict[prop.Name] = new PropertyValue() { Type = type, Value = vals.FirstOrDefault() };
                    }

                    if (descendants == "param")
                    {
                        foreach (var attr in prop.Elements.Attributes())
                        {
                            vals.Add(attr.Value);
                            pdict[attr.Name.ToString()] = new PropertyValue() { Type = type, Values = vals };
                        }
                    }
                }
            }

            return pdict;
        }

        /// <summary>
        /// Parses any bfw_meta properties from the item's data element.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IDictionary<string, MetadataValue> ParseContentMetadata(XElement data)
        {
            var meta = new Dictionary<string, MetadataValue>();

            var properties = data.Descendants("bfw_metadata");

            if (!properties.IsNullOrEmpty())
            {
                var props = from prop in properties
                            group prop by prop.Attribute("name") into p
                            select new { Name = p.Key.Value, Elements = p };

                foreach (var prop in props)
                {
                    var seed = new List<object>();
                    PropertyType type = PropertyType.String;
                    var vals = prop.Elements.Map(v => ParsePropertyValue(v)).Reduce((v, vs) => { type = v.Type; vs.Add(v.Value); return vs; }, seed);

                    if (vals.Count() > 1)
                    {
                        meta[prop.Name] = new MetadataValue() { Type = type, Values = vals };
                    }
                    else
                    {
                        meta[prop.Name] = new MetadataValue() { Type = type, Value = vals.FirstOrDefault() };
                    }
                }
            }

            return meta;
        }

        /// <summary>
        /// Parses all meta-* elements in the item
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>

        private static IDictionary<string, string> ParseContentFacetMetadata(XElement data)
        {

            var meta = new Dictionary<string, string>();
            var properties = data.Elements().Where(e => e.Name.LocalName.StartsWith("meta-"));

            if (properties.Any())
            {
                properties.ToList().ForEach(p =>
                {
                    if (meta.ContainsKey(p.Name.LocalName))
                    {
                        meta[p.Name.LocalName] = p.Value;
                    }
                    else
                    {
                        if (p.HasElements == true)
                        {

                            var childrenMetaValues = ParseContentFacetMetadata(p);

                            var childKey = childrenMetaValues.FirstOrDefault().Key;
                            var childValue = childrenMetaValues.FirstOrDefault().Value;
                            if (meta.ContainsKey(childKey))
                            {
                                meta[childKey] = childValue;
                            }
                            else
                            {
                                meta.Add(childKey, childValue);
                            }

                        }
                        meta.Add(p.Name.LocalName, p.Value);
                    }

                });
            }

            return meta;
        }
        /// <summary>
        /// Parses out a property value from an XML property element.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static PropertyValue ParsePropertyValue(XElement prop)
        {
            var type = prop.Attribute("type") ?? new XAttribute("type", "string");
            PropertyValue val = null;

            if (null != type)
            {
                switch (type.Value.ToLowerInvariant())
                {
                    case "boolean":
                        bool b = false;
                        bool.TryParse(prop.Value, out b);
                        val = new PropertyValue() { Type = PropertyType.Boolean, Value = b };
                        break;

                    case "integer":
                        int i = 0;
                        int.TryParse(prop.Value, out i);
                        val = new PropertyValue() { Type = PropertyType.Integer, Value = i };
                        break;

                    case "float":
                        double d = 0.0;
                        double.TryParse(prop.Value, out d);
                        val = new PropertyValue() { Type = PropertyType.Float, Value = d };
                        break;

                    case "string":
                        val = new PropertyValue() { Type = PropertyType.String, Value = prop.Value };
                        break;

                    case "datetime":
                        DateTime dt = DateTime.MinValue;
                        DateTime.TryParse(prop.Value, out dt);
                        val = new PropertyValue() { Type = PropertyType.DateTime, Value = dt };
                        break;

                    case "xml":
                        XElement elm = null;
                        bool isHasError = false;
                        try
                        {
                            elm = XElement.Parse(prop.Value);
                        }
                        catch
                        {
                            //swallow!
                            isHasError = true;
                        }

                        if (isHasError)
                        {
                            try
                            {
                                elm = XElement.Parse(prop.FirstNode.ToString());
                            }
                            catch
                            {
                                //swallow!
                                isHasError = true;
                            }
                        }
                        val = new PropertyValue() { Type = PropertyType.Xml, Value = elm };
                        break;

                    case "html":
                        string h = HttpUtility.HtmlDecode(prop.Value);
                        val = new PropertyValue() { Type = PropertyType.Html, Value = h };
                        break;

                    default:
                        val = val = new PropertyValue() { Type = PropertyType.String, Value = prop.Value };
                        break;
                }
            }

            return val;
        }

        /// <summary>
        /// Converts a string value to a PropertyType enum value.
        /// </summary>
        /// <param name="type">The string type value.</param>
        /// <returns></returns>
        public static PropertyType PropertyTypeFromString(string type)
        {
            var ptype = PropertyType.String;

            switch (type.ToLowerInvariant())
            {
                case "boolean":
                    ptype = PropertyType.Boolean;
                    break;

                case "integer":
                    ptype = PropertyType.Integer;
                    break;

                case "float":
                    ptype = PropertyType.Float;
                    break;

                case "string":
                    ptype = PropertyType.String;
                    break;

                case "datetime":
                    ptype = PropertyType.DateTime;
                    break;

                case "xml":
                    ptype = PropertyType.Xml;
                    break;

                case "html":
                    ptype = PropertyType.Html;
                    break;

                default:
                    ptype = PropertyType.String;
                    break;
            }

            return ptype;
        }

        /// <summary>
        /// Parses relevant data from the item to get assessment settings.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static AssessmentSettings ToAssessmentSettings(this AgxDC.Item item)
        {
            XElement datum;
            var biz = new AssessmentSettings()
            {
                QuizType = "",
                AttemptLimit = 0,
                DefaultScore = 0,
                ExamFlags = ExamFlags.None,
                TimeLimit = 0,
                DueDate = DateTime.MinValue,
                GradeRule = GradeRule.Last,
                LearningCurveTargetScore = "",
                AutoCalibrateDifficulty = false,
                AutoTargetScore = true

            };

            datum = item.Data.Element("attemptlimit");
            if (null != datum)
            {
                biz.AttemptLimit = Int32.Parse(datum.Value);
            }

            datum = item.Data.Element("defaultscore");
            if (null != datum)
            {
                biz.DefaultScore = Int32.Parse(datum.Value);
            }

            datum = item.Data.Element("examflags");
            if (null != datum)
            {
                biz.ExamFlags = (ExamFlags)Int32.Parse(datum.Value);
            }

            biz.AttemptLimit = item.AttemptLimit;
            biz.SubmissionGradeAction = (SubmissionGradeAction)item.SubmissionGradeAction;
            biz.QuestionDelivery = (QuestionDelivery)item.QuestionsPerPage;
            biz.AllowSaveAndContinue = item.AllowSaveAndContinue;
            biz.AutoSubmitAssessments = item.AutoSubmitAssessments;
            biz.RandomizeQuestionOrder = item.ShuffleQuestions;
            biz.RandomizeAnswerOrder = item.ShuffleAnswers;
            biz.AllowViewHints = item.AllowViewHints;
            biz.PercentSubstractHint = item.PercentSubstractHint;

            biz.ShowScoreAfter = (ReviewSetting)item.ShowScoreAfter;
            biz.ShowQuestionsAnswers = (ReviewSetting)item.ShowQuestionsAnswers;
            biz.ShowRightWrong = (ReviewSetting)item.ShowRightWrong;
            biz.ShowAnswers = (ReviewSetting)item.ShowAnswers;
            biz.ShowFeedbackAndRemarks = (ReviewSetting)item.ShowFeedbackAndRemarks;
            biz.ShowSolutions = (ReviewSetting)item.ShowSolutions;

            biz.StudentsCanEmailInstructors = item.StudentsCanEmailInstructors;

            datum = item.Data.Element("type");
            if (null != datum)
            {
                biz.QuizType = datum.Value;
            }

            datum = item.Data.Element("duedate");
            if (null != datum)
            {
                var dElm = item.Data.Element("duedate");
                if (dElm != null)
                {
                    DateTime dt;
                    DateTime.TryParse(dElm.Value, out dt);

                    biz.DueDate = dt;
                }
            }

            datum = item.Data.Element("timelimit");
            if (null != datum)
            {
                biz.TimeLimit = Int32.Parse(datum.Value);
            }

            datum = item.Data.Element("questionsperpage");
            if (null != datum)
            {
                biz.QuestionsPerPage = Int32.Parse(datum.Value);
            }


            var targetScore = item.Data.XPathSelectElement("//bfw_learning_curve/targetscore");
            if (targetScore != null)
            {
                biz.LearningCurveTargetScore = targetScore.Value;
            }

            var autoTargetScore = item.Data.XPathSelectElement("//bfw_learning_curve/autotargetscore");
            if (autoTargetScore != null)
            {
                biz.AutoTargetScore = Convert.ToBoolean(autoTargetScore.Value);
            }

            var autocalibrate = item.Data.XPathSelectElement("//bfw_learning_curve/autocalibrate");
            if (autocalibrate != null)
            {
                var tempValue = false;
                bool.TryParse(autocalibrate.Value, out tempValue);
                biz.AutoCalibrateDifficulty = tempValue;
            }


            //var scoreattempt = (item.Data.Element("submissiongradeaction"));
            //if (scoreattempt != null)
            //{
            //    biz.SubmissionGradeAction= (SubmissionGradeAction)(int)scoreattempt;
            //}
            //else
            //{
            //    biz.SubmissionGradeAction= (SubmissionGradeAction)item.ScoredAttempt;
            //}

            var graderule = (item.Data.Element("graderule"));
            if (graderule != null)
            {
                biz.GradeRule = (GradeRule)(int)graderule;
            }

            //biz.ShowScoreAfter = item.ShowScoreAfter;

            return biz;
        }

        /// <summary>
        /// Parses relevant data from the item to get homework groups.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        private static List<HomeworkGroupInfo> GetHWGroupInfo(AgxDC.Item item)
        {
            var groupinfo = new List<HomeworkGroupInfo>()
            {
            };
            var hwgroupinfo = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"hw_group_info\"]");

            if (null != hwgroupinfo)
            {
                foreach (XElement group in hwgroupinfo.Elements("group"))
                {
                    HomeworkGroupInfo hwGroup = new HomeworkGroupInfo();
                    hwGroup.Name = group.Attribute("name").Value;

                    foreach (XElement property in group.Elements("property"))
                    {
                        if (property.Attribute("name").Value == "scrambled")
                        {
                            hwGroup.Scrambled = property.Attribute("value").Value;
                        }
                        if (property.Attribute("name").Value == "hints")
                        {
                            hwGroup.Hints = property.Attribute("value").Value;
                        }
                    }
                    groupinfo.Add(hwGroup);
                }
            }
            return groupinfo;
        }

        /// <summary>
        /// Parses relevant data from the item to get assessment groups.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static List<AssessmentGroup> ToAssessmentGroups(this AgxDC.Item item)
        {
            var biz = new List<AssessmentGroup>()
            {
            };

            var homeworkgroups = item.Data.Element("homeworkgroups");

            if (null != homeworkgroups)
            {
                var hwgroupinfo = GetHWGroupInfo(item);
                var properties = ParseContentProperties(item.Data);
                foreach (XElement group in homeworkgroups.Elements("group"))
                {
                    AssessmentGroup assessmentGroup = new AssessmentGroup();
                    if (null != group.Attribute("name"))
                    {
                        assessmentGroup.Name = group.Attribute("name").Value;
                    }

                    if (null != group.Attribute("attemptlimit"))
                    {
                        assessmentGroup.Attempts = group.Attribute("attemptlimit").Value;
                    }
                    if (null != group.Attribute("timelimit"))
                    {
                        assessmentGroup.TimeLimit = group.Attribute("timelimit").Value;
                    }
                    if (null != group.Attribute("submissiongradeaction"))
                    {
                        assessmentGroup.SubmissionGradeAction = (SubmissionGradeAction)Int32.Parse(group.Attribute("submissiongradeaction").Value);
                    }
                    if (null != group.Attribute("flags"))
                    {
                        assessmentGroup.Review = (HomeworkGroupFlags)Int32.Parse(group.Attribute("flags").Value);
                    }
                    foreach (var grp in hwgroupinfo)
                    {
                        if (grp.Name == assessmentGroup.Name)
                        {
                            assessmentGroup.Scrambled = grp.Scrambled;
                            assessmentGroup.Hints = grp.Hints;
                        }
                    }

                    if (null != group.Element("examreviewrules"))
                    {
                        ReviewSettings reviewSettings = new ReviewSettings();
                        reviewSettings.ParseFrom(group.Element("examreviewrules"));
                        assessmentGroup.ReviewSettings = reviewSettings;
                    }
                    else
                    {
                        assessmentGroup.ReviewSettings = new ReviewSettings();
                    }

                    biz.Add(assessmentGroup);
                }
            }

            return biz;
        }

        /// <summary>
        /// Parses any relevant data from the item to get the settings for an assignment.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static AssignmentSettings ToAssignmentSettings(this AgxDC.Item item)
        {
            var biz = new AssignmentSettings()
            {
                DueDate = item.DueDate,
                GradeReleaseDate = item.GradeReleaseDate,
                meta_bfw_Assigned = item.meta_bfw_Assigned,
                Category = "",
                Points = 0,
                IsAssignable = item.IsGradable,
                IsGradeable = item.IsGradable,
                DropBoxType = DropBoxType.None,
                Rubric = item.HasRubric ? item.Rubric : "",
                CategorySequence = item.CategorySequence == null ? string.Empty : item.CategorySequence
            };

            var dateElement = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_startdate\"]");
            if (dateElement != null)
            {
                DateTime startDate = DateTime.MinValue;
                DateTime.TryParse(dateElement.Value, out startDate);
                biz.StartDate = startDate;
            }

            var sendReminder = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_SendReminder\"]");
            if (sendReminder != null)
            {
                biz.IsSendReminder = sendReminder.Value.Equals("true");
            }

            var highlightLateSubmission = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_highlightlatesubmission\"]");
            if (highlightLateSubmission != null)
            {
                biz.IsHighlightLateSubmission = highlightLateSubmission.Value.Equals("true");
            }

            var allowLateSubmissionGrace = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_allowlatesubmissiongrace\"]");
            if (allowLateSubmissionGrace != null)
            {
                biz.IsAllowLateGracePeriod = allowLateSubmissionGrace.Value.Equals("true");
            }

            var allowExtraCredit = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_allow_extracredit\"]");
            if (allowExtraCredit != null)
            {
                biz.IsAllowExtraCredit = allowExtraCredit.Value.Equals("true");
            }

            var lateSubmissionGraceDuration = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_latesubmissiongraceduration\"]");
            if (lateSubmissionGraceDuration != null)
            {
                long graceDuration = 0;
                long.TryParse(lateSubmissionGraceDuration.Value, out graceDuration);
                biz.LateGraceDuration = graceDuration;
            }

            var lateSubmissionGraceDurationType = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_latesubmissiongracedurationtype\"]");
            if (lateSubmissionGraceDurationType != null)
            {
                biz.LateGraceDurationType = lateSubmissionGraceDurationType.Value;
            }

            var includeGbbScoreTrigger = item.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_IncludeGbbScoreTrigger\"]");
            if (includeGbbScoreTrigger != null)
            {
                int iIncludeGbbScoreTrigger = 0;
                int.TryParse(includeGbbScoreTrigger.Value, out iIncludeGbbScoreTrigger);
                biz.IncludeGbbScoreTrigger = iIncludeGbbScoreTrigger;
            }

            var category = item.Data.Element("category");

            biz.Points = item.MaxPoints;

            if (item.MaxPoints <= 0.0)
            {
                var weight = item.Data.Element("weight");
                if (null != weight)
                {
                    double p = 0.0;
                    double.TryParse(weight.Value, out p);
                    //not sure what is the purpose
                    //biz.Points = p;
                }
            }

            if (item.DropBox)
            {
                biz.DropBoxType = (DropBoxType)Enum.Parse(typeof(DropBoxType), item.DropBoxType.ToString());
            }

            if (null != category)
            {
                var c = 0;
                int.TryParse(category.Value, out c);
                biz.Category = c.ToString();
            }

            var completionTrigger = item.Data.Element("completiontrigger");
            if (null != completionTrigger)
            {
                int completionTriggerInt = 1;
                int.TryParse(completionTrigger.Value, out completionTriggerInt);
                biz.CompletionTrigger = (CompletionTrigger)completionTriggerInt;
            }
            else
            {
                biz.CompletionTrigger = CompletionTrigger.Submission;
            }

            biz.TimeToComplete = item.TimeToComplete;
            biz.PassingScore = item.PassingScore;
            biz.AllowLateSubmission = item.IsAllowLateSubmission;
            
            bool isMarkAsCompleteChecked;
            bool.TryParse(
                item.Data.Element("IsMarkAsCompleteChecked") != null
                    ? item.Data.Element("IsMarkAsCompleteChecked").Value
                    : "false", out isMarkAsCompleteChecked);
            biz.IsMarkAsCompleteChecked = isMarkAsCompleteChecked;

            var submissionGradeAction = (item.Data.Element("submissiongradeaction"));
            if (submissionGradeAction != null)
            {
                biz.SubmissionGradeAction = (SubmissionGradeAction)(int)submissionGradeAction;
            }
            else
            {
                biz.SubmissionGradeAction = (SubmissionGradeAction)item.SubmissionGradeAction;
            }

            var graderule = (item.Data.Element("graderule"));
            if (graderule != null)
            {
                biz.GradeRule = (GradeRule)(int)graderule;
            }
            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to question choice.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static QuestionChoice ToQuestionChoice(this AgxDC.QuestionChoice agx)
        {
            return new QuestionChoice() { Id = agx.Id, Text = agx.Text, Feedback = agx.Feedback, Answer = agx.Answer };
        }

        /// <summary>
        /// Convert a teacher response to an agilix object.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AgxDC.TeacherResponse ToTeacherResponse(this TeacherResponse biz)
        {
            if (biz == null) return new AgxDC.TeacherResponse();
            var item = new AgxDC.TeacherResponse
            {
                TeacherResponseType =
                    (AgxDC.TeacherResponseType)biz.TeacherResponseType,
                Mask = (AgxDC.GradeStatus)biz.Mask,
                Status = (AgxDC.GradeStatus)biz.Status,
                PointsPossible = biz.PointsPossible,
                PointsComputed = biz.PointsComputed,
                PointsAssigned = biz.PointsAssigned,
                ForeignId = biz.ForeignId,
                ScoredVersion = biz.ScoredVersion,
                StudentEnrollmentId = biz.StudentEnrollmentId,
                Responses =
                    !biz.Responses.IsNullOrEmpty()
                        ? biz.Responses.Map(r => r.ToTeacherResponse()).ToList()
                        : new List<Agilix.DataContracts.TeacherResponse>()
            };
            return item;
        }

        /// <summary>
        /// Convert from an agilix object to teacher response.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static TeacherResponse ToTeacherResponse(this AgxDC.TeacherResponse item)
        {
            if (item == null) return new TeacherResponse();
            var biz = new TeacherResponse
            {
                TeacherResponseType = (TeacherResponseType)item.TeacherResponseType,
                Mask = (GradeStatus)item.Mask,
                Status = (GradeStatus)item.Status,
                PointsPossible = item.PointsPossible,
                PointsComputed = item.PointsComputed,
                PointsAssigned = item.PointsAssigned,
                ForeignId = item.ForeignId,
                ScoredVersion = item.ScoredVersion,
                StudentEnrollmentId = item.StudentEnrollmentId,
                TeacherComment = item.TeacherComment,
                Responses =
                    !item.Responses.IsNullOrEmpty()
                        ? item.Responses.Map(r => r.ToTeacherResponse()).ToList()
                        : new List<TeacherResponse>()
            };

            if (item.TeacherAttachments != null)
            {
                biz.TeacherAttachments = new List<Bfw.PX.Biz.DataContracts.Attachment>();
                foreach (Attachment attachement in item.TeacherAttachments)
                {
                    Bfw.PX.Biz.DataContracts.Attachment bizAttachment = new Bfw.PX.Biz.DataContracts.Attachment();
                    bizAttachment.Name = attachement.Name;
                    bizAttachment.Href = attachement.Href;
                    biz.TeacherAttachments.Add(bizAttachment);
                }
            }
            if (item.ResourceStream != null)
            {
                biz.ResourceStream = new MemoryStream();
                item.ResourceStream.Seek(0, SeekOrigin.Begin);
                item.ResourceStream.CopyTo(biz.ResourceStream);
                biz.ResourceStream.Seek(0, SeekOrigin.Begin);
            }


            return biz;
        }

        /// <summary>
        /// Gets the inner text value of an XML node attribute.
        /// </summary>
        /// <param name="node">The XML node.</param>
        /// <param name="attribute">The XML attribute.</param>
        /// <returns></returns>
        public static string GetAttributeAsString(this XmlNode node, string attribute)
        {
            return GetAttributeAsString(node, attribute, "");
        }

        /// <summary>
        /// Gets the inner text value of an XML node attribute.
        /// </summary>
        /// <param name="node">The XML node.</param>
        /// <param name="attribute">The XML attribute.</param>
        /// <returns></returns>
        public static bool GetAttributeAsBoolean(this XmlNode node, string attribute)
        {
            var strValue = GetAttributeAsString(node, attribute, "");

            bool results = false;
            bool.TryParse(strValue, out results);
            return results;
        }

        /// <summary>
        /// Gets the inner text value of an XML node attribute.
        /// </summary>
        /// <param name="node">The XML node.</param>
        /// <param name="attribute">The XML attribute.</param>
        /// <param name="defaultValue">Default value to return if null.</param>
        /// <returns></returns>
        public static string GetAttributeAsString(this XmlNode node, string attribute, string defaultValue)
        {
            if (node != null && node.Attributes[attribute] != null)
                return node.Attributes[attribute].Value.Trim();

            return defaultValue;
        }

        /// <summary>
        /// Get the value of a descendant
        /// </summary>
        /// <param name="data"></param>
        /// <param name="descendant"></param>
        /// <returns></returns>
        public static string GetDescendantAsString(this XElement data, string descendant)
        {
            string returnValue = "";
            if (!data.Descendants(descendant).IsNullOrEmpty())
            {
                var elm = data.Descendants(descendant).FirstOrDefault();
                if (elm != null)
                {
                    returnValue = elm.Value;
                    returnValue = System.Web.HttpUtility.HtmlDecode(returnValue.Trim());
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Converts a note object to a resource.
        /// </summary>
        /// <param name="biz">The note to convert.</param>
        /// <param name="urlBase">The resource path.</param>
        /// <returns></returns>
        public static Resource ToNote(this Note biz, string urlBase)
        {
            if (biz == null)
                return new XmlResource();

            var note = new XmlResource
                        {
                            Status = ResourceStatus.Normal,
                            EntityId = biz.EntityId,
                            CreationDate = biz.Created,
                            Body = NoteToXml(biz),
                            Title = biz.Title,
                            ModifiedDate = biz.Modified,
                            Url = string.Format(urlBase, biz.Id)
                        };
            return note;

        }

        /// <summary>
        /// Converts a note object to XML.
        /// </summary>
        /// <param name="biz">The note object.</param>
        /// <returns></returns>
        private static string NoteToXml(Note biz)
        {
            var xml = string.Empty;

            var doc = new XDocument();
            var root = new XElement("Note");
            root.Add(new XAttribute("Id", biz.Id));
            root.Add(new XAttribute("Sequence", biz.Sequence));
            root.Add(new XAttribute("Text", biz.Text));
            root.Add(new XAttribute("Title", biz.Title));
            root.Add(new XAttribute("ModifiedDate", biz.Modified));

            doc.Add(root);

            return doc.ToString();
        }

        /// <summary>
        /// Converts a resource to a note object.
        /// </summary>
        /// <param name="biz">The resource to convert.</param>
        /// <returns></returns>
        public static Note ToNote(this Resource biz)
        {
            if (biz == null)
                return new Note();

            var note = XmlToNote(biz);

            return note;

        }

        /// <summary>
        /// Converts a resource to a note object.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        private static Note XmlToNote(Resource resource)
        {
            var dest = resource.GetStream();
            var sr = new System.IO.StreamReader(dest);

            string xmlDoc = sr.ReadToEnd();
            dest.Flush();
            dest.Seek(0, SeekOrigin.Begin);
            if (xmlDoc.Contains("&nbsp;"))
                xmlDoc = xmlDoc.Replace("&nbsp;", " ");

            XDocument xDoc = XDocument.Parse(xmlDoc);

            XElement xElement = xDoc.XPathSelectElement("//Note");
            Note note = new Note()
                        {
                            Id = xElement.Attribute("Id").Value,
                            Sequence = xElement.Attribute("Sequence").Value,
                            Text = xElement.Attribute("Text").Value,
                            Title = xElement.Attribute("Title").Value,
                            Modified = resource.ModifiedDate,
                            EntityId = resource.EntityId
                        };

            return note;
        }

        /// <summary>
        /// Convert from an agilix object to task.
        /// </summary>
        /// <param name="agx">The agilix item.</param>
        /// <returns></returns>
        public static Task ToTask(this AgxDC.Task agx)
        {
            Task biz = null;

            if (null != agx)
            {
                biz = new Task();
                biz.TaskId = agx.TaskId;
                biz.Command = agx.Command;
                biz.CreationDate = agx.CreationDate;
                biz.CurrentItem = agx.CurrentItem;
                biz.Error = agx.Error;
                biz.Finished = agx.Finished;
                biz.LastRunEndDate = agx.LastRunEndDate;
                biz.LastRunStartDate = agx.LastRunStartDate;
                biz.LastRunEndDate = agx.LastRunEndDate;
                biz.PeriodMinutes = agx.PeriodMinutes;
                biz.PortionComplete = agx.PortionComplete;
                biz.StartDate = agx.StartDate;
                biz.Success = agx.Success;
            }

            return biz;
        }

        /// <summary>
        /// Converts a ShareNoteResult object to NoteSettingEntity object.
        /// </summary>
        /// <param name="biz">The ShareNoteResult object.</param>
        /// <returns></returns>
        public static Notes.NoteSettingEntity ToShareNote(this ShareNoteResult biz)
        {
            var dest = new Notes.NoteSettingEntity
            {
                StudentId = biz.StudentId,
                SharedStudentId = biz.SharedStudentId,
                CourseId = biz.CourseId,
                FirstNameSharer = biz.FirstNameSharer,
                LastNameSharer = biz.LastNameSharer,
                FirstNameSharee = biz.FirstNameSharee,
                LastNameSharee = biz.LastNameSharee,
            };
            return dest;
        }

        /// <summary>
        /// Convert from an agilix object to grade book weight.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static GradeBookWeights ToGradeBookWeight(this AgxDC.GradeBookWeights item)
        {
            if (item == null) return new GradeBookWeights();
            var biz = new GradeBookWeights
            {
                CategoryWeightTotal = item.CategoryWeightTotal,
                Total = item.Total,
                TotalWithExtraCredit = item.TotalWithExtraCredit,
                WeightedCategories = item.WeightedCategories,

                GradeWeightCategories =
                    !item.GradeWeightCategories.IsNullOrEmpty()
                        ? item.GradeWeightCategories.Map(r => r.ToGradeBookWeightCategory()).ToList()
                        : new List<GradeBookWeightCategory>()
            };
            return biz;
        }

        /// <summary>
        /// Convert from an agilix object to grade book weight category.
        /// </summary>
        /// <param name="item">The agilix item.</param>
        /// <returns></returns>
        public static GradeBookWeightCategory ToGradeBookWeightCategory(this AgxDC.GradeBookWeightCategory item)
        {
            if (item == null) return new GradeBookWeightCategory();
            var biz = new GradeBookWeightCategory
            {
                Id = item.Id,
                Percent = item.Percent,
                ItemWeightTotal = item.ItemWeightTotal,
                PercentWithExtraCredit = item.PercentWithExtraCredit,
                Text = item.Text,
                Weight = item.Weight,
                DropLowest = item.DropLowest,
                Sequence = item.Sequence,
                Items = item.Items.Map(o => o.ToContentItem()).ToList()
            };
            return biz;
        }

        /// <summary>
        /// Converts a business review settings object into an XElement
        /// required by the Agilix layer
        /// </summary>
        /// <param name="settings">The review settings</param>
        /// <returns></returns>
        public static XElement ToAgilixItem(this ReviewSettings settings)
        {
            XElement reviewSettings = new XElement("examreviewrules");
            XElement newSetting; // Used as a temporary variable

            newSetting = new XElement("rule");
            newSetting.SetAttributeValue("setting", "Question");
            newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowQuestionsAnswers));
            reviewSettings.Add(newSetting);

            newSetting = new XElement("rule");
            newSetting.SetAttributeValue("setting", "Answer");
            newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowQuestionsAnswers));
            reviewSettings.Add(newSetting);

            newSetting = new XElement("rule");
            newSetting.SetAttributeValue("setting", "Possible");
            newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowScoreAfter));
            reviewSettings.Add(newSetting);

            var showCorrectQuestion = reviewSettings.Elements().SingleOrDefault(x => x.Attribute("setting").ToString() == "CorrectQuestion");
            if (null != showCorrectQuestion)
            {
                showCorrectQuestion.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowRightWrong));
            }
            else
            {
                newSetting = new XElement("rule");
                newSetting.SetAttributeValue("setting", "CorrectQuestion");
                newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowRightWrong));
                reviewSettings.Add(newSetting);
            }

            var showAnswers = reviewSettings.Elements().SingleOrDefault(x => x.Attribute("setting").ToString() == "CorrectChoice");
            if (null != showAnswers)
            {
                showAnswers.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowAnswers));
            }
            else
            {
                newSetting = new XElement("rule");
                newSetting.SetAttributeValue("setting", "CorrectChoice");
                newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowAnswers));
                reviewSettings.Add(newSetting);
            }

            var showFeedback = reviewSettings.Elements().SingleOrDefault(x => x.Attribute("setting").ToString() == "Feedback");
            if (null != showFeedback)
            {
                showFeedback.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowFeedbackAndRemarks));
            }
            else
            {
                newSetting = new XElement("rule");
                newSetting.SetAttributeValue("setting", "Feedback");
                newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowFeedbackAndRemarks));
                reviewSettings.Add(newSetting);
            }

            var showSolutions = reviewSettings.Elements().SingleOrDefault(x => x.Attribute("setting").ToString() == "Feedback-GROUP");
            if (null != showSolutions)
            {
                showSolutions.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowSolutions));
            }
            else
            {
                newSetting = new XElement("rule");
                newSetting.SetAttributeValue("setting", "Feedback-GROUP");
                newSetting.SetAttributeValue("condition", AgxDC.Item.ReviewSettingToDlap((AgxDC.ReviewSetting)settings.ShowSolutions));
                reviewSettings.Add(newSetting);
            }

            return reviewSettings;
        }

        /// <summary>
        /// Converts review settings to business object
        /// </summary>
        /// <param name="settings">The review settings</param>
        /// <returns></returns>
        public static void ParseFrom(this ReviewSettings bizSettings, XElement reviewSettings)
        {
            // Review Settings Section
            if (null != reviewSettings && reviewSettings.HasElements)
            {
                var showQuestionAnswersPoints =
                    from setting in reviewSettings.Elements()
                    where setting.Attribute("setting").Value == "Question" ||
                          setting.Attribute("setting").Value == "Answer" ||
                          setting.Attribute("setting").Value == "Possible"
                    select setting;

                XElement firstNode = showQuestionAnswersPoints.FirstOrDefault();
                string firstCondition = firstNode.Attribute("condition").Value;

                if (null != firstNode)
                {
                    foreach (var item in showQuestionAnswersPoints)
                    {
                        if (item.Attribute("condition").Value != firstCondition)
                        {
                            bizSettings.ShowQuestionsAnswers = ReviewSetting.Each;
                            break;
                        }
                        else
                        {
                            bizSettings.ShowQuestionsAnswers = (ReviewSetting)AgxDC.Item.DlapToReviewSetting(firstCondition);
                        }
                    }
                }
                else
                {
                    bizSettings.ShowQuestionsAnswers = ReviewSetting.Each;
                }

                var showRightWrong = reviewSettings.Elements().FirstOrDefault(x => x.Attribute("setting").Value == "CorrectQuestion");
                bizSettings.ShowRightWrong = (null != showRightWrong) ? (ReviewSetting)AgxDC.Item.DlapToReviewSetting(showRightWrong.Attribute("condition").Value) : ReviewSetting.Each;

                var showAnswers = reviewSettings.Elements().FirstOrDefault(x => x.Attribute("setting").Value == "CorrectChoice");
                bizSettings.ShowAnswers = (null != showAnswers) ? (ReviewSetting)AgxDC.Item.DlapToReviewSetting(showAnswers.Attribute("condition").Value) : ReviewSetting.Each;

                var showFeedback = reviewSettings.Elements().FirstOrDefault(x => x.Attribute("setting").Value == "Feedback");
                bizSettings.ShowFeedbackAndRemarks = (null != showFeedback) ? (ReviewSetting)AgxDC.Item.DlapToReviewSetting(showFeedback.Attribute("condition").Value) : ReviewSetting.Each;

                var showSolutions = reviewSettings.Elements().FirstOrDefault(x => x.Attribute("setting").Value == "Feedback-GROUP");
                bizSettings.ShowSolutions = (null != showSolutions) ? (ReviewSetting)AgxDC.Item.DlapToReviewSetting(showSolutions.Attribute("condition").Value) : ReviewSetting.Each;
            }
        }

        /// <summary>
        /// Converts the Xelement to CourseAcademicTerm Object
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CourseAcademicTerm ToCourseAcademicTerm(this XElement element)
        {
            return new CourseAcademicTerm
            {
                Id = element.Attribute("id").Value,
                Name = element.Value,
                StartDate = DateTime.Parse(element.Attribute("start").Value),
                EndDate = DateTime.Parse(element.Attribute("end").Value)
            };
        }
        /// <summary>
        /// Returns the a list of Container
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Container> GetContainers(List<Bfw.Agilix.DataContracts.Container> containers)
        {
            List<Container> containerList = new List<Container>();
            foreach (var container in containers)
            {
                containerList.Add(new Container(container.Toc, container.Value, container.DlapType));
            }
            return containerList;
        }

        /// <summary>
        /// Returns the a list of Container
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Bfw.Agilix.DataContracts.Container> GetContainers(List<Container> containers)
        {
            List<Bfw.Agilix.DataContracts.Container> containerList = new List<Bfw.Agilix.DataContracts.Container>();
            foreach (var container in containers)
            {
                containerList.Add(new Bfw.Agilix.DataContracts.Container(container.Toc, container.Value, container.DlapType));
            }
            return containerList;
        }

        /// <summary>
        /// Returns the DateTime adjusted for server time zone
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DateTime GetCourseDateTime(this DateTime dt, IBusinessContext context)
        {
            if (context != null)
            {
                try
                {
                    return context.Course.UtcRelativeAdjust(dt);
                }
                catch (Exception)
                {

                    return dt;
                }

            }
            return dt;
        }

        /// <summary>
        /// If passing score is not express in interval [0,1], then divide it by 100.
        /// </summary>
        /// <param name="passingScore"></param>
        /// <returns></returns>
        private static double AdjustPassingScore(double passingScore)
        {
            return passingScore > 1.0 ? (passingScore / 100.0) : passingScore;
        }

        public static GradeList ToGradeList(this AgxDC.GradeList gradelist)
        {
            GradeList bizGradeList = new GradeList();
            bizGradeList.Status = gradelist.Status;
            bizGradeList.Responseversion = gradelist.Responseversion;
            bizGradeList.Seconds = gradelist.Seconds;
            bizGradeList.SubmittedDate = gradelist.SubmittedDate;
            bizGradeList.Submittedversion = gradelist.Submittedversion;

            return bizGradeList;
        }

        public static ItemLink ToItemLink(this AgxDC.ItemLink itemLink)
        {
            ItemLink item = new ItemLink();
            item.EntityId = itemLink.EntityId;
            item.Id = itemLink.Id;
            return item;
        }
    }
}