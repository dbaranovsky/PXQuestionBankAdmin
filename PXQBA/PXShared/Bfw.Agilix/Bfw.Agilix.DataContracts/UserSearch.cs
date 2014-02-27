using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents the information necessary to make a user search in DLAP.
    /// </summary>
    [DataContract]
    public class UserSearch : IDlapEntityTransformer
    {
        #region Properties


        /// <summary>
        /// ID of the user to find.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Username search string. Wild card '*' is allowed.
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// First or last name of the user. Wild card '*' is allowed.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Id of the domain the user list should be filtered by.
        /// </summary>
        [DataMember]
        public string DomainId { get; set; }

        /// <summary>
        /// ExternalId of the user.
        /// </summary>
        [DataMember]
        public string ExternalId { get; set; }

        #endregion

        #region IDlapEntityTransformer Members

        /// <summary>
        /// Creating XML element of request type which contains agilix command and its parameter as attribute.
        /// It will look like &lt;request cmd="getuser" userid="value" />
        /// </summary>
        /// <returns>Return SearchUser XML element.</returns>
        public XElement ToEntity()
        {
            var element = new XElement(ElStrings.Request);

            if (!string.IsNullOrEmpty(Id))
            {
                element.Add(new XAttribute(ElStrings.Cmd, ElStrings.GetUser.LocalName));
                element.Add(new XAttribute(ElStrings.UserId, Id));
            }
            else
            {
                element.Add(new XAttribute(ElStrings.Cmd, ElStrings.GetUserList.LocalName));

                if (!string.IsNullOrEmpty(Username))
                {
                    element.Add(new XAttribute(ElStrings.UserName, Username));
                }

                if (!string.IsNullOrEmpty(Name))
                {
                    element.Add(new XAttribute(ElStrings.Name, Name));
                }

                if (!string.IsNullOrEmpty(DomainId))
                {
                    element.Add(new XAttribute(ElStrings.DomainId, DomainId));
                }

                if (!string.IsNullOrEmpty(ExternalId))
                {
                    element.Add(new XAttribute(ElStrings.Reference, ExternalId));
                }
            }

            return element;
        }

        #endregion
    }
}
