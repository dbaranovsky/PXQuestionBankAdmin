using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Common.HttpModules.Configuration.ResourceCompression
{
    /// <summary>
    /// Represents information on how to access the report about what resources
    /// are available.
    /// </summary>
    public class ReportElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// The URL to the report page. The report page shows any errors or warning
        /// that may cause problems during compression. The report also shows how much
        /// value there is in compressing each resource and file.
        /// </summary>
        [ConfigurationProperty(name: "url", IsRequired = false)]
        public string Url
        {
            get
            {
                return (string)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

        #endregion
    }
}
