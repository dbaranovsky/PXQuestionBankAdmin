using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
     [XmlRoot(Namespace = "", ElementName = "CoreWebServiceResponse", IsNullable = false)]
    public class PackageSiteInfoListResponse : IError
    {
        public Error Error { get; set; }

        [XmlElement("PackageSiteInfoList")]
        public PackageSiteInfoList PackageSiteInfoList { get; set; }
    }
}
