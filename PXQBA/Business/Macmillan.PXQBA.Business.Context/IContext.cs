using System.Security.Cryptography.X509Certificates;

namespace Macmillan.PXQBA.Business
{
    public interface IContext
    {
        void InitializeSession(string userName);
    }
}