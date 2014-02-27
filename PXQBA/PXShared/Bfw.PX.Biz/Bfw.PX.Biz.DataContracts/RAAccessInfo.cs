using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
    [XmlRoot(Namespace="", ElementName="CoreWebServiceResponse", IsNullable=false)]
    public class RAAccessInfo
    {
        //<CoreWebServiceResponse>
        //    <Error>
        //            <Code>0</Code>
        //            <IsValidationError>false</IsValidationError>
        //            <Message/>
        //    </Error>
        //    <UserAccessLevel>
        //        <AccessLevel>
        //            <Description>Adopting Instructor</Description>
        //            <LevelOfAccess>70</LevelOfAccess>
        //        </AccessLevel>
        //            <CourseID>0</CourseID>
        //            <ExpirationDate>20700101T00:00:00</ExpirationDate>
        //            <IsRevoked>false</IsRevoked>
        //            <LevelOfAccess>70</LevelOfAccess>
        //    </UserAccessLevel>
        //</CoreWebServiceResponse>

        public Error Error { get; set; }

        [XmlElement(ElementName = "UserAccessLevel")]
        public UserAccessLevel AccessLevel { get; set; }
    }

    public class UserAccessLevel
    {
        [XmlElement(ElementName = "CourseId")]
        public string CourseId { get; set; }

        [XmlElement(ElementName = "ExpirationDate")]
        public DateTime ExpirationDate { get; set; }

        [XmlElement(ElementName = "IsRevoked")]
        public bool IsRevoked { get; set; }

        [XmlElement(ElementName = "LevelOfAccess")]
        public int LevelOfAccess { get; set; }
    }
}
