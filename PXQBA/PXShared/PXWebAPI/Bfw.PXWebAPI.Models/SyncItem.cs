using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents an item or set of items that can be synced back to a central
    /// content repository, and then be pushed back into Platform-X.
    /// </summary>
    public class SyncItem
    {
        #region Data Members

        /// <summary>
        /// For convenience, the DLAP Id of the item is returned as part of the message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Contains the name of the system that created the content originally.
        /// </summary>
        public string CreatedBySystem { get; set; }

        /// <summary>
        /// Contains the name of the user from the system that created the content originally.
        /// </summary>
        public string CreatedByUser { get; set; }

        /// <summary>
        /// Contains the name of the system that modified the content.
        /// </summary>
        public string ModifiedBySystem { get; set; }

        /// <summary>
        /// Contains the name of the user from the system that modified the content.
        /// </summary>
        public string ModifiedByUser { get; set; }

        /// <summary>
        /// Contains the date and time the content was modified in UTC, using the following format:
        /// 2010-11-30T23:59:00Z
        /// </summary>
        public string ModifiedDate { get; set; }

        /// <summary>
        /// Contains the full XML representation of the DLAP PutItems call necessary to push the content
        /// back into DLAP from the central content repository. The central repository can simply store this
        /// as a blob or can extract/modify the data contained in the XML document. The reference for the full
        /// XML schema is: http://dev.dlap.bfwpub.com/js/docs/#!/Schema/ItemData
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Tells the system being synced to what type of state the item being synced is in, e.g. Created, Modified, Deleted.
        /// </summary>
        public SyncStatus Status { get; set; }

        #endregion
    }
}
