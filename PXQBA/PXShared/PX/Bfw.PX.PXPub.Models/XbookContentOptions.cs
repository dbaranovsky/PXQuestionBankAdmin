using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Holds information for properly displaying a content item's header in content area.  Right now is xbook specific
    /// </summary>
    public class XbookContentOptions
    {
        /// <summary>
        /// Contained all except ReadyOnly, None, Metadata
        /// </summary>
        private static ContentViewMode BaseOptions = ContentViewMode.Assign | ContentViewMode.Create | ContentViewMode.Discussion | ContentViewMode.Edit | ContentViewMode.ExternalUrl |
                                                ContentViewMode.Grading | ContentViewMode.MoreResources | ContentViewMode.MoreResourcesStatic | ContentViewMode.Preview | 
                                                ContentViewMode.Questions | ContentViewMode.Results | ContentViewMode.Rubrics | ContentViewMode.Settings;

        #region Content view options
        /// <summary>
        /// Holds the ContentViewModes for all supported content types
        /// </summary>
        private static Dictionary<Type, ContentViewMode> ContentViewOptions = new Dictionary<Type, ContentViewMode>()
        { 
            //Document Collection
            { typeof(DocumentCollection), ContentViewMode.Edit | ContentViewMode.Create | ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings },
            //Folder
            { typeof(Folder), ContentViewMode.Edit | ContentViewMode.Create | ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings },
            //Quiz
            { typeof(Quiz), ContentViewMode.Edit | ContentViewMode.Create | ContentViewMode.Preview | ContentViewMode.Assign | ContentViewMode.Settings | 
                ContentViewMode.Questions },
            //HtmlQuiz
            { typeof(HtmlQuiz), ContentViewMode.Preview | ContentViewMode.Assign |  ContentViewMode.Settings },
            { typeof(HtmlDocument), ContentViewMode.Edit | ContentViewMode.Preview | ContentViewMode.Assign |  ContentViewMode.Settings },
            { typeof(ExternalContent), ContentViewMode.Preview | ContentViewMode.Assign |  ContentViewMode.Settings },
            { typeof(PxUnit), ContentViewMode.Preview | ContentViewMode.Assign |  ContentViewMode.Settings }
        };
        #endregion Content view options

        #region Product view options
        /// <summary>
        /// Holds the content view mode options for each product type
        /// </summary>
        private static Dictionary<string, ContentViewMode> ProductViewOptions = new Dictionary<string, ContentViewMode>()
        {
            { "xbook", BaseOptions & ~ContentViewMode.Rubrics },
            { "xbookv2", BaseOptions & ~ContentViewMode.Rubrics },
        };
        #endregion Product view options

        /// <summary>
        /// Holds the content view mode options for all access levels
        /// </summary>
        private static Dictionary<AccessLevel, ContentViewMode> AccessLevelViewOptions = new Dictionary<AccessLevel, ContentViewMode>() 
        {
            { AccessLevel.Student, (BaseOptions | ContentViewMode.ReadOnly) & ~(ContentViewMode.Edit) & ~(ContentViewMode.Questions) & ~(ContentViewMode.Assign) & ~(ContentViewMode.Settings) },       
            { AccessLevel.Instructor, BaseOptions },
            { AccessLevel.None, ContentViewMode.None }
        };

        #region Product/Course/Item States
        /// <summary>
        /// Holds content view options for when the content is assigned
        /// </summary>
        public static ContentViewMode IsNotAssignedViewOptions = BaseOptions & ~ContentViewMode.Results;
        /// <summary>
        /// Holds content view options for when the product is in sandbox mode
        /// </summary>
        public static ContentViewMode IsSandboxViewOptions = BaseOptions | ContentViewMode.Metadata;
        /// <summary>
        /// Holds content view options for when the course has more resources TODO: WHAT????????
        /// </summary>
        public static ContentViewMode HasMoreResourcesViewOptions = BaseOptions;
        /// <summary>
        /// Holds content view options for when product has rubrics or learning objectives
        /// </summary>
        public static ContentViewMode HasRubricOrLearningObjectiveViewOptions = BaseOptions;
        /// <summary>
        /// Holds content view options for when the course is a product course. Ln 635 - ContentWidgetController
        /// </summary>
        public static ContentViewMode IsProductCourseViewOptions = BaseOptions & ~(ContentViewMode.Assign) & ~(ContentViewMode.Settings) & ~(ContentViewMode.Create);
        /// <summary>
        /// Holds content view options for when the course is shared
        /// </summary>
        public static ContentViewMode IsSharedViewOptions = ContentViewMode.None;
        #endregion Product/Course/Item States

        /// <summary>
        /// Id of the content item
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// The active view mode for the content item
        /// </summary>
        public ContentViewMode ActiveOption { get; set; }
        /// <summary>
        /// The available view modes for the content item
        /// </summary>
        public ContentViewMode Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemid">ID of the content item to create header options for</param>
        /// <param name="itemType">Type of the item to create header options for</param>
        /// <param name="productType">Type of the produce to create header options for</param>
        /// <param name="accesslevel">Current access level of user</param>
        /// <param name="isSandboxCourse">If the current course is a sandbox course</param>
        /// <param name="isSharedCourse">If the current course is a shared course</param>
        /// <param name="isProductCourse">If the current course is a product course</param>
        /// <param name="isAssigned">If the current content item is assigned (due date set).</param>
        public XbookContentOptions(string itemid, Type itemType, string productType, AccessLevel accesslevel, bool isSandboxCourse, bool isSharedCourse, 
                                    bool isProductCourse, bool isAssigned)
        {
            ItemId = itemid;
            Options = XbookContentOptions.ContentViewOptions[itemType] & XbookContentOptions.ProductViewOptions[productType] & XbookContentOptions.AccessLevelViewOptions[accesslevel];

            //TODO: Are these applicable to xbook
            if (isSandboxCourse)
                Options &= XbookContentOptions.IsSandboxViewOptions;
            if (isSharedCourse)
                Options &= XbookContentOptions.IsSharedViewOptions;
            if (isProductCourse)
                Options &= XbookContentOptions.IsProductCourseViewOptions;
            if (!isAssigned)
                Options &= XbookContentOptions.IsNotAssignedViewOptions;
        }

        /// <summary>
        /// Checks if <paramref name="option"/> is an available view mode for the content
        /// </summary>
        /// <param name="option">The view mode to check if is available</param>
        /// <returns>True is the content item is viewable in this mode</returns>
        public bool IsOptionAvailable(ContentViewMode option)
        {
            return (Options & option) == option;
        }  
    }
}
