// -----------------------------------------------------------------------
// <copyright file="SchoolRespose.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.DataContracts
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [System.Serializable]
    /// <summary>
    /// This is list of school response returned from Onyx Service
    /// <string xmlns="http://www.bfwpub.com/webservice">
    ///     <Schools>
    ///         <School>
    ///             <iCompanyId />
    ///             <vchCompanyName />
    ///         </School>
    ///     </Schools>
    ///     <Error />
    ///     <Warning />
    ///     <Feedback />
    /// </string>
    /// </summary>
    [XmlRoot(Namespace = "http://www.bfwpub.com/webservice", ElementName = "string", IsNullable = false)]
    public class SchoolRespose
    {
        /// <summary>
        /// Gets or sets List of schools
        /// </summary>
        [XmlElement("Schools")]
        public Schools Schools { get; set; }

        /// <summary>
        /// Gets or sets Error received from Onyx service
        /// </summary>
        [XmlElement("Error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets Warning received from service
        /// </summary>
        [XmlElement("Warning")]
        public string Warning { get; set; }

        /// <summary>
        /// Gets or sets Feedback received from Onyx service. Contains service result info
        /// </summary>
        [XmlElement("Feedback")]
        public string Feedback { get; set; }
    }

    /// <summary>
    /// structure of Schools used as list collection
    /// </summary>
    public class Schools
    {
        /// <summary>
        /// Gets or sets School element
        /// </summary>
        [XmlElement("School")]
        public List<School> School { get; set; }
    }

    /// <summary>
    /// structure of individual school element
    /// </summary>
    public class School
    {
        /// <summary>
        /// Gets or sets School / Company Id element
        /// </summary>
        [XmlElement("iCompanyId")]
        public string CompanyId { get; set; }

        /// <summary>
        /// Gets or sets Name of school / company element
        /// </summary>
        [XmlElement("vchCompanyName")]
        public string CompanyName { get; set; }
    }
}