using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that create and auto enroll users
    /// </summary>
    public interface IAutoEnrollmentActions
    {
        /// <summary>
        /// Create users and enrollments for the current course.
        /// </summary>
        /// <returns></returns>
        Boolean CreateEnrollments();        
    }
}