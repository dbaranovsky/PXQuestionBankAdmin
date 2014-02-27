using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using System.Xml;

namespace Bfw.Agilix.DataContracts
{
    [Serializable]
    /// <summary>
    /// Represents any kind of item in Agilix.
    /// </summary>
    [DataContract]
    public class Item : IDlapEntityParser, IDlapEntityTransformer, IItem
    {
        #region Properties

        /// <summary>
        /// The Id of the item. This id can be any string as long as it is
        /// unique within the containing entity (e.g. course).
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Id of the entity the item belongs to.
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// ID of the Actual Entity id
        /// </summary>
        [DataMember]
        public string ActualEntityid { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        [DataMember]
        public string ResourceEntityId { get; set; }

        /// <summary>
        /// Id of the user that created the item.
        /// </summary>
        [DataMember]
        public string CreationBy { get; set; }

        /// <summary>
        /// Id of the parent item, null or empty if there is no parent (i.e. DEFAULT).
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }


        /// <summary>
        /// Date the item was created.
        /// </summary>
        [DataMember]
        public DateTime? Created { get; set; }

        /// <summary>
        /// Date the item was last modified.
        /// </summary>
        [DataMember]
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Title from the standard "item data schema", if it exsits.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Subtitle of the item
        /// </summary>
        [DataMember]
        public string SubTitle { get; set; }

        /// <summary>
        /// Description from the standard "item data schema", if it exsits.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// This is the discipine course id, generally used to point to the content course
        /// NOTE: This id can be over written from a bfw_property "agilixdisciplineid" found on the item
        /// </summary>
        [DataMember]
        public string HrefDisciplineCourseId { get; set; }

        /// <summary>
        /// Identifies the Item as a specific Dlap type.  If Type == DlapItemType.Custom then the CustomType
        /// property contains the "raw" string that was stored in the type element.
        /// </summary>
        [DataMember]
        public DlapItemType Type { get; set; }


        /// <summary>
        /// mapped to (bfw_type) for PX Custom Types Project / Source
        /// </summary>
        [DataMember]
        public string SubType { get; set; }

        /// <summary>
        /// Stores the raw string that was stored in the type element.
        /// </summary>
        [DataMember]
        public string CustomType { get; set; }

        /// <summary>
        /// Used only for quizzes.  If this is a custom exam type, what is it (e.g. LearningCurve)?
        /// </summary>
        [DataMember]
        public string CustomExamType { get; set; }

        /// <summary>
        /// True if a grade can be assigned to this item, false otherwise.
        /// </summary>
        [DataMember]
        public bool IsGradable { get; set; }

        /// <summary>
        /// True if a late submission is allowed for the assignment
        /// </summary>
        [DataMember]
        public bool IsAllowLateSubmission { get; set; }

        /// <summary>
        /// True if the item has rubric.
        /// </summary>
        [DataMember]
        public bool HasRubric { get; set; }

        /// <summary>
        /// URL to the rubrics resource.
        /// </summary>
        [DataMember]
        public string Rubric { get; set; }

        /// <summary>
        /// Id of the resource folder for this item.
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// True if the item should NOT appear in the table of contents, false otherwise (default).
        /// </summary>
        [DataMember]
        public bool HiddenFromToc { get; set; }

        /// <summary>
        /// True if the item should NOT appear for the students, false otherwise (default).
        /// </summary>
        [DataMember]
        public bool HiddenFromStudents { get; set; }

        /// <summary>
        /// Available date for the item.
        /// </summary>
        public DateTime AvailableDate { get; set; }

        /// <summary>
        /// URL that points to an internal or external resource.
        /// </summary>
        [DataMember]
        public string Href { get; set; }

        /// <summary>
        /// Order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// When true, this item sets its own score in the gradebook.
        /// </summary>
        [DataMember]
        public bool Sco { get; set; }

        /// <summary>
        /// Order in which this item should be displayed relative to other items within assigned category.
        /// </summary>
        [DataMember]
        public string CategorySequence { get; set; }

        /// <summary>
        /// Contains any of the item's data.
        /// See http://dev.dlap.bfwpub.com/Docs/Schema/ItemData
        /// </summary>
        private string _dataString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }
        }

        [NonSerialized]
        private XElement _data;

        /// <summary>
        /// XML item data read from agilix item retrieval.
        /// </summary>
        [DataMember]
        public XElement Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Any children of the item.
        /// </summary>
        [DataMember]
        public List<Item> Children { get; set; }

        /// <summary>
        /// Thumbnail for Project Item Type
        /// </summary>
        [DataMember]
        public string Thumbnail { get; set; }

        /// <summary>
        /// A friendly ToString override for items.
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} [{1}]", Id, Title);
        }

        private DateTime _dueDate = DateTime.MinValue;
        /// <summary>
        /// Date the item is due.
        /// </summary>
        public DateTime DueDate
        {
            get
            {
                if (_dueDate.Year == DateTime.MinValue.Year && Data != null)
                {
                    var dElm = Data.Element("duedate");
                    if (dElm != null)
                    {
                        DateTime dt;
                        DateTime.TryParse(dElm.Value, out dt);

                        _dueDate = dt;
                    }
                }

                return _dueDate;
            }

            set
            {
                var dElm = Data.Element("duedate");
                if (dElm == null && value.Year != DateTime.MinValue.Year)
                {
                    dElm = new XElement("duedate");
                    //add 1 second to due date for this rule:
                    //The UTC due date and time for this item. If the seconds part of the time is zero, the time is shifted to the time zone of the student, otherwise the time is not shifted.
                    //2010-11-30T23:59:00Z - The item is due at the end of the day (for the student) of November 30th.
                    //2010-11-30T21:30:01Z - The item is due at 21:30 UTC which is 4:30 PM Eastern or 1:30 PM Pacific.
                    value  = value.AddSeconds(-1*value.Second).AddSeconds(1);
                    dElm.SetValue(DateRule.Format(value)); 
                    Data.Add(dElm);
                }
            }
        }

        private DateTime _gradeReleaseDate = DateTime.MinValue;
        /// <summary>
        /// The date and time when students can see their score for this item
        /// </summary>
        public DateTime GradeReleaseDate
        {
            get
            {
                if (_gradeReleaseDate.Year == DateTime.MinValue.Year && Data != null)
                {
                    var dElm = Data.Element("gradereleasedate");
                    if (dElm != null)
                    {
                        DateTime dt;
                        DateTime.TryParse(dElm.Value, out dt);

                        _gradeReleaseDate = dt;
                    }
                }

                return _gradeReleaseDate;
            }
        }

        /// <summary>
        /// This is primarly used for x-Book for right now,
        /// It will contain the Assignment Folder Id which is how it is linked as an assignment
        /// <meta-xbook-assignment-id>ID</meta-xbook-assignment-id>
        /// </summary>
        public string AssignmentFolderId { get; set; }

        /// <summary>
        /// True if item is assigned [even with no due date].
        /// </summary>
        public bool meta_bfw_Assigned { get; set; }


        /// <summary>
        /// Number of points the item is worth.
        /// </summary>
        public double MaxPoints { get; set; }


        /// <summary>
        /// Number of points the item is worth.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// category's percent grade
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// minimum score a student must achieve to pass
        /// </summary>
        public Double? PassingScore { get; set; }

        /// <summary>
        /// Property that determines whether the folder is a student created folder or not
        /// </summary>
        public bool IsStudentCreatedFolder { get; set; }

        /// <summary>
        /// Whether the item has a drop box.
        /// </summary>
        public bool DropBox { get; set; }

        /// <summary>
        /// The type of drop box.
        /// </summary>
        public DropBoxType DropBoxType
        {
            get
            {
                if (null != Data)
                {
                    var boxType = Data.Element("dropboxtype");
                    if (null != boxType)
                    {
                        int bt = 0;
                        if (int.TryParse(boxType.Value, out bt))
                        {
                            if (Enum.IsDefined(typeof (DlapDropboxType), bt))
                            {
                                return (DropBoxType) Enum.Parse(typeof (DlapDropboxType), boxType.Value);
                            }
                        }
                    }
                }

                return DropBoxType.SingleDocument;
            }
            set
            {
                AddOnCondition(Data, "dropboxtype", DropBox, (int)value);
            }
        }

        /// <summary>
        /// Category the item belongs to.
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// dropdown value of Include score in gradebook
        /// </summary>
        [DataMember]
        public int IncludeGbbScoreTrigger { get; set; }

        /// <summary>
        /// Determines settings defined in GradeFlags enum
        /// </summary>
        [DataMember]
        public GradeFlags GradeFlags { get; set; }

        /// <summary>
        /// Way in which the item is determined to be completed.
        /// </summary>
        [DataMember]
        public CompletionTrigger CompletionTrigger { get; set; }

        /// <summary>
        /// in case of CompletionTrigger == 0 (Minutes) 
        /// </summary>
        [DataMember]
        public int TimeToComplete { get; set; }

        [DataMember]
        public Boolean IsMarkAsCompleteChecked { get; set; }

        /// <summary>
        /// Owner id of the item
        /// </summary>
        [DataMember]
        public string OwnerId { get; set; }
        /// <summary>
        /// Attachments in item.
        /// </summary>
        [DataMember]
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Collection of learning objectives associated with the item.
        /// </summary>
        public List<ItemLearningObjective> LearningObjectives { get; set; }

        /// <summary>
        /// Gets or sets the related contents.
        /// </summary>
        /// <value>
        /// The related contents.
        /// </value>
        public List<ItemRelatedContent> RelatedContents { get; set; }

        private List<string> adjustedGroups;

        /// <summary>
        /// List of groupids for which item has been modified
        /// </summary>
        public List<string> AdjustedGroups 
        {
            get 
            {
                if (adjustedGroups == null)
                {
                    adjustedGroups = new List<string>();
                }

                return adjustedGroups;
            }
            set
            {
                adjustedGroups = value;
            }
        }

        public int AttemptLimit { get; set; }
        public GradeRule GradeRule { get; set; }
        public SubmissionGradeAction SubmissionGradeAction { get; set; }
        public int TimeLimit { get; set; }
        public QuestionDelivery QuestionsPerPage { get; set; }
        public bool AllowSaveAndContinue { get; set; }
        public bool AutoSubmitAssessments { get; set; }

        public bool ShuffleQuestions { get; set; }
        public bool ShuffleAnswers { get; set; }
        public bool AllowViewHints { get; set; }
        public int PercentSubstractHint { get; set; }

        public ReviewSetting ShowScoreAfter { get; set; }
        public ReviewSetting ShowQuestionsAnswers { get; set; }
        public ReviewSetting ShowRightWrong { get; set; }
        public ReviewSetting ShowAnswers { get; set; }
        public ReviewSetting ShowFeedbackAndRemarks { get; set; }
        public ReviewSetting ShowSolutions { get; set; }

        public bool StudentsCanEmailInstructors { get; set; }

        /// <summary>
        /// Course type to which the item is locked by.
        /// </summary>
        public string LockedCourseType { get; set; }

        /// <summary>
        /// Collection of related templates associated with the item.
        /// </summary>
        public List<ItemRelatedTemplate> RelatedTemplates { get; set; }

        /// <summary>
        /// Structure holding information about usage of the item on the proxy page
        /// </summary>
        public ProxyConfig ProxyConfig { get; set; }

        /// <summary>
        /// Collection of containers
        /// </summary>
        public List<Container> Containers { get; set; }
        /// <summary>
        /// Collection of subcontainers
        /// </summary>
        public List<Container> SubContainerIds { get; set; }

        /// <summary>
        /// The number of minutes after duedate wherein the student may still submit their work. 
        /// The value -1 indicates infinite grace period. The value 0 indicates no grace period. 
        /// The duedategrace attribute supercedes allowlatesubmission and if both are present, duedategrace takes precedence.
        /// </summary>
        public long DueDateGrace { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Item()
        {
            Data = new XElement(ElStrings.Data);
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public virtual void ParseEntity(XElement element)
        {
            if (null != element)
            {
                var root = element.Name;
                if (ElStrings.Item.LocalName != root)
                    throw new DlapEntityFormatException(string.Format("Expected an element of type 'item', but got an element of type '{0}'", element));

                var id = element.Attribute(AttributeStrings.Id);
                var resourceId = element.Attribute(AttributeStrings.ResourceId);
                var entityId = element.Attribute(AttributeStrings.EntityId);
                var actualentityId = element.Attribute(AttributeStrings.ActualEntityId);
                var resourceEntityId = element.Attribute(AttributeStrings.ResourceEntityId);
                var creationby = element.Attribute(AttributeStrings.CreationBy);
                var created = element.Attribute(AttributeStrings.CreationDate);
                var modified = element.Attribute(AttributeStrings.ModifiedDate);
                var data = element.Element(AttributeStrings.Data);

                if (null == id) id = element.Attribute(AttributeStrings.ItemId);

                if (null == id) throw new DlapEntityFormatException("Expected an attribute 'id' on the item, but none was found");

                Id = id.Value;

                if (null != resourceId)
                {
                    EntityId = resourceId.Value;
                }
                else if (null != entityId)
                {
                    EntityId = entityId.Value;
                }

                if (actualentityId != null) ActualEntityid = actualentityId.Value;
                
                
                if (null != resourceEntityId)  ResourceEntityId = resourceEntityId.Value;
                

                if (null != creationby) CreationBy = creationby.Value;
                
                if (null != created)
                {
                    DateTime createdDate;
                    if (DateTime.TryParse(created.Value, out createdDate)) Created = createdDate;
                }

                if (null != modified)
                {
                    DateTime modifiedDate;
                    if (DateTime.TryParse(modified.Value, out modifiedDate)) Modified = modifiedDate;
                }

				Data = data;

	            if (data != null) ParseData(data);
                                    
            }
        }

		private void ParseData(XElement data)
		{
			var parent = Data.Element(ElStrings.parent);
			var type = Data.Element(ElStrings.type);
			var subtype = Data.Element(ElStrings.bfw_type);
			var title = Data.Element(ElStrings.title);
			var subtitle = Data.Element(ElStrings.subtitle);
			var description = Data.Element(ElStrings.description);
			var gradable = Data.Element(ElStrings.gradable);
			var allowLateSubmission = Data.Element(ElStrings.allowLateSubmission);
			var rubric = Data.Element(ElStrings.rubric);
			var folder = Data.Element(ElStrings.folder);
			var href = Data.Element(ElStrings.href);
			var hftoc = Data.Element(ElStrings.hiddenfromtoc);
			var hfstudent = Data.Element(ElStrings.hiddenfromstudent);
			var availabledate = Data.Element(ElStrings.availabledate);
			var due = Data.Element(ElStrings.duedate);
			var meta_bfw_assigned = Data.Element(ElStrings.meta_bfw_assigned);
			var container = Data.Element(ElStrings.container);
			var subcontainerid = Data.Element(ElStrings.subcontainerid);
			var maxpoints = Data.Element(ElStrings.maxpoints);
			var weight = Data.Element(ElStrings.weight);
			var dropbox = Data.Element(ElStrings.dropbox);
			var category = Data.Element(ElStrings.category);
			var seq = Data.Element(ElStrings.sequence);
			var sco = Data.Element(ElStrings.sco);
			var attachments = Data.Element(ElStrings.attachments);
			var associatedTocItems = Data.Element(ElStrings.associatedTOCItems);
			var learningObjectives = Data.Element(ElStrings.learningObjectives);
			var relatedContents = Data.Element(ElStrings.bfw_related_contents);
			var attemptLimit = Data.Element(ElStrings.AttemptLimit);
			var gradeRule = Data.Element(ElStrings.GradeRule);
			var submissionGradeAction = Data.Element(ElStrings.SubmissionGradeAction);
			var timeLimit = Data.Element(ElStrings.TimeLimit);
			var examFlags = Data.Element(ElStrings.ExamFlags);
			var questionsPerPage = Data.Element(ElStrings.QuestionsPerPage);
			var allowViewHints = Data.Element(ElStrings.AllowViewHints);
			var percentSubstractHint = Data.Element(ElStrings.PercentSubstractHint);
			var reviewSettings = Data.Element(ElStrings.ExamReviewRules);
			var gradeReleaseDate = Data.Element(ElStrings.GradeReleaseDate);
			var studentsCanEmail = Data.Element(ElStrings.AllowStudentEmailInstructors);
			var lockedCourseType = Data.Element(ElStrings.bfw_locked);
			var relatedTemplates = Data.Element(ElStrings.relatedTemplates);
			var isDepreciated = Data.Element(ElStrings.IsDepreciated);
			var completionTrigger = Data.Element(ElStrings.CompletionTrigger);
			var timeToComplete = Data.Element(ElStrings.TimeToComplete);
			var categorySequence = Data.Element(ElStrings.CategorySequence) == null ? string.Empty : Data.Element(ElStrings.CategorySequence).Value;
			var containers = Data.Element(ElStrings.containers);
			var subcontainerids = Data.Element(ElStrings.metasubcontainerids);
			var assignmentFolderId = Data.Element(ElStrings.AssignmentFolderId);
			var ownerId = Data.Element(ElStrings.OwnerId);
			var gradeFlags = Data.Element(ElStrings.GradeFlags);
			var passingScore = Data.Element(ElStrings.PassingScore);

			ParseChildrenItems(data);
			ParseDueDate(due);
			ParseMeta_bfw_Assigned(meta_bfw_assigned);
			ParseMaxPoints(maxpoints);
			ParseWeight(weight);
			ParseDropBox(dropbox);
			ParseCategory(category);	
			type = ParseFolder(type);
			ParseItemType(type);
			ParseHiddenFromToc(hftoc);
			ParseHiddenFromStudents(hfstudent);
			ParseAvailableDate(availabledate);
			ParseIsGradable(gradable);
			ParseIsAllowLateSubmission(data);
			ParseHasRubric(rubric);
			ParseHrefDisciplineCourseId(href);
			ParseSco(sco);
			ParseAttachments(attachments);
			ParseMetaDataContainers(containers);
			ParseMetaDataSubContainers(subcontainerids);
			ParseAssociatedTocItemsAndAddToAttachments(associatedTocItems);
			ParseCustomExamType(data);
			ParseLearningObjectives(learningObjectives);
			ParseRelatedContents(relatedContents);
			ParseRelatedTemplates(relatedTemplates);
			ParseAttemptLimit(attemptLimit);
			ParseGradeRule(gradeRule);
			ParseSubmissionGradeAction(submissionGradeAction);
			ParseCompletionTrigger(completionTrigger);
			ParseTimeLimit(timeLimit);
			ParseQuestionsPerPage(questionsPerPage);
			ParseExamFlagsAndSetQuestionPerPageSettings(examFlags);
			ParseAllowViewHints(allowViewHints);
			ParsePercentSubstractHint(percentSubstractHint);
			ParseProxyConfig(data);
			ParseReviewSettings(reviewSettings, gradeReleaseDate);
			ParseStudentsCanEmailInstructors(studentsCanEmail);
            ParseGraceDueDate(data);
            ParseAdjustedGroups(data);
		
			if (null != lockedCourseType) LockedCourseType = lockedCourseType.Value;
			if (null != passingScore) PassingScore = Double.Parse(passingScore.Value);
			if (null != assignmentFolderId) AssignmentFolderId = assignmentFolderId.Value;
			if (null != ownerId) OwnerId = ownerId.Value;
			if (null != parent) ParentId = parent.Value;
			if (null != title) Title = title.Value;
			if (null != subtitle) SubTitle = subtitle.Value;
			if (null != description) Description = description.Value;
			if (null != folder) Folder = folder.Value;
			if (null != seq) Sequence = seq.Value;
			if (null != categorySequence) CategorySequence = categorySequence;
			if (null != timeToComplete) TimeToComplete = XmlConvert.ToTimeSpan(timeToComplete.Value).Minutes;
			if (null != subtype) SubType = subtype.Value;
		    if (null != gradeFlags) GradeFlags = (GradeFlags) Enum.Parse(typeof (GradeFlags), gradeFlags.Value);
		}

        private void ParseAdjustedGroups(XElement data)
        {
            var groups = data.Element(ElStrings.AdjustedGroups);

            if (null != groups)
            {
                AdjustedGroups = groups.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        private void ParseGraceDueDate(XElement data)
        {
            var grace = data.Element(ElStrings.duedategrace);
            
            if (null != grace)
            {
                long graceDueDate;
                long.TryParse(grace.Value, out graceDueDate);
                DueDateGrace = graceDueDate;
            }
        }

        private void ParseHiddenFromToc(XElement hftoc)
		{
			var hft = false;
			if (null != hftoc) bool.TryParse(hftoc.Value, out hft);

			HiddenFromToc = hft;
		}

		private void ParseHiddenFromStudents(XElement hfstudent)
		{
			// Parsing hidden from student. defaults to false -- g. chernyak 
			var hfs = false;
			if (hfstudent != null) bool.TryParse(hfstudent.Value, out hfs);

			HiddenFromStudents = hfs;
		}

		private void ParseReviewSettings(XElement reviewSettings, XElement gradeReleaseDate)
		{
			// Review Settings Section
			if (null != reviewSettings && reviewSettings.HasElements)
			{
				var showQuestionAnswersPoints =
					from setting in reviewSettings.Elements()
					where setting.Attribute(AttributeStrings.Setting).Value == ElStrings.Question.LocalName ||
						  setting.Attribute(AttributeStrings.Setting).Value == ElStrings.Answer.LocalName 
					select setting;


				var firstNode = showQuestionAnswersPoints.FirstOrDefault();
				var firstCondition = firstNode.Attribute(AttributeStrings.Condition).Value;

				if (null != firstNode)
				{
					foreach (var item in showQuestionAnswersPoints)
					{
						if (item.Attribute(AttributeStrings.Condition).Value != firstCondition)
						{
							ShowQuestionsAnswers = ReviewSetting.Each;
							break;
						}
						ShowQuestionsAnswers = DlapToReviewSetting(firstCondition);
					}
				}
				else
				{
					ShowQuestionsAnswers = ReviewSetting.Each;
				}


				var showRightWrong = reviewSettings.Elements().FirstOrDefault(x => x.Attribute(AttributeStrings.Setting).Value == ElStrings.CorrectQuestion.LocalName);

				ShowRightWrong = null != showRightWrong ? DlapToReviewSetting(showRightWrong.Attribute(AttributeStrings.Condition).Value) : ReviewSetting.Each;

				var showAnswers = reviewSettings.Elements().FirstOrDefault(x => x.Attribute(AttributeStrings.Setting).Value == ElStrings.CorrectChoice.LocalName);

				ShowAnswers = null != showAnswers ? DlapToReviewSetting(showAnswers.Attribute(AttributeStrings.Condition).Value) : ReviewSetting.Each;

                var showScoreAfter = reviewSettings.Elements().FirstOrDefault(x => x.Attribute(AttributeStrings.Setting).Value == ElStrings.Possible.LocalName);

                ShowScoreAfter = null != showScoreAfter ? DlapToReviewSetting(showScoreAfter.Attribute(AttributeStrings.Condition).Value) : ReviewSetting.Each;

				var showFeedback = reviewSettings.Elements().FirstOrDefault(x => x.Attribute(AttributeStrings.Setting).Value == ElStrings.Feedback.LocalName);

				ShowFeedbackAndRemarks = null != showFeedback ? DlapToReviewSetting(showFeedback.Attribute(AttributeStrings.Condition).Value) : ReviewSetting.Each;

				var showSolutions = reviewSettings.Elements().FirstOrDefault(x => x.Attribute(AttributeStrings.Setting).Value == ElStrings.Feedback_GROUP.LocalName);

				ShowSolutions = null != showSolutions ? DlapToReviewSetting(showSolutions.Attribute(AttributeStrings.Condition).Value) : ReviewSetting.Each;
			}
		}

		private void ParseStudentsCanEmailInstructors(XElement studentsCanEmail)
		{
			if (null != studentsCanEmail)
			{
				bool studentsCanEmailInstructors;
				bool.TryParse(studentsCanEmail.Value, out studentsCanEmailInstructors);

				StudentsCanEmailInstructors = studentsCanEmailInstructors;
			}
			else
			{
				StudentsCanEmailInstructors = true;
			}
		}

		private void ParseProxyConfig(XElement data)
		{
			var proxyConfig = data.Element(ElStrings.proxyConfig);
			if (proxyConfig != null)
			{
				ProxyConfig = new DataContracts.ProxyConfig();

				var allowComments = proxyConfig.Element(ElStrings.allowComments);
				if (allowComments != null)
				{
					bool tempAllowComments;
					if (bool.TryParse(allowComments.Value, out tempAllowComments))
					{
						ProxyConfig.AllowComments = tempAllowComments;
					}
				}
			}
		}

		private void ParsePercentSubstractHint(XElement percentSubstractHint)
		{
			if (null != percentSubstractHint)
			{
				int psh;
				int.TryParse(percentSubstractHint.Value, out psh);

				PercentSubstractHint = psh;
			}
		}

		private void ParseAllowViewHints(XElement allowViewHints)
		{
			if (null != allowViewHints)
			{
				bool avh;
				bool.TryParse(allowViewHints.Value, out avh);

				AllowViewHints = avh;
			}
		}

		private void ParseExamFlagsAndSetQuestionPerPageSettings(XElement examFlags)
		{

			// Check if the ForwardNavigation flag is set
			// in which case it overrides the questionperpage setting
			AllowSaveAndContinue = true;
			ShuffleAnswers = true;
			ShowFeedbackAndRemarks = ReviewSetting.Each;
			ShowSolutions = ReviewSetting.Each;

			if (null != examFlags)
			{
				int ef;
				int.TryParse(examFlags.Value, out ef);
				var nbt = ef & (int)ExamFlags.ForwardNavigation;

				if (nbt == (int)ExamFlags.ForwardNavigation)
				{
					QuestionsPerPage = QuestionDelivery.OneNoBacktrack;
				}

				var asc = ef & (int)ExamFlags.AllowSaveAndContinue;
				AllowSaveAndContinue = asc == (int)ExamFlags.AllowSaveAndContinue;

				var rqo = ef & (int)ExamFlags.ShuffleQuestions;
				ShuffleQuestions = rqo == (int)ExamFlags.ShuffleQuestions;

				var rao = ef & (int)ExamFlags.ShuffleAnswers;
				ShuffleAnswers = rao == (int)ExamFlags.ShuffleAnswers;
			}
		}

		private void ParseQuestionsPerPage(XElement questionsPerPage)
		{
			if (null != questionsPerPage)
			{
				int qpp;
				int.TryParse(questionsPerPage.Value, out qpp);
				QuestionsPerPage = (QuestionDelivery)qpp;
			}
		}

		private void ParseTimeLimit(XElement timeLimit)
		{
			if (null != timeLimit)
			{
				int tl;
				int.TryParse(timeLimit.Value, out tl);
				TimeLimit = tl;
			}
		}

		private void ParseCompletionTrigger(XElement completionTrigger)
		{
			if (null != completionTrigger)
			{
				int gr;
				int.TryParse(completionTrigger.Value, out gr);
				CompletionTrigger = (CompletionTrigger)gr;
			}
			else
			{
				// Default value
				CompletionTrigger = CompletionTrigger.Submission;
			}
		}

		private void ParseSubmissionGradeAction(XElement submissionGradeAction)
		{
			if (null != submissionGradeAction)
			{
				int gr;
				int.TryParse(submissionGradeAction.Value, out gr);
				SubmissionGradeAction = (SubmissionGradeAction)gr;
			}
			else
			{
				// Default value
				SubmissionGradeAction = SubmissionGradeAction.NotSet;
			}
		}

		private void ParseGradeRule(XElement gradeRule)
		{
			if (null != gradeRule)
			{
				int gr;
				int.TryParse(gradeRule.Value, out gr);
				GradeRule = (GradeRule)gr;
			}
			else
			{
				// Default value
				GradeRule = GradeRule.Last;
			}
		}

		private void ParseAttemptLimit(XElement attemptLimit)
		{
			if (null != attemptLimit)
			{
				int al;
				int.TryParse(attemptLimit.Value, out al);
				AttemptLimit = al;
			}
			else
			{
				// Default value
				AttemptLimit = 3;
			}
		}

		private void ParseCustomExamType(XElement data)
		{
			var customexamtype = data.Element(ElStrings.customexamtype);
			if (customexamtype != null) CustomExamType = customexamtype.Value;
		}

		private void ParseRelatedTemplates(XElement relatedTemplates)
		{
			if (null != relatedTemplates)
			{
				RelatedTemplates = new List<ItemRelatedTemplate>();

				foreach (var template in relatedTemplates.Elements(ElStrings.Template))
				{
					var newTemplate = new ItemRelatedTemplate();
					newTemplate.ParseEntity(template);
					RelatedTemplates.Add(newTemplate);
				}
			}
		}

		private void ParseRelatedContents(XElement relatedContents)
		{
			if (null != relatedContents)
			{
				var bizRelatedContents = new List<ItemRelatedContent>();
				foreach (var content in relatedContents.Elements(ElStrings.Item))
				{
					var relatedContent = new ItemRelatedContent();
					relatedContent.ParseEntity(content);

					bizRelatedContents.Add(relatedContent);
				}

				RelatedContents = bizRelatedContents;
			}
		}

		private void ParseLearningObjectives(XElement learningObjectives)
		{
			if (null != learningObjectives)
			{
				var bizLearningObjectives = new List<ItemLearningObjective>();
				foreach (var objective in learningObjectives.Elements(ElStrings.Objective))
				{
					var learningObjective = new ItemLearningObjective();
					learningObjective.ParseEntity(objective);

					bizLearningObjectives.Add(learningObjective);
				}

				LearningObjectives = bizLearningObjectives;
			}
		}

		private void ParseAssociatedTocItemsAndAddToAttachments(XElement associatedTocItems)
		{
			if (null != associatedTocItems)
			{
				foreach (var attachment in associatedTocItems.Descendants(ElStrings.TocItem))
				{
					if (Attachments == null) Attachments = new List<Attachment>();
					Attachments.Add(new Attachment { Href = attachment.Attribute(AttributeStrings.Href).Value });
				}
			}
		}

		private void ParseAttachments(XElement attachments)
		{
			if (null != attachments)
			{
				foreach (var attachment in attachments.Descendants(ElStrings.Attachment))
				{
					if (Attachments == null) Attachments = new List<Attachment>();
					Attachments.Add(new Attachment { Href = attachment.Attribute(AttributeStrings.Href).Value });
				}
			}
		}

		private void ParseSco(XElement sco)
		{
			var sCo = false;
			if (null != sco) bool.TryParse(sco.Value, out sCo);

			Sco = sCo;
		}

		private void ParseHrefDisciplineCourseId(XElement href)
		{
			if (null != href)
			{
				Href = href.Value;
				if (href.HasAttributes) HrefDisciplineCourseId = href.Attribute("entityid").Value ?? "";
			}
		}

		private void ParseHasRubric(XElement rubric)
		{
			HasRubric = false;
			if (null != rubric)
			{
				Rubric = rubric.Value;
				HasRubric = true;
			}
		}

		private void ParseIsAllowLateSubmission(XElement data)
		{
            //If allowlatesubmission != null, read its value. Otherwise, read "duedategrace"
            if (data.Element(ElStrings.allowLateSubmission) != null)
            {
                bool allowlatesubmission;
                bool.TryParse(data.Element(ElStrings.allowLateSubmission).Value, out allowlatesubmission);
                IsAllowLateSubmission = allowlatesubmission;
            }
            else if (data.Element(ElStrings.duedategrace) != null)
            {
                long duedategrace;
                long.TryParse(data.Element(ElStrings.duedategrace).Value, out duedategrace);
                IsAllowLateSubmission = duedategrace != 0;
            }
            else
            {
                IsAllowLateSubmission = false;
            }
		}

		private void ParseIsGradable(XElement gradable)
		{
			bool bg = false;
			if (null != gradable) bool.TryParse(gradable.Value, out bg);
			IsGradable = bg;
		}

		private void ParseAvailableDate(XElement availabledate)
		{
			// Parsing availabe date -- g. chernyak
			if (availabledate != null)
			{
				DateTime dd;
				DateTime.TryParse(availabledate.Value, out dd);
				AvailableDate = dd;
			}
		}

		private void ParseItemType(XElement type)
		{
			if (null != type)
			{
				var typeValue = type.Value;
				try
				{
					Type = (DlapItemType)Enum.Parse(typeof(DlapItemType), typeValue, true);
				}
				catch
				{
					CustomType = typeValue;
					Type = DlapItemType.Custom;
				}
			}
		}

		private XElement ParseFolder(XElement type)
		{
			if (null == type)
			{
				type = Data.Element(ElStrings.folder);
				if (null != type || Children.Count > 0)
				{
					type = new XElement(ElStrings.type);
					type.SetValue(ElStrings.folder);
				}
			}
			return type;
		}

		private void ParseCategory(XElement category)
		{
			if (null != category)
			{
				int cat;
				int.TryParse(category.Value, out cat);
				Category = cat.ToString();
			}
		}

		private void ParseDropBox(XElement dropbox)
		{
			if (null != dropbox)
			{
				bool db;
				bool.TryParse(dropbox.Value, out db);
				DropBox = db;
			}
		}

		private void ParseWeight(XElement weight)
		{
			if (null != weight)
			{
				double mp;
				double.TryParse(weight.Value, out mp);
				Weight = mp;
			}
		}

		private void ParseMaxPoints(XElement maxpoints)
		{
			if (null != maxpoints)
			{
				double mp;
				double.TryParse(maxpoints.Value, out mp);
				MaxPoints = mp;
			}
		}

		private void ParseMeta_bfw_Assigned(XElement meta_bfw_assigned)
		{
			if (null != meta_bfw_assigned)
			{
				bool meta_bfw_a;
				bool.TryParse(meta_bfw_assigned.Value, out meta_bfw_a);
				meta_bfw_Assigned = meta_bfw_a;
			}
		}

		private void ParseDueDate(XElement due)
		{
			if (null != due)
			{
				DateTime dd;
				DateTime.TryParse(due.Value, out dd);
				DueDate = dd;
			}
		}

		private void ParseChildrenItems(XElement data)
		{
			var children = data.ElementsAfterSelf(ElStrings.Item);
			{
				Children = new List<Item>();
				foreach (var childElm in children)
				{
					var child = new Item();
					child.ParseEntity(childElm);
					Children.Add(child);
				}
			}
		}

		private void ParseMetaDataSubContainers(XElement subcontainerids)
		{
			if (null == subcontainerids) return;
			foreach (var subcontainer in subcontainerids.Descendants(ElStrings.metasubcontainerid))
			{
				if (SubContainerIds == null) SubContainerIds = new List<Container>();
				var subcontainerObj = SubContainerIds.FirstOrDefault(i => i.Toc.ToLower() == subcontainer.Attribute(ElStrings.toc).Value);
				if (subcontainerObj == null)
				{
					if (subcontainer.Attribute(ElStrings.dlaptype) == null) subcontainer.Add(new XAttribute(ElStrings.dlaptype, "exact"));
					SubContainerIds.Add(new Container(subcontainer.Attribute(ElStrings.toc).Value, subcontainer.Value, subcontainer.Attribute(ElStrings.dlaptype).Value));
				}
				else
				{
					subcontainerObj.Value = subcontainer.Value;
				}

			}
		}

	    private void ParseMetaDataContainers(XElement containers)
		{
			if (null == containers) return;
			foreach (var innerContainer in containers.Descendants(ElStrings.container))
			{
				if (Containers == null) Containers = new List<Container>();
				if (Containers.Any(i => i.Toc.ToLower() == innerContainer.Attribute(ElStrings.toc).Value)) continue;
				if (innerContainer.Attribute(ElStrings.dlaptype) == null) innerContainer.Add(new XAttribute(ElStrings.dlaptype, "exact"));
				Containers.Add(new Container(innerContainer.Attribute(ElStrings.toc).Value, innerContainer.Value, innerContainer.Attribute(ElStrings.dlaptype).Value));
			}
		}

	    #endregion

        #region IDlapEntityTransformer Members

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public virtual XElement ToEntity()
        {
            var root = new XElement(ElStrings.Item);
            root.Add(new XAttribute(ElStrings.ItemId, Id));

            if (!string.IsNullOrEmpty(EntityId)) root.Add(new XAttribute(ElStrings.Entityid, EntityId));

            var data = Data ?? new XElement(ElStrings.Data);

			AddParentIdToXml(data);
			AddTitleToXml(data);
			AddSubTitleToXml(data);
			AddDescriptionToXml(data);
			AddCreationByToXml(data);
			AddIsGradableXml(data);

            AddOnCondition(data, ElStrings.IsMarkAsCompleteChecked.LocalName, true, IsMarkAsCompleteChecked);
            AddOnCondition(data, ElStrings.allowLateSubmission.LocalName, true, IsAllowLateSubmission);
            AddOnCondition(data, ElStrings.duedategrace.LocalName, true, DueDateGrace);
            AddOnCondition(data, ElStrings.rubric.LocalName, HasRubric, Rubric);
            AddOnCondition(data, ElStrings.dropbox.LocalName, DropBox, DropBox);
            AddOnCondition(data, ElStrings.DropBoxType.LocalName, DropBox, (int)DropBoxType);
            AddOnCondition(data, ElStrings.maxpoints.LocalName, true, MaxPoints);
            AddOnCondition(data, ElStrings.weight.LocalName, true, Weight);

			var dueDateIsValid = AddDueDateElementXml(data);
            AddOnCondition(data, ElStrings.duedate.LocalName, dueDateIsValid, DueDate);

			AddBfw_lockedXml(data);
			AddRubricXml(data);
			AddMaxPointsXml(data);
			AddPassingScoreXml(data);

            data.Add(new XElement(ElStrings.meta_bfw_assigned, meta_bfw_Assigned));

			AddCategoryXml(data);

			var trigger = AddCompletionTriggerXml(data);

			AddThumbnailXml(data);
            AddOnCondition(data, ElStrings.associatedtocourse.LocalName, true, "");
            AddOnCondition(data, ElStrings.TimeToComplete.LocalName, trigger == (int)CompletionTrigger.Minutes, XmlConvert.ToString(new TimeSpan(0, Convert.ToInt32(TimeToComplete), 0)));
			AddHiddenFromTocToXml(data);
			AddHiddenFromStudentsToXml(data);
			AddAvailableDateXml(data);
			AddFolderXml(data);
			AddHrefXml(data);
			AddTypeXml(data);
			AddScoXml(data);
			AddSequenceXml(data);
			AddCategorySequence(data);
			AddMetaContainersXml(data);
			AddMetaSubcontainersXml(data);
			AddAttahcmentsXml(data);
			AddLearningObjectivesXml(data);
			AddRelatedContentsXml(data);
			GetIncludeGbbScoreTrigger(data);

            AddOnCondition(data, ElStrings.GradeReleaseDate.LocalName, true, 
                IncludeGbbScoreTrigger == 2 ? DueDate : DateTime.Now.ToUniversalTime());

            AddAdjustedGroups(data);
			AddGradeFlagsXml(data);
			AddGradeRuleXml(data);
			AddSubmissionGradeActionXml(data);
			AddQuizHomeworkSettingsXml(data);
			AddAssignmentFolderIdToXml(data);
            root.Add(data);
			AddChildrenItemsToXml(root);
            return root;
        }

        private void AddAdjustedGroups(XElement data)
        {
            if (AdjustedGroups.Count > 0)
            {
                var groups = string.Join(",", AdjustedGroups.ToArray());
                data.Add(new XElement(ElStrings.AdjustedGroups, groups));
            }
            else
            {
                data.Add(new XElement(ElStrings.AdjustedGroups));
            }
        }

		private void AddIsGradableXml(XElement data)
		{
			if (Category == "-1") IsGradable = false;
			AddOnCondition(data, ElStrings.gradable.LocalName, true, IsGradable);
		}

		private void AddQuizHomeworkSettingsXml(XElement data)
		{
			// Quiz/Homework settings section
			if (Type != DlapItemType.Assessment && Type != DlapItemType.Homework) return;

			AddAttemptLimitXml(data);
			AddGradeRuleXml(data);
			AddTimeLimitXml(data);
			AddExamFlagsXml(data);
			AddAllowViewHintsXml(data);
			AddPercentSubstractHintXml(data);
			AddHomeworkReviewSettingsXml(data);
		}

		private void AddExamFlagsXml(XElement data)
		{
			int examFlagsBitmask;
			XElement examFlags;
			GetExamFlagsBitmask(data, out examFlagsBitmask, out examFlags);

			examFlagsBitmask = AddQuestionsPerPageXml(data, examFlagsBitmask);

			if (AllowSaveAndContinue)
			{
				examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.AllowSaveAndContinue;
			}
			else
			{
				examFlagsBitmask = examFlagsBitmask & ~(int)ExamFlags.AllowSaveAndContinue;
			}

			if (ShuffleQuestions)
			{
				examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.ShuffleQuestions;
			}
			else
			{
				examFlagsBitmask = examFlagsBitmask & ~(int)ExamFlags.ShuffleQuestions;
			}

			if (ShuffleAnswers)
			{
				examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.ShuffleAnswers;
			}
			else
			{
				examFlagsBitmask = examFlagsBitmask & ~(int)ExamFlags.ShuffleAnswers;
			}

			examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.ShowFeedback;
			examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.ShowCorrectChoice;

			// Add the exam flags
			examFlags.SetValue(examFlagsBitmask);
			data.Add(examFlags);
		}

		internal void AddHomeworkReviewSettingsXml(XElement data)
		{
			// Review Settings Section
		    var reviewSettings = data.Element(ElStrings.ExamReviewRules) ?? new XElement(ElStrings.ExamReviewRules);

			XElement newSetting = new XElement(ElStrings.Rule);
			newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.Question.LocalName);
			newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowQuestionsAnswers));
			reviewSettings.Add(newSetting);

			newSetting = new XElement(ElStrings.Rule);
			newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.Answer.LocalName);
			newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowQuestionsAnswers));
			reviewSettings.Add(newSetting);

            var showScoreAfterElment = reviewSettings.Descendants(ElStrings.Possible.LocalName).FirstOrDefault();
		    if (null == showScoreAfterElment)
		    {
		        showScoreAfterElment = new XElement(ElStrings.Rule);
		        showScoreAfterElment.SetAttributeValue(ElStrings.Setting, ElStrings.Possible.LocalName);
		        showScoreAfterElment.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowScoreAfter));
		        reviewSettings.Add(showScoreAfterElment);
		    }
		    else
		    {
                showScoreAfterElment.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowScoreAfter));
		    }

			var showCorrectQuestion =
				reviewSettings.Elements()
							  .SingleOrDefault(x => x.Attribute(ElStrings.Setting).ToString() == ElStrings.CorrectQuestion.LocalName);

			if (null != showCorrectQuestion)
			{
				showCorrectQuestion.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowRightWrong));
			}
			else
			{
				newSetting = new XElement(ElStrings.Rule);
				newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.CorrectQuestion.LocalName);
				newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowRightWrong));
				reviewSettings.Add(newSetting);
			}

			var showAnswers =
				reviewSettings.Elements()
							  .SingleOrDefault(x => x.Attribute(ElStrings.Setting).ToString() == ElStrings.CorrectChoice.LocalName);

			if (null != showAnswers)
			{
				showAnswers.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowAnswers));
			}
			else
			{
				newSetting = new XElement(ElStrings.Rule);
				newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.CorrectChoice.LocalName);
				newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowAnswers));
				reviewSettings.Add(newSetting);
			}

			var showFeedback =
				reviewSettings.Elements()
							  .SingleOrDefault(x => x.Attribute(ElStrings.Setting).ToString() == ElStrings.Feedback.LocalName);

			if (null != showFeedback)
			{
				showFeedback.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowFeedbackAndRemarks));
			}
			else
			{
				newSetting = new XElement(ElStrings.Rule);
				newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.Feedback.LocalName);
				newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowFeedbackAndRemarks));
				reviewSettings.Add(newSetting);
			}

			var showSolutions =
				reviewSettings.Elements()
							  .SingleOrDefault(x => x.Attribute(ElStrings.Setting).ToString() == ElStrings.Feedback_GROUP.LocalName);

			if (null != showSolutions)
			{
				showSolutions.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowSolutions));
			}
			else
			{
				newSetting = new XElement(ElStrings.Rule);
				newSetting.SetAttributeValue(ElStrings.Setting, ElStrings.Feedback_GROUP.LocalName);
				newSetting.SetAttributeValue(ElStrings.Condition, ReviewSettingToDlap(ShowSolutions));
				reviewSettings.Add(newSetting);
			}

			// If there are review settings changes, add them to the data
			if (reviewSettings.HasElements)
			{
				data.Add(reviewSettings);
			}

			var allowStudentsEmail = data.Element(ElStrings.AllowStudentEmailInstructors);
			if (null != allowStudentsEmail)
			{
				allowStudentsEmail.SetValue(StudentsCanEmailInstructors);
			}
			else
			{
				newSetting = new XElement(ElStrings.AllowStudentEmailInstructors);
				newSetting.SetValue(StudentsCanEmailInstructors);
				data.Add(newSetting);
			}
		}

		private void AddPercentSubstractHintXml(XElement data)
		{
			if (PercentSubstractHint >= 0)
			{
				var percentSubstractHint = data.Element(ElStrings.PercentSubstractHint);
				if (null != percentSubstractHint)
				{
					percentSubstractHint.SetValue(PercentSubstractHint);
				}
				else
				{
					data.Add(new XElement(ElStrings.PercentSubstractHint, PercentSubstractHint));
				}
			}
		}

		private void AddAllowViewHintsXml(XElement data)
		{
			var allowViewHints = data.Element(ElStrings.AllowViewHints);
			if (null != allowViewHints)
			{
				allowViewHints.SetValue(AllowViewHints);
			}
			else
			{
				data.Add(new XElement(ElStrings.AllowViewHints, AllowViewHints));
			}
		}

		private static void GetExamFlagsBitmask(XElement data, out int examFlagsBitmask, out XElement examFlags)
		{

			examFlags = data.Element(ElStrings.ExamFlags);
			if (null == examFlags)
			{
				examFlags = new XElement(ElStrings.ExamFlags);
				examFlagsBitmask = 0;
			}
			else
			{
				int.TryParse(examFlags.Value, out examFlagsBitmask);
			}
		}

		private int AddQuestionsPerPageXml(XElement data, int examFlagsBitmask)
		{
			if (QuestionsPerPage >= 0)
			{
				if (QuestionsPerPage == QuestionDelivery.All || QuestionsPerPage == QuestionDelivery.One)
				{
					var questionsPerPage = data.Element(ElStrings.QuestionsPerPage);
					if (null != questionsPerPage)
					{
						questionsPerPage.SetValue((int)QuestionsPerPage);
					}
					else
					{
						data.Add(new XElement(ElStrings.QuestionsPerPage, (int)QuestionsPerPage));
					}
				}
				else
				{
					examFlagsBitmask = examFlagsBitmask | (int)ExamFlags.ForwardNavigation;
				}
			}
			return examFlagsBitmask;
		}

		private void AddTimeLimitXml(XElement data)
		{

			if (TimeLimit >= 0)
			{
				var timeLimit = data.Element(ElStrings.TimeLimit);
				if (null != timeLimit)
				{
					timeLimit.SetValue(TimeLimit);
				}
				else
				{
					data.Add(new XElement(ElStrings.TimeLimit, TimeLimit));
				}
			}
		}

		private void AddAttemptLimitXml(XElement data)
		{
			if (AttemptLimit >= 0)
			{
				var attemptLimit = data.Element(ElStrings.AttemptLimit);
				if (null != attemptLimit)
				{
					attemptLimit.SetValue(AttemptLimit);
				}
				else
				{
					data.Add(new XElement(ElStrings.AttemptLimit, AttemptLimit));
				}
			}
		}

		private void AddCategorySequence(XElement data)
		{
			if (CategorySequence != null)
			{
				var categorySequence = data.Element(ElStrings.CategorySequence);

				if (null == categorySequence)
				{
					categorySequence = new XElement(ElStrings.CategorySequence);
					data.Add(categorySequence);
				}

				categorySequence.SetValue(CategorySequence);
			}
		}

		private void AddSequenceXml(XElement data)
		{
			if (!string.IsNullOrEmpty(Sequence))
			{
				var seq = data.Element(ElStrings.sequence);
				if (null == seq)
				{
					seq = new XElement(ElStrings.sequence);
					data.Add(seq);
				}
				seq.SetValue(Sequence);
			}
		}

		private void AddScoXml(XElement data)
		{
			var sco = data.Element(ElStrings.sco);
			if (null == sco)
			{
				sco = new XElement(ElStrings.sco);
				data.Add(sco);
			}
			sco.SetValue(Sco);
		}

		private void AddTypeXml(XElement data)
		{
			if (DlapItemType.None != Type)
			{
				var type = data.Element(ElStrings.type);

				var typeValue = DlapItemType.Custom == Type ? CustomType : Enum.GetName(typeof(DlapItemType), Type);

				if (null != type)
				{
					type.SetValue(typeValue);
				}
				else
				{
					data.Add(new XElement(ElStrings.type, typeValue));
				}
			}
		}

		private void AddHrefXml(XElement data)
		{
			if (!string.IsNullOrEmpty(Href))
			{
				var hr = data.Element(ElStrings.href);
				if (null == hr)
				{
					hr = new XElement(ElStrings.href);
					data.Add(hr);
				}

				hr.SetAttributeValue("entityid", HrefDisciplineCourseId);

				hr.SetValue(Href);
			}
		}

		private void AddFolderXml(XElement data)
		{
			if (!string.IsNullOrEmpty(Folder))
			{
				var f = data.Element(ElStrings.folder);
				if (null == f)
				{
					f = new XElement(ElStrings.folder);
					data.Add(f);
				}

				f.SetValue(Folder);
			}
		}

		private void AddAvailableDateXml(XElement data)
		{
			// Somewhat validating new available date -- g. chernyak
			var availableElement = data.Element(ElStrings.availabledate);
			if (null == availableElement)
			{
				availableElement = new XElement(ElStrings.availabledate);
				data.Add(availableElement);
			}

			availableElement.SetValue(AvailableDate);
		}

		private void AddHiddenFromStudentsToXml(XElement data)
		{
			// Adding new student visibility parameter -- g. chernyak
			var hfstudent = data.Element(ElStrings.hiddenfromstudent);
			if (null == hfstudent)
			{
				hfstudent = new XElement(ElStrings.hiddenfromstudent);
				data.Add(hfstudent);
			}
			hfstudent.SetValue(HiddenFromStudents);
		}

		private void AddHiddenFromTocToXml(XElement data)
		{
			var hftoc = data.Element(ElStrings.hiddenfromtoc);
			if (null == hftoc)
			{
				hftoc = new XElement(ElStrings.hiddenfromtoc);
				data.Add(hftoc);
			}

			hftoc.SetValue(HiddenFromToc);
		}

		private void AddThumbnailXml(XElement data)
		{
			var thumbnailpath = this.Thumbnail;
			AddOnCondition(data, ElStrings.thumbnail.LocalName, true, thumbnailpath);
		}

		private int AddCompletionTriggerXml(XElement data)
		{
			var trigger = (int)CompletionTrigger;
			AddOnCondition(data, ElStrings.CompletionTrigger.LocalName, true, trigger);
			return trigger;
		}

		private void AddCategoryXml(XElement data)
		{
			int cat;
			int.TryParse(Category, out cat);
			AddOnCondition(data, ElStrings.category.LocalName, true, cat);
		}

		private void AddPassingScoreXml(XElement data)
		{
			if (PassingScore.HasValue)
			{
				var passingScore = data.Element(ElStrings.PassingScore);

				if (passingScore == null)
				{
					passingScore = new XElement(ElStrings.PassingScore);
					data.Add(passingScore);
				}

				passingScore.SetValue(PassingScore.Value);
			}
		}

		private void AddMaxPointsXml(XElement data)
		{
			if (MaxPoints <= 0)
			{
				var maxPointsElement = data.Element(ElStrings.maxpoints);

				if (maxPointsElement != null)
				{
					maxPointsElement.Remove();
					maxPointsElement = null;
				}
				maxPointsElement = new XElement(ElStrings.maxpoints);
				data.Add(maxPointsElement);
			}
		}

		private void AddRubricXml(XElement data)
		{
			if (string.IsNullOrEmpty(Rubric))
			{
				var rubricElement = data.Element(ElStrings.rubric);

				if (rubricElement != null)
				{
					rubricElement.Remove();
					rubricElement = null;
				}
				rubricElement = new XElement(ElStrings.rubric);
				data.Add(rubricElement);
			}
		}

		private void AddBfw_lockedXml(XElement data)
		{
			if (!string.IsNullOrEmpty(LockedCourseType))
			{
				var bfw_locked = data.Element(ElStrings.bfw_locked);
				if (null != bfw_locked)
				{
					bfw_locked.SetValue(LockedCourseType);
				}
				else
				{
					data.Add(new XElement(ElStrings.bfw_locked, LockedCourseType));
				}
			}
		}

		private bool AddDueDateElementXml(XElement data)
		{
			var dueDateIsValid = ( DueDate.Year != DateTime.MinValue.Year );
			if (!dueDateIsValid)
			{
				var dueDateElement = data.Element(ElStrings.duedate);

				if (dueDateElement != null)
				{
					dueDateElement.Remove();
					dueDateElement = null;
				}

				dueDateElement = new XElement(ElStrings.duedate);
				data.Add(dueDateElement);
			}
			return dueDateIsValid;
		}

		private void AddCreationByToXml(XElement data)
		{
			if (!string.IsNullOrEmpty(CreationBy))
			{
				var creationby = data.Element(ElStrings.CreationBy);
				if (null != creationby)
				{
					creationby.SetValue(CreationBy);
				}
				else
				{
					data.Add(new XElement(ElStrings.CreationBy, CreationBy));
				}
			}
		}

		private void AddDescriptionToXml(XElement data)
		{
			if (!String.IsNullOrEmpty(Description))
			{
				var description = data.Element(ElStrings.description);
				if (null != description)
				{
					description.SetValue(Description);
				}
				else
				{
					data.Add(new XElement(ElStrings.description, Description));
				}
			}
		}

		private void AddSubTitleToXml(XElement data)
		{
			if (!string.IsNullOrEmpty(Title))
			{
				var subtitle = data.Element(ElStrings.subtitle);
				if (null != subtitle)
				{
					subtitle.SetValue(SubTitle);
				}
				else
				{
					data.Add(new XElement(ElStrings.subtitle, SubTitle));
				}
			}
		}

		private void AddTitleToXml(XElement data)
		{
			if (!string.IsNullOrEmpty(Title))
			{
				var title = data.Element(ElStrings.title);
				if (null != title)
				{
					title.SetValue(Title);
				}
				else
				{
					data.Add(new XElement(ElStrings.title, Title));
				}
			}
		}

		private void AddParentIdToXml(XElement data)
		{
			if (!string.IsNullOrEmpty(ParentId))
			{
				var parent = data.Element(ElStrings.parent);
				if (null != parent)
				{
					parent.SetValue(ParentId);
				}
				else
				{
					data.Add(new XElement(ElStrings.parent, ParentId));
				}
			}
		}

		private void AddChildrenItemsToXml(XElement root)
		{
			if (null != Children)
			{
				foreach (var child in Children)
				{
					root.Add(child.ToEntity());
				}
			}
		}

		private void AddAssignmentFolderIdToXml(XElement data)
		{
			// assignment folder ids
			if (AssignmentFolderId != null)
			{
				var assignmentFolder = new XElement(ElStrings.AssignmentFolderId, AssignmentFolderId);

				data.Add(assignmentFolder);
			}
		}

		private void AddSubmissionGradeActionXml(XElement data)
		{
			var submissiongradeaction = data.Element(ElStrings.SubmissionGradeAction);
			if (null != submissiongradeaction)
			{
				submissiongradeaction.SetValue((int)submissiongradeaction);
			}
			else
			{
				data.Add(new XElement(ElStrings.SubmissionGradeAction, (int)SubmissionGradeAction));
			}
		}

		private void AddGradeRuleXml(XElement data)
		{
			// if type is assingment add GradeRule integer value
			if (Type != DlapItemType.Assignment && Type != DlapItemType.Assessment && Type != DlapItemType.Homework) return;
			if (GradeRule < 0) return;
			var gradeRule = data.Element(ElStrings.GradeRule);
			if (null != gradeRule)
			{
				gradeRule.SetValue((int)GradeRule);
			}
			else
			{
				data.Add(new XElement(ElStrings.GradeRule, (int)GradeRule));
			}
		}

	    private void AddGradeFlagsXml(XElement data)
		{
			var gradeFlags = data.Element(ElStrings.GradeFlags);

			if (null == gradeFlags)
			{
				gradeFlags = new XElement(ElStrings.GradeFlags);
				data.Add(gradeFlags);
			}
			gradeFlags.SetValue((byte)GradeFlags);
		}

        private void GetIncludeGbbScoreTrigger(XElement data)
		{
			//// check Include score in GBB dropdown value
			var properties = data.Elements(ElStrings.bfw_properties);
			if (null != properties)
			{
				foreach (var el in properties.Elements(ElStrings.Bfw_Property))
				{
					if ((string)el.Attribute(ElStrings.Name) == ElStrings.Bfw_IncludeGbbScoreTrigger.LocalName)
					{
						IncludeGbbScoreTrigger = Convert.ToInt16(el.Value);
					}
				}
			}
		}

		private void AddRelatedContentsXml(XElement data)
		{
			if (RelatedContents != null)
			{
				var relatedContents = new XElement(ElStrings.bfw_related_contents);
				foreach (var content in RelatedContents)
				{
					if (!string.IsNullOrEmpty(content.Id) && !string.IsNullOrEmpty(content.Type))
					{
						var xItem = new XElement(ElStrings.Item, new XAttribute(ElStrings.Id, content.Id));
						xItem.Add(new XAttribute(ElStrings.type, content.Type));

						if (!string.IsNullOrEmpty(content.ParentId))
							xItem.Add(new XAttribute(ElStrings.ParentId, content.ParentId));

						relatedContents.Add(xItem);

					}
					else if (content.Id != null)
						relatedContents.Add(new XElement(ElStrings.Item, new XAttribute(ElStrings.Id, content.Id)));
				}
				data.Add(relatedContents);
			}
		}

		private void AddLearningObjectivesXml(XElement data)
		{
			if (LearningObjectives != null)
			{
				var learningObjectives = new XElement(ElStrings.learningObjectives);
				foreach (var objective in LearningObjectives.Where(objective => objective.Guid != null))
				{
					learningObjectives.Add(new XElement(ElStrings.Objective, new XAttribute(ElStrings.Guid, objective.Guid)));
				}
				data.Add(learningObjectives);
			}
		}

		private void AddAttahcmentsXml(XElement data)
		{
			if (Attachments != null && Attachments.Count > 0)
			{
				var attachments = new XElement(ElStrings.attachments);
				foreach (var attachment in Attachments)
				{
					if (attachment.Href != null)
						attachments.Add(new XElement(ElStrings.Attachment, new XAttribute(ElStrings.href, attachment.Href)));
				}
				data.Add(attachments);
			}
		}

		private void AddMetaSubcontainersXml(XElement data)
		{
			if (this.SubContainerIds != null)
			{
				var subContainers = data.Element(ElStrings.metasubcontainerids);

				if (subContainers == null)
				{
					subContainers = new XElement(( ElStrings.metasubcontainerids ));
					foreach (var container in this.SubContainerIds)
					{
						var subContainer = new XElement(( ElStrings.subcontainerid ));
						subContainer.Add(new XAttribute(ElStrings.toc, container.Toc));

						//Add dlaptype="exact" to subcontainer
						subContainer.Add(new XAttribute(ElStrings.dlaptype, "exact"));

						subContainer.SetValue(container.Value);
						subContainers.Add(subContainer);
					}
				}

				data.Add(subContainers);
			}
		}

		private void AddMetaContainersXml(XElement data)
		{
			if (this.Containers != null)
			{
				var metaContainers = data.Element(ElStrings.containers);
				if (metaContainers == null)
				{
					metaContainers = new XElement(( ElStrings.containers ));
					foreach (var container in this.Containers)
					{
						var metaContainer = new XElement(( ElStrings.container ));
						metaContainer.Add(new XAttribute(ElStrings.toc, container.Toc));
						metaContainer.SetValue(container.Value);

						//Add dlaptype="exact" to metaContainer
						metaContainer.Add(new XAttribute(ElStrings.dlaptype, "exact"));

						metaContainers.Add(metaContainer);
					}
				}

				data.Add(metaContainers);
			}
		}

        /// <summary>
        /// If a given condition is true, then add this property, otherwise
        /// don't add it (but change it if it already exists).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="elementName"></param>
        /// <param name="addCondition"></param>
        /// <param name="propertyVal"></param>
        private static void AddOnCondition(XElement data, string elementName, bool addCondition, object propertyVal)
        {
            var el = data.Element(elementName);
            if (null == el && addCondition)
            {
                el = new XElement(elementName);
                data.Add(el);
            }

            if (null != el && propertyVal != null)
            {

                el.SetValue(propertyVal);
            }
        }

        #endregion

        #region IItem Members

        /// <summary>
        /// Implementor can turn its internal state into an Agilix Item
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Item AsItem()
        {
            return this;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns the DLAP representation of review setting condition
        /// </summary>
        /// <param name="setting">Enum value of the setting</param>
        /// <returns>String representing the review setting condition</returns>
        public static string ReviewSettingToDlap(ReviewSetting setting)
        {
            switch (setting)
            {
                case ReviewSetting.Each: return "true";
                case ReviewSetting.Second: return "/attemptminimum<=attempt";
                case ReviewSetting.Final: return "score>=1";
                case ReviewSetting.DueDate: return "/duedate<=now";
                case ReviewSetting.Never: return "false";
                default: return "";
            }
        }

        /// <summary>
        /// Returns the integer representation of a DLAP review setting condition
        /// </summary>
        /// <param name="condition">DLAP value of the condition</param>
        /// <returns>Integer representing the review setting condition</returns>
        public static ReviewSetting DlapToReviewSetting(string setting = "")
        {
            switch (setting)
            {
                case "true": return ReviewSetting.Each;
                case "/attemptminimum<=attempt": return ReviewSetting.Second;
                case "score>=1": return ReviewSetting.Final;
                case "/duedate<=now": return ReviewSetting.DueDate;
                case "false": return ReviewSetting.Never;
                default: return ReviewSetting.Each;
            }
        }

        #endregion
    }
}
