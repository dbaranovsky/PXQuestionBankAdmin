using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Logging;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;

using Bfw.PX.Biz.Direct.Services.RA.CheckUsername;
using Bfw.PX.Biz.Direct.Services.RA.StudentRegistration;
using Bfw.PX.Biz.Direct.Services.RA.EnterActivationCode;


namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Provides an implementation of the IAutoEnrollmentActions.
    /// </summary>
    public class AutoEnrollmentActions : IAutoEnrollmentActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        // <summary>
        /// The IEnrollmentActions implementation to use.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// The IUserActions implementation to use.
        /// </summary>
        protected IUserActions UserActions { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Gets or sets the INoteActions implementation to use.
        /// </summary>
        protected INoteActions NoteActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableAutoEnrollmentActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation.</param>
        /// <param name="userActions">The IUserActions implementation.</param>
        public AutoEnrollmentActions(IBusinessContext ctx, IEnrollmentActions enrollmentActions, IUserActions userActions, INoteActions noteActions, ISessionManager sessionManager)
        {
            EnrollmentActions = enrollmentActions;
            UserActions = userActions;
            Context = ctx;
            SessionManager = sessionManager;
            NoteActions = noteActions;
        }

        #endregion

        #region IAutoEnrollmentActions Members

        /// <summary>
        /// Add users to RA and Agilix and enroll in current course
        /// </summary>
        /// <returns></returns>
        [Obsolete("This method depends on IEnrollmentActions and IUserActions objects.", false)]
        public Boolean CreateEnrollments()  //***LMS - this function creates a bunch of dummy users, it needs to be updated to create valid LMSID users
        {
            var batch = new Batch();
            var cmdCreate = new CreateUsers();
            var cmdUpdate = new UpdateUsers();
            var cmdEnroll = new CreateEnrollment();

            List<Adc.AgilixUser> usersToCreate = new List<Adc.AgilixUser>();
            List<Adc.AgilixUser> usersToUpdate = new List<Adc.AgilixUser>();
            List<Bdc.UserInfo> usersToInitialize = new List<Bdc.UserInfo>();
            List<Adc.Enrollment> usersToEnroll = new List<Adc.Enrollment>();

            using (Context.Tracer.StartTrace("AutoEnrollmentActions.CreateEnrollments"))
            {
                usersToCreate = GenerateUserList();
                
                foreach (Adc.AgilixUser agxUser in usersToCreate)
                {
                    RegisterRA(agxUser);
                    agxUser.Credentials.Password = ConfigurationManager.AppSettings["BrainhoneyDefaultPassword"];

                    var existingUser = UserExist(agxUser);
                    if (existingUser == null)
                    {
                        cmdCreate.Add(agxUser);
                    }
                    else
                    {
                        usersToInitialize.Add(existingUser);
                        if (existingUser.Username != agxUser.UserName)
                        {
                            existingUser.Username = agxUser.UserName;
                        }

                        cmdUpdate.Add(existingUser.ToUserInfo());
                    }

                    var newEnrollment = ToEnrollment(agxUser);
                    cmdEnroll.Add(newEnrollment);
                }

                if (!cmdUpdate.Users.IsNullOrEmpty())
                {
                    batch.Add(cmdUpdate);
                }

                if (!cmdCreate.Users.IsNullOrEmpty())
                {
                    batch.Add(cmdCreate);
                }                

                if (!cmdEnroll.Enrollments.IsNullOrEmpty())
                {
                    batch.Add(cmdEnroll);
                }

                if (!batch.Commands.IsNullOrEmpty())
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                    if (!cmdCreate.Users.IsNullOrEmpty())
                    {
                        foreach (var agx in cmdCreate.Users)
                        {
                            usersToInitialize.Add(new Bdc.UserInfo()
                            {
                                Id = agx.Id,
                                FirstName = agx.FirstName,
                                LastName = agx.LastName
                            });
                        }
                    }
                }

                if (!usersToInitialize.IsNullOrEmpty())
                {
                    foreach (var initUser in usersToInitialize)
                    {
                        NoteActions.InitializeUser(initUser, Context.CourseId, Bdc.UserType.Student);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Parses the XML registration response from the RA service.
        /// </summary>
        /// <param name="xmlResponse">The XML response to parse.</param>
        private Bdc.StudentRegistrationResult ParseRegistration(string xmlResponse)
        {
            Bdc.StudentRegistrationResult result = new Bdc.StudentRegistrationResult();

            using (Context.Tracer.StartTrace("AutoEnrollmentActions.ParseRegistration(xmlResponse)"))
            {
                var xDoc = new XmlDocument();

                xDoc.LoadXml(xmlResponse); //Created the Parent Node.

                XmlNode rootNode = xDoc.SelectSingleNode("udtUserProfile");
                if (rootNode == null) return result;

                result.UserId = xDoc.SelectSingleNode("udtUserProfile/iUserID").NodeValue("0");
                result.Username = xDoc.SelectSingleNode("udtUserProfile/sUserName").NodeValue("");
                result.Password = xDoc.SelectSingleNode("udtUserProfile/sPassword").NodeValue("");
                result.Firstname = xDoc.SelectSingleNode("udtUserProfile/sFirstName").NodeValue("");
                result.Lastname = xDoc.SelectSingleNode("udtUserProfile/sLastName").NodeValue("");
                result.PasswordHint = xDoc.SelectSingleNode("udtUserProfile/sPasswordHint").NodeValue("");
                result.MailPreference = xDoc.SelectSingleNode("udtUserProfile/sMailPreference").NodeValue("");
                result.OptInEmail = xDoc.SelectSingleNode("udtUserProfile/sOptInEmail").NodeValue("");
                result.InstructorEmail = xDoc.SelectSingleNode("udtUserProfile/sInstructorEmail").NodeValue("");
                result.LevelOfAccess = xDoc.SelectSingleNode("udtUserProfile/iLevelOfAccess").NodeValue("");
            }

            return result;
        }

        /// <summary>
        /// Creates a list of student AgilixUser objects based off of the current instructor info.
        /// </summary>
        /// <returns>A list of students.</returns>
        private List<Adc.AgilixUser> GenerateUserList()
        {
            List<Adc.AgilixUser> userList = new List<Adc.AgilixUser>();
            String instructorName = Context.CurrentUser.FirstName;
            String instructorUserName = Context.CurrentUser.Username;

            using (Context.Tracer.StartTrace("AutoEnrollmentActions.GenerateUserList()"))
            {

                for (int i = 1; i < 6; i++)
                {
                    String instructorEmail = Context.CurrentUser.Email;
                    String studentFirstName = String.Format("student{0}{1}", i, instructorName);
                    String studentLastName = "Demo";
                    String studentUserName = String.Format("student{0}-{1}", i, instructorUserName);
                    String studentEmail = String.Format("student{0}-{1}", i, instructorUserName);
                    String studentPW = "123456";

                    var user = new Adc.AgilixUser()
                    {
                        Reference = "",
                        UserName = "",
                        Email = studentEmail,
                        FirstName = studentFirstName,
                        LastName = studentLastName,
                        Domain = new Adc.Domain()
                        {
                            Id = Context.Domain.Id,
                            Name = Context.Domain.Name
                        },
                        Credentials = new Adc.Credentials()
                        {
                            Username = studentUserName,
                            Password = studentPW,
                            UserSpace = Context.Domain.Userspace,
                            PasswordQuestion = string.Empty,
                            PasswordAnswer = string.Empty
                        }
                    };

                    userList.Add(user);
                }
            }

            return userList;
        }

        /// <summary>
        /// Registers the new user in RA and assigns the reference/external ID. 
        /// </summary>
        /// <param name="agxUser">The user to register.</param>
        /// <returns>An update AgilixUser object with the newly assigned reference ID.</returns>
        private Adc.AgilixUser RegisterRA(Adc.AgilixUser agxUser)
        {
            RACheckUsername svcCheck = new RACheckUsername();
            String ERROR_MSG = string.Empty;
            String studentActivationCode = ConfigurationManager.AppSettings["RAStudentActivationCode"];

            using (Context.Tracer.StartTrace("AutoEnrollmentActions.RegisterRA(agxUser)"))
            {

                int raId = svcCheck.CheckUsername(agxUser.Email, out ERROR_MSG);

                // If user doesnt exist register in RA.
                if (raId == 0)
                {
                    RAStudentRegistration svcReg = new RAStudentRegistration();
                    String regResult = svcReg.StudentRegistration(
                        agxUser.Email,
                        agxUser.Email,
                        "Student",
                        "DemoPxEnroll",
                        agxUser.Credentials.Password,
                        agxUser.Credentials.Password,
                        "Demo",
                        string.Empty,
                        string.Empty,
                        out ERROR_MSG);

                    if (String.IsNullOrEmpty(ERROR_MSG))
                    {
                        var svcResult = ParseRegistration(regResult);
                        agxUser.Reference = svcResult.UserId;
                    }
                    else
                    {
                        Context.Logger.Log(string.Format("Error during RA User registration: {0}", ERROR_MSG), Bfw.Common.Logging.LogSeverity.Debug);
                    }
                }
                else
                {
                    agxUser.Reference = raId.ToString();
                }

                if (String.IsNullOrEmpty(ERROR_MSG))
                {
                    RAEnterActivationCode svcActivate = new RAEnterActivationCode();
                    String activateResult = svcActivate.EnterActivationCode(studentActivationCode, Convert.ToInt32(agxUser.Reference), Convert.ToInt32(Context.SiteID), out ERROR_MSG);
                    if (!String.IsNullOrEmpty(ERROR_MSG))
                    {
                        Context.Logger.Log(string.Format("Error during RA User activation: {0}", ERROR_MSG), Bfw.Common.Logging.LogSeverity.Debug);
                    }
                }
            }

            return agxUser;
        }

        /// <summary>
        /// Checks if the specified user exist in the Agilix DLAP database.
        /// </summary>
        /// <param name="agxUser">The user to check for.</param>
        /// <returns>True if user already exist, false otherwise.</returns>
        private Bdc.UserInfo UserExist(Adc.AgilixUser agxUser)
        {
            Bdc.UserInfo user = null;

            using (Context.Tracer.StartTrace("AutoEnrollmentActions.UserExist(agxUser)"))
            {
                List<Bdc.UserInfo> users = null;
                users = UserActions.ListUsersLike(new Bdc.UserInfo() { Username = agxUser.UserName, DomainId = agxUser.Domain.Id });

                if (!users.IsNullOrEmpty())
                {
                    user = users.First();
                }
            }

            return user;
        }

        /// <summary>
        /// Helper method to convert an AgilixUser object to an Enrollment object.
        /// </summary>
        /// <param name="agxUser">The AgilixUser object to convert.</param>
        /// <returns>The new enrollment object.</returns>
        private Adc.Enrollment ToEnrollment(Adc.AgilixUser agxUser)
        {
            DateTime sDate = DateTime.Now.GetCourseDateTime(Context);
            DateTime eDate = DateTime.Now.GetCourseDateTime(Context).AddYears(1);
            String studentPermissionFlags = ConfigurationManager.AppSettings["StudentPermissionFlags"]; ;

            String extendedId = String.Format("{0}/{1}", agxUser.Domain.Id, agxUser.Reference);

            var enrollment = new Adc.Enrollment()
                {
                    Domain = new Adc.Domain() { Id = agxUser.Domain.Id },
                    User = new Adc.AgilixUser() { Id = extendedId },
                    Course = new Adc.Course() { Id = Context.EntityId },
                    Flags = (DlapRights)Enum.Parse(typeof(DlapRights), studentPermissionFlags),
                    Status = "1",
                    StartDate = sDate,
                    EndDate = eDate,
                    Reference = string.Empty,
                    Schema = string.Empty
                };

            return enrollment;
        }

        #endregion
    }
}