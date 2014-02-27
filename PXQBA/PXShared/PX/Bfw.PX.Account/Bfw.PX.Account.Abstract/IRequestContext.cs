using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Bfw.Common.SSO;
using Bfw.Common.Logging;

namespace Bfw.PX.Account.Abstract
{
    /// <summary>
    /// Represents all custom context information for this request.
    /// </summary>
    public interface IRequestContext
    {
        #region Properties

        /// <summary>
        /// Provides direct access to any SSO related data.
        /// </summary>
        SSOData SSOData { get; }

        /// <summary>
        /// Gets the public URL.
        /// </summary>
        Uri PublicUrl { get; }
        
        /// <summary>
        /// Gets the secure URL.
        /// </summary>
        Uri SecureUrl { get; }
        
        /// <summary>
        /// Gets the target URL.
        /// </summary>
        Uri TargetUrl { get; }

        /// <summary>
        /// Gets the site info.
        /// </summary>
        BFW.RAg.Site SiteInfo { get; }

        /// <summary>
        /// Gets the site user info.
        /// </summary>
        BFW.RAg.SiteUserData SiteUserInfo { get; }

        /// <summary>
        /// Gets or sets a logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        Bfw.Common.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the tracer.
        /// </summary>
        /// <value>
        /// The tracer.
        /// </value>
        Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the IRequestContext.
        /// </summary>
        void Init();

        /// <summary>
        /// Redirects to the target URL.
        /// </summary>
        void RedirectToTarget();

        /// <summary>
        /// Initializes the site data.
        /// </summary>
        void InitSiteData();

        /// <summary>
        /// Initializes the site data for a given URL.
        /// </summary>
        /// <param name="Url">The URL.</param>
        void InitSiteData(Uri Url);

        /// <summary>
        /// Reinitializes the site data.
        /// </summary>
        void ReInitSiteData();

        /// <summary>
        /// Reinitializes the site data for a given URL.
        /// </summary>
        /// <param name="Url">The URL.</param>
        void ReInitSiteData(Uri Url);

        #endregion
    }
}
