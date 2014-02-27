using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a user's activity with an Item.
    /// </summary>
    public class ItemActivity
    {
        /// <summary>
        /// Id of the item the user is interacting with.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Enrollment id of the user interacting with the item.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Time at which the interaction started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Duration the interaction lasted.
        /// </summary>
        public int Seconds { get; set; }
    }
}
