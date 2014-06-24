using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.User;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class UserController : MasterController
    {
        private readonly IUserManagementService userManagementService;

        private readonly IProductCourseManagementService productCourseManagementService;

        private readonly int usersPerPage;

        public UserController(IUserManagementService userManagementService, IProductCourseManagementService productCourseManagementService):base(productCourseManagementService, userManagementService)
        {
            this.userManagementService = userManagementService;
            usersPerPage = ConfigurationHelper.GetUsersPerPage();
            this.productCourseManagementService = productCourseManagementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Keeps the session alive.
        /// </summary>
        /// <param name="data">Random value.</param>
        /// <returns></returns>
        public ActionResult KeepSessionAlive(string data)
        {
            return new JsonResult { Data = "Success" };
        }

        /// <summary>
        /// Returns availible notification for current user
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentUserNotificationForUrl()
        {
            return  JsonCamel(GetAllNotification());
        }

        /// <summary>
        /// Forbid notification showing for current user
        /// </summary>
        /// <param name="type">notification type</param>
        /// <returns></returns>
        public ActionResult DontShowForCurrentUser(NotificationType type)
        {
            userManagementService.CreateNotShownNotification(type);
            return JsonCamel(new {isSuccess = true});
        }

        private IEnumerable<UserNotificationViewModel> GetAllNotification()
        {
            var notShownNotifications = userManagementService.GetNotShownNotification();
           return (from notification in (NotificationType[]) Enum.GetValues(typeof (NotificationType))
                   select new UserNotificationViewModel()
                       {
                           IsShown = notShownNotifications.All(n => n.NotificationType != notification), 
                           Message = EnumHelper.GetEnumDescription(notification), 
                           NotificationTypeId = (int) notification
                       }).ToList();
        }


        public ActionResult GetRolesForCourse(string courseId)
        {
            UpdateCurrentCourse(courseId);
            var roles = Mapper.Map<RoleListViewModel>(userManagementService.GetRolesForCourse(courseId));
            UpdateCapabilities(roles, courseId);
            return JsonCamel(roles);
        }

        private void UpdateCapabilities(RoleListViewModel roles, string courseId)
        {
            roles.CanDefineNewRole = UserCapabilitiesHelper.Capabilities.Contains(Capability.DefineRole);
        }

        public ActionResult DeleteRole(string courseId, int roleId)
        {
            userManagementService.DeleteRole(roleId);
            return JsonCamel(true);
        }

        public ActionResult GetRoleCapabilities(int? roleId, string courseId)
        {
            if (roleId.HasValue && roleId.Value > 0)
            {
            }
            var role = Mapper.Map<RoleViewModel>(userManagementService.GetRole(courseId, roleId));
            return JsonCamel(role);
        }



        public ActionResult SaveRole(RoleViewModel role, string courseId)
        {
            userManagementService.UpdateRole(courseId, Mapper.Map<Role>(role));
            return JsonCamel(new { isSuccess = true });
        }

        [HttpPost]
        public ActionResult GetAllCourses()
        {
            var courses = Mapper.Map<IEnumerable<ProductCourseViewModel>>(
                                            productCourseManagementService.GetAllCourses());
            return JsonCamel(courses);
        }

        /// <summary>
        /// Return all users for page
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUsers(int page)
        {
            var users = userManagementService.GetUsers((page - 1) * usersPerPage, usersPerPage);

            var totalPages = (users.Count() + usersPerPage - (users.Count() % usersPerPage)) / usersPerPage;
            return JsonCamel(new
                             {
                               Users = users,
                               TotalPages = totalPages,
                               CurrentPage = page,
                             });
        }

        /// <summary>
        /// Return titles with already set roles for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetAvailibleTitles(string userId)
        {
            var user = userManagementService.GetUser(userId);
            return JsonCamel(user.ProductCourses.Where(c => c.CurrentRole != null));
        }

        /// <summary>
        /// Return all titles with availible roles for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetUserRoles(string userId)
        {
            var user = userManagementService.GetUser(userId);
            UpdateCapabilities(user);
            return JsonCamel(user);
        }

        private void UpdateCapabilities(QBAUser user)
        {
            foreach (var userProductCourse in user.ProductCourses)
            {
                var capabilities = userManagementService.GetUserCapabilities(userProductCourse.Id);
                userProductCourse.CanSetRoles = capabilities.Contains(Capability.SetRoleToTitle);
            }
        }

        /// <summary>
        /// Saves user roles
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveUserRoles(QBAUser user)
        {        
            userManagementService.UpdateUserRoles(user);
            return JsonCamel(new { isSuccess = true });
        }

    }
}