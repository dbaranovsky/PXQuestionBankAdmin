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
    public static class StudentMapper
    {

        /// <summary>
        /// Converts to a Student from a Biz Share note result.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Student ToStudent(this BizDC.ShareNoteResult model)
        {
            var biz = new Student();

            biz.FirstName = model.FirstNameSharee;
            biz.LastName = model.LastNameSharee;
            biz.Id = model.SharedStudentId;

            return biz;
        }
        /// <summary>
        /// Maps a UserInfo object to a Student object.
        /// </summary>
        /// <param name="biz">UserInfo business object.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Account model.
        /// </returns>
        public static Student ToStudent(this BizDC.UserInfo biz, BizDC.UserType type)
        {
            var model = new Student();

            if (null != biz)
            {
                model.Id = biz.Id;
                model.FirstName = biz.FirstName;
                model.LastName = biz.LastName;
                model.Email = biz.Email;
                model.IsInstructor = (type == BizDC.UserType.Instructor);
            }

            return model;
        }

        /// <summary>
        /// Maps an Enrollment object to a Student object.
        /// </summary>
        /// <param name="biz">UserInfo business object.</param>
        /// <returns>
        /// Account model.
        /// </returns>
        public static Student ToStudent(this BizDC.Enrollment biz)
        {
            var model = biz.User.ToStudent(BizDC.UserType.All);

            model.EnrollmentId = biz.Id;
            model.CourseCompletePercentage = Convert.ToInt32(biz.PercentGraded);
            model.LastLoginDescription = TimeAgoDescription(biz.User.LastLogin);

            return model;
        }

        /// <summary>
        /// Times the Ago description.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        private static string TimeAgoDescription(DateTime? dt)
        {
            if (dt == null)
            {
                return "Unknown";
            }

            TimeSpan ts = DateTime.Now.GetCourseDateTime() - dt.Value;

            if (ts.Days > 0)
            {
                return String.Format("{0} {1} ago", ts.Days, ts.Days == 1 ? "day" : "days");
            }

            if (ts.Hours > 0)
            {
                return String.Format("{0} {1} ago", ts.Hours, ts.Hours == 1 ? "hour" : "hours");
            }

            if (ts.Minutes > 0)
            {
                return String.Format("{0} {1} ago", ts.Minutes, ts.Minutes == 1 ? "minute" : "minutes");
            }

            return "Currently logged in";
        }
    }
}
