using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    public class GradingController:Controller
    {
        /// <summary>
        /// 
        /// </summary>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected BizSC.IPxGradeBookActions GradeBookActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Bfw.PX.PXPub.Controllers.Mappers.ContentHelper ContentHelper {get; set;}

        /// <summary>
        /// widget's pubic constructor
        /// </summary>
        /// <param name="context"></param>
        public GradingController(BizSC.IBusinessContext context, BizSC.IEnrollmentActions enrollmentActions, BizSC.IPxGradeBookActions gradeBookActions,
                                 BizSC.IContentActions contentActions, BizSC.IGradeActions gradeActions, Bfw.PX.PXPub.Controllers.Mappers.ContentHelper contenthelper)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            GradeBookActions = gradeBookActions;
            ContentActions = contentActions;
            GradeActions = gradeActions;
            ContentHelper = contenthelper;
        }

        /// <summary>
        /// Return Students who are enrolled in course and have at least one submission
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected IList<Biz.DataContracts.UserInfo> GetStudentsWithSubmissions(string itemId, bool assigned, out List<string> lstLookup)
        {
            var userinfo = new List<Biz.DataContracts.UserInfo>();
            var enrollments = EnrollmentActions.GetEntityEnrollments(Context.EntityId, BizDC.UserType.Student);
            lstLookup = new List<string>();

            foreach (var enrollment in enrollments)
            {
                var attempts = GradeBookActions.GetAttemptsByStudent(itemId, enrollment.Id);
                var grade = GradeBookActions.GetGradesByEnrollment(enrollment.Id, assigned).Where(e => e.GradedItem.Id == itemId).ToList();

                enrollment.User.SubmissionStatus = GetGradeStatus(grade.FirstOrDefault(), itemId, enrollment.Id);
                lstLookup.Add(enrollment.Id + "|" + enrollment.User.SubmissionStatus);
                enrollment.User.EnrollmentIdForCurrentCourse = enrollment.Id;

                if (attempts == null || attempts.Count() <=0) continue;

                userinfo.Add(enrollment.User);
            }

            return userinfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="enrollmentid"></param>
        /// <returns></returns>
        public ActionResult GetHeader(string itemid, string enrollmentid, bool assigned=true)
        {
            var item = ContentActions.GetItems(Context.EntityId, new List<string>() { itemid }).FirstOrDefault();
            var title = item.Title;
            var dueDate = item.AssignmentSettings.DueDate.ToShortDateString();
            var points = item.AssignmentSettings.Points;
            var grade = GradeBookActions.GetGradesByEnrollment(enrollmentid, assigned).Where(e=>e.GradedItem.Id==itemid).ToList();
            List<string> lstLookup;
            var enrollments = GetStudentsWithSubmissions(itemid, assigned, out lstLookup);
            var isInstructor = (Context.AccessLevel == BizSC.AccessLevel.Instructor);
            var status = GetGradeStatus(grade.FirstOrDefault(), itemid, enrollmentid);

            ViewData["Lookup"] = lstLookup;
            ViewData["Enrollments"] = enrollments;
            ViewData["DueDate"] = dueDate;
            ViewData["Grade"] = grade;
            ViewData["Title"] = title;
            ViewData["GradeStatus"] = status;
            ViewData["IsInstructor"] = isInstructor;

            return View("~/Views/Grading/Header.ascx");
        }

        /// <summary>
        /// Gets the Grade Status of an assignment for a student. GradeStatus can be 'Saved', 'NeedsGrading', 'Graded'
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="itemid"></param>
        /// <param name="enrollmentid"></param>
        /// <returns></returns>
        protected BizDC.SubmissionStatus GetGradeStatus(BizDC.Grade grade, string itemid, string enrollmentid)
        {
            //GradeStatus can be 'Saved', 'NeedsGrading', 'Graded'

            var gradeStatus = BizDC.SubmissionStatus.Unsubmitted;

            if (grade.ScoredVersion > 0)
            {
                gradeStatus = BizDC.SubmissionStatus.Graded;
            }
            else if(grade.SubmittedVersion  > 0)
            {
                var submission = GradeActions.GetStudentSubmission(enrollmentid, grade.ItemId);
                //get the latest submission action
                var submissionAction = submission.Actions.OrderByDescending(a => a.Date).FirstOrDefault();
                if (submissionAction.Type == BizDC.SubmissionActionType.save)
                    gradeStatus = BizDC.SubmissionStatus.Saved;
                if (submissionAction.Type == BizDC.SubmissionActionType.submit)
                    gradeStatus = BizDC.SubmissionStatus.NotGraded;
            }

            return gradeStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDetail()
        {
            return View("~/Views/Grading/DisplayItem.ascx");
        }
    }
}
