using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Configuration
{
    /// <summary>
    /// Contains information about the annonymous user. This is the account used when an anonymous session is started.
    /// <example>
    /// &lt;annonymous id="13" username="pxmigration/anonymous" password="Px-Anon-123">&lt;/annonymous>
    /// </example>
    /// </summary>
    public class AnnonymousUserElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>id of the annonymous user in DLAP</remarks>
        [ConfigurationProperty("id", IsRequired = false)]
        public string Id
        {
            get
            {
                return (string)this["id"];
            }
            set
            {
                this["id"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        /// <remarks>Password of the annonymous user in DLAP</remarks>
        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        /// <remarks>Username of the annonymous user in DLAP. expects {domain}/{username} format</remarks>
        [ConfigurationProperty("username", IsRequired = false)]
        public string Username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }

        #endregion
    }
}
