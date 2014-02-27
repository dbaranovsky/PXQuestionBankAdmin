using System;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models
{
	/// <summary>
	/// This is light version of PX.Models.ContentItem
	/// </summary>
    [Serializable]
    [DataContract]
	public class ContentItem
	{
		/// <summary>
		/// Item Id
		/// </summary>
        [DataMember]
		public string ItemId { get; set; }

        /// <summary>
        /// Date the item is due.
        /// </summary>
        [DataMember]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Number of points the item is worth.
        /// </summary>
        [DataMember]
        public double MaxPoints { get; set; }

		/// <summary>
		/// Title of the item
		/// </summary>
        [DataMember]
        public string Title { get; set; }

		/// <summary>
		/// Description of the item
		/// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Category the item belongs to.
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Visibility for student.
        /// </summary>
        [DataMember]
        public bool Visibility { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// Thumbnail of the unit/folder
        /// </summary>
        [DataMember]
        public string iconUri { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// Id of the subcontainer
        /// </summary>
        [DataMember]
        public string SubContainerId { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// The subtype of the item (the PX type).
        /// </summary>
        [DataMember]
        public string SubType { get; set; }

		/// <summary>
		/// The unique Id of the course this content belongs to.
		/// </summary>
        [DataMember]
        public string CourseId { get; set; }

        /// <summary>
        /// Returns true if item is removed from PX
        /// </summary>
        [DataMember]
        public bool Removed { get; set; }
	}
}
