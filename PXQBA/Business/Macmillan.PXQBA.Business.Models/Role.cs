using System.Collections.Generic;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        private bool canEdit = true;

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

        public int CapabilitiesCount { get; set; }
    }
}