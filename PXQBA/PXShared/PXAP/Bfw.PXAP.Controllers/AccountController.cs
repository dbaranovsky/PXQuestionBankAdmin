
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Profile;
using Bfw.Common.JqGridHelper;

using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;



namespace Bfw.PXAP.Controllers
{
    public class AccountController : ApplicationController//Controller
    {

        public AccountController(IApplicationContext context)
            : base(context)
        {                        
        }

        //
        // GET: /Account/LogOn
        [HttpGet]
        public ActionResult LogOn()
        {            
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            int envId = 0;
            int.TryParse(Request["Environment"], out envId);
            
            LayoutModel layoutModel = this.ViewBag.LayoutModel as LayoutModel;
            var env = (from e in layoutModel.PxEnvironments
                       where e.EnvironmentId == envId
                       select e.Title).FirstOrDefault();
            

            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        var url = String.Format("{0}/{1}/Home/Index", Request.ApplicationPath, env);
                        url = VirtualPathUtility.ToAppRelative(url);
                        return Redirect(url);
                    }
                }
                else
                {
                    ModelState.AddModelError("IncorrectLogin", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOn", "Account");
        }


        [Authorize]
        public ActionResult Users()
        {
            IMenuService menuService = new MenuService();


            SettingsModel settingsModel = new SettingsModel();
            settingsModel.MenuModel = menuService.GetSettingsMenu();

            this.ViewData.Model = settingsModel;

            return View();
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                var user = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                //this.HttpContext.Profile.SetPropertyValue

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[Authorize]
        //public ActionResult GetAllUSers(string sidx, string sord, int page, int rows)
        //{
        //    var users = Membership.GetAllUsers();

        //    List<UserInfoModel> userInfoModels = new List<UserInfoModel>();

        //    foreach (MembershipUser u in users)
        //    {
        //        UserInfoModel uModel = new UserInfoModel();
        //        uModel.UserName = u.UserName;
        //        uModel.Email = u.Email;

        //        var profile = ProfileBase.Create(u.UserName, true);
        //        uModel.Name = profile.GetPropertyValue("Name") as string;
        //        userInfoModels.Add(uModel);
        //    }


        //    string editUrl = "<input type=\"button\" id=\"editUser\" title=\"Edit User: #userName#\" class=\"edit-button\" onclick=\"EditUser('#userName#');\" />";
        //    string deleteUrl = "<input type=\"button\" id=\"deleteUser\" title=\"Delete User: #userName#\" class=\"delete-button\" onclick=\"DeleteUser('#userName#');\" />";
        //    //string editUrl = string.Format("<a href=\"javascript:EditUser('#userName#')\"> Edit </a>");
        //    //string deleteUrl = string.Format("<a href=\"javascript:DeleteUser('#userName#')\"> Delete </a>");

        //    string action = string.Format("{0} {1}", editUrl, deleteUrl);

        //    var model = from r in userInfoModels.AsQueryable()
        //                select new
        //                {
        //                    UserName = r.UserName,
        //                    Name = r.Name,
        //                    Email = r.Email,
        //                    //Actions = string.Format("<a href=\"javascript:showMessageDdetails({0})\">View</a>", log.LogID)
        //                    Actions = action.Replace("#userName#", r.UserName)
        //                };

        //    var result = model.ToJqGridData(page, rows, sidx + " " + sord, "",
        //        new[] {
        //            "UserName", "Name", "Email", "Actions"
        //        });

        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}

        [Authorize]
        [HttpPost]
        public ActionResult DeleteUser(string userName)
        {
            if (this.User.Identity.Name == userName)
            {
                return Json(new { result = false, message = "User Not Deleted: You are currently logged in with the same user" });
            }

            bool status = Membership.DeleteUser(userName);
            string retMessage = "SUCCESS";
            if (!status)
            {
                retMessage = "An error occured on server, User Not Deleted";
            }
            return Json(new { result = status, message = retMessage });
        }

        //[Authorize]
        //public ActionResult AddUpdateUser(string userName)
        //{
        //    MembershipUser user = Membership.GetUser(userName);

        //    var profile = ProfileBase.Create(userName);
        //    var uName = profile.GetPropertyValue("Name");

        //    UserInfoModel model = new UserInfoModel();
        //    model.Mode = "adduser";
        //    if (user != null)
        //    {
        //        model.UserName = user.UserName;
        //        model.Name = uName.ToString();
        //        model.Email = user.Email;
        //        model.Password = string.Empty;
        //        model.ConfirmPassword = string.Empty;
        //        model.Mode = "edituser";
        //    }
        //    return View("UserInfo", model);
        //}

        //[HttpPost]
        //[Authorize]
        //public ActionResult AddUpdateUser(User model)
        //{
        //    MembershipUser  user = Membership.GetUser(model.UserName);
        //    if (model.Mode == "adduser" )
        //    {
        //        if (user != null)
        //        {
        //            return Json(new {username= user.UserName, message = "User with this name already exist. Please select a different UserName"});
        //        }
        //        MembershipCreateStatus createStatus;
        //        user = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
           
        //    }
        //    else if (model.Mode == "edituser")
        //    {
        //        if (!string.IsNullOrEmpty(model.Password))
        //        {
        //            user.ChangePassword(user.ResetPassword(), model.Password);
        //        } 
        //        user.Email = model.Email;
        //        Membership.UpdateUser(user);
        //    }
            
        //    var profile = ProfileBase.Create(model.UserName);
        //    profile.SetPropertyValue("Name", model.Name);
        //    profile.Save();

        //    return Json(new { username = user.UserName, message = "SUCCESS" });
        //}


       
        //
        // GET: /Account/ChangePassword

        [Authorize]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                     
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserInfo()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
