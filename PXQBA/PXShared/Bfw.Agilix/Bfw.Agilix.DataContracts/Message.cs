using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// A message that is part of a blog or other communication between users.
    /// </summary>
    public class Message : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Properties

        /// <summary>
        /// Id of the message, extracted from the ZIP file.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Version of the message, extracted from the ZIP file.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Author of the message.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Author information for the message.
        /// </summary>
        public AgilixUser AuthorInfo { get; set; }

        /// <summary>
        /// Id of the enrollment that created the message.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Date and time the message was created at.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Textual content of the message.
        /// </summary>
        public string Body { get; set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(XElement element)
        {
            if (null != element)
            {
                var idAttr = element.Attribute(ElStrings.MessageId);
                var verAttr = element.Attribute(ElStrings.Version);
                var authAttr = element.Attribute(ElStrings.CreationBy);
                var enrAttr = element.Attribute(ElStrings.EnrollmentId);
                var creAttr = element.Attribute(ElStrings.Created);

                if (null != idAttr)
                    Id = idAttr.Value;

                if (null != verAttr)
                    Version = verAttr.Value;

                if (null != enrAttr)
                    EnrollmentId = enrAttr.Value;

                if (null != authAttr)
                    Author = authAttr.Value;

                if (null != creAttr)
                {
                    DateTime dt;
                    if (DateTime.TryParse(creAttr.Value, out dt))
                        Created = dt;
                }
            }
        }

        #endregion

        #region IDlapEntityTransformer Members

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public XElement ToEntity()
        {
            var element = new XElement(ElStrings.Message,
                new XAttribute(ElStrings.Version, Version),
                new XAttribute(ElStrings.EnrollmentId, EnrollmentId),
                new XAttribute(ElStrings.CreationBy, Author),
                new XAttribute(ElStrings.Created, DateRule.Format(Created)),
                new XElement(ElStrings.attachments));
            
            return element;
        }

        #endregion
    }
}
