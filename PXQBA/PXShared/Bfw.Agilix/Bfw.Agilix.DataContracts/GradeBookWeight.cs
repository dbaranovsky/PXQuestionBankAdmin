using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Bfw.Common;

using Bfw.Agilix.Dlap.Session;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a Grade business object (see http://dev.dlap.bfwpub.com/Docs/Schema/Grades)
    /// </summary>
    public class GradeBookWeights : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// The item (e.g. an assignment) to which this grade applies.
        /// </summary>
        public Item Item { get; protected set; }

        /// <summary>
        /// True if categories are weighted, false otherwise.
        /// </summary>
        [DataMember]
        public bool WeightedCategories { get; protected set; }

   
        /// <summary>
        /// Total grade with category weights applied.
        /// </summary>
        [DataMember]
        public Double CategoryWeightTotal { get; protected set; }

        /// <summary>
        /// Total grade.
        /// </summary>
        [DataMember]
        public Double Total { get; protected set; }

     
        /// <summary>
        /// Total grade including extra credit.
        /// </summary>
        [DataMember]
        public Double TotalWithExtraCredit { get; protected set; }

        /// <summary>
        /// Categories for grade weights.
        /// </summary>
        public List<GradeBookWeightCategory> GradeWeightCategories { get; protected set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            if (null != element)
            {
                // Build the Item object
                Item = new Item();
                Item.Data = new XElement(ElStrings.Data);

                var itemId = element.Attribute(ElStrings.ItemId);
                var itemTitle = element.Attribute(ElStrings.title);

                if (null != itemId)
                {
                    Item.Id = itemId.Value;
                }
                if (null != itemTitle)
                {
                    Item.Title = itemTitle.Value;
                }
                

                // Populate the rest of the Grade object

                WeightedCategories = bool.Parse ( element.AttributeValue("weightedcategories","false") );
                double categoryWeightTotal = 100;
                double.TryParse(element.AttributeValue("categoryweighttotal","100"), out categoryWeightTotal);
                CategoryWeightTotal = categoryWeightTotal;
                Total = Double.Parse(element.AttributeValue("total","100"));
                TotalWithExtraCredit = Double.Parse (element.AttributeValue("totalwithextracredit","100"));

                if (element.Attribute(ElStrings.CategoryWeightTotal) != null)
                {
                    CategoryWeightTotal = Double.Parse(element.Attribute(ElStrings.CategoryWeightTotal).Value);
                }

                Total = Double.Parse ( element.Attribute(ElStrings.Total).Value );
                TotalWithExtraCredit = Double.Parse (element.Attribute(ElStrings.TotalWithExtraCredit).Value);


                GradeWeightCategories = new List<GradeBookWeightCategory>();
                var categories = element.Elements(ElStrings.category);
                if (categories.Count() > 0 )
                {
                    foreach (var category in categories)
                    {
                        var name = category.Attribute(ElStrings.Name).Value.ToString();
                        var id = category.Attribute(ElStrings.Id).Value.ToString();
                        var percent = category.Attribute(ElStrings.Percent).Value.ToString();
                        var itemWeightTotal = category.Attribute(ElStrings.ItemWeightTotal).Value.ToString();
                        var weight = category.Attribute(ElStrings.weight).Value.ToString();

                        var droplowest = "0";
                        if (category.Attribute(ElStrings.droplowest) != null)
                        {
                            droplowest = category.Attribute(ElStrings.droplowest).Value.ToString();
                        }

                        var sequence = "a";
                        if (category.Attribute(ElStrings.sequence) != null)
                        {
                            sequence = category.Attribute(ElStrings.sequence).Value.ToString();
                        }

                        List<Item> items = new List<Item>();
                        var nodes = category.Elements(ElStrings.Item);

                        foreach (var node in nodes)
                        {
                            items.Add(new Item()
                                {
                                    Id = node.Attribute(ElStrings.Id).Value.ToString(),
                                    Title = node.Attribute(ElStrings.title).Value.ToString(),
                                    Weight = Double.Parse(node.Attribute(ElStrings.weight).Value.ToString()),
                                    Percent = Double.Parse(node.Attribute(ElStrings.Percent).Value.ToString())
                                });
                        }

                        GradeWeightCategories.Add(new GradeBookWeightCategory() { Text = name, Id = id, ItemWeightTotal = itemWeightTotal, Weight = weight, DropLowest = droplowest, Sequence = sequence, Percent = percent, Items = items });
                    }
                }
            }
        }

        #endregion
    }
}
