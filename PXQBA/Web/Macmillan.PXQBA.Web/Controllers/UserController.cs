﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
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
            var roles = userManagementService.GetRolesForCourse(courseId);
            return JsonCamel(Mapper.Map<IEnumerable<RoleViewModel>>(roles));
        }


        public ActionResult AddRoleToCourse(string courseId, RoleViewModel role)
        {
            return JsonCamel(true);
        }

        public ActionResult RemoveRoleFromCourse(string courseId, int roleId)
        {
            return JsonCamel(true);
        }

        public ActionResult GetRoleCapabilities(string roleId, string courseId)
        {
            var role = new RoleViewModel()
                       {
                           CanDelete = true,
                           CapabilityGroups = Mapper.Map<IEnumerable<CapabilityGroupViewModel>>(CapabilityHelper.GetCapabilityGroups().ToList())
                       };

            return JsonCamel(role);
        }



        public ActionResult SaveRole(RoleViewModel role, string courseId)
        {
             
            return JsonCamel(new { isSuccess = true });
        }
       
        /// <summary>
        /// Return all users
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUsers()
        {

            return JsonCamel(GetHardCodedUsers());
        }

        /// <summary>
        /// Return titles with already set roles for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetAvailibleTitles(string userId)
        {
            return JsonCamel(GetHardCodedAvailibleRoles());
        }

        /// <summary>
        /// Return all titles with availible roles for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult GetUserRoles(string userId)
        {
            return JsonCamel(GetHardCodedUserRoles());
        }

        /// <summary>
        /// Saves user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="titles">Title roles which  was changed</param>
        /// <returns></returns>
        public ActionResult SaveUserRoles(string userId, IEnumerable<TitleRolesViewModel> titles)
        {
             return JsonCamel(new { isSuccess = true });
        
        }

        #region HardCode
        private IEnumerable<TitleRolesViewModel> GetHardCodedUserRoles()
        {
            return new List<TitleRolesViewModel>
                    {
                        new TitleRolesViewModel
                        {
                           TitleId = "14532",
                           TitleName = "Modern History",
                           CurrentRole = new RoleViewModel()
                                        {
                                             Id =  "1",
                                             Name = "Author"
                                        },
                           AvailibleRoles = new Dictionary<string, string>()
                                            {
                                                {"1","Author"},
                                                {"2"," Super Author"}
                                            }
                        },


                        new TitleRolesViewModel
                        {
                            TitleId = "4564",
                             TitleName = "Economics",
                             CurrentRole = new RoleViewModel()
                                        {       
                                               Id = "2",
                                               Name = "Super Admin"
                                        },
                            AvailibleRoles = new Dictionary<string, string>()
                                            {
                                                {"1","Admin"},
                                                {"2","Super Admin"}
                                            }
      
                        },

                         new TitleRolesViewModel
                        {
                             TitleId = "1101",
                             TitleName = "MacroEconomics",
                             CurrentRole = null,
                             AvailibleRoles = new Dictionary<string, string>()
                                            {
                                                {"1","Admin"},
                                                {"2"," Super Admin"}
                                            }
      
                        }


                    };
        }

        private IEnumerable<TitleRolesViewModel> GetHardCodedAvailibleRoles()
        {
            return new List<TitleRolesViewModel>
                    {
                        new TitleRolesViewModel
                        {
                           TitleName = "Modern History",
                           CurrentRole = new RoleViewModel()
                                        {
                                             Name = "Super Author"
                                        }
                        },

                        new TitleRolesViewModel
                        {
                             TitleName = "Economics",
                             CurrentRole = new RoleViewModel()
                                        {
                                               Name = "Administrator"
                                        }
                         
                        }
                    };
            
        }

        private IEnumerable<UserViewModel> GetHardCodedUsers()
        {
            return new List<UserViewModel>
                   {
                       new UserViewModel
                       {
                          Id = "1",
                          AvailibleTitlesCount = 32,
                          UserName = "John Smith"
                       },
                         new UserViewModel
                       {
                          Id = "2",
                          AvailibleTitlesCount = 32,
                          UserName = "John Doe"
                       },

                         new UserViewModel
                       {
                          Id = "3",
                          AvailibleTitlesCount = 32,
                          UserName = "Alex Murphy"
                       },
                   };
        }

        private IEnumerable<RoleViewModel> GetHardCodedRoles()
        {
            return new List<RoleViewModel>
                   {
                       new RoleViewModel
                       {
                           Id = "1",
                           Name = "Admin",
                           CapabilityGroups = GetHardCodedCapabilities()
                       },
                       new RoleViewModel
                       {
                           Id = "2",
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
                                                  Id = 2,
                                                  Name = "Edit roles",
                                                  IsActive = true
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 3,
                                                  Name = "Create roles",
                                                  IsActive = true
                                              },
                                          }
                       },

                       new CapabilityGroupViewModel()
                       {
                           Id = 2,
                           Name = "Questions",
                           Capabilities = new List<CapabilityViewModel>()
                                          {
                                              new CapabilityViewModel()
                                              {
                                                  Id = 4,
                                                  Name = "Set status in progress",
                                                  IsActive = true
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 5,
                                                  Name = "Set status deleted",
                                                  IsActive = false
                                              },

                                              new CapabilityViewModel()
                                              {
                                                  Id = 6,
                                                  Name = "Set status availible to instructor",
                                                  IsActive = false
                                              },
                                          }
                       }
                   };
        }

        #endregion
    }
}