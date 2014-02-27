using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Holds data necessary to display a view of a content item
    /// </summary>
    public class ContentView
    {
        /// <summary>
        /// Mode in which the view is currently operating
        /// </summary>
        /// <value>
        /// The active mode.
        /// </value>
        public ContentViewMode ActiveMode { get; set; }

        /// <summary>
        /// Returns whether the current mode is an "edit" mode
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool ContentIsBeingEdited()
        {
            return ActiveMode == ContentViewMode.Settings || ActiveMode == ContentViewMode.Edit ||
                   ActiveMode == ContentViewMode.Questions || ActiveMode == ContentViewMode.Metadata ||
                   ActiveMode == ContentViewMode.Assign || ActiveMode == ContentViewMode.Rubrics;
        }
        /// <summary>
        /// Based on permissions, this is a set of the views the user is allowed to see
        /// </summary>
        /// <value>
        /// The allowed modes.
        /// </value>
        public ContentViewMode AllowedModes { get; set; }

        /// <summary>
        /// Item to display in the view
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public ContentItem Content { get; set; }

        /// <summary>
        /// Url to display in lieu of a content item
        /// </summary>
        /// <value>
        /// The URL
        /// </value>
        public string Url { get; set; }


        /// <summary>
        /// Gets or sets the content items.
        /// </summary>
        /// <value>
        /// The content items.
        /// </value>
        public List<ContentItem> ContentItems { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IEnumerable<TocCategory> Categories { get; set; }

        /// <summary>
        /// Gets or sets the table of contents.
        /// </summary>
        /// <value>
        /// The table of contents.
        /// </value>
        public IEnumerable<TocItem> TableOfContents { get; set; }

        /// <summary>
        /// Null if the ContentItem doesn't have any assignment data. Otherwise
        /// this contains information about when a ContentItem is due, how many points
        /// it is worth, etc.
        /// </summary>
        /// <value>
        /// The assigned item.
        /// </value>
        public AssignedItem AssignedItem { get; set; }

        /// <summary>
        /// Whether or not to include the discussion view.
        /// </summary>
        /// <value>
        /// The include discussion.
        /// </value>
        public bool? IncludeDiscussion { get; set; }

        /// <summary>
        /// Whether or not to Display the Social Commenting (Discus).
        /// </summary>
        /// <value>
        /// The include Social Commenting.
        /// </value>
        public SocialCommentingState SocialCommenting { get; set; }

        /// <summary>
        /// Whether or not to include the navigation view.
        /// </summary>
        /// <value>
        /// The include navigation.
        /// </value>
        public bool? IncludeNavigation { get; set; }

        /// <summary>
        /// Whether or not to include the content modes
        /// </summary>
        /// <value>
        /// The include navigation.
        /// </value>
        public bool? IncludeContentModes { get; set; }

        /// <summary>
        /// Whether or not to create FNE links with content modes
        /// </summary>
        /// <value>
        /// The include navigation.
        /// </value>
        public bool? RenderContentModesInFne { get; set; }

        /// <summary>
        /// Whether or not to include the breadcrumb
        /// </summary>
        /// <value>
        /// The include navigation.
        /// </value>
        public bool? IncludeBreadcrumb { get; set; }


        /// <summary>
        /// Whether or not the view is a template editor
        /// </summary>
        public bool IsTemplateEditor { get; set; }

        public string DomainUserSpace { get; set; }

        /// <summary>
        /// List of allowed content items which can be created from the Assign tab
        /// </summary>        
        public List<RelatedTemplate> RelatedTemplates { get; set; }

        /// <summary>
        /// If applicable then use this group Is
        /// </summary>
        public string GroupId { get; set; }


        /// <summary>
        /// Specifies the context of the TOC the item is being opened in
        /// </summary>
        public string Toc { get; set; }

        public string Path
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Toc))
                {
                    var subcontainer = Content.SubContainerIds.FirstOrDefault(s => s.Toc == Toc);
                    if (subcontainer != null)
                    {
                        var path = subcontainer.Value;
                        return path + "/" + this.Content.Id;
                    }
                }
                return this.Content.Id;
            }
        }

        /// <summary>
        /// Used in xbook to determine of the content being displayed is related content or content from the TOC
        /// </summary>
        public bool IsRelatedContent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignedItem"/> class.
        /// </summary>
        public ContentView()
        {
            RelatedTemplates = new List<RelatedTemplate>();
        }

    }
}
