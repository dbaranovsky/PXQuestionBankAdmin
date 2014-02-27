using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.PX.Biz.DataContracts;
using Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the IAnnouncementActions interface
    /// </summary>
    public class AnnouncementActions : IAnnouncementActions
    {
        #region Properties

        /// <summary>
        /// Current Business Context
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The ISessionManager implementation to use for communicating with Dlap
        /// </summary>
        private ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a default AnnouncementActions, which depends on IBusinessContext
        /// and IAnnouncementService implementations.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public AnnouncementActions(IBusinessContext context, ISessionManager sessionManager)
        {
            Context = context;
            SessionManager = sessionManager;
        }

        #endregion

        #region IAnnouncementActions Members

        /// <summary>
        /// List of active Announcements made on the current entity
        /// </summary>
        /// <param name="archivedCount"></param>
        /// <returns>
        /// list of active Announcements for the current entity
        /// </returns>
        public List<DataContracts.Announcement> ListAnnouncements(out int archivedCount)
        {
            List<DataContracts.Announcement> items = new List<DataContracts.Announcement>();
            List<DataContracts.Announcement> announcementList = new List<DataContracts.Announcement>();
            List<DataContracts.Announcement> activeItems = new List<DataContracts.Announcement>();
            List<DataContracts.Announcement> archivedItems = new List<DataContracts.Announcement>();

            archivedCount = 0;

            using (Context.Tracer.StartTrace("AnnouncementActions.ListAnnouncements(archiveCount)"))
            {
                //Get the Announcement List for the current Entity
                var cmd = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = Context.EntityId } };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                items = cmd.Announcements.Map(a => a.ToAnnouncement()).ToList();

                //Get the Announcement body given the path of the announcement
                if (!items.IsNullOrEmpty())
                {
                    activeItems = (from item in items
                                   where item.EndDate > DateTime.Now.GetCourseDateTime(Context)
                                   select item).ToList();

                    archivedItems = (from item in items
                                     where item.EndDate <= DateTime.Now.GetCourseDateTime(Context)
                                     select item).ToList();

                    foreach (var item in activeItems)
                    {
                        var cmdAnnouncement = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = Context.EntityId, Path = item.Path } };
                        SessionManager.CurrentSession.ExecuteAsAdmin(cmdAnnouncement);
                        if (!cmdAnnouncement.Announcements.IsNullOrEmpty())
                        {
                            announcementList.Add(cmdAnnouncement.Announcements.First().ToAnnouncement());
                        }
                    }

                    var Announcements = (from item in announcementList
                                         where string.IsNullOrEmpty(item.PinSortOrder)
                                         select item).ToList();
                    Announcements = Announcements.OrderBy(LexicalSortFunction()).ToList();
                    var pinnedAnnouncements = (from item in announcementList
                                               where !(string.IsNullOrEmpty(item.PinSortOrder))
                                               select item).ToList();
                    pinnedAnnouncements = pinnedAnnouncements.OrderBy(NumericalSortFunction()).ToList();

                    foreach (var announcement in pinnedAnnouncements)
                    {
                        var index = int.Parse(announcement.PinSortOrder) - 1;
                        if (index < Announcements.Count)
                        {
                            Announcements.Insert(index, announcement);
                        }
                        else
                        {
                            Announcements.Add(announcement);
                        }
                    }

                    announcementList = Announcements;
                    //announcementList = announcementList.OrderBy(LexicalSortFunction()).ToList();
                    archivedCount = archivedItems.Count();
                }

            }

            return announcementList;
        }

        /// <summary>
        /// List of all Annoucements made on the current entity
        /// </summary>
        /// <returns></returns>
        public List<DataContracts.Announcement> ListAllAnnouncements()
        {
            List<DataContracts.Announcement> items = new List<DataContracts.Announcement>();
            List<DataContracts.Announcement> announcementList = new List<DataContracts.Announcement>();

            using (Context.Tracer.StartTrace("AnnouncementActions.ListAllAnnouncements"))
            {
                //Get the Announcement List for the current Entity
                var cmd = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = Context.EntityId } };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                items = cmd.Announcements.Map(a => a.ToAnnouncement()).ToList();

                //Get the Announcement body given the path of the announcement
                if (!items.IsNullOrEmpty())
                {
                    foreach (var item in items)
                    {
                        var cmdAnnouncement = new GetAnnouncements() { SearchParameters = new AnnouncementSearch() { EntityId = Context.EntityId, Path = item.Path } };
                        SessionManager.CurrentSession.ExecuteAsAdmin(cmdAnnouncement);
                        if (!cmdAnnouncement.Announcements.IsNullOrEmpty())
                        {
                            var announcement = cmdAnnouncement.Announcements.First().ToAnnouncement();
                            if (announcement.EndDate <= DateTime.Now.GetCourseDateTime(Context))
                            {
                                announcement.IsArchived = true;
                            }
                            announcementList.Add(announcement);
                        }
                    }

                    //find out the un-pinned announcements (unarchived)
                    var Announcements = (from item in announcementList
                                         where string.IsNullOrEmpty(item.PinSortOrder) && !item.IsArchived
                                         select item).ToList();
                    Announcements = Announcements.OrderBy(LexicalSortFunction()).ToList();
                    //find out the pinned announcements (unarchived)
                    var pinnedAnnouncements = (from item in announcementList
                                               where !(string.IsNullOrEmpty(item.PinSortOrder)) && !item.IsArchived
                                               select item).ToList();
                    pinnedAnnouncements = pinnedAnnouncements.OrderBy(NumericalSortFunction()).ToList();
                    //find out the archived announcements
                    var archivedAnnouncements = (from item in announcementList
                                                 where item.IsArchived
                                                select item).ToList();
                    archivedAnnouncements = archivedAnnouncements.OrderByDescending(DateSortFunction()).ToList();


                    foreach (var announcement in pinnedAnnouncements)
                    {
                        var index = int.Parse(announcement.PinSortOrder) - 1;
                        if (index < Announcements.Count)
                        {
                            Announcements.Insert(index, announcement);
                        }
                        else
                        {
                            Announcements.Add(announcement);
                        }
                    }

                    //add the archived announcements to the end of the list
                    Announcements.AddRange(archivedAnnouncements);

                    
                    announcementList = Announcements;                    
                }
            }

            return announcementList;
        }

        /// <summary>
        /// Adds an Announcement in DLAP
        /// </summary>
        /// <param name="announcementText"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        public void AddAnnouncement(string announcementText, string prevSequence, string nextSequence)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.AddAnnouncement(announcementText={0}, prevSequence={1}, nextSequence={2})", announcementText, prevSequence,nextSequence)))
            {
                var path = Guid.NewGuid().ToString("N");
                var creationDate = DateTime.Now.GetCourseDateTime(Context);
                var sequence = Context.Sequence(prevSequence, nextSequence);
                var pinSortOder = "";
                var endDate = DateTime.MaxValue;
                InsertAnnouncement(path, announcementText, creationDate, sequence, pinSortOder, endDate);
            }
        }

        /// <summary>
        /// Removes the announcement from Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        public void RemoveAnnouncement(string announcementID)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.RemoveAnnouncement(announcementID={0})", announcementID)))
            {
                var cmd = new DeleteAnnouncements()
                {
                    Announcements = new Bfw.Agilix.DataContracts.Announcement[] {
                        new Bfw.Agilix.DataContracts.Announcement(){ EntityId = Context.EntityId, Path = announcementID }
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        /// <summary>
        /// Edit the announcement in the Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>
        public void EditAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.EditAnnouncement(announcementID={0}, announcementText={1}, creationDate={2}, sequence={3}, pinSortOrder={4})", announcementID, announcementText, creationDate.ToString(), sequence, pinSortOrder)))
            {
                var endDate = DateTime.MaxValue;
                InsertAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder, endDate); 
            }
        }

        /// <summary>
        /// Update the Annoucement sequence in Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        /// <param name="pinSortOrder"></param>
        /// <returns></returns>
        public string MoveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder)
        {
            string sequence = string.Empty;
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.MoveAnnouncement(announcementID={0}, announcementText={1}, creationDate={2}, prevSequence={3}, nextSequence={4}, pinSortOrder={5})", announcementID, announcementText, creationDate.ToString(), prevSequence, nextSequence, pinSortOrder)))
            {
                sequence = Context.Sequence(prevSequence, nextSequence);
                var endDate = DateTime.MaxValue;
                InsertAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder, endDate);                
            }
            return sequence;
        }

        /// <summary>
        /// Pin the announcement by updating the PinSortOrder on the Agilix server
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>
        public void PinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.PinAnnouncement(announcementID={0}, announcementText={1}, creationDate={2}, sequence={3}, pinSortOrder={4})", announcementID, announcementText, creationDate.ToString(), sequence, pinSortOrder)))
            {
                var endDate = DateTime.MaxValue;
                InsertAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder, endDate);
            }
        }

        /// <summary>
        /// Upin the announcement by updating the PinSortOrder on the agilix server
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        /// <param name="pinSortOrder"></param>
        /// <returns></returns>
        public string UnPinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder)
        {
            var sequence = string.Empty;
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.UnPinAnnouncement(announcementID={0}, announcementText={1}, creationDate={2}, prevSequence={3}, nextSequence={4}, pinSortOrder={5})", announcementID, announcementText, creationDate.ToString(), prevSequence, nextSequence, pinSortOrder)))
            {
                sequence = Context.Sequence(prevSequence, nextSequence);
                var endDate = DateTime.MaxValue;
                InsertAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder, endDate);
            }
            return sequence;
        }

        /// <summary>
        /// Archives the announcement in Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>
        public void ArchiveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.ArchiveAnnouncement(announcementID={0}, announcementText={1}, creationDate={2}, sequence={3}, pinSortOrder={4})", announcementID, announcementText, creationDate.ToString(), sequence, pinSortOrder)))
            {
                var endDate = DateTime.Now.GetCourseDateTime(Context);
                InsertAnnouncement(announcementID, announcementText, creationDate, sequence, pinSortOrder, endDate); 
            }
        }

        /// <summary>
        /// function to perform sorting on creation date of the announcement
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        protected Func<DataContracts.Announcement, DateTime> DateSortFunction()
        {
            Func<DataContracts.Announcement, DateTime> func = null;
            func = x => x.EndDate;
            return func;
        }

        /// <summary>
        /// function to perform lexical sorting
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        protected Func<DataContracts.Announcement, string> LexicalSortFunction()
        {
            Func<DataContracts.Announcement, string> func = null;
            func = x => x.PrimarySortOrder;
            return func;
        }

        /// <summary>
        /// perform sorting based on PinSortOrder
        /// </summary>
        /// <returns></returns>
        protected Func<DataContracts.Announcement, int> NumericalSortFunction()
        {
            Func<DataContracts.Announcement, int> func = null;
            func = x => int.Parse(x.PinSortOrder);
            return func;
        }

        /// <summary>
        /// Insert the announcement into Agilix
        /// </summary>
        protected void InsertAnnouncement(string announcementPath, string announcementText, DateTime creationDate, string sequence, string pinSortOrder, DateTime endDate)
        {
            using (Context.Tracer.StartTrace(string.Format("AnnouncementActions.InsertAnnouncement(announcementPath={0}, announcementText={1}, creationDate={2}, sequence={3}, pinSortOrder={4}, endDate={5})", announcementPath, announcementText, creationDate.ToString(), sequence, pinSortOrder, endDate)))
            {
                var announ = new Bfw.Agilix.DataContracts.Announcement()
                {
                    Html = announcementText,
                    Path = announcementPath,
                    CreationDate = creationDate,
                    EntityId = Context.EntityId,
                    Title = "Announcement",
                    Version = "0.0.0.1",
                    StartDate = creationDate,
                    EndDate = endDate,
                    PrimarySortOrder = sequence,
                    PinSortOrder = pinSortOrder
                };

                DlapCommand cmdAnnouncement = new PutAnnoucement()
                {
                    DomainId = Context.EntityId,
                    Announcement = announ,
                    Path = announ.Path
                };
                SessionManager.CurrentSession.Execute(cmdAnnouncement);
            }
        }
        
        #endregion
    }
}
