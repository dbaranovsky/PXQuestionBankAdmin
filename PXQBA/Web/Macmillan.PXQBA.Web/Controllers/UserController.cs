﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.ViewModels.User;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class UserController : MasterController
    {
        private readonly IUserManagementService userManagementService;

        public UserController(IUserManagementService userManagementService)
        {
            this.userManagementService = userManagementService;
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
            return JsonCamel(GetHardCodedRoles());
        }


        public ActionResult AddRoleToCourse(string courseId, RoleViewModel role)
        {
            return JsonCamel(true);
        }

        public ActionResult RemoveRoleFromCourse(string courseId, int roleId)
        {
            return JsonCamel(true);
        }


        private IEnumerable<RoleViewModel> GetHardCodedRoles()
        {
            return new List<RoleViewModel>
                   {
                       new RoleViewModel
                       {
                           Id = 1,
                           Name = "Admin",
                           CapabilityGroups = GetHardCodedCapabilities()
                       },
                       new RoleViewModel
                       {
                           Id = 2,
                           Name = "Super Author",
                           CapabilityGroups = GetHardCodedCapabilities()
                       }
                   };
        }

        private IEnumerable<CapabilityGroupViewModel> GetHardCodedCapabilities()
        {
            return new List<CapabilityGroupViewModel>
                   {
                       new CapabilityGroupViewModel()
                       {
                           Id = 1,
                           Name = "Roles",
                           Capabilities = new List<CapabilityViewModel>()
                                          {
                                              new CapabilityViewModel()
                                              {
                                                  Id = 1,
                                                  Name = "Read roles",
                                                  IsActive = true
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 1,
                                                  Name = "Edit roles",
                                                  IsActive = true
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 1,
                                                  Name = "Create roles",
                                                  IsActive = true
                                              },
                                          }
                       },

                       new CapabilityGroupViewModel()
                       {
                           Id = 1,
                           Name = "Questions",
                           Capabilities = new List<CapabilityViewModel>()
                                          {
                                              new CapabilityViewModel()
                                              {
                                                  Id = 1,
                                                  Name = "Set status in progress",
                                                  IsActive = true
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 2,
                                                  Name = "Set status deleted",
                                                  IsActive = false
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 3,
                                                  Name = "Set status availible to instructor",
                                                  IsActive = false
                                              },
                                          }
                       }
                   };
        }
    }
}