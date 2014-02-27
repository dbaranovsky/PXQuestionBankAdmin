using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
	/// <summary>
	/// Defines a TocCategory that is available.
	/// </summary>
    [System.Serializable]
	[KnownType(typeof(Bfw.PX.Biz.DataContracts.TocCategory))]
	[DataContract]
	public class TocCategory
	{
		/// <summary>
		/// Unique identifier for the category.
		/// </summary>
		[DataMember]
		public string Id { get; set; }

		/// <summary>
		/// Human friendly text that represents the category.
		/// </summary>
		[DataMember]
		public string Text { get; set; }

		/// <summary>
		/// This is the item ID of the parent of the item this object is attached to.
		/// </summary>
		[DataMember]
		public string ItemParentId { get; set; }

		/// <summary>
		/// Relative sequence of the item when viewed in this category.
		/// </summary>
		[DataMember]
		public string Sequence { get; set; }

		/// <summary>
		/// Type of category
		/// </summary>
		[DataMember]
		public string Type { get; set; }
	}
}