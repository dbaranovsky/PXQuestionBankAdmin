using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class MenuContainer
    {
        /// <summary>
        /// Gets or sets the navigation items.
        /// </summary>
        /// <value>
        /// The navigation items.
        /// </value>
        public List<NavigationItem> NavigationItems { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public List<Link> Links { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<ContentItem> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuContainer"/> class.
        /// </summary>
        public MenuContainer()
        {
            NavigationItems = new List<NavigationItem>();
            Links = new List<Link>();
            Id = string.Empty;
            Children = new List<ContentItem>();
        }

        /// <summary>
        /// Finds the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ContentItem FindById(string id)
        {
            var n = NavigationItems.FirstOrDefault(i => i.Id == id);

            if (n != null)
            {
                return (ContentItem)n;
            }

            var l = Links.FirstOrDefault(i => i.Id == id);

            if (l != null)
            {
                return (ContentItem)l;
            }

            return new ContentItem();
        }
    }
}
