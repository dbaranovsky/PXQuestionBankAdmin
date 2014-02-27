using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Bfw.Common;
using PxWebUser;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Entity that stores all user information.
    /// </summary>
    [DataContract]
    public class UserInfo : IEqualityComparer<UserInfo>
    {

        [DataMember]
        public Error Error { get; set; }

        /// <summary>
        /// ID of the user.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets the user's first and last names together in a string.
        /// </summary>
        public string FormattedName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }

        /// <summary>
        /// User's e-mail address.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Username of the user.
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// User's password. Not always populated.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// User's password question. Not always populated.
        /// </summary>
        [DataMember]
        public string PasswordQuestion { get; set; }

        /// <summary>
        /// User's password answer. Not always populated.
        /// </summary>
        [DataMember]
        public string PasswordAnswer { get; set; }

        /// <summary>
        /// Date and time of the user's last login. Set to DateRule.MinDate if the user hasn't logged in.
        /// </summary>
        [DataMember]
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// ID of the user's domain.
        /// </summary>
        [DataMember]
        public string DomainId { get; set; }

        /// <summary>
        /// Name of the user's domain.
        /// </summary>
        [DataMember]
        public string DomainName { get; set; }

        /// <summary>
        /// User's External ID.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// User picture
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Enrollment ID for current course
        /// </summary>
        public string EnrollmentIdForCurrentCourse { get; set; }

        /// <summary>
        /// Set of bfw properties stored for the user
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Indicates the Submission Status of an assignment - Saved|Submitted|Gradedd
        /// </summary>
        public SubmissionStatus SubmissionStatus { get; set; }

        public PxWebUserRights WebRights { get; set; }

        public bool Equals(UserInfo x, UserInfo y)
        {

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(UserInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }


}