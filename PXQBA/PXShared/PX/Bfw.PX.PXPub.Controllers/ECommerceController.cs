using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Configuration;
//using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.Direct.Services;
using PxWebUser;
using Course = Bfw.PX.PXPub.Models.Course;
using Domain = Bfw.PX.PXPub.Models.Domain;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers
{
    public class ECommerceController : Controller
    {
        /// <summary>
        /// Gets or sets the context
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        protected BizSC.IContentActions ContentActions { get; set; }

        protected BizSC.ICourseActions CourseActions { get; set; }

        protected BizSC.IUserActions UserActions { get; set; }

        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        protected BizSC.IDomainActions DomainActions { get; set; }

        /// <summary>
        /// Gets or sets the course helper
        /// </summary>
        public ICourseHelper CourseHelper { get; set; }

        /// <summary>
        /// A const for that holds the Generic Domain Name.
        /// </summary>
        private readonly string GENERIC_DOMAIN_NAME = System.Configuration.ConfigurationManager.AppSettings["GenericCourseDomain"];

        public ECommerceController(BizSC.IBusinessContext context, BizSC.IContentActions content, BizSC.ICourseActions course, BizSC.IUserActions user, BizSC.IEnrollmentActions enroll, BizSC.IDomainActions domainActions, ICourseHelper courseHelper)
        {
            Context = context;
            ContentActions = content;
            CourseActions = course;
            UserActions = user;
            EnrollmentActions = enroll;
            DomainActions = domainActions;
            CourseHelper = courseHelper;

            //get the activation link info
            ViewData["StudentAccessCodeLink"] = ConfigurationManager.AppSettings["eComActivationCode"].ToString();
            ViewData["PurchaseLink"] = ConfigurationManager.AppSettings["ecomPurchaseAccess"].ToString();
            ViewData["TrialAccessLink"] = ConfigurationManager.AppSettings["ecomTrialAccess"].ToString();
            ViewData["ecomRequestAccess"] = ConfigurationManager.AppSettings["ecomRequestAccess"].ToString();

            string isbn = String.IsNullOrEmpty(context.Course.Isbn13) ? string.Empty : context.Course.Isbn13; //hard coding a isbn until the product info is set properly


            ViewData["ISBN"] = isbn;
            ViewData["Isbn10"] = Bfw.Common.ISBNHelper.ConvertIsbn13To10(isbn);
            ViewData["allowSampling"] = Context.Course.AllowSampling;
            ViewData["allowPurchase"] = Context.Course.AllowPurchase;
            ViewData["allowTrialAccess"] = Context.Course.AllowTrialAccess;
        }

        public ActionResult Index()
        {
            if (Context.CurrentUser != null)
            {
                //RedirectToAction("Authenticated");
                return RedirectToAction("Authenticated", "ECommerce");
            }
            else
            {
                //RedirectToAction("Unauthenticated");
                return RedirectToAction("Unauthenticated", "ECommerce");
            }
        }

        /// <summary>
        /// The buffer page that accepts returnUrl and redirects to the login page using js (to preserve hash)
        /// </summary>
        /// <returns></returns>
        public ActionResult UnauthenticatedTransfer()
        {
            return View();
        }

        /// <summary>
        /// Returns the view when the user is unauthenticated 
        /// </summary>
        /// <returns></returns>
        public ActionResult Unauthenticated()
        {
            ECommerceInfo ecomInfo = new ECommerceInfo()
            {
                IsProduct = Context.CourseIsProductCourse,
                MarketingInfo = GetMarketingInfo(Context.CourseId)
            };

            string course_info = "";
            if (!ecomInfo.IsProduct)
            {
                //need to display the deriv course/section info
                var course = CourseActions.GetCourseByCourseId(Context.CourseId);
                course_info = course.InstructorName;
                ecomInfo.CourseTitle = course.Title;
            }

            ViewData["course_info"] = course_info;
            return View("Index", ecomInfo);
        }


        public ActionResult Authenticated()
        {
            if (Context.AccessType == BizSC.AccessType.Adopter)
            {
                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    return RedirectToAction("Entitled");
                }
                else
                {
                    return RedirectToAction("Enroll");
                }
            }
            else
            {
                return RedirectToAction("NotEntitled");
            }
        }


        public ActionResult NotEntitled()
        {

            ECommerceInfo ecomInfo = new ECommerceInfo()
            {
                IsProduct = Context.CourseIsProductCourse,
                MarketingInfo = GetMarketingInfo(Context.CourseId),
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                Authenticated = true,
                IsEntitled = false
            };

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                ecomInfo.IsInstructor = true;
            else
                ecomInfo.IsStudent = true;

            string course_info = "";
            if (!ecomInfo.IsProduct)
            {
                //need to display the deriv course/section info
                var course = CourseActions.GetCourseByCourseId(Context.CourseId);
                course_info = string.Format("{0}-{1}-{2}<br/>{3}", course.CourseNumber, course.Title, course.SectionNumber, course.InstructorName);

            }

            ViewData["course_info"] = course_info;
            return View("Index", ecomInfo);

        }


        public ActionResult Entitled()
        {
            if (Context.CurrentUser == null)
            {
                return RedirectToAction("Unauthenticated");
            }

            bool AllowEditSandboxCourse = false;
            bool AllowQuestionBankAdmin = false;
            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                if (Context.CurrentUser.WebRights != null && Context.CurrentUser.WebRights.AdminTool != null)
                    AllowEditSandboxCourse = Context.CurrentUser.WebRights.AdminTool.AllowEditSandboxCourse;

                if (Context.CurrentUser.WebRights != null && Context.CurrentUser.WebRights.QuestionBank != null)
                    AllowQuestionBankAdmin = Context.CurrentUser.WebRights.QuestionBank.ShowQuestionBankManager;
            }

            if (!Context.CourseIsProductCourse && RouteData.Values["switchEnrollFromCourse"] != null)
            {
                return RedirectToRoute("EcomJoin");
            }

            var derivedCourseIds = GetDerivedCourseIds().ToList();

            if (derivedCourseIds.Any() && !AllowEditSandboxCourse && !AllowQuestionBankAdmin && Context.CourseIsProductCourse)
            {
                // TODO: make into a course config
                if (Context.AccessLevel == BizSC.AccessLevel.Student && !Context.Course.ToCourse().CourseType.ToString().ToLowerInvariant().Contains("eportfolio") ||
                    derivedCourseIds.Count() == 1 && Context.AccessLevel == BizSC.AccessLevel.Instructor && Context.Course.CourseType.Equals(CourseType.LearningCurve.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    var url = Url.RouteUrl("CourseSectionHome", new { courseid = derivedCourseIds.First() });
                    return Redirect(url);
                } 
                
            }

            ViewData["has_derived_courses"] = (derivedCourseIds.Any());

            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                if (Context.CourseIsProductCourse || Context.Course.CourseSubType == "instructor_dashboard")
                {
                    return RedirectToAction("Enroll");
                }
                else
                {
                    return RedirectToAction("Join");
                }
            }

            ECommerceInfo ecomInfo = new ECommerceInfo()
            {
                IsProduct = Context.CourseIsProductCourse,
                MarketingInfo = GetMarketingInfo(Context.CourseId),
                GettingStartedInfo = GetGettingStartedInfo(Context.CourseId),
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                AllowEditSandboxCourse = AllowEditSandboxCourse,
                AllowQuestionBankAdmin = AllowQuestionBankAdmin,
                Authenticated = true,
                IsEntitled = true,
                IsInstructor = true,
                IsStudent = false
            };

            string course_info = "";
            if (!ecomInfo.IsProduct)
            {
                //need to display the deriv course/section info
                var course = CourseActions.GetCourseByCourseId(Context.CourseId);
                course_info = string.Format("{0}-{1}-{2}<br/>{3}", course.CourseNumber, course.Title, course.SectionNumber, course.InstructorName);
            }

            IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains().Distinct();
            if (userDomains.Count() > 1)
            {
                ecomInfo.InMultipleDomains = true;
            }

            ViewData["course_info"] = course_info;

            ecomInfo.EnrollmentCount = 0;
            var userData = new Bfw.PX.Biz.DataContracts.UserInfo();
            userData.Username = Context.CurrentUser.Username;

            var users = UserActions.ListUsersLike(userData);
            if (!users.IsNullOrEmpty())
            {
                //var courses = Context.FindCoursesByUserEnrollmentBatch(users, Context.ProductCourseId);

                if (!derivedCourseIds.IsNullOrEmpty())
                {
                    ecomInfo.EnrollmentCount = derivedCourseIds.Count();

                    if (ecomInfo.EnrollmentCount > 0)
                    {
                        return RedirectToRoute("Dashboard");
                    }
                }
            }
            return View("Index", ecomInfo);
        }

        private void writecookie(string message)
        {
            System.Web.HttpContext.Current.Response.Cookies.Add(new System.Web.HttpCookie("PX_DEBUG", message));
        }

        public ActionResult Enroll()
        {
            string parentCourseId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.ProductCourseId;
            string genericCourseId = string.Empty;
            string switchEnrollFromCourse = string.Empty;
            Course ParentCourseInfo = CourseActions.GetCourseByCourseId(parentCourseId).ToCourse();
            Course GenericCourseInfo = null;

            if (ParentCourseInfo != null && ParentCourseInfo.GenericCourseSupported && string.IsNullOrEmpty(ParentCourseInfo.GenericCourseId))
            {
                string parentDomainId;
                GenericCourseInfo = CourseActions.GetGenericCourse(string.Empty, out parentDomainId).ToCourse();
                genericCourseId = (GenericCourseInfo != null) ? GenericCourseInfo.Id : string.Empty;
            }
            else
            {
                genericCourseId = ParentCourseInfo.GenericCourseId;
            }

            if (RouteData.Values["switchEnrollFromCourse"] != null)
            {
                switchEnrollFromCourse = RouteData.Values["switchEnrollFromCourse"].ToString();
            }

            ECommerceInfo ecomInfo = new ECommerceInfo()
            {
                IsProduct = Context.CourseIsProductCourse,
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                IsStudent = true,
                Authenticated = true,
                GenericCourseSupported = ParentCourseInfo.GenericCourseSupported,
                GenericCourseId = genericCourseId,
                SwitchEnrollFromCourse = switchEnrollFromCourse
            };

            var forceList = ConfigurationManager.AppSettings["institution_force_list"].Split(',');
            var domains = EnrollmentActions.GetEnrollableDomains(Context.CurrentUser.Username, ConfigurationManager.AppSettings["institution_domain_id"], forceList);
            var allDomains = new List<Domain>();
            Domain blank = new Domain { Id = "", Name = "--Select your school--" };
            allDomains.Add(blank);

            foreach (var d in domains)
            {
                allDomains.Add(d.ToDomain());
            }
            ViewData["domains"] = allDomains;
            ViewData["Courses"] = Context.Course.Id;

            //Temporary code to customize launchPad only enroll course user interfaces
            ViewData["CourseType"] = Context.Course.CourseType == "FACEPLATE" ||
                                     Context.Course.CourseType == "LAUNCHPAD"
                ? "LaunchPad"
                : Context.Course.CourseType;

            return !switchEnrollFromCourse.IsNullOrEmpty() ? View("EnrollForm", ecomInfo) : View(ecomInfo);
        }

        public ActionResult Join()
        {
            string courseid = string.Empty;
            string univ = string.Empty;
            string instructor = string.Empty;
            string switchEnrollFromCourse = string.Empty;
            BizDC.Course course = new BizDC.Course();
            EcomerceJoinCourse EcomJoin = new EcomerceJoinCourse();

            if (Context.Course.Properties.ContainsKey("bfw_show_course_number"))
            {
                EcomJoin.ShowCourseNumber = Convert.ToBoolean(Context.Course.Properties["bfw_show_course_number"].Value.ToString());
            }

            if (Context.Course.Properties.ContainsKey("bfw_show_course_section"))
            {
                EcomJoin.ShowCourseSection = Convert.ToBoolean(Context.Course.Properties["bfw_show_course_section"].Value.ToString());
            }

            if (Request["courses"] == null)
            {
                courseid = RouteData.Values["courseid"].ToString();
            }
            else
            {
                courseid = Request["courses"].ToString();
            }

            if (Request["switchEnrollFromCourse"] != null)
            {
                switchEnrollFromCourse = Request["switchEnrollFromCourse"].ToString();
            }

            if (RouteData.Values["switchEnrollFromCourse"] != null)
            {
                switchEnrollFromCourse = RouteData.Values["switchEnrollFromCourse"].ToString();
            }

            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid);
                if (course.ProductCourseId == null)
                {
                    return RedirectToAction("Enroll");
                }

                univ = course.Domain.Name;
                if (string.IsNullOrEmpty(univ) && course.Domain.Id != null)
                {
                    course.Domain = DomainActions.GetDomainById(course.Domain.Id);
                    univ = course.Domain.Name;
                }

                instructor = course.InstructorName;
                var academicTerms = CourseActions.ListAcademicTerms(course.Domain.Id);
                if (academicTerms != null && academicTerms.Count() > 0 && !string.IsNullOrEmpty(course.AcademicTerm))
                {
                    var academicTermsFirst = academicTerms.FirstOrDefault(x => x.Id == course.AcademicTerm);
                    if (academicTermsFirst != null)
                    {
                        EcomJoin.AcademicTerm = academicTermsFirst.ToAcademicTerm().Name;
                    }
                    else
                    {
                        EcomJoin.AcademicTerm = string.Empty;
                    }

                }
                else
                {
                    EcomJoin.AcademicTerm = string.Empty;
                }
            }

            EcomJoin.Domain = course.Domain.Id;
            EcomJoin.University = univ;
            EcomJoin.CurrentUser = Context.CurrentUser.FormattedName;
            EcomJoin.SwitchEnrollFromCourse = switchEnrollFromCourse;
            EcomJoin.Instructor = instructor;
            EcomJoin.EnrollmentStatus = (EnrollmentStatus)(Context.EnrollmentStatus.IsNullOrEmpty() ? 0 : Int32.Parse(Context.EnrollmentStatus));
            string enrollmentId = string.Empty;
            
            BizDC.UserInfo userToJoin = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, course.Domain.Id);
            
            if (userToJoin.Id != null)
            {
                enrollmentId = EnrollmentActions.GetUserEnrollmentId(userToJoin.Id, course.Id);
            }

            EcomJoin.EnrollmentID = enrollmentId.IsNullOrEmpty() ? "" : enrollmentId;

            EcomJoin.course = course.ToCourse();
            
            ViewData["lmsStudentId"] = Context.CurrentUser.ReferenceId;

            if (!switchEnrollFromCourse.IsNullOrEmpty())
            {
                return View("JoinForm", EcomJoin);
            }

            return View(EcomJoin);
        }

        /// <summary>
        /// Enrollment confirmation is working in two modes
        /// 1. Enrollment switching between two courses
        /// 2. Creating enrollment for new course
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnrollmentConfirmation()
        {
            var lmsStudentId = (Request["lmsStudentId"] != null) ? Request["lmsStudentId"].ToString().Trim() : string.Empty;

            string courseId = Request["courses"].ToString();
            string domainId = Request["domain"].ToString();
            string instructorId = Request["instructors"].ToString();
            string switchEnrollFromCourse = Request["SwitchEnrollFromCourse"].ToString();
            //string sourceId = switchEnrollFromCourse;// Context.CourseId;
            BizDC.Course course = null;
            BizDC.Course originalCourse = null;

            if (!string.IsNullOrEmpty(lmsStudentId))
            {
                UpdateLMSId(lmsStudentId, domainId);
            }

            if (string.IsNullOrEmpty(domainId))
            {
                string parentCourse = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.Course.ProductCourseId;
                Course GenericCourseInfo = CourseActions.GetCourseByCourseId(parentCourse).ToCourse();
                course = CourseActions.GetGenericCourse(GenericCourseInfo.GenericCourseId, out domainId);
            }
            else
            {
                course = CourseActions.GetCourseByCourseId(courseId);
            }

            if (!string.IsNullOrEmpty(switchEnrollFromCourse))
            {
                originalCourse = CourseActions.GetCourseByCourseId(switchEnrollFromCourse);
                BizDC.UserInfo userToSwitch = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, originalCourse.Domain.Id);
                BizDC.UserInfo newUserInfo = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, domainId);
                BizDC.Enrollment enrollment = null;
                BizDC.Enrollment originalEnrollment = null;

                if (userToSwitch != null)
                {
                    originalEnrollment = EnrollmentActions.GetEnrollment(userToSwitch.Id, switchEnrollFromCourse);

                    enrollment = new BizDC.Enrollment()
                    {
                        Id = originalEnrollment.Id,
                        User = newUserInfo,
                        Course = null,
                        CourseId = courseId, // new course id
                        Flags = originalEnrollment.Flags,
                        StartDate = DateTime.Now, //originalEnrollment.StartDate,
                        EndDate = DateTime.Now.AddYears(1), //originalEnrollment.EndDate,
                        OverallGrade = originalEnrollment.OverallGrade,
                        PercentGraded = originalEnrollment.PercentGraded,
                        ItemGrades = originalEnrollment.ItemGrades,                        
                        Status = originalEnrollment.Status,
                        Reference = originalEnrollment.Reference
                    };
                }

                if (!EnrollmentActions.UpdateEnrollment(enrollment))
                {
                    return RedirectToAction("Enroll");
                }

                Context.CacheProvider.StoreEnrollment(enrollment);
                Context.CacheProvider.InvalidateEnrollment(originalEnrollment);
                var productCourseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
            }
            else if (!Context.EnrollmentId.IsNullOrEmpty() && (Context.EnrollmentStatus == "4" || Context.EnrollmentStatus == "10") && !Context.EntityId.IsNullOrEmpty())
            {
                //we just have to set the status to "1" meaning the student is active student.
                var originalEnrollment = EnrollmentActions.GetInactiveEnrollment(Context.CurrentUser.Id, Context.EntityId);
                BizDC.Enrollment enrollment = null;
                enrollment = originalEnrollment;
                enrollment.Status = "1";
                if (!EnrollmentActions.UpdateEnrollment(enrollment))
                {
                    return RedirectToAction("Enroll");
                }
                Context.CacheProvider.StoreEnrollment(enrollment);
                Context.CacheProvider.InvalidateEnrollment(originalEnrollment);
                var productCourseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, productCourseId);

            }
            else
            {
                BizDC.UserInfo userToEnroll = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, domainId);                                

                if (Context.EnrollmentId == null)
                {
                    List<BizDC.Enrollment> enroll = new List<BizDC.Enrollment>();

                    enroll = EnrollmentActions.CreateEnrollments(domainId, userToEnroll.Id, course.Id,
                        ConfigurationManager.AppSettings["StudentPermissionFlags"], "1", DateTime.Now,
                        DateTime.Now.AddYears(1), string.Empty, string.Empty, true);

                    if (enroll.Count() == 0)
                    {
                        return RedirectToAction("Enroll");
                    }
                }
                else
                {
                    return RedirectToRoute("CourseSectionHome");
                }
            }

            if (!switchEnrollFromCourse.IsNullOrEmpty())
            {
                return View("EnrollmentConfirmationForm", course.ToCourse());
            }

            return View(course.ToCourse());
        }

        [HttpGet]
        public JsonResult GetTerms()
        {
            string domainId = Request["id"].ToString();

            var terms = CourseActions.ListAcademicTerms(domainId);
            //var instructors = getinstructors();

            return Json(terms.ToArray(), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetInstructors()
        {
            string domainId = Request["id"].ToString();
            string academicTerm = Request["term"].ToString();
            String application_type = CourseActions.getApplicationType(Context.Course.CourseType);
            var instructors = UserActions.ListInstructorsForDomain(application_type, domainId, academicTerm);

            return Json(instructors.ToArray().OrderBy(e => e.LastName), JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetInstructorCourseList()
        {
            string domainId = Request["id"].ToString();
            string academicTerm = Request["term"].ToString();
            string userId = Request["userid"].ToString();

            String application_type = CourseActions.getApplicationType(Context.Course.CourseType);

            string parentId = string.Empty;

            var user = UserActions.GetUser(userId);

            if (Context.Product != null && Context.Product.IsDashboardActive == true && user != null)
            {
                if (!string.IsNullOrEmpty(Context.Course.ProductCourseId))
                {
                    parentId = Context.Course.ProductCourseId;
                }
            }


            // gets all courses the instructor is teaching for that term
            IEnumerable<BizDC.Course> courses = UserActions.FindCoursesByInstructor(application_type, userId, domainId, academicTerm, parentId);
            // gets the course from dlap (currently does not contain all information)
            var fullCourses = CourseActions.GetCourseListInformation(courses);
            // make sure all courses are activated
            var activeCourses = fullCourses.Where(c => c.ToCourse().IsActivated).OrderByDescending(c => c.Id);

            return Json(activeCourses.ToArray().OrderBy(e => e.CourseNumber).Take(500), JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateLMSId(string sReference, string domainid)
        {
            var fail = new
            {
                success = false,
                message = "Failed to update user"
            };

            //populate userid in case it is missing from context
            if (string.IsNullOrEmpty(Context.CurrentUser.Id))
            {
                var user = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, domainid);

                Context.CurrentUser.Id = user.Id;
            }

            if (string.IsNullOrEmpty(sReference))
            {
                return Json(fail);
            }

            try
            {
                var userinfo = Context.CurrentUser;

                    UserActions.UpdateUser(userinfo.Id, userinfo.DomainId, userinfo.DomainName, userinfo.Username,
                        userinfo.FirstName, userinfo.LastName, userinfo.Email, sReference);
            }
            catch (Exception ex)
            {
                return Json(fail);
            }

            var json = new
            {
                status = "success",
                message = "Update Successful"
            };

            return Json(json);
        }

        /// <summary>
        /// Get derived course ids
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDerivedCourseIds()
        {
            if (Context.CurrentUser != null)
            {
                var referenceId = Context.CurrentUser.Username;
                var courseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                var enrolledCourses = EnrollmentActions.ListEnrollments(referenceId, courseId);
                if (!enrolledCourses.IsNullOrEmpty()) {
                    enrolledCourses = enrolledCourses.OrderByDescending(e => e.StartDate).ToList();
                }
                return enrolledCourses.Select(e => e.Course.Id);

            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Check if this has derived courses
        /// </summary>
        /// <returns></returns>
        public bool HasDerivedCourses()
        {
            var courseIds = GetDerivedCourseIds();

            if (courseIds.IsNullOrEmpty())
            {
                return false;
            }

            return (courseIds.Count() > 0);
        }

        public ActionResult CourseList()
        {
            List<Course> userCourses = new List<Course>();
            IEnumerable<BizDC.Course> deriveUserCourses = null;
            if (Context.Course.CourseType == CourseType.LearningCurve.ToString())
            {
                deriveUserCourses = CourseHelper.ListCourses(Context.CurrentUser.Username, Context.Course.ProductCourseId, false, false, Context.Course.CourseType);
            }
            else
            {
                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    var userData = new Bfw.PX.Biz.DataContracts.UserInfo();
                    userData.Username = Context.CurrentUser.Username;
                    var users = UserActions.ListUsersLike(userData);
                    var courses = Context.FindCoursesByUserEnrollmentBatch(users, Context.ProductCourseId);
                    deriveUserCourses = courses.ToList();

                }
                else
                {
                    deriveUserCourses = Context.FindCoursesByUserEnrollment(Context.CurrentUser.Id, Context.Domain.Id, Context.Product.Id);
                }

                deriveUserCourses = CourseActions.GetCourseListInformation(deriveUserCourses);

                if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    deriveUserCourses = deriveUserCourses.Where(c => c.CourseType == Context.Course.CourseType.ToString());
                    string ParentProductCourseId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.Course.ProductCourseId;
                    deriveUserCourses = deriveUserCourses.Where(c => c.ProductCourseId == ParentProductCourseId);
                }
            }
            if (deriveUserCourses.Count() > 0)
            {

                foreach (BizDC.Course course in deriveUserCourses)
                {
                    try
                    {
                        var m = course.ToCourse();
                        userCourses.Add(m);
                    }
                    catch { }
                }
            }

            ECommerceInfo ecomInfo = new ECommerceInfo()
            {
                IsProduct = Context.CourseIsProductCourse,
                MarketingInfo = GetMarketingInfo(Context.CourseId),
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                Authenticated = true
            };

            ViewData["CourseList"] = userCourses;
            ViewData["ProductName"] = Context.Product.Title;

            var userHasMultipleDomains = false;
            if (!Context.IsAnonymous)
            {
                userHasMultipleDomains = (Context.GetRaUserDomains().Count() > 1);
            }

            ViewData["userHasMultipleDomains"] = userHasMultipleDomains;
            ViewData["ProductCourseId"] = Context.ProductCourseId;

            return View(ecomInfo);
        }

        private string GetGettingStartedInfo(string courseId)
        {
            var gettingStarted = ContentActions.GetContent(courseId, "PX_PRODUCT_GETTING_STARTED", false);
            var copy = string.Empty;

            if (gettingStarted != null)
            {
                copy = gettingStarted.Description;
            }

            return copy;
        }

        private String GetMarketingInfo(string courseId)
        {
            var marketingInfo = ContentActions.GetContent(courseId, "PX_PRODUCT_MARKETING", false);
            var copy = string.Empty;

            if (marketingInfo != null)
            {
                copy = marketingInfo.Description;
            }

            return copy;
        }

        private string GetGenericDomainId()
        {
            return DomainActions.GetGenericDomainId();
        }
    }
}