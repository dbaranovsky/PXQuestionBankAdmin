using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.Common;

namespace Bfw.PXWebAPI.Helpers
{
    public class EnrollmentActions
    {
        #region Properties

        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public EnrollmentActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }
        #endregion

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="enrollmentId">ID of the enrollment.</param>
        /// <returns>An enrollment record.</returns>
        public Adc.Enrollment GetEnrollment(string enrollmentId)
        {
            Adc.Enrollment enrolled = null;
            var cmd = new GetEntityEnrollmentList()
            {
                SearchParameters = new Adc.EntitySearch()
                {
                    EnrollmentId = enrollmentId
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            enrolled = cmd.Enrollments.First();
            return enrolled;
        }

        /// <summary>
        /// Retruns a list of all domains that a user can enroll in.
        /// </summary>
        /// <param name="userReferenceId">Reference ID of the user that wants to be enrolled.</param>
        /// <param name="parentDomainId">ID of the parent domain to restring domain list to.</param>
        /// <param name="forceIfMember">If populated with a list of domain ids, then these domains will be in the result if the user is a member of the domain.</param>
        /// <returns>Distinct list of domains that the user may enroll in.</returns>
        public IEnumerable<Adc.Domain> GetEnrollableDomains(string userReferenceId, string parentDomainId, IEnumerable<string> forceIfMember = null)
        {
            var results = new List<Adc.Domain>();

            //first get the list of all child domains.
            var getDomains = new GetDomainList()
            {
                SearchParameters = new Adc.Domain()
                {
                    Id = parentDomainId
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(getDomains);

            if (!getDomains.Domains.IsNullOrEmpty())
            {
                //remove the "parent domain" as DLAP for some reason always returns it...
                results = getDomains.Domains.Filter(d => d.Id != parentDomainId).ToList();
            }

            if (!forceIfMember.IsNullOrEmpty())
            {
                //since we have a forceIfMember list we need to make sure we add them if necessary...
                var userActions = new UserActions(SessionManager);
                var users = userActions.GetUsers(rauserid: userReferenceId);
                var domainActions = new DomainActions(SessionManager);
                var domains = users.Map(u => domainActions.GetDomainById(u.Domain.Id)).Distinct((a, b) => a.Id == b.Id);
                foreach (var domain in domains)
                {
                    if (forceIfMember.Contains(domain.Id) && !results.Any(x => x.Id == domain.Id))
                    {
                        results.Add(domain);
                    }
                }
            }

            results = results.Distinct((a, b) => a.Id == b.Id).OrderBy(d => d.Name).ToList();

            return results;
        }

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns>An enrollment record for the specified user and entity ID.</returns>
        public Adc.Enrollment GetEnrollment(string userId, string entityId)
        {
            Adc.Enrollment enrolled = null;
            var entityEnrollments = GetEntityEnrollmentsAsAdmin(entityId).Filter(e => e.User.Id == userId);
            if (!entityEnrollments.IsNullOrEmpty())
            {
                enrolled = entityEnrollments.First();

            }
            return enrolled;
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
        /// <param name="disallowduplicates">This will disallow to create duplicate enrollment</param>
        /// <returns></returns>
        public List<Adc.Enrollment> CreateEnrollments(string domainId, string userId, string entityId, string flags, string status, DateTime startDate, DateTime endDate, string reference, string schema, bool disallowduplicates = false)
        {
            var cmd = new CreateEnrollment();
            cmd.Disallowduplicates = disallowduplicates;
            cmd.Add(new Adc.Enrollment()
                        {
                            Domain = new Adc.Domain() { Id = domainId },
                            User = new Adc.AgilixUser() { Id = userId },
                            Course = new Adc.Course() { Id = entityId },
                            Flags = (DlapRights)Enum.Parse(typeof(DlapRights), flags),
                            Status = status,
                            StartDate = startDate,
                            EndDate = endDate,
                            Reference = reference,
                            Schema = schema
                        });

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Enrollments;
        }

        /// <summary>
        /// Gets a user enrollment ID.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment ID.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns></returns>
        public String GetUserEnrollmentId(string userId, string entityId, bool allStatus = false)
        {
            string agxEnrollmentId = string.Empty;
            Adc.Enrollment enrolled = null;
            Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId, UserId = userId };
            var cmd = new GetUserEnrollmentList()
                          {
                              AllStatus = allStatus,
                              SearchParameters = searchParameters
                          };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            enrolled = cmd.Enrollments.FirstOrDefault();
            agxEnrollmentId = (enrolled != null) ? enrolled.Id : string.Empty;
            return agxEnrollmentId;
        }

        /// <summary>
        /// Gets the list of enrollments in the specified entity filtered by user type.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>A list of enrollments.</returns>
        public IEnumerable<Adc.Enrollment> GetEntityEnrollments(string entityId, UserType userType)
        {
            IEnumerable<Adc.Enrollment> result = null;

            var role = new Dictionary<UserType, string>
                           {
                               {UserType.Student, "Participate"},
                               {UserType.Instructor, "SubmitFinalGrade"},
                               {UserType.All, ""}
                           }[userType];

            result = GetEntityEnrollmentsAsAdmin(entityId);
            if (result != null)
            {
                result = result.Filter(e => e.Flags.ToString().Contains(role));
            }

            return result;
        }

        /// <summary>
        /// Gets the list of users enrolled in the specified entity via an Admin login.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns>List of Enrollments.</returns>
        public List<Adc.Enrollment> GetEntityEnrollmentsAsAdmin(string entityId)
        {
            List<Adc.Enrollment> entityEnrollmentList = null;

            Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId };

            var cmd = new GetEntityEnrollmentList()
                          {
                              SearchParameters = searchParameters
                          };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Enrollments.IsNullOrEmpty())
            {
                entityEnrollmentList = cmd.Enrollments.ToList();
            }

            return entityEnrollmentList;
        }

        /// <summary>
        /// Updates the enrollment of the user with new attributes
        /// </summary>
        /// <param name="enrollment">enrollment object that will be updated</param>
        /// <returns>Success or failure as true / false</returns>
        public bool UpdateEnrollment(Enrollment enrollment)
        {
            var enrollments = new List<Adc.Enrollment>();
            Adc.AgilixUser user = null;
            if (enrollment.User != null)
            {
                user = new Adc.AgilixUser();
                user.Id = enrollment.User.Id;
                user.FirstName = enrollment.User.FirstName;
                user.LastName = enrollment.User.LastName;
                user.Reference = enrollment.User.ReferenceId;
                user.Email = enrollment.User.Email;
            }

            enrollments.Add(new Adc.Enrollment()
                                {
                                    Id = enrollment.Id,
                                    User = user,
                                    CourseId = enrollment.CourseId,
                                    Status = enrollment.Status,
                                    EndDate = enrollment.EndDate.HasValue ? enrollment.EndDate.Value : DateTime.MinValue,
                                    StartDate =
                                        enrollment.StartDate.HasValue ? enrollment.StartDate.Value : DateTime.MinValue,
                                    Reference = enrollment.Reference
                                });

            var cmd = new UpdateEnrollments();
            cmd.Enrollments = enrollments;

            try
            {
                cmd.ParseResponse(SessionManager.CurrentSession.Send(cmd.ToRequest(), asAdmin: true));
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
