using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
     [XmlRoot("PackageSiteInfoList")]
    public class PackageSiteInfoList
    {
        [XmlElement("PackageSiteInfo")]
        public List<PackageSiteInfo> PackageSiteInfoL { get; set; }
    }
}
