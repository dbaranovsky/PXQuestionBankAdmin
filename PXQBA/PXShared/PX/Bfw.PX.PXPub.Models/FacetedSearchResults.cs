using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    [Serializable]
    public class FacetedSearchResults : SearchResults
    {
        /// <summary>
        /// Initialized a new instance of FacetedSearchResults
        /// </summary>
        public FacetedSearchResults() : base()
        {
            FacetFields = new List<FacetField>();
        }

        /// <summary>
        /// List of 
        /// </summary>
        public List<FacetField> FacetFields { get; set; }

        /// <summary>
        /// Convert the XML to and Entity
        /// </summary>
        /// <param name="element">The element.</param>
        public new void ParseEntity(XElement element) 
        {
            base.ParseEntity(element);

            var facetCounts = element.Elements("lst").First(n => n.Attribute("name") != null && n.Attribute("name").Value == "facet_counts");
            if (facetCounts != null)
            {
                foreach (var facetField in facetCounts.Elements())
                {
                    if (facetField.Name.LocalName == "lst")
                    {
                        //field name (ie: meta-folder_dlap_e)
                        string name = facetField.Attribute("name").Value;
                        
                        FacetField field = new FacetField(){FieldName = name};
                        FacetFields.Add(field);

                        foreach (var facetValue in facetField.Elements())
                        {
                            //fieldValue (ie: <int name="folder_1" >18</int>)
                            string type = facetValue.Name.LocalName;
                            if(type == "int")
                            {
                                string value = facetValue.Attribute("name").Value;
                                int count = int.Parse(facetValue.Value);
                                field.FieldValues.Add(new FacetValue(){ Value = value, Count = count});
                            }
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// Single field-value and count instance returned by faceted search.
    /// ie. <int name="folder_1" >18</int>
    /// </summary>
    [Serializable]
    public class FacetValue
    {
        public string Value { get; set; } // value of the facet 
        public int Count { get; set; } //count of the face
    }

    /// <summary>
    /// Facet values used in instructor console
    /// Selection indicates facet should be included in console
    /// </summary>
    [Serializable]
    public class FacetValueSetting : FacetValue
    {
        public bool Selected { get; set; }
        public FacetValueSetting(FacetValue val)
        {
            this.Value = val.Value;
            this.Count = val.Count;
            Selected = false;
        }
    }

    [Serializable]
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
