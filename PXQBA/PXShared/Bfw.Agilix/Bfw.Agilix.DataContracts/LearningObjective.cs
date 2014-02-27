using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a Learning objective associated with a course.
    /// See http://dev.dlap.bfwpub.com/Docs/Schema/CourseData
    /// </summary>
    public class LearningObjective : IDlapEntityParser
    {
        /// <summary>
        /// The GUID of the learning objective
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Id of the group to which the learning objective applies.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Parent Id of the learning objective.
        /// </summary>
        public string ParentId { get; set; }


        /// <summary>
        /// Sequence of the learning objective.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Title of the learning objective.
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// Id of the learning objective.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Description of the learning objective.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Parse and objective XML element and populate this object's state.
        /// </summary>
        /// <param name="root"></param>
        public void ParseEntity(XElement root)
        {
            if (root.Name != ElStrings.Objective.LocalName)
            {
                throw new DlapEntityFormatException(string.Format("Expected root element to be 'objective', but got '{0}' instead", root.Name));
            }

            var group = root.Attribute(ElStrings.Group);
            if (group != null)
            {
                Group = group.Value;
            }

            var id = root.Attribute(ElStrings.Id);
            var parentId = root.Attribute(ElStrings.ParentId);
            var sequence = root.Attribute(ElStrings.sequence);
            var title = root.Element(ElStrings.description);
            
            var description = root.Element(ElStrings.Bfw_description);

            if (id == null)
            {
                throw new DlapEntityFormatException("Required attribute 'id' is missing from 'objective' element");
            }
            else
            {
                Id = id.Value;
            }

            if (parentId != null)
            {
                ParentId = parentId.Value;
            }

            if (sequence != null)
            {
                Sequence = sequence.Value;
            }

            if (description != null)
            {
                Description = description.Value;
            }


            if (title == null)
            {
                throw new DlapEntityFormatException("Required element 'description' missing from 'objective' element");
            }
            else
            {
                Title = title.Value;
            }

            var guid = root.Attribute(ElStrings.Guid);
            if (guid == null)
            {
                throw new DlapEntityFormatException("Required attribute 'guid' is missing from 'objective' element");
            }
            else
            {
                Guid = guid.Value;
            }
        }
    }
}
