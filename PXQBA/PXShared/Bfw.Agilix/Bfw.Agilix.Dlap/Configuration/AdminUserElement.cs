using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Configuration
{
    /// <summary>
    /// Element that represents and admin user. This is the account used when a command is executed with elevated privileges.
    /// <example>
    /// &lt;admin id="7" username="root/pxmigration" password="asdfasasd">&lt;/admin>
    /// </example>
    /// </summary>
    public class AdminUserElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        /// <remarks>id of the administrative user account in DLAP</remarks>
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
        /// <remarks>password of the administrative user in DLAP</remarks>
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
        /// <remarks>username of the administrative user in DLAP. expects {domain}/{username} format</remarks>
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
