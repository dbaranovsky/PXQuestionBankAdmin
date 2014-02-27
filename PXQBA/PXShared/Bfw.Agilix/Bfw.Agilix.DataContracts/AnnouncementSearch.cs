using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Search Parameters that can be used to find and filter a search 
    /// for Announcements
    /// </summary>
    [DataContract]
    public class AnnouncementSearch
    {
        /// <summary>
        /// Id of the entity to filter announcements by, if any
        /// </summary>
        /// 
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Path to the announcment
        /// </summary>   
        /// 
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// If set, only announcements up to and including this date will be returned
        /// </summary>
        /// 
        [DataMember]
        public DateTime? Date { get; set; }

        /// <summary>
        /// If set to a value creater than zero, then only this number of announcements will be returned
        /// </summary>
        /// 
        [DataMember]
        public int Limit { get; set; }
    }
}
