using System;
using System.Xml.Serialization;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    [XmlRoot("PackageSiteInfo")]
    public class PackageSiteInfo
    {
        /// <summary>
        /// The BaseUrl.
        /// </summary>
          [XmlElement("BaseUrl")]
        public string BaseUrl { get; set; }

        /// <summary>
        /// The Expiration.
        /// </summary>
        [XmlElement("Expiration")]
        public DateTime Expiration { get; set; }

        /// <summary>
        /// The LevelOfAccess.
        /// </summary>
         [XmlElement("LevelOfAccess")]
        public string LevelOfAccess { get; set; }

        /// <summary>
        /// The SiteID.
        /// </summary>
           [XmlElement("SiteID")]
        public string SiteID { get; set; }

        /// <summary>
        /// The SiteISBN.
        /// </summary>
         [XmlElement("SiteISBN")]
        public string SiteISBN { get; set; }

        /// <summary>
        /// The Type.
        /// </summary>
         [XmlElement("Type")]
        public string Type { get; set; }
    }
}
