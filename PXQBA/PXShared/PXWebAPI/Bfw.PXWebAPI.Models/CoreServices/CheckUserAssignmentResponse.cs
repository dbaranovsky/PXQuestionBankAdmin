using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    [XmlRoot(Namespace = "", ElementName = "CoreWebServiceResponse", IsNullable = false)]
    public class CheckUserAssignmentResponse : IError
    {
        public Error Error { get; set; }

        [XmlElement("UserAssignment")]
        public UserAssignment UserAssignmentInfo { get; set; }
    }
}
