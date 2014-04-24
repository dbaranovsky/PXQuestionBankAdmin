using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IProductCourseManagementService
    {
        Course GetProductCourse(string productCourseId);
    }
}