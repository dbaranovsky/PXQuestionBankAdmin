using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Agilix.Dlap.Configuration
{
    /// <summary>
    /// Custom configuration section used to setup access to DLAP and BrainHoney.
    /// <example>
    /// 		&lt;configSections>
    /// 			&lt;section name="agilixSessionManager" type="Bfw.Agilix.Dlap.Configuration.SessionManagerSection, Bfw.Agilix.Dlap"/>
    /// 		&lt;/configSections>
    /// 		&lt;agilixSessionManager>
    /// 			&lt;dlapConnection url="http://dev.dlap.bfwpub.com/dlap.ashx" cookiename="AZT">&lt;/dlapConnection>
    /// 			&lt;bhConnection url="http://{1}.dev.brainhoney.bfwpub.com/BrainHoney/Controls/CredentialsUI.ashx" baseurl="http://pxmigration.dev.brainhoney.bfwpub.com/BrainHoney" userDomain="pxmigration" cookieDomain=".bfwpub.com" cookiename="BHAUTH"/>
    /// 			&lt;annonymous id="13" username="pxmigration/anonymous" password="Px-Anon-123">&lt;/annonymous>
    /// 			&lt;admin id="7" username="root/pxmigration" password="asdfasasd">&lt;/admin>
    /// 		&lt;/agilixSessionManager>
    /// 	</example>
    /// </summary>
    public class SessionManagerSection : ConfigurationSection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the admin user.
        /// </summary>
        /// <value>The admin user.</value>
        /// <remarks>User account used when requests require elevated permissions</remarks>
        [ConfigurationProperty("admin", IsRequired = false)]
        public AdminUserElement AdminUser
        {
            get
            {
                return (AdminUserElement)this["admin"];
            }
            set
            {
                this["admin"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the annonymous user.
        /// </summary>
        /// <value>The annonymous user.</value>
        /// <remarks>Annonymous user is used when no actual user is authenticated against the system</remarks>
        [ConfigurationProperty("annonymous", IsRequired = false)]
        public AnnonymousUserElement AnnonymousUser
        {
            get
            {
                return (AnnonymousUserElement)this["annonymous"];
            }
            set
            {
                this["annonymous"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the brain honey connection.
        /// </summary>
        /// <value>The brain honey connection.</value>
        /// <remarks>Connection to the BrainHoney server, from which components are loaded</remarks>
        [ConfigurationProperty("bhConnection", IsRequired = true)]
        public BrainHoneyConnectionElement BrainHoneyConnection
        {
            get
            {
                return (BrainHoneyConnectionElement)this["bhConnection"];
            }
            set
            {
                this["bhConnection"] = value;
            }
        }   

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        /// <remarks>Connection to the DLAP server, to which all requests are sent</remarks>
        [ConfigurationProperty("dlapConnection", IsRequired = true)]
        public ConnectionElement Connection
        {
            get
            {
                return (ConnectionElement)this["dlapConnection"];
            }
            set
            {
                this["dlapConnection"] = value;
            }
        }                 

        #endregion
    }
}
