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
    /// Contains information about each grade returned by a command.
    /// </summary>
    public class GradeList : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Properties

        /// <summary>
        /// Status of the grade.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// version of the last teacher resposne.
        /// </summary>
        public string Responseversion { get; set; }

        /// <summary>
        /// Duration of time spent on the material.
        /// </summary>
        public string Seconds { get; set; }

        /// <summary>
        /// Date of last submission.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Last submitted version.
        /// </summary>
        public string Submittedversion { get; set; }

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
                var statusAttr = element.Attribute("status");
                var responseversionAttr = element.Attribute("responseversion");
                var secondsAttr = element.Attribute("seconds");
                var submitteddateAttr = element.Attribute("submitteddate");
                var submittedversionAttr = element.Attribute("submittedversion");

                if (null != statusAttr)
                    Status = statusAttr.Value;

                if (null != responseversionAttr)
                    Responseversion = responseversionAttr.Value;

                if (null != secondsAttr)
                    Seconds = secondsAttr.Value;

                if (null != submitteddateAttr)
                {
                    DateTime dt;
                    if (DateTime.TryParse(submitteddateAttr.Value, out dt))
                        SubmittedDate = dt;
                }

                if (null != submittedversionAttr)
                    Submittedversion = submittedversionAttr.Value;
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
            var element = new XElement(ElStrings.GradeList,
                new XAttribute(ElStrings.Status, Status),
                new XAttribute(ElStrings.ResponseVersion, Responseversion),
                new XAttribute(ElStrings.Seconds, Seconds),
                new XAttribute(ElStrings.SubmittedDate, DateRule.Format(SubmittedDate)),
                new XAttribute(ElStrings.SubmittedVersion, Submittedversion));

            return element;
        }

        #endregion
    }
}
