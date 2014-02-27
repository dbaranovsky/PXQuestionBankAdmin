using System.Linq;
using System.Web.Mvc;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Models;
using ADC = Bfw.Agilix.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// HomeController
	/// </summary>
	public class HomeController : Controller
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// HomeController
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		public HomeController(ISessionManager sessionManager, BizSC.IBusinessContext context)
		{
			SessionManager = sessionManager;
			Context = context;
		}

		/// <summary>
		/// Index
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			var title = string.Empty;

			var cmd = new GetItems
			{
				SearchParameters = new ADC.ItemSearch
				{
					EntityId = "57704",
					ItemId = "MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407"
				}
			};

			SessionManager.CurrentSession.Execute(cmd);

			if (!cmd.Items.IsNullOrEmpty())
			{
				title = cmd.Items.First().Title;
			}

			ViewData["itemTitle"] = title;

			return View();
		}

		/// <summary>
		/// GetItem
		/// </summary>
		/// <returns></returns>
		public ActionResult GetItem()
		{
			var model = new Item
			{
				id = "n/a",
				title = "n/a"
			};

			var cmd = new GetItems
			{
				SearchParameters = new ADC.ItemSearch
				{
					EntityId = "57704",
					ItemId = "MODULE_bsi__9922C838__B084__432D__A72E__D073B21B9407"
				}
			};

			SessionManager.CurrentSession.Execute(cmd);

			if (!cmd.Items.IsNullOrEmpty())
			{
				model.id = cmd.Items.First().Id;
				model.title = cmd.Items.First().Title;
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
