using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [XmlRoot(Namespace = "", ElementName = "RAWebServiceResponse", IsNullable = false)]
	public class UserProfileResponse
	{
	//<RAWebServiceResponse xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	//<Error>
	//    <Code>0</Code>
	//    <Message />
	//</Error>
	//<UserProfile>
	//    <Error>
	//        <Code>0</Code>
	//        <Message />
	//    </Error>
	//    <UserId>2</UserId>
	//    <Email>student5@bfwpub.com</Email>
	//    <FirstName>QA</FirstName>
	//    <LastName>Student</LastName>
	//    <Revoked>false</Revoked>
	//    <ImageURL />
	//</UserProfile>

		public Error Error { get; set; }

		[XmlElement("UserProfile")]
		public List<UserProfile> UserProfile { get; set; }

	}

    [Serializable]
    [XmlRoot(Namespace="", ElementName="CoreWebServiceResponse", IsNullable=false)]
    public class UserAuthResponse
    {
    //    <CoreWebServiceResponse xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">
    //    <Error>
    //        <Code>0</Code>
    //        <Message/>
    //    </Error>
    //    <UserInfo>
    //        <DateCreated>2011-01-27T10:54:24.077</DateCreated>
    //        <FirstName>QA</FirstName>
    //        <LastName>instructor 3</LastName>
    //        <MailPreferences>HTML</MailPreferences>
    //        <OptInEmail>false</OptInEmail>
    //        <PasswordHint>1-6</PasswordHint>
    //        <Revoked>false</Revoked>
    //        <UserEmail>instructor3@bfwpub.com</UserEmail>
    //        <UserID>9</UserID>
    //        <UserName>instructor3@bfwpub.com</UserName>
    //    </UserInfo>
    //</CoreWebServiceResponse>

        public Error Error { get; set; }

        [XmlElement(ElementName = "UserInfo", IsNullable = true)]
        public AuthUserInfo UserInfo { get; set; }
    }

    public class AuthUserInfo
    {
        [XmlElement("DateCreated")]
        public DateTime DateCreated { get; set; }

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlElement("MailPreferences")]
        public string MailPreferences { get; set; }

        [XmlElement("OptInEmail")]
        public bool OptInEmail { get; set; }

        [XmlElement("PasswordHint")]
        public string PasswordHint { get; set; }

        [XmlElement("Revoked")]
        public string Revoked { get; set; }

        [XmlElement("UserEmail")]
        public string Email { get; set; }

        [XmlElement("UserID")]
        public string UserId { get; set; }

        [XmlElement("UserName")]
        public string UserName { get; set; }
    }
	
    [Serializable]
	public class UserProfileError
	{
		[XmlElement("Code")]
		public string Code { get; set; }

		[XmlElement("Message")]
		public string Message { get; set; }
	}

    [Serializable]
	public class Error
	{
		[XmlElement("Code")]
		public string Code { get; set; }

		[XmlElement("Message")]
		public string Message { get; set; }
	}

	[Serializable]
	public class UserProfile
	{
		[XmlElement("Error")]
		public UserProfileError Error { get; set; }

		[XmlElement("Id")]
		public string UserId { get; set; }

		[XmlElement("UserId")]
		public string ReferenceId { get; set; }

        [XmlElement("UserName")]
        public string Username { get; set; }

		[XmlElement("Email")]
		public string Email { get; set; }

		[XmlElement("FirstName")]
		public string FirstName { get; set; }

		[XmlElement("LastName")]
		public string LastName { get; set; }

		[XmlElement("Revoked")]
		public string Revoked { get; set; }

        [XmlElement("LastLogin")]
	    public DateTime? LastLogin { get; set; }

	    [XmlElement("ImageURL")]
		public string AvatarUrl { get; set; }

	}


}

