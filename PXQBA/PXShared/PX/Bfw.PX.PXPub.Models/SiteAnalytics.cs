using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Properties used to configure a site analytics provider such as 
    /// Google Analytics
    /// </summary>
    public class SiteAnalytics
    {
        #region Data Members

        /// <summary>
        /// Unique key used to track traffic for this site. This is typically
        /// tied to the account where the reporting is done.
        /// </summary>
        public string SiteKey { get; set; }

        /// <summary>
        /// Domain to be used for this request for tracking purposes.
        /// </summary>
        public string RequestDomain { get; set; }

        /// <summary>
        /// Path to restric reporting to for this request.
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// Collection of custom parameters passed for this request.
        /// </summary>
        public Dictionary<string, string> CustomParams { get; set; }

        #endregion

        public SiteAnalytics()
        {
            CustomParams = new Dictionary<string, string>();
        }
    }
}
