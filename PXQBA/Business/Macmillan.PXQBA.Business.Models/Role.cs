using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// QBA role
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Role id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }

        private bool canEdit = true;

        /// <summary>
        /// Indicates if current user has capability to edit the role
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return canEdit;
            }
            set
            {
                canEdit = value;
            }
        }

        private IList<Capability> capabilities;

        /// <summary>
        /// List of capabilities for the role
        /// </summary>
        public IList<Capability> Capabilities
        {
            get
            {
                if (capabilities == null)
                {
                    capabilities = new List<Capability>();
                }
                return capabilities;

            }
            set
            {
                capabilities = value;
            }
        }

        /// <summary>
        /// Count of checked capabilities of the role
        /// </summary>
        public int CapabilitiesCount { get; set; }
    }
}