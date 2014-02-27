using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a course item in the system. This includes settings and metadata.
    /// </summary>
    public class Course : IComparable<Course>
    {
        private string _activatedDate;

        /// <summary>
        /// The ID of the course (entity ID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// mishka
        /// parent course id
        /// </summary>
        public string ProductCourseId { get; set; }
        /// <summary>
        /// The course's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The course's domain.
        /// </summary>
        public Domain Domain { get; set; }

        /// <summary>
        /// Parent Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Date the course was activated.
        /// </summary>
        public string ActivatedDate {
            get {
                if (!String.IsNullOrWhiteSpace(_activatedDate))
                {
                    var dtActivated = DateTime.Parse(_activatedDate);
                    if (dtActivated != null)
                    {
                        return (dtActivated == DateTime.MinValue || dtActivated == DateTime.MaxValue)
                                ? _activatedDate
                                : Convert.ToDateTime(_activatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    }
                }

                return _activatedDate;
            }
            set { _activatedDate = value; }
        }

        /// <summary>
        ///Course Creation Date
        /// </summary>
        public string CreationDate { get; set; }

        public DateTime CreatedDate
        {

            get
            {
                return Convert.ToDateTime(this.CreationDate).ToLocalTime();
                
            }

        }
    
        /// <summary>
        /// Sub Type Of Course
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Collection of group sets on this course.
        /// </summary>
        public IEnumerable<GroupSet> GroupSets { get; set; }

        /// <summary>
        /// If flag is set to true the Instructor will be redirected to the dashboard instead of product page.
        /// </summary>
        public bool IsDashboardActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TabSettings bfw_tab_settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<GradeBookWeightCategory> GradeBookCategoryList { get; set; }

        /// <summary>
        /// Minimal score percentage to be achieved
        /// </summary>
        public double PassingScore { get; set; }

        /// <summary>
        /// The tinyMCE configuration
        /// </summary>
        public tinyMCE TinyMCE { get; set; }

        /// <summary>
        /// A list of rubric types allowed in the course
        /// </summary>
        public List<string> RubricTypes { get; set; }

        /// <summary>
        /// allows a js files to be added to a course level application
        /// </summary>
        public string ApplicationJSFiles
        {
            get { return Properties.GetPropertyValue<string>("application_js_files", ""); }
        }

        /// <summary>
        /// Title of the product course, which is appended to the Title property.
        /// </summary>
        public string CourseProductName
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseProductName", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseProductName", PropertyType.String, value);
            }
        }

        /// <summary>
        /// TimeZone of the product course
        /// </summary>
        public string CourseTimeZone
        {
            get
            {
                string timezone =  Properties.GetPropertyValue<string>("CourseTimeZone", TimeZoneInfo.Local.Id);
                if (string.IsNullOrWhiteSpace(timezone))
                {
                    timezone =  TimeZoneInfo.Local.Id;
                }
                return timezone;
            }
            set
            {
                Properties.SetPropertyValue("CourseTimeZone", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the instructor that created the course.
        /// </summary>
        /// <value>
        /// The name of the instructor.
        /// </value>
        public string InstructorName
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseUserName", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseUserName", PropertyType.String, value);
            }
        }

        /// <summary>
        /// A flag which indicates whether the course needs to be accessed in a readonly mode
        /// </summary>
        public String ReadOnly 
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_read_only", string.Empty);
            }
            set
            {
                Properties.SetPropertyValue("bfw_read_only", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Gets or sets the coursenumber the instructor set  on the course
        /// </summary>
        public string CourseNumber
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseNumber", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseNumber", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Gets or sets the section number the instructor set on the course
        /// </summary>
        public string SectionNumber
        {
            get
            {
                return Properties.GetPropertyValue<string>("SectionNumber", "");
            }
            set
            {
                Properties.SetPropertyValue("SectionNumber", PropertyType.String, value);
            }
        }

        public bool ShowRelatedContent
        {
            get {
                return Properties.GetPropertyValue<bool>("bfw_related_content", true);
            }
            set {
                Properties.SetPropertyValue("bfw_related_content", PropertyType.Boolean, value);
            }
        }

        public bool ObtainHBAuthToken
        {
            get { return Properties.GetPropertyValue<bool>("bfw_obtain_bh_auth_cookie", false); }
        }
        public bool IsAllowExtraCredit
        {
            get { return Properties.GetPropertyValue<bool>("bfw_allow_extracredit", true); }
            set { Properties.SetPropertyValue("bfw_allow_extracredit", PropertyType.Boolean, value); }
        }        

        /// <summary>
        /// Determines if the more resources tab is displayed in the course content browser.
        /// </summary>
        /// <value>
        /// True if the more resources tab should display.
        /// </value>
        public string MoreResources
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_more_resources", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_more_resources", PropertyType.String, value);
            }
        }


        public string Isbn13
        {
            get
            {
                return Properties.GetPropertyValue<string>("meta-book-isbn", "");
            }
            set
            {
                Properties.SetPropertyValue("meta-book-isbn", PropertyType.String, value);
            }
        }

        /// <summary>
        /// The type of this course
        /// </summary>
        public string CourseType
        {
            get
            {
                string retval =  Properties.GetPropertyValue<string>("bfw_course_type", "LearningCurve");
                if (string.IsNullOrEmpty (retval))
                    return "LearningCurve";
                return retval;
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_type", PropertyType.String, value);
                Properties.SetPropertyValue("meta-bfw_course_type", PropertyType.String, value);
            }
        }

        /// <summary>
        /// The type of this course section
        /// </summary>
        public string CourseSectionType { get; set; }

        /// <summary>
        /// The list of custom css parameters for the course
        /// </summary>
        public List<CSSParameter> TitleCSSParameters { get; set; }

        /// <summary>
        /// Course Discipline Name (ie: Bio, Psych, Econ, etc) used for course title
        /// </summary>
        public string CourseDisciplineAbbreviation { get; set; }

        /// <summary>
        /// Holds reference to coourse id with questions for the product
        /// </summary>
        public string QuestionBankRepositoryCourse { get; set; }

        /// <summary>
        /// Download Only types
        /// </summary>
        public string DownloadOnlyDocumentTypes
        {
            get
            {
                string retval = Properties.GetPropertyValue<string>("bfw_download_only_document_types", "");
                if (string.IsNullOrEmpty(retval))
                    return string.Empty;
                return retval;
            }
            set
            {
                Properties.SetPropertyValue("bfw_download_only_document_types", PropertyType.String, value);
            }
        }


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
        /// Academic term in which the course is present
        /// </summary>
        public string AcademicTerm
        {
            get
            {
                string retval = Properties.GetPropertyValue<string>("meta-bfw_academic_term", "");
                if (string.IsNullOrEmpty(retval))
                    return "";
                return retval;
            }
            set
            {
                Properties.SetPropertyValue("meta-bfw_academic_term", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseOwner
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_owner", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_owner", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseDescription
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_description", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_description", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseAuthor
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_author", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_author", PropertyType.String, value);
            }
        }

        
        /// <summary>
        /// Property that decides wether to show the student view link in a course or not
        /// </summary>
        public bool HideStudentViewLink
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_hide_student_view_in_header", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_hide_student_view_in_header", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// Setting that dictates whether to show learning curve quiz on a course or not
        /// </summary>
        public bool EnableLearningCurveQuiz
        {
            get
            {
                return Properties.GetPropertyValue<bool>("enable_learningcurve_quiz", true);
            }
            set
            {
                Properties.SetPropertyValue("enable_learningcurve_quiz", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// Setting that dictates whether to show homework quiz on a course or not
        /// </summary>
        public bool EnableHomeworkQuiz
        {
            get
            {
                return Properties.GetPropertyValue<bool>("enable_homework_quiz", true);
            }
            set
            {
                Properties.SetPropertyValue("enable_homework_quiz", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// Property that stores a course-id. This course-id represents the course from which some content was copied into the course, but is not in the course hierarchy.
        /// </summary>
        public string CourseTemplate
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_template", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_template", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores a dashboard course-id.
        /// </summary>
        public string DashboardCourseId
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_dashboard_course", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_dashboard_course", PropertyType.String, value);
            }
        }

        /// <summary>
        /// to determine the visbility of the student view link
        /// </summary>
        public bool ShowStudentViewLink { get; set; }

        /// <summary>
        /// Property that stores the name of the Page Wizard page to use as the home page
        /// The default value for this property should be "Index". 
        /// </summary>
        public string CourseHomePage
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_home_page", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_home_page", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the name of the Page Wizard page to use as the start page
        /// </summary>
        public string CourseStartPage
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_home_page_start", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_home_page_start", PropertyType.String, value);
            }
        }
        public string StudentStartPage
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_student_page_start", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_student_page_start", PropertyType.String, value);
            }
        }
        public string InstructorStartPage
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_instructor_page_start", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_instructor_page_start", PropertyType.String, value);
            }
        }

        /// <summary>
        /// This is the course theme
        /// contains the name of the theme that is applied to the course. default is "default_theme"
        /// </summary>
        public string Theme
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_theme", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_theme", PropertyType.String, value);
            }
        }

        
        /// <summary>
        /// The welcome displayed to students
        ///  contains HTML that acts as the course's welcome message to students.
        /// </summary>
        public string WelcomeMessage 
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_welcome_message", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_welcome_message", PropertyType.String, value);
            }
        }

        /// <summary>
        /// stores the path to a resource file used for the course's banner graphic.
        /// </summary>
        public string BannerImage
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_banner", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_banner", PropertyType.String, value);
            }
        }

        public string AllowedThemes 
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_allowed_themes", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_allowed_themes", PropertyType.String, value);
            }
        }

        /// <summary>
        /// stores whether the comment sharing is enabled or disabled
        /// </summary>
        public bool AllowCommentSharing
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_allow_comment_sharing", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_allow_comment_sharing", PropertyType.Boolean, value);
            }
        }


        /// <summary>
        /// Stores Published Status of presentation
        /// </summary>
        public string PublishedStatus
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_shared", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_shared", PropertyType.String, value);
            }
        }



        /// <summary>
        /// stores whether the SearchEngineIndex is enabled or disabled
        /// </summary>
        public bool SearchEngineIndex
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_search_engine_index", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_search_engine_index", PropertyType.Boolean, value);
            }
        }


        /// <summary>
        /// stores whether the Course is SandboxCourse
        /// </summary>
        public bool IsSandboxCourse
        {
            get
            {
                return Properties.GetPropertyValue<bool>("meta-bfw_is_sandbox_course", false);
            }
            set
            {
                Properties.SetPropertyValue("meta-bfw_is_sandbox_course", PropertyType.Boolean, value);
            }
        }




        /// <summary>
        /// stores whether the SearchEngineIndex is enabled or disabled
        /// </summary>
        public bool FacebookIntegration 
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_facebook_integration", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_facebook_integration", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// stores a boolean value that is used to turn on and off social comments across the entire course
        /// </summary>
        public bool SocialCommentingIntegration
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_social_commenting_integration", false);
            }
            set
            {
                Properties.SetPropertyValue("bfw_social_commenting_integration", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// stores a list of object types that are allowed to have social commenting
        /// </summary>
        protected String[] socialCommentingAllowedTypes = null;
        public String[] SocialCommentingAllowedTypes
        {
            get
            {
                if (null == socialCommentingAllowedTypes)
                {
                    socialCommentingAllowedTypes = Properties.GetPropertyValue<String>("bfw_social_commenting_allowed_types", "").Split(',', '|');
                }
                return socialCommentingAllowedTypes;
            }
        }

        /// <summary>
        /// stores whether course activation is enabled or disabled
        /// </summary>
        public bool AllowActivation
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_allow_activation", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_allow_activation", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// stores whether the Course launches the start page as landing page
        /// </summary>
        public bool IsLoadStartOnInit
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_isloadstartoninit", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_isloadstartoninit", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// stores office hours for the course
        /// </summary>
        public string OfficeHours
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_office_hours", string.Empty);
            }
            set
            {
                Properties.SetPropertyValue("bfw_office_hours", PropertyType.String, value);
            }
        }

        /// <summary>
        /// stores whether the site search is enabled or disabled
        /// </summary>
        public bool AllowSiteSearch
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_allow_site_search", true);
            }
            set
            {
                Properties.SetPropertyValue("bfw_allow_site_search", PropertyType.Boolean, value);
            }
        }


        /// <summary>
        /// Stores Locked learning objectives
        /// </summary>
        public List<string> LockedLearningObjectives
        {
            get
            {
                var lockedLo = Properties.GetPropertyValue<string>("bfw_locked_lo", "");
                return string.IsNullOrEmpty(lockedLo) ? new List<string>() : new List<string>(lockedLo.Split(','));
            }
            set
            {
                Properties.SetPropertyValue("bfw_locked_lo", PropertyType.String, string.Join<string>(",", value));
            }
        }

        /// <summary>
        /// Stores Quetion Card Layout
        /// </summary>
        public string QuestionCardLayout
        {
            get
            {
                return Properties.GetPropertyValue<string>("questioncardlayout", string.Empty);
            }
            set
            {
                Properties.SetPropertyValue("questioncardlayout", PropertyType.String, value);
            }
        }

        public bool EnableLoadingScreen
        {
            get
            {
                return Properties.GetPropertyValue<bool>("enableloadingscreen", false);
            }
            set
            {
                Properties.SetPropertyValue("enableloadingscreen", PropertyType.Boolean, value);
            }
        }

        /// <summary>
        /// List of available table of content categories for this course.
        /// </summary>
        /// <value>
        /// A collection of <see cref="TocCategory" /> items.
        /// </value>

        private IList<TocCategory> _categories = null;
        public IList<TocCategory> Categories 
        {
            get { return _categories; }
            set { _categories = value; }
        }

        /// <summary>
        /// settings for the assign tab which is used to assign the content items
        /// </summary>
        [DataMember]
        public AssignTabSettings AssignTabSettings { get; set; }

        /// <summary>
        /// Search schema/categories supported by the course.
        /// </summary>
        /// <value>
        /// A <see cref="SearchSchema" /> value.
        /// </value>
        [DataMember]
        public SearchSchema SearchSchema { get; set; }

        /// <summary>
        /// Question Filter.
        /// </summary>
        [DataMember]
        public QuestionFilter QuestionFilter { get; set; }

        
        /// <summary>
        /// Facet Search schema/categories supported by the course.
        /// </summary>
        /// <value>
        /// A <see cref="FacetSearchSchema" /> value.
        /// </value>
        [DataMember]
        public FacetSearchSchema FacetSearchSchema { get; set; }

        /// <summary>
        /// List of feeds available in the course
        /// </summary>
        public List<RSSCourseFeed> RSSCourseFeeds { get; set; }

        /// <summary>
        /// Assessment configuration for the course.
        /// </summary>
        /// <value>
        /// A <see cref="CourseAssessmentConfiguration" /> value.
        /// </value>
        [DataMember]
        public CourseAssessmentConfiguration AssessmentConfiguration { get; set; }

        /// <summary>
        /// Set of properties stored in the item
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        [DataMember]
        public IDictionary<string, MetadataValue> Metadata { get; set; }

        /// <summary>
        /// Gets/Sets ability to diable commenting/annotations for the entire course.
        /// </summary>
        public bool DisableComments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// </summary>
        public Course()
        {
            Properties = new Dictionary<string, PropertyValue>();
            
            //default value of show_student_view_link should be true
            ShowStudentViewLink = true;
            RSSCourseFeeds = new List<RSSCourseFeed>();
            QuestionFilter = new QuestionFilter();
            QuestionCardData = new List<QuestionCardData>();
            TitleCSSParameters = new List<CSSParameter>();
        }

        /// <summary>
        /// Collection of learning objectives tied to the course.
        /// </summary>
        public IEnumerable<LearningObjective> LearningObjectives { get; set; }

        /// <summary>
        /// Collection of contact details tied to the course.
        /// </summary>
        public List<ContactInfo> ContactInformation { get; set; }

        /// <summary>
        /// Syllabus Name
        /// </summary>
        public string Syllabus { get; set; }

        /// <summary>
        /// Syllabus Type: Url or File
        /// </summary>
        public string SyllabusType { get; set; }

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
        /// Collection of learning objectives tied to the course.
        /// This is duplicate but we need to do this to capture extra attributes(parentid, sequence etc).
        /// </summary>
        public IEnumerable<LearningObjective> EportfolioLearningObjectives { get; set; }

        /// <summary>
        /// Collection of dashboard learning objectives tied to the course.
        /// </summary>
        public IEnumerable<LearningObjective> DashboardLearningObjectives { get; set; }

        /// <summary>
        /// Collection of publishers template learning objectives tied to the course.
        /// </summary>
        public IEnumerable<LearningObjective> PublisherTemplateLearningObjectives { get; set; }

        /// <summary>
        /// Collection of program template learning objectives tied to the course.
        /// </summary>
        public IEnumerable<LearningObjective> ProgramLearningObjectives { get; set; }

        /// <summary>
        /// Structure holding instructor console settings
        /// </summary>
        public InstructorConsoleSettings ConsoleSettings { get; set; }

        /// <summary>
        /// True if sampling is allowed
        /// </summary>
        public bool AllowSampling { get; set; }

        /// <summary>
        /// True if purchase is allowed
        /// </summary>
        public bool AllowPurchase { get; set; }

        /// <summary>
        /// trial access to the course, default is false
        /// </summary>
        public bool AllowTrialAccess { get; set; }

        /// <summary>
        /// Specifies if LMSId is mandatory
        /// </summary>
        public bool LmsIdRequired { get; set; }

        /// <summary>
        /// Label to be displayed when entering LMSId
        /// </summary>
        public string LmsIdLabel { get; set; }

        /// <summary>
        /// Prompt to be displayed when entering LMSId
        /// </summary>
        public string LmsIdPrompt { get; set; }

        /// <summary>
        /// Adjust time to the course time zone
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public DateTime UtcRelativeAdjust(DateTime dt)
        {
            return Bfw.Common.DateTimeConversion.UtcRelativeAdjustCommon(dt, CourseTimeZone);
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
        /// Instructor can switch domain of product. This flag is at product course level
        /// </summary>
        public string DerivedCourseId { get; set; }

        /// <summary>
        /// Settings for dashboard
        /// </summary>
        public DashboardSettings DashboardSettings { get; set; }

        /// <summary>
        /// Flage whether to use weighted categories
        /// </summary>
        public bool UseWeightedCategories { get; set; }

        /// <summary>
        /// When true, enable arga url transformation
        /// </summary>
        public bool EnableArgaUrlMapping{ get; set; }

        /// <summary>
        /// List of Question card metadata for a question
        /// </summary>
        public List<QuestionCardData> QuestionCardData { get; set; }

        /// <summary>
        /// Number of students enrolled into the course
        /// </summary>
        public int StudentEnrollmentCount { get; set; }


        public static Comparison<Course> IDComparison = delegate(Course course1, Course course2)
        {
            return course1.Id.CompareTo(course2.Id);
        };

        public int CompareTo(Course other)
        {
            return Id.CompareTo(other.Id);
        }
    }

    [Serializable]
    public class CSSParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }

    }

    public class CourseEqualityComparer : IEqualityComparer<Course>
    {

        public bool Equals(Course x, Course y)
        {
            if (x == null || y == null)
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(Course obj)
        {
            return obj == null ? 0 : int.Parse(obj.Id);
        }
    }
}