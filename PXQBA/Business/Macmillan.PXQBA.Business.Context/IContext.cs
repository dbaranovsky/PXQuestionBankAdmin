using System.Security.Cryptography.X509Certificates;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business
{
    public interface IContext
    {
        void Initialize(string loggedUserId);
        ISessionManager SessionManager { get; }

        UserInfo CurrentUser { get; set; }

        string EnrollmentId { get; }
    }
}