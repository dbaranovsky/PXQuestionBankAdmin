using System.Collections.Generic;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    public class Role
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool CanDelete { get; set; }

        private IEnumerable<Capability> capabilities;

        public IEnumerable<Capability> Capabilities
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