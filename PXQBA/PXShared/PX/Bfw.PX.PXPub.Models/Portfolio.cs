using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class Portfolio : ContentItem
    {
        public Portfolio() { Type = "ePortfolio";}

        public string Instruction { get; set; }

        /// <summary>
        /// Rubrics for the item
        /// </summary>
        public List<Rubric> Rubrics { get; set; }

        /// <summary>
        /// Text to describe the policies/settings of the assignment
        /// </summary>
        public IList<string> Policies { get; set; }
    }
}
