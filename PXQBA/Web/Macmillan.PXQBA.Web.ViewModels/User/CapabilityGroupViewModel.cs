using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// Group of capabilities
    /// </summary>
    public class CapabilityGroupViewModel
    {
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of capabilities in the group
        /// </summary>
        public IEnumerable<CapabilityViewModel> Capabilities { get; set; } 
    }
}
