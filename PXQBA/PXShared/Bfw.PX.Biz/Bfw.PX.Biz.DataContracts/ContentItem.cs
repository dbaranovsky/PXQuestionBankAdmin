using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Linq;
using Bfw.Common.Collections;

namespace Bfw.PX.Biz.DataContracts
{
    [Serializable]
    /// <summary>
    /// Represents any type of content in the system. This includes html documents, files,
    /// folders, etc.
    /// </summary>
    [DataContract]
    [KnownType(typeof(TimeZoneInfo.AdjustmentRule))]
    [KnownType(typeof(TimeZoneInfo.AdjustmentRule[]))]
    [KnownType(typeof(TimeZoneInfo.TransitionTime))]
    [KnownType(typeof(DayOfWeek))]
    public class ContentItem
    {
        ///// <summary>
        ///// MetaDataTemplate is only used for Sandbox.  
        ///// </summary>
        //public MetaData MetaDataTemplate { get; set; }

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
        /// ItemDataXml represents raw DLAP Item Data XML and is only used for Sandbox.
        /// </summary>
        public XElement ItemDataXml
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
        /// The unique Id of the item.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        [DataMember]
        public string ResourceEntityId { get; set; }

        /// <summary>
        /// The unique Id of the course this content belongs to.
        /// </summary>
        [DataMember]
        public string CourseId { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// The parent field in brain honeys item structure.  Nothing to do with PX tree structure.
        /// Right now only mapped dlap->model. Dont set this and expect it to save.
        /// </summary>
        [DataMember]
        public string BHParentId { get; set; }

        /// <summary>
        /// ID of the Actual Entity id
        /// </summary>
        [DataMember]
        public string ActualEntityid { get; set; }

        /// <summary>
        /// This is the discipine course id, generally used to point to the content course
        /// NOTE: This id can be over written from a bfw_property "agilixdisciplineid" found on the item
        /// </summary>
        [DataMember]
        public string HrefDisciplineCourseId { get; set; }

        /// <summary>
        /// The title of the item.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The subtitle of the item.
        /// </summary>
        [DataMember]
        public string SubTitle { get; set; }

        /// <summary>
        /// The description of the item.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Template the item was created from.
        /// </summary>
        [DataMember]
        public string Template { get; set; }

        /// <summary>
        /// The subtype, if any. This is always a custom value.
        /// </summary>
        [DataMember]
        public string Subtype { get; set; }

        /// <summary>
        /// Common property to store the url the content item points to or represents.
        /// </summary>
        [DataMember]
        public string Href { get; set; }

        /// <summary>
        /// Id of the resource folder for this content item, if any exist.
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// When true, this item sets its own score in the gradebook.
        /// </summary>
        [DataMember]
        public bool Sco { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// True if the item should be hidden from all users, false otherwise (default).
        /// </summary>
        [DataMember]
        public bool Hidden { get; set; }

        /// <summary>
        /// True if the item should be hidden from all users, false otherwise (default).
        /// </summary>
        [DataMember]
        public double MaxPoints { get; set; }

        /// <summary>
        /// Used only for quizzes.  If this is a custom exam type, what is it (e.g. LearningCurve)?
        /// </summary>
        [DataMember]
        public string CustomExamType { get; set; }

        /// <summary>
        /// True if the item should be hidden from students, but shown to instructors. False (default) otherwise.
        /// </summary>
        [DataMember]
        public bool HiddenFromStudents
        {
            get { return Properties.IsHiddenFromStudent(); }
            set { Properties.SetVisibilityForStudent(value); }
        }

        /// <summary>
        /// True if the item should be hidden from TOC.
        /// </summary>
        [DataMember]
        public bool HiddenFromToc
        {
            get
            {
                return Properties.GetPropertyValue<bool>("HiddenFromToc", false);
            }
            set
            {
                Properties.SetPropertyValue("HiddenFromToc", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// List of groupids for which item has been modified
        /// </summary>
        public List<string> AdjustedGroups { get; set; }

        /// <summary>
        /// True if the item is download only.
        /// </summary>
        [DataMember]
        public bool DownloadOnly
        {
            get
            {
                return Properties.GetPropertyValue<bool>("DownloadOnly", false);
            }
            set
            {
                Properties.SetPropertyValue("DownloadOnly", PropertyType.Boolean, value);
            }
        }


        /// <summary>
        /// Gets or sets the date when hidden items will start.
        /// </summary>
        /// <value>
        /// The read date.
        /// </value>
        [DataMember]
        public DateTime AvailableDate { get; set; }

        /// <summary>
        /// XML string defining role(instructor, student) based visibility settings for the item.
        /// </summary>
        /// <value>
        /// The visibility settings.
        /// </value>
        [DataMember]
        public string Visibility
        {
            get {  
                var localVisibility = Properties.GetPropertyValue<string>("bfw_visibility", "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>"); 
                if (string.IsNullOrEmpty ( localVisibility ))
                {
                    localVisibility = "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>";
                }

                return localVisibility;
            }

            set { Properties.SetPropertyValue("bfw_visibility", PropertyType.String, value); }
        }

        /// <summary>
        /// Allows social commenting to be controlled on an item level (on or off) when the course setting is also equal to true
        /// </summary>
        [DataMember]
        public bool SocialCommentingIntegration 
        {
            get { return Properties.GetPropertyValue<bool>("bfw_social_commenting_integration", false); }
            set { Properties.SetPropertyValue("bfw_social_commenting_integration", PropertyType.Boolean, value); }
        }

        /// <summary>
        /// CourseType the rubric is stored under
        /// </summary>
        public string RubricCourseType
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_type", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_type", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the rubric.
        /// </summary>
        public string RubricOwner
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_rubric_owner", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_rubric_owner", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that determines whether the rubric is active.
        /// </summary>
        public bool isActiveRubric
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_rubric_active", false);
            }
            set
            {
                Properties.SetPropertyValue("bfw_rubric_active", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// Property that determines whether the rubric is active.
        /// </summary>
        public bool WasDueDateManuallySet
        {
            get { return Properties.GetPropertyValue<bool>("bfw_duedate_manuallyset", false); }
            set { Properties.SetPropertyValue("bfw_duedate_manuallyset", PropertyType.Boolean, value); }
        }


        /// <summary>
        /// Property that determines whether the folder is a student created folder or not
        /// </summary>
        [DataMember]
        public bool IsStudentCreatedFolder { get; set; }

        /// <summary>
        /// A list of resources required by the content item that will be stored.
        /// </summary>
        [DataMember]
        public IEnumerable<Resource> Resources { get; set; }

        /// <summary>
        /// A list of attachments required by the content item that will be stored.
        /// </summary>
        [DataMember]
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Set of custom fields stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, string> CustomFields { get; set; }

        /// <summary>
        /// Set of metadata values assigned to the content item.
        /// </summary>
        [DataMember]
        public IDictionary<string, MetadataValue> Metadata { get; set; }



        /// <summary>
        /// Set of metadata values assigned to the content item to be used for faceting;
        /// This is used for more resource items in faceplate
        /// </summary>
        [DataMember]
        public IDictionary<string, string> FacetMetadata { get; set; }

        /// <summary>
        /// Identifies the chapter of this content item
        /// </summary>
        public string UnitChapter { get; set; }

        /// <summary>
        /// Contains the Sort Index for assignments with no due date
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// List of available categories for this content item.
        /// </summary>
        [DataMember]
        public IList<TocCategory> Categories { get; set; }

        /// <summary>
        /// Parent ID of this item when no category is selected.
        /// </summary>
        public string DefaultCategoryParentId { get; set; }

        /// <summary>
        /// Sequence of the item when no category is selected.
        /// </summary>
        public string DefaultCategorySequence { get; set; }

        /// <summary>
        /// Settings that determine the behavior of this content item as an assignment.
        /// </summary>
        /// <value>
        /// An <see cref="AssignmentSettings" /> value.
        /// </value>
        [DataMember]
        public AssignmentSettings AssignmentSettings { get; set; }

        /// <summary>
        /// Settings that determine the behavior of this content item as an assessment.
        /// </summary>
        /// <value>
        /// An <see cref="AssessmentSettings" /> value.
        /// </value>
        [DataMember]
        public AssessmentSettings AssessmentSettings { get; set; }

        /// <summary>
        /// Summary of gradebook information for item, if assigned.
        /// </summary>
        /// <value>
        /// A <see cref="GradebookInfo" /> value.
        /// </value>
        [DataMember]
        public GradebookInfo GradebookInfo { get; set; }

        /// <summary>
        /// Gets or sets the unit grade book category.
        /// </summary>
        /// <value>
        /// The unit grade book category.
        /// </value>
        public string UnitGradebookCategory
        {
            get { return Properties.GetPropertyValue<string>("bfw_unit_gradebook_category", string.Empty); }
            set { Properties.SetPropertyValue("bfw_unit_gradebook_category", PropertyType.String, value); }
        }

        /// <summary>
        /// A collection of settings that determine the group properties of an assessment.
        /// </summary>
        /// <value>
        /// A list collection of <see cref="AssessmentGroup" /> items.
        /// </value>
        [DataMember]
        public List<AssessmentGroup> AssessmentGroups { get; set; }

        /// <summary>
        /// Stores a the list of references to question items if there are any.
        /// </summary>
        [DataMember]
        public IList<QuizQuestion> QuizQuestions { get; set; }

        /// <summary>
        /// Gets or sets the read date.
        /// </summary>
        /// <value>
        /// The read date.
        /// </value>
        [DataMember]
        public DateTime ReadDate { get; set; }

        /// <summary>
        /// Gets or sets the Rubric data for item.
        /// </summary>
        /// <value>
        /// The Rubric data.
        /// </value>
        [DataMember]
        public Resource Rubric { get; set; }

        /// <summary>
        /// Gets or sets the Rubric data for item.
        /// </summary>
        /// <value>
        /// The Rubric data.
        /// </value>
        [DataMember]
        public List<LearningObjective> LearningObjectives { get; set; }

        /// <summary>
        /// Gets or sets the related contents.
        /// </summary>
        /// <value>
        /// The related contents.
        /// </value>
        [DataMember]
        public List<RelatedContent> RelatedContents { get; set; }

        /// <summary>
        /// Gets or sets the important flag
        /// </summary>
        [DataMember]
        public bool IsImportant { get; set; }

        /// <summary>
        /// True if the item is in the assignment center
        /// </summary>
        [DataMember]
        public bool InAssignmentCenter { get; set; }

        /// <summary>
        /// The creation date of the item
        /// </summary>
        [DataMember]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// List of allowed content items which can be created from the Assign tab
        /// </summary>
        [DataMember]
        public List<RelatedTemplate> RelatedTemplates { get; set; }

        /// <summary>
        /// Default points value for content
        /// </summary>
        [DataMember]
        public int DefaultPoints { get; set; }

        [DataMember]
        public string CustomSettings { get; set; }

        /// <summary>
        /// Course type to which the item is locked by.
        /// </summary>
        [DataMember]
        public string LockedCourseType { get; set; }

        /// <summary>
        /// Return whether the item is locked for edit for the current course type
        /// </summary>
        public bool IsItemLocked { get; set; }

        /// <summary>
        /// Structure holding information about usage of the item on the proxy page
        /// </summary>
        [DataMember]
        public ProxyConfig ProxyConfig { get; set; }

        /// <summary>
        /// Gets/sets a variable that determines if the start button should be displayed regardless of duedates
        /// </summary>
        [DataMember]
        public bool OverrideDueDateReq { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentItem"/> class and all member collections.
        /// </summary>
        public ContentItem()
        {
            Properties = new Dictionary<string, PropertyValue>();
            CustomFields = new Dictionary<string, string>();
            Metadata = new Dictionary<string, MetadataValue>();
            FacetMetadata = new Dictionary<string, string>();
            QuizQuestions = new List<QuizQuestion>();
            RelatedTemplates = new List<RelatedTemplate>();
            Containers = new List<Container>();
            SubContainerIds = new List<Container>();
        }

        public void SetVisibility(string visibility, DateTime DueDate)
        {
            var node = XElement.Parse("<bfw_visibility />", LoadOptions.None);
            var roles = new XElement("roles");

            node.Add(roles);
            roles.Add(new XElement("instructor"));

            if (visibility != "hidefromstudent")
            {
                var student = (new XElement("student"));

                if (visibility == "restrictedbydate")
                {
                    var restriction = new XElement("restriction");
                    var endDateAttribute = new XAttribute("endate", DueDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    var endDate = new XElement("date", endDateAttribute);

                    restriction.Add(endDate);
                    student.Add(restriction);
                }

                roles.Add(student);
            }

            Visibility = node.ToString();
        }

        /// <summary>
        /// A useful string representation of a ContentItem.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} [{1}]", Id, Title);
        }

        /// <summary>
        /// Tooltip for Project Item Type
        /// </summary>
        [DataMember]
        public string Tooltip { get; set; }

        /// <summary>
        /// Gradebook catergory weight
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// category's percent grade
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// Date the item was last modified.
        /// </summary>
        public DateTime? Modified { get; set; }

        /// <summary>
        /// Thumbnail for Project Item Type
        /// </summary>
        [DataMember]
        public string Thumbnail
        {
            get { return Properties.GetPropertyValue<string>("thumbnail", string.Empty); }
            set { Properties.SetPropertyValue("thumbnail", PropertyType.String, value); }
        }

        /// <summary>
        /// Gets or sets the conainers
        /// </summary>
        public List<Container> Containers { get; set; }
        public List<Container> SubContainerIds { get; set; }

        /// <summary>
        /// This is primarly used for x-Book for right now,
        /// It will contain the Assignment Folder Id which is how it is linked as an assignment
        /// <meta-xbook-assignment-id>ID</meta-xbook-assignment-id>
        /// </summary>
        [DataMember]
        public string AssignmentFolderId { get; set; }

        /// <summary>
        /// Determines settings defined in GradeFlags enum
        /// </summary>
        [DataMember]
        public GradeFlags GradeFlags { get; set; }

        /// <summary>
        /// The number of minutes after duedate wherein the student may still submit their work. 
        /// The value -1 indicates infinite grace period. The value 0 indicates no grace period. 
        /// The duedategrace attribute supercedes allowlatesubmission and if both are present, duedategrace takes precedence.
        /// </summary>
        [DataMember]
        public long DueDateGrace { get; set; }

        /// <summary>
        /// Get/Set the exam template for the quiz. Only in use for html quiz right now
        /// </summary>
        [DataMember]
        public string ExamTemplate { get; set; }

        #region Containers and SubContainers

        /// <summary>
        /// Add a new TocCategory to a content item.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="itemParentId">The item parent ID.</param>
        public void AddCategoryToItem(string category, string itemParentId, string sequence)
        {
            if (!Categories.IsNullOrEmpty())
            {
                var cat = Categories.Where(t => t.Text == category);
                if (cat.IsNullOrEmpty())
                {
                    Categories.Add(new TocCategory() { Id = category, Text = category, Sequence = sequence, ItemParentId = itemParentId });
                }
            }
            else
            {
                Categories = new List<TocCategory>() { new TocCategory() { Id = category, Text = category, Sequence = sequence, ItemParentId = itemParentId } };
            }
        }

        /// <summary>
        /// Gets the container value of the specified toc
        /// </summary>
        /// <param name="toc">The toc.</param>
        /// <returns></returns>
        public string GetContainer(string toc = "syllabusfilter")
        {
            var container = Containers.FirstOrDefault(c => c.Toc == toc);

            return container == null ? null : container.Value;
        }

        /// <summary>
        /// Gets the sub container value of the specified toc
        /// </summary>
        /// <param name="toc">The toc.</param>
        /// <returns></returns>
        public string GetSubContainer(string toc = "syllabusfilter")
        {
            var subcontainer = SubContainerIds.FirstOrDefault(c => c.Toc == toc);

            return subcontainer == null ? null : subcontainer.Value;
        }

        /// <summary>
        /// Sets the val to toc container.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="toc">The toc.</param>
        public void SetContainer(string val, string toc = "syllabusfilter")
        {
            var container = Containers.FirstOrDefault(c => c.Toc == toc);
            if (container != null)
            {
                container.Value = val;
            }
            else if (!String.IsNullOrEmpty(toc))
            {
                Containers.Add(new Container(toc, val, "exact"));
            }
        }

        /// <summary>
        /// Sets the val to toc sub-container.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="toc">The toc.</param>
        public void SetSubContainer(string val, string toc = "syllabusfilter")
        {
            var subcontainer = SubContainerIds.FirstOrDefault(c => c.Toc == toc);
            if (subcontainer != null)
            {
                subcontainer.Value = val;
            }
            else if (!String.IsNullOrEmpty(toc))
            {
                SubContainerIds.Add(new Container(toc, val, "exact"));
            }
        }

        /// <summary>
        /// Sets the syllabus filter category.
        /// </summary>
        /// <param name="parentid">The parent id.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="sequence">The sequence.</param>
        public void SetSyllabusFilterCategory(string parentid, string toc, string sequence = null)
        {
            if (String.IsNullOrEmpty(toc)) return;

            if (!Categories.IsNullOrEmpty())
            {
                var cat = Categories.FirstOrDefault(c => c.Id == toc);
                if (cat != null)
                {
                    //save sequence for saving later
                    if (sequence.IsNullOrEmpty())
                    {
                        sequence = cat.Sequence;
                    }
                    //remove category so we can re-add it
                    Categories.Remove(cat);
                }
            }

            if (sequence.IsNullOrEmpty())
            {
                sequence = Sequence; //default sequence to the item's sequence
            }

            if (!string.IsNullOrEmpty(parentid))
                AddCategoryToItem(toc, parentid, sequence);
        }

        #endregion
    }
}
