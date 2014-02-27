using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a downloadable file
    /// </summary>
    public class Document : ContentItem
    {
        /// <summary>
        /// Size, in bytes, of the document
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long Size
        {
            get;
            set;
        }

        /// <summary>
        /// Original filename of the document, i.e. the name at time of upload
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Mime-Type of the file
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Date the file was uploaded
        /// </summary>
        /// <value>
        /// The uploaded.
        /// </value>
        public DateTime Uploaded
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
        {            
            Type = "Document";
            Uploaded = DateTime.MinValue;
        }
    }
}
