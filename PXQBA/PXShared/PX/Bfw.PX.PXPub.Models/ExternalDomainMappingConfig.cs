using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{

    public class ExternalDomainMappingConfig
    {

        public bool Enable { get;set; }
        public List<ExternalDomainMap> Mappings;

        public ExternalDomainMappingConfig()
        {
            Mappings = new List<ExternalDomainMap>();
        }
    }
    public class ExternalDomainMap
    {
        public string MapFrom;
        public string MapTo;
    }
}
