using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// Role view model
    /// </summary>
     public class RoleViewModel
    {
         /// <summary>
         /// Role id
         /// </summary>
         public int Id { get; set; }

         /// <summary>
         /// Role name
         /// </summary>
         public string Name { get; set; }

         /// <summary>
         /// Capability groups
         /// </summary>
         public IEnumerable<CapabilityGroupViewModel> CapabilityGroups { get; set; }

         /// <summary>
         /// Number of checked for this role capabilities
         /// </summary>
         public int ActiveCapabiltiesCount { get; set; }
        
         /// <summary>
         /// Indicates if current user can edit the role
         /// </summary>
         public bool CanEdit { get; set; }
    }
}
