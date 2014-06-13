using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    public class CapabilityGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<CapabilityViewModel> Capabilities { get; set; } 
    }
}
