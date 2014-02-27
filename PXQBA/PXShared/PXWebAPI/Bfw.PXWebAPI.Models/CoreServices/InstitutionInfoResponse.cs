using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
     [XmlRoot(Namespace = "", ElementName = "CoreWebServiceResponse", IsNullable = false)]
    public class InstitutionInfoResponse : IError
    {
        public Error Error { get; set; }

        [XmlElement("Institution")]
        public Institution InstitutionInfo { get; set; }
    }
}
