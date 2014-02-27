using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters used to search for <see cref="Message" /> in the system.
    /// </summary>
    public class MessageSearch
    {
        /// <summary>
        /// Id of the entity the <see cref="Message" /> exists in.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Id of the <see cref="Discussion" /> the <see cref="Message" /> is part of.
        /// </summary>
        public string DiscussionId { get; set; }

        /// <summary>
        /// Id of the <see cref="Message" /> to find.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Maximum number of <see cref="Message" /> to return.
        /// </summary>
        public int Limit { get; set; }
    }
}
