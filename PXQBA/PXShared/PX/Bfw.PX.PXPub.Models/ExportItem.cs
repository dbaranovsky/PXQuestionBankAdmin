using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// eportofolio item that needs to be exported
    /// </summary>
    public class ExportItem
    {
        /// <summary>
        /// Id of the content item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ParentId of the selected item
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The type of content the object represents
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Title of the content item
        /// </summary>
        ///                
        public string Title { get; set; }

        /// <summary>
        /// The sequence key
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }

        /// <summary>
        /// whether an item is selected
        /// </summary>
        public bool IsSelected { get; set; }

    }
}
