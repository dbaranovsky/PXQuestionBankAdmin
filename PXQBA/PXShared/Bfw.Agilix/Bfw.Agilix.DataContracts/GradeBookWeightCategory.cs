using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Weighted grading category.
    /// </summary>
    public class GradeBookWeightCategory
    {
        /// <summary>
        /// Unique id of the weigthed category.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Text to display for the category.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Relative weighted of the category.
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// The number of lowest scores to drop from this category.
        /// </summary>
        public string DropLowest { get; set; }

        /// <summary>
        /// Sequence of the category in the list of categories
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Total category weight.
        /// </summary>
        public string ItemWeightTotal { get; set; }

        /// <summary>
        /// Percentage form of the category weight.
        /// </summary>
        public string Percent { get; set; }

        /// <summary>
        /// Percentage form of the category weight, including exra credit.
        /// </summary>
        public string PercentWithExtraCredit { get; set; }

        /// <summary>
        /// Items associated with the category
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
