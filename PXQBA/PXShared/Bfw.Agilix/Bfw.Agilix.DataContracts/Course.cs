using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Runtime.Serialization;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using System.Xml;
using System.Xml.XPath;

namespace Bfw.Agilix.DataContracts
{
	/// <summary>
	/// Conforms to http://dev.dlap.bfwpub.com/Docs/Schema/CourseData
	/// </summary>
	public class Course : IDlapEntityParser
	{
		#region Properties

		/// <summary>
		///Course ID
		/// </summary>
		[DataMember]
		public string Id { get; set; }

		/// <summary>
		/// Parent ID
		/// </summary>
		[DataMember]
		public string ParentId { get; set; }

		/// <summary>
		///Course Title
		/// </summary>
		[DataMember]
		public string Title { get; set; }
		
		/// <summary>
		/// Activated date
		/// </summary>
		[DataMember]
		public string ActivatedDate { get; set; }

		/// <summary>
		///Course Title
		/// </summary>
		[DataMember]
		public string CreationDate { get; set; }
		/// <summary>
		///Reference
		/// </summary>
		[DataMember]
		public string Reference { get; set; }

		/// <summary>
		/// Domian of the Course
		/// </summary>
		[DataMember]
		public Domain Domain { get; set; }

		/// <summary>
		/// Information about the domain the course belongs to
		/// </summary>
		[DataMember]
		public string EnrollmentId { get; set; }

		/// <summary>
		/// product course id - root course id
		/// </summary>
		[DataMember]
		public string ProductCourseId { get; set; }

		/// <summary>
		/// A flag which indicates whether the course needs to be accessed in a readonly mode
		/// </summary>
		[DataMember]
		public string ReadOnly { get; set; }

		/// <summary>
		/// Course Number for the course
		/// </summary>
		[DataMember]
		public string CourseNumber { get; set; }

		/// <summary>
		/// Section Number for the course
		/// </summary>
		[DataMember]
		public string SectionNumber { get; set; }

		/// <summary>
		/// Minimal score percentage to be achieved
		/// </summary>
		[DataMember]
		public double PassingScore { get; set; }

		///// <summary>
		///// grade book categories drop down
		///// </summary>
		//[DataMember]
		//public List<GradeBookWeightCategory> GradeBookWeightCategoryList { get; set; }


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
        /// 
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
		/// Collection of learning objectives associated with the course.
		/// </summary>
		public IEnumerable<LearningObjective> LearningObjectives { get; set; }

		/// <summary>
		/// Collection of learning objectives tied to the course.
		/// This is duplicate but we need to do this to capture extra attributes(parentid, sequence etc).
		/// </summary>
		public IEnumerable<LearningObjective> EportfolioLearningObjectives { get; set; }

		/// <summary>
		/// Collection of contact details tied to the course.
		/// </summary>
		public XElement ContactInformation { get; set; }

		/// <summary>
		/// Syllabus information: type, filename or link name
		/// </summary>
		public XElement SyllabusInformation { get; set; }

		/// <summary>
		/// Collection of dashboard learning objectives associated with the course.
		/// </summary>
		public IEnumerable<LearningObjective> DashboardLearningObjectives { get; set; }

		/// <summary>
		/// Collection of dashboard learning objectives associated with the course.
		/// </summary>
		public IEnumerable<LearningObjective> PublisherTemplateLearningObjectives { get; set; }

		/// <summary>
		/// Collection of dashboard learning objectives associated with the course.
		/// </summary>
		public IEnumerable<LearningObjective> ProgramLearningObjectives { get; set; }

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
		/// Sub Type Of Course
		/// </summary>
		public string SubType { get; set; }
		/// <summary>
		/// Settings for dashboard
		/// </summary>
		public DashboardSettings DashboardSettings { get; set; }

		/// <summary>
		/// List of Question card metadata for a question
		/// </summary>
		public List<QuestionCardData> QuestionCardData { get; set; }

        /// <summary>
        /// Repository Course id
        /// </summary>
        public string QuestionBankRepositoryCourse { get; set; }
		
		/// <summary>
		/// Instructor can switch domain of product. This flag is at product course level
		/// </summary>
		public string DerivedCourseId { get; set; }

		public bool UseWeightedCategories { get; set; }

		#endregion

		#region IDlapEntityParser Members

   

		/// <summary>
		/// See (~/Docs/Schema/User) for format
		/// </summary>
		/// <returns></returns>
		public XElement ToEntity()
		{
			var element = new XElement("course");

			if (!string.IsNullOrEmpty(Id))
				element.Add(new XAttribute("courseid", Id));

			if (!string.IsNullOrEmpty(Title))
				element.Add(new XAttribute("title", Title));

			if (!string.IsNullOrEmpty(Reference))
				element.Add(new XAttribute("reference", Reference));
			
			if (Domain != null && !string.IsNullOrEmpty(Domain.Id))
				element.Add(new XAttribute("domainid", Domain.Id));
			
			if (!string.IsNullOrEmpty(ActivatedDate))
			{
				var dtActivated = Convert.ToDateTime(ActivatedDate);
				element.Add(new XAttribute("startdate", (dtActivated == DateTime.MinValue)? DateRule.Format(DateRule.MinDate) : DateRule.Format(dtActivated)));
			}

			if (!string.IsNullOrEmpty(EnrollmentId))
				element.Add(new XAttribute("enrollmentid", EnrollmentId));

			if (!string.IsNullOrEmpty(EnrollmentId))
				element.Add(new XAttribute("Parentid", EnrollmentId));

			var dataElement = new XElement("data");

			if (Data != null)
			{
				XElement properties = Data.Element("bfw_properties");
                if (properties != null)
                {
                    foreach (var item in properties.Elements())
                    {
                        var el = new XElement(item.Attribute("name").Value);

                        if (el.Name == "questioncardlayout")
                        {
                            if (item.Value != string.Empty)
                                el = XElement.Parse(item.Value);
                            else
                                el.SetValue(" ");
                        }
                        else
                        {
                            el.SetValue(item.Value ?? " ");
                        }

                        dataElement.Add(el);
                    }
                }

                //TODO: remove later - bfw activateddate element is replaced with startdate attribute
                dataElement.Add(new XElement("ActivatedDate"));

				var gbb_categories = Data.Element("categories");
				if (gbb_categories != null)
				{
					dataElement.Add(gbb_categories);
				}

				if (!string.IsNullOrEmpty(DerivedCourseId))
				{
					element.Add(new XAttribute("sourceid", DerivedCourseId));
					element.Add(new XElement("sourceid", DerivedCourseId));
				}

			    if (DashboardSettings != null)
			    {
				    var dashboard_settings = new XElement("bfw_dashboard_settings");

				    var dashboard_course_home_page = new XElement("bfw_dashboard_course_home_page", DashboardSettings.DashboardHomePageStart);

				    var program_dashboard_course_home_page = new XElement("bfw_program_dashboard_course_home_page", DashboardSettings.ProgramDashboardHomePageStart);
				    var instructor_dashboard_on = new XElement("instructor_dashboard", DashboardSettings.IsInstructorDashboardOn.ToString());
				    var program_dashboard_on = new XElement("program_dashboard", DashboardSettings.IsProgramDashboardOn.ToString());
                
				    dashboard_settings.Add(dashboard_course_home_page);
				    dashboard_settings.Add(instructor_dashboard_on);
				    dashboard_settings.Add(program_dashboard_on);
				    dashboard_settings.Add(program_dashboard_course_home_page);

				    dataElement.Add(dashboard_settings);
			    }

			    var contactInfo = Data.Element("bfw_contact_info");
			    if (contactInfo != null)
			    {
				    dataElement.Add(contactInfo);
			    }

			    var syllabusInfo = Data.Element("bfw_syllabus_info");
			    if (syllabusInfo != null)
			    {
				    dataElement.Add(syllabusInfo);
			    }

			    var objectives = Data.Element("learningobjectives");
			    if (objectives != null)
			    {
				    dataElement.Add(objectives);
			    }

			    // custom learning objectives
			    foreach (String nodeName in new string[]
											    {
												    "bfw_template_learningobjectives",
												    "bfw_dashboard_learningobjectives",
												    "bfw_eportfolio_learningobjectives",
												    "bfw_program_learningobjectives"
											    })
			    {
				    var learningObjectives = Data.Element(nodeName);
				    if (null != learningObjectives)
					    dataElement.Add(learningObjectives);
			    }

			    //mishka add product course id to xml
			    if (Data.Element("meta-product-course-id") != null)
				    dataElement.Add(Data.Element("meta-product-course-id"));

				    //course activation property
				    if (Data.Element("bfw_allow_activation") != null)
					    dataElement.Add(Data.Element("bfw_allow_activation"));

			    if (Data.Element("bfw_isloadstartoninit") != null)
				    dataElement.Add(Data.Element("bfw_isloadstartoninit"));

			    if (Data.Element("bfw_office_hours") != null)
				    dataElement.Add(Data.Element("bfw_office_hours"));

			    if(Data.Element("bfw_read_only") != null)
				    dataElement.Add(Data.Element("bfw_read_only"));
                			
			    // Save the group sets for this course
			    var groupSetsEl = Data.Element("groupsets");
			    dataElement.Add(groupSetsEl);

			    var genericCourse = Data.Element("bfw_generic_course");
			    if (genericCourse != null)
			    {
                    if (!string.IsNullOrEmpty(GenericCourseId))
                    {
                        if (genericCourse.Attribute("id") == null)
                        {
                            genericCourse.Add(new XAttribute("id", GenericCourseId));
                        }
                        else
                        {
                            genericCourse.Attribute("id").Value = GenericCourseId;
                        }
                    }

				    dataElement.Add(genericCourse);
			    }

			    if (Data.Element("bfw_enrollment_switch") != null)
				    dataElement.Add(Data.Element("bfw_enrollment_switch"));

			    if (Data.Element("bfw_course_domain_switch") != null)
				    dataElement.Add(Data.Element("bfw_course_domain_switch"));

			    if (Data.Element("meta-bfw_course_subtype") != null)
				    dataElement.Add(Data.Element("meta-bfw_course_subtype"));

			    if (Data.Element("meta-course_subtype") != null)
				    dataElement.Add(Data.Element("meta-course_subtype"));

                if (Data.Element("bfw_lmsid_required") != null)
                    dataElement.Add(Data.Element("bfw_lmsid_required"));

                if (Data.Element("bfw_lmsid_label") != null)
                    dataElement.Add(Data.Element("bfw_lmsid_label"));

                if (Data.Element("bfw_lmsid_prompt") != null)
                    dataElement.Add(Data.Element("bfw_lmsid_prompt"));

			    if (Data.Element("passingscore") != null)
				    dataElement.Add(Data.Element("passingscore"));            

			    element.Add(dataElement);
			}

		    if (QuestionCardData != null)
		    {
		        var metaAvailableQuestionData = dataElement.Element("meta-available-question-data");
                if(metaAvailableQuestionData == null)
                {
                    metaAvailableQuestionData = new XElement("meta-available-question-data");
                    dataElement.Add(metaAvailableQuestionData);
                }
                
		        foreach (var questionCardData in QuestionCardData)
		        {
		            metaAvailableQuestionData.Add(questionCardData.ToEntity());
		        }
		    }

			if (!string.IsNullOrEmpty(DerivedCourseId))
			{
				dataElement.Add(new XElement("sourceid", DerivedCourseId));
			}

			return element;
		}

		/// <summary>
		/// Parses an XElement into internal object state. This allows for objects to be decomposed from
		/// parts of Dlap responses.
		/// </summary>
		/// <param name="element">element that contains the state to parse</param>
		/// <remarks></remarks>
		public void ParseEntity(System.Xml.Linq.XElement element)
		{
			var courseid = element.Attribute("courseid");
			if (courseid == null)
			{
				courseid = element.Attribute("id");
			}
			var title = element.Attribute("title");
			var domainid = element.Attribute("domainid");
			var reference = element.Attribute("reference");
			var enrollmentid = element.Attribute("enrollmentid");
			var domainname = element.Attribute("domainname");
			var baseid = element.Attribute("baseid");
			var startDate = element.Attribute("startdate");
			var creationdate = element.Attribute("creationdate");
			var sectionnumber = element.Attribute("SectionNumber");			

			if (null != courseid)
			{
				Id = courseid.Value;
			}

			if (null != title)
			{
				Title = title.Value;
			}

			Domain = new Domain();

			if (null != domainid)
			{
				Domain.Id = domainid.Value;
			}

			if (null != domainname)
				Domain.Name = domainname.Value;

			if (null != reference)
			{
				Reference = reference.Value;
			}

			if (null != enrollmentid)
			{
				EnrollmentId = enrollmentid.Value;
			}

			if (null != baseid)
			{
				ParentId = baseid.Value;
			}

			if (null != startDate)
			{
				ActivatedDate = (Convert.ToDateTime(startDate.Value) == DateRule.MinDate)? DateTime.MinValue.ToString() : startDate.Value;
			}

			if (null != creationdate)
			{
				CreationDate = creationdate.Value;
			}

			if (element.Element("data") == null) { Data = new XElement("data"); }
			else { Data = element.Element("data"); }

			Data.Add(new XElement("bfw_type", "type"));
			var propsElm = new XElement("bfw_properties");

			var responseData = Data;

			var courseProductName = "";
			var courseUserName = "";
			var courseTimeZone = "";
			var parentId = "";
			var productCourseId = "";
			var courseType = "";
			var courseOwner = "";
			var courseTemplate = "";
			var dashboardCourseId = "";
			var courseHomePage = "";
			var courseStartPage = "";
			var studentStartPage = "";
			var instructorStartPage = "";

			var theme = "";
			var welcomeMessage = "";
			var bannerImage = "";
			var allowedThemes = "";
			var allowCommentSharing = true;
			var bfwshared = "";
			var academicTerm = "";
			var coursenumber = "";
			var sectionumber = "";
			var officeHours = string.Empty;

			var isbn13 = "";

			var searchEngineIndex = false;
			var facebookIntegration = false;
			var socialCommentingIntegration = false;
			var socialCommentingAllowedTypes = "";
			var hideStudentViewLink = false;
			var allowSiteSearch = true;
			var allowActivation = true;
			var isLoadStartOnInit = false;
			var enableLearningCurveQuiz = true;
			var questioncardlayout = "";
			var enableHomeworkQuiz = true;
			var enableLoadingScreen = false;

			var lockedLO = "";

			var downloadOnlyDocumentTypes = "";

			var courseDescription = "";
			var courseAuthor = "";

			var isSandboxCourse = false;
            var disableComments = false;

			if (responseData != null)
			{
				courseProductName = (responseData.Element("CourseProductName") == null) ? "" : responseData.Element("CourseProductName").Value;
				courseUserName = (responseData.Element("CourseUserName") == null) ? "" : responseData.Element("CourseUserName").Value;
				courseTimeZone = (responseData.Element("CourseTimeZone") == null) ? "" : responseData.Element("CourseTimeZone").Value;

                //TODO: remove later - bfw activateddate element is replaced with startdate attribute
                var bfw_activatedDate = (responseData.Element("ActivatedDate") == null) ? "" : responseData.Element("ActivatedDate").Value;
                if (!String.IsNullOrWhiteSpace(bfw_activatedDate)
                    && Convert.ToDateTime(bfw_activatedDate).Date != DateTime.MaxValue.Date
                    && Convert.ToDateTime(bfw_activatedDate).Date != DateTime.MinValue.Date
                    && Convert.ToDateTime(ActivatedDate) < Convert.ToDateTime(bfw_activatedDate))
                {
                    ActivatedDate = bfw_activatedDate;
                }

				coursenumber = (responseData.Element("CourseNumber") == null) ? "" : responseData.Element("CourseNumber").Value;
				sectionumber = (responseData.Element("SectionNumber") == null) ? "" : responseData.Element("SectionNumber").Value;
				parentId = (responseData.Element("ParentId") == null) ? "" : responseData.Element("ParentId").Value;
				productCourseId = (responseData.Element("meta-product-course-id") == null) ? "" : responseData.Element("meta-product-course-id").Value;

				courseType = (responseData.Element("bfw_course_type") == null) ? "" : responseData.Element("bfw_course_type").Value;
				courseOwner = (responseData.Element("bfw_course_owner") == null) ? "" : responseData.Element("bfw_course_owner").Value;
				courseDescription = (responseData.Element("description") == null) ? "" : responseData.Element("description").Value;
				courseAuthor = (responseData.Element("bfw_course_author") == null) ? "" : responseData.Element("bfw_course_author").Value;
				courseTemplate = (responseData.Element("bfw_course_template") == null) ? "" : responseData.Element("bfw_course_template").Value;
				dashboardCourseId = (responseData.Element("bfw_dashboard_course") == null) ? "" : responseData.Element("bfw_dashboard_course").Value;
				courseHomePage = (responseData.Element("bfw_course_home_page") == null) ? "" : responseData.Element("bfw_course_home_page").Value;
				courseStartPage = (responseData.Element("bfw_course_home_page_start") == null) ? "" : responseData.Element("bfw_course_home_page_start").Value;
				studentStartPage = (responseData.Element("bfw_student_page_start") == null) ? "" : responseData.Element("bfw_student_page_start").Value;
				instructorStartPage = (responseData.Element("bfw_instructor_page_start") == null) ? "" : responseData.Element("bfw_instructor_page_start").Value;
				isbn13 = (responseData.Element("meta-book-isbn") == null) ? "" : responseData.Element("meta-book-isbn").Value;

				theme = (responseData.Element("bfw_course_theme") == null) ? "" : responseData.Element("bfw_course_theme").Value;
				welcomeMessage = (responseData.Element("bfw_welcome_message") == null) ? "" : responseData.Element("bfw_welcome_message").Value;
				bannerImage = (responseData.Element("bfw_banner") == null) ? "" : responseData.Element("bfw_banner").Value;
				allowedThemes = (responseData.Element("bfw_allowed_themes") == null) ? "" : responseData.Element("bfw_allowed_themes").ToString();
				if (responseData.Element("bfw_allow_comment_sharing") != null) { bool.TryParse(responseData.Element("bfw_allow_comment_sharing").Value, out allowCommentSharing); }
				if (responseData.Element("bfw_hide_student_view_in_header") != null) { bool.TryParse(responseData.Element("bfw_hide_student_view_in_header").Value, out hideStudentViewLink); }
				bfwshared = (responseData.Element("bfw_shared") == null) ? "Unpublished" : responseData.Element("bfw_shared").Value;
				academicTerm = (responseData.Element("meta-bfw_academic_term") == null) ? "" : responseData.Element("meta-bfw_academic_term").Value;
				if (responseData.Element("enable_learningcurve_quiz") != null) { bool.TryParse(responseData.Element("enable_learningcurve_quiz").Value, out enableLearningCurveQuiz); }
				
				if (responseData.Element("enable_homework_quiz") != null) { bool.TryParse(responseData.Element("enable_homework_quiz").Value, out enableHomeworkQuiz); }
				if (responseData.Element("enableloadingscreen") != null) { bool.TryParse(responseData.Element("enableloadingscreen").Value, out enableLoadingScreen); }

				if (responseData.Element("bfw_search_engine_index") != null) { bool.TryParse(responseData.Element("bfw_search_engine_index").Value, out searchEngineIndex); }
				if (responseData.Element("bfw_facebook_integration") != null) { bool.TryParse(responseData.Element("bfw_facebook_integration").Value, out facebookIntegration); }
				if (responseData.Element("bfw_social_commenting_integration") != null) { bool.TryParse(responseData.Element("bfw_social_commenting_integration").Value, out socialCommentingIntegration); }
				socialCommentingAllowedTypes = (responseData.Element("bfw_social_commenting_allowed_types") == null) ? "" : responseData.Element("bfw_social_commenting_allowed_types").Value;
				if (responseData.Element("bfw_allow_site_search") != null) { bool.TryParse(responseData.Element("bfw_allow_site_search").Value, out allowSiteSearch); }

				lockedLO = (responseData.Element("bfw_locked_lo") == null) ? "" : responseData.Element("bfw_locked_lo").Value;

				questioncardlayout = (responseData.Element("questioncardlayout") == null) ? "" : responseData.Element("questioncardlayout").Value;

				if (responseData.Element("bfw_allow_activation") != null) { bool.TryParse(responseData.Element("bfw_allow_activation").Value, out allowActivation); }

				if (responseData.Element("bfw_isloadstartoninit") != null) { bool.TryParse(responseData.Element("bfw_isloadstartoninit").Value, out isLoadStartOnInit); }

				officeHours = responseData.Element("bfw_office_hours") == null ? string.Empty : responseData.Element("bfw_office_hours").Value;

				downloadOnlyDocumentTypes = (responseData.Element("bfw_download_only_document_types") == null) ? "" : responseData.Element("bfw_download_only_document_types").Value;

				if (responseData.Element("meta-bfw_is_sandbox_course") != null) { bool.TryParse(responseData.Element("meta-bfw_is_sandbox_course").Value, out isSandboxCourse); }

                //Ability to disable comments at course level.  Used in XBookv2 until we get commenting worked out.
			    if (responseData.Element("bfw_disable_comments") != null)
			    {
			        if(!bool.TryParse(responseData.Element("bfw_disable_comments").Value, out disableComments))
                        disableComments = false;
			    }
			}

			//if (String.IsNullOrEmpty(activatedDate))
			//    activatedDate = DateTime.MinValue.ToString();

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "CourseProductName"),
									new XAttribute("type", "string"),
									courseProductName));
			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "CourseUserName"),
									new XAttribute("type", "string"),
									courseUserName));
			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "CourseTimeZone"),
									new XAttribute("type", "string"),
									courseTimeZone));
			//propsElm.Add(new XElement("bfw_property",
			//                        new XAttribute("name", "ActivatedDate"),
			//                        new XAttribute("type", "string"),
			//                        activatedDate));
			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "ParentId"),
									new XAttribute("type", "string"),
									parentId));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_type"),
									new XAttribute("type", "string"),
									courseType));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_owner"),
									new XAttribute("type", "string"),
									courseOwner));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_description"),
									new XAttribute("type", "string"),
									courseDescription));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_author"),
									new XAttribute("type", "string"),
									courseAuthor));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_template"),
									new XAttribute("type", "string"),
									courseTemplate));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_dashboard_course"),
									new XAttribute("type", "string"),
									dashboardCourseId));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_home_page"),
									new XAttribute("type", "string"),
									courseHomePage));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_home_page_start"),
									new XAttribute("type", "string"),
									courseStartPage));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_instructor_page_start"),
									new XAttribute("type", "string"),
									instructorStartPage));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_student_page_start"),
									new XAttribute("type", "string"),
									studentStartPage));

			propsElm.Add(new XElement("bfw_property",
								   new XAttribute("name", "bfw_course_type"),
								   new XAttribute("type", "string"),
								   courseType));

			propsElm.Add(new XElement("bfw_property",
								   new XAttribute("name", "meta-bfw_course_type"),
								   new XAttribute("type", "string"),
								   courseType));


			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_course_theme"),
									new XAttribute("type", "string"),
									theme));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_welcome_message"),
									new XAttribute("type", "string"),
									welcomeMessage));

			propsElm.Add(new XElement("bfw_property",
								   new XAttribute("name", "bfw_banner"),
								   new XAttribute("type", "string"),
								   bannerImage));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_allowed_themes"),
									new XAttribute("type", "string"),
									allowedThemes));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_allow_comment_sharing"),
									new XAttribute("type", "bool"),
									allowCommentSharing));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_hide_student_view_in_header"),
									new XAttribute("type", "bool"),
									hideStudentViewLink));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "enable_learningcurve_quiz"),
									new XAttribute("type", "bool"),
									enableLearningCurveQuiz));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "enable_homework_quiz"),
									new XAttribute("type", "bool"),
									enableHomeworkQuiz));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "enableloadingscreen"),
									new XAttribute("type", "bool"),
									enableLoadingScreen));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_shared"),
									new XAttribute("type", "string"),
									bfwshared));
			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "meta-bfw_academic_term"),
									new XAttribute("type", "string"),
									academicTerm));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_search_engine_index"),
									new XAttribute("type", "bool"),
									searchEngineIndex));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_facebook_integration"),
									new XAttribute("type", "bool"),
									facebookIntegration));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_allow_site_search"),
									new XAttribute("type", "bool"),
									allowSiteSearch));
			
			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "CourseNumber"),
									new XAttribute("type", "string"),
									coursenumber));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_allow_activation"),
									new XAttribute("type", "bool"),
									allowActivation));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_isloadstartoninit"),
									new XAttribute("type", "bool"),
									isLoadStartOnInit));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_office_hours"),
									new XAttribute("type", "string"),
									officeHours));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "bfw_locked_lo"),
									new XAttribute("type", "string"),
									lockedLO));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "questioncardlayout"),
									new XAttribute("type", "string"),
									questioncardlayout));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "SectionNumber"),
									new XAttribute("type", "string"),
									sectionumber));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "meta-book-isbn"),
									new XAttribute("type", "string"),
									isbn13));

			propsElm.Add(new XElement("bfw_property",
						new XAttribute("name", "bfw_download_only_document_types"),
						new XAttribute("type", "string"),
						downloadOnlyDocumentTypes));

			propsElm.Add(new XElement("bfw_property",
						new XAttribute("name", "bfw_social_commenting_integration"),
						new XAttribute("type", "bool"),
						socialCommentingIntegration));
			
			propsElm.Add(new XElement("bfw_property",
						new XAttribute("name", "bfw_social_commenting_allowed_types"),
						new XAttribute("type", "string"),
						socialCommentingAllowedTypes));

			propsElm.Add(new XElement("bfw_property",
									new XAttribute("name", "meta-bfw_is_sandbox_course"),
									new XAttribute("type", "bool"),
									isSandboxCourse));
			
			Data.Add(propsElm);

			var questionCard = Data.Element("meta-available-question-data");
			List<QuestionCardData> questionCardResults = new List<QuestionCardData>();
			if (questionCard != null)
			{
				foreach (XElement questiondataXml in questionCard.Elements())
				{
					var questiondata = new QuestionCardData();
					if (questiondataXml != null)
					{

						questiondata.ParseEntity(questiondataXml);

						if (questiondata != null)
						{
							questionCardResults.Add(questiondata);

						}
					}

				}
				QuestionCardData = questionCardResults;
			}

		    var questionBankRepository = Data.Element("QuestionBankRepositoryCourse");
            if (questionBankRepository != null)
            {
                QuestionBankRepositoryCourse = questionBankRepository.Value;
            }

			var objectives = Data.Element("learningobjectives");
			if (objectives != null)
			{
				var learningObjectives = new List<LearningObjective>();
				foreach (var objective in objectives.Elements("objective"))
				{
					var learningObjective = new LearningObjective();
					learningObjective.ParseEntity(objective);

					learningObjectives.Add(learningObjective);
				}

				LearningObjectives = learningObjectives;
			}

			foreach (String nodeName in new string[]
												{
													"bfw_template_learningobjectives",
													"bfw_dashboard_learningobjectives",
													"bfw_eportfolio_learningobjectives",
													"bfw_program_learningobjectives"
												})
			{
				var learningObjectiveType = Data.Element(nodeName);
				if (learningObjectiveType == null)
					continue; // if learning objective type element could not be found, skip it

				var lObjectives = learningObjectiveType.Element("learningobjectives");
				if (lObjectives == null)
					continue; // if learning objective elements could not be found, skip it

				var learningObjectives = new List<LearningObjective>();
				foreach (var objective in lObjectives.Elements("objective"))
				{
					var learningObjective = new LearningObjective();
					learningObjective.ParseEntity(objective);
					learningObjectives.Add(learningObjective);
				}

				if (false) ; // i enjoy when things line up
				else if (nodeName == "bfw_template_learningobjectives")
					PublisherTemplateLearningObjectives = learningObjectives;
				else if (nodeName == "bfw_dashboard_learningobjectives")
					DashboardLearningObjectives = learningObjectives;
				else if (nodeName == "bfw_eportfolio_learningobjectives")
					EportfolioLearningObjectives = learningObjectives;
				else if (nodeName == "bfw_program_learningobjectives")
					ProgramLearningObjectives = learningObjectives;
			}

			// make sure they have been defaulted to an empty list and not NULL
			PublisherTemplateLearningObjectives = PublisherTemplateLearningObjectives ?? new List<LearningObjective>();
			DashboardLearningObjectives = DashboardLearningObjectives ?? new List<LearningObjective>();
			EportfolioLearningObjectives = EportfolioLearningObjectives ?? new List<LearningObjective>();
			ProgramLearningObjectives = ProgramLearningObjectives ?? new List<LearningObjective>();

			bool flag = false;
			string genericCourseId = string.Empty;

			if (Data.Element("bfw_generic_course") != null)
			{
				bool.TryParse(Data.Element("bfw_generic_course").Value, out flag);
				genericCourseId = (Data.Element("bfw_generic_course").Attribute("id") == null) ? string.Empty : Data.Element("bfw_generic_course").Attribute("id").Value;
			}

			GenericCourseSupported = flag;
			GenericCourseId = genericCourseId;

			flag = false;
			if (Data.Element("bfw_enrollment_switch") != null)
			{
				bool.TryParse(Data.Element("bfw_enrollment_switch").Value, out flag);
			}
			EnrollmentSwitchSupported = flag;

			flag = false;
			if (Data.Element("bfw_course_domain_switch") != null)
			{
				bool.TryParse(Data.Element("bfw_course_domain_switch").Value, out flag);
			}
			DomainSwitchSupported = flag;

			SubType = string.Empty;

			if (Data.Element("meta-course_subtype") != null)
			{
				this.SubType = Data.Element("meta-course_subtype").Value;
			}
			else
			{
				this.SubType = "regular";
			}


			CourseSubType = string.Empty;
			if (Data.Element("meta-bfw_course_subtype") != null)
			{
				this.CourseSubType = Data.Element("meta-bfw_course_subtype").Value.ToString();
			}

			DashboardSettings d = new DataContracts.DashboardSettings();
			var dashboardData = Data.Element("bfw_dashboard_settings");

			if (dashboardData != null)
			{
				if (dashboardData.Element("bfw_dashboard_course_home_page") != null)
				{
					d.DashboardHomePageStart = dashboardData.Element("bfw_dashboard_course_home_page").Value.ToString();
				}
				if (dashboardData.Element("bfw_program_dashboard_course_home_page") != null)
				{
					d.ProgramDashboardHomePageStart = dashboardData.Element("bfw_program_dashboard_course_home_page").Value.ToString();
				}
				if (dashboardData.Element("instructor_dashboard") != null)
				{
					d.IsInstructorDashboardOn = bool.Parse(dashboardData.Element("instructor_dashboard").Value);
				}

				if (dashboardData.Element("program_dashboard") != null)
				{
					d.IsProgramDashboardOn = bool.Parse(dashboardData.Element("program_dashboard").Value);
				}
				DashboardSettings = d;
			}            

			if (Data.Element("sourceid") != null)
			{
				DerivedCourseId = Data.Element("sourceid").Value;
			}

			if (Data.Element("categories") != null && Data.Element("categories").Attribute("weighted") != null) 
			{
				UseWeightedCategories = Boolean.Parse(Data.Element("categories").Attribute("weighted").Value);
			}
		}

		#endregion
	}
}
