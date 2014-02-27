using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml.XPath;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;
using System.Xml.Linq;
using BizDC = Bfw.PX.Biz.DataContracts;

using System.ComponentModel;
using System.Reflection;

namespace Bfw.PX.PXPub.Models
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static void TemplateFor<TModel>(this HtmlHelper<TModel> html, object model, String folder)
        {
            html.RenderPartial(folder + "Templates/" + model.GetType().Name, model);
        }
        
        public static string Pager(this HtmlHelper helper, int currentPage, int currentPageSize, int totalRecords, string urlPrefix)
        {
            StringBuilder sb1 = new StringBuilder();

            int seed = currentPage % currentPageSize == 0 ? currentPage : currentPage - (currentPage % currentPageSize);

            if (currentPage > 0)
                sb1.AppendLine(String.Format("<a href=\"{0}/{1}\">Previous</a>", urlPrefix, currentPage));

            if (currentPage - currentPageSize >= 0)
                sb1.AppendLine(String.Format("<a href=\"{0}/{1}\">...</a>", urlPrefix, (currentPage - currentPageSize) + 1));

            for (int i = seed; i < Math.Round((totalRecords / 10) + 0.5) && i < seed + currentPageSize; i++)
            {
                sb1.AppendLine(String.Format("<a href=\"{0}/{1}\">{1}</a>", urlPrefix, i + 1));
            }

            if (currentPage + currentPageSize <= (Math.Round((totalRecords / 10) + 0.5) - 1))
                sb1.AppendLine(String.Format("<a href=\"{0}/{1}\">...</a>", urlPrefix, (currentPage + currentPageSize) + 1));

            if (currentPage < (Math.Round((totalRecords / 10) + 0.5) - 1))
                sb1.AppendLine(String.Format("<a href=\"{0}/{1}\">Next</a>", urlPrefix, currentPage + 2));

            return sb1.ToString();
        }

        public static string GetMultipartPartLessonId(System.Web.HttpRequest request, ViewDataDictionary vd)
        {
            var lessonId = (request.QueryString["hasParentLesson"] != null) ? request.QueryString["hasParentLesson"].ToString() : "";
            if (lessonId == "")
            {
                lessonId = (vd["lessonId"] != null) ? vd["lessonId"].ToString() : "";
            }

            if (lessonId == "")
            {
                lessonId = (vd["hasParentLesson"] != null) ? vd["hasParentLesson"].ToString() : "";
            }

            return lessonId;
        }

        public static string GetVisibilityClasses(this NavigationItem navItem, bool isProductCourse)
        {
            bool hiddenFromStudent = navItem.Visibility.Descendants("student").Count() == 0;
            bool hiddenFromInstructor = navItem.Visibility.Descendants("student").Count() == 0;
            string styleClass = "";
            if (navItem.Highlighted)
            {
                styleClass = "highlighted";
            }

            if (hiddenFromInstructor)
            {
                styleClass += " hiddenFromInstructorSSlock";
            }

            if (hiddenFromStudent)
            {
                styleClass += " hiddenFromStudentSSlock";
            }

            if (isProductCourse)
                styleClass += " displaynone";

            return styleClass;
        }

        public static string GetVisibilityClasses(this Link lnk, bool isProductCourse)
        {

            bool hiddenFromStudent = lnk.Visibility.Descendants("student").Count() == 0;
            bool hiddenFromInstructor = lnk.Visibility.Descendants("student").Count() == 0;
            string styleClass = "";

            if (lnk.Visibility.Descendants("instructor").Count() == 0)
            {
                styleClass = " hiddenFromInstructorSSlock";
            }

            if (hiddenFromStudent)
            {
                styleClass += " hiddenFromStudentSSlock";
            }

            if (isProductCourse)
                styleClass += " displaynone";

            return styleClass;
        }

        public static string GetVisibilityClasses(this WidgetConfiguration lnk, bool isProductCourse)
        {

            bool hiddenFromStudent = lnk.Visibility.Descendants("student").Count() == 0;
            bool hiddenFromInstructor = lnk.Visibility.Descendants("student").Count() == 0;
            string styleClass = "";

            if (lnk.Visibility.Descendants("instructor").Count() == 0)
            {
                styleClass = " hiddenFromInstructorSSlock";
            }

            if (hiddenFromStudent)
            {
                styleClass += " hiddenFromStudentSSlock";
            }

            if (isProductCourse)
                styleClass += " displaynone";

            return styleClass;
        }

        public static string GetVisibilityClasses(this Widget lnk, bool isProductCourse)
        {
            string styleClass = "";

            if (lnk.WidgetDisplayOptions.DisplayOptions.Contains(Biz.DataContracts.DisplayOption.Instructor))
            {
                styleClass = " hiddenFromInstructorSSlock";
            }

            if (lnk.WidgetDisplayOptions.DisplayOptions.Contains(Biz.DataContracts.DisplayOption.Student))
            {
                styleClass = " hiddenFromStudentSSlock";
            }


            if (isProductCourse)
            {
                styleClass += " displaynone";
            }

            return styleClass;
        }

        public static string GetVisibilityClasses(this AssignmentCenterFilterSection filter, bool isProductCourse)
        {
            string styleClass = "";

            //if (filter.Visibility.Elements("instructor").Count() == 0)
            //{
            //    styleClass = " hiddenFromInstructorSS";
            //}

            if (filter.Id == "PX_ASSIGNMENT_CENTER_SYLLABUS_INSTRUCTOR_WORKSPACE")
            {
                if (filter.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
                {
                    styleClass += " displaynone";
                }
            }
            else
            {
                if (filter.Visibility.Descendants("student").Count() == 0)
                {
                    styleClass += " hiddenFromStudentSS";
                }

                if (isProductCourse)
                    styleClass += " displaynone";
            }

            return styleClass;
        }

        public static string GetVisibilityClasses(this ContentItem contentItem, bool isProductCourse, bool isSupportInstuctor, bool isSupportStudent)
        {

            string styleClass = "";

            if (isSupportInstuctor)
            {
                if (contentItem.Visibility.Descendants("instructor").Count() == 0)
                {
                    styleClass = " hiddenFromInstructorSS";
                }
            }

            if (isSupportStudent)
            {
                if (contentItem.HiddenFromStudents)
                {
                    styleClass += " hiddenFromStudentSS";
                }
            }

            if (isProductCourse)
                styleClass += " displaynone";

            return styleClass;
        }

        public static Boolean ApplyRestrictedAccess(this ContentItem contentItem, bool asStudent = false)
        {
            if (!asStudent && contentItem.UserAccess != AccessLevel.Student)
            {
                return false;
            }

            if (contentItem.Visibility.Descendants("restriction").Count() > 0)
            {
                var dateString = RestrictedDate(contentItem);
                dateString = string.IsNullOrEmpty(dateString) ? DateTime.MinValue.ToShortDateString() : dateString;

                DateTime restrictedDate = DateTime.MinValue;
                DateTime.TryParse(dateString, out restrictedDate);

                if (restrictedDate > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        public static Boolean RestrictedAccess(this ContentItem contentItem)
        {
            return RestrictedByDate(contentItem);
        }

        public static Boolean RestrictedByDate(this ContentItem contentItem)
        {
            return (contentItem.Visibility.Descendants("date").Count() > 0);
        }

        public static string RestrictedDate(this ContentItem contentItem)
        {
            var restriction = contentItem.Visibility.Descendants("date");

            if (restriction.Count() > 0)
            {
                XName date = "endate";
                var dateElement = restriction.Attributes(date).FirstOrDefault();

                if (dateElement != null)
                {
                    var restrictedDate = dateElement.Value;

                    // fix for dates that were previously saved as MM/dd/yyy hh:mm tt
                    DateTime dt;
                    DateTime.TryParse(restrictedDate, out dt);
                    if (dt.Kind == DateTimeKind.Unspecified)
                    {
                        restrictedDate += " GMT";
                    }

                    return restrictedDate;
                }
            }

            return "";
        }

        public static Boolean HideFromStudents(this ContentItem contentItem)
        {
            return (contentItem.Visibility.Descendants("student").Count() == 0);
        }
        /// <summary>
        /// Set the SetSyllabusFilter
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="filterId"></param>
        public static void SetSyllabusFilter(this Bfw.PX.Biz.DataContracts.ContentItem contentItem, string filterId)
        {
            if (contentItem.Properties.ContainsKey("bfw_syllabusfilter"))
            {
                contentItem.Properties["bfw_syllabusfilter"].Value = filterId;
            }
            else
            {
                contentItem.Properties["bfw_syllabusfilter"] = new Bfw.PX.Biz.DataContracts.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = filterId };
            }
        }

        /// <summary>
        /// Set the SetSyllabusFilter
        /// </summary>
        /// <param name="contentItem"></param>
        /// <param name="filterId"></param>
        public static void SetSyllabusFilterCategory(this Bfw.PX.PXPub.Models.ContentItem contentItem, string filterId, string toc, string sequence)
        {
           
            var cat = contentItem.Categories.FirstOrDefault(c => c.Id == toc);
            if (cat == null)
            {
                cat = new TocCategory();
                cat.Id = toc;
                cat.Sequence = contentItem.Sequence;
                var categories = contentItem.Categories.ToList();
                categories.Add(cat);
                contentItem.Categories = categories;

            }
            cat.ItemParentId = filterId;
            if (sequence != null && sequence != cat.Sequence)
            {
                cat.Sequence = sequence;
            }
        }

        /// <summary>
        /// Get friendly content type string
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetFriendlyItemContentType(this ContentItem item)
        {
            string result = string.Empty;
            
            result = String.IsNullOrWhiteSpace(item.SubTitle)? string.Empty : item.SubTitle;


            return result;
        }
        /// <summary>
        /// Get the completion status list based on the Content type
        /// </summary>
        /// <param name="assignedItem"></param>
        /// <returns></returns>
        public static List<SelectListItem> CompletionStatusSelectItem(this AssignedItem assignedItem)
        {
            var selectListItems = new List<SelectListItem>();

            // Check for self-scoring first
            if (assignedItem.Sco)
            {
                selectListItems.Add(new SelectListItem() { Text = "Submits the activity", Value = "1" });
                selectListItems.Add(new SelectListItem() { Text = "Achieves a passing score", Value = "2" });
            }
            else
            {
                switch (assignedItem.SourceType.ToLowerInvariant())
                {
                    case "documentcollection":
                    case "externalcontent":
                    case "link":
                    case "rsslink":
                    case "assetlink":
                    case "linkcollection":
                    case "htmldocument":
                        selectListItems.Add(new SelectListItem() { Text = "Views the activity", Value = "1" });
                        selectListItems.Add(new SelectListItem() { Text = "Views the activity for a specified amount of time", Value = "0" });
                        break;
                    case "discussion":
                        selectListItems.Add(new SelectListItem() { Text = "Makes a post or response", Value = "1" });
                        break;
                    case "quiz":
                    case "assessment":
                    case "bfw_qti_document":
                    case "homework":
                        selectListItems.Add(new SelectListItem() { Text = "Makes a submission", Value = "1" });
                        selectListItems.Add(new SelectListItem() { Text = "Achieves a passing score", Value = "2" });
                        break;
                    case "assignment":
                    case "dropbox":
                        selectListItems.Add(new SelectListItem() { Text = "Makes a submission", Value = "1" });
                        break;
                    default: // Check for default completion triggers
                        foreach (var trigger in assignedItem.AvailableCompletionTriggers)
                        {
                            switch (trigger)
                            {
                                case CompletionTrigger.Minutes:
                                    selectListItems.Add(new SelectListItem() { Text = "Minutes spent", Value = "0" });
                                    break;
                                case CompletionTrigger.Submission:
                                    selectListItems.Add(new SelectListItem() { Text = "Submits the activity", Value = "1" });
                                    break;
                                case CompletionTrigger.PassingScore:
                                    selectListItems.Add(new SelectListItem() { Text = "Achieves a passing score", Value = "2" });
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }

            return selectListItems;
        }

        public static string GetSyllabusFilterFromCategory(this BizDC.NavigationItem contentItem, string toc)
        {
            string syllabusfilter = "";
            if (string.IsNullOrWhiteSpace(toc))
            {
                toc = "syllabusfilter";
            }
            foreach (BizDC.TocCategory category in contentItem.Categories)
            {
                if (category.Id == toc)
                {
                    syllabusfilter = category.ItemParentId;
                }
            }

            if (string.IsNullOrEmpty(syllabusfilter))
            {
                foreach (BizDC.TocCategory category in contentItem.Categories)
                {
                    if (category.Id == category.ItemParentId)
                    {
                        syllabusfilter = category.ItemParentId;
                    }
                }
            }

            return syllabusfilter;
        }

        public static string GetSyllabusFilterFromCategory(this ContentItem contentItem, string toc)
        {
            string sequence = "";
            return GetSyllabusFilterFromCategory(contentItem, out sequence, toc);
        }

        public static string GetSyllabusFilterFromCategory(this ContentItem contentItem, out string sequence, string toc)
        {
            string syllabusfilter = "";
            sequence = "";

            if (string.IsNullOrWhiteSpace(toc))
            {
                toc = "syllabusfilter";
            }
            foreach (TocCategory category in contentItem.Categories)
            {
                if (category.Id == toc)
                {
                    syllabusfilter = category.ItemParentId;
                    sequence = category.Sequence;
                }
            }

            if (string.IsNullOrEmpty(syllabusfilter))
            {
                foreach (TocCategory category in contentItem.Categories)
                {
                    if (category.Id == category.ItemParentId)
                    {
                        syllabusfilter = category.ItemParentId;
                        sequence = category.Sequence;
                    }
                }
            }

            if (string.IsNullOrEmpty(syllabusfilter) && toc != "syllabusfilter")
            {
                //if toc not found, default to syllabusfilter (for shitty data) 
                //TODO: remove this when sitebuilder imports the appropriate bfw_tocs
                return contentItem.GetSyllabusFilterFromCategory(out sequence, "syllabusfilter");
            }

            return syllabusfilter;
        }

        public static string GetSequenceFromCategory(this ContentItem contentItem, string toc)
        {
            var sequence = "";
            contentItem.GetSyllabusFilterFromCategory(out sequence, toc);
            return sequence;
        }

        /// <summary>
        /// Returns the DateTime adjusted for couse time zone
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetCourseDateTime(this DateTime dt)
        {
            IBusinessContext context = null;
            try
            {
                context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            }
            catch { }
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
        /// Returns abbreviated timezone for the course 
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public static string GetCourseTimeZoneAbbreviation(this BizDC.Course course)
        {
            string timeZone = string.Empty;

            course.CourseTimeZone.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(r =>
                {
                    timeZone += r[0];
                });

            return timeZone;
        }

        public static string GetSyllabusFilterFromCategory(this BizDC.ContentItem contentItem, string toc)
        {
            string sequence = "";
            return GetSyllabusFilterFromCategory(contentItem, toc, out sequence);
        }

        public static string GetSyllabusFilterFromCategory(this Bfw.PX.Biz.DataContracts.ContentItem contentItem, string toc, out string sequence)
        {
            string syllabusfilter = "";
            sequence = "";
            if (string.IsNullOrWhiteSpace(toc))
            {
                toc = "syllabusfilter";
            }
            if (contentItem.Categories != null)
            {
                foreach (var category in contentItem.Categories)
                {
                    if (category.Id == toc)
                    {
                        syllabusfilter = category.ItemParentId;
                        sequence = category.Sequence;
                    }
                }


                if (string.IsNullOrEmpty(syllabusfilter))
                {
                    foreach (var category in contentItem.Categories)
                    {
                        if (category.Id == category.ItemParentId)
                        {
                            syllabusfilter = category.ItemParentId;
                            sequence = category.Sequence;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(syllabusfilter))
            {
                syllabusfilter = contentItem.ParentId;
                sequence = contentItem.Sequence;
            }

            return syllabusfilter;
        }

        /// <summary>
        /// Set the property value or add a new property
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(this BizDC.ContentItem biz, string key, string value)
        {
            if (biz.Properties.ContainsKey(key))
            {
                biz.Properties[key] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = value };
            }
            else
            {
                biz.Properties.Add(key, new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = value });
            }
        }

        /// <summary>
        /// get the value from properties
        /// </summary>
        /// <param name="biz"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromProperty(this BizDC.ContentItem biz, string key)
        {
            if (biz.Properties.ContainsKey(key))
            {
                return biz.Properties[key].Value.ToString();
            }

            return "";
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Determines whether the specified item is removable by running the query filter against the item's xml data.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="setting">The setting.</param>
        /// <returns></returns>
        public static bool IsRemovable(this BizDC.ContentItem item, RemovableSetting setting)
        {
            var isRemovable = setting.Switch;

            if (setting.XPathQueryFilter == null) return isRemovable;

            if (isRemovable)
            {
                isRemovable = item.ItemDataXml.XPathSelectElements(setting.XPathQueryFilter).Any();
            }
 
            return isRemovable;
        }
    }
}
