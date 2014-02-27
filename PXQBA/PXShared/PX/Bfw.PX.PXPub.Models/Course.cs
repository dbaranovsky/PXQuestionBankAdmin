using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Bfw.Common;
using System.Xml.Linq;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    public class Course
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// </summary>
        public Course()
        {
            Children = new List<ContentItem>();
            CompletedItems = new List<ContentItem>();
            NoDueDate = new List<ContentItem>();
            EndOfClass = new List<ContentItem>();
            PassedDueDate = new List<ContentItem>();
            DueNextMonth = new List<ContentItem>();
            DueTwoWeeks = new List<ContentItem>();
            DueThisWeek = new List<ContentItem>();
            ProductCourseId = string.Empty;
            AssessmentConfiguration = new CourseAssessmentConfiguration();
            CourseHomePage = "";
            QuestionFilter = new QuestionFilter();
        }

        /// <summary>
        /// The ID of the course (entity ID).
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// The course's title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required(ErrorMessage = "You must specify a title")]
        [System.ComponentModel.DisplayName("Course Name")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the course user.
        /// </summary>
        /// <value>
        /// The name of the course user.
        /// </value>
        [Required(ErrorMessage = "You must specify a name")]
        [System.ComponentModel.DisplayName("Instructor Name(s)")]
        public string CourseUserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the course time zone.
        /// </summary>
        /// <value>
        /// The Id of the time zone.
        /// </value>
        [Required(ErrorMessage = "You must specify a time zone")]
        [System.ComponentModel.DisplayName("Time Zone")]
        public string CourseTimeZone { get; set; }

        /// <summary>
        /// Backing store for the CourseProductName property.
        /// </summary>
        private string _courseProductName = "course";
        /// <summary>
        /// Gets or sets the name of the course product.
        /// </summary>
        /// <value>
        /// The name of the course product.
        /// </value>
        [System.ComponentModel.DisplayName("Product Name")]
        public string CourseProductName
        {
            get
            {
                return _courseProductName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _courseProductName = value;
                }
            }
        }

        /// <summary>
        /// The tinyMCE configuration
        /// </summary>
        public TextEditorConfiguration TextEditorConfiguration { get; set; }


        /// <summary>
        /// Gets or sets the optional course number
        /// </summary>
        [System.ComponentModel.DisplayName("Course Number")]
        public string CourseNumber { get; set; }

        /// <summary>
        /// Gets or sets the optional section number
        /// </summary>
        [System.ComponentModel.DisplayName("Course Section")]
        public string SectionNumber { get; set; }

        /// <summary>
        /// Instructor can switch domain of product. This flag is at product course level
        /// </summary>
        public string DerivedCourseId { get; set; }

        /// <summary>
        /// Determines whether [is sandbox course].
        /// </summary>
        public bool IsDashboardActive { get; set; }

        /// <summary>
        /// If flag is set to true the Instructor will be redirected to the dashboard instead of product page.
        /// </summary>
        public bool IsSandboxCourse { get; set; }

        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        /// <value>
        /// The name of the current user.
        /// </value>
        public string CurrentUserName { get; set; }

        /// <summary>
        /// Gets or sets school name
        /// </summary>
        /// <value>
        /// The name of the school.
        /// </value>
        [System.ComponentModel.DisplayName("School")]
        public string SchoolName { get; set; }

        /// <summary>
        /// Gets or sets the activated date.
        /// </summary>
        /// <value>
        /// The activated date.
        /// </value>
        [System.ComponentModel.DisplayName("Course Start Date")]
        public string ActivatedDate { get; set; }

        /// <summary>
        /// Gets or sets the possible derivative domains.
        /// </summary>
        /// <value>
        /// The possible derivative domains.
        /// </value>
        public IEnumerable<Domain> PossibleDerivativeDomains { get; set; }

        /// <summary>
        /// Gets or sets the selected derivative domain.
        /// </summary>
        /// <value>
        /// The selected derivative domain.
        /// </value>
        public string SelectedDerivativeDomain { get; set; }

        /// <summary>
        /// Gets the derivative domain Id
        /// </summary>
        public string DerivativeDomainId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is activated; otherwise, <c>false</c>.
        /// </value>
        public bool IsActivated { get { return (!String.IsNullOrEmpty(ActivatedDate) && Convert.ToDateTime(ActivatedDate) != DateTime.MinValue); } }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// Minimal score percentage to be achieved
        /// </summary>
        [Display(Name = "Passing Score (%)")]
        public double PassingScore { get; set; }

        /// <summary>
        /// settings for the assign tab used to assign the content items
        /// </summary>
        public AssignTabSettings AssignTabSettings { get; set; }

        /// <summary>
        /// mishka
        /// Gets or sets the product course id.
        /// </summary>
        /// <value>
        /// The product course id.
        /// </value>
        public string ProductCourseId { get; set; }

        /// <summary>
        /// a list of rubric type that allowed in the course
        /// </summary>
        public List<string> RubricTypes { get; set; }

        /// <summary>
        /// Gets or sets the Instructor Name on the course
        /// </summary>
        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is product course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is product course; otherwise, <c>false</c>.
        /// </value>
        public bool IsProductCourse { get; set; }

        /// <summary>
        /// Get a list of Units associated to the Course
        /// </summary>
        public List<PxUnit> Units
        {
            get
            {
                var items = new List<PxUnit>();
                foreach (var item in Children)
                {
                    if (item is PxUnit)
                    {
                        items.Add((PxUnit)item);
                    }
                }
                return items;
            }
        }

        /// <summary>
        /// Gets or sets the user access.
        /// </summary>
        /// <value>
        /// The user access.
        /// </value>
        public Bfw.PX.Biz.ServiceContracts.AccessLevel UserAccess
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user access Type.
        /// </summary>
        /// <value>
        /// The user access.
        /// </value>
        public Bfw.PX.Biz.ServiceContracts.AccessType UserAccessType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets download only document types.
        /// </summary>
        /// <value>
        /// The download only docyment types.
        /// </value>
        public string DownloadOnlyDocumentTypes
        {
            get;
            set;
        }


        public bool IsAllowExtraCredit { get; set; }

        /// <summary>
        /// Gets or sets the selected lesson id.
        /// </summary>
        /// <value>
        /// The selected lesson id.
        /// </value>
        public string SelectedLessonId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected content id.
        /// </summary>
        /// <value>
        /// The selected content id.
        /// </value>
        public string SelectedContentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter section.
        /// </summary>
        /// <value>
        /// The filter section.
        /// </value>
        public AssignmentCenterFilterSection FilterSection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the assessment configuration.
        /// </summary>
        /// <value>
        /// The assessment configuration.
        /// </value>
        public CourseAssessmentConfiguration AssessmentConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Academic tem details for the course
        /// </summary>
        /// <value>
        /// A academic terms value.
        /// </value>
        [Display(Name = "Academic Term")]
        public string AcademicTerm { get; set; }

        /// <summary>
        /// List of possible academic terms
        /// </summary>
        /// <value>
        /// List of academic terms
        /// </value>
        public IEnumerable<CourseAcademicTerm> PossibleAcademicTerms { get; set; }

        /// <summary>
        /// Gets or sets List of possible domains that can be used to register.
        /// </summary>
        public IEnumerable<Domain> PossibleDomains { get; set; }

        /// <summary>
        /// List of possible instructors that can be assigned as owner
        /// </summary>
        /// <value>
        /// List of instructor owners
        /// </value>
        public IEnumerable<KeyValuePair<string, string>> PossibleOwners { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has user materials.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has user materials; otherwise, <c>false</c>.
        /// </value>
        public bool HasUserMaterials { get; set; }

        /// <summary>
        /// Create list objects for the various date categories 
        /// </summary>
        public List<ContentItem> CompletedItems { get; set; }
        public List<ContentItem> Children { get; set; }
        public List<ContentItem> NoDueDate { get; set; }
        public List<ContentItem> EndOfClass { get; set; }
        public List<ContentItem> PassedDueDate { get; set; }
        public List<ContentItem> DueNextMonth { get; set; }
        public List<ContentItem> DueTwoWeeks { get; set; }
        public List<ContentItem> DueThisWeek { get; set; }

        /// <summary>
        /// The type of this course
        /// </summary>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// The type of this course section
        /// </summary>
        public string CourseSectionType { get; set; }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseOwner { get; set; }

        /// <summary>
        /// Property that stores a course-id. This course-id represents the course from which some content was copied into the course, but is not in the course hierarchy.
        /// </summary>
        public string CourseTemplate { get; set; }

        /// <summary>
        /// Property that stores a dashboard course-id.
        /// </summary>
        public string DashboardCourseId { get; set; }

        /// <summary>
        /// Property that stores the name of the Page Wizard page to use as the home page
        /// The default value for this property should be "Index". 
        /// </summary>
        public string CourseHomePage { get; set; }
        public string StudentStartPage { get; set; }
        public string InstructorStartPage  { get; set; }
        /// <summary>
        /// contains the name of the theme that is applied to the course. default is "default_theme"
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// contains HTML that acts as the course's welcome message to students.
        /// </summary>
        public string WelcomeMessage { get; set; }

        /// <summary>
        /// stores the path to a resource file used for the course's banner graphic.
        /// </summary>
        public string BannerImage { get; set; }

        /// <summary>
        /// contains a list of the theme names that the user can choose from for this course. 
        /// </summary>
        public string AllowedThemes { get; set; }

        /// <summary>
        /// stores whether the comment sharing is enabled or disabled. 
        /// </summary>
        public bool AllowCommentSharing { get; set; }

        /// <summary>
        /// stores whether the site search is enabled or disabled. 
        /// </summary>
        public bool AllowSiteSearch { get; set; }

        /// <summary>
        /// stores question card layout. 
        /// </summary>
        public string QuestionCardLayout { get; set; }

        /// <summary>
        /// Question Filter.
        /// </summary>
        public QuestionFilter QuestionFilter { get; set; }

        /// <summary>
        /// whether to show or not to show the sutdent view link
        /// </summary>
        public bool HideStudentViewLink { get; set; }

		/// <summary>
		/// Course Creation Date
		/// </summary>
		public string CourseCreationDate { get; set; }

        /// <summary>
        /// Stores Published Status of presentation
        /// </summary>
        public string PublishedStatus { get; set; }

        /// <summary>
        /// stores whether the SearchEngineIndex is enabled or disabled
        /// </summary>
        public bool SearchEngineIndex { get; set; }

        /// <summary>
        /// stores whether the FacebookIntegration is enabled or disabled
        /// </summary>
        public bool FacebookIntegration { get; set; }

        /// <summary>
        /// stores whether the social commenting plugin is enabled or disabled across the entire course
        /// (currently using discus plugin)
        /// </summary>
        public bool SocialCommentingIntegration { get; set; }

        /// <summary>
        /// stores the types of objects that can have social commenting
        /// </summary>
        public String[] SocialCommentingAllowedTypes { get; set; }


        /// <summary>
        /// stores whether the Course Activation is enabled or disabled
        /// </summary>
        public bool AllowActivation { get; set; }

        /// <summary>
        /// stores whether the Course launches the start page as landing page
        /// </summary>
        public bool IsLoadStartOnInit { get; set; }

        /// <summary>
        /// stores office hours for the course
        /// </summary>
        [Display(Name = "Office Hours:")]
        public string OfficeHours { get; set; }

        /// <summary>
        /// stores contact information for the course
        /// </summary>
        public List<Bfw.PX.Biz.DataContracts.ContactInfo> ContactInformation { get; set; }

        /// <summary>
        /// Syllabus Name
        /// </summary>
        public string Syllabus { get; set; }

        /// <summary>
        /// Syllabus Type: Url or File
        /// </summary>
        public string SyllabusType { get; set; }

        /// <summary>
        /// Flage whether to use weighted categories
        /// </summary>
        [Display(Name = "Use Weighted Categories")]
        public bool UseWeightedCategories { get; set; }

        /// <summary>
        /// Settings for dashboard
        /// </summary>
        public DashboardSettings DashboardSettings { get; set; }

        /// <summary>
        /// setting for Trial access button
        /// </summary>
        public bool AllowTrialAccess {get; set;}

        /// <summary>
        /// Syllabus Url
        /// </summary>
        public string SyllabusUrl
        {
            get
            {
                var result = string.Empty;

                if (SyllabusType == "Url")
                {
                    result = Syllabus;
                }

                return result;
            }
        }

        /// <summary>
        /// Syllabus File Name
        /// </summary>
        public string SyllabusFileName
        {
            get
            {
                var result = string.Empty;

                if (SyllabusType == "File")
                {
                    result = Syllabus;
                    var index = Syllabus.IndexOf("/");

                    if (index > 0)
                    {
                        result = Syllabus.Substring(Syllabus.IndexOf("/") + 1);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// This is the Generic Course Id which has no instructor associated
        /// &lt;bfw_generic_course id="{Generic Course ID}"&gt; true / false &lt;/bfw_generic_course&gt;
        /// </summary>
        public string GenericCourseId { get; set; }

        /// <summary>
        /// This is the Flag to tell Generic Course is supported by the product course
        /// &lt;bfw_generic_course id="{Generic Course ID}"&gt; true / false &lt;/bfw_generic_course&gt;
        /// </summary>
        public bool GenericCourseSupported { get; set; }

        /// <summary>
        /// Gets or sets Course sub type attribute like "generic" / "dashboard"
        /// </summary>
        public string CourseSubType { get; set; }

        /// <summary>
        /// Student can switch course from genric to different course. This flag is at product course level
        /// </summary>
        public bool EnrollmentSwitchSupported { get; set; }

        /// <summary>
        /// Instructor can switch domain of product. This flag is at product course level
        /// </summary>
        public bool DomainSwitchSupported { get; set; }

        /// <summary>
        /// Instructor managed course level attribute, decides if LMS Id is mandatory when student joins the course
        /// </summary>
        public bool LmsIdRequired { get; set; }

        /// <summary>
        /// Label that gets displayed for LMS Id
        /// </summary>
        public string LmsIdLabel { get; set; }

        /// <summary>
        /// LMS Id prompt that user sees when inputting LMS Id
        /// </summary>
        public string LmsIdPrompt { get; set; }

        /// <summary>
        /// Gets the start date for this Course
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        private DateTime GetStartDate(DateTime dt, bool isStartDate)
        {
            if (isStartDate)
            {
                return Convert.ToDateTime(dt.ToShortDateString() + " 12:00:00 AM");
            }
            else
            {
                return Convert.ToDateTime(dt.ToShortDateString() + " 11:59:59 PM");
            }
        }

        /// <summary>
        /// Since dates capured for module are based on server time, this method 
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        public DateTime UtcRelativeAdjust(DateTime dt)
        {
            return Bfw.Common.DateTimeConversion.UtcRelativeAdjustCommon(dt, CourseTimeZone);
        }

        /// <summary>
        /// Gets the no due date Assignment Center Category
        /// </summary>
        public void SetNoDueDate()
        {
            var allItems = new List<ContentItem>();
            var correctUnitItems = FindTopLevelItems(this.Children.Where(l => l is PxUnit).ToList(), this.Children.Where(l => l is PxUnit).ToList());

            foreach (var item in correctUnitItems)
            {
                PxUnit module = (PxUnit)item;

                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if ((string.IsNullOrEmpty(module.AttachedToDateCategory)
                        || module.AttachedToDateCategory == "NoDueDate")
                        && module.DueDate.ToShortDateString() == DateTime.MinValue.ToShortDateString())
                    {
                        allItems.Add(module);
                    }
                }
            }

            var contentItems = this.Children.Where(l => (!(l is PxUnit)) && (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) || l.GetExtendedProperty("AttachedToDateCategory") == "NoDueDate")).Where(l => l.DueDate.ToShortDateString() == DateTime.MinValue.ToShortDateString());
            var contentItemsList = contentItems.ToList();

            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));

            SetAttachedCategory("NoDueDate", allItems);
            SortItem(allItems);
            NoDueDate = allItems;
        }

        /// <summary>
        /// Gets the due this week Assignment Center Category.
        /// </summary>
        public void SetDueThisWeek()
        {
            //var start = UtcRelativeAdjust(DateTime.Now.StartOfWeek(DayOfWeek.Sunday).StartOfDay());
            var start = UtcRelativeAdjust(DateTime.Now);
            var end = UtcRelativeAdjust(DateTime.Now.EndOfWeek(DayOfWeek.Sunday).EndOfDay());
            var allItems = new List<ContentItem>();

            var contentItems = this.Children.Where(l => (!(l is PxUnit)) &&
                    (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) ||
                    l.GetExtendedProperty("AttachedToDateCategory") == "DueThisWeek")).Where(
                    l => (l.DueDate < end && l.DueDate >= start));

            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));

            SetAttachedCategory("DueThisWeek", allItems);
            SortItem(allItems);

            DueThisWeek = allItems;
        }

        /// <summary>
        /// Gets the due two weeks Assignment Center Category.
        /// </summary>
        public void SetDueTwoWeeks()
        {
            DayOfWeek day = UtcRelativeAdjust(DateTime.Now).DayOfWeek;
            int days = day - DayOfWeek.Monday;

            DateTime start = GetStartDate(UtcRelativeAdjust(DateTime.Now).AddDays(7 - days), true);
            DateTime end = GetStartDate(start.AddDays(14), false);
            //start = UtcRelativeAdjust(DateTime.Today);

            var allItems = new List<ContentItem>();
            var correctUnitItems = FindTopLevelItems(this.Children.Where(l => l is PxUnit).ToList(), this.Children.Where(l => l is PxUnit).ToList());

            foreach (var item in correctUnitItems)
            {
                PxUnit module = (PxUnit)item;
                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if ((string.IsNullOrEmpty(module.AttachedToDateCategory) || module.AttachedToDateCategory == "DueTwoWeeks") && module.DueDate >= start && module.DueDate < end && module.DueDate.Date != DateTime.Now.GetCourseDateTime().Date)
                    {
                        allItems.Add(module);

                    }
                }
            }

            var contentItems = this.Children.Where(l => (!(l is PxUnit)) && (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) || l.GetExtendedProperty("AttachedToDateCategory") == "DueTwoWeeks")).Where(l => (l.DueDate >= start && l.DueDate <= end && l.DueDate.Date != DateTime.Now.GetCourseDateTime().Date));
            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));



            SetAttachedCategory("DueTwoWeeks", allItems);
            SortItem(allItems);

            DueTwoWeeks = allItems;
        }

        /// <summary>
        /// Gets the due next month Assignment Center Category.
        /// </summary>
        public void SetDueNextMonth()
        {
            DayOfWeek day = UtcRelativeAdjust(DateTime.Now).DayOfWeek;
            int days = day - DayOfWeek.Monday;

            DateTime start = GetStartDate(UtcRelativeAdjust(DateTime.Now).AddDays(21 - days), true);
            DateTime end = GetStartDate(start.AddDays(31), false);
            var allItems = new List<ContentItem>();

            var correctUnitItems = FindTopLevelItems(this.Children.Where(l => l is PxUnit).ToList(), this.Children.Where(l => l is PxUnit).ToList());

            foreach (var item in correctUnitItems)
            {
                PxUnit module = (PxUnit)item;
                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if ((string.IsNullOrEmpty(module.AttachedToDateCategory) || module.AttachedToDateCategory == "DueNextMonth") && module.DueDate >= start && module.DueDate < end && module.DueDate.Date != UtcRelativeAdjust(DateTime.Now).Date)
                    {
                        allItems.Add(module);

                    }
                }
            }

            //var contentItems = this.Children.Where(l => (!(l is PxUnit)) && (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) || l.GetExtendedProperty("AttachedToDateCategory") == "DueNextMonth")).Where(l => (l.DueDate.Month == DateTime.Now.Month && l.DueDate.Year == DateTime.Now.Year) && l.DueDate > DateTime.Now);
            var contentItems = this.Children.Where(l => (!(l is PxUnit)) &&
                            (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) ||
                            l.GetExtendedProperty("AttachedToDateCategory") == "DueNextMonth")).Where(
                            l => (l.DueDate < end && l.DueDate >= start));
            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));


            SetAttachedCategory("DueNextMonth", allItems);
            SortItem(allItems);

            //foreach (var remove in itemsToRemove)
            //{
            //    this.Children.Remove(remove);
            //}

            DueNextMonth = allItems;
        }

        public List<ContentItem> FindTopLevelItems(List<ContentItem> unitItems, List<ContentItem> correctUnitItems)
        {
            foreach (PxUnit unit in unitItems)
            {
                var results = unitItems.Where(u => u.Id == unit.SyllabusFilter);
                if (results.Any())
                {
                    correctUnitItems.Remove(unit);
                }
            }
            return correctUnitItems;

        }

        /// <summary>
        /// Removes Items that are in units so they don't appear twice
        /// </summary>
        /// <param name="contentItems">List of items except Units </param>
        /// <param name="allItems">List of All content Items</param>
        /// <returns>List of content items that aren't in units</returns>
        public List<ContentItem> RemoveItemsInUnits(List<ContentItem> contentItems, List<ContentItem> allItems)
        {
            var contentItemsList = contentItems.ToList();

            foreach (ContentItem content in contentItems)
            {
                var foundItems = allItems.Where(i => i.Id == content.SyllabusFilter);


                if (foundItems.Any())
                {

                    contentItemsList.Remove(content);
                }
            }

            allItems.AddRange(contentItemsList);
            return allItems;
        }

        /// <summary>
        /// Gets the passed due date Assignment Center Category.
        /// </summary>
        public void SetPassedDueDate()
        {
            var allItems = new List<ContentItem>();
            var correctUnitItems = FindTopLevelItems(this.Children.Where(l => l is PxUnit).ToList(), this.Children.Where(l => l is PxUnit).ToList());

            foreach (var item in correctUnitItems)
            {
                PxUnit module = (PxUnit)item;
                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if ((string.IsNullOrEmpty(module.AttachedToDateCategory) || module.AttachedToDateCategory == "PassedDueDate") && module.DueDate < DateTime.Now.GetCourseDateTime().AddDays(-1) && module.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString())
                    {
                        allItems.Add(module);

                    }
                }
            }

            //var contentItems = this.Children.Where(l => (!(l is PxUnit)) && (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) || l.GetExtendedProperty("AttachedToDateCategory") == "PassedDueDate"))
            //    .Where(l => (DateTime.Compare(l.DueDate, UtcRelativeAdjust(DateTime.Now)) <= 1 && l.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()));

            var contentItems = this.Children.Where(l => (!(l is PxUnit)))
            .Where(l => (DateTime.Compare(l.DueDate, UtcRelativeAdjust(DateTime.Now)) <= 0 && l.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()));
            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));

            SetAttachedCategory("PassedDueDate", allItems);
            SortItem(allItems);

            //foreach (var remove in itemsToRemove)
            //{
            //    this.Children.Remove(remove);
            //}

            PassedDueDate = allItems;
        }

        /// <summary>
        /// Gets the end of class Assignment Center Category.
        /// </summary>
        public void SetEndOfClass()
        {
            var nextMonth = UtcRelativeAdjust(DateTime.Now).AddMonths(1);
            var allItems = new List<ContentItem>();
            var itemsToRemove = new List<ContentItem>();
            var correctUnitItems = FindTopLevelItems(this.Children.Where(l => l is PxUnit).ToList(), this.Children.Where(l => l is PxUnit).ToList());

            foreach (var item in correctUnitItems)
            {
                PxUnit module = (PxUnit)item;
                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if (string.IsNullOrEmpty(module.AttachedToDateCategory) && module.AttachedToDateCategory != "DueToday" && module.AttachedToDateCategory != "PassedDueDate" && module.AttachedToDateCategory != "DueNextMonth" && module.AttachedToDateCategory != "DueTwoWeeks" && module.AttachedToDateCategory != "DueThisWeek" && module.AttachedToDateCategory != "NoDueDate")
                    {
                        allItems.Add(module);
                        itemsToRemove.Add(module);
                    }
                }
            }

            var contentItems = this.Children.Where(l => (!(l is PxUnit)) && (string.IsNullOrEmpty(l.GetExtendedProperty("AttachedToDateCategory")) || l.GetExtendedProperty("AttachedToDateCategory") == "DueToday")).Where(l => (l.GetExtendedProperty("AttachedToDateCategory") != "PassedDueDate" && l.GetExtendedProperty("AttachedToDateCategory") != "DueNextMonth" && l.GetExtendedProperty("AttachedToDateCategory") != "DueTwoWeeks" && l.GetExtendedProperty("AttachedToDateCategory") != "DueThisWeek" && l.GetExtendedProperty("AttachedToDateCategory") != "NoDueDate"));
            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));

            itemsToRemove.AddRange(allItems.Where(i => i.DueDate <= UtcRelativeAdjust(DateTime.Today)));
            allItems.RemoveAll(i => i.DueDate <= UtcRelativeAdjust(DateTime.Now));
            SetAttachedCategory("EndOfClass", allItems);
            SortItem(allItems);

            EndOfClass = allItems;
        }

        public void SetCompletedItems()
        {
            var allItems = new List<ContentItem>();
            var contentItems = this.Children.Where(i => i.SubmittedDate.Year > DateTime.MinValue.Year && i.SubmittedDate.Year < DateTime.MaxValue.Year).ToList();

            var contentItemsList = contentItems.ToList();
            allItems.AddRange(RemoveItemsInUnits(contentItemsList, allItems));

            SetAttachedCategory("Completed", allItems);
            SortItem(allItems);
            CompletedItems = allItems;
        }

        public void InitializeItems()
        {
            SetCompletedItems();
            SetDueThisWeek();
            SetDueTwoWeeks();
            SetDueNextMonth();
            SetEndOfClass();
            SetPassedDueDate();
            SetNoDueDate();
        }

        /// <summary>
        /// Shows the blank date if min value.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static string ShowBlankDateIfMinValue(DateTime dateTime)
        {
            if (DateTime.MinValue.ToShortDateString() == dateTime.ToShortDateString())
            {
                return "";
            }

            if (DateTime.MaxValue.ToShortDateString() == dateTime.ToShortDateString())
            {
                return "";
            }

            return dateTime.ToShortDateString();
        }

        /// <summary>
        /// Content item Title and Due date comparison
        /// </summary>
        /// <param name="l1">The l1.</param>
        /// <param name="l2">The l2.</param>
        /// <returns></returns>
        public static int ContentItemSort(ContentItem l1, ContentItem l2)
        {
            if (l1.DueDate == l2.DueDate)
            {
                l1.Title = l1.Title ?? "";
                l2.Title = l2.Title ?? "";
                return l1.Title.CompareTo(l2.Title);
            }

            return l1.DueDate.CompareTo(l2.DueDate);
        }

        /// <summary>
        /// Gets the Child Lessons but Duedata and Category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns></returns>
        public List<ContentItem> GetLessons(string category, DateTime dueDate)
        {
            var allItems = new List<ContentItem>();
            foreach (var item in this.Children.Where(l => l is PxUnit))
            {
                PxUnit module = (PxUnit)item;
                if (!string.IsNullOrEmpty(module.AssociatedToCourse))
                {
                    if ((string.IsNullOrEmpty(module.AttachedToDateCategory) || module.AttachedToDateCategory == category) && module.DueDate.ToShortDateString() == dueDate.ToShortDateString())
                    {
                        allItems.Add(module);
                    }
                }
            }

            return allItems;
        }

        /// <summary>
        /// Gets the Child Lessons as modules.
        /// </summary>
        public List<PxUnit> GetLessons()
        {
            var allItems = new List<PxUnit>();

            foreach (var item in this.Children.Where(l => l is PxUnit))
            {
                allItems.Add((PxUnit)item);
            }

            return allItems;
        }

        /// <summary>
        /// Sets the attached category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="items">The items.</param>
        public void SetAttachedCategory(string category, List<ContentItem> items)
        {
            foreach (var item in items)
            {
                if (item is PxUnit)
                {
                    ((PxUnit)item).AttachedToDateCategory = category;
                }
                else
                {
                    item.ExtendedProperties["AttachedToDateCategory"] = category;
                }
            }
        }

        /// <summary>
        /// Sorts the item.
        /// </summary>
        /// <param name="items">The items.</param>
        public void SortItem(List<ContentItem> items)
        {
            items.Sort(ContentItemSort);
        }

        /// <summary>
        /// settings to show/hide tabs
        /// </summary>
        public TabSettings bfw_tab_settings { get; set; }

        public List<GradeBookWeightCategory> GradeBookCategoryList { get; set; }
       
    }
}
