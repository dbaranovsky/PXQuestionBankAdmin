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

        public Course GetProductCourse(string productCourseId)
        {
            return productCourseOperation.GetProductCourse(productCourseId);
        }
    }
}