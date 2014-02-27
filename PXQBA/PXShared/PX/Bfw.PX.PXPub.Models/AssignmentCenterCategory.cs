using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a category in the assignment center along with 
    /// all of its content.
    /// </summary>
    /// 
    [DataContract]
    public class AssignmentCenterCategory
    {
        /// <summary>
        /// The Id of the category.
        /// </summary>
        /// 
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Title of the category
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Start date of the category, if assigned.
        /// </summary>
        /// 
        [DataMember(Name = "startDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the category, if assigned.
        /// </summary>
        /// 
        [DataMember(Name = "endDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Root level nodes in the tree.
        /// </summary>
        /// 
        [DataMember(Name = "items")]
        public List<AssignmentCenterItem> Items { get; set; }
    }
}
