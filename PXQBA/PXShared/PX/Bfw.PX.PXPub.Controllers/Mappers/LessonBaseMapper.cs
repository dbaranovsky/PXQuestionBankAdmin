using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;
using System.Xml.Linq;
using System.Configuration;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class LessonBaseMapper
    {

        /// <summary>
        /// Convert to a Biz content item from a LessonBase.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this LessonBase model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);
            var rootFolder = ConfigurationManager.AppSettings["assignmentCenterRootFolder"];
            if (null != model)
            {
                biz.Type = "Folder";
                biz.Subtype = "PxUnit";
                if (model is PxUnit) biz.ParentId = rootFolder;
                biz.Properties["thumbnail"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.Thumbnail };
                biz.Properties["associatedtocourse"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.AssociatedToCourse };
                biz.Properties["bfw_syllabusfilter"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.SyllabusFilter };

                if (model.AssignedItem == null)
                {
                    model.AssignedItem = new AssignedItem();
                }

                if (model.AssignedItem.Score == null)
                {
                    model.AssignedItem.Score = new Score();
                }

                biz.AssignmentSettings = new BizDC.AssignmentSettings()
                {
                    IsAssignable = true,
                    DropBoxType = BizDC.DropBoxType.None,
                    DueDate = model.DueDate,
                    Points = model.AssignedItem.Score.Possible,
                    Category = model.AssignedItem.Category,
                    CompletionTrigger = (BizDC.CompletionTrigger)model.AssignedItem.CompletionTrigger
                };

                if (model.Categories.IsNullOrEmpty())
                {
                    biz.DefaultCategoryParentId = biz.ParentId;
                    biz.DefaultCategorySequence = "";
                }

                var associatedTocItems = new XElement("associatedTOCItems");

                if (!model.GetAssociatedItems().IsNullOrEmpty())
                {
                    foreach (var item in model.GetAssociatedItems())
                    {
                        associatedTocItems.Add(new XElement("tocItem", new XAttribute("tocId", item.Id)));
                    }
                }

                biz.Properties["associatedTOCItems"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = associatedTocItems.ToString() };

            }

            return biz;
        }

        /// <summary>
        /// Convert to a Lesson Base from a Content Item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        internal static void ToLessonBase(this LessonBase model, BizDC.ContentItem biz, BizSC.IContentActions content, 
            string toc = "syllabusfilter", IEnumerable<BizDC.ContentItem> children = null, bool getChildrenGrades = false, bool isLoadChildren = true)
        {
            using (content.Context.Tracer.StartTrace(string.Format("ToLessonBase(lessonid={0})", biz.Id)))
            {
                model.Id = biz.Id;
                model.Title = biz.Title;
                model.ParentId = biz.ParentId;
                model.Sequence = biz.Sequence;
                model.AttachedToDateCategory = "";
                model.Description = biz.Description;
                model.UnitChapter = biz.UnitChapter;

                if (biz.Properties.ContainsKey("bfw_syllabusfilter"))
                {
                    if (biz.Properties["bfw_syllabusfilter"].Value != null)
                    {
                        model.SyllabusFilter = biz.Properties["bfw_syllabusfilter"].Value.ToString();
                    }
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

                model.Thumbnail = biz.Properties.ContainsKey("thumbnail") ? biz.Properties["thumbnail"].Value.ToString() : "";
                model.Visibility = string.IsNullOrEmpty(biz.Visibility) ? new XElement("Visibility") : XElement.Parse(biz.Visibility, LoadOptions.None);
                model.AssociatedToCourse = biz.Properties.ContainsKey("associatedtocourse") ? biz.Properties["associatedtocourse"].Value.ToString() : "";

                var assigmentItem = biz.ToAssignedItem();

                if (assigmentItem != null)
                {
                    model.DueDate = assigmentItem.DueDate;
                    model.StartDate = assigmentItem.StartDate;
                }
                
                IEnumerable<BizDC.ContentItem> items = null;
                IEnumerable<BizDC.ContentItem> legacyItems = null;

                if (children != null)
                {
                   items = children.Filter(c => c.Categories.FirstOrDefault(cat => cat.Id == biz.Id || cat.ItemParentId == biz.Id) != null);
                }

                if (items.IsNullOrEmpty() && isLoadChildren)
                {
                    items = content.ListChildren(content.Context.CourseId, biz.Id, 1, toc);
                    legacyItems = content.ListChildren(content.Context.CourseId, biz.Id, 1, biz.Id);
                }

                var itemsList = (items == null) ? new List<BizDC.ContentItem>() : items.ToList();

                if (!legacyItems.IsNullOrEmpty())
                {
                    foreach (var item in legacyItems)
                    {
                        if (itemsList.Exists(i => i.Id == item.Id) == false)
                        {
                            itemsList.Add(item);
                        }
                    }
                }


                foreach (var item in itemsList)
                {
                    var ci = item.ToContentItem(content, false, children);
                    ci.MaxPoints = item.MaxPoints;
                    assigmentItem = item.ToAssignedItem();
                    if (assigmentItem != null)
                    {
                        ci.DueDate = assigmentItem.DueDate;
                    }

                    var cat = item.Categories.FirstOrDefault(i => i.Id == biz.Id);
                    if (cat != null)
                    {
                        ci.Sequence = cat.Sequence;
                    }

                    ci.ReadOnly = (content.Context.AccessLevel == BizSC.AccessLevel.Student);
                    ci.ParentId = model.Id;
                    model.AddAssociatedItem(ci);
                }

                if (model.Categories.IsNullOrEmpty())
                {
                    biz.DefaultCategoryParentId = model.ParentId;
                    biz.DefaultCategorySequence = model.Sequence;
                }
                
                if (biz.FacetMetadata.ContainsKey("meta-topic"))
                {
                    model.MoreResourcesTags.Add("topic", biz.FacetMetadata["meta-topic"]);
                }
                if (biz.FacetMetadata.ContainsKey("meta-topics/meta-topic"))
                {
                    model.MoreResourcesTags.Add("topic", biz.FacetMetadata["meta-topics/meta-topic"]);
                }
                if (biz.FacetMetadata.ContainsKey("meta-subtopic"))
                {
                    model.MoreResourcesTags.Add("subtopic", biz.FacetMetadata["meta-subtopic"]);
                }
                if (biz.FacetMetadata.ContainsKey("meta-content-type"))
                {
                    model.MoreResourcesTags.Add("type", biz.FacetMetadata["meta-content-type"]);
                }
            }
        }
    }
}
