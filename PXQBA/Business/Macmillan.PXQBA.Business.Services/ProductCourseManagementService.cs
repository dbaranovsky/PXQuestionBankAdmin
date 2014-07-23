using System.Collections.Generic;
using System.Runtime.Caching;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;

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
            roleOperation.UpdateRolesCapabilities(newCourse.ProductCourseId, PredefinedRoleHelper.GetPredefinedRoles());
            roleOperation.GrantPredefinedRoleToCurrentUser(PredefinedRole.SuperAdministrator, newCourse.ProductCourseId);
            newCourse.FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                         {
                                             GetFieldDescriptor(MetadataFieldNames.Bank),
                                             GetFieldDescriptor(MetadataFieldNames.Chapter)
                                         };
            productCourseOperation.UpdateCourse(newCourse);
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
