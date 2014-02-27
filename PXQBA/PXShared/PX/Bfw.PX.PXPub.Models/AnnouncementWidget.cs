using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data necessary to render the AnnouncementWidget
    /// </summary>
    public class AnnouncementWidget
    {
        /// <summary>
        /// List of Announcements to render
        /// </summary>
        /// <value>
        /// The announcements.
        /// </value>
        public List<Announcement> Announcements { get; set; }

        /// <summary>
        /// Number of announcements past the display date
        /// </summary>
        public int ArchivedCount { get; set; }

        public bool IsInstructor { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnnouncementWidget()
        {
            Announcements = new List<Announcement>();
        }

        

    }
}
