using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents a list of SyncItem instances that are being synced back
    /// to the central content repository.
    /// </summary>
    public class SyncItemList
    {
        #region Data Members

        /// <summary>
        /// True if an error has occured during processing.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Contains an error message if Error is true.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The number of SyncItem instances returned in this response.
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                if (SyncItems != null)
                {
                    count = SyncItems.Count();
                }

                return count;
            }
        }

        /// <summary>
        /// Collection of SyncItem instances that are contained in this response.
        /// </summary>
        public IEnumerable<SyncItem> SyncItems { get; set; }

        #endregion
    }
}
