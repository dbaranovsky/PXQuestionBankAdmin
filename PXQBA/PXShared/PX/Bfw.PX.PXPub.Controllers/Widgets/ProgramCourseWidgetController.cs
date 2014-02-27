using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDS = Bfw.PX.Biz.Direct.Services;

using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using System.Web.Mvc;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class ProgramCourseWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Access to the current business context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to EporfolioCourseActions functionality
        /// </summary>
        protected BizSC.IEportfolioCourseActions EportfolioCourseActions { get; set; }

        /// <summary>
        /// Access to CourseActions functionality
        /// </summary>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Access to RaServices functionality
        /// </summary>
        protected BizDS.RAServices RaServices { get; set; }

        /// <summary>
        /// Access to RaServices functionality
        /// </summary>
        protected BizDS.NoteActions NoteActions { get; set; }

        /// <summary>
        /// Access to UserActions functionality
        /// </summary>
        protected BizDS.UserActions UserActions { get; set; }

        /// <summary>
        /// Access to EnrollmentActions functionality
        /// </summary>
        protected BizDS.EnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        public ProgramCourseWidgetController(BizSC.IBusinessContext context, BizSC.IEportfolioCourseActions eportfolioCourseActions, BizSC.ICourseActions courseActions, BizDS.RAServices raServices, BizDS.NoteActions noteActions, BizDS.UserActions userActions, BizDS.EnrollmentActions enrollmentActions)
        {
            Context = context;
            EportfolioCourseActions = eportfolioCourseActions;
            CourseActions = courseActions;
            RaServices = raServices;
            NoteActions = noteActions;
            UserActions = userActions;
            EnrollmentActions = enrollmentActions;
        }

        #region IPXWidget members

        public ActionResult Summary(Models.Widget widget)
        {
            var academicTerms = CourseActions.ListAcademicTerms();
            var programWidget = new ProgramCourseWidget();
            var currentTerm = academicTerms.FirstOrDefault(e => (e.StartDate ?? DateTime.MinValue) < DateTime.Now.GetCourseDateTime() && (e.EndDate ?? DateTime.MinValue) > DateTime.Now);

            programWidget.AcademicTerm = academicTerms.Select(e => e.ToAcademicTerm()).ToList();
            programWidget.CurrentAcademicTerm = currentTerm != null ? currentTerm.Id : string.Empty;

            return View(programWidget);
        }

        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        #endregion

        public ActionResult TermCourses(string termId, String courseIdWithYellowFade = "")
        {
            var eportfolioCourses = EportfolioCourseActions.GetEportfolioCourses(termId);
            var courses = eportfolioCourses.Map(e => new DashboardItem
                {
                    CourseId = e.Course.Id,
                    CourseTitle = e.Course.Title,
                    Status = e.Status,
                    Count = e.Count,
                    OwnerName = e.OwnerName,
                    OwnerReferenceId = e.OwnerReferenceId,
                    OwnerId = e.OwnerId
                });
            ViewData["CurrentUser"] = Context.CurrentUser.Id;
            ViewData["CourseIdsWithYellowFade"] = courseIdWithYellowFade.IfNull("").Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            return View("~/Views/ProgramCourseWidget/TermCourses.ascx", courses);
        }

        public ActionResult ReAssignInstructor(String courseId, String userRefId)
        {
            BizDC.Course course = CourseActions.GetCourseByCourseId(courseId);
            ViewData["CourseName"] = course.Title;
            ViewData["CourseId"] = courseId;
            Dictionary<int, String> instructors = new Dictionary<int, String>();

            IList<BizDC.UserProgramByDomain> insts = EportfolioCourseActions.GetUserProgramByDomain(Context.Domain.Id);
            String[] user_ref_ids = insts.Map(inst => inst.UserRefId.ToString()).ToArray();
            BizDC.UserProfileResponse upr = RaServices.GetUserProfile(user_ref_ids);

            foreach (BizDC.UserProfile up in upr.UserProfile)
            {
                // the user being taken off the course should not be in the list of users to be switched to.
                if (!String.IsNullOrEmpty(userRefId) && userRefId.Equals(up.ReferenceId))
                    continue;

                int refid = 0;
                if (!Int32.TryParse(up.ReferenceId, out refid))
                    continue;
                if(!instructors.ContainsKey(refid))
                    instructors.Add(refid, String.Format("{0}, {1}", up.LastName, up.FirstName).Trim(' ', ','));
            }
            
            return View(instructors.OrderBy(item => item.Value));
        }

        public ActionResult UpdateInstructor(String courseId, String instructorId)
        {
            var result = new JsonResult();

            // pull the source course for information to help create the new course
            BizDC.Course oldCourse = CourseActions.GetCourseByCourseId(courseId);

            // get new instructors dashboard information
            BizDC.UserProgram userProgram = EportfolioCourseActions.GetUserProgram(new BizDC.UserProgramSerach(){ UserRefId = instructorId, UserDomainId =  oldCourse.Domain.Id });

            // could not find user, must not be an instructor
            if (null == userProgram){
                result.Data = Json(new { success = false });
                return result;
            }
            
            // if the dashboard id could not be found, create one
            String dashboardCourseId = userProgram.UserDashboardId.HasValue ? userProgram.UserDashboardId.ToString() : null;
            if (dashboardCourseId.IsNullOrEmpty()){
                dashboardCourseId = EportfolioCourseActions.CreateDashboard(
                    userProgram.ProgramId.ToString(),
                    userProgram.ProgramDashboardId.ToString(),
                    userProgram.UserId.ToString(),
                    "",
                    oldCourse.Domain.Id,
                    oldCourse.AcademicTerm,
                    oldCourse.CourseTimeZone);
            }

            BizDC.UserProfileResponse raResponse = RaServices.GetUserProfile(new String[] { instructorId });
            BizDC.UserProfile raUserProfile = raResponse.UserProfile.FirstOrDefault();
            if (null != raUserProfile){
                BizDC.Course dashboardCourse = CourseActions.GetCourseByCourseId(dashboardCourseId);
                dashboardCourse.InstructorName = String.Join(" ", new String[] { raUserProfile.FirstName, raUserProfile.LastName }).Trim(' ', ',');
                CourseActions.UpdateCourse(dashboardCourse);
            }

            BizDC.Course newCourse = EportfolioCourseActions.MoveInstructorCourseAcrossUserLineage(courseId, dashboardCourseId);
            
            var userInfo = UserActions.GetUser(userProgram.UserId.ToString());
            NoteActions.InitializeUser(userInfo, newCourse.Id, BizDC.UserType.Student);
            NoteActions.InitializeUser(userInfo, newCourse.Id, BizDC.UserType.Instructor);

            // remove the instructor from the old course
            BizDC.Enrollment enrollment = EnrollmentActions.GetEnrollment(oldCourse.CourseOwner, oldCourse.Id);
            if (null != enrollment)
                EnrollmentActions.InactiveEnrollment(enrollment);

            // delete the old course
            bool deletedSourceCourse = EportfolioCourseActions.DeleteEportfolioCourses(courseId);

            return TermCourses(newCourse.AcademicTerm, newCourse.Id);
        }
    }
}
