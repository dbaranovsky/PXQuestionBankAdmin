using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// An Announcement that was made on a Course or Domain.
    /// </summary>
    [DataContract]
    public class Announcement
    {
        /// <summary>
        /// Title of the Announcement.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Html representing the content of the Announcement.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// Date the Announcement was created.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Path (unique identifier) of the Announcement.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Date after which the announcement should display
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date before which the announcement should display
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Indicates whether the announcement is archived
        /// </summary>
        [DataMember]
        public bool IsArchived { get; set; }

        /// <summary>
        /// sequence id of announcement
        /// </summary>
        [DataMember]
        public string PrimarySortOrder { get; set; }

        /// <summary>
        /// Pin position of an announcement
        /// </summary>
        [DataMember]
        public string PinSortOrder { get; set; }
    }
}