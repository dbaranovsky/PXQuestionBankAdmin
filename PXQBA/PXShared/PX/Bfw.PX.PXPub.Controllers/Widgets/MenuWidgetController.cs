using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Configuration;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the MenuWidget
    /// </summary>
    [PerfTraceFilter]
    public class MenuWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }


        /// <summary>
        /// Constructor to initialize the Menu Widget Controller
        /// </summary>
        /// <param name="context"></param>
        public MenuWidgetController(BizSC.IBusinessContext context, BizSC.IEnrollmentActions enrollmentActions, BizSC.IPageActions pageActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            PageActions = pageActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets the Menu Widget summary
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            Menu menu = new Menu();
            Course course = Context.Course.ToCourse();
            string menuKey = "";

            if (widget.Properties.ContainsKey("bfw_target_menu"))
            {
                menuKey = widget.Properties["bfw_target_menu"].Value.ToString();
            }

            if (!string.IsNullOrEmpty(menuKey))
            {
                menu = this.PageActions.LoadMenu(menuKey).ToMenu();

                if (!menu.MenuItems.IsNullOrEmpty())
                {
                    if (Context.CourseIsProductCourse)
                        menu.MenuItems.RemoveAll(i => i.BfwDisplayOnProductCourse == false);
                }

                menu.SetActiveItem(menu.MenuItems, widget);

                var displayOption = (Context.AccessLevel == BizSC.AccessLevel.Student) ? 
                    BizDC.DisplayOption.Student : 
                    BizDC.DisplayOption.Instructor;
                
                menu.RemoveMenuItemsBasedOnRole(displayOption);
            }

            ViewData.Model = menu;
            ViewData["AccessLevel"] = Context.AccessLevel;

            return View("Summary");
        }

        /// <summary>
        /// Get the Menu in the View All mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View("ViewAll");
        }

        #endregion


    }
}
