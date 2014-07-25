using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.Common.Logging;

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
            newCourse.FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                         {
                                             GetFieldDescriptor(MetadataFieldNames.Bank),
                                             GetFieldDescriptor(MetadataFieldNames.Chapter)
                                         };
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

        private CourseMetadataFieldDescriptor GetFieldDescriptor(string internalName)
        {
            return new CourseMetadataFieldDescriptor
            {
                Type = MetadataFieldType.SingleSelect,
                FriendlyName = internalName[0].ToString().ToUpper() + internalName.Substring(1),
                Name = internalName,
                Filterable = true,
                DisplayInBanks = true,
                ShowFilterInBanks = true,
                MatchInBanks = true,
                DisplayInCurrentQuiz = true,
                DisplayInInstructorQuiz = true,
                DisplayInResources = true,
                ShowFilterInResources = true,
                MatchInResources = true,
                CourseMetadataFieldValues = null
            };
        }
    }

       
    }
