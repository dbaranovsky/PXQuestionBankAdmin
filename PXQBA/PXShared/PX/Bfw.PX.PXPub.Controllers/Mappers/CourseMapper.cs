using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using GradeBookWeightCategory = Bfw.PX.Biz.DataContracts.GradeBookWeightCategory;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
	public static class CourseMapper
	{

		/// <summary>
		/// Maps a Course business object to a Course model.
		/// </summary>
		/// <param name="biz">The Course business object.</param>
		/// <returns>
		/// Course model.
		/// </returns>
		public static Course ToCourse(this Biz.DataContracts.Course biz)
		{
			var model = new Course();

			if (null != biz)
			{
				model.Id = biz.Id;
				model.Title = biz.Title;
				model.ActivatedDate = biz.ActivatedDate;
			    model.CourseProductName = biz.CourseProductName;
			    model.CourseUserName = biz.InstructorName;
			    model.CourseTimeZone = biz.CourseTimeZone;
				model.CourseNumber = biz.CourseNumber;
				model.SectionNumber = biz.SectionNumber;
				model.InstructorName = biz.InstructorName;
				model.OfficeHours = biz.OfficeHours;
				model.ContactInformation = biz.ContactInformation;
				model.SyllabusType = biz.SyllabusType;
				model.Syllabus = biz.Syllabus;
				model.IsLoadStartOnInit = biz.IsLoadStartOnInit;
				model.DerivedCourseId = biz.DerivedCourseId;
				model.IsDashboardActive = biz.IsDashboardActive;
				model.IsSandboxCourse = biz.IsSandboxCourse;
                model.TextEditorConfiguration = new TextEditorConfiguration() { EditorOptions = biz.TinyMCE != null ? biz.TinyMCE.EditorOptions : "" };
				model.RubricTypes = biz.RubricTypes;
				model.PassingScore = biz.PassingScore;
				model.UseWeightedCategories = biz.UseWeightedCategories;

				model.CourseType = (CourseType)Enum.Parse(typeof(CourseType), biz.CourseType, true);
				model.CourseOwner = biz.CourseOwner;
				model.CourseTemplate = biz.CourseTemplate;
				model.DashboardCourseId = biz.DashboardCourseId;
				model.CourseHomePage = biz.CourseHomePage;
				model.StudentStartPage = biz.StudentStartPage;
				model.InstructorStartPage = biz.InstructorStartPage;
				model.AssignTabSettings = biz.AssignTabSettings.ToAssignTabSettings();
				model.Theme = biz.Theme;
				model.WelcomeMessage = biz.WelcomeMessage;
				model.BannerImage = biz.BannerImage;
				model.AllowedThemes = biz.AllowedThemes;
				model.AllowCommentSharing = biz.AllowCommentSharing;
				model.AllowSiteSearch = biz.AllowSiteSearch;
				model.AcademicTerm = biz.AcademicTerm;
				model.HideStudentViewLink = biz.HideStudentViewLink;
				model.CourseSectionType = biz.CourseSectionType;
				model.QuestionCardLayout = biz.QuestionCardLayout;
				model.CourseCreationDate = biz.CreatedDate.ToString();
				model.PublishedStatus = biz.PublishedStatus;
				model.FacebookIntegration = biz.FacebookIntegration;
				model.SocialCommentingIntegration = biz.SocialCommentingIntegration;
				model.SocialCommentingAllowedTypes = biz.SocialCommentingAllowedTypes;
				model.AllowActivation = biz.AllowActivation;
				model.SearchEngineIndex = biz.SearchEngineIndex;
				model.IsAllowExtraCredit = biz.IsAllowExtraCredit;
				model.DownloadOnlyDocumentTypes = biz.DownloadOnlyDocumentTypes;
                model.LmsIdRequired = biz.LmsIdRequired;
                model.LmsIdLabel = biz.LmsIdLabel;
                model.LmsIdPrompt = biz.LmsIdPrompt;
			    model.ProductCourseId = biz.ProductCourseId;

                // mishka, map view tab settings to course model
			    if (biz.bfw_tab_settings != null)
			    {
			        TabSettings settings = new TabSettings()
			        {
			            view_tab = new ViewTab()
			            {
			                show_assignment_details = biz.bfw_tab_settings.view_tab.show_assignment_details,
			                show_learning_objectives = biz.bfw_tab_settings.view_tab.show_learning_objectives,
			                show_policies = biz.bfw_tab_settings.view_tab.show_policies,
			                show_rubrics = biz.bfw_tab_settings.view_tab.show_rubrics
			            }
			        };

                    model.bfw_tab_settings = settings;
			    }


				if (biz.AssessmentConfiguration != null)
				{
					model.AssessmentConfiguration = biz.AssessmentConfiguration.ToModel();
				}


				if (!biz.QuestionFilter.FilterMetadata.IsNullOrEmpty())
				{
					List<QuestionFilterMetadata> lstFilterMetadata = new List<QuestionFilterMetadata>();
					foreach (Biz.DataContracts.QuestionFilterMetadata filter in biz.QuestionFilter.FilterMetadata)
					{
						QuestionFilterMetadata filterMetadata = new QuestionFilterMetadata();
						filterMetadata.Name = filter.Name;
						filterMetadata.Filterable = filter.Filterable;
						filterMetadata.Searchterm = filter.Searchterm;
						filterMetadata.Friendlyname = filter.Friendlyname;
						lstFilterMetadata.Add(filterMetadata);
					}
					model.QuestionFilter.FilterMetadata = lstFilterMetadata;
				}

				model.GenericCourseId = biz.GenericCourseId;
				model.GenericCourseSupported = biz.GenericCourseSupported;
				model.EnrollmentSwitchSupported = biz.EnrollmentSwitchSupported;
				model.DomainSwitchSupported = biz.DomainSwitchSupported;
				model.CourseSubType = biz.CourseSubType;
			}

			if (biz.DashboardSettings != null)
			{
				model.DashboardSettings = biz.DashboardSettings.ToDashboardSettings();
			}
			if (biz.Domain != null)
			{
				model.DerivativeDomainId = biz.Domain.Id;
			}
			model.Children = new List<ContentItem>();
			return model;
		}


		/// <summary>
		/// Convert to a course.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static Biz.DataContracts.Course ToCourse(this Course model)
		{
			var biz = new Biz.DataContracts.Course();

			if (null != model)
			{
				biz.Title = model.Title;
				biz.Id = model.Id;
				biz.ActivatedDate = model.ActivatedDate;
                biz.CourseOwner = model.CourseOwner;
				biz.Properties["CourseProductName"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseProductName };
				biz.Properties["CourseUserName"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseUserName };
				biz.Properties["CourseType"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseType.ToString() };
				biz.Properties["CourseOwner"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseOwner };
				biz.Properties["CourseTemplate"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseTemplate };
				biz.Properties["DashboardCourseId"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.DashboardCourseId };
				biz.Properties["CourseHomePage"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseHomePage };

				biz.Properties["Theme"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.Theme };
				biz.Properties["WelcomeMessage"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.WelcomeMessage };
				biz.Properties["BannerImage"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.BannerImage };
				biz.Properties["AllowedThemes"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.AllowedThemes.ToString() };
				biz.Properties["AllowCommentSharing"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.Boolean, Value = model.AllowCommentSharing };
				biz.Properties["HideStudentViewLink"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.Boolean, Value = model.HideStudentViewLink };

				biz.Properties["SectionNumber"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.SectionNumber };
				biz.Properties["CourseNumber"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.CourseNumber };

				biz.SocialCommentingIntegration = model.SocialCommentingIntegration;

				biz.Properties["DownloadOnlyDocumentTypes"] = new Biz.DataContracts.PropertyValue() { Type = Biz.DataContracts.PropertyType.String, Value = model.DownloadOnlyDocumentTypes };

				//// mishka, map view tab settings to course model
				if (model.bfw_tab_settings != null & ( biz.bfw_tab_settings != null && biz.bfw_tab_settings.view_tab != null ))
				{
					biz.bfw_tab_settings.view_tab.show_assignment_details = model.bfw_tab_settings.view_tab.show_assignment_details;
					biz.bfw_tab_settings.view_tab.show_learning_objectives = model.bfw_tab_settings.view_tab.show_learning_objectives;
					biz.bfw_tab_settings.view_tab.show_policies = model.bfw_tab_settings.view_tab.show_policies;
					biz.bfw_tab_settings.view_tab.show_rubrics = model.bfw_tab_settings.view_tab.show_rubrics;
				}

				//add grade book category
                if (!model.GradeBookCategoryList.IsNullOrEmpty())
                {
                    List<GradeBookWeightCategory> GradeBookCategory_List = new List<GradeBookWeightCategory>();

                    foreach (var Category in model.GradeBookCategoryList)
                    {
                        GradeBookCategory_List.Add(new GradeBookWeightCategory()
                        {
                            Id = Category.Id,
                            Text = Category.Text,
                            Weight = Category.Weight
                        });
                    }

                    biz.GradeBookCategoryList = GradeBookCategory_List;
                }

				biz.DerivedCourseId = model.DerivedCourseId;
				biz.DashboardSettings = model.DashboardSettings.ToDashboardSettings();
                biz.LmsIdRequired = model.LmsIdRequired;
                biz.LmsIdPrompt = model.LmsIdPrompt;
                biz.LmsIdLabel = model.LmsIdLabel;
			}

			return biz;
		}
	}
}
