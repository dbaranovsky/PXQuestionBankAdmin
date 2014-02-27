using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class TemplateContext
    {
        /// <summary>
        /// Used to differentiate the different contexts in which we might be looking for templates
        /// </summary>
        public enum TemplateDisplayContext
        {
            Content,
            Assignments,
            Eportfolio,
            Default,
            FacePlate,
            Xbook
        }    
    }

    /// <summary>
    /// 
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the template parent id.
        /// </summary>
        /// <value>
        /// The template parent id.
        /// </value>
        public string TemplateParentId { get; set; }

        /// <summary>
        /// Gets or sets the template's policies.
        /// </summary>
        /// <value>
        /// The template's policies.
        /// </value>
        public IEnumerable<string> Policies { get; set; }
    }
}
