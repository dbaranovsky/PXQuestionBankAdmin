using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Bfw.Common.HttpModules.ResourceCompression
{
    /// <summary>
    /// File inside of a resource.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Path to the file on the Physical filesystem.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Absolute path of the URI the user would enter to
        /// access the file.
        /// </summary>
        public string ServerPath { get; set; }

        /// <summary>
        /// True if the file should be compressed. This value can
        /// override the one set on the resource itself.
        /// </summary>
        public bool Compress { get; set; }
    }
}
