using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Wraps a content item with relevant information regarding its place in a ContentTreeWidget
    /// </summary>
    public class TreeWidgetViewItem
    {
        /// <summary>
        /// The content item
        /// </summary>
        public ContentItem Item { get; set; }

        //CHANGE!
        private string _parentId;
        /// <summary>
        /// ID of immediate parent of the item in the tree
        /// </summary>
        public string ParentId
        {
            get
            {
                if (!string.IsNullOrEmpty(_parentId))
                {
                    return _parentId;
                }

                return Item.GetSyllabusFilterFromCategory(Settings.TOC);
            }
            set
            {
                _parentId = value;
            }
        }

        /// <summary>
        /// The level the content item exists at in the tree
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Get/Set if the item has any children in the tree heirarchy 
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Basically points to the subcontainer for hash functionality
        /// </summary>
        public string Path
        {
            get
            {
                //CHANGE!
                var subcontainer = Item.SubContainerIds.FirstOrDefault(c => c.Toc == Settings.TOC);
                if (subcontainer != null)
                    return subcontainer.Value + "/";

                return string.Empty;
            }
        }

        /// <summary>
        /// Settings associated with the tree widget this item belongs to
        /// </summary>
        public TreeWidgetSettings Settings { get; set; }

        /// <summary>
        /// THIS IS FOR TESTING PURPOSES ONLY.  USE THIS AND DIEEE
        /// </summary>
        public TreeWidgetViewItem()
        {
            Item = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeWidgetViewItem" /> class.
        /// </summary>
        /// <param name="item">The ContentItem itself</param>
        /// <param name="settings">The settings.</param>
        /// <param name="level">Level the contentitem exists at in the content tree</param>
        public TreeWidgetViewItem(ContentItem item, TreeWidgetSettings settings, int level = 0)
        {
            Item = item;

            Level = level;
            Settings = settings;
        }

        /// <summary>
        /// The due date comparer
        /// </summary>
        public static Comparison<TreeWidgetViewItem> DueDateComparer = (a, b) =>
        {
            var aDate = a.Item.Type.ToLowerInvariant() == "pxunit" ? a.Item.StartDate : a.Item.DueDate;

            var bDate = b.Item.Type.ToLowerInvariant() == "pxunit" ? b.Item.StartDate : b.Item.DueDate;

            if (aDate.Equals(bDate) || (aDate.Year == DateTime.MinValue.Year && bDate.Year == DateTime.MinValue.Year))
            {
                if (a.Item.Sequence != b.Item.Sequence)
                {
                    //use sequence (it's possible for sequences to be the same)
                    return String.CompareOrdinal(a.Item.Sequence, b.Item.Sequence);
                }

                //use title as last resort
                return String.CompareOrdinal(a.Item.Title.ToLowerInvariant(), b.Item.Title.ToLowerInvariant());
            }

            if (aDate.Year == DateTime.MinValue.Year)
                return 1;
            if (bDate.Year == DateTime.MinValue.Year)
                return -1;
            return DateTime.Compare(aDate, bDate);
        };
    }
}
