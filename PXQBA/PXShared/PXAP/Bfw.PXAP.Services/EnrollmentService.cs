using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.PXAP.Components;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.Common.Collections;
using System.Configuration;


namespace Bfw.PXAP.Services
{
    public class EnrollmentService : IEnrollmentService
    {

        private ISession Session { get; set; }
        private IApplicationContext Context { get; set; }

        public EnrollmentService(IApplicationContext context)
        {
            Context = context;
        }

        /// <summary>
        /// EnrollStudent dummy students to the course
        /// </summary>
        /// <param name="entityId">CourseId</param>
        /// <param name="studnetCount">StudnetCount</param>
        /// <returns></returns>
        public void EnrollStudent(string entityId, int studentCount, Int64 processId)
        {
            // Confiqure the Service
            ConfigureServiceLocator();
            var sm = EstablishConnection();
            Session = sm.CurrentSession;
            
            // Setup progress bar information
            int percentageDone = 0;
            IProgressService progress = new ProgressService();
            String message = "";
            ProgressModel progModel = new ProgressModel() { ProcessId = processId, Percentage = 0, Status = "Processing" };


            string domainId = string.Empty;
            // Find To Course ID            
            if (!string.IsNullOrEmpty(entityId) || entityId != "0")
            {                
                var cmdCourse = new GetCourse()
                {
                    SearchParameters = new CourseSearch()
                    {
                        CourseId = entityId
                    }
                };
                Session.ExecuteAsAdmin(Context.Environment,cmdCourse);
                Course result = cmdCourse.Courses.First();
                if(result != null)
                    domainId = result.Domain.Id;
            }

            if (!string.IsNullOrEmpty(domainId))
            {
                // Student rights flag
                String studentPermissionFlags = ConfigurationManager.AppSettings["StudentPermissionFlags"];
                
                // Get list of already added dummy users from the same domain
                List<AgilixUser> Users = GetUsers(domainId);

                // Get enrollment list for the course
                List<Enrollment> EnrollmentList = GetEnrollmentList(entityId);

                int currentStudentIndex = 0;
                if (Users != null)
                    currentStudentIndex = Users.Count;
                
                var query =
                    from user in Users
                    where !(from enrollment in EnrollmentList
                            select enrollment.User.Id).Contains(user.Id)
                    select user;

                // Filter out non enrolled user from the user list 
                Users = query.ToList<AgilixUser>();

                int existingUserCount = 0;
                
                if (Users != null)
                    existingUserCount = Users.Count;

                //int usersToAdd = existingUserCount > studentCount ? 0 : existingUserCount - studentCount;

                // Create more users
                for (int x = existingUserCount; x < studentCount; x += 20)
                {
                    percentageDone = Convert.ToInt32(x * 100 / studentCount);
                    progModel.Percentage = percentageDone;
                    progress.AddUpdateProcess(progModel, out message);
                    int count = x + 20 > studentCount ? studentCount - x : 20;
                    Users.AddRange(AddUsers(entityId, domainId, count, currentStudentIndex));
                    currentStudentIndex += 20;
                }

                // Enroll users to the course
                CreateEnrollments(domainId, Users.Take(studentCount).ToList(), entityId, studentPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(1), string.Empty, string.Empty);
            
            }
                            
            //update the progress to 100%
            progModel.Percentage = 100;
            progress.AddUpdateProcess(progModel, out message);
        }

        /// <summary>
        /// Get Users from the DLAP        
        /// </summary>
        /// <param name="domainId">The ID of the domain to get users from.</param>
        /// <returns></returns>
        public List<AgilixUser> GetUsers(string domainId)
        {
            string studentEmail = "DummyStudent{0}@bfwpub.com";
            string reference = "12345678";

            var cmd = new GetUsers();
            cmd.SearchParameters = new UserSearch
            {
                DomainId = domainId,
                Username = string.Format(studentEmail, "*"),
                ExternalId = reference                 
            };

            Session.ExecuteAsAdmin(Context.Environment,cmd);
            return cmd.Users;            
        }

        /// <summary>
        /// Creates new users        
        /// </summary>
        /// <param name="entityId">The ID of the course or section in which to add the user.</param>
        /// <param name="domainId">The ID of the domain to create the user in.</param>
        /// <param name="count">Number of users to create</param>
        /// <param name="previousCount">Previously added dummy user count</param>
        /// <returns></returns>
        public List<AgilixUser> AddUsers(string entityId, string domainId, int count, int previousCount)
        {          
            // User property
            string studentEmail = "DummyStudent{0}@bfwpub.com";
            string password = "123456";
            string pwdQuestion = "who am i?";
            string pwdAnswer = "Dummy Student";
            string reference = "12345678";
            string firstName = "Dummy {0}";
            string lastName = "Student";            

            // Student index
            int index = previousCount;
            var cmd = new CreateUsers();
            for (int x = 0; x < count; x++)
            {
                index = index + 1;
                cmd.Add(new AgilixUser()
                {
                    FirstName = string.Format(firstName, index),
                    LastName = lastName,
                    Email = string.Format(studentEmail, index),
                    Credentials = new Credentials() { Username = string.Format(studentEmail, index), Password = password, PasswordQuestion = pwdQuestion, PasswordAnswer = pwdAnswer },
                    Domain = new Domain() { Id = domainId },
                    Reference = reference
                });
            }
            Session.ExecuteAsAdmin(Context.Environment,cmd);            
            return cmd.Users;
        }


        /// <summary>
        /// Get enrollments      
        /// </summary>
        /// <param name="entityId">The ID of the course or section for enrollments.</param>
        /// <returns></returns>
        public List<Enrollment> GetEnrollmentList(string entityId)
        {
            var cmd = new GetEntityEnrollmentList();

            cmd.SearchParameters = new EntitySearch()
            {
                CourseId = entityId
            };

            Session.ExecuteAsAdmin(Context.Environment,cmd);

            return cmd.Enrollments;
        }

        /// <summary>
        /// Creates the enrollments.
        /// See http://gls.agilix.com/Docs/Command/CreateEnrollments.
        /// </summary>
        /// <param name="domainId">The ID of the domain to create the enrollment in.</param>
        /// <param name="userId">The ID of the user to enroll.</param>
        /// <param name="entityId">The ID of the course or section in which to enroll the user.</param>
        /// <param name="flags">Bitwise OR of RightsFlags to grant to the user.</param>
        /// <param name="status">EnrollmentStatus for the user.</param>
        /// <param name="startDate">Date that the enrollment begins.</param>
        /// <param name="endDate">Date that the enrollment ends.</param>
        /// <param name="reference">Optional field reserved for any data the caller wishes to store. Used to store the RA ID.</param>
        /// <param name="schema">An optional parameter that specifies how to interpret flags in agilix.</param>
        /// <returns></returns>
        public List<Enrollment> CreateEnrollments(string domainId, List<AgilixUser> users, string entityId, string flags, string status, DateTime startDate, DateTime endDate, string reference, string schema)
        {
            var cmd = new CreateEnrollment();
            cmd.Disallowduplicates = true;
            foreach (AgilixUser user in users)
            {                
                if (!string.IsNullOrEmpty(user.Id))
                {
                    cmd.Add(new Enrollment()
                    {
                        Domain = new Domain() { Id = domainId },
                        User = new AgilixUser() { Id = user.Id },
                        Course = new Course() { Id = entityId },
                        Flags = (DlapRights)Enum.Parse(typeof(DlapRights), flags),
                        Status = status,
                        StartDate = startDate,
                        EndDate = endDate,
                        Reference = reference,
                        Schema = schema,
                        
                    });
                }
            }
            Session.ExecuteAsAdmin(Context.Environment,cmd);
            return cmd.Enrollments;
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static ISessionManager EstablishConnection()
        {            
            var username = ConfigurationManager.AppSettings["user"];
            var userid = ConfigurationManager.AppSettings["userid"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, string.Empty);
            return sm;
        }
    }
}
