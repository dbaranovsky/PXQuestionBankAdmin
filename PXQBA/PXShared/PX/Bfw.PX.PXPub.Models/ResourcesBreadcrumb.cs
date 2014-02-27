using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Data necessary to display a resources breadcrumb trail.
    /// </summary>
    public class BreadcrumbData
    {
        /// <summary>
        /// Title of the item being displayed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Controller to invoke when breadcrumb item is clicked
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Action to invoke when breadcrumb item is clicked
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Rotue values to pass the to the action being invoked
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }
    }
}