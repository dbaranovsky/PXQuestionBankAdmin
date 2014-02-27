using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a GradeBookWeights business object. (See http://gls.agilix.com/Docs/Command/GetGradebookWeights)
    /// </summary>
    public class GradeBookWeights 
    {
        /// <summary>
        /// Indicates whether the entity's grading categories are weighted or not.
        /// </summary>
        public bool WeightedCategories { get; set; }
   
        /// <summary>
        /// Present when weightedcategories is true. Contains the sum of all category weight values.
        /// </summary>
        public Double CategoryWeightTotal { get; set; }

        /// <summary>
        /// The sum of all the category percent's excluding extra credit items and categories. If there is at least one non-extra credit category or item, this equals 1.0.
        /// </summary>
        public Double Total { get; set; }
     
        /// <summary>
        /// The sum of all the category percent's including extra credit items and categories. This can equal more than 1.0.
        /// </summary>
        public Double TotalWithExtraCredit { get; set; }

        /// <summary>
        /// Gets or sets a collection of gradebook weight categories.
        /// </summary>
        public List<GradeBookWeightCategory> GradeWeightCategories { get; set; }
    }
}