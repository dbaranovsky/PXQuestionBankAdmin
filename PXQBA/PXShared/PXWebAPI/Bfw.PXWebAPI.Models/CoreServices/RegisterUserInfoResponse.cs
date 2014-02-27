using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    [XmlRoot(Namespace = "", ElementName = "CoreWebServiceResponse", IsNullable = false)]
    public class RegisterUserInfoResponse : IError
    {
        public Error Error { get; set; }

        [XmlElement("UserId")]
        public UserId UserIdInfo { get; set; }
    }
}
