using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
    [XmlRoot(Namespace="", ElementName="CoreWebServiceResponse")]
    public class RASiteInfo
    {
        //<CoreWebServiceResponse>
        //    <Error>
        //        <Code>0</Code>
        //        <IsValidationError>false</IsValidationError>
        //        <Message/>
        //    </Error>
        //    <Sites>
        //        <SiteInfoAndType>
        //            <AgilixCourseID>100406</AgilixCourseID>
        //            <Author>Jeremy M. Berg</Author>
        //            <BaseUrl>qa.whfreeman.com/launchpad/berg7e</BaseUrl>
        //            <EditionNumber>7</EditionNumber>
        //            <Format>Portal</Format>
        //            <Isbn>1464167028 </Isbn>
        //            <Media>Ancillary</Media>
        //            <PublicationDate>20140715T00:00:00</PublicationDate>
        //            <SiteID>28414</SiteID>
        //            <SubType/>
        //            <Type>PX</Type>
        //        </SiteInfoAndType>
        //    </Sites>
        //</CoreWebServiceResponse>
        public Error Error { get; set; }

        public SiteInfoAndType[] Sites { get; set; }
    }

    public class SiteInfoAndType
    {
        [XmlElement(ElementName="AgilixCourseID")]
        public string AgilixCourseId { get; set; }

        [XmlElement(ElementName="SiteID")]
        public string SiteId { get; set; }

        [XmlElement(ElementName = "BaseUrl")]
        public string BaseUrl { get; set; }
    }
}
