using System.Web.Mvc;
using Bfw.Common.Collections;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Collections.Generic;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Linq;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;


namespace Bfw.PX.PXPub.Controllers
{

    [PerfTraceFilter]    
    public class NoteSettingController : Controller
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the note actions.
        /// </summary>
        /// <value>
        /// The note actions.
        /// </value>
        protected BizSC.INoteActions NoteActions{ get; set; }

        /// <summary>
        /// A private member for blank space
        /// </summary>
        private const string BLANK_SPACE = " ";

        /// <summary>
        /// A private member for storing Students.
        /// </summary>
        private List<Student> _students = new List<Student>();

        /// <summary>
        /// A private member for storing Users.
        /// </summary>
        private List<UserInfo> _users = new List<UserInfo>();

        /// <summary>
        /// Gets the shared students.
        /// </summary>
        protected List<Student> SharedStudents
        {
            get
            {
                if (_students.IsNullOrEmpty())
                {
                    var sharedInstructors = (from c in EnrollmentActions.GetEntityEnrollments(Context.CourseId, UserType.Instructor) select c.User).ToList();
                    _students = sharedInstructors.Map(b => b.ToStudent(UserType.Instructor)).Where(i=> i.Id != Context.CurrentUser.Id && i.FirstName != "pxmigration" ).ToList();
                    var biz = NoteActions.GetAllSharedNotes(Context.CurrentUser.Id,Context.CourseId);

                    if (!biz.IsNullOrEmpty())
                    {
                        _students.AddRange(biz.Map(b => b.ToStudent()).ToList());
                    }
                }

                return _students;
            }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        protected List<UserInfo> Users
        {
            get
            {
                if (_users.IsNullOrEmpty())
                {
                    _users = (from c in EnrollmentActions.GetEntityEnrollments(Context.CourseId, UserType.Student) select c.User).ToList();
                    
                }

                return _users;
            }
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteSettingController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        /// <param name="noteActions">The note actions.</param>
        public NoteSettingController(BizSC.IBusinessContext context, BizSC.IEnrollmentActions enrollmentActions, BizSC.INoteActions noteActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            NoteActions = noteActions;
        }

        #region Action Methods


        /// <summary>
        /// Returns the Index View for this controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!Context.CourseIsProductCourse)
            {
                return View(SharedStudents);
            }
            else
            {
                return View();
            }            
        }

        /// <summary>
        /// Shares Notes filtered by a Student's name.
        /// </summary>
        /// <param name="studentFullName">Full name of the student.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShareNotes(string studentFullName)
        {
            var biz = (from c in Users where c.FormattedName.ToLower().Equals(studentFullName.ToLower()) select c).FirstOrDefault();

            if (null != biz)
            {   
                Student sharedStudent = biz.ToStudent(UserType.Student);
                ShareNoteResult sharedNote = new ShareNoteResult
                                        {
                                             CourseId = Context.CourseId,
                                             FirstNameSharer = Context.CurrentUser.FirstName,
                                             LastNameSharer = Context.CurrentUser.LastName,
                                             FirstNameSharee = sharedStudent.FirstName,
                                             LastNameSharee = sharedStudent.LastName,
                                             SharedStudentId = sharedStudent.Id,
                                             StudentId = Context.CurrentUser.Id                                            

                                        };
                NoteActions.ShareNotes(sharedNote);
            }

            ViewData.Model = SharedStudents;
            return View("ShareList");
        }

        /// <summary>
        /// Stops the sharing a note.
        /// </summary>
        /// <param name="stopSharingToId">The stop sharing to id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StopSharing(string stopSharingToId)
        {
            ShareNoteResult sharedNote = new ShareNoteResult
            {
                CourseId = Context.CourseId,
                SharedStudentId = stopSharingToId,
                StudentId = Context.CurrentUser.Id
            };

            NoteActions.StopSharing(sharedNote);
            
            ViewData.Model = SharedStudents;
            return View("ShareList");
        }

        /// <summary>
        /// Gets the students.
        /// </summary>
        /// <param name="contactName">Name of the contact.</param>
        /// <param name="maxResults">The max results.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetStudents(string contactName, int maxResults)
        {
            List<Student> students = new List<Student>();  
            if (Users.Count > 0)
            {
                var foundStudents = (from c in Users
                           where c.FormattedName.ToLower().Contains(contactName.ToLower()) 
                           && c.Id != Context.CurrentUser.Id
                           orderby c.FirstName
                           select c).ToList();

               students = foundStudents.Map(b => b.ToStudent(UserType.Student)).ToList();
                           
            }

            return Json(students);       
        }

        public ActionResult StudentsExist(string studentName, string studentId)
        {
            List<Student> students = new List<Student>();
            if (Users.Count > 0)
            {
                var foundStudents = (from c in Users
                                     where c.FormattedName.ToLower().Contains(studentName.ToLower())
                                     && c.Id == studentId
                                     orderby c.FirstName
                                     select c).ToList();

                students = foundStudents.Map(b => b.ToStudent(UserType.Student)).ToList();

            }

            string result = (students.Count == 1).ToString();
            return new ContentResult() { Content = result  };
        }


        #endregion
    }
}
