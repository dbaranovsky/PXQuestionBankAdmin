using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Indicates data that the user can have shared with them
    /// </summary>
    public class DataShare
    {
        /// <summary>
        /// Id of the User that has shared the data (i.e. the data's source)
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Username of the user that has shared the data
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// True is the current user has accepted the shared data
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Type of the data being shared
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public ShareType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Number of items that UserId has available to share
        /// </summary>
        /// <value>
        /// The item count.
        /// </value>
        public int? ItemCount
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Indicates the type of data being shared
    /// </summary>
    public enum ShareType
    {
        Note = 0,
        Highlight = 1
    }
}
