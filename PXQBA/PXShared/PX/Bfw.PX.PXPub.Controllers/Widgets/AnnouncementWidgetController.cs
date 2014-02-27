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

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides all actions necessary to support the AnnouncementWidget.
    /// </summary>
    [PerfTraceFilter]
    public class AnnouncementWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The announcements actions.
        /// </summary>
        /// <value>
        /// The announcement actions.
        /// </value>
        protected BizSC.IAnnouncementActions AnnouncementActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IAnnouncementActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="announcementActions">The announcement actions.</param>
        public AnnouncementWidgetController(BizSC.IBusinessContext context, BizSC.IAnnouncementActions announcementActions)
        {
            Context = context;
            AnnouncementActions = announcementActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets a summarized list of all Announcements for the current entity and
        /// renders them in a view.
        /// </summary>
        /// <returns>
        /// ViewResult that renders a summarized list of Announcements.
        /// </returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ActionResult result = null;
            ViewData.Model = MostRecent(10);
            result = View();
            return result;
        }

        /// <summary>
        /// Lists all announcements.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {            
            ViewData.Model = AllAnnouncements();
            return View("ViewAllAnnouncement");
        }

        /// <summary>
        /// Fill an AnnouncementWidget model with all announcements.
        /// </summary>
        /// <returns></returns>
        protected AnnouncementWidget AllAnnouncements()
        {            
            var biz = AnnouncementActions.ListAllAnnouncements();
            var model = new AnnouncementWidget();
            model.IsInstructor = (Context.AccessLevel == BizSC.AccessLevel.Instructor);
            if (!biz.IsNullOrEmpty())
            {
                model.Announcements = biz.Map(b => b.ToAnnouncement()).ToList();
            }
            //adjust the date to timezone
            
            foreach (var announcement in model.Announcements)
            {
                announcement.DisplayDate = DateTimeConversion.UtcRelativeAdjustCommon(announcement.DisplayDate, Context.Course.CourseTimeZone);
            }
            return model;
        }

        /// <summary>
        /// Mosts the recent.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        protected AnnouncementWidget MostRecent(int n)
        {
            int archivedCount;
            var biz = AnnouncementActions.ListAnnouncements(out archivedCount);
            var model = new AnnouncementWidget();
            model.IsInstructor = (Context.AccessLevel == BizSC.AccessLevel.Instructor);
            if (!biz.IsNullOrEmpty())
            {
                model.Announcements = biz.Map(b => b.ToAnnouncement()).Take(n).ToList();
            }            
            //adjust the date to timezone
            foreach (var announcement in model.Announcements)
            {
                announcement.DisplayDate = DateTimeConversion.UtcRelativeAdjustCommon(announcement.DisplayDate, Context.Course.CourseTimeZone);
            }

            model.ArchivedCount = archivedCount;

            return model;
        }

        /// <summary>
        /// Save an announcement, or update an existing one.
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveAnnouncement(HtmlDocument doc)
        {
            return null;
        }

        /// <summary>
        /// Gets the list of all Annoucements for the current entity and renders them in a view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewSummary()
        {            
            ViewData.Model = MostRecent(10);            
            return View("Summary");
        }

        /// <summary>
        /// Add an announcement for the current course
        /// </summary>
        public void AddAnnoucement(string announcementText, string prevSequence, string nextSequence)
        {
            AnnouncementActions.AddAnnouncement(announcementText, prevSequence, nextSequence);
        }

        /// <summary>
        /// Remove an announcement from Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <returns></returns>
        public ActionResult RemoveAnnouncement(string announcementID)
        {
            AnnouncementActions.RemoveAnnouncement(announcementID);
            return Json(new { Result = "Deleted the announcement successfully." });
        }

        /// <summary>
        /// Edit the announcement in Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <returns></returns>
        public ActionResult EditAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            creationDate = DateTimeConversion.ConvertToServerTime(creationDate, Context.Course.CourseTimeZone);
            AnnouncementActions.EditAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder);
            return Json(new { Result = "Announcement Edited Successfully." });
        }

        /// <summary>
        /// Update the sequence of the announcement in Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        /// <returns></returns>
        public ActionResult MoveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder)
        {
            creationDate = DateTimeConversion.ConvertToServerTime(creationDate, Context.Course.CourseTimeZone);
            var announcementSequence = AnnouncementActions.MoveAnnouncement(announcementID, announcementText, creationDate, prevSequence, nextSequence, pinSortOrder);
            return Json(new { result = "Announcement Move Successful.", sequence = announcementSequence });
        }

        /// <summary>
        /// Update the PinSortOrder on the Agilix server
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public ActionResult PinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            creationDate = DateTimeConversion.ConvertToServerTime(creationDate, Context.Course.CourseTimeZone);
            AnnouncementActions.PinAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder);
            return Json(new { result = "Pinning the announcement successsful!" });
        }

        /// <summary>
        /// Un-pin the announcement by updating the PinSortOrder
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        /// <param name="pinSortOrder"></param>
        /// <returns></returns>
        public ActionResult UnPinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder)
        {
            creationDate = DateTimeConversion.ConvertToServerTime(creationDate, Context.Course.CourseTimeZone);
            var sequence = AnnouncementActions.UnPinAnnouncement(announcementID, announcementText, creationDate, prevSequence, nextSequence, pinSortOrder);
            return Json(new { result = "Announcement Upin successfull", sequence = sequence });
        }

        public ActionResult ArchiveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            creationDate = DateTimeConversion.ConvertToServerTime(creationDate, Context.Course.CourseTimeZone);
            AnnouncementActions.ArchiveAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder);
            return Json(new { Result = "Announcement archived successfully." });
        }
        #endregion
    }
}
