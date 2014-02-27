using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;

using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DAL.Models;

namespace Mhe.GrantAccess.DAL
{
    public class PxUserStore : IUserStore
    {
        protected ISessionManager SessionManager { get; set; }

        public PxUserStore(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        /// <summary>
        /// Looks up the reference id attached to the user account with the given e-mail address.
        /// </summary>
        /// <param name="forEmail">e-mail address of the user to lookup.</param>
        /// <returns>reference id of the user.</returns>
        /// <exception cref="System.Exception">if no user with matching email is found.</exception>
        /// <exception cref="Bfw.Agilix.Dlap.DlapException">if there is a communication error with DLAP.</exception>
        public string UserReferenceId(string forEmail)
        {
            var email = forEmail;
            var session = SessionManager.CurrentSession;

            var cmd = new ListUsers
            {
                Query = string.Format("/email='{0}'", forEmail)
            };

            session.ExecuteAsAdmin(cmd);

            var user = cmd.Users.FirstOrDefault();

            if (user == null)
            {
                throw new Exception(string.Format("user with e-mail {0} could not be found", email));
            }

            return user.UserName;
        }
    }
}
