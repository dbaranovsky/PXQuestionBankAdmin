using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Configuration
{
    /// <summary>
    /// Represents the "connection" element and holds values related to the 
    /// DlapConnection configuration used by the ISessionManager implementation
    /// <example>
    /// &lt;dlapConnection url="http://dev.dlap.bfwpub.com/dlap.ashx" compress="true" cookiename="AZT">&lt;/dlapConnection>
    /// </example>
    /// </summary>
    public class ConnectionElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        /// <value>The agent.</value>
        /// <remarks>Valid user agent string from any major browser should work</remarks>
        [ConfigurationProperty("agent", IsRequired = false, DefaultValue = "Bfw.Agilix.Dlap.DlapConnection")]
        public string Agent
        {
            get
            {
                return (string)this["agent"];
            }
            set
            {
                this["agent"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether compression is enabled.
        /// </summary>
        /// <value><c>true</c> if compression is enabled; otherwise, <c>false</c>.</value>
        /// <remarks>Default is false</remarks>
        [ConfigurationProperty("compress", IsRequired = false, DefaultValue = false)]
        public bool Compress
        {
            get
            {
                return (bool)this["compress"];
            }
            set
            {
                this["compress"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the cookie.
        /// </summary>
        /// <value>The name of the cookie.</value>
        /// <remarks>Name of the authentication cookie the DLAP server is configured to use</remarks>
        [ConfigurationProperty("cookiename", IsRequired = true)]
        public string CookieName
        {
            get
            {
                return (string)this["cookiename"];
            }
            set
            {
                this["cookiename"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        /// <remarks>Unit of time is seconds</remarks>
        [ConfigurationProperty("timeout", IsRequired = false, DefaultValue = 600000)]
        [IntegerValidator()]
        public int Timeout
        {
            get
            {
                return (int)this["timeout"];
            }
            set
            {
                this["timeout"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL of the DLAP server
        /// </summary>
        /// <value>The URL.</value>
        /// <remarks>URL to the handler to which all DLAP requests are sent</remarks>
        [ConfigurationProperty("url", IsRequired = true)]
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

        /// <summary>
        /// Gets or sets the secret key used to create the trust header hash.
        /// </summary>
        [ConfigurationProperty("secretkey", IsRequired = false)]
        public string SecretKey
        {
            get
            {
                return (string)this["secretkey"];
            }
            set
            {
                this["secretkey"] = value;
            }
        }

        [ConfigurationProperty("allowasync", IsRequired = false, DefaultValue = false)]
        public bool AllowAsync
        {
            get
            {
                return (bool)this["allowasync"];
            }
            set
            {
                this["allowasync"] = value;
            }
        }

        #endregion
    }
}
