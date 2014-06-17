using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
     public class RoleViewModel
    {
         public string Id { get; set; }
         public string Name { get; set; }

         public IEnumerable<CapabilityGroupViewModel> CapabilityGroups { get; set; }

         public int ActiveCapabiltiesCount { get; set; }
        
         public bool CanDelete { get; set; }
         public int ActiveCapabiltiesCount
         {
             get
             {
                 return CapabilityGroups == null ? 0 : CapabilityGroups.Sum(capabilities => capabilities.Capabilities.Count(x => x.IsActive));
             }
         }
    }
}
