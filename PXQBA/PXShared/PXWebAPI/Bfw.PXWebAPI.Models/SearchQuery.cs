using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models
{

	public class SearchQuery
	{
		/// <summary>
		/// Gets or sets the exact phrase.
		/// </summary>
		/// <value>
		/// The exact phrase.
		/// </value>
		public string ExactPhrase { get; set; }

		/// <summary>
		/// Gets or sets the include words.
		/// </summary>
		/// <value>
		/// The include words.
		/// </value>
		public string IncludeWords { get; set; }

		/// <summary>
		/// Gets or sets the exclude words.
		/// </summary>
		/// <value>
		/// The exclude words.
		/// </value>
		public string ExcludeWords { get; set; }

		/// <summary>
		/// Gets or sets the content types.
		/// </summary>
		/// <value>
		/// The content types.
		/// </value>
		public string ContentTypes { get; set; }

		/// <summary>
		/// Gets or sets the include fields.
		/// </summary>
		/// <value>
		/// The include fields.
		/// </value>
		public string IncludeFields { get; set; }

		/// <summary>
		/// Gets or sets the meta include fields (used for facet searching, etc)
		/// </summary>
		/// <value>
		/// The include fields.
		/// </value>
		public string MetaIncludeFields { get; set; }

		/// <summary>
		/// Gets or sets the meta categories.
		/// </summary>
		/// <value>
		/// The meta categories.
		/// </value>
		public string MetaCategories { get; set; }

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>
		/// The start.
		/// </value>
		public int Start { get; set; }

		/// <summary>
		/// Gets or sets the rows.
		/// </summary>
		/// <value>
		/// The rows.
		/// </value>
		public int Rows { get; set; }

		/// <summary>
		/// Gets or sets the selected category.
		/// </summary>
		/// <value>
		/// The selected category.
		/// </value>
		public string SelectedCategory { get; set; }

		/// <summary>
		/// Gets or sets the course categories.
		/// </summary>
		/// <value>
		/// The course categories.
		/// </value>
		public IEnumerable<SearchCategory> CourseCategories { get; set; }

		/// <summary>
		/// Gets or sets the type of the class.
		/// </summary>
		/// <value>
		/// The type of the class.
		/// </value>
		public string ClassType { get; set; }

		/// <summary>
		/// Gets or sets the exact query for metadata search.
		/// </summary>
		/// <value>
		/// The exact query.
		/// </value>
		public string ExactQuery { get; set; }


		/// <summary>
		/// Enable Faceted search
		/// </summary>
		[DataMember]
		public bool IsFaceted { get; set; }

		/// <summary>
		/// Parameters for Faceted Search
		/// </summary>
		public FacetedSearchQuery FacetedQuery { get; set; }

		/// <summary>
		/// Entity id - overrides current context entity id (used for searching against product course)
		/// </summary>
		public string EntityId { get; set; }
	}


	/// <summary>
	/// FacetedSearchQuery
	/// </summary>
	public class FacetedSearchQuery
	{
		/// <summary>
		/// List of metadata fields to search
		/// </summary>
		public List<string> Fields { get; set; }

		/// <summary>
		/// Limits number of results per field
		/// Default: no limit
		/// </summary>
		public int Limit { get; set; }

		/// <summary>
		/// Indicates the minimum count for facet fields to be included in the response.
		/// Default: 0
		/// </summary>
		public int MinCount { get; set; }

		/// <summary>
		/// FacetedSearchQuery
		/// </summary>
		public FacetedSearchQuery()
		{
			this.Fields = new List<string>();
		}
	}
}
