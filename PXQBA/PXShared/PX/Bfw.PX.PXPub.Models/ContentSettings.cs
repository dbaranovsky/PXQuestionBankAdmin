using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.PXPub.Models
{
    public class ContentSettings
    {
        /// <summary>
        /// Show to Student or Not
        /// </summary>
        public string Visibility { get; set; }

        /// <summary>
        /// The restrictions for student by date or my pre - req
        /// </summary>
        public string Restricted { get; set; }

        /// <summary>
        /// the due date restriction for a student
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The unique Id of the item.
        /// </summary>
        [DataMember]
        public string Id { get; set; }


        public string SelSyllabusFilter { get; set; }

        /// <summary>
        /// Settings entity id
        /// </summary>
        public string SettingsEntityId { get; set; }
    }
}
