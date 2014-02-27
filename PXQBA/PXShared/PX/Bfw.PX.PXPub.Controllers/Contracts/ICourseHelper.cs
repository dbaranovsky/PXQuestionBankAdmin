using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface ICourseHelper
    {
        IEnumerable<Course> ListCourses(string userReferenceId, string productId, bool includeGenericCourse, bool includeDashboard, string sProductType);
    }
}
