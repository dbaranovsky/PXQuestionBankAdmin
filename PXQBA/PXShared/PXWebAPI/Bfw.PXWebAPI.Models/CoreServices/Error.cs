using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    public class Error
    {
        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; }
    }
}
