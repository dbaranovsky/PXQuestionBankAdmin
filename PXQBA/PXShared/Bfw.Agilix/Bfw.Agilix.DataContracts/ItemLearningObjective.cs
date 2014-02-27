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
    public class ItemLearningObjective : IDlapEntityParser
    {
        /// <summary>
        /// The GUID of the learning objective
        /// </summary>
        public string Guid { get; set; }

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