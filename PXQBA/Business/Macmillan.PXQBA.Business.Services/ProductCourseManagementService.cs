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

        public ProductCourseManagementService(IProductCourseOperation productCourseOperation)
        {
            this.productCourseOperation = productCourseOperation;
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
    }
}