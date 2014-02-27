using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.ServiceContracts;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PXWebAPI.Helpers
{
	/// <summary>
	/// IApiGradeBookActions
	/// </summary>
	public interface IApiGradeBookActions
	{
		/// <summary>
		/// GetGradeBookWeights
		/// </summary>
		/// <param name="courseid"></param>
		/// <returns></returns>
		Agilix.DataContracts.GradeBookWeights GetGradeBookWeights(string courseid);
	}

	/// <summary>
	/// ApiGradeBookActions
	/// </summary>
	public class ApiGradeBookActions : IApiGradeBookActions
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// ApiGradeBookActions
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		public ApiGradeBookActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context)
		{
			SessionManager = sessionManager;
			Context = context;
		}

		/// <summary>
		/// GetGradeBookWeights
		/// </summary>
		/// <param name="courseid"></param>
		/// <returns></returns>
		public Adc.GradeBookWeights GetGradeBookWeights(string courseid)
		{

			var cmd = new GetGradeBookWeights
			{
				SearchParameters = new Adc.GradeBookWeightSearch
				{
					EntityId = courseid
				}
			};

			SessionManager.CurrentSession.Execute(cmd);


			return cmd.GradeBookWeights;
		}

	}
}
