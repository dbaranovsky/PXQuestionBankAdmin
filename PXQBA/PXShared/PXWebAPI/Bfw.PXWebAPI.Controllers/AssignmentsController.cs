using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Models.Response;


namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	public class AssignmentController : ApiController
	{
		#region Properties

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected IApiAssignmentActions ApiAssignmentActions { get; set; }

		protected IApiCourseActions ApiCourseActions { get; set; }

		protected IApiUserActions ApiUserActions { get; set; }
		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AssignmentController"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
		/// <param name="apiAssignmentActions"> </param>
		/// <param name="apiCourseActions"> </param>
		/// <param name="apiUserActions"> </param>
		public AssignmentController(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context,
																	 Bfw.PXWebAPI.Helpers.IApiAssignmentActions apiAssignmentActions,
																	 Bfw.PXWebAPI.Helpers.IApiCourseActions apiCourseActions,
																	 Bfw.PXWebAPI.Helpers.IApiUserActions apiUserActions)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiAssignmentActions = apiAssignmentActions;
			ApiCourseActions = apiCourseActions;
			ApiUserActions = apiUserActions;
		}
		#endregion

		/// <summary>
		/// Get Assignments
		/// Test URL http://localhost:60078/Assignment/Assignments/98011/ 
		/// + Optional QueryString Parameters: userRole and enrollmentId.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userRole"> </param>
		/// <param name="enrollmentId"> </param>
		/// <returns></returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("Assignments")]
		public PxAssignmentsResponse GetAssignments(string id, string userRole = "0", string enrollmentId = "")
		{
			var response = new PxAssignmentsResponse();

			ApiHelper.InitializeInstructorPxBusinessContext(Context, ApiCourseActions, ApiUserActions, id, userRole);
			if (enrollmentId.Length > 0) Context.EnrollmentId = enrollmentId;
			var assignments = ApiAssignmentActions.GetAssignments(id);
			if (!assignments.IsNullOrEmpty())
			{
				response.results = assignments;
				return response;
			}

			response.error_message = "No results found";
			return response;
		}





	}
}