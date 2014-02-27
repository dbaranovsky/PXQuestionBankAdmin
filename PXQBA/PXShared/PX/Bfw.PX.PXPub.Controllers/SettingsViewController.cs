using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using System.Xml.Linq;

using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;


namespace Bfw.PX.PXPub.Controllers {

    [PerfTraceFilter]
    public class SettingsViewController : Controller
    {

        #region Properties

        /// <summary>
        /// Content Actions
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>/// <param name="contentHelper">Context Helper</param>
        public SettingsViewController( BizSC.IBusinessContext context, IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        #endregion

        #region Methods

        public PartialViewResult Index(string id)
        {
            ContentItem item = ContentActions.GetContent( Context.EntityId, id );
            return PartialView("~/Views/ContentWidget/SettingsView.ascx", item);
        }

        public JsonResult SaveSettings( string itemId, bool visibleInToc, bool visibleToStudent, bool byDate, string date, string time )
        {
            DateTime availableDate;
            Regex checktime = new Regex( @"^((([0]?[1-9]|1[0-2])(:|\.)[0-5][0-9]((:|\.)[0-5][0-9])?( )?(AM|am|aM|Am|PM|pm|pM|Pm))|(([0]?[0-9]|1[0-9]|2[0-3])(:|\.)[0-5][0-9]((:|\.)[0-5][0-9])?))$" );

            bool isDate = DateTime.TryParse(date, out availableDate);
            bool isTime = checktime.IsMatch(time);
            // Getting existing ContentItem
            ContentItem item = ContentActions.GetContent(Context.EntityId, itemId);

            // had to invert here because the IU uses Visible and the application uses hidden
            item.Hidden = !visibleInToc;
            item.HiddenFromStudents = !visibleToStudent;
            item.HiddenFromToc = !visibleInToc;
            // checking to make sure that the checkbox is checked
            if (byDate && !string.IsNullOrEmpty(time))
            {
                // making sure the year is not 1 and that the date string is a date 
                if (isDate && availableDate.Year != 1)
                {
                    if ( (time.ToUpper().Contains( "AM" ) || time.ToUpper().Contains( "PM" ) ) && isTime )
                    {
                        DateTime newDateTime;
                        bool isNewDate = DateTime.TryParse( string.Format("{0} {1}", date, time), out newDateTime );
                        if (isNewDate)
                        {
                            // finally we set the new date and time
                            item.AvailableDate = newDateTime.ToUniversalTime();

                        } else
                        {
                            return Json( new { status = "fail", reason = "Ooops! Something went wrong." } );
                        }
                    }
                    else
                    {
                        return Json( new { status = "fail", reason = "Please enter a valid time HH:MM AM/PM" } );
                    }
                }
                else
                {
                    return Json( new { status = "fail", reason = "Please enter a valid date." } );
                }
            } else
            {
                item.AvailableDate = DateTime.MinValue;
            }
            // doing a try catch here since the StoreContent method is void.
            try
            {
                ContentActions.StoreContent(item);
                return Json( new { status = "sucess", reason = "Your settings have been saved", itemid = item.Id, title = item.Title, hfs = item.HiddenFromStudents, hft = item.HiddenFromToc, ad = availableDate } );
            } catch(Exception ex)
            {
                return Json( new { status = "fail", reason = ex.Message } );
            }
        }

        public ActionResult SaveContentSettings(ContentSettings contentSettings)
        {
            string entityId = string.IsNullOrEmpty(contentSettings.SettingsEntityId) ? Context.EntityId : contentSettings.SettingsEntityId;
            var ci = ContentActions.GetContent(entityId, contentSettings.Id);

            ci.SetVisibility(contentSettings.Visibility, contentSettings.DueDate);
            //SetVisibility(ci, contentSettings.Visibility, contentSettings.DueDate);
            ci.Properties["bfw_syllabusfilter"] = new BizDC.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = contentSettings.SelSyllabusFilter };            
            ContentActions.StoreContent(ci, entityId);
            
            return new EmptyResult();
        }

        #endregion
    }
}
