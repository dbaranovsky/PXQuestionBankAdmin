using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class StudentProfileMapper
    {
        /// <summary>
        /// Converts biz object to model
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
       public static StudentProfile ToStudentProfile(this BizDC.StudentProfile biz)
       {
            var model = new StudentProfile();
            model.EnrollmentId = biz.EnrollmentId;
            model.UserId = biz.UserId;
            model.LastLogin = biz.LastLogin;
            model.FirstName = biz.FirstName;
            model.LastName = biz.LastName;
            model.FormattedName = biz.FormattedName;
            model.TotalPossiblePoints = biz.TotalPossiblePoints;
            model.TotalAssignedPoints = biz.TotalAssignedPoints;
            model.Email = biz.Email;
            return model;
       }
    }
}
