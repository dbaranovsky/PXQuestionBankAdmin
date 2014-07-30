using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// Capability
    /// </summary>
    public class CapabilityViewModel
    {
        /// <summary>
        /// Capability id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Capability name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if capability is checked
        /// </summary>
        public bool IsActive { get; set; }
    }
}
