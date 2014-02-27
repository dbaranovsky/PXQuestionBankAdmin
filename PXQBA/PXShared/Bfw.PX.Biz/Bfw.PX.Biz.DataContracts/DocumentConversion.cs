using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a document conversion transaction.
    /// </summary>
    public class DocumentConversion
    {
        /// <summary>
        /// FileName of the document.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Stream to be converted.
        /// </summary>
        public System.IO.Stream DataStream { get; set; }

        /// <summary>
        /// Output type of the download file.
        /// </summary>
        public DocumentOutputType OutputType { get; set; }
    }
}
