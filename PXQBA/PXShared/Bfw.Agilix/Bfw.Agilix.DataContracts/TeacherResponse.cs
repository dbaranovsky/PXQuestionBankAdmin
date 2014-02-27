using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using System.IO;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Conforms to http://dev.dlap.bfwpub.com/Docs/Schema/Response schema
    /// </summary>
    public class TeacherResponse : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Properties

        /// <summary>
        /// Type of the response.
        /// </summary>
        public TeacherResponseType TeacherResponseType { get; set; }

        /// <summary>
        /// Status of the grade in the response.
        /// </summary>
        public GradeStatus Mask { get; set; }

        /// <summary>
        /// Points that have been assigned in the response.
        /// </summary>
        public double PointsAssigned { get; set; }

        /// <summary>
        /// Points calculated as part of the response.
        /// </summary>
        public double PointsComputed { get; set; }

        /// <summary>
        /// Maximum number of points possible.
        /// </summary>
        public double PointsPossible { get; set; }

        /// <summary>
        /// Version of the submission scored as part of the response.
        /// </summary>
        public int ScoredVersion { get; set; }

        /// <summary>
        /// Status of the grade in the response.
        /// </summary>
        public GradeStatus Status { get; set; }

        /// <summary>
        /// Date and time the response was submitted.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Id of the object the response applies to.
        /// </summary>
        public string ForeignId { get; set; }

        /// <summary>
        /// Teacher comment.
        /// </summary>
        public string TeacherComment { get; set; }

        /// <summary>
        /// Memorystream.
        /// </summary>
        public MemoryStream ResourceStream { get; set; }

        /// <summary>
        /// Teacher Attachments.
        /// </summary>
        public List<Attachment> TeacherAttachments { get; set; }
        
        /// <summary>
        /// True if teacher Response exist. False otherwise
        /// </summary>
        public bool DoesResponseExist { get; set; }

        /// <summary>
        /// Id of the enrollment for the student he resposne applies to.
        /// </summary>
        public string StudentEnrollmentId { get; set; }

        /// <summary>
        /// Any nested responses.
        /// </summary>
        public List<TeacherResponse> Responses { get; set; }

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

            if (null == element) return;
            var typeAttr = element.Attribute(ElStrings.type);
            var maskAttr = element.Attribute(ElStrings.Mask);
            var pointsAssignedAttr = element.Attribute(ElStrings.PointsAssigned);
            var pointsComputedAttr = element.Attribute(ElStrings.PointsComputed);
            var pointsPossibleAttr = element.Attribute(ElStrings.PointsPossible);
            var scoredVersionAttr = element.Attribute(ElStrings.ScoredVersion);
            var statusAttr = element.Attribute(ElStrings.Status);
            var submittedDateAttr = element.Attribute(ElStrings.SubmittedDate);
            var foreignidAttr = element.Attribute(ElStrings.ForeignId);

            var teacherComment = string.Empty;
            if (element.Element(ElStrings.Notes) != null)
            {
                teacherComment = element.Element(ElStrings.Notes).Value; 
            }

            var teacherAttachments = new List<Attachment>();
            if (element.Element(ElStrings.attachments) != null)
            {
                var xElements = element.Element(ElStrings.attachments).Elements();
                foreach (XElement xElement in xElements)
                { 
                    Attachment attachmentFile = new Attachment();
                    attachmentFile.Name = xElement.Attribute(ElStrings.Name).Value;
                    attachmentFile.Href = xElement.Attribute(ElStrings.Path).Value;
                    teacherAttachments.Add(attachmentFile);
                }
            }


            if (null != typeAttr)
            {
                TeacherResponseType tempResponseType;
                TeacherResponseType.TryParse(typeAttr.Value, true, out tempResponseType);
                TeacherResponseType = tempResponseType;
            }

            if (null != maskAttr)
            {
                int mask;
                if (int.TryParse(maskAttr.Value, out mask))
                {
                    Mask = (GradeStatus)mask;
                }
            }

            if (null != statusAttr)
            {
                int status;
                if (int.TryParse(statusAttr.Value, out status))
                {
                    Status = (GradeStatus)status;
                }
            }

            if (null != pointsAssignedAttr)
            {
                double d;
                if (Double.TryParse(pointsAssignedAttr.Value, out d))
                {
                    PointsAssigned = d;
                }
            }

            if (null != pointsPossibleAttr)
            {
                double d;
                if (Double.TryParse(pointsPossibleAttr.Value, out d))
                {
                    PointsPossible = d;
                }
            }

            if (null != teacherComment)
            {
                TeacherComment = teacherComment;
            }

            if (null != teacherComment)
            {
                TeacherAttachments = teacherAttachments;
            }
            
            
            if (null != pointsComputedAttr)
            {
                double d;
                if (Double.TryParse(pointsComputedAttr.Value, out d))
                {
                    PointsComputed = d;
                }
            }

            if (null != scoredVersionAttr)
            {
                int version;
                if (int.TryParse(scoredVersionAttr.Value, out version))
                {
                    ScoredVersion = version;
                }
            }

            if (null != submittedDateAttr)
            {
                DateTime dt;
                if (DateTime.TryParse(submittedDateAttr.Value, out dt))
                {
                    SubmittedDate = dt;
                }
            }

            if (null != foreignidAttr)
            {
                ForeignId = foreignidAttr.Value;
            }

            var childResponses = element.Elements(ElStrings.Response);
            if (null != childResponses)
            {
                TeacherResponse response;
                Responses = new List<TeacherResponse>();
                foreach (var tempResponse in childResponses)
                {
                    response = new TeacherResponse();
                    response.ParseEntity(tempResponse);
                    Responses.Add(response);
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
            XElement element = null;
            if (TeacherResponseType != TeacherResponseType.None)
            {
                element = new XElement(ElStrings.Response,
                                          new XAttribute(ElStrings.type, TeacherResponseType.ToString().ToLowerInvariant()),
                                          new XAttribute(ElStrings.Mask, Convert.ToInt32(Mask)),
                                          new XAttribute(ElStrings.PointsAssigned, PointsAssigned),
                                          new XAttribute(ElStrings.PointsComputed, PointsComputed),
                                          new XAttribute(ElStrings.PointsPossible, PointsPossible),
                                          new XAttribute(ElStrings.ScoredVersion, ScoredVersion),
                                          new XAttribute(ElStrings.Status, Convert.ToInt32(Status)));
            }
            else
            {
                element = new XElement(ElStrings.Response,
                                          new XAttribute(ElStrings.type, TeacherResponseType.ToString().ToLowerInvariant()),
                                          new XAttribute(ElStrings.Mask, Convert.ToInt32(Mask)),
                                          new XAttribute(ElStrings.PointsAssigned, PointsAssigned),
                                          new XAttribute(ElStrings.ScoredVersion, ScoredVersion),
                                          new XAttribute(ElStrings.Status, Convert.ToInt32(Status)));
            }

            foreach (var response in Responses)
            {
                ToXResponse(response, element);
            }
            return element;
        }

        /// <summary>
        /// To teacher response entity
        /// </summary>
        /// <returns></returns>
        public XElement ToTeacherResponseEntity()
        {
            XElement element = null;
            element = new XElement(ElStrings.TeacherResponse,
                                          new XAttribute(ElStrings.EnrollmentId, StudentEnrollmentId),
                                          new XAttribute(ElStrings.ItemId, ForeignId));
            return element;

        }

        #endregion

        #region Implementation
        public void ToXResponse(TeacherResponse response, XElement root)
        {
            var XResponse = new XElement(ElStrings.Response,
                                         new XAttribute(ElStrings.type, response.TeacherResponseType.ToString().ToLowerInvariant()),
                                         new XAttribute(ElStrings.ForeignId, response.ForeignId),
                                         new XAttribute(ElStrings.PointsAssigned, response.PointsAssigned),
                                         new XAttribute(ElStrings.PointsPossible, response.PointsPossible));
            
            if (response.Responses != null) { 
                foreach (var subResposne in response.Responses)
                {
                    ToXResponse(subResposne, XResponse);
                }
            }
            root.Add(XResponse);
        }
        #endregion
    }
}
