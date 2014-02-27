using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ContentItemMapper
    {
        internal static void ToBaseContentItem(this ContentItem model, Biz.DataContracts.ContentItem biz,
            Biz.ServiceContracts.IContentActions content)
        {
            ToBaseContentItem(model, biz);
        }
        /// <summary>
        /// Convert to a Base Content Item from a Models Content Item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        internal static void ToBaseContentItem(this ContentItem model, Biz.DataContracts.ContentItem biz)
        {
            if (string.IsNullOrWhiteSpace(model.Id)) model.Id = biz.Id;
            if (string.IsNullOrWhiteSpace(model.ActualEntityid)) model.ActualEntityid = biz.ActualEntityid;
            if (string.IsNullOrWhiteSpace(model.Title)) model.Title = System.Web.HttpUtility.HtmlDecode(biz.Title);
            if (string.IsNullOrWhiteSpace(model.SubTitle)) model.SubTitle = System.Web.HttpUtility.HtmlDecode(biz.SubTitle);
            if (string.IsNullOrWhiteSpace(model.ParentId)) model.ParentId = biz.ParentId;
            if (string.IsNullOrWhiteSpace(model.Sequence)) model.Sequence = biz.Sequence;
            if (string.IsNullOrWhiteSpace(model.Type)) model.Type = string.IsNullOrEmpty(biz.Subtype) ? biz.Type : biz.Subtype;
            if (string.IsNullOrWhiteSpace(model.Url)) model.Url = biz.Href;
            if (string.IsNullOrWhiteSpace(model.Description)) model.Description = biz.Description;
            if (string.IsNullOrWhiteSpace(model.HrefDisciplineCourseId)) model.HrefDisciplineCourseId = biz.HrefDisciplineCourseId;
            model.BHParentId = biz.BHParentId;

            model.Sco = biz.Sco;
            Biz.DataContracts.MetadataValue disciplineId;

            if (biz.ProxyConfig != null)
            {
                model.ProxyConfig = new ProxyConfig()
                {
                    AllowComments = biz.ProxyConfig.AllowComments
                };
            }

            if (!biz.Metadata.TryGetValue("AgilixDisciplineId", out disciplineId))
            {
                // first check the entityid in the href than add the course id
                String hrefdId = null;
                if (biz.ItemDataXml != null)
                {
                    var hrefNode = biz.ItemDataXml.Element("href");
                    
                    if (hrefNode != null)
                    {
                        var node = hrefNode.Attributes("entityid").FirstOrDefault();
                        if (node != null)
                            hrefdId = node.Value;
                    }

                }
                if (!String.IsNullOrEmpty(hrefdId))
                {
                    disciplineId = new Biz.DataContracts.MetadataValue() { Value = hrefdId };
                }
                else
                {
                    disciplineId = new Biz.DataContracts.MetadataValue() { Value = biz.CourseId };
                }
            }

            model.DefaultPoints = biz.DefaultPoints;
            model.OverrideDueDateReq = biz.OverrideDueDateReq;
            if (string.IsNullOrWhiteSpace(model.DisciplineId)) model.DisciplineId = disciplineId.Value != null ? disciplineId.Value.ToString() : String.Empty;

            if (!biz.Metadata.IsNullOrEmpty() && biz.Metadata.ContainsKey("bfw_allowcomments"))
            {
                model.AllowComments = biz.Metadata["bfw_allowcomments"].As<bool>();
            }

            if (!biz.Properties.IsNullOrEmpty() && biz.Properties.ContainsKey("bfw_syllabusfilter"))
            {
                model.SyllabusFilter = biz.Properties["bfw_syllabusfilter"].As<string>();
            }

            if (!biz.Properties.IsNullOrEmpty() && biz.Properties.ContainsKey("bfw_submission_date"))
            {
                model.SubmittedDate = biz.Properties["bfw_submission_date"].As<DateTime>();
            }

            if (!string.IsNullOrEmpty(biz.Visibility))
            {
                model.Visibility = XElement.Parse(biz.Visibility, LoadOptions.None);
            }

            if (!biz.Categories.IsNullOrEmpty())
            {
                model.Categories = biz.Categories.Map(cat => cat.ToTocCategory());

                foreach (var cat in biz.Categories)
                {
                    if (cat.Id.Contains("PX_ASSIGNMENT_CENTER_SYLLABUS"))
                    {
                        model.SyllabusFilter = cat.Id;
                        break;
                    }
                }
            }

            if (biz.Containers != null)
            {
                model.Containers = GetContainers(biz.Containers);
            }

            if (biz.SubContainerIds != null)
            {
                model.SubContainerIds = GetContainers(biz.SubContainerIds);
            }

            if (!biz.FacetMetadata.IsNullOrEmpty())
            {
                biz.FacetMetadata.ToList().ForEach(i => model.FacetMetadata[i.Key] = i.Value);
            }

            model.SocialCommentingIntegration = biz.SocialCommentingIntegration;

            model.Thumbnail = biz.Thumbnail;
            model.Tooltip = biz.Tooltip;

            model.CustomFields = biz.CustomFields;

            model.ItemDataXml = biz.ItemDataXml;
        }

        public static AdminMetaData GetAdminMetaData(Biz.DataContracts.ContentItem biz)
        {
            XElement metaDataXml = biz.ItemDataXml.XPathSelectElement("./bfw_metadata_admin");

            AdminMetaData contentMetaData = null;
            if (metaDataXml != null)
                using (var reader = metaDataXml.CreateReader())
                {
                    reader.MoveToContent();
                    contentMetaData = PxXmlSerializer.Deserialize<AdminMetaData>(reader.ReadOuterXml());
                }

            return contentMetaData;
        }

        /// <summary>
        /// Convert to a base content item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        internal static BizDC.ContentItem ToBaseContentItem(this ContentItem model, string courseId)
        {
            var biz = new BizDC.ContentItem();
            
            if (null != model)
            {
                biz.Id = model.Id;
                biz.ParentId = model.ParentId;
                biz.Title = model.Title;
                biz.Description = model.Description;
                biz.CourseId = courseId;
                biz.Folder = model.Id;
                biz.Sequence = model.Sequence ?? "";
                biz.Visibility = model.Visibility.ToString();
                // changed this to reflect new show hid visibility properties -- g. chernyak
                biz.HiddenFromToc = model.Hidden;
                biz.DefaultCategoryParentId = model.DefaultCategoryParentId;
                // Additional visibility properties -- g. chernyak
                biz.AvailableDate = model.AvailableDate;
                biz.HiddenFromStudents = model.HiddenFromStudents;
                biz.SocialCommentingIntegration = model.SocialCommentingIntegration;
                biz.SubTitle = model.SubTitle;

                biz.Properties["bfw_syllabusfilter"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.SyllabusFilter };
                if (model is AssetLink)
                {
                    biz.Href = model.Url;
                    biz.HrefDisciplineCourseId = model.HrefDisciplineCourseId;
                    biz.Type = model.Type;
                }

                if (!model.Categories.IsNullOrEmpty())
                {
                    biz.Categories = model.Categories.Map(cat => cat.ToTocCategory()).ToList();
                }

                if (biz.AssessmentSettings != null)
                {
                    model.Policies = AssignmentHelper.PolicyDescriptionFromSettings(biz.AssessmentSettings, biz.Type);   
                }

                biz.Containers = model.Containers.Map(c => new BizDC.Container(c.Toc, c.Value, c.DlapType)).ToList();
                biz.SubContainerIds = model.SubContainerIds.Map(c => new BizDC.Container(c.Toc, c.Value, c.DlapType)).ToList();

            }

            return biz;
        }

        /// <summary>
        /// Maps a business contentitem to a model contentitem based on Type and SubType.  The object returned is
        /// of the concrete type, but cast to the ContentItem base class.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <param name="loadChildren">if set to <c>true</c> [load children].</param>
        /// <returns></returns>
        public static ContentItem ToContentItem(this BizDC.ContentItem biz, BizSC.IContentActions content, 
            bool loadChildren, IEnumerable<BizDC.ContentItem> children = null, bool getChildrenGrades = false,
            string toc = "syllabusfilter")
        {
            if (biz == null)
            {
                return null;
            }

            var model = new ContentItem();
            string type = string.IsNullOrEmpty(biz.Subtype) ? biz.Type : biz.Subtype;

            var context = content != null ? content.Context : null;
            

            //apply type-specific conversion
            switch (type.ToLowerInvariant())
            {
                case "htmlquiz":
                case "epage":
                    model = biz.ToHtmlQuiz(content, loadChildren);
                    model.AllowComments = true;
                    break;
                case "htmldocument":
                    model = biz.ToHtmlDocument();
                    break;

                case "documentcollection":
                    model = biz.ToDocumentCollection(content, loadChildren);
                    break;

                case "uploadorcompose":
                    model = biz.ToDocumentCollection(content, loadChildren);
                    ((DocumentCollection)model).DocumentCollectionSubType = "UploadOrCompose";
                    break;

                case "linkcollection":
                    model = biz.ToLinkCollection(content, loadChildren);
                    break;

                case "folder":
                case "bfw_toc_document":
                    model = biz.ToFolder();

                    if (loadChildren)
                    {
                        ((Folder)model).LoadChildren(content, biz.CourseId);
                    }
                    break;

                case "quiz":
                case "assessment":
                case "bfw_qti_document":
                case "homework":
                    //The html quizzes rendered through bh activity player have their bfw_type set to quiz
                    //They can be identified if they have an exam template set
                    if (!string.IsNullOrWhiteSpace(biz.ExamTemplate))
                        model = biz.ToHtmlQuiz(content);
                    else
                        model = biz.ToQuiz(content, null, loadChildren);
                    break;

                case "discussion":
                    model = biz.ToDiscussion(content, loadChildren);
                    break;

                case "rssfeed":
                    model = biz.ToRssFeed();
                    break;

                case "rsslink":
                    model = biz.ToRssLink();
                    break;

                case "link":
                    model = biz.ToLink();
                    break;
                case "assetlink":
                    model = biz.ToAssetLink();
                    break;

                case "externalcontent":
                case "bfw_document":
                    model = biz.ToExternalContent(context);
                    break;

                case "module":
                case "pxunit":
                    model = biz.ToPxUnit(content, toc, children, loadChildren);
                    break;

                case "learningcurveactivity":
                    model = biz.ToLearningCurveActivity(content, null, false, false);
                    break;
                case "dropbox":
                    model = biz.ToDropbox(context, content, loadChildren);
                    break;
                default:

                    break;
            }
            //apply standard conversion to base content item
            ToBaseContentItem(model, biz);

            DlapItemType dlaptype;
            if(Enum.TryParse(biz.Type, true, out dlaptype))
            {
                model.AgilixType = dlaptype;
            }
            else
            {
                model.AgilixType = DlapItemType.Custom;   
            }

            PopulateGradebookInfo(biz, model);
            model.SetSyllabusFilter(biz, toc);

            if (biz.Properties.ContainsKey("templateparent"))
            {
                model.TemplateParentId = biz.Properties["templateparent"].Value.ToString();
            }

            if (biz.Properties.ContainsKey("bfw_assignment_instructions"))
            {
                model.Instructions = biz.Properties["bfw_assignment_instructions"].Value.ToString();
            }

            if (biz.Properties.ContainsKey("bfw_contentcreate_assign"))
            {
                model.IsContentCreateAssign = Convert.ToBoolean(biz.Properties["bfw_contentcreate_assign"].Value.ToString());
            }

            if (biz.Properties.ContainsKey("FileName"))
            {
                if (!model.ExtendedProperties.ContainsKey("FileName"))
                    model.ExtendedProperties.Add("FileName", biz.Properties["FileName"].Value.ToString());
            }

            if (context != null)
            {
                model.UserAccess = context.AccessLevel;
            }

            return model;
        }

        private static void PopulateGradebookInfo(Biz.DataContracts.ContentItem biz, ContentItem model)
        {
            // GradebookInfo is populated on the contentactions getGradesPerItem method.
            if (biz.GradebookInfo != null)
            {
                model.TotalSubmissions = biz.GradebookInfo.TotalSubmissions;
                model.AverageScore = biz.GradebookInfo.AverageScore;
                model.TotalGrades = biz.GradebookInfo.TotalGrades;
                model.IsUserSubmitted = biz.GradebookInfo.IsUserSubmitted;
                model.IsUserGraded = biz.GradebookInfo.IsUserGraded;
                model.Score = biz.GradebookInfo.LastScore;
            }

            model.ReadDate = biz.ReadDate;
            model = AssignmentHelper.SetAssignmentSettings(model, biz.AssignmentSettings);
            SetVisibility(model, biz);
            model.Sequence = biz.Sequence;
            model.Hidden = biz.Hidden;
            if (string.IsNullOrEmpty(model.Description)) model.Description = biz.Description;
            model.DefaultCategoryParentId = biz.DefaultCategoryParentId;
            model.HiddenFromStudents = biz.HiddenFromStudents;

            model.AvailableDate = biz.AvailableDate;
            model.WasDueDateManuallySet = biz.WasDueDateManuallySet;
            model.IsItemLocked = biz.IsItemLocked;
            model.IsAssigned = (biz.AssignmentSettings != null) ? biz.AssignmentSettings.meta_bfw_Assigned : false;
        }

        /// <summary>
        /// Sets the syllabus filter.
        /// </summary>
        /// <param name="ciModel">The ci model.</param>
        /// <param name="biz">The biz.</param>
        public static void SetSyllabusFilter(this ContentItem ciModel, BizDC.ContentItem biz, string toc)
        {
            ciModel.SyllabusFilter = "";

            if (biz.Properties.ContainsKey("bfw_syllabusfilter"))
            {
                var syllabusfilter = biz.Properties["bfw_syllabusfilter"].Value;
                ciModel.SyllabusFilter = (syllabusfilter == null) ? "" : syllabusfilter.ToString();
            }

            if (string.IsNullOrEmpty(ciModel.SyllabusFilter))
            {
                ciModel.SyllabusFilter = biz.GetSyllabusFilterFromCategory(toc);
            }

        }

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="ciModel">The ci model.</param>
        /// <param name="biz">The biz.</param>
        public static void SetVisibility(this ContentItem ciModel, BizDC.ContentItem biz)
        {
            try
            {
                ciModel.Visibility = XElement.Parse(biz.Visibility, LoadOptions.None);
            }
            catch
            {
                ciModel.Visibility = XElement.Parse("<bfw_visibility><roles><instructor /> <student /> </roles> </bfw_visibility> ", LoadOptions.None);

                if (biz.Properties.ContainsKey("bfw_visibility"))
                {
                    var value = biz.Properties.ContainsKey("bfw_visibility") ? biz.Properties["bfw_visibility"].Value.ToString() : "";
                    if (!string.IsNullOrEmpty(value))
                    {
                        ciModel.Visibility = XElement.Parse(value, LoadOptions.None);
                    }
                }
            }
        }

        /// <summary>
        /// An overload to allow the client to specify whether or not to get children for the item where
        /// applicable, e.g., folders.  Default is to not get children.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// /// <param name="isLoadChildren">load child items?</param>
        /// <returns></returns>
        public static ContentItem ToContentItem(this BizDC.ContentItem biz, BizSC.IContentActions content, bool isLoadChildren = false)
        {
            return biz.ToContentItem(content, loadChildren: isLoadChildren);
        }

        /// <summary>
        /// Returns a string representing the Object type or SubType.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static string ModelType(this BizDC.ContentItem biz)
        {
            string type = string.IsNullOrEmpty(biz.Subtype) ? biz.Type : biz.Subtype;
            return type;
        }

        /// <summary>
        /// Returns the a list of Container
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Container> GetContainers(List<Bfw.PX.Biz.DataContracts.Container> containers)
        {
            List<Container> containerList = new List<Container>();
            foreach (var container in containers)
            {
                containerList.Add(new Container(container.Toc, container.Value, container.DlapType));
            }
            return containerList;
        }

        /// <summary>
        /// Updates a content item with the edited fields for that content item
        /// title, subtitle, description
        /// </summary>
        /// <param name="item"></param>
        /// <param name="editedItem"></param>
        /// <returns></returns>
        public static ContentItem UpdateContentItemWithEdits(this BizDC.ContentItem item, BizDC.ContentItem editedItem, BizSC.IContentActions content)
        {
            item.Title = editedItem.Title;
            item.SubTitle = editedItem.SubTitle;
            item.Description = editedItem.Description;

            return item.ToContentItem(content);
        }
    }
}
