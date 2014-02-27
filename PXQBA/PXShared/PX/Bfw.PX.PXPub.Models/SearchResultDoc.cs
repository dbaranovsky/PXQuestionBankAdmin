using System;
using System.Collections.Generic;



namespace Bfw.PX.PXPub.Models
{
	/// <summary>
	/// Used to identified what Dlap primitive the item element represents
	/// </summary>
    [Serializable]
	public enum DlapItemType
	{
		Resource = 0,
		Assignment,
		Assessment,
		Discussion,
		Folder,
		CustomActivity,
		AssetLink,
		RssFeed,
		Survey,
		Shortcut,
		Homework,
		Custom,
		HtmlDocument,
		Comment = 101
	}

    [Serializable]
	public class SearchResultDoc
	{
		/// <summary>
		/// A single result item
		/// </summary>
		/// <value>
		/// The entityid.
		/// </value>

		public string entityid { get; set; }

		/// <summary>
		/// Gets or sets the doc_class.
		/// </summary>
		/// <value>
		/// The doc_class.
		/// </value>
		public string doc_class { get; set; }

		/// <summary>
		/// Gets or sets the itemid.
		/// </summary>
		/// <value>
		/// The itemid.
		/// </value>
		public string itemid { get; set; }

		/// <summary>
		/// Gets or sets the score.
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		public string score { get; set; }

		/// <summary>
		/// Gets or sets the dlap_class.
		/// </summary>
		/// <value>
		/// The dlap_class.
		/// </value>
		public string dlap_class { get; set; }

		/// <summary>
		/// Gets or sets the dlap_hiddenfromstudent.
		/// </summary>
		/// <value>
		/// The dlap_hiddenfromstudent.
		/// </value>
		public string dlap_hiddenfromstudent { get; set; }

		/// <summary>
		/// Gets or sets the dlap_id.
		/// </summary>
		/// <value>
		/// The dlap_id.
		/// </value>
		public string dlap_id { get; set; }

		/// <summary>
		/// Gets or sets the dlap_itemtype.
		/// </summary>
		/// <value>
		/// The dlap_itemtype.
		/// </value>
		public string dlap_itemtype { get; set; }

		/// <summary>
		/// Gets or sets the dlap_contenttype.
		/// </summary>
		/// <value>
		/// The dlap_itemtype.
		/// </value>
		public string dlap_contenttype { get; set; }

		/// <summary>
		/// Gets or sets the dlap_subtitle.
		/// </summary>
		/// <value>
		/// The dlap_subtitle.
		/// </value>
		public string dlap_subtitle { get; set; }

		/// <summary>
		/// Gets or sets the dlap_title.
		/// </summary>
		/// <value>
		/// The dlap_title.
		/// </value>
		public string dlap_title { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the dlap_html_text.
		/// </summary>
		/// <value>
		/// The dlap_html_text.
		/// </value>
		public string dlap_html_text { get; set; }

		/// <summary>
		/// Gets or sets the dlap_text.
		/// </summary>
		/// <value>
		/// The dlap_text.
		/// </value>
		public string dlap_text { get; set; }

		/// <summary>
		/// Gets the CSS class.
		/// </summary>
		public string CssClass
		{
			get
			{
				DlapItemType dlapItmType;

				DlapItemType.TryParse(this.dlap_itemtype, true, out dlapItmType);

				return dlapItmType.ToString();
			}
		}

		/// <summary>
		/// Gets the type of the result.
		/// </summary>
		/// <value>
		/// The type of the result.
		/// </value>
		public string ResultType
		{
			get
			{

				DlapItemType dlapItmType;

				DlapItemType.TryParse(this.dlap_itemtype, true, out dlapItmType);

				switch (dlapItmType)
				{
					case DlapItemType.Assignment:
						return "Assignment";
					case DlapItemType.Comment:
					case DlapItemType.Discussion:
						return "Discussion";
					case DlapItemType.AssetLink:
					case DlapItemType.Resource:
						return "E-book";
					default:
						return "Result";
				}

			}
		}

		/// <summary>
		/// Collection of elements in item that begin with "meta-" tag (and are indexed by solr)
		/// </summary>
		public Dictionary<string, string> Metadata { get; set; }


		/// <summary>
		/// (Faceplate more resources) item is included in current chapter
		/// </summary>
		public bool Included { get; set; }


		/// <summary>
		/// (Faceplate more resources) item is included in current course
		/// </summary>
		public bool InUse { get; set; }


		/// <summary>
		/// (Faceplate more resources) Id of root Unit(chapter) that the item belongs to
		/// </summary>
		public string RootParentId { get; set; }


		/// <summary>
		/// (Faceplate more resources) Name of root Unit(chapter) that the item belongs to
		/// </summary>
		public string RootParentName { get; set; }

		public SearchResultDoc()
		{
			this.Metadata = new Dictionary<string, string>();
		}
	}
}
