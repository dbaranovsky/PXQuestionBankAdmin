using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// List of roles
    /// </summary>
    public class RoleListViewModel
    {
        private IEnumerable<RoleViewModel> roles;

        /// <summary>
        /// List of roles
        /// </summary>
        public IEnumerable<RoleViewModel> Roles
        {
            get
            {
                if (roles == null)
                {
                    roles = new List<RoleViewModel>();
                }
                return roles;
            }
            set
            {
                roles = value;
            }
        }

        /// <summary>
        /// Indicates if current user can define new role
        /// </summary>
        public bool CanDefineNewRole { get; set; }
    }
}