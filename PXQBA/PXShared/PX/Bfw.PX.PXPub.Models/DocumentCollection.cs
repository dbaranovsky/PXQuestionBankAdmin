using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// A ContentItem that acts as a container for a set of documents that have been uploaded.
    /// </summary>
    public class DocumentCollection : ContentItem
    {
        /// <summary>
        /// List of documents in the collection
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        public IList<Document> Documents
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes an empty description and documents list
        /// </summary>
        public DocumentCollection()
        {
            Type = "DocumentCollection";
            Documents = new List<Document>();
        }

        /// <summary>
        /// Gets or sets the type of the document collection sub.
        /// </summary>
        /// <value>
        /// The type of the document collection sub.
        /// </value>
        public string DocumentCollectionSubType { get; set; }
    }
}
