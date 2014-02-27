using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents a category for a gradebook weight (see http://gls.agilix.com/Docs/Command/GetGradebookWeights).
    /// </summary>
    public class GradeBookWeightCategory
    {
        /// <summary>
        /// The grading category ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The end-user assigned item weight.
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Contains the sum of all item weight values within this category.
        /// </summary>
        public string ItemWeightTotal { get; set; }

        /// <summary>
        /// A number between 0 and 1 that is this item's calculated percentage of the gradebook total. For extra credit items, this number is 0.
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// A number between 0 and 1 that is this item's calculated percentage of the totalwithextracredit.
        /// </summary>
        public string PercentWithExtraCredit { get; set; }
    }
}
