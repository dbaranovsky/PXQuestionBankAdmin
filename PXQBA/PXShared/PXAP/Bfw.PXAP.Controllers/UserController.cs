using System;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.JqGridHelper;
using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.Models.Domain;
using Bfw.PXAP.Models.View;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
	/// <summary>
	/// Controler used to render the user actions 
	/// </summary>
	public class UserController : ApplicationController
	{
		private const int pageSize = 5;
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository, IApplicationContext context)
			: base(context)
		{
			_userRepository = userRepository;
		}

		/// <summary>
		/// User list view Action
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		[HttpGet]
		public ViewResult Users(int page = 1)
		{
			IMenuService menuService = new MenuService();

			SettingsModel settingsModel = new SettingsModel();
			settingsModel.MenuModel = menuService.GetSettingsMenu();

			return View(settingsModel);
		}

		/// <summary>
		/// Action that gets the list of Users in JSON format. 
		/// Params passed from jqgrid
		/// </summary>
		/// <param name="page">Current Page</param>
		/// <param name="rows">No of rows per page</param>
		/// <param name="sidx">Sort column</param>
		/// <param name="sord">Sort direction</param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult _UsersPartial(int page = 1, int rows = 20, string sidx = "", string sord = "")
		{
			var users = from log in _userRepository.GetUsers()
						select new
						{
							UserName = log.UserName,
							Name = log.Name,
							Email = log.Email,
							Actions = string.Format("<a href=\"javascript:PxApUser.EditUser('{0}')\" title=\"Edit\">Edit</a> <a href=\"javascript:PxApUser.DeleteUser('{0}')\" title=\"Delete\">Delete</a>", log.UserName)
						};

			var result = users.ToJqGridData(page, rows, string.Format("{0} {1}", sidx, sord), "",
				new[] {
                    "UserName", "Name", "Email", "Actions"
                });

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Action to load the edit user view
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		[HttpGet]
		public PartialViewResult Edit(string username)
		{
			var user = _userRepository.GetUser(username);

			return PartialView(new EditUserViewModel { Name = user.Name, UserName = user.UserName, Email = user.Email, UpdatePassword = false });
		}

		/// <summary>
		/// Edit user page save action
		/// </summary>
		/// <param name="editUser"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Edit(EditUserViewModel editUser)
		{
			try
			{
				var user = new User { Name = editUser.Name, Email = editUser.Email, UserName = editUser.UserName };

				if (editUser.UpdatePassword)
				{
					user.CurrentPassword = editUser.CurrentPassword;
					user.NewPassword = editUser.NewPassword;
				}
				_userRepository.SaveUser(user);

				return Content(bool.TrueString);
			}
			catch (Exception ex)
			{
				editUser.UpdatePassword = false;
				ViewData.ModelState.AddModelError("UserName", ex.Message);
				return PartialView(editUser);
			}
		}

		/// <summary>
		/// Action to load the create user view
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public PartialViewResult Create()
		{
			return PartialView();
		}

		/// <summary>
		/// Create user save action
		/// </summary>
		/// <param name="createUser"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Create(CreateUserViewModel createUser)
		{
			try
			{
				var user = new User { Name = createUser.Name, Email = createUser.Email, UserName = createUser.UserName, NewPassword = createUser.Password };

				_userRepository.SaveUser(user, true);

				return Content(bool.TrueString);
			}
			catch (Exception ex)
			{
				ViewData.ModelState.AddModelError("UserName", ex.Message);
				return PartialView(createUser);
			}
		}

		/// <summary>
		/// Action to create the delete user view
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		[HttpGet]
		public PartialViewResult Delete(string username)
		{
			User user = _userRepository.GetUser(username);

			return PartialView(user);
		}

		/// <summary>
		/// Delete user confirm action
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult delete(User user)
		{
			_userRepository.DeleteUser(user.UserName);

			return Content(bool.TrueString);
		}
	}
}