using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class GradeBookWeightCategory
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// The number of lowest scores to drop from this category.
        /// </summary>
        public string DropLowest { get; set; }

        /// <summary>
        /// Sequence of the category in the list of categories
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public string Weight { get; set; }

        /// <summary>
        /// Gets or sets the item weight total.
        /// </summary>
        /// <value>
        /// The item weight total.
        /// </value>
        public string ItemWeightTotal { get; set; }

        /// <summary>
        /// Gets or sets the percent.
        /// </summary>
        /// <value>
        /// The percent.
        /// </value>
        public string Percent { get; set; }

        /// <summary>
        /// Gets or sets the percent with extra credit.
        /// </summary>
        /// <value>
        /// The percent with extra credit.
        /// </value>
        public string PercentWithExtraCredit { get; set; }

        /// <summary>
        /// Items associated with the category
        /// </summary>
        public List<ContentItem> Items { get; set; }
    }
}
