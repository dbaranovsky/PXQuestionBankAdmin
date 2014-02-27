using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

using Bfw.Common;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data required to display a resource
    /// </summary>
    public class ResourceDocument
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required(ErrorMessage="Title Required")]
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string path { get; set; }

        /// <summary>
        /// Gets or sets the contenttype.
        /// </summary>
        /// <value>
        /// The contenttype.
        /// </value>
        public string contenttype { get; set; }

        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string enrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the word count.
        /// </summary>
        /// <value>
        /// The word count.
        /// </value>
        public string wordCount { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string url { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is stand alone.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is stand alone; otherwise, <c>false</c>.
        /// </value>
        public bool isStandAlone { get; set; }

        /// <summary>
        /// Gets or sets the last modified date of this document
        /// </summary>
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the assignment this document associated with
        /// </summary>
        public Assignment AssociatedAssignment { get; set; }

        /// <summary>
        /// Gets or sets this document is private.
        /// </summary>
        public bool IsPrivate { get; set; }

        
    }
}
