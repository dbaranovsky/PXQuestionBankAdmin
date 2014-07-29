using System.Security.Cryptography.X509Certificates;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business
{
    /// <summary>
    /// Represents business context of the application. Stores information about logged in user and dlap session
    /// Provides access to cache.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Initializes logged in user with dlap user and session with dlap admin connection
        /// </summary>
        /// <param name="loggedUserId">Logged in user id</param>
        void Initialize(string loggedUserId);

        /// <summary>
        /// Provides access to dlap and allows to run dlap commands
        /// </summary>
        ISessionManager SessionManager { get; }

        /// <summary>
        /// Contains information about logged in user
        /// </summary>
        UserInfo CurrentUser { get; set; }

        /// <summary>
        /// Provides access to application cache
        /// </summary>
        Bfw.Common.Caching.ICacheProvider CacheProvider { get; }

        /// <summary>
        /// Gets information about site by its url
        /// </summary>
        /// <param name="url">Site url</param>
        /// <returns>Site information</returns>
        RAg.Net.RAWS.GetCourseSiteID.SiteInfo GetSiteInfo(string url);

    }
}