using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mhe.GrantAccess.DAL.Models;

namespace Mhe.GrantAccess.DAL.Contracts
{
    public interface IRightsStore
    {
        /// <summary>
        /// Stores the specified rights.
        /// </summary>
        /// <param name="rights">Information about the rights that should be granted.</param>
        void StoreRights(PxWebUserRights rights);

        /// <summary>
        /// Finds any rights for the given user, course, and component.
        /// </summary>
        /// <param name="forUser">Reference Id of the user to find rights for.</param>
        /// <param name="andCourse">Course Id to restrict results to.</param>
        /// <param name="andComponent">Component name to restrict results to.</param>
        /// <returns></returns>
        PxWebUserRights FindRights(string forUser, string andCourse, PxWebRights andComponent);
    }
}
