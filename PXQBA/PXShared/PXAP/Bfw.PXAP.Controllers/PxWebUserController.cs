using System;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.JqGridHelper;
using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;
using PxWebUser;

namespace Bfw.PXAP.Controllers
{
	// Controller for the move content action
	public class PxWebUserController : ApplicationController
	{
		private const int pageSize = 5;
		private readonly IPxWebUserRepository _webUserRightsRepository;

		public PxWebUserController(IPxWebUserRepository webUserRightsRepository, IApplicationContext context)
			: base(context)
		{
			_webUserRightsRepository = webUserRightsRepository;
		}


		public ActionResult Index()
		{
			IMenuService menuService = new MenuService();
			PxWebUserModel pxWebUserModel = new PxWebUserModel { MenuModel = menuService.GetContentMenu() };
			ViewData.Model = pxWebUserModel;
			return View();
		}

		[HttpPost]
		public ActionResult _SaveWebUserRights(string UserId, string CourseId)
		{
			IMenuService menuService = new MenuService();
			PxWebUserModel pxWebUserModel = new PxWebUserModel { MenuModel = menuService.GetContentMenu() };
			ViewData.Model = pxWebUserModel;

			var form = ControllerContext.HttpContext.Request.Form;

			PxWebUserRights rights = new PxWebUserRights(UserId, CourseId);
			rights.AdminTool.None = form["WebUserRights.AdminTool.None"].Contains("true");
			rights.AdminTool.AllowEditSandboxCourse = form["WebUserRights.AdminTool.AllowEditSandboxCourse"].Contains("true");
			rights.AdminTool.AllowPublishCourse = Convert.ToBoolean(form["WebUserRights.AdminTool.AllowPublishCourse"].Contains("true"));
			rights.AdminTool.AllowRevertCourse = Convert.ToBoolean(form["WebUserRights.AdminTool.AllowRevertCourse"].Contains("true"));

			rights.QuestionBank.None = form["WebUserRights.QuestionBank.None"].Contains("true");
			rights.QuestionBank.AllowAddNote = Convert.ToBoolean(form["WebUserRights.QuestionBank.AllowAddNote"].Contains("true"));
			rights.QuestionBank.AllowAddQuestion = Convert.ToBoolean(form["WebUserRights.QuestionBank.AllowAddQuestion"].Contains("true"));
			rights.QuestionBank.AllowEditQuestion = Convert.ToBoolean(form["WebUserRights.QuestionBank.AllowEditQuestion"].Contains("true"));
			rights.QuestionBank.AllowFlagQuestion = Convert.ToBoolean(form["WebUserRights.QuestionBank.AllowFlagQuestion"].Contains("true"));
			rights.QuestionBank.ShowHistory = Convert.ToBoolean(form["WebUserRights.QuestionBank.ShowHistory"].Contains("true"));
			rights.QuestionBank.ShowQuestionBankManager = Convert.ToBoolean(form["WebUserRights.QuestionBank.ShowQuestionBankManager"].Contains("true"));
			rights.QuestionBank.ShowQuestionEditor = Convert.ToBoolean(form["WebUserRights.QuestionBank.ShowQuestionEditor"].Contains("true"));
			rights.QuestionBank.ShowQuestionNotes = Convert.ToBoolean(form["WebUserRights.QuestionBank.ShowQuestionNotes"].Contains("true"));

			rights.Save(enumPxWebRightType.All);

			return Json(rights, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public ActionResult _WebUserRights(string UserId, string Email, string CourseId)
		//public ActionResult _WebUserRights(PxWebUserModel user)
		{
			IMenuService menuService = new MenuService();
			PxWebUserModel pxWebUserModel = new PxWebUserModel { MenuModel = menuService.GetContentMenu() };
			ViewData.Model = pxWebUserModel;

			PxWebUserModel pxWebUser = new PxWebUserModel
										{
											CourseId = CourseId,
											UserId = UserId,
											Email = Email,
											WebUserRights = new PxWebUserRights(UserId, CourseId)
										};

			return PartialView(pxWebUser);
		}


		[HttpPost]
		public ActionResult _SearchResult(PxWebUserModel pxWebUser)
		{
			// Validation logic
			if (!ModelState.IsValidField(pxWebUser.Email)) ModelState.AddModelError("Email", "Valid Email is required.");
			if (!ModelState.IsValidField(pxWebUser.CourseId)) ModelState.AddModelError("CourseId", "Valid CourseId is required.");
			if (!ModelState.IsValid) return Json(ModelState, JsonRequestBehavior.AllowGet);

			IMenuService menuService = new MenuService();
			PxWebUserModel pxWebUserModel = new PxWebUserModel { MenuModel = menuService.GetContentMenu() };
			ViewData.Model = pxWebUserModel;

			IQueryable<PxWebUserModel> users = _webUserRightsRepository.SearchPxWebUsers(pxWebUser);

			return PartialView(users);

		}

		[HttpPost]
		public JsonResult JsonSearchResult(PxWebUserModel pxWebUser, int page = 1, int rows = 20)
		{
			// Validation logic
			if (!ModelState.IsValidField(pxWebUser.Email)) ModelState.AddModelError("Email", "Valid Email is required.");
			if (!ModelState.IsValidField(pxWebUser.CourseId)) ModelState.AddModelError("CourseId", "Valid CourseId is required.");
			if (!ModelState.IsValid) return Json(ModelState, JsonRequestBehavior.AllowGet);

			IMenuService menuService = new MenuService();
			PxWebUserModel pxWebUserModel = new PxWebUserModel { MenuModel = menuService.GetContentMenu() };
			ViewData.Model = pxWebUserModel;

			string actions = string.Format("<a href=\"javascript:PxApUser.EditUser('{0}')\" title=\"Edit\">Edit</a>",
										   pxWebUser.Email);

			IQueryable<PxWebUserModel> users = _webUserRightsRepository.SearchPxWebUsers(pxWebUser, actions);

			var result = users.ToJqGridData(page, rows, "UserId", pxWebUser.Email,
				new[] {
			        "UserId", "Email", "CourseId", "Actions"
			    });

			return Json(result, JsonRequestBehavior.AllowGet);

			//return PartialView(users);

		}

	}
}
