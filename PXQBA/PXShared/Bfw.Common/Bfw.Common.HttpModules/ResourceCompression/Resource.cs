using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.HttpModules.ResourceCompression
{
    /// <summary>
    /// Resources are groupings of files that are processed into a single 
    /// stream of data for return to the client.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Path the user requests to get the processed resource.
        /// </summary>
        public string Path { get; set; }

		/// <summary>
		/// CdnPath the user requests to get the processed resource.
		/// </summary>
		public string CdnPath { get; set; }

        /// <summary>
        /// Set of files that make up the resource.
        /// </summary>
        public IList<File> Files { get; set; }

        /// <summary>
        /// True if all files in the resource should be compressed.
        /// </summary>
        public bool Compress { get; set; }

        /// <summary>
        /// True if the final output of the resource should be cached.
        /// The default is false.
        /// </summary>
        public bool Cache { get; set; }

        /// <summary>
        /// Sets the last modified date for this resource
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Version number of the resource
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Type of resource.
        /// </summary>
        public Configuration.ResourceCompression.ResourceType Type { get; set; }

        public string Content { get; set; }

        public string ContentType
        {
            get
            {
                var contentType = string.Empty;
                if (Type == Configuration.ResourceCompression.ResourceType.js)
                {
                    contentType = "text/javascript";
                }
                else if (Type == Configuration.ResourceCompression.ResourceType.css)
                {
                    contentType = "text/css";
                }

                return contentType;
            }
        }
    }
}
