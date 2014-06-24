using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
     public class RoleViewModel
    {
         public int Id { get; set; }
         public string Name { get; set; }

         public IEnumerable<CapabilityGroupViewModel> CapabilityGroups { get; set; }

         public int ActiveCapabiltiesCount { get; set; }
        
         public bool CanEdit { get; set; }
    }

    public class RoleListViewModel
    {
        private IEnumerable<RoleViewModel> roles;

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

        public bool CanDefineNewRole { get; set; }
    }
}
