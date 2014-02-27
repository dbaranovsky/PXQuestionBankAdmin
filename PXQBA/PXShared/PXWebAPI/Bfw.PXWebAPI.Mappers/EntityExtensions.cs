using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using AgxDC = Bfw.Agilix.DataContracts;
using Course = Bfw.PXWebAPI.Models.Course;
using Domain = Bfw.PXWebAPI.Models.Domain;
using Enrollment = Bfw.PXWebAPI.Models.Enrollment;
using GradeBookWeightCategory = Bfw.PXWebAPI.Models.GradeBookWeightCategory;
using LearningObjective = Bfw.PXWebAPI.Models.LearningObjective;
using PxDC = Bfw.PX.Biz.DataContracts;
using Resource = Bfw.PXWebAPI.Models.Resource;
using ResourceStatus = Bfw.PXWebAPI.Models.ResourceStatus;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PXWebAPI.Mappers
{
	public static class EntityExtensions
	{

        public static string removedCategoryId = System.Configuration.ConfigurationManager.AppSettings["FaceplateRemoved"];
		/// <summary>
		/// Convert from an agilix object to user object.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static User ToUser(this AgxDC.AgilixUser user)
		{
			User bizUser = null;

			if (null != user)
			{
				bizUser = new User
							  {
								  Id = user.Id,
								  FirstName = user.FirstName,
								  LastName = user.LastName,
								  ReferenceId = user.Reference,
								  LastLogin = user.LastLogin
							  };
				bizUser.ReferenceId = user.Reference;
				if (user.Credentials != null)
				{
					bizUser.Username = user.Credentials.Username;
					bizUser.Password = user.Credentials.Password;
				}
				else
				{
					bizUser.Username = "";
					bizUser.Password = "";
				}
				bizUser.Email = user.Email;

				if (user.Domain != null)
				{
					bizUser.DomainId = user.Domain.Id;
					bizUser.DomainName = user.Domain.Name;
				}
				else
				{
					bizUser.DomainId = "";
					bizUser.DomainName = "";
				}

			}

			return bizUser;
		}

		/// <summary>
		/// Convert from User object to base user object.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public static BaseUser ToBaseUser(this User user)
		{
			BaseUser bizUser = null;

			if (null != user)
			{
				bizUser = new BaseUser()
				{
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					ReferenceId = user.ReferenceId,
					Username = user.Username,
					Email = user.Email,
					DomainId = user.DomainId,
					DomainName = user.DomainName,
					LastLogin = user.LastLogin
				};

			}

			return bizUser;
		}
		/// <summary>
		/// Convert from an agilix object to user object.
		/// </summary>
		/// <param name="enrollment">Enrollment.</param>
		/// <returns></returns>
		public static Enrollment ToEnrollment(this AgxDC.Enrollment enrollment)
		{
			Enrollment bizEnrollment = null;

			if (null != enrollment)
			{
				bizEnrollment = new Enrollment
									{
										CourseId = enrollment.Course.Id,
										Id = enrollment.Id,
										User = enrollment.User.ToUser(),
										PercentGraded = enrollment.PercentGraded,
										DomainId = enrollment.Domain.Id,
										DomainName = enrollment.Domain.Name,
										EndDate = enrollment.EndDate.Date == DateTime.MinValue ? (DateTime?)null : enrollment.EndDate,
										Flags = enrollment.Flags.ToString(),
										Reference = enrollment.Reference,
										Schema = enrollment.Schema,
										StartDate = enrollment.StartDate.Date == DateTime.MinValue ? (DateTime?)null : enrollment.StartDate,
										Status = enrollment.Status
									};
				if (enrollment.OverallGrade != null && enrollment.OverallGrade.Possible != 0)
					bizEnrollment.OverallGrade = String.Format("{0:P2}",
															   ( enrollment.OverallGrade.Achieved /
																enrollment.OverallGrade.Possible ));
				else
					bizEnrollment.OverallGrade = "Unknown";
			}

			return bizEnrollment;
		}


		/// <summary>
		/// Convert from a px content item object to ContentItem object.
		/// </summary>
		/// <param name="itm">px content item</param>
		/// <returns></returns>
		//TODO: Remove PX data contract reference. Create a content item class in PXWebApi Models (SK)
		public static ContentItem ToApiContentItem(this PX.PXPub.Models.ContentItem itm)
		{            
			ContentItem bizContentDetailsItem = null;

			if (null != itm)
			{
				bizContentDetailsItem = new ContentItem
											{
												ItemId = itm.Id,
												Title = itm.Title,
												Description = itm.Description,
												CourseId = itm.CourseInfo.Id,
												ParentId = itm.ParentId,
												Sequence = itm.Sequence,
												Type = itm.Type,
												DueDate = itm.DueDate.Date == DateTime.MinValue ? (DateTime?)null : itm.DueDate,
												MaxPoints = itm.MaxPoints,
												Visibility = itm.Visibility.ToString().IndexOf("student") > -1
											};
			}

			return bizContentDetailsItem;
		}

		/// <summary>
		/// Convert from a px content item object to ContentItem object.
		/// </summary>
        /// <param name="item">px content item</param>
        /// <param name="toc">toc</param>
		/// <returns></returns>
		//TODO: Remove PX data contract reference. Create a content item class in PXWebApi Models (SK)
        public static ContentItem ToApiContentItemFromBiz(this PxDC.ContentItem item, string toc = "syllabusfilter")
		{
		    ContentItem bizContentDetailsItem = null;
		    string sequence = null;
		    PxDC.TocCategory cat = null;
            if (item.Categories != null && (cat = item.Categories.FirstOrDefault(c => c.Id == toc)) != null)
            {
                sequence = cat.Sequence;
            }
		    else
		    {
                sequence = item.Sequence;
            }

            if (null != item)
			{                
				bizContentDetailsItem = new ContentItem
				{
                    ItemId = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    ParentId = item.ToItem().ParentId,
                    SubContainerId = item.GetSubContainer(toc),
                    Sequence = sequence,
                    Type = item.Type,
                    SubType = item.Subtype,
                    Category = item.AssignmentSettings.Category,
                    iconUri = item.Thumbnail,
                    DueDate = item.AssignmentSettings.DueDate.Date == DateTime.MinValue ? (DateTime?)null : item.AssignmentSettings.DueDate,
                    MaxPoints = item.MaxPoints,
                    Visibility = item.Visibility.ToString().IndexOf("student") > -1,
                    Removed = GetRemovedFlag(item)                    
				};
			}

			return bizContentDetailsItem;
		}

	    /// <summary>
	    /// Returns the ture / false removed flag for item
	    /// </summary>
	    /// <param name="item"></param>
	    /// <returns></returns>
	    public static bool GetRemovedFlag(PxDC.ContentItem item)
	    {
	        return item.Containers != null && item.Containers.Any(x => x.Value == removedCategoryId);
	    }

	    /// <summary>
		/// Convert from an agilix object to domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns></returns>
		public static DomainDto ToDomainDto(this AgxDC.Domain domain)
		{
			Dictionary<string, string> customQuestionUrls = new Dictionary<string, string>();
			try
			{
				if (domain != null && domain.Data != null)
				{
					var customLocations = domain.Data.XPathSelectElements("//customization/customquestions/customquestion");
					if (!customLocations.IsNullOrEmpty())
					{
						customQuestionUrls = customLocations.ToDictionary(cl => cl.Attribute("name").Value, cl => cl.Attribute("location").Value);
						//htsPlayerUrl = location.Attribute("location").Value;
					}
				}
			}
			catch
			{
				// Just don't set the hts player url if it wasn't found
			}

			return ( domain != null ) ?
				new DomainDto()
				{
					Id = domain.Id,
					Name = domain.Name,
					Reference = domain.Reference,
					Userspace = domain.Userspace,
					CustomQuestionUrls = customQuestionUrls
				} : null;
		}

		/// <summary>
		/// Convert from a px content item object to GradesItem object.
		/// </summary>
		/// <param name="item">px content item</param>
        /// <param name="toc">toc</param>
		/// <returns></returns>
		//TODO: Remove PX data contract reference. Create a contentitem class in PXWebApi Models (SK)
		public static GradesItem ToGradesItem(this PxDC.ContentItem item, string toc = "syllabusfilter")
		{
			GradesItem bizGradesDetailsItem = null;

			if (null != item)
			{
				bizGradesDetailsItem = new GradesItem
										   {
											   ItemId = item.Id,
											   Title = item.Title,
											   Description = item.Description,
											   CourseId = item.CourseId,
											   ParentId = item.ParentId,
                                               SubContainerId = item.GetSubContainer(toc),
											   Sequence = item.Sequence,
											   Type = item.Type,
                                               SubType = item.Subtype,
                                               iconUri = item.Thumbnail,
                                               DueDate = item.AssignmentSettings.DueDate.Date == DateTime.MinValue ? (DateTime?)null : item.AssignmentSettings.DueDate,
											   MaxPoints = item.MaxPoints,
											   Visibility = item.Visibility.IndexOf("student") > -1                                               
										   };
			}

			return bizGradesDetailsItem;
		}


		/// <summary>
		/// Convert from an agilix Grade object to GradeItemResult object.
		/// </summary>
		/// <param name="grade">Agilix grade</param>
		/// <returns></returns>
		public static GradeItemResult ToGradeItemResult(this AgxDC.Grade grade)
		{
			GradeItemResult bizGetGradeItemResult = null;

			if (null != grade)
			{
				bizGetGradeItemResult = new GradeItemResult
											{
												score = grade.Achieved,
												date = grade.ScoredDate,
												status = grade.Status.ToString(),
												duration = grade.Seconds
											};
			}

			return bizGetGradeItemResult;
		}

		/// <summary>
		/// Convert from ContentItem object to TableofContentsItem object.
		/// </summary>
		/// <param name="grade">Agilix grade</param>
		/// <returns></returns>
		public static TableofContentsItem ToTableofContentsItem(this  ContentItem cItem)
		{
			TableofContentsItem biz = null;

			if (null != cItem)
			{
				biz = new TableofContentsItem
						  {
							  ItemId = cItem.ItemId,
							  Category = cItem.Category,
							  CourseId = cItem.CourseId,
							  Description = cItem.Description,
							  DueDate = cItem.DueDate,
                              SubType=  cItem.SubType,
							  MaxPoints = cItem.MaxPoints,
							  ParentId = cItem.ParentId,
                              SubContainerId = cItem.SubContainerId,
							  Sequence = cItem.Sequence,
							  Title = cItem.Title,
							  Type = cItem.Type,
							  Visibility = cItem.Visibility,
                              Removed = cItem.Removed
						  };

			}

			return biz;
		}

        /// <summary>
        /// Convert from ContentItem object to TableofContentsItem object.
        /// </summary>        
        /// <returns></returns>
        public static TableofContentsItem ToTableofContentsItem(this Bfw.PX.Biz.DataContracts.ContentItem cItem, string toc = "syllabusfilter")
        {
            return ToTableofContentsItem(cItem.ToApiContentItemFromBiz(toc));
        }

		/// <summary>
		/// Convert from an agilix Grade object to UserGrades object.
		/// </summary>
		/// <param name="grade">Agilix grade</param>
		/// <returns></returns>
		public static UserGrade ToUserGrade(this AgxDC.Grade grade)
		{
			UserGrade bizGradeDetailsUserGrade = null;

			if (null != grade)
			{
				bizGradeDetailsUserGrade = new UserGrade
											   {
												   Score = grade.Achieved,
												   ScoredDate = grade.ScoredDate.Date == DateTime.MinValue ? (DateTime?)null : grade.ScoredDate,
												   Status = grade.Status.ToString(),
												   Duration = grade.Seconds
											   };
			}

			return bizGradeDetailsUserGrade;
		}

		/// <summary>
		/// Convert from an Enrollment object to UserEnrollment.
		/// </summary>
		/// <param name="enrollment">Enrollment.</param>
		/// <returns></returns>
		public static UserEnrollment ToUserEnrollment(this Enrollment enrollment)
		{
			UserEnrollment biz = null;
			if (null != enrollment)
			{
				biz = new UserEnrollment();
				biz.EnrollmentId = enrollment.Id;
				biz.DomainId = enrollment.DomainId;
				biz.DomainName = enrollment.DomainName;
				biz.StartDate = enrollment.StartDate;
				biz.EndDate = enrollment.EndDate;
				biz.Status = enrollment.Status;
				biz.CourseId = enrollment.CourseId;
			}
			return biz;
		}

		/// <summary>
		/// Convert from an agilix object to EnrolmentDto.
		/// </summary>
		/// <param name="enrollment">Enrollment.</param>
		/// <returns></returns>
		public static UserEnrollmentDto ToUserEnrollmentDto(this AgxDC.Enrollment enrollment)
		{
			UserEnrollmentDto biz = null;
			if (null != enrollment)
			{
				biz = new UserEnrollmentDto();
				biz.EnrollmentId = enrollment.Id;
				biz.User = enrollment.User.ToUser().ToBaseUser();
				biz.DomainId = enrollment.Domain.Id;
				biz.DomainName = enrollment.Domain.Name;
				biz.StartDate = ( enrollment.StartDate == DateTime.MinValue ) ? (DateTime?)null : enrollment.StartDate;
				AgxDC.EnrollmentStatus enrollmentStatus;
				if (Enum.TryParse(enrollment.Status, out enrollmentStatus))
				{
					biz.Status = enrollmentStatus.ToString();
				}
				var course = enrollment.Course.ToCourseDto();
				biz.CourseId = enrollment.Course.Id;
				biz.CourseTitle = enrollment.Course.Title;
				biz.InstructorName = course.InstructorName;
				DateTime activationDate;
				if (DateTime.TryParse(course.ActivatedDate, out activationDate))
				{
					biz.CourseActivationDate = activationDate;
				}
				if (enrollment.OverallGrade != null && enrollment.OverallGrade.Possible != 0)
				{
					biz.Score = enrollment.OverallGrade.Achieved;
					biz.OverallGrade = String.Format("{0:P2}",
													 ( enrollment.OverallGrade.Achieved / enrollment.OverallGrade.Possible ));
				}
				else
				{
					biz.OverallGrade = "Unknown";
				}
				//var studentPermissionFlags = (DlapRights)long.Parse(ConfigurationManager.AppSettings["StudentPermissionFlags"]);
				//var instructorPermissionFlags = (DlapRights)long.Parse(ConfigurationManager.AppSettings["InstructorPermissionFlags"]);
				if (enrollment.Flags.HasFlag(DlapRights.SubmitFinalGrade))
				{
					biz.UserRole = "Instructor";
				}
				else if (enrollment.Flags.HasFlag(DlapRights.Participate))
				{
					biz.UserRole = "Student";
				}
				if (enrollment.User.LastLogin != null && enrollment.User.LastLogin.Value.ToUniversalTime() > DateRule.MinDate.ToUniversalTime() && enrollment.User.LastLogin.Value.ToUniversalTime() < DateRule.MaxDate.ToUniversalTime())
					biz.User.LastLogin = enrollment.User.LastLogin;
			}
			return biz;
		}

		/// <summary>
		/// Convert from an agilix object to courseDto.
		/// </summary>
		/// <param name="agx">The agilix item.</param>
		/// <returns></returns>
		public static CourseDto ToCourseDto(this AgxDC.Course agx)
		{
			CourseDto biz = null;

			if (null != agx)
			{
				biz = new CourseDto()
						  {
							  Id = agx.Id,
							  Title = agx.Title,
							  ParentId = agx.ParentId,
                              ActivatedDate = agx.ActivatedDate
						  };
				if (agx.Domain != null)
				{
					biz.DomainId = agx.Domain.Id;
				}

				if (agx.Data != null)
				{
					biz.Properties = ParseContentProperties(agx.Data);
					biz.ContactInformation = ParseAvailableContactInfo(agx.Data);

					biz.SyllabusType = "Url";
					biz.Syllabus = string.Empty;

					if (agx.Data.Descendants("bfw_syllabus_info") != null &&
						agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault() !=
						null)
					{
						biz.SyllabusType =
							agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault()
								.
								Value;
					}

					if (agx.Data.Descendants("bfw_syllabus_info") != null &&
						agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault() !=
						null)
					{
						biz.Syllabus =
							agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault()
								.
								Value;
					}

                    var propertyCourseId = agx.Data.Element("meta-product-course-id");
					if (propertyCourseId != null)
					{
						biz.ProductCourseId = propertyCourseId.Value;

					}
				}
			}
			return biz;
		}




		/// <summary>
		/// Convert from an agilix object to courseDto.
		/// </summary>
		/// <param name="agx">The agilix item.</param>
		/// <returns></returns>
		public static CourseDto ToCourseDto(this Course crse)
		{
			CourseDto biz = null;

			if (null != crse)
			{
				biz = new CourseDto
						  {
							  Id = crse.Id,
							  Title = crse.Title,
							  ParentId = crse.ParentId,
							  Properties = crse.Properties
						  };
				biz.ContactInformation = biz.ContactInformation;
				biz.SyllabusType = "Url";
				biz.Syllabus = string.Empty;
				biz.SyllabusType = crse.SyllabusType;
				biz.ParentId = crse.ParentId;
				biz.Syllabus = crse.Syllabus;
				biz.ProductCourseId = crse.ProductCourseId;
				biz.CourseProductName = crse.CourseProductName;
				biz.CourseTimeZone = crse.CourseTimeZone;
				biz.InstructorName = crse.InstructorName;
				biz.CourseNumber = crse.CourseNumber;
				biz.SectionNumber = crse.SectionNumber;
				biz.Isbn13 = crse.Isbn13;
				biz.CourseOwner = crse.CourseOwner;
				biz.ActivatedDate = crse.ActivatedDate;
				biz.AcademicTerm = crse.AcademicTerm;
				biz.CourseType = crse.CourseType;
				biz.CourseOwner = crse.CourseOwner;
				biz.CourseDescription = crse.CourseDescription;
				biz.CourseAuthor = crse.CourseAuthor;
				biz.DashboardCourseId = crse.DashboardCourseId;
				if (crse.Domain != null)
				{
					biz.DomainId = crse.Domain.Id;
				}
			}
			return biz;
		}

		/// <summary>
		/// Convert from an agilix object to domain.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns></returns>
		public static Domain ToDomain(this AgxDC.Domain domain)
		{
			Dictionary<string, string> customQuestionUrls = new Dictionary<string, string>();
			try
			{
				if (domain != null && domain.Data != null)
				{
					var customLocations = domain.Data.XPathSelectElements("//customization/customquestions/customquestion");
					if (!customLocations.IsNullOrEmpty())
					{
						customQuestionUrls = customLocations.ToDictionary(cl => cl.Attribute("name").Value, cl => cl.Attribute("location").Value);
						//htsPlayerUrl = location.Attribute("location").Value;
					}
				}
			}
			catch
			{
				// Just don't set the hts player url if it wasn't found
			}

			return ( domain != null ) ?
				new Domain()
				{
					Id = domain.Id,
					Name = domain.Name,
					Reference = domain.Reference,
					Userspace = domain.Userspace,
					CustomQuestionUrls = customQuestionUrls
				} : null;
		}

		/// <summary>
		/// Parses any bfw_property elements from the item's data element.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static IDictionary<string, PropertyValue> ParseContentProperties(XElement data)
		{
			return ParseContentProperties(data, "bfw_property");
		}

		/// <summary>
		/// Parses any bfw_property elements from the item's data element.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static IDictionary<string, PropertyValue> ParseContentProperties(XElement data, string descendants)
		{
			var pdict = new Dictionary<string, PropertyValue>();

			var properties = data.Descendants(descendants);

			if (!properties.IsNullOrEmpty())
			{
				var props = from prop in properties
							group prop by prop.Attribute("name") into p
							select new { Name = p.Key.Value, Elements = p };

				foreach (var prop in props)
				{
					var seed = new List<object>();
					PropertyType type = PropertyType.String;
					var vals = prop.Elements.Map(v => ParsePropertyValue(v)).Reduce((v, vs) => { type = v.Type; vs.Add(v.Value); return vs; }, seed);

					if (vals.Count() > 1)
					{
						pdict[prop.Name] = new PropertyValue() { Type = type, Values = vals };
					}
					else
					{
						pdict[prop.Name] = new PropertyValue() { Type = type, Value = vals.FirstOrDefault() };
					}
				}
			}

			return pdict;
		}

		/// <summary>
		/// Parses out a property value from an XML property element.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		private static PropertyValue ParsePropertyValue(XElement prop)
		{
			var type = prop.Attribute("type") ?? new XAttribute("type", "string");
			PropertyValue val = null;

			if (null != type)
			{
				switch (type.Value.ToLowerInvariant())
				{
					case "boolean":
						bool b = false;
						bool.TryParse(prop.Value, out b);
						val = new PropertyValue() { Type = PropertyType.Boolean, Value = b };
						break;

					case "integer":
						int i = 0;
						int.TryParse(prop.Value, out i);
						val = new PropertyValue() { Type = PropertyType.Integer, Value = i };
						break;

					case "float":
						double d = 0.0;
						double.TryParse(prop.Value, out d);
						val = new PropertyValue() { Type = PropertyType.Float, Value = d };
						break;

					case "string":
						val = new PropertyValue() { Type = PropertyType.String, Value = prop.Value };
						break;

					case "datetime":
						DateTime dt = DateTime.MinValue;
						DateTime.TryParse(prop.Value, out dt);
						val = new PropertyValue() { Type = PropertyType.DateTime, Value = dt };
						break;

					case "xml":
						XElement elm = null;
						bool isHasError = false;
						try
						{
							elm = XElement.Parse(prop.Value);
						}
						catch
						{
							//swallow!
							isHasError = true;
						}

						if (isHasError)
						{
							try
							{
								elm = XElement.Parse(prop.FirstNode.ToString());
							}
							catch
							{
								//swallow!
								isHasError = true;
							}
						}
						val = new PropertyValue() { Type = PropertyType.Xml, Value = elm };
						break;

					case "html":
						string h = HttpUtility.HtmlDecode(prop.Value);
						val = new PropertyValue() { Type = PropertyType.Html, Value = h };
						break;

					default:
						val = val = new PropertyValue() { Type = PropertyType.String, Value = prop.Value };
						break;
				}
			}

			return val;
		}


		/// <summary>
		/// Parses course assessment configuration data out of XML and returns a CourseAssessmentConfiguration object.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		private static CourseAssessmentConfiguration ParseAssessmentConfiguration(XElement data)
		{
			var config = new CourseAssessmentConfiguration();
			var rubricConfig = config.Rubrics;
			var objectiveConfig = config.Objectives;

			if (null != data)
			{
				XElement configNode = null;
				var rubricStyle = data.Descendants("rubric_management_style");
				if (!rubricStyle.IsNullOrEmpty())
				{
					configNode = rubricStyle.First();
				}

				if (null != configNode)
				{
					if (configNode.Element("show_left_column") != null)
						rubricConfig.ShowLeftColumn = Convert.ToBoolean(configNode.Element("show_left_column").Value);

					if (configNode.Element("show_right_column") != null)
						rubricConfig.ShowRightColumn = Convert.ToBoolean(configNode.Element("show_right_column").Value);

					if (configNode.Element("show_edit_on_left") != null)
						rubricConfig.ShowEditOnLeft = Convert.ToBoolean(configNode.Element("show_edit_on_left").Value);

					if (configNode.Element("show_edit_on_right") != null)
						rubricConfig.ShowEditOnRight = Convert.ToBoolean(configNode.Element("show_edit_on_right").Value);

					if (configNode.Element("show_preview_on_left") != null)
						rubricConfig.ShowPreviewOnLeft = Convert.ToBoolean(configNode.Element("show_preview_on_left").Value);

					if (configNode.Element("show_preview_on_right") != null)
						rubricConfig.ShowPreviewOnRight = Convert.ToBoolean(configNode.Element("show_preview_on_right").Value);

					if (configNode.Element("show_view_alignments") != null)
						rubricConfig.ShowViewAlignments = Convert.ToBoolean(configNode.Element("show_view_alignments").Value);

					var alignmentNode = configNode.Element("rubric_alignment_views");
					if (alignmentNode != null)
					{
						if (alignmentNode.Element("rubric") != null)
							rubricConfig.ShowRubricAlignments = Convert.ToBoolean(alignmentNode.Element("rubric").Value);

						if (alignmentNode.Element("assignment") != null)
							rubricConfig.ShowAssignmentAlignments = Convert.ToBoolean(alignmentNode.Element("assignment").Value);
					}
				}

				var objectiveStyle = data.Descendants("objective_management_style");
				if (!objectiveStyle.IsNullOrEmpty())
				{
					configNode = objectiveStyle.First();
				}

				if (null != configNode)
				{
					if (configNode.Element("show_left_column") != null)
						objectiveConfig.ShowLeftColumn = Convert.ToBoolean(configNode.Element("show_left_column").Value);

					if (configNode.Element("show_right_column") != null)
						objectiveConfig.ShowRightColumn = Convert.ToBoolean(configNode.Element("show_right_column").Value);

					if (configNode.Element("show_edit_on_left") != null)
						objectiveConfig.ShowEditOnLeft = Convert.ToBoolean(configNode.Element("show_edit_on_left").Value);

					if (configNode.Element("show_edit_on_right") != null)
						objectiveConfig.ShowEditOnRight = Convert.ToBoolean(configNode.Element("show_edit_on_right").Value);

					if (configNode.Element("show_view_alignments") != null)
						objectiveConfig.ShowViewAlignments = Convert.ToBoolean(configNode.Element("show_view_alignments").Value);

					var alignmentNode = configNode.Element("learningobj_alignment_views");
					if (alignmentNode != null)
					{
						if (alignmentNode.Element("objective") != null)
							objectiveConfig.ShowObjectiveAlignments = Convert.ToBoolean(alignmentNode.Element("objective").Value);

						if (alignmentNode.Element("assignment") != null)
							objectiveConfig.ShowAssignmentAlignments = Convert.ToBoolean(alignmentNode.Element("assignment").Value);
					}
				}
			}

			return config;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static List<GradeBookWeightCategory> ParseGrageBookCategories(XElement categories)
		{
			var gbbList = new List<GradeBookWeightCategory>();
			foreach (var category in categories.Elements("category"))
			{
				var newcategory = new GradeBookWeightCategory()
				{
					Id = category.Attribute("id").Value,
					Text = category.Attribute("name").Value,
					Weight = category.Attribute("weight").Value,
					//ItemWeightTotal = "0",
					Percent = "0"

				};


				gbbList.Add(newcategory);
			}



			return gbbList;
		}


		private static TabSettings ParseTabSettings(XElement data)
		{
			TabSettings tab_settings = new TabSettings();
			if (null != data.Element("bfw_tab_settings"))
			{
				var vt = data.Element("bfw_tab_settings").Element("view_tab");

				if (null != vt)
				{
					tab_settings.view_tab.show_policies = Convert.ToBoolean(vt.GetDescendantAsString("show_policies"));
					tab_settings.view_tab.show_assignment_details = Convert.ToBoolean(vt.GetDescendantAsString("show_assignment_details"));
					tab_settings.view_tab.show_rubrics = Convert.ToBoolean(vt.GetDescendantAsString("show_rubrics"));
					tab_settings.view_tab.show_learning_objectives = Convert.ToBoolean(vt.GetDescendantAsString("show_learning_objectives"));
				}

			}

			return tab_settings;
		}

		/// <summary>
		/// Parses available contact info
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static List<ContactInfo> ParseAvailableContactInfo(XElement data)
		{
			List<ContactInfo> result = new List<ContactInfo>();

			if (data != null)
			{
				var schemaNode = data.Descendants("bfw_contact_info");

				if (schemaNode != null && !schemaNode.IsNullOrEmpty())
				{
					foreach (var xfeed in schemaNode.Descendants("info"))
					{
						ContactInfo info = new ContactInfo()
						{
							ContactType = xfeed.Attribute("ContactType").Value,
							ContactValue = xfeed.Attribute("ContactValue").Value
						};

						result.Add(info);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Parses any bfw_meta properties from the item's data element.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static IDictionary<string, MetadataValue> ParseContentMetadata(XElement data)
		{
			var meta = new Dictionary<string, MetadataValue>();

			var properties = data.Descendants("bfw_metadata");

			if (!properties.IsNullOrEmpty())
			{
				var props = from prop in properties
							group prop by prop.Attribute("name") into p
							select new { Name = p.Key.Value, Elements = p };

				foreach (var prop in props)
				{
					var seed = new List<object>();
					PropertyType type = PropertyType.String;
					var vals = prop.Elements.Map(v => ParsePropertyValue(v)).Reduce((v, vs) => { type = v.Type; vs.Add(v.Value); return vs; }, seed);

					if (vals.Count() > 1)
					{
						meta[prop.Name] = new MetadataValue() { Type = type, Values = vals };
					}
					else
					{
						meta[prop.Name] = new MetadataValue() { Type = type, Value = vals.FirstOrDefault() };
					}
				}
			}

			return meta;
		}

		/// <summary>
		/// Parses TOC category data out of XML and returns a list of TOC categories.
		/// </summary>
		/// <param name="data">The XML data.</param>
		/// <returns></returns>
		private static IList<TocCategory> ParseTocCategories(XElement data)
		{
			var categories = new List<TocCategory>();

			if (null != data)
			{
				var tocs = data.Descendants("bfw_tocs");

				if (null != tocs && !tocs.IsNullOrEmpty())
				{
					var tocElms = from t in tocs.First().Elements()
								  //Assuming that the type will be bfw_toc if it is under bfw_tocs.
								  //where t.Attribute("type") != null && t.Attribute("type").Value == "bfw_toc"
								  select t;

					if (!tocElms.IsNullOrEmpty())
					{
						foreach (var toc in tocElms)
						{
							var cat = new TocCategory()
							{
								Id = toc.Name.ToString(),
								Text = toc.Value,

							};

							if (toc.Attribute("parentid") != null)
								cat.ItemParentId = toc.Attribute("parentid").Value;

							if (toc.Attribute("sequence") != null)
								cat.Sequence = toc.Attribute("sequence").Value;


							if (toc.Attribute("type") != null)
								cat.Type = toc.Attribute("type").Value;


							categories.Add(cat);
						}
					}
				}
			}

			return categories;
		}

		/// <summary>
		/// Parses group set data out of XML Returns a list of group sets.
		/// </summary>
		/// <param name="data">The XML data.</param>
		/// <returns></returns>
		private static IEnumerable<GroupSet> ParseGroupSets(XElement data)
		{
			var groupSets = new List<GroupSet>();

			if (null != data)
			{
				var groupsetsEl = data.Element("groupsets");
				if (null != groupsetsEl)
				{
					foreach (var groupSetEl in groupsetsEl.Elements("groupset"))
					{
						int id = Convert.ToInt32(groupSetEl.Attribute("id").Value);
						string title = groupSetEl.Attribute("title").Value;

						groupSets.Add(new GroupSet()
						{
							Id = id,
							Name = title
						});
					}
				}
			}

			return groupSets;
		}

		/// <summary>
		/// Parses search schema data out of XML and returns a SearchSchema object.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		private static SearchSchema ParseSearchSchema(XElement data)
		{
			var schema = new SearchSchema();

			List<SearchCategory> catList = new List<SearchCategory>();

			if (null != data)
			{
				var schemaNode = data.Descendants("bfw_search");

				if (null != schemaNode && !schemaNode.IsNullOrEmpty())
				{
					var searchCats = from s in schemaNode.First().Descendants("searchcategory") select s;

					if (!searchCats.IsNullOrEmpty())
					{
						foreach (var elm in searchCats)
						{
							var cat = new SearchCategory();

							if (elm.Attribute("title") != null)
								cat.Title = elm.Attribute("title").Value;

							if (elm.Attribute("searchable") != null)
								cat.IsSearchable = Convert.ToBoolean(elm.Attribute("searchable").Value);

							if (elm.Attribute("metadata") != null)
							{
								cat.AssociatedItems = elm.Attribute("metadata").Value.Split(',').ToList();
								cat.MetaTag = elm.Attribute("metadata").Value;
							}

							catList.Add(cat);
						}

						schema.Categories = catList;
					}
				}
			}

			return schema;
		}

		/// <summary>
		/// Parses available RSS feeds
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static List<RSSCourseFeed> ParseAvailableRssFeeds(XElement data)
		{
			List<RSSCourseFeed> result = new List<RSSCourseFeed>();
			if (data != null)
			{
				var schemaNode = data.Descendants("bfw_rss_feeds");
				if (schemaNode != null && !schemaNode.IsNullOrEmpty())
				{
					foreach (var xfeed in schemaNode.Descendants("feed"))
					{
						RSSCourseFeed feed = new RSSCourseFeed()
						{
							Name = xfeed.Attribute("name").Value,
							Url = xfeed.Attribute("url").Value
						};
						result.Add(feed);

					}
				}
			}
			return result;
		}


		/// <summary>
		/// Parses Assign Tab Settings data out of XML
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static AssignTabSettings ParseAssignTabSettings(XElement data)
		{
			var assignTabSettings = new AssignTabSettings();

			if (null != data)
			{
				var bfwTabSettings = data.Element("bfw_tab_settings");

				if (null != bfwTabSettings)
				{
					var assignTab = bfwTabSettings.Element("assign_tab");

					if (null != assignTab)
					{
						var showMarkGradeable = assignTab.Element("show_make_gradeable");
						if (null != showMarkGradeable)
						{
							assignTabSettings.ShowMakeGradeable = Convert.ToBoolean(showMarkGradeable.Value);
						}

						var showAllowLateSubmissions = assignTab.Element("show_allow_late_submissions");
						if (null != showAllowLateSubmissions)
						{
							assignTabSettings.ShowAllowLateSubmissions = Convert.ToBoolean(showAllowLateSubmissions.Value);
						}

						var showScheduleReminder = assignTab.Element("show_schedule_reminder");
						if (null != showScheduleReminder)
						{
							assignTabSettings.ShowScheduleReminder = Convert.ToBoolean(showScheduleReminder.Value);
						}

						var showSubcontentCreation = assignTab.Element("show_subcontent_creation");
						if (null != showSubcontentCreation)
						{
							assignTabSettings.ShowScheduleReminder = Convert.ToBoolean(showSubcontentCreation.Value);
						}

						var showAssignedSameDay = assignTab.Element("show_assigned_on_same_day");
						if (null != showAssignedSameDay)
						{
							assignTabSettings.ShowAssignedSameDay = Convert.ToBoolean(showAssignedSameDay.Value);
						}

						var showGradebookCategory = assignTab.Element("show_gradebook_category");
						if (null != showGradebookCategory)
						{
							assignTabSettings.ShowGradebookCategory = Convert.ToBoolean(showGradebookCategory.Value);
						}

						var showCalculationType = assignTab.Element("show_calculation_type");
						if (null != showCalculationType)
						{
							assignTabSettings.ShowCalculationType = Convert.ToBoolean(showCalculationType.Value);
						}

						var showMarkAsComplete = assignTab.Element("show_mark_as_complete");
						if (null != showMarkAsComplete)
						{
							assignTabSettings.ShowMarkAsComplete = Convert.ToBoolean(showMarkAsComplete.Value);
						}

						var showSyllabusCategory = assignTab.Element("show_syllabus_category");
						if (null != showSyllabusCategory)
						{
							assignTabSettings.ShowSyllabusCategory = Convert.ToBoolean(showSyllabusCategory.Value);
						}

						var showPointsPossible = assignTab.Element("show_points_possible");
						if (null != showPointsPossible)
						{
							assignTabSettings.ShowPointsPossible = Convert.ToBoolean(showPointsPossible.Value);
						}

						var showSendReminder = assignTab.Element("show_send_reminder");
						if (null != showSendReminder)
						{
							assignTabSettings.ShowSendReminder = Convert.ToBoolean(showSendReminder.Value);
						}

						var showIncludeScore = assignTab.Element("show_include_score");
						if (null != showIncludeScore)
						{
							assignTabSettings.ShowIncludeScore = Convert.ToBoolean(showIncludeScore.Value);
						}

						var showCompletionCategory = assignTab.Element("show_completion_category");
						if (null != showCompletionCategory)
						{
							assignTabSettings.ShowIncludeScore = Convert.ToBoolean(showCompletionCategory.Value);
						}
					}

				}
			}

			return assignTabSettings;

		}


		/// <summary>
		/// Parses instructor console settings
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static InstructorConsoleSettings ParseConsoleSettings(XElement data)
		{
			InstructorConsoleSettings result = new InstructorConsoleSettings();

			if (data != null)
			{
				var schemaNode = data.Descendants("bfw_console_settings");

				if (schemaNode != null && !schemaNode.IsNullOrEmpty())
				{
					result.ShowGeneral = schemaNode.Descendants("show_general").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_general").SingleOrDefault().Value) : true;
					result.ShowNavigation = schemaNode.Descendants("show_navigation").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_navigation").SingleOrDefault().Value) : true;
					result.ShowLaunchPad = schemaNode.Descendants("show_launchpad").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_launchpad").SingleOrDefault().Value) : true;
					result.ShowTypes = schemaNode.Descendants("show_types").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_types").SingleOrDefault().Value) : true;
					result.ShowEbook = schemaNode.Descendants("show_ebook").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_ebook").SingleOrDefault().Value) : true;
					result.ShowMyResources = schemaNode.Descendants("show_myresources").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_myresources").SingleOrDefault().Value) : true;
					result.ShowChapters = schemaNode.Descendants("show_chapters").SingleOrDefault() != null ? Boolean.Parse(schemaNode.Descendants("show_chapters").SingleOrDefault().Value) : true;
				}
			}

			return result;
		}

		/// <summary>
		/// Parses facet search schema data out of XML and returns a FacetSearchSchema object.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		private static FacetSearchSchema ParseFacetSchema(XElement data)
		{
			var schema = new FacetSearchSchema();

			List<FacetSearchCategory> catList = new List<FacetSearchCategory>();

			if (null != data)
			{
				var schemaNode = data.Descendants("bfw_facets");

				if (null != schemaNode && !schemaNode.IsNullOrEmpty())
				{
					var facetCats = from s in schemaNode.First().Descendants("facet") select s;

					if (!facetCats.IsNullOrEmpty())
					{
						foreach (var elm in facetCats)
						{
							var cat = new FacetSearchCategory();

							if (elm.Attribute("title") != null)
								cat.Title = elm.Attribute("title").Value;


							if (elm.Attribute("metadatakey") != null)
							{
								cat.MetaDataKey = elm.Attribute("metadatakey").Value;
							}


							if (elm.Attribute("default") != null)
							{
								bool def = false;
								bool.TryParse(elm.Attribute("default").Value, out def);
								cat.Default = def;
							}
							catList.Add(cat);
						}

						schema.Categories = catList;
					}
				}
			}

			return schema;
		}
		/// <summary>
		/// Convert from domain to an agilix object.
		/// </summary>
		/// <param name="domain">The domain.</param>
		/// <returns></returns>
		public static AgxDC.Domain ToDomain(this Domain domain)
		{
			return ( domain != null ) ?
				new AgxDC.Domain()
				{
					Id = domain.Id,
					Name = domain.Name,
					Reference = domain.Reference,
					Userspace = domain.Userspace
				} : null;
		}
		/// <summary>
		/// Converts a properties collection to XML.
		/// </summary>
		/// <param name="Properties">The properties collection.</param>
		/// <param name="subType">The sub type property.</param>
		/// <returns></returns>
		public static XElement GetXElementFromProperties(IDictionary<string, PropertyValue> Properties, string subType)
		{
			XElement data = new XElement(subType);
			data.Add(new XElement("bfw_type", subType));

			if (!Properties.IsNullOrEmpty())
			{
				var propsElm = new XElement("bfw_properties");
				foreach (var prop in Properties)
				{
					if (!prop.Value.Values.IsNullOrEmpty())
					{
						foreach (var v in prop.Value.Values)
						{
							propsElm.Add(new XElement("bfw_property",
								new XAttribute("name", prop.Key),
								new XAttribute("type", prop.Value.Type.ToString()),
								v));
						}
					}
					else
					{
						propsElm.Add(new XElement("bfw_property",
								new XAttribute("name", prop.Key),
								new XAttribute("type", prop.Value.Type.ToString()),
								prop.Value.Value));
					}
				}
				data.Add(propsElm);
			}

			return data;
		}

		/// <summary>
		/// Get the value of a descendant
		/// </summary>
		/// <param name="data"></param>
		/// <param name="descendant"></param>
		/// <returns></returns>
		public static string GetDescendantAsString(this XElement data, string descendant)
		{
			string returnValue = "";
			if (!data.Descendants(descendant).IsNullOrEmpty())
			{
				var elm = data.Descendants(descendant).FirstOrDefault();
				if (elm != null)
				{
					returnValue = elm.Value;
					returnValue = System.Web.HttpUtility.HtmlDecode(returnValue.Trim());
				}
			}

			return returnValue;
		}
		/// <summary>
		/// Convert from course to an agilix object.
		/// </summary>
		/// <param name="course">The course.</param>
		/// <returns></returns>
		public static AgxDC.Course ToAgxCourse(this Course course)
		{
			var agxCourse = new AgxDC.Course();

			if (null != course)
			{
				agxCourse.Id = course.Id;
				agxCourse.Title = course.Title;
				agxCourse.Domain = new Bfw.Agilix.DataContracts.Domain();
				agxCourse.Domain = course.Domain.ToDomain();

				agxCourse.Data = GetXElementFromProperties(course.Properties, "course");
				//StoreAssessmentConfiguration()
				//course.AssessmentConfiguration.Objectives.
				//if (course.bfw_tab_settings != null)
				//{
				//    var view_tab = new XElement("view_tab");

				//    view_tab.Add(new XElement("show_policies", XmlConvert.ToString(course.bfw_tab_settings.view_tab.show_policies)));
				//    view_tab.Add(new XElement("show_assignment_details", XmlConvert.ToString(course.bfw_tab_settings.view_tab.show_assignment_details)));
				//    view_tab.Add(new XElement("show_rubrics", XmlConvert.ToString(course.bfw_tab_settings.view_tab.show_rubrics)));
				//    view_tab.Add(new XElement("show_learning_objectives", XmlConvert.ToString(course.bfw_tab_settings.view_tab.show_learning_objectives)));

				//    var bfw_tab_settings = new XElement("bfw_tab_settings");
				//    bfw_tab_settings.Add("view_tab", view_tab);

				//    agxCourse.Data.Add(new XElement("bfw_tab_settings", bfw_tab_settings ));
				//}

				//add gradebook categories list
				if (!course.GradeBookCategoryList.IsNullOrEmpty())
				{
					var gbb_categories = new XElement("categories");
					foreach (var category in course.GradeBookCategoryList)
					{
						var newCategory = new XElement("category",
													   new XAttribute("id", category.Id),
													   new XAttribute("name", category.Text),
													   new XAttribute("weight", ( ( category.Weight == null ) ? "0" : category.Weight ).ToString(CultureInfo.InvariantCulture))
						);
						gbb_categories.Add(newCategory);

					}
					agxCourse.Data.Add(gbb_categories);

				}

				if (!course.ContactInformation.IsNullOrEmpty())
				{
					var contactInfo = new XElement("bfw_contact_info");
					agxCourse.Data.Add(contactInfo);

					foreach (var info in course.ContactInformation)
					{
						var newInfo = new XElement("info", new XAttribute("ContactType", info.ContactType), new XAttribute("ContactValue", info.ContactValue));
						contactInfo.Add(newInfo);
					}
				}

				if (course.ConsoleSettings != null)
				{
					var consoleSetting = new XElement("bfw_console_settings");
					agxCourse.Data.Add(consoleSetting);

					var showGeneral = new XElement("show_general", course.ConsoleSettings.ShowGeneral);
					var showNavigation = new XElement("show_navigation", course.ConsoleSettings.ShowNavigation);
					var showLaunchpad = new XElement("show_launchpad", course.ConsoleSettings.ShowLaunchPad);
					var showchapters = new XElement("show_chapters", course.ConsoleSettings.ShowChapters);
					var showTypes = new XElement("show_types", course.ConsoleSettings.ShowTypes);
					var showEbook = new XElement("show_ebook", course.ConsoleSettings.ShowEbook);
					var showMyResources = new XElement("show_myresources", course.ConsoleSettings.ShowMyResources);

					consoleSetting.Add(showGeneral);
					consoleSetting.Add(showNavigation);
					consoleSetting.Add(showLaunchpad);
					consoleSetting.Add(showchapters);
					consoleSetting.Add(showTypes);
					consoleSetting.Add(showEbook);
					consoleSetting.Add(showMyResources);
				}

				if (!string.IsNullOrEmpty(course.Syllabus))
				{
					var syllabusInfo = new XElement("bfw_syllabus_info");
					agxCourse.Data.Add(syllabusInfo);

					var type = new XElement("syllabus_type", course.SyllabusType);
					var value = new XElement("syllabus_value", course.Syllabus);

					syllabusInfo.Add(type);
					syllabusInfo.Add(value);
				}

				var learningobjectives = new XElement("learningobjectives");
				agxCourse.Data.Add(learningobjectives); // allowed to be empty

				if (!course.LearningObjectives.IsNullOrEmpty())
				{
					foreach (var objective in course.LearningObjectives)
					{
						var newObjective = new XElement("objective",
							new XAttribute("guid", objective.Guid),
							new XAttribute("id", objective.Id)
							//new XAttribute("group", objective.Group)
						);

						newObjective.Add(new XElement("description", objective.Title));
						learningobjectives.Add(newObjective);
					}
				}

				foreach (String nodeName in new string[]
                                                {
                                                    "bfw_template_learningobjectives",
                                                    "bfw_dashboard_learningobjectives",
                                                    "bfw_eportfolio_learningobjectives",
                                                    "bfw_program_learningobjectives"
                                                })
				{
					IEnumerable<LearningObjective> objectives = null;
					if (false) ; // done for alignment purposes 
					else if (nodeName == "bfw_template_learningobjectives") objectives = course.PublisherTemplateLearningObjectives;
					else if (nodeName == "bfw_dashboard_learningobjectives") objectives = course.DashboardLearningObjectives;
					else if (nodeName == "bfw_eportfolio_learningobjectives") objectives = course.EportfolioLearningObjectives;
					else if (nodeName == "bfw_program_learningobjectives") objectives = course.ProgramLearningObjectives;

					// create header nodes for learning objective type
					var bfwObjectives = new XElement(nodeName);
					var learningObjectives = new XElement("learningobjectives");
					bfwObjectives.Add(learningObjectives);

					agxCourse.Data.Add(bfwObjectives); // allowed to be converted to empty

					// skip learning objective type if no objects exist for that type
					if (objectives.IsNullOrEmpty())
						continue;

					foreach (var objective in objectives)
					{
						var newObjective = new XElement("objective",
							new XAttribute("guid", objective.Guid),
							new XAttribute("id", objective.Id),
							new XAttribute("parentid", !String.IsNullOrEmpty(objective.ParentId) ? objective.ParentId : ""),
							new XAttribute("sequence", !String.IsNullOrEmpty(objective.Sequence) ? objective.Sequence : "")
							//new XAttribute("group", objective.Group)
						);

						if (!String.IsNullOrEmpty(objective.Title))
						{
							newObjective.Add(new XElement("description", objective.Title));
						}
						if (!String.IsNullOrEmpty(objective.Description))
						{
							newObjective.Add(new XElement("bfw_description", objective.Description));
						}


						learningObjectives.Add(newObjective);
					}
				}

				if (!string.IsNullOrEmpty(course.ProductCourseId))
				{
					agxCourse.Data.Add(new XElement("bfw_productcourseid", course.ProductCourseId));
				}

			}

			return agxCourse;
		}


		/// <summary>
		/// Convert from an agilix object to course.
		/// </summary>
		/// <param name="agx">The agilix item.</param>
		/// <returns></returns>
		public static Course ToCourse(this AgxDC.Course agx)
		{
			Course biz = null;

			if (null != agx)
			{
				biz = new Course()
				{
					Id = agx.Id,
					Title = agx.Title,
					Domain = agx.Domain != null ? agx.Domain.ToDomain() : null,
					ParentId = agx.ParentId

				};

				if (agx.Data != null)
				{
					biz.Properties = ParseContentProperties(agx.Data);
					biz.Metadata = ParseContentMetadata(agx.Data);
					biz.Categories = ParseTocCategories(agx.Data);
					biz.GroupSets = ParseGroupSets(agx.Data);
					biz.SearchSchema = ParseSearchSchema(agx.Data);
					biz.FacetSearchSchema = ParseFacetSchema(agx.Data);
					biz.RSSCourseFeeds = ParseAvailableRssFeeds(agx.Data);
					biz.ContactInformation = ParseAvailableContactInfo(agx.Data);
					biz.ConsoleSettings = ParseConsoleSettings(agx.Data);

					biz.SyllabusType = "Url";
					biz.Syllabus = string.Empty;

					if (agx.Data.Descendants("bfw_syllabus_info") != null && agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault() != null)
					{
						biz.SyllabusType = agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_type").SingleOrDefault().Value;
					}

					if (agx.Data.Descendants("bfw_syllabus_info") != null && agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault() != null)
					{
						biz.Syllabus = agx.Data.Descendants("bfw_syllabus_info").Descendants("syllabus_value").SingleOrDefault().Value;
					}

					biz.AssessmentConfiguration = ParseAssessmentConfiguration(agx.Data);

					if (biz.CourseType == CourseType.ProgramDashboard.ToString())
					{
						// currently there is no way of coding an update to assessment confiuations
						// since program managers are the only screen that could change this setting it was placed here
						biz.AssessmentConfiguration.Objectives.ShowRightColumn = false;
						biz.AssessmentConfiguration.Rubrics.ShowRightColumn = false;
					}

					biz.bfw_tab_settings = ParseTabSettings(agx.Data);
					biz.AssignTabSettings = ParseAssignTabSettings(agx.Data);

					//load grade book categories
					if (agx.Data.Element("categories") != null)
					{
						biz.GradeBookCategoryList = ParseGrageBookCategories(agx.Data.Element("categories"));
					}

					var propertyCourseId = agx.Data.Element("bfw_productcourseid");
					if (propertyCourseId != null)
					{
						biz.ProductCourseId = propertyCourseId.Value;

					}

					var propertyReadOnly = agx.Data.Element("bfw_read_only");
					if (propertyReadOnly != null)
					{
						biz.ReadOnly = propertyReadOnly.Value;
					}

					var allowSampling = agx.Data.Element("bfw_allow_sampling");
					if (allowSampling != null)
					{
						bool samplingOk = false;
						bool.TryParse(allowSampling.Value, out samplingOk);

						biz.AllowSampling = samplingOk;
					}

					var allowPurchase = agx.Data.Element("bfw_allow_purchase");
					if (allowPurchase != null)
					{
						bool purchaseOk = false;
						bool.TryParse(allowPurchase.Value, out purchaseOk);

						biz.AllowPurchase = purchaseOk;
					}

					var allowExtraCredit = agx.Data.XPathSelectElement("./bfw_properties/bfw_property[@name=\"bfw_allow_extracredit\"]");
					if (allowExtraCredit != null)
					{
						biz.IsAllowExtraCredit = allowExtraCredit.Value.Equals("true");
					}

					var lockedLOs = biz.LockedLearningObjectives;
					foreach (String nodeName in new string[]
                                                    {
                                                        "bfw_template_learningobjectives",
                                                        "bfw_dashboard_learningobjectives",
                                                        "bfw_eportfolio_learningobjectives",
                                                        "bfw_program_learningobjectives"
                                                    })
					{
						var learningObjectiveType = agx.Data.Element(nodeName);
						if (learningObjectiveType == null)
							continue; // if learning objective type element could not be found, skip it

						var lObjectives = learningObjectiveType.Element("learningobjectives");
						if (lObjectives == null)
							continue; // if learning objective elements could not be found, skip it

						var learningObjectives = new List<LearningObjective>();
						foreach (var objective in lObjectives.Elements("objective"))
						{
							var learningObjective = new LearningObjective();

							learningObjective.Guid = objective.Attribute("guid").Value;
							learningObjective.Id = objective.Attribute("id").Value;
							learningObjective.Title = objective.Element("description").Value;
							if (objective.Attribute("parentid") != null)
							{
								learningObjective.ParentId = objective.Attribute("parentid").Value;
							}
							if (objective.Attribute("sequence") != null)
							{
								learningObjective.Sequence = objective.Attribute("sequence").Value;
							}
							if (objective.Element("bfw_description") != null)
							{
								learningObjective.Description = objective.Element("bfw_description").Value;
							}
							learningObjective.IsLocked = lockedLOs.Contains(learningObjective.Guid);

							learningObjectives.Add(learningObjective);
						}

						if (false) ; // i enjoy when things line up
						else if (nodeName == "bfw_template_learningobjectives")
							biz.PublisherTemplateLearningObjectives = learningObjectives;
						else if (nodeName == "bfw_dashboard_learningobjectives")
							biz.DashboardLearningObjectives = learningObjectives;
						else if (nodeName == "bfw_eportfolio_learningobjectives")
							biz.EportfolioLearningObjectives = learningObjectives;
						else if (nodeName == "bfw_program_learningobjectives")
							biz.ProgramLearningObjectives = learningObjectives;
					}
				}

				if (!agx.LearningObjectives.IsNullOrEmpty())
				{
					var learningObjectives = new List<LearningObjective>();
					foreach (var objective in agx.LearningObjectives)
					{
						var bizObjective = new LearningObjective()
						{
							Id = objective.Id,
							ParentId = objective.ParentId,
							Title = objective.Title,
							Sequence = objective.Sequence,
							Description = objective.Description,
							Group = objective.Group,
							Guid = objective.Guid
						};

						learningObjectives.Add(bizObjective);
					}

					biz.LearningObjectives = learningObjectives;
				}

				if (!biz.EportfolioLearningObjectives.IsNullOrEmpty())
				{
					var guids = biz.EportfolioLearningObjectives.Map(x => x.Guid).ToList();
					var learningObjectives = new List<LearningObjective>();
					foreach (String guid in guids)
					{
						LearningObjective lo = null;
						if (null == lo && !biz.PublisherTemplateLearningObjectives.IsNullOrEmpty())
							lo = biz.PublisherTemplateLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.ProgramLearningObjectives.IsNullOrEmpty())
							lo = biz.ProgramLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.DashboardLearningObjectives.IsNullOrEmpty())
							lo = biz.DashboardLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.EportfolioLearningObjectives.IsNullOrEmpty())
							lo = biz.EportfolioLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));

						learningObjectives.Add(lo);
					}
					biz.EportfolioLearningObjectives = learningObjectives;
				}
				if (!biz.LearningObjectives.IsNullOrEmpty())
				{
					var guids = biz.LearningObjectives.Map(x => x.Guid).ToList();
					var learningObjectives = new List<LearningObjective>();
					foreach (String guid in guids)
					{
						LearningObjective lo = null;
						if (null == lo && !biz.PublisherTemplateLearningObjectives.IsNullOrEmpty())
							lo = biz.PublisherTemplateLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.ProgramLearningObjectives.IsNullOrEmpty())
							lo = biz.ProgramLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.DashboardLearningObjectives.IsNullOrEmpty())
							lo = biz.DashboardLearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));
						if (null == lo && !biz.LearningObjectives.IsNullOrEmpty())
							lo = biz.LearningObjectives.FirstOrDefault(x => x.Guid.Equals(guid));

						learningObjectives.Add(lo);
					}
					biz.LearningObjectives = learningObjectives;
				}

			}

			return biz;
		}

		/// <summary>
		/// Converts the Xelement to CourseAcademicTerm Object
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static CourseAcademicTerm ToCourseAcademicTerm(this XElement element)
		{
			return new CourseAcademicTerm
			{
				Id = element.Attribute("id").Value,
				Name = element.Value,
				StartDate = DateTime.Parse(element.Attribute("start").Value),
				EndDate = DateTime.Parse(element.Attribute("end").Value)
			};
		}

		/// <summary>
		/// Convert from an agilix object to resource.
		/// </summary>
		/// <param name="agx">The agilix item.</param>
		/// <returns></returns>
		public static Resource ToApiResource(this AgxDC.Resource agx)
		{
			var resource = new Resource();

			if (null != agx)
			{
				resource.EntityId = agx.EntityId;
				resource.ContentType = agx.ContentType;
				resource.Extension = agx.Extension;
				resource.Status = (ResourceStatus)Enum.Parse(typeof(ResourceStatus), agx.Status.ToString());
				resource.Url = agx.Url;
				resource.CreationDate = agx.CreationDate;
				resource.ModifiedDate = agx.ModifiedDate;

				if (String.IsNullOrEmpty(resource.Extension))
					resource.Extension = System.IO.Path.GetExtension(resource.Url).Replace(".", "");

				var source = agx.GetStream();
				var dest = resource.GetStream();
				source.Copy(dest);

				dest.Flush();
				dest.Seek(0, System.IO.SeekOrigin.Begin);
			}

			return resource;
		}

		/// <summary>
		/// Convert from an agilix object to XML resource.
		/// </summary>
		/// <param name="agx">The agilix item.</param>
		/// <returns></returns>
		public static XmlResource ToApiXmlResource(this AgxDC.Resource agx)
		{
			var resource = new XmlResource();

			if (null != agx)
			{
				resource.EntityId = agx.EntityId;
				resource.ContentType = agx.ContentType;
				resource.Extension = agx.Extension;
				resource.Status = (ResourceStatus)Enum.Parse(typeof(ResourceStatus), agx.Status.ToString());
				resource.Url = agx.Url;
				resource.CreationDate = agx.CreationDate;
				resource.ModifiedDate = agx.ModifiedDate;

				if (String.IsNullOrEmpty(resource.Extension))
					resource.Extension = System.IO.Path.GetExtension(resource.Url).Replace(".", "");

				var source = agx.GetStream();
				var dest = resource.GetStream();
				source.Copy(dest);

				dest.Flush();
				dest.Seek(0, System.IO.SeekOrigin.Begin);

				var sr = new System.IO.StreamReader(dest);

				string xmlDoc = sr.ReadToEnd();
				dest.Flush();
				dest.Seek(0, SeekOrigin.Begin);

				if (xmlDoc.Contains("&nbsp;"))
					xmlDoc = xmlDoc.Replace("&nbsp;", " ");

				XDocument xDoc = XDocument.Parse(xmlDoc);

				XElement xElement = xDoc.XPathSelectElement("//body");

				if (xElement != null)
				{
					string body = xElement.Value;
					dest.SetLength(0);
					var sw = new System.IO.StreamWriter(dest);
					sw.Write(System.Web.HttpUtility.HtmlDecode(body));
					sw.Flush();
				}

				xElement = xDoc.XPathSelectElement("//title");
				if (xElement != null) resource.Name = xElement.Value ?? "Document";

				xElement = xDoc.XPathSelectElement("//ExtendedProperties");
				if (xElement != null)
				{
					var extendedProperties = xElement.Elements();
					if (!extendedProperties.IsNullOrEmpty())
					{
						foreach (XElement prop in extendedProperties)
						{
							resource.ExtendedProperties.Add(prop.Name.ToString(), prop.Value);
						}
					}
				}

			}

			return resource;
		}

		/// <summary>
		/// Maps an Enrollment object to a Enrollee object.
		/// </summary>
		/// <param name="enrollment">User object.</param>
		/// <returns>
		/// Enrollee model.
		/// </returns>
		public static Enrollee ToEnrollee(this AgxDC.Enrollment enrollment)
		{
			if (enrollment == null)
				return null;

			var enrollee = new Enrollee();
			enrollee.Id = enrollment.User.Id;
			enrollee.FirstName = enrollment.User.FirstName;
			enrollee.LastName = enrollment.User.LastName;
			enrollee.Email = enrollment.User.Email;
			enrollee.LastLogin = enrollment.User.LastLogin;
			enrollee.ReferenceId = enrollment.User.Reference;

			if (enrollment.Flags.HasFlag(DlapRights.SubmitFinalGrade))
			{
				enrollee.UserRole = "Instructor";
			}
			else if (enrollment.Flags.HasFlag(DlapRights.Participate))
			{
				enrollee.UserRole = "Student";
			}
			enrollee.CourseCompletePercentage = Convert.ToInt32(enrollment.PercentGraded);
			return enrollee;
		}
	}
}
