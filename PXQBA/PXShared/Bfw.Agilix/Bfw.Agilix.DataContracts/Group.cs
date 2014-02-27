using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Conforms to http://dev.dlap.bfwpub.com/Docs/Schema/Group.
    /// </summary>
    public class Group : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// Unique id of the group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the group.
        /// </summary>
        public string Title;

        /// <summary>
        /// Id of the user that owns the group.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// External id that identifies the group.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Id of the domain the group belongs to.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Id of the group set the group belongs to.
        /// </summary>
        public string SetId { get; set; }

        /// <summary>
        /// Enrollments that are part of the group.
        /// </summary>
        public IEnumerable<Enrollment> MemberEnrollments;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Group()  
        {
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// See http://dev.dlap.bfwpub.com/Docs/Schema/Group for format
        /// </summary>
        /// <returns>XML representation of the group</returns>
        public XElement ToEntity()
        {
            var element = new XElement(ElStrings.Group);
            if (!string.IsNullOrEmpty(Title))
                element.Add(new XAttribute(ElStrings.title, Title));
            if (!string.IsNullOrEmpty(OwnerId))
                element.Add(new XAttribute(ElStrings.OwnerId, OwnerId));
            element.Add(new XAttribute(ElStrings.Reference, ""));
            if (!string.IsNullOrEmpty(DomainId))
                element.Add(new XAttribute(ElStrings.DomainId,DomainId ));
            if (!string.IsNullOrEmpty(SetId))
                element.Add(new XAttribute(ElStrings.SetId, SetId));             

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
            var id = element.Attribute(ElStrings.GroupId);
            var title = element.Attribute(ElStrings.title);
            var ownerid = element.Attribute(ElStrings.OwnerId);
            var reference = element.Attribute(ElStrings.Reference);
            var domainid = element.Attribute(ElStrings.DomainId);
            var setid = element.Attribute(ElStrings.SetId);
            
           if (null != id)
            {
                Id = Convert.ToInt32(id.Value);
            }
           
            if (null != title)
            {
                Title = title.Value  ;
            }

            if (null != ownerid)
            {
                OwnerId = ownerid.Value;
            }

            if (null != reference)
            {
                Reference = reference.Value;
            }
           
            if (null != domainid)
            {
                DomainId = domainid.Value;
            }

            if (null != setid)
            {
                SetId = setid.Value;
            }           
        }

        #endregion
    }
}
