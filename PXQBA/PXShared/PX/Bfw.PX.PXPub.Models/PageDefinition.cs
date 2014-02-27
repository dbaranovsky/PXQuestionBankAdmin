using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class PageDefinition
    {
        /// <summary>
        /// Name of the Page
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of custom div's that will be created inside the page definition
        /// </summary>
        public List<string> CustomDivs { get; set; }

        /// <summary>
        /// List of zone items for the page
        /// </summary>
        public List<Zone> Zones { get; set; }

        public PageDefinition()
        {
            this.Zones = new List<Zone>();
        }
    }
}
