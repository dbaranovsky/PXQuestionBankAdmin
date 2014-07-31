using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.Common.Logging;
using Course = Macmillan.PXQBA.Business.Models.Course;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Represents the service that handles operations with titles
    /// </summary>
    public class ProductCourseManagementService : IProductCourseManagementService
    {
        private readonly IProductCourseOperation productCourseOperation;
        private readonly IRoleOperation roleOperation;

        public ProductCourseManagementService(IProductCourseOperation productCourseOperation, IRoleOperation roleOperation)
        {
            this.productCourseOperation = productCourseOperation;
            this.roleOperation = roleOperation;
        }

        public Course GetProductCourse(string productCourseId, bool requiredQuestionBankRepository = false)
        {
            return productCourseOperation.GetProductCourse(productCourseId, requiredQuestionBankRepository);
        }

        public IEnumerable<Course> GetAvailableCourses()
        {
            return productCourseOperation.GetUserAvailableCourses();
        }

        public IEnumerable<Course> GetCourseList()
        {
            return productCourseOperation.GetUserAvailableCourses(true);
        }

        public Course UpdateMetadataConfig(Course course)
        {
            return productCourseOperation.UpdateCourse(course);
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return productCourseOperation.GetAllCourses();
        }

        public void CreateNewDraftCourse(string title)
        {
            var newCourse = productCourseOperation.CreateDraftCourse(title);
            GrantUserSuperAdmin(newCourse.ProductCourseId);
            newCourse.AddStaticFieldsToCourse();
            productCourseOperation.UpdateCourse(newCourse);
        }
        private void GrantUserSuperAdmin(string productCourseId)
        {
            roleOperation.UpdateRolesCapabilities(productCourseId, PredefinedRoleHelper.GetPredefinedRoles());
            roleOperation.GrantPredefinedRoleToCurrentUser(PredefinedRole.SuperAdministrator, productCourseId);
        }

        public string AddSiteBuilderCourse(string url)
        {
            try
            {
                var courseId = productCourseOperation.AddSiteBuilderCourseToQBA(url);
                if (!string.IsNullOrEmpty(courseId))
                {
                    var course = productCourseOperation.GetProductCourse(courseId);
                    course.AddStaticFieldsToCourse();
                    productCourseOperation.UpdateCourse(course);
                    GrantUserSuperAdmin(courseId);
                }
                return courseId;
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("ProductCourseManagementService.AddSiteBuilderCourse: Site Builder course wasn't found\added."), ex);
                return null;
            }
        }

        public void RemoveResources(string getTemporaryCourseId, List<string> getQuestionRelatedResources)
        {
            productCourseOperation.RemoveResources(getTemporaryCourseId, getQuestionRelatedResources);
        }

        public void PutResources(List<Resource> resources)
        {
            productCourseOperation.PutResources(resources);
        }
    }

       
    }
