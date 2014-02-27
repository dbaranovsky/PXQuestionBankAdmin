using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Configuration
{
    /// <summary>
    /// Contains information about the BrainHoney Connection.
    /// The {1} will be replaced with login prefix for the domain being accessed
    /// <example>
    /// &lt;bhConnection url="http://{1}.dev.brainhoney.bfwpub.com/BrainHoney/Controls/CredentialsUI.ashx" baseurl="http://pxmigration.dev.brainhoney.bfwpub.com/BrainHoney" userDomain="pxmigration" cookieDomain=".bfwpub.com" cookiename="BHAUTH"/>
    /// </example>
    /// </summary>
    public class BrainHoneyConnectionElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base URL. This URL is used as the starting point when loading BrainHoney components
        /// </summary>
        /// <value>The base URL.</value>
        /// <remarks>Used to load BrainHoney components</remarks>
        [ConfigurationProperty("baseurl", IsRequired = true)]
        public string BaseUrl
        {
            get
            {
                return (string)this["baseurl"];
            }
            set
            {
                this["baseurl"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the cookie domain.
        /// </summary>
        /// <value>The cookie domain.</value>
        /// <remarks>The <see cref="CookieName"/> will be written to this domain in the client browser</remarks>
        [ConfigurationProperty("cookieDomain", IsRequired = true)]
        public string CookieDomain
        {
            get
            {
                return (string)this["cookieDomain"];
            }
            set
            {
                this["cookieDomain"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the cookie.
        /// </summary>
        /// <value>The name of the cookie.</value>
        /// <remarks>Name the BrainHoney server is configured to use for the authentication token</remarks>
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
        /// Gets or sets the name of the cookie that contains the active domain name.
        /// The active domain dictates which BrainHoney endpoint is communicated with when
        /// authenticating and working with components. It is also used to help determine
        /// if the user has changed domains and therefore needs to be reauthenticated.
        /// </summary>
        [ConfigurationProperty("activedomaincookiename", IsRequired = false, DefaultValue="bhdomain")]
        public string ActiveDomainCookieName
        {
            get
            {
                var cookieName = "bhdomain";

                cookieName = (string)this["activedomaincookiename"];

                return cookieName;
            }

            set
            {
                this["activedomaincookiename"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL to the BrainHoney handler that supports user authentication
        /// </summary>
        /// <value>The URL.</value>
        /// <remarks>Note this may or may not be the same of the URL of the DLAP server</remarks>
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
        /// Gets or sets the user domain.
        /// </summary>
        /// <value>The user domain.</value>
        /// <remarks>Domain that users are expected to belong in</remarks>
        [ConfigurationProperty("userDomain", IsRequired = true)]
        public string UserDomain
        {
            get
            {
                return (string)this["userDomain"];
            }
            set
            {
                this["userDomain"] = value;
            }
        }

        /// <summary>
        /// True if the session manager allows brainhoney sessions to be resumed, false otherwise.
        /// Default value is true.
        /// </summary>
        [ConfigurationProperty("allowResumeSession", IsRequired = false, DefaultValue=true)]
        public bool AllowResumeSession
        {
            get
            {
                return (bool)this["allowResumeSession"];
            }
            set
            {
                this["allowResumeSession"] = value;
            }
        }

        #endregion
    }
}
