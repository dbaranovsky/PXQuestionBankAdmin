using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// An Announcement made on the current entity
    /// </summary>
    public class Announcement
    {
        /// <summary>
        /// Path (unique ID) of the Announcement.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Title of the Announcement.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Html representing the Announcment's content.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// A date to display about the announcement.
        /// </summary>
        /// <value>
        /// The display date.
        /// </value>
        public DateTime DisplayDate { get; set; }

        /// <summary>
        /// Date after which the announcement should display
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date before which the announcment should display
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets the CSS safe path.
        /// </summary>
        /// <returns></returns>
        public string GetCssSafePath()
        {
            return Path.Replace('/', '_').Replace('.', '-');
        }

        /// <summary>
        /// Indicates whether an announcement is archived
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// sequence id of the announcement
        /// </summary>
        public string PrimarySortOrder { get; set; }
        
        /// <summary>
        /// Pin position of an announcement
        /// </summary>
        public string PinSortOrder { get; set; }
    }
}
