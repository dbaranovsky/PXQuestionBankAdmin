using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Models;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiEnrollmentActions
	/// </summary>
	public interface IApiEnrollmentActions
	{
		/// <summary>
		/// CreateEnrollments
		/// </summary>
		/// <param name="domainId"></param>
		/// <param name="userId"></param>
		/// <param name="entityId"></param>
		/// <param name="flags"></param>
		/// <param name="status"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="reference"></param>
		/// <param name="schema"></param>
		/// <param name="disallowduplicates"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Enrollment> CreateEnrollments(string domainId, string userId, string entityId, string flags, string status, DateTime startDate, DateTime endDate, string reference, string schema, bool disallowduplicates = false);

		/// <summary>
		/// GetEnrollableDomains
		/// </summary>
		/// <param name="userReferenceId"></param>
		/// <param name="parentDomainId"></param>
		/// <param name="forceIfMember"></param>
		/// <returns></returns>
		IEnumerable<Agilix.DataContracts.Domain> GetEnrollableDomains(string userReferenceId, string parentDomainId, IEnumerable<string> forceIfMember = null);

		/// <summary>
		/// GetEnrollment
		/// </summary>
		/// <param name="enrollmentId"></param>
		/// <returns></returns>
		Agilix.DataContracts.Enrollment GetEnrollment(string enrollmentId);

		/// <summary>
		/// GetEnrollment
		/// </summary>
		/// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <param name="allStatus"></param>
		/// <returns></returns>
        Agilix.DataContracts.Enrollment GetEnrollment(string userId, string entityId, bool allStatus = false);

	    /// <summary>
	    /// GetEnrollment
	    /// </summary>
	    /// <param name="userId"></param>
	    /// <param name="entityId"></param>
	    /// <param name="allStatus"></param>
	    /// <returns></returns>
	    IEnumerable<Adc.Enrollment> GetEnrollments(string userId, string entityId, bool allStatus = false);

		/// <summary>
		/// GetEntityEnrollments
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="userType"></param>
		/// <returns></returns>
		IEnumerable<Agilix.DataContracts.Enrollment> GetEntityEnrollments(string entityId, UserType userType);

		/// <summary>
		/// GetEntityEnrollmentsAsAdmin
		/// </summary>
        /// <param name="entityId"></param>
        /// <param name="allStatus"></param>
		/// <returns></returns>
        List<Agilix.DataContracts.Enrollment> GetEntityEnrollmentsAsAdmin(string entityId, bool allStatus = false);

		/// <summary>
		/// GetUserEnrollmentId
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="entityId"></param>
		/// <param name="allStatus"></param>
		/// <returns></returns>
		string GetUserEnrollmentId(string userId, string entityId, bool allStatus = false);

		/// <summary>
		/// UpdateEnrollment
		/// </summary>
		/// <param name="enrollment"></param>
		/// <returns></returns>
		bool UpdateEnrollment(Enrollment enrollment);

	    /// <summary>
	    /// DropEnrollment
	    /// </summary>
	    /// <param name="enrollment"></param>
	    /// <returns></returns>
	    bool DropEnrollment(Enrollment enrollment);
	}

	/// <summary>
	/// ApiEnrollmentActions
	/// </summary>
	public class ApiEnrollmentActions : IApiEnrollmentActions
	{
		#region Properties

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected IApiUserActions ApiUserActions { get; set; }
		protected IApiDomainActions ApiDomainActions { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiEnrollmentActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
        /// <param name="userActions"> </param>
        /// <param name="context"> </param>
		public ApiEnrollmentActions(ISessionManager sessionManager, IUserActions userActions, PX.Biz.ServiceContracts.IBusinessContext context)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiDomainActions = new ApiDomainActions(sessionManager, context);
			ApiUserActions = new ApiUserActions(sessionManager, userActions, context);
		}
		#endregion

		/// <summary>
		/// Gets an enrollment record.
		/// </summary>
		/// <param name="enrollmentId">ID of the enrollment.</param>
		/// <returns>An enrollment record.</returns>
		public Adc.Enrollment GetEnrollment(string enrollmentId)
		{
			var cmd = new GetEntityEnrollmentList
						{
							SearchParameters = new Adc.EntitySearch
												{
													EnrollmentId = enrollmentId
												}
						};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			Agilix.DataContracts.Enrollment enrolled = cmd.Enrollments.First();
			return enrolled;
		}

		/// <summary>
		/// Returns a list of all domains that a user can enroll in.
		/// </summary>
		/// <param name="userReferenceId">Reference ID of the user that wants to be enrolled.</param>
		/// <param name="parentDomainId">ID of the parent domain to restring domain list to.</param>
		/// <param name="forceIfMember">If populated with a list of domain ids, then these domains will be in the result if the user is a member of the domain.</param>
		/// <returns>Distinct list of domains that the user may enroll in.</returns>
		public IEnumerable<Adc.Domain> GetEnrollableDomains(string userReferenceId, string parentDomainId, IEnumerable<string> forceIfMember = null)
		{
			var results = new List<Adc.Domain>();

			//first get the list of all child domains.
			var getDomains = new GetDomainList
								{
									SearchParameters = new Adc.Domain
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
				//rauserid: userReferenceId
				var userActions = ApiUserActions;
				var users = userActions.GetUsers(userReferenceId);
				var domainActions = ApiDomainActions;
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
        /// <param name="allStatus">AllStatus</param>
		/// <returns>An enrollment record for the specified user and entity ID.</returns>
		public Adc.Enrollment GetEnrollment(string userId, string entityId, bool allStatus = false)
		{
            var entityEnrollments = GetEntityEnrollmentsAsAdmin(entityId, allStatus).Filter(e => e.User.Id == userId);
			return !entityEnrollments.IsNullOrEmpty() ? entityEnrollments.First() : null;
		}

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="allStatus">AllStatus</param>
        /// <returns>An enrollment record for the specified user and entity ID.</returns>
        public IEnumerable<Adc.Enrollment> GetEnrollments(string userId, string entityId, bool allStatus = false)
        {
            var entityEnrollments = GetEntityEnrollmentsAsAdmin(entityId, allStatus).Filter(e => e.User.Id == userId);
            return !entityEnrollments.IsNullOrEmpty() ? entityEnrollments : null;
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
			var cmd = new CreateEnrollment { Disallowduplicates = disallowduplicates };
			cmd.Add(new Adc.Enrollment
						{
							Domain = new Adc.Domain { Id = domainId },
							User = new Adc.AgilixUser { Id = userId },
							Course = new Adc.Course { Id = entityId },
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
		/// <param name="allStatus"> </param>
		/// <returns></returns>
		public String GetUserEnrollmentId(string userId, string entityId, bool allStatus = false)
		{
			var searchParameters = new Adc.EntitySearch { CourseId = entityId, UserId = userId };
			var cmd = new GetUserEnrollmentList
						{
							AllStatus = allStatus,
							SearchParameters = searchParameters
						};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			Agilix.DataContracts.Enrollment enrolled = cmd.Enrollments.FirstOrDefault();
			string agxEnrollmentId = ( enrolled != null ) ? enrolled.Id : string.Empty;
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
			var role = new Dictionary<UserType, string>
                           {
                               {UserType.Student, "Participate"},
                               {UserType.Instructor, "SubmitFinalGrade"},
                               {UserType.All, ""}
                           }[userType];

			IEnumerable<Agilix.DataContracts.Enrollment> result = GetEntityEnrollmentsAsAdmin(entityId);
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
		public List<Adc.Enrollment> GetEntityEnrollmentsAsAdmin(string entityId, bool allStatus = false)
		{
			List<Adc.Enrollment> entityEnrollmentList = null;

			var searchParameters = new Adc.EntitySearch { CourseId = entityId, AllStatus = allStatus};

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
				user = new Adc.AgilixUser
						{
							Id = enrollment.User.Id,
							FirstName = enrollment.User.FirstName,
							LastName = enrollment.User.LastName,
							Reference = enrollment.User.ReferenceId,
							Email = enrollment.User.Email
						};
			}

			enrollments.Add(new Adc.Enrollment
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

			var cmd = new UpdateEnrollments { Enrollments = enrollments };

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

	    /// <summary>
	    /// Drops the enrollment
	    /// </summary>
	    /// <param name="enrollment">enrollment object that will be dropped</param>
	    /// <returns>Success or failure as true / false</returns>
	    public bool DropEnrollment(Enrollment enrollment)
	    {
            var enrollments = new List<Adc.Enrollment>();
            Adc.AgilixUser user = null;
            if (enrollment.User != null)
            {
                user = new Adc.AgilixUser
                {
                    Id = enrollment.User.Id,
                    FirstName = enrollment.User.FirstName,
                    LastName = enrollment.User.LastName,
                    Reference = enrollment.User.ReferenceId,
                    Email = enrollment.User.Email
                };
            }

            enrollments.Add(new Adc.Enrollment
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

            var cmd = new DeleteEnrollments { Enrollments = enrollments };

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
