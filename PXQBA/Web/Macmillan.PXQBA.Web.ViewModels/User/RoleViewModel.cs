using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
     public class RoleViewModel
    {
         public int Id { get; set; }
         public string Name { get; set; }

         public IEnumerable<CapabilityGroupViewModel> CapabilityGroups { get; set; }

         public int ActiveCapabiltiesCount { get; set; }
        
         public bool CanDelete { get; set; }
    }
}
