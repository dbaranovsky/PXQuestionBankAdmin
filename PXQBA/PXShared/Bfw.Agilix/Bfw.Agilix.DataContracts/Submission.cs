using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a submission of an assignment by a student.
    /// See http://dev.dlap.bfwpub.com/Docs/Schema/Submission for full schema.
    /// </summary>
    public class Submission : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Properties

        /// <summary>
        /// Submission version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// List of submission actions done on the item
        /// </summary>
        public IList<SubmissionAction> Actions { get; set; }

        /// <summary>
        /// Date and time submission was submitted.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Grade, if any, received for the submission.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// First name of the student that submitted the item.
        /// </summary>
        public string StudentFirstName { get; set; }

        /// <summary>
        /// Last name of the student that submitted the item.
        /// </summary>
        public string StudentLastName { get; set; }

        /// <summary>
        /// Id of the item the submission corresponds to.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Id of the enrollment the submission belongs to.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Body of the submission.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Full XML data of the submission as returned by DLAP.
        /// </summary>
        public XDocument Data { get; set; }

        /// <summary>
        /// Full straeam data of the submission as returned by DLAP.
        /// </summary>
        public Stream StreamData { get; set; }

        /// <summary>
        /// Notes for the submission.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Submitted file for the submission.
        /// </summary>
        public string SubmittedFileName { get; set; }

        /// <summary>
        /// Type of the submission.
        /// </summary>
        public SubmissionType SubmissionType { get; set; }

        /// <summary>
        /// True if the submission has been sent to DLAP, false otherwise.
        /// </summary>
        public bool Submitted { get; set; }

        /// <summary>
        /// Question attempts that were part of the submission.
        /// </summary>
        public IDictionary<string, IList<QuestionAttempt>> QuestionAttempts { get; protected set; }

        /// <summary>
        /// Gets or sets the submission attempt.
        /// </summary>
        /// <value>
        /// The submission attempt.
        /// </value>
        public IDictionary<string, SubmissionAttempt> SubmissionAttempts { get; protected set; }

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

            var verAttr = element.Attribute(ElStrings.Version);
            var typeAttr = element.Attribute(ElStrings.type);
            Grade = new Grade();
            Grade.ParseEntity(element);
            if (null != verAttr)
            {
                int version;
                if (Int32.TryParse(verAttr.Value, out version))
                {
                    Version = version;
                }
            }
            if (null != typeAttr)
            {
                SubmissionType tempSubType;
                SubmissionType.TryParse(typeAttr.Value, true, out tempSubType);
                SubmissionType = tempSubType;
            }

            Actions = new List<SubmissionAction>();
            var actionsElement = element.Element(ElStrings.Actions);
            if (actionsElement != null)
            {
                var actionElements = actionsElement.Elements(ElStrings.Action);
                if (actionElements != null)
                {
                    foreach(var actionElement in actionElements)
                    {
                        var sa = new SubmissionAction();

                        String date = null, location = null, type = null;
                        if (actionElement.Attribute(ElStrings.Date) != null)
                        {
                            date = actionElement.Attribute(ElStrings.Date).Value;
                            sa.Date = DateTime.Parse(date);
                        }
                        if (actionElement.Attribute(ElStrings.type) != null)
                        {
                            type = actionElement.Attribute(ElStrings.type).Value;
                            try
                            {
                                sa.Type = (SubmissionActionType)Enum.Parse(typeof(SubmissionActionType), type, true);
                            } catch(Exception) {
                                sa.Type = SubmissionActionType.na;
                            }
                        }
                        if (actionElement.Attribute(ElStrings.Location) != null)
                        {
                            location = actionElement.Attribute(ElStrings.Location).Value;
                            sa.Location = location;
                        }

                        Actions.Add(sa);
                    }
                }
            }

            if (SubmissionType == SubmissionType.Homework || SubmissionType == SubmissionType.Attempt)
            {
                var attempts = element.XPathSelectElements(@"//submission[@type=""question""]");
                var submissionAttempts = element.XPathSelectElements(@"//submission[@type=""attempt""]");

                if (!submissionAttempts.IsNullOrEmpty())
                {
                    SubmissionAttempts = new Dictionary<string, SubmissionAttempt>();
                    SubmissionAttempt sa;
                    string qid = string.Empty;
                    foreach (var subAttempt in submissionAttempts)
                    {
                        var subElement = subAttempt.Element(ElStrings.Submission);
                        qid = subElement.Element(ElStrings.AttemptQuestion).Attribute(ElStrings.Id).Value;
                        var partIdAttr = subAttempt.Attribute(ElStrings.PartId);
                        string partid = "";

                        if (partIdAttr != null)
                        {
                            partid = partIdAttr.Value;
                        }

                        if (!SubmissionAttempts.TryGetValue(qid, out sa))
                        {
                            sa = new SubmissionAttempt();
                            SubmissionAttempts.Add(qid, sa);
                        }

                        bool toContinue = false;
                        try
                        {
                            if (subAttempt.Attribute(ElStrings.Continue).Value != null)
                            {
                                toContinue = Convert.ToBoolean(subAttempt.Attribute(ElStrings.Continue).Value);
                            }
                        }
                        catch { }

                        string lastSave = string.Empty;
                        try
                        {
                            if (subAttempt.Attribute(ElStrings.LastSave).Value != null)
                            {
                                lastSave = subAttempt.Attribute(ElStrings.LastSave).Value;
                            }
                        }
                        catch { }

                        string seconds = string.Empty;
                        try
                        {
                            if (subAttempt.Attribute(ElStrings.Seconds).Value != null)
                            {
                                seconds = subAttempt.Attribute(ElStrings.Seconds).Value;
                            }
                        }
                        catch { }

                        if (!string.IsNullOrEmpty(partid))
                        {
                            sa.PartId = partid;
                            sa.QuestionId = qid;
                            sa.SecondsSpent = seconds;
                            sa.ToContinue = toContinue;
                            sa.LastSave = lastSave;
                        }
                    }
                }

                if (!attempts.IsNullOrEmpty())
                {
                    QuestionAttempts = new Dictionary<string, IList<QuestionAttempt>>();
                    IList<QuestionAttempt> qa;
                    string qid = string.Empty;

                    foreach (var attempt in attempts)
                    {
                        qid = attempt.Element(ElStrings.AttemptQuestion).Attribute(ElStrings.Id).Value;
                        string partid = attempt.Attribute(ElStrings.PartId).Value;
                        qa = null;
                        if (!QuestionAttempts.TryGetValue(qid, out qa))
                        {
                            qa = new List<QuestionAttempt>();
                            QuestionAttempts.Add(qid, qa);
                        }

                        string answer = "";
                        try // add the attempt even if the question wasn't answered.
                        {
                            if (attempt.Element(ElStrings.answer).Value != null)
                            {
                                answer = attempt.Element(ElStrings.answer).Value;
                            }
                        }
                        catch { }

                        string attemptVersion = "";
                        try // add the attempt even if the question wasn't answered.
                        {
                            if (attempt.Element(ElStrings.AttemptQuestion).Attribute(ElStrings.Version).Value != null)
                            {
                                attemptVersion = attempt.Element(ElStrings.AttemptQuestion).Attribute(ElStrings.Version).Value;
                            }
                        }
                        catch { }


                        if (answer != "" || attempt.Element(ElStrings.Notes) == null)
                        {
                            qa.Add(new QuestionAttempt()
                            {
                                QuestionId = qid,
                                AttemptAnswer = answer,
                                PartId = partid,
                                AttemptVersion = attemptVersion
                            });
                        }
                        else if (attempt.Element(ElStrings.Notes) != null)
                        {
                            qa.Add(new QuestionAttempt()
                            {
                                QuestionId = qid,
                                AttemptAnswer = attempt.Element(ElStrings.Notes).Value,
                                PartId = partid,
                                AttemptVersion = attemptVersion
                            });
                        }
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
            var element = new XElement(ElStrings.Submission,
                                       new XAttribute(ElStrings.type, SubmissionType.ToString().ToLower()),
                                       new XElement(ElStrings.attachments));
            if (!string.IsNullOrEmpty(Body))
            {
                element.Element(ElStrings.attachments).Add(new XElement(ElStrings.attachments,
                             new XAttribute(ElStrings.Name, "SubmissionContent"),
                             new XAttribute(ElStrings.Path, "submission.htm")));
            }
            if (StreamData != null)
            {
                if (SubmittedFileName.IsNullOrEmpty())
                {
                    SubmittedFileName = "SubmissionData";
                }
                element.Element(ElStrings.attachments).Add(new XElement(ElStrings.Attachment,
                             new XAttribute(ElStrings.Name, SubmittedFileName),
                             new XAttribute(ElStrings.Path, "data.xml")));
            }
            else if (Data != null)
            {
                if (SubmittedFileName.IsNullOrEmpty())
                {
                    SubmittedFileName = "SubmissionData";
                }
                element.Element(ElStrings.attachments).Add(new XElement(ElStrings.Attachment,
                             new XAttribute(ElStrings.Name, SubmittedFileName),
                             new XAttribute(ElStrings.Path, "data.xml")));
            }

            if (!Notes.IsNullOrEmpty())
            {
                element.Add(new XElement(ElStrings.Notes));
                element.Element(ElStrings.Notes).Value = Notes;
            }
            return element;
        }

        #endregion
    }
}
