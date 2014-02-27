using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents a binary resource of some type.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// ID of the entity the resource will live under.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Path to the resource.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Extension of the underlying file type, e.g. html, pdf, etc.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Stores the resource's binary data, if any.
        /// </summary>
        protected MemoryStream ResourceStream { get; set; }

        /// <summary>
        /// The content type of the resource, defaults to empty but should be set
        /// to one of the values defined by ResourceContentType.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Determines if a resource is hidden or normal (default).
        /// </summary>
        public ResourceStatus Status { get; set; }

        /// <summary>
        /// Date that resource created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Date that resource created.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Constructs a resource with an empty stream and a status of Normal.
        /// </summary>
        public Resource()
        {
            Status = ResourceStatus.Normal;
            ExtendedProperties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Additional key-value pairs for xml meta data loading.
        /// </summary>
        public Dictionary<string, string> ExtendedProperties;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns a read/write/seek capable stream that holds the resource's 
        /// binary data, if any. The stream returned will be set to the first
        /// byte (i.e. offset 0 from SeekOrigin.Begin).
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            if (null == ResourceStream)
            {
                ResourceStream = new MemoryStream();
            }

            ResourceStream.Seek(0, SeekOrigin.Begin);

            return ResourceStream;
        }

        /// <summary>
        /// Gets or sets the extended ID.
        /// </summary>
        /// <value>
        /// The extended id.
        /// </value>
        public string ExtendedId { get; set; }
    }

}
