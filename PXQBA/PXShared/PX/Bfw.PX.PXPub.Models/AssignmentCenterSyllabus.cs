using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;
using Bfw.Common.Collections;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public enum eSyllabus
    {
        Weeks,
        Modules,
        Topics
    }

    public class AssignmentCenterFilterSection : ContentItem
    {
        /// <summary>
        /// Gets or sets the filter titles.
        /// </summary>
        /// <value>
        /// The filter titles.
        /// </value>
        public string FilterTitles { get; set; }

        /// <summary>
        /// Gets or sets the type of the syllabus.
        /// </summary>
        /// <value>
        /// The type of the syllabus.
        /// </value>
        public string SyllabusType { get; set; }

        /// <summary>
        /// Gets or sets the topics count.
        /// </summary>
        /// <value>
        /// The topics count.
        /// </value>
        public int TopicsCount { get; set; }        

        /// <summary>
        /// Gets or sets a value indicating whether this instance is child filter section.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is child filter section; otherwise, <c>false</c>.
        /// </value>
        public bool IsChildFilterSection { get; set; }

        /// <summary>
        /// Gets or sets the children filter sections.
        /// </summary>
        /// <value>
        /// The children filter sections.
        /// </value>
        public List<AssignmentCenterFilterSection> ChildrenFilterSections { get; set; }

        /// <summary>
        /// A private variable to hold the children Content Items.
        /// </summary>
        private List<ContentItem> _childItems = new List<ContentItem>();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is root.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is root; otherwise, <c>false</c>.
        /// </value>
        public bool IsRoot { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentCenterFilterSection"/> class.
        /// </summary>
        public AssignmentCenterFilterSection()
        {
            Type = "Folder";
            ChildrenFilterSections = new List<AssignmentCenterFilterSection>();
            StartDate = DateTime.MinValue;
            DueDate = DateTime.MinValue;
        }

        /// <summary>
        /// Adds the child items.
        /// </summary>
        /// <param name="contentItems">The content items.</param>
        public void AddChildItems(List<ContentItem> contentItems)
        {
            foreach (var item in contentItems)
            {
                AddChildItems(item);
            }
        }

        /// <summary>
        /// Adds the child items.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        public void AddChildItems(ContentItem contentItem)
        {
            if (!_childItems.Exists(i => i.Id == contentItem.Id))
            {
                _childItems.Add(contentItem);
            }
        }

        /// <summary>
        /// Removes the child item.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        public void RemoveChildItem(string contentId)
        {
            _childItems.RemoveAll(i => i.Id == contentId);
        }

        /// <summary>
        /// Gets the child items.
        /// </summary>
        /// <returns></returns>
        public List<ContentItem> GetChildItems()
        {
            return _childItems.OrderBy(i => i.Sequence).ToList();
        }

        /// <summary>
        /// Sub items count.
        /// </summary>
        /// <returns></returns>
        public List<ContentItem> GetChildItemsFullTree()
        {
            var items = new List<ContentItem>();

            foreach (var childFilter in ChildrenFilterSections)
            {
                items.AddRange(childFilter.GetChildItemsFullTree());
            }

            items.AddRange (_childItems);
            return items;
        }
        /// <summary>
        /// Sub items count.
        /// </summary>
        /// <returns></returns>
        public int ItemCount()
        {
            var count = _childItems.Count;

            foreach (var childFilter in ChildrenFilterSections)
            {
                count += childFilter.ItemCount();
            }

            return count;
        }

        /// <summary>
        /// Gets or sets the toc id.
        /// </summary>
        /// <value>
        /// The toc id.
        /// </value>
        public string TocId { get; set; }
    }
}
