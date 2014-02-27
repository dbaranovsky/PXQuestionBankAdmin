using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Bfw.PXWebAPI.Models
{
	/// <summary>
	/// Search Results
	/// </summary>
	public class SearchResults
	{
		/// <summary>
		/// Gets or sets the max score.
		/// </summary>
		/// <value>
		/// The max score.
		/// </value>
		public string maxScore { get; set; }

		/// <summary>
		/// Gets or sets the num found.
		/// </summary>
		/// <value>
		/// The num found.
		/// </value>
		public string numFound { get; set; }

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>
		/// The start.
		/// </value>
		public string start { get; set; }

		/// <summary>
		/// Gets or sets the time.
		/// </summary>
		/// <value>
		/// The time.
		/// </value>
		public string time { get; set; }

		/// <summary>
		/// Gets or sets the doc_class.
		/// </summary>
		/// <value>
		/// The doc_class.
		/// </value>
		public string doc_class { get; set; }

		/// <summary>
		/// Gets or sets the entityid.
		/// </summary>
		/// <value>
		/// The entityid.
		/// </value>
		public string entityid { get; set; }

		/// <summary>
		/// Gets or sets the itemid.
		/// </summary>
		/// <value>
		/// The itemid.
		/// </value>
		public string itemid { get; set; }

		/// <summary>
		/// Gets or sets the meta value.
		/// </summary>
		/// <value>
		/// The meta value.
		/// </value>
		public string metaValue { get; set; }

		/// <summary>
		/// Gets or sets the docs.
		/// </summary>
		/// <value>
		/// The docs.
		/// </value>
		public List<SearchResultDoc> docs { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResults"/> class.
		/// </summary>
		public SearchResults()
		{
			docs = new List<SearchResultDoc>();
		}

		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>
		/// The query.
		/// </value>
		public SearchQuery Query { get; set; }

		/// <summary>
		/// Exts the node.
		/// </summary>
		/// <param name="ex">The ex.</param>
		/// <param name="nName">Name of the n.</param>
		/// <param name="aName">A name.</param>
		/// <param name="aValue">A value.</param>
		/// <returns></returns>
		public string ExtNode(XElement ex, string nName, string aName, string aValue)
		{
			string strRetVal = "";
			try
			{
				strRetVal = ( ex.Elements().Where(a => a.Attribute(aName).Value == aValue)
				  .Select(a => a.Value) ).FirstOrDefault();
			}
			catch { }
			return strRetVal;
		}

		/// <summary>
		/// Convert the XML to and Entity
		/// </summary>
		/// <param name="element">The element.</param>
		public void ParseEntity(XElement element)
		{
			if (null != element)
			{
				maxScore = element.Attribute("maxScore").Value;
				numFound = element.Attribute("numFound").Value;
				start = element.Attribute("start").Value;
				time = element.Attribute("time").Value;

				// Set the results
				docs = new List<SearchResultDoc>();
				var resultDocs = element.Elements("doc");

				foreach (XElement resultDoc in resultDocs)
				{
					SearchResultDoc e = new SearchResultDoc();
					e.doc_class = ( resultDoc.Attribute("class") != null ) ? resultDoc.Attribute("class").Value : "";
					e.entityid = ( resultDoc.Attribute("entityid") != null ) ? resultDoc.Attribute("entityid").Value : "";
					e.itemid = ( resultDoc.Attribute("itemid") != null ) ? resultDoc.Attribute("itemid").Value : "";
					e.score = ExtNode(resultDoc, "float", "name", "score");
					e.dlap_class = ExtNode(resultDoc, "str", "name", "dlap_class");
					e.dlap_hiddenfromstudent = ExtNode(resultDoc, "str", "name", "dlap_hiddenfromstudent");
					e.dlap_id = ExtNode(resultDoc, "str", "name", "dlap_id");
					e.dlap_itemtype = ExtNode(resultDoc, "int", "name", "dlap_itemtype");
					e.dlap_contenttype = ExtNode(resultDoc, "str", "name", "dlap_contenttype");
					e.dlap_subtitle = ExtNode(resultDoc, "str", "name", "dlap_subtitle");
					e.dlap_title = ExtNode(resultDoc, "str", "name", "dlap_title");

					docs.Add(e);
				}
			}
		}
	}
}