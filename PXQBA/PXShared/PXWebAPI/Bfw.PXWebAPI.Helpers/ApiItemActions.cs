using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiItemActions
	/// </summary>
	public interface IApiItemActions
	{
		/// <summary>
		/// GetItems
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="itemId"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		List<Item> GetItems(string courseId, string itemId, string query = "");
	}


	/// <summary>
	/// ApiItemActions
	/// </summary>
	public class ApiItemActions : IApiItemActions
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// ApiItemActions
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		public ApiItemActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context)
		{
			SessionManager = sessionManager;
			Context = context;
		}


		/// <summary>
		/// GetItems
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="itemId"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public List<Item> GetItems(string courseId, string itemId, string query = "")
		{
			var cmd = new Agilix.Commands.GetItems
			{
				SearchParameters = new ItemSearch
				{
					EntityId = courseId,
					ItemId = itemId,
					Query = query
				}
			};

			SessionManager.CurrentSession.Execute(cmd);

			return !cmd.Items.IsNullOrEmpty() ? cmd.Items : null;
		}

	}
}
