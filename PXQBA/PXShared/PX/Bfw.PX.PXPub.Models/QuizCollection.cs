using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Representing a list of Browse Types of Quiz
    /// </summary>
    public class QuizCollection
    {

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<TocItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the type of the current collection.
        /// </summary>
        /// <value>
        /// The type of the current collection.
        /// </value>
        public QuizCollectionType CollectionType { get; set; }
        
    }
}
