using System.Security.Cryptography.X509Certificates;
using Bfw.Agilix.Dlap.Session;

namespace Macmillan.PXQBA.Business
{
    public interface IContext
    {
        void InitializeSession(string userName);
        ISessionManager SessionManager { get; }
    }
}