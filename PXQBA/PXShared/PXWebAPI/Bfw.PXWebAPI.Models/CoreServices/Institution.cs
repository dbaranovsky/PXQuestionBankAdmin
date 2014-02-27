using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
     [XmlRoot("Institution")]
    public class Institution
    {
        /// <summary>
        /// Institution Id
        /// </summary>
        [XmlElement("ID")]
         public string Id { get; set; }

        /// <summary>
        /// Domain Name.
        /// </summary>
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Agilix Domain Id.
        /// </summary>
        [XmlElement("AgilixDomainId")]
        public string AgilixDomainId { get; set; }

        /// <summary>
        /// Short Code.
        /// </summary>
        [XmlElement("ShortCode")]
        public string ShortCode { get; set; }

     
    }
}
