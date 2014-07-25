using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class MetadataController : MasterController
    {

        private readonly IProductCourseManagementService productCourseManagementService;

        private readonly IUserManagementService userManagementService;

        public MetadataController(IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
            :base(productCourseManagementService, userManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
            this.userManagementService = userManagementService;
        }

        public ActionResult Index(string courseId)
        {
            if (String.IsNullOrEmpty(courseId))
            {
                courseId = null;
            }

            return View(new MetadataConfigPageViewModel()
                        {
                            SelectedCourseId = courseId
                        });
        }


        [HttpPost]
        public ActionResult GetAvailableCourses()
        {
            var courses = Mapper.Map<IEnumerable<ProductCourseViewModel>>(
                                            productCourseManagementService.GetAvailableCourses());
            return JsonCamel(courses);
        }

        [HttpPost]
        public ActionResult GetMetadataConfig(string courseId)
        {
            var course = productCourseManagementService.GetProductCourse(courseId);
            UpdateCurrentCourse(courseId);
            var viewModel = Mapper.Map<MetadataConfigViewModel>(course);
            UpdateCapabilities(viewModel);
            return JsonCamel(viewModel);
        }

        private void UpdateCapabilities(MetadataConfigViewModel viewModel)
        {
            viewModel.CanEditMetadataValues = UserCapabilitiesHelper.Capabilities.Contains(Capability.EditMetadataConfigValues);
            viewModel.CanEditQuestionCardTemplate = UserCapabilitiesHelper.Capabilities.Contains(Capability.EditQuestionCardTemplate);
            viewModel.CanEditTitleMetadataFull = UserCapabilitiesHelper.Capabilities.Contains(Capability.EditTitleMetadataFull);
            viewModel.CanEditTitleMetadataReduced = UserCapabilitiesHelper.Capabilities.Contains(Capability.EditTitleMetadataReduced);
            if (viewModel.CanEditTitleMetadataFull)
            {
                viewModel.CanEditTitleMetadataReduced = true;
            }
        }

        [HttpPost]
        public ActionResult SaveMetadataConfig(MetadataConfigViewModel metadataConfig)
        {
            var course = Mapper.Map(metadataConfig, 
                                   productCourseManagementService.GetProductCourse(metadataConfig.CourseId, true));
            UpdateCurrentCourse(course.ProductCourseId);
            if (!IsAuthorizedToSave(course))
            {
                return new HttpUnauthorizedResult();
            }
            productCourseManagementService.UpdateMetadataConfig(course);
            CourseHelper.CurrentCourse = null;
            return JsonCamel(new {IsError = false});
        }

        private bool IsAuthorizedToSave(Course course)
        {
             if ((!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditTitleMetadataFull))&&
                ((!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditTitleMetadataReduced))))
             {
                 return false;
             }
          
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditQuestionCardTemplate) &&
                course.QuestionCardLayout != CourseHelper.CurrentCourse.QuestionCardLayout)
            {
                return false;
            }
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditMetadataConfigValues))
            {
                var existingFields = course.FieldDescriptors.Where(f => CourseHelper.CurrentCourse.FieldDescriptors.Select(d => d.Name).Contains(f.Name));
                var newFields = course.FieldDescriptors.Where(f => !CourseHelper.CurrentCourse.FieldDescriptors.Select(d => d.Name).Contains(f.Name));
                if (newFields.Any(f => f.CourseMetadataFieldValues.Any()))
                {
                    return false;
                }
                foreach (var existingField in existingFields)
                {
                    var oldValues = CourseHelper.CurrentCourse.FieldDescriptors.Where(f => f.Name == existingField.Name).SelectMany(f => f.CourseMetadataFieldValues.Select(v => v.Text));
                    var newValues = existingField.CourseMetadataFieldValues.Select(v => v.Text);

                    if (!(oldValues.IsCollectionEqual(newValues)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}