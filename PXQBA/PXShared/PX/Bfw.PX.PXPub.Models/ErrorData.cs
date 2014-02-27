using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents an Error message & exception details
    /// </summary>
    public class ErrorData
    {
        /// <summary>
        /// User friendly message title
        /// </summary>
        public string DisplayMessageTitle { get; set; }

        /// <summary>
        /// User friendly message
        /// </summary>       
        public string DisplayMessage { get; set; }
        
        /// <summary>
        /// Actual exception details
        /// </summary>        
        public Exception Exception { get; set; }

        /// <summary>
        /// Flag to show detail errors on error page
        /// </summary>        
        public bool ShowDetailErrors { get; set; }
    }
}
