using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents all settings pertintent to Notes and Highlights for a user
    /// </summary>
    public class NoteSettings
    {
        /// <summary>
        /// Set of notes that have been shared with this user
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public IEnumerable<DataShare> Notes { get; set; }

        /// <summary>
        /// Set of highlights that have been shared with this user
        /// </summary>
        /// <value>
        /// The highlights.
        /// </value>
        public IEnumerable<DataShare> Highlights { get; set; }
    }
}
