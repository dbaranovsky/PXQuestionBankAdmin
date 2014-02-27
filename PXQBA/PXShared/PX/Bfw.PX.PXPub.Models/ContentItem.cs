using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Common;

namespace Bfw.PX.PXPub.Models
{
	/// <summary>
	/// Represents any type of content in the system.
	/// </summary>
	[KnownType(typeof(Bfw.PX.PXPub.Models.LearningCurveActivity))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.HtmlQuiz))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.ContentItem))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.TocCategory))]
	[KnownType(typeof(Bfw.PX.Biz.DataContracts.TocCategory))]

	[KnownType(typeof(IEnumerable<Bfw.PX.PXPub.Models.TocCategory>))]
	[KnownType(typeof(IEnumerable<Bfw.PX.Biz.DataContracts.TocCategory>))]

	[KnownType(typeof(Bfw.PX.PXPub.Models.Quiz))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.PxUnit))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.LessonBase))]

	[KnownType(typeof(Bfw.PX.PXPub.Models.ExternalContent))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Container))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.AssetLink))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Assignment))]

	[KnownType(typeof(Bfw.PX.PXPub.Models.Dropbox))]


	[KnownType(typeof(Bfw.PX.PXPub.Models.AssignmentCenterFilterSection))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Content404))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.CustomWidget))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Discussion))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Document))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.DocumentCollection))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Ebook))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.EbookBrowser))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Folder))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.HtmlDocument))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.Link))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.LinkCollection))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.NavigationItem))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.QuickLink))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.RelatedContent))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.RssFeed))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.RssLink))]	
	[KnownType(typeof(Bfw.PX.PXPub.Models.WidgetConfig))]
	[KnownType(typeof(Bfw.PX.PXPub.Models.WidgetConfiguration))]

	[DataContract]
	[Serializable]
	public class ContentItem
	{
		/// <summary>
		/// ID of the Actual Entity id
		/// </summary>
		[DataMember]
		public string ActualEntityid { get; set; }

		/// <summary>
		/// This is the discipline course id, generally used to point to the content course
		/// NOTE: This id can be over written from a bfw_property "agilixdisciplineid" found on the item
		/// </summary>
		[DataMember]
		public string HrefDisciplineCourseId { get; set; }

		/// <summary>
		/// AdminMetaData is only used by AdminTool  
		/// </summary>

		[DataMember]
		public AdminMetaData AdminMetaData { get; set; }

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
        protected XElement _data;

		/// <summary>
		/// ItemDataXml represents raw DLAP Item Data XML and is only used for Sandbox.
		/// </summary>
        [DataMember]
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
		/// True if the item is in read only mode and edits shouldn't be allowed.
		/// </summary>
		/// <value>
		///   <c>true</c> if [read only]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool ReadOnly { get; set; }

		/// <summary>
		/// Gets or sets the environment URL.
		/// </summary>
		/// <value>
		/// The environment URL.
		/// </value>
		[DataMember]
		public string EnvironmentUrl { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[DataMember]
		public ContentStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the course info.
		/// </summary>
		/// <value>
		/// The course info.
		/// </value>
		[DataMember]
		public Course CourseInfo { get; set; }

		/// <summary>
		/// Gets or sets the enrollment id.
		/// </summary>
		/// <value>
		/// The enrollment id.
		/// </value>
		[DataMember]
		public string EnrollmentId { get; set; }

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>
		/// The user id.
		/// </value>
		[DataMember]
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the user access.
		/// </summary>
		/// <value>
		/// The user access.
		/// </value>
		[DataMember]
		public Bfw.PX.Biz.ServiceContracts.AccessLevel UserAccess { get; set; }

		/// <summary>
		/// Id of the parent piece of content, null or empty if no parent exists.
		/// </summary>
		/// <value>
		/// The parent id.
		/// </value>
		[DataMember]
		public string ParentId { get; set; }

        /// <summary>
        /// The parent field in brain honeys item structure.  Nothing to do with PX tree structure.
        /// Right now only mapped dlap->model. Dont set this and expect it to save.
        /// </summary>
        [DataMember]
        public string BHParentId { get; set; }

		/// <summary>
		/// Parent ID of this item when no category is selected.
		/// </summary>
		/// <value>
		/// The default category parent id.
		/// </value>
		[DataMember]
		public string DefaultCategoryParentId { get; set; }

		/// <summary>
		/// When true, this item sets its own score in the grade book.
		/// </summary>
		[DataMember]
		public bool Sco { get; set; }

		/// <summary>
		/// The sequence key
		/// </summary>
		/// <value>
		/// The sequence.
		/// </value>
		[DataMember]
		public string Sequence { get; set; }

		/// <summary>
		/// Order in which this item should be displayed relative to other items within assigned category.
		/// </summary>
		[DataMember]
		public string CategorySequence { get; set; }

		/// <summary>
		/// Id of the content item
		/// </summary>
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public int level { get; set; }

		/// <summary>
		/// Title of the content item
		/// </summary>
		///        
		[Required(ErrorMessage = "You must specify a title")]
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Subtitle of the content item
		/// </summary>
		///        
		[Display(Description = "Subtitle", Name = "Subtitle")]
		[DataMember]
		public string SubTitle { get; set; }


		/// <summary>
		/// The description of this content item
		/// </summary>
		[DataMember]
		public virtual string Description { get; set; }

		/// <summary>
		/// The type of content the object represents
		/// </summary>
		[DataMember]
		public string Type { get; set; }

		/// <summary>
		/// Set by some ContentItem derivatives to distinguish type on 
		/// a more granular level. Defaults to string.Empty.
		/// </summary>
		[DataMember]
		public virtual string SubType { get; protected set; }
        
        /// <summary>
        /// Agilix item type
        /// </summary>
        [DataMember]
        public virtual DlapItemType AgilixType { get; set; }

		/// <summary>
		/// True if the content item can be assigned to a student for a grade. 
		/// </summary>
		[DataMember]
		public bool IsAssignable { get; set; }

		/// <summary>
		/// True if the content item is assigned to a student for a grade
		/// </summary>
		[DataMember]
		public bool IsAssigned { get; set; }
		
        /// <summary>
        /// This maps one way from the biz item's AssignmentSettings.IsAssigned.  Use to determine if the item is gradeable
        /// </summary>
		[DataMember]
		public bool IsGradable { get; set; }

        /// <summary>
        /// Gets/sets a variable that determines if the start button should be displayed regardless of duedates
        /// </summary>
        [DataMember]
        public bool OverrideDueDateReq { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow comments].
		/// </summary>
		/// <value>
		///   <c>true</c> if [allow comments]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool AllowComments { get; set; }

		/// <summary>
		/// Structure holding information about usage of the item on the proxy page
		/// </summary>
		[DataMember]
		public ProxyConfig ProxyConfig { get; set; }

		/// <summary>
		/// Determines which host to show
		/// </summary>
		[DataMember]
		public HostMode HostMode { get; set; }

		/// <summary>
		/// Is allow extra credit
		/// </summary>
		[DataMember]
		public bool IsAllowExtraCredit { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ContentItem"/> is hidden.
		/// </summary>
		/// <value>
		///   <c>true</c> if hidden; otherwise, <c>false</c>.
		/// </value>
		/// Maps to HiddenFromToc in DataContracts
		[DataMember]
		public bool Hidden { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ContentItem"/> is hidden to students.
		/// </summary>
		/// <value>
		///   <c>true</c> if hidden; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool HiddenFromStudents
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ContentItem"/> is hidden to students.
		/// </summary>
		/// <value>
		///   <c>true</c> if hidden; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool RestrictStudentAccess
		{
			get;
			set;
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
		/// The Url of the Content item
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[DataMember]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the discipline id.
		/// </summary>
		/// <value>
		/// The discipline id.
		/// </value>
		[DataMember]
		public string DisciplineId { get; set; }

		/// <summary>
		/// Gets or sets the start date.
		/// </summary>
		/// <value>
		/// The start date.
		/// </value>
        private DateTime _startDate;

        [DisplayFormat(DataFormatString = "{0:dddd MMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataMember]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                if (StartDateTZ != null)
                {
                    StartDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone StartDateTZ { get; set; }

		/// <summary>
		/// Gets or sets the due date.
		/// </summary>
		/// <value>
		/// The due date.
		/// </value>
        private DateTime _dueDate;

        [DisplayFormat(DataFormatString = "{0:dddd MMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataMember]
        public DateTime DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
                if (DueDateTZ != null)
                {
                    DueDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone DueDateTZ { get; set; }

        [DataMember]
        public DateTime GradeReleaseDate { get; set; }

		/// <summary>
		/// Gets or sets the extended properties.
		/// </summary>
		/// <value>
		/// The extended properties.
		/// </value>
		[DataMember]
		public Hashtable ExtendedProperties { get; set; }

		/// <summary>
		/// Gets or sets the custom fields
		/// </summary>
		/// <value>
		/// The custom fields.
		/// </value>
		[DataMember]
		public IDictionary<string, string> CustomFields { get; set; }

		/// <summary>
		/// Allows social commenting to be controlled on an item level (on or off) when the course setting is also equal to true
		/// </summary>
		[DataMember]
		public bool SocialCommentingIntegration { get; set; }

		/// <summary>
		/// Gets or sets the visibility.
		/// </summary>
		/// <value>
		/// The visibility.
		/// </value>
		[DataMember]
		public XElement Visibility { get; set; }

		/// <summary>
		/// Gets or sets the syllabus filter.
		/// </summary>
		/// <value>
		/// The syllabus filter.
		/// </value>
		[DataMember]
		public string SyllabusFilter { get; set; }

		/// <summary>
		/// The set of categories this content item is assigned
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[IgnoreDataMember]
		//[TypeConverter(typeof(List<TocCategory>))]

		//[TypeConverter(typeof(Bfw.Common.Collections.IEnumerableExtensions.Map<Bfw.PX.Biz.DataContracts.TocCategory, Bfw.PX.PXPub.Models.TocCategory>))]
		public IEnumerable<TocCategory> Categories { get; set; }

		/// <summary>
		/// Gets the extended property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public string GetExtendedProperty(string property) { return ExtendedProperties.ContainsKey(property) ? ExtendedProperties[property].ToString() : ""; }

		/// <summary>
		/// Make a list of policy (which might be used, e.g., on a quiz or homework) descriptions available
		/// </summary>
		/// <value>
		/// The policies.
		/// </value>
		[DataMember]
		public IEnumerable<string> Policies { get; set; }

		/// <summary>
		/// Gets or sets the template parent id.
		/// </summary>
		/// <value>
		/// The template parent id.
		/// </value>
		[DataMember]
		public string TemplateParentId { get; set; }

		/// <summary>
		/// Gets or sets the max points.
		/// </summary>
		/// <value>
		/// The max points.
		/// </value>
		[DataMember]
		public double MaxPoints { get; set; }

		/// <summary>
		/// Gets or sets the score.
		/// </summary>
		/// <value>
		/// The score.
		/// </value>
		[DataMember]
		public double Score { get; set; }

		/// <summary>
		/// The average score for graded submissions of this item.
		/// </summary>
		[DataMember]
		public double AverageScore { get; set; }

		/// <summary>
		/// The number of gradebook submissions made on this item.
		/// </summary>
		[DataMember]
		public int TotalSubmissions { get; set; }

		/// <summary>
		/// The number of graded submissions made on this item.
		/// </summary>
		[DataMember]
		public int TotalGrades { get; set; }

		/// <summary>
		/// Gets or sets the submitted date.
		/// </summary>
		/// <value>
		/// The submitted date.
		/// </value>
		[DataMember]
		public DateTime SubmittedDate { get; set; }

		/// <summary>
		/// Gets or sets the grade book weight category id.
		/// </summary>
		/// <value>
		/// The grade book weight category id.
		/// </value>
		[DataMember]
		public string GradeBookWeightCategoryId { get; set; }

		/// <summary>
		/// Gets or sets the unit grade book category.
		/// </summary>
		/// <value>
		/// The unit grade book category.
		/// </value>
		[DataMember]
		public string UnitGradebookCategory { get; set; }

		/// <summary>
		/// Gets or sets the source template id.
		/// </summary>
		/// <value>
		/// The source template id.
		/// </value>
		[DataMember]
		public string SourceTemplateId { get; set; }

		/// <summary>
		/// Gets or sets the read date.
		/// </summary>
		/// <value>
		/// The read date.
		/// </value>
		[DataMember]
		public DateTime ReadDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is being edited.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is being edited; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsBeingEdited { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has been submmited by user.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has been submitted; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsUserSubmitted { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance has been graded for current user.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has been graded; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsUserGraded { get; set; }

		/// <summary>
		/// Gets or sets the applicable enrollment id.
		/// </summary>
		/// <value>
		/// The applicable enrollment id.
		/// </value>
		[DataMember]
		public string ApplicableEnrollmentId { get; set; }

		/// <summary>
		/// Default points value for content
		/// </summary>
		[DataMember]
		public int DefaultPoints { get; set; }		

		/// <summary>
		/// Course type to which the item is locked by.
		/// </summary>
		[DataMember]
		public string LockedCourseType { get; set; }

		/// <summary>
		/// Return whether the item is locked for edit for the current course type
		/// </summary>

		[DataMember]
		public bool IsItemLocked { get; set; }

		/// <summary>
		/// Gets the description or title.
		/// </summary>
		/// <returns></returns>

		public string GetDescriptionOrTitle()
		{
			if (string.IsNullOrEmpty(Description))
			{
				return Title;
			}

			return Description;
		}

		/// <summary>
		/// Gets the unit description for content item.
		/// </summary>
		/// <returns></returns>		
		public string GetUnitDescription()
		{
			var unitDesc = "unit description";

			switch (Type.ToLowerInvariant())
			{
				case "quiz":
					unitDesc = Type;
					// Display the # of students that completed the assignment
					// Display the average score on the assignment
					break;
				case "externalcontent":
					unitDesc = "eBook";
					// Display the # of students that completed the assignment
					// Display the average score on the assignment
					break;
				case "pxunit":
				case "folder":
					// For folders, display "Collection"
					unitDesc = "Collection";
					break;
				default:
					// For all other content types, display the name of content type
					unitDesc = Type;
					break;
			}

			// If the metadata value for "length" exists on the content item, display "Length:"
			//if (true) unitDesc += " Length:Short";

			return unitDesc;
		}

		/// <summary>
		/// Gets the unit assigned date range.
		/// </summary>
		/// <returns></returns>
		public string GetFriendlyDateRange()
		{
			var dateAssigned = "";
			if (DueDate.Year > DateTime.MinValue.Year)
			{
				dateAssigned = DueDate.ToShortDateString();
				if (StartDate.Year > DateTime.MinValue.Year)
				{
					dateAssigned = string.Format("{0}", StartDate.ToShortDateString());
				}
			}

			return dateAssigned;
		}

		/// <summary>
		/// CommentNote Id to load conversation when loading any document.
		/// </summary>
		/// <value>
		/// The note id.
		/// </value>
		[DataMember]
		public string NoteId { get; set; }

		/// <summary>
		/// Whether current course is product course or not
		/// </summary>
		[DataMember]
		public bool IsProductCourse
		{
			get;
			set;
		}

		[DataMember]
		public Boolean IsSupportRestoreDefault { get; set; }

		/// <summary>
		/// A flag which indicates whether an Assign tab is available in content create dialog
		/// </summary>
		[DataMember]
		public bool IsContentCreateAssign { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is instructor.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is instructor; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsInstructor { get; set; }

		/// <summary>
		/// Gets or sets the entity id.
		/// </summary>
		/// <value>
		/// The entity id.
		/// </value>
		[DataMember]
		public string EntityId { get; set; }

		/// <summary>
		/// is the due date manually set
		/// </summary>
		[DataMember]
		public bool WasDueDateManuallySet { get; set; }

		[DataMember]
		public int StudentCompletedPercentage { get; set; }
		[DataMember]
		public int StudentScorePercentage { get; set; }
		[DataMember]
		public int StudentCompletedItems { get; set; }
		[DataMember]
		public int StudentItemsAssigned { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentItem"/> class.
		/// </summary>
		public ContentItem()
		{
			Type = "ContentItem";
            TrackMinutesSpent = true;
			ExtendedProperties = new Hashtable();
			CustomFields = new Dictionary<string, string>();
			Categories = new List<TocCategory>();
			Policies = new List<string>();
			AllowComments = false;
			Visibility = XElement.Parse("<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>", LoadOptions.None);
			FacetMetadata = new Dictionary<string, string>();
			Containers = new List<Container>();
			SubContainerIds = new List<Container>();
		}

        /// <summary>
        /// Gets the container value for the specified toc
        /// </summary>
        /// <param name="toc">The TOC to get the container value for</param>
        /// <returns></returns>
        public string GetContainer(string toc = "syllabusfilter")
        {
            var container = this.Containers.FirstOrDefault(c => c.Toc == toc);
            if (container == null)
                return null;
            else
                return container.Value;
        }

        /// <summary>
        /// Sets the container value for the specified TOC
        /// </summary>
        /// <param name="toc">The TOC to set the container value for. If the TOC doesn't exist, it will be created</param>
        /// <param name="val">The value to set for the container</param>
        public void SetContainer(string val, string toc = "syllabusfilter")
        {
            var container = this.Containers.FirstOrDefault(c => c.Toc == toc);
            if (container != null)
            {
                container.Value = val;
            }
            else
            {
                this.Containers.Add(new Container(toc, val, "exact"));
            }
        }

        /// <summary>
        /// Gets the container value for the specified toc
        /// </summary>
        /// <param name="toc">The TOC to get the container value for</param>
        /// <returns></returns>
        public string GetSubContainer(string toc = "syllabusfilter")
        {
            var subcontainer = this.SubContainerIds.FirstOrDefault(c => c.Toc == toc);
            if (subcontainer == null)
                return null;
            else
                return subcontainer.Value;
        }

        /// <summary>
        /// Sets the container value for the specified TOC
        /// </summary>
        /// <param name="toc">The TOC to set the container value for. If the TOC doesn't exist, it will be created</param>
        /// <param name="val">The value to set for the container</param>
        public void SetSubContainer(string val, string toc = "syllabusfilter")
        {
            var subcontainer = this.SubContainerIds.FirstOrDefault(c => c.Toc == toc);
            if (subcontainer != null)
            {
                subcontainer.Value = val;
            }
            else
            {
                this.SubContainerIds.Add(new Container(toc, val, "exact"));
            }
        }

		/// <summary>
		/// If applicable then use this group Is
		/// </summary>
		[DataMember]
		public string GroupId { get; set; }

		/// <summary>
		/// Dictionary of metadata that has the faceted properties of the content item
		/// </summary>
		[DataMember]
		public Dictionary<string, string> FacetMetadata { get; set; }

		/// <summary>
		/// Components this instance.
		/// </summary>
		/// <returns></returns>
		public virtual BhComponent Component()
		{
			var bh = new BhComponent();

			return bh;
		}

		/// <summary>
		/// Tooltip for Project Item Type
		/// </summary>
		[DataMember]
		public string Tooltip { get; set; }
		/// <summary>
		/// Thumbnail for Project Item Type
		/// </summary>
		[DataMember]
		public string Thumbnail { get; set; }

		public string Instructions { get; set; }

		/// <summary>
		/// Grade book category weight
		/// </summary>
		[DataMember]
		public double Weight { get; set; }

		/// <summary>
		/// category's percent grade
		/// </summary>
		[DataMember]
		public double Percent { get; set; }

		/// <summary>
		/// Threshold
		/// </summary>
		[DataMember]
		public double Threshold { get; set; }

		/// <summary>
		/// Gets or sets the containers
		/// </summary>
		[DataMember]
		public List<Container> Containers { get; set; }
		[DataMember]
		public List<Container> SubContainerIds { get; set; }

		/// <summary>
		/// Completion Status
		/// </summary>
		[DataMember]
		public CompletionStatus CompletionStatus { get; set; }

		[DataMember]
		public double PossibleScore { get; set; }

		[DataMember]
		public double AchievedScore { get; set; }

        /// <summary>
        /// When true, we track student minutes spent on activity in PX
        /// </summary>
        [DataMember]
        public bool TrackMinutesSpent { get; set; }

	}
}
