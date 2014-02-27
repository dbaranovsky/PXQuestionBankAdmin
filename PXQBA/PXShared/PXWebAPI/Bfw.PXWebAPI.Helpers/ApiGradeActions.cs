using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Adc = Bfw.Agilix.DataContracts;
using GetGrades = Bfw.Agilix.Commands.GetGrades;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiGradeActions
	/// </summary>
	public interface IApiGradeActions
	{
		/// <summary>
		/// GetGradesByEnrollment
		/// </summary>
		/// <param name="studentEnrollmentId"> </param>
		/// <returns></returns>
		List<Grade> GetPxStudentGradesByEnrollment(string studentEnrollmentId);


		/// <summary>
		/// GetGradesByEnrollment
		/// </summary>
		/// <param name="enrollmentId"></param>
		/// <param name="itemId"></param>
		/// <param name="enrollment"></param>
		/// <returns></returns>
		IEnumerable<Agilix.DataContracts.Grade> GetGradesByEnrollment(string enrollmentId, List<string> itemId, out Agilix.DataContracts.Enrollment enrollment);

		/// <summary>
		/// PxGradeActions
		/// </summary>
		IGradeActions PxGradeActions { get; set; }
	}

	/// <summary>
	/// ApiGradeActions
	/// </summary>
	public class ApiGradeActions : IApiGradeActions
	{
		#region Properties

		protected ISessionManager SessionManager { get; set; }
        
		protected IBusinessContext Context { get; set; }

        protected ICacheProvider CacheProvider { get; set; }
		public IGradeActions PxGradeActions { get; set; }

		protected IContentActions PxContentActions { get; set; }
		protected ICourseActions PxCourseActions { get; set; }
		protected INoteActions PxNoteActions { get; set; }
		protected IDocumentConverter PxDocumentConverter { get; set; }
		protected IDomainActions PxDomainActions { get; set; }
		protected IEnrollmentActions PxEnrollmentActions { get; set; }
		protected IUserActions PxUserActions { get; set; }
		protected IPxGradeBookActions PxGradeBookActions { get; set; }
        protected IItemQueryActions PXItemQueryActions { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiGradeActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
		public ApiGradeActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, ICacheProvider cacheProvider)
		{
			SessionManager = sessionManager;
			Context = context;
		    CacheProvider = cacheProvider;

			PxDocumentConverter = new AsposeDocumentConverter();
            var databaseManager = new Bfw.Common.Database.DatabaseManager();
			PxContentActions = new ContentActions(context, sessionManager, PxDocumentConverter, databaseManager, PXItemQueryActions);
            PxDomainActions = new DomainActions(context, sessionManager, cacheProvider);
			PxUserActions = new UserActions(context, sessionManager, PxContentActions, PxDomainActions);
			PxEnrollmentActions = new EnrollmentActions(context, sessionManager, PxNoteActions, PxUserActions, PxContentActions);
			PxGradeActions = new GradeActions(context, sessionManager, PxDocumentConverter, PxContentActions, PxEnrollmentActions);
			PxGradeBookActions = new PxGradeBookActions(context, PxEnrollmentActions, PxUserActions, sessionManager,
														PxContentActions);

		}
	    

	    #endregion

		/// <summary>
		/// GetGradesByEnrollment
		/// </summary>
		/// <param name="enrollmentId"></param>
		/// <param name="itemId"></param>
		/// <param name="enrollment"></param>
		/// <returns></returns>
		public IEnumerable<Adc.Grade> GetGradesByEnrollment(string enrollmentId, List<string> itemId, out Adc.Enrollment enrollment)
		{
			var results = new List<Adc.Grade>();
			var cmd = new GetGrades
						  {

							  SearchParameters = new Adc.GradeSearch
													 {
														 EnrollmentId = enrollmentId,
														 ItemIds = itemId
													 }

						  };

			SessionManager.CurrentSession.Execute(cmd);
			enrollment = cmd.Enrollments.FirstOrDefault();
			foreach (var item in cmd.Enrollments)
			{
                results.AddRange(item.ItemGrades);
			}
			return results;
		}

		/// <summary>
		/// GetGradesByEnrollment
		/// </summary>
		/// <param name="studentEnrollmentId"> </param>
		/// <returns></returns>
		public List<Bfw.PX.PXPub.Models.Grade> GetPxStudentGradesByEnrollment(string studentEnrollmentId)
		{
			//var grades = PxGradeActions.GetGradesByEnrollment(studentEnrollmentId, itemIdList);

			var bizGrades = PxGradeBookActions.GetGradesByEnrollment(studentEnrollmentId, true);
			var grades = bizGrades.Map(delegate(PX.Biz.DataContracts.Grade bizGrade)
			{
				var grade = new Grade()
				{
					EnrollmentId = studentEnrollmentId,
					ItemId = bizGrade.ItemId,
					Possible = bizGrade.Possible,
					Achieved = bizGrade.Achieved,
					SubmittedDate = bizGrade.SubmittedDate,
					ScoredDate = bizGrade.ScoredDate,
					ScoredVersion = bizGrade.ScoredVersion,
					ItemTitle = bizGrade.ItemName,
					GradeRule = (GradeRule)bizGrade.Rule,
					AttemptLimit = bizGrade.GradedItem.AssessmentSettings.AttemptLimit,
					AttemptList = bizGrades.Any() ? PxGradeBookActions.GetAttemptsByStudent(bizGrade.ItemId, studentEnrollmentId).Map(sl => new Attempt() { Count = sl.AttemptNo, RawPossible = sl.RawPossible, RawAchieved = sl.RawAchieved, Submitted = sl.SubmittedDate.Value }).ToList() : null
				};
				return grade;
			}).ToList();

			return grades.ToList();
		}


	}
}
