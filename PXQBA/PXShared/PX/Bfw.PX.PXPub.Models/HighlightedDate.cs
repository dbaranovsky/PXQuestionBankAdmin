using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a calendar date that is highlighted in some way
    /// </summary>
    public class HighlightedDate
    {
        /// <summary>
        /// The date to be highlighted
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// The css class used to highlight the date
        /// </summary>
        public string CssClass;
    }
}
