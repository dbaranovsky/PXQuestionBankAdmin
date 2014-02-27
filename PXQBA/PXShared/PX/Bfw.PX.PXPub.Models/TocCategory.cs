using System.Runtime.Serialization;

namespace Bfw.PX.PXPub.Models
{

	[DataContract]
	[KnownType(typeof(TocCategory))]
	public class TocCategory
	{
		/// <summary>
		/// Unique identifier for the category
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		[DataMember]
		public string Id { get; set; }

		/// <summary>
		/// Human friendly text that represents the category
		/// </summary>
		/// <value>
		/// The text.
		/// </value>
		[DataMember]
		public string Text { get; set; }

		/// <summary>
		/// This is the item id of the parent of the item this object is attached to
		/// </summary>
		/// <value>
		/// The item parent id.
		/// </value>
		[DataMember]
		public string ItemParentId { get; set; }

		/// <summary>
		/// True if this is the currently active category, false otherwise
		/// </summary>
		/// <value>
		///   <c>true</c> if active; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool Active { get; set; }

		/// <summary>
		/// Relative sequence of the item when viewed in this category
		/// </summary>
		/// <value>
		/// The sequence.
		/// </value>
		[DataMember]
		public string Sequence { get; set; }
		/// <summary>
		/// Type of catagory
		/// </summary>
		[DataMember]
		public string Type { get; set; }
	}
}
