using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides ability to list all or specific Announcements on the current entity
    /// </summary>
    public interface IAnnouncementActions
    {
        /// <summary>
        /// List of active Announcements made on the current entity
        /// </summary>
        /// <returns>list of active Announcements for the current entity</returns>
        List<Announcement> ListAnnouncements(out int archivedCount);

        /// <summary>
        /// List of all Annoucements made on the current entity
        /// </summary>
        /// <returns></returns>
        List<Announcement> ListAllAnnouncements();

        /// <summary>
        ///  Adds an Announcement in DLAP
        /// </summary>
        /// <param name="announcementText"></param>
        /// <param name="prevSequence"></param>
        /// <param name="nextSequence"></param>
        void AddAnnouncement(string announcementText, string prevSequence, string nextSequence);

        /// <summary>
        /// Removes the announcement from Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        void RemoveAnnouncement(string announcementID);

        /// <summary>
        /// Edit the announcement in the Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>
        void EditAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder);

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
        string MoveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder);

        /// <summary>
        /// Pin the announcement by updating the PinSortOrder on the Agilix server
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>        
        void PinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder);

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
        string UnPinAnnouncement(string announcementID, string announcementText, DateTime creationDate, string prevSequence, string nextSequence, string pinSortOrder);

        /// <summary>
        /// Archives the announcement in Agilix
        /// </summary>
        /// <param name="announcementID"></param>
        /// <param name="announcementText"></param>
        /// <param name="creationDate"></param>
        /// <param name="sequence"></param>
        /// <param name="pinSortOrder"></param>
        void ArchiveAnnouncement(string announcementID, string announcementText, DateTime creationDate, string sequence, string pinSortOrder);

    }
}
