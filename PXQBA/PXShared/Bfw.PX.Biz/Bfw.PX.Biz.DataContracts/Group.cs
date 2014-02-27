using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// /// Represents a Group business object. (See http://gls.agilix.com/Docs/Command/GetGroup)
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The ID of the created group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title for the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of member enrollments in the group.
        /// </summary>
        public IEnumerable<Enrollment> Members { get; set; }

        /// <summary>
        /// ID of the course to which this group belongs.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Optional field reserved for any data to be stored in association with the agilix group entry.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// ID of the domain the group is in.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// The ID of the group set within the owning entity to which this group belongs.
        /// </summary>
        public string SetId { get; set; }
    }
}