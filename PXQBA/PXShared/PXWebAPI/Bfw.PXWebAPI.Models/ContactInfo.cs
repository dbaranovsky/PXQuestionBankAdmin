using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// The following XML should be used to tell what contact information is available to a particular course: 
    /// <bfw_contact_info> 
    ///   <info ContactType="phone" ContactValue="9175685565" /> 
    ///   <info ContactType="email" ContactValue="test@test.com" /> 
    ///</bfw_contact_info> 
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Type of the contact
        /// </summary>
        public string ContactType { get; set; }

        /// <summary>
        /// Value for the contact info
        /// </summary>
        public string ContactValue { get; set; }
    }
}
