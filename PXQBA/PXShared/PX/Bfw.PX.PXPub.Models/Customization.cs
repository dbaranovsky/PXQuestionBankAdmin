using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.ServiceContracts;
using System.Web;

namespace Bfw.PX.PXPub.Models
{
    public class Customization
    {
        /// <summary>
        /// The context in which the customization is taking place.
        /// </summary>
        public Context DisplayContext { get; set; }

        /// <summary>
        /// The ID of the item being customized, or null for a course.
        /// </summary>
        public string ItemId { get; set; }

        public string ParentId { get; set; }

        public string Sequence { get; set; }

        public string Title { get; set; }

        public string CoverTitle { get; set; }

        public bool IsFolderPersonalization { get; set; }

        public string Instructorname { get; set; }

        public string BannerImage { get; set; }

        public string Instructions { get; set; }

        public string SelectedTheme { get; set; }

        public string CourseType { get; set; }

        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the access level.
        /// </summary>
        public AccessLevel AccessLevel { get; set; }

        public List<Theme> Themes = new List<Theme>();

        public HttpPostedFileBase docFile { get; set; }

        public string SaveController { get; set; }

        public string SaveAction { get; set; }

        public bool EditTitleByUser { get; set; }
        public class Theme
        {
            public string Title { get; set; }
            public string Color { get; set; }
            public string Key { get; set; }
        }

        public enum Context
        {
            HomePage
        }

        /// <summary>
        /// Sharing Status
        /// </summary>
        public string PublishedStatus { get; set; }

        /// <summary>
        /// Search Engine Index 
        /// </summary>
        public bool SearchEngineIndex { get; set; }

        /// <summary>
        /// Facebook Integration 
        /// </summary>
        public bool FacebookIntegration { get; set; }

        /// <summary>
        /// Turns on and off social comments across the entire course (disqus.com)
        /// </summary>
        public bool SocialCommentIntegration { get; set; }
    }
}
