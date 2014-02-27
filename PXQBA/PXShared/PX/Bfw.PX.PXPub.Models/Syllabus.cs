using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Determines how the Syllabus tool is rendered
    /// </summary>
    public class Syllabus
    {
        /// <summary>
        /// True if the link to download the file should be rendered
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow download]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDownload { get; set; }

        /// <summary>
        /// True if the UI for uploading a syllabus is to be displayed
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow upload]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowUpload { get; set; }

        /// <summary>
        /// Id of the Syllabus, or empty if one doesn't exist
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }
    }
}
