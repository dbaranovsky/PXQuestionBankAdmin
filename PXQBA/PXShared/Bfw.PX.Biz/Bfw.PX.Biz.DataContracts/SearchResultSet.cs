using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Common.Collections;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a search result set returned after executing a search, along with summary data.
    /// </summary>
    public class SearchResultSet
    {
        /// <summary>
        /// Gets or sets the meta value.
        /// </summary>
        public string metaValue { get; set; }

        /// <summary>
        /// Gets or sets the max score.
        /// </summary>
        /// <value>
        /// The max score.
        /// </value>
        public string maxScore { get; set; }

        /// <summary>
        /// Number of results found.
        /// </summary>
        public string numFound { get; set; }

        /// <summary>
        /// Start time of search execution.
        /// </summary>
        public string start { get; set; }

        /// <summary>
        /// Search completion time.
        /// </summary>
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
        /// List of face fields
        /// </summary>
        public List<FacetField> FacetFields { get; set; }


        /// <summary>
        /// Collection of results returned from search, if exist.
        /// </summary>
        public List<SearchResultDoc> docs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultSet"/> class.
        /// </summary>
        public SearchResultSet()
        {
            docs = new List<SearchResultDoc>();
            FacetFields = new List<FacetField>();
        }

        /// <summary>
        /// Original search query criteria.
        /// </summary>
        public SearchQuery Query { get; set; }

    /// <summary>
    /// LINQ based helper method to search and extract XML node data.
    /// </summary>
    /// <param name="ex">The XML element.</param>
    /// <param name="nName">Name of the XML node.</param>
    /// <param name="aName">Name of the XML attribute.</param>
    /// <param name="aValue">XML node value.</param>
    /// <param name="joinVal">String to use when joining values in an array</param>
    /// <returns></returns>
    public string ExtNode(XElement ex, string nName, string aName, string aValue, string joinVal = "")
        {
            string strRetVal = "";
            try
            {
                var elems = ex.Elements().Where(a => a.Attribute(aName).Value == aValue);
                var childElems = elems.Elements();
                if (childElems.Any())
                {
                    strRetVal += string.Join(joinVal, childElems.Select(a => a.Value));
                }
                else
                {
                    strRetVal = string.Join(joinVal, elems.Select(a => a.Value));
                }
                   
                
            }
            catch { }
            return strRetVal;
        }

        /// <summary>
        /// Parses the raw xml data from agilix and populates the docs collection.
        /// </summary>
        /// <param name="element">The root XML element from the search return.</param>
        public void ParseEntity(XElement element)
        {
            if (null != element)
            {
                if (!string.IsNullOrWhiteSpace(maxScore))
                {
                    maxScore = Math.Max(double.Parse(element.Attribute("maxScore").Value), double.Parse(maxScore)).ToString();
                }
                else
                {
                    maxScore = element.Attribute("maxScore").Value;
                }
                if (!string.IsNullOrWhiteSpace(numFound))
                {
                    numFound = (int.Parse(element.Attribute("numFound").Value) + int.Parse(numFound)).ToString();
                }
                else
                {
                    numFound = element.Attribute("numFound").Value;
                }
                if (!string.IsNullOrWhiteSpace(start))
                {
                    start = (int.Parse(element.Attribute("start").Value) + int.Parse(start)).ToString();
                }
                else
                {
                    start = element.Attribute("start").Value;
                }
                if (!string.IsNullOrWhiteSpace(time))
                {
                    time = (int.Parse(element.Attribute("time").Value) + int.Parse(time)).ToString();
                }
                else
                {
                    time = element.Attribute("time").Value;
                }

                var resultDocs = element.Elements("doc");
                foreach (XElement resultDoc in resultDocs)
                {
                    SearchResultDoc e = new SearchResultDoc();

                    e.doc_class = (resultDoc.Attribute("class") != null) ? resultDoc.Attribute("class").Value : "";
                    e.entityid = (resultDoc.Attribute("entityid") != null) ? resultDoc.Attribute("entityid").Value : "";
                    e.itemid = (resultDoc.Attribute("itemid") != null) ? resultDoc.Attribute("itemid").Value : "";
                    e.score = ExtNode(resultDoc, "float", "name", "score");
                    e.dlap_class = ExtNode(resultDoc, "str", "name", "dlap_class");
                    e.dlap_hiddenfromstudent = ExtNode(resultDoc, "str", "name", "dlap_hiddenfromstudent");
                    e.dlap_id = ExtNode(resultDoc, "str", "name", "dlap_id");
                    e.dlap_itemtype = ExtNode(resultDoc, "int", "name", "dlap_itemtype");
                    e.dlap_contenttype = ExtNode(resultDoc, "str", "name", "dlap_contenttype");
                    e.dlap_subtitle = ExtNode(resultDoc, "str", "name", "dlap_subtitle");
                    e.dlap_title = ExtNode(resultDoc, "str", "name", "dlap_title");
                    e.dlap_html_text = ExtNode(resultDoc, "str", "name", "dlap_html_text", "|");

                    if (e.dlap_class == "question")
                    {
                        e.dlap_q_flags = ExtNode(resultDoc, "str", "name", "dlap_q_flags");
                        var score = 1.0;
                        Double.TryParse(ExtNode(resultDoc, "double", "name", "dlap_q_score"), out score);
                        e.dlap_q_score = score; 
                        e.dlap_q_texttype = ExtNode(resultDoc, "str", "name", "dlap_q_texttype");
                        e.dlap_q_type = ExtNode(resultDoc, "str", "name", "dlap_q_type");
                        e.dlap_objectives = ExtNode(resultDoc, "str", "name", "dlap_objectives");
                        e.dlap_objective_text = ExtNode(resultDoc, "str", "name", "dlap_objective_text");
                    }


                    //parse array elements -  
                    //<arr name="meta-content-type_dlap_e">
                    //  <str>Text Type1</str>
                    //</arr>
                    resultDoc.Elements()
                        .Where(elem => elem.Name.LocalName.Equals("arr")).ToList()
                        .ForEach(elem =>
                                     {
                                         var name = elem.Attribute("name").Value;
                                         if (name != "dlap_html_text" && name != "score") //ignore dlap_html_text and score - dlap_html_text never has meta-data, score is the search score
                                         {
                                             List<string> values = elem.Elements().Select(v => v.Value).ToList();
                                             values.ForEach(v => e.Metadata[name] = v);
                                         }

                                     });

                  
                    docs.Add(e);


                }

               
            }
        }
        /// <summary>
        /// Parses raw xml from agilix and populates facet fields
        /// </summary>
        /// <param name="element"></param>
        public void ParseFacetResults(XElement element)
        {
            if (element == null)
                return;
            var lstFacetFields =
                            element.Elements("lst").Where(e => e.Attribute("name").Value == "facet_fields");
            foreach (var lstField in lstFacetFields.Elements("lst"))
            {
                //<lst name="meta-topic_dlap_e">
                var fieldName = lstField.Attribute("name").Value;

                FacetField field = new FacetField() { FieldName = fieldName };
                this.FacetFields.Add(field);
                foreach (var facetValue in lstField.Elements())
                {
                    //fieldValue (ie: <int name="folder_1" >18</int>)
                    string type = facetValue.Name.LocalName;
                    if (type == "int")
                    {
                        string value = facetValue.Attribute("name").Value;
                        int count = int.Parse(facetValue.Value);
                        field.FieldValues.Add(new FacetValue() {   Value = value, Count = count });
                    }
                }
            }
        }
    }
    /// <summary>
    /// Single field-value and count instance returned by faceted search.
    /// ie. <int name="folder_1" >18</int>
    /// </summary>
    public class FacetValue
    {
        public string Value { get; set; }
        public int Count { get; set; }
    }

    public class FacetField
    {
        /// <summary>
        /// Name of the field being faceted
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Values and Counts for each member of the field
        /// </summary>
        public List<FacetValue> FieldValues { get; set; }
        /// <summary>
        /// Default constuctor
        /// </summary>
        public FacetField()
        {
            FieldValues = new List<FacetValue>();
        }
    }
}
