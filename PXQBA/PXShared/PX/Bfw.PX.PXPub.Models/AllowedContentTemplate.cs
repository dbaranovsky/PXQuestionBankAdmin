using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Allowed content items which can be created from the Assign tab
    /// </summary>
    public class AllowedContentTemplate
    {
        /// <summary>
        /// ItemID of the allowed content template
        /// </summary>
        public string TemplateID { get; set; }

        /// <summary>
        /// name of the template
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Display name of the content template that can be used on the create button in the Assign Tab
        /// </summary>
        public string DisplayName { get; set; }
    }
}
