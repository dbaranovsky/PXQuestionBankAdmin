using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class LessonBase : ContentItem
    {
        /// <summary>
        /// Gets or sets the thumbnail.
        /// </summary>
        /// <value>
        /// The thumbnail.
        /// </value>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the uploaded.
        /// </summary>
        /// <value>
        /// The uploaded.
        /// </value>
        public DateTime Uploaded { get; set; }

        /// <summary>
        /// Gets or sets the associated to course.
        /// </summary>
        /// <value>
        /// The associated to course.
        /// </value>
        public string AssociatedToCourse { get; set; }

        /// <summary>
        /// Gets or sets the is hidden.
        /// </summary>
        /// <value>
        /// The is hidden.
        /// </value>
        public string IsHidden { get; set; }

        public List<ContentItem> GetAssociatedItems(string category, bool orderByDate = false)
        {
            //u.DueDate == DateTime.MinValue ? DateTime.MaxValue.Ticks : u.DueDate.Ticks
            var categoryItems = new List<ContentItem>();
            if (!string.IsNullOrEmpty(category))
            {
                categoryItems = AssociatedTocItems.Where(a => a.Categories.FirstOrDefault(c => c.Id == category) != null).ToList();
            }
            else
            {
                categoryItems = AssociatedTocItems;
            }

            if (orderByDate)
            {
                Comparison<ContentItem> comparer = (a, b) =>
                {
                    DateTime aDate, bDate;
                    if (a.Type.ToLowerInvariant() == "pxunit")
                    {
                        aDate = a.StartDate;
                    }
                    else
                    {
                        aDate = a.DueDate;
                    }
                    if (b.Type.ToLowerInvariant() == "pxunit")
                    {
                        bDate = b.StartDate;
                    }
                    else
                    {
                        bDate = b.DueDate;
                    }
                    if (aDate.Equals(bDate) || (aDate.Year == DateTime.MinValue.Year && bDate.Year == DateTime.MinValue.Year))
                    {
                        if (a.Sequence != b.Sequence)
                        {//use sequence (it's possible for sequences to be the same)
                            return System.String.CompareOrdinal(a.Sequence, b.Sequence);
                        }
                        else
                        {
                            //use title as last resort
                            return System.String.CompareOrdinal(a.Title.ToLowerInvariant(), b.Title.ToLowerInvariant());
                        }
                    }
                    else
                    {
                        if (aDate.Year == DateTime.MinValue.Year)
                            return 1;
                        else if (bDate.Year == DateTime.MinValue.Year)
                            return -1;
                        else return DateTime.Compare(aDate, bDate);
                    }
                };
                categoryItems.Sort(comparer);

            }
            else
            {
                categoryItems = categoryItems.OrderBy(i => i.Sequence).ToList();
            }
            return categoryItems;
        }

        /// <summary>
        /// Items Associated to this Lesson.
        /// </summary>
        public List<ContentItem> AssociatedTocItems { get; set; }

        /// <summary>
        /// Toc Item items
        /// </summary>
        /// <value>
        /// The toc item.
        /// </value>
        public IEnumerable<TocItem> TocItem { get; set; }

        /// <summary>
        /// Gets or sets the attached to date category.
        /// </summary>
        /// <value>
        /// The attached to date category.
        /// </value>
        public string AttachedToDateCategory { get; set; }

        /// <summary>
        /// Gets or sets the _assigned item.
        /// </summary>
        /// <value>
        /// The _assigned item.
        /// </value>
        public AssignedItem AssignedItem { get; set; }

        /// <summary>
        /// Key-value pairs for each more resource tags in units
        /// Supports: meta-content-type, meta-topic, meta-subtopic
        /// </summary>
        public Dictionary<string, string> MoreResourcesTags { get; set; }

        /// <summary>
        /// Identifies the chapter of this unit
        /// </summary>
        public string UnitChapter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LessonBase"/> class.
        /// </summary>
        public LessonBase()
        {
            Type = "Folder";
            AssociatedTocItems = new List<ContentItem>();
            MoreResourcesTags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public bool RemoveItem(string itemId)
        {
            return (AssociatedTocItems.RemoveAll(i => i.Id == itemId) > 0);
        }

        /// <summary>
        /// Gets the associated items.
        /// </summary>
        /// <returns></returns>
        public List<ContentItem> GetAssociatedItems(bool orderByDate = false)
        {
            return GetAssociatedItems("", orderByDate);
        }

        /// <summary>
        /// Gets the blurb.
        /// </summary>
        /// <returns></returns>
        public string GetBlurb()
        {
            string html = "";
            html += Title;
            html += "Unit: " + Title;
            html += "<br/>";
            html += "<br/>";
            html += "Due Date: " + DueDate;
            html += "<br/>";
            html += "Status: ";
            html += "Scored Attempt:";
            return html;
        }

         /// <summary>
        /// Adds the associated item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool AddAssociatedItem(List<ContentItem> items)
        {
            foreach (ContentItem item in items)
            {
                AddAssociatedItem(item);
            }

            return true;
        }

        /// <summary>
        /// Adds the associated item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool AddAssociatedItem(ContentItem item)
        {
            if (!AssociatedTocItems.Exists(i => i.Id == item.Id))
            {
                AssociatedTocItems.Add(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the associated item.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public void AddPxUnit(List<PxUnit> items)
        {
            foreach (ContentItem item in items)
            {
                AddAssociatedItem(item);
            }
        }

        /// <summary>
        /// Adds the associated item.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public void AddPxUnit(PxUnit pxUnit)
        {
            AddAssociatedItem(pxUnit);
        }
    }
}
