using System.Security.Cryptography.X509Certificates;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business
{
    public interface IContext
    {
        void Initialize(string loggedUserId);
        ISessionManager SessionManager { get; }

        UserInfo CurrentUser { get; set; }

        /// <summary>
        /// Gets the cache provider.
        /// </summary>
        Bfw.Common.Caching.ICacheProvider CacheProvider { get; }

    }
}