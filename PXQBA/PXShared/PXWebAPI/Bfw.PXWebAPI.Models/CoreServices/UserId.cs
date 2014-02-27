using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    [XmlRoot("UserId")]
    public class UserId
    {
        [XmlElement("Id")]
        public string Id { get; set; }
    }
}
