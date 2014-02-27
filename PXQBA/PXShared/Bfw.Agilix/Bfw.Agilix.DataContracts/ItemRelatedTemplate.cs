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
    /// Represents a Related Template associated with a item.
    /// See http://dev.dlap.bfwpub.com/Docs/Schema/ItemData
    /// </summary>
    public class ItemRelatedTemplate : IDlapEntityParser
    {
        /// <summary>
        /// The Id of the Related template
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Name of the Related template
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Message of the Related template
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Parse and objective XML element and populate this object's state.
        /// </summary>
        /// <param name="root"></param>
        public void ParseEntity(XElement root)
        {
            if (root.Name != ElStrings.Template.LocalName)
            {
                throw new DlapEntityFormatException(string.Format("Expected root element to be 'template', but got '{0}' instead", root.Name));
            }

            var idAttribute = root.Attribute(AttributeStrings.Id);
            if (idAttribute == null)
            {
                throw new DlapEntityFormatException("Required attribute 'Id' is missing from 'template' element");
            }
            else
            {
                Id = idAttribute.Value;
            }

            var nameAttribute = root.Attribute(ElStrings.Name);
            if (nameAttribute == null)
            {
                throw new DlapEntityFormatException("Required attribute 'Name' is missing from 'template' element");
            }
            else
            {
                Name = nameAttribute.Value;
            }


            var messageAttribute = root.Attribute(ElStrings.Message);
            if (messageAttribute == null)
            {
                throw new DlapEntityFormatException("Required attribute 'Message' is missing from 'template' element");
            }
            else
            {
                Message = messageAttribute.Value;
            }
        }

    }
}