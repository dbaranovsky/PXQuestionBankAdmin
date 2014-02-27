using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a Discussion Board
    /// </summary>
    public class Discussion : ContentItem
    {
        /// <summary>
        /// List of documents in the collection
        /// </summary>
        /// <value>
        /// The document collection.
        /// </value>
        public DocumentCollection DocumentCollection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Discussion"/> class.
        /// </summary>
        public Discussion()
        {
            Type = "Discussion";
            DocumentCollection = new DocumentCollection();
        }
    }
}
