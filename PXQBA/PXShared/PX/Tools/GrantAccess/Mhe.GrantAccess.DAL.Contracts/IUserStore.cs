using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mhe.GrantAccess.DAL.Contracts
{
    public interface IUserStore
    {
        /// <summary>
        /// Looks up the reference id attached to the user account with the given e-mail address.
        /// </summary>
        /// <param name="forEmail">e-mail address of the user to lookup.</param>
        /// <returns>reference id of the user.</returns>
        /// <exception cref="System.Exception">if no user with matching email is found.</exception>
        /// <exception cref="Bfw.Agilix.Dlap.DlapException">if there is a communication error with DLAP.</exception>
        string UserReferenceId(string forEmail);
    }
}
