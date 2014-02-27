using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using System.Xml.Linq;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Maps an enrollment to a group.
    /// </summary>
    public class GroupMember : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// Id of the group.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Id of the enrollment.
        /// </summary>
        public string EnrollmentId  { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public GroupMember()
        {
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Constructs the XML representation of the object.
        /// </summary>
        /// <returns>XML representation of the object.</returns>
        public XElement ToEntity()
        {
            var element = new XElement(ElStrings.Member);
            if (!string.IsNullOrEmpty(GroupId))
                element.Add(new XAttribute(ElStrings.GroupId, GroupId));
            if (!string.IsNullOrEmpty(EnrollmentId))
                element.Add(new XAttribute(ElStrings.EnrollmentId, EnrollmentId));

            return element;
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            // Set the Enrollment items
            var groupId = element.Attribute(ElStrings.GroupId);
            var enrollmentId = element.Attribute(ElStrings.EnrollmentId);

            if (null != groupId)
            {
                GroupId = groupId.Value;
            }

            if (null != enrollmentId)
            {
                EnrollmentId = enrollmentId.Value;
            }            
        }

        #endregion
    }
}
