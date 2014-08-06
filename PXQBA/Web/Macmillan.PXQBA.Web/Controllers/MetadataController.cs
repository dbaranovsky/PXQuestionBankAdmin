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
            var viewModel = Mapper.Map<MetadataConfigViewModel>(course);
            UpdateCapabilities(courseId, viewModel);
            return JsonCamel(viewModel);
        }

        private void UpdateCapabilities(string courseId, MetadataConfigViewModel viewModel)
        {
            var capabilities = UserCapabilitiesHelper.GetCapabilities(courseId).ToList();
            viewModel.CanEditMetadataValues = capabilities.Contains(Capability.EditMetadataConfigValues);
            viewModel.CanEditQuestionCardTemplate = capabilities.Contains(Capability.EditQuestionCardTemplate);
            viewModel.CanEditTitleMetadataFull = capabilities.Contains(Capability.EditTitleMetadataFull);
            viewModel.CanEditTitleMetadataReduced = capabilities.Contains(Capability.EditTitleMetadataReduced);
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
            var oldCourse = productCourseManagementService.GetProductCourse(course.ProductCourseId, true);

            if (!IsAuthorizedToSave(course, oldCourse))
            {
                return new HttpUnauthorizedResult();
            }
            productCourseManagementService.UpdateMetadataConfig(course);

            CourseHelper.ClearCache();
            return JsonCamel(new {IsError = false});
        }

        private bool IsAuthorizedToSave(Course newCourse, Course oldCourse)
        {
            var capabilities = UserCapabilitiesHelper.GetCapabilities(oldCourse.ProductCourseId).ToList();
            if ((!capabilities.Contains(Capability.EditTitleMetadataFull)) &&
                ((!capabilities.Contains(Capability.EditTitleMetadataReduced))))
             {
                 return false;
             }

            if (!capabilities.Contains(Capability.EditQuestionCardTemplate) &&
                newCourse.QuestionCardLayout != oldCourse.QuestionCardLayout)
            {
                return false;
            }
            if (!capabilities.Contains(Capability.EditMetadataConfigValues))
            {
                var existingFields = newCourse.FieldDescriptors.Where(f => oldCourse.FieldDescriptors.Select(d => d.Name).Contains(f.Name));
                var newFields = newCourse.FieldDescriptors.Where(f => !oldCourse.FieldDescriptors.Select(d => d.Name).Contains(f.Name));
                if (newFields.Any(f => f.CourseMetadataFieldValues.Any()))
                {
                    return false;
                }
                foreach (var existingField in existingFields)
                {
                    var oldValues = oldCourse.FieldDescriptors.Where(f => f.Name == existingField.Name).SelectMany(f => f.CourseMetadataFieldValues.Select(v => v.Text));
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