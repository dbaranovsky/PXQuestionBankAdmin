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
    /// Represents an event that has occured on the DLAP server.
    /// </summary>
    public class Signal : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Properties
        
        /// <summary>
        /// Id of the signal that occured.
        /// </summary>
        public string SignalId { get; set; }

        /// <summary>
        /// Id of the domain to which the signal applies.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Id of the question to which the signal applies
        /// Signal types: QuestionChanged 4.4, QuestionDeleted 4.5, UserComment 4.7
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Type of signal that occured.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Id of the entity to which the signal applies.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Date and time at which the signal was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Data about who or what created the signal.
        /// </summary>
        public string CreationBy { get; set; }

        /// <summary>
        /// Enrollment status before change
        /// Signal types: EnrollmentChanged 1.2, GradeStatusChanged 3.3
        /// </summary>
        public EnrollmentStatus OldStatus { get; set; }

        /// <summary>
        /// Enrollment status after change
        /// Signal types: EnrollmentChanged 1.2, GradeStatusChanged 3.3
        /// </summary>
        public EnrollmentStatus NewStatus { get; set; }

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
                var signalidAttr = element.Attribute(ElStrings.SignalId);
                var entityAttr = element.Attribute(ElStrings.Entityid);
                var domainidAttr = element.Attribute(ElStrings.DomainId);
                var typeAttr = element.Attribute(ElStrings.type);
                var creationdateAttr = element.Attribute(ElStrings.CreationDate);
                var creationbyAttr = element.Attribute(ElStrings.CreationBy);

                if (null != signalidAttr)
                {
                    SignalId = signalidAttr.Value;
                }

                if (null != entityAttr)
                {
                    EntityId = entityAttr.Value;
                }

                if (null != domainidAttr)
                {
                    DomainId = domainidAttr.Value;
                }

                if (null != typeAttr)
                {
                    Type = typeAttr.Value;
                }

                if (null != creationdateAttr)
                {
                    DateTime dt;

                    if (DateTime.TryParse(creationdateAttr.Value, out dt))
                    {
                        CreationDate = dt;
                    }
                }

                if (null != creationbyAttr)
                {
                    CreationBy = creationbyAttr.Value;
                }

                var data = element.Descendants("data").FirstOrDefault();

                if (data != null)
                {
                    var oldstatusAttr = data.Attribute(ElStrings.OldStatus);
                    var newstatusAttr = data.Attribute(ElStrings.NewStatus);
                    var questionAttr = data.Attribute(ElStrings.QuestionId);

                    if (null != oldstatusAttr)
                    {
                        OldStatus = (EnrollmentStatus)(oldstatusAttr.Value == null ? 0 : Int32.Parse(oldstatusAttr.Value));
                    }

                    if (null != newstatusAttr)
                    {
                        NewStatus = (EnrollmentStatus)(newstatusAttr.Value == null ? 0 : Int32.Parse(newstatusAttr.Value));
                    }

                    if (null != questionAttr)
                    {
                        QuestionId = questionAttr.Value;
                    }
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
            var element = new XElement(ElStrings.Signal,
                new XAttribute(ElStrings.SignalId, SignalId),
                new XAttribute(ElStrings.DomainId, DomainId),
                new XAttribute(ElStrings.type, Type),
                new XAttribute(ElStrings.CreationDate, DateRule.Format(CreationDate)),
                new XAttribute(ElStrings.CreationBy, CreationBy));

            return element;
        }

        #endregion
    }
}