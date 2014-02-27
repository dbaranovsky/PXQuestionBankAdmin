using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    [XmlRoot("UserId")]
    public class UserAssignment
    {
        public Error Error { get; set; }

        [XmlElement("IsNew")]
        public bool IsNew { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlElement("WasReset")]
        public bool WasReset { get; set; }
    }
}
