using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class Domain
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>        
        public string ParentId { get; set; }

        /// <summary>
        /// Unique name that identifies the domain. 
        /// This is also the "login prefix" that each user enters with their username when they sign in.
        /// </summary>        
        public string Userspace { get; set; }

        /// <summary>
        /// Field reserved for any data the caller wishes to store. 
        /// We recommend it be a unique reference, such as from an external SIS system.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the HTS player URL.
        /// </summary>
        //public string HtsPlayerUrl { get; set; }

        public Dictionary<string, string> CustomQuestionUrls { get; set; }
    }
}
