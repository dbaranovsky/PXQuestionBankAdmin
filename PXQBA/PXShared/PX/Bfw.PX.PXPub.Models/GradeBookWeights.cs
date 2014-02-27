using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


using System.Runtime.Serialization;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a Grade business object (see http://192.168.78.60/Docs/Schema/Grades)
    /// </summary>
    public class GradeBookWeights
    {
        /// <summary>
        /// WeightedCategories
        /// </summary>
        /// <value>
        ///   <c>true</c> if [weighted categories]; otherwise, <c>false</c>.
        /// </value>
        public bool WeightedCategories { get; set; }

        /// <summary>
        /// CategoryWeightTotal
        /// </summary>
        /// <value>
        /// The category weight total.
        /// </value>
        public Double CategoryWeightTotal { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public Double Total { get; set; }

        /// <summary>
        /// TotalWithExtraCredit
        /// </summary>
        /// <value>
        /// The total with extra credit.
        /// </value>
        public Double TotalWithExtraCredit { get; set; }

        /// <summary>
        /// Gets or sets the grade weight categories.
        /// </summary>
        /// <value>
        /// The grade weight categories.
        /// </value>
        public List<GradeBookWeightCategory> GradeWeightCategories { get; set; }
    }
}
