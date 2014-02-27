using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents any type of content in the system
    /// </summary>
    public class ContentIndex
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exact phrase.
        /// </summary>
        /// <value>
        /// The exact phrase.
        /// </value>
        public string ExactPhrase
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the include words.
        /// </summary>
        /// <value>
        /// The include words.
        /// </value>
        public string IncludeWords
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exclude words.
        /// </summary>
        /// <value>
        /// The exclude words.
        /// </value>
        public string ExcludeWords
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets from search.
        /// </summary>
        /// <value>
        /// From search.
        /// </value>
        public string fromSearch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content types.
        /// </summary>
        /// <value>
        /// The content types.
        /// </value>
        public string ContentTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the metacategories.
        /// </summary>
        /// <value>
        /// The metacategories.
        /// </value>
        public string Metacategories
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentIndex"/> class.
        /// </summary>
        public ContentIndex()
        {
            
        }
    }
}
