using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Bfw.Common;
using PxWebUser;

namespace Bfw.PX.PXPub.Models
{
    
    /// <summary>
    /// This model will store all the necessary details for a user that is accessing the site and hits one of the ecommerce pages.
    /// </summary>
    public class ECommerceInfo
    {
        public ECommerceInfo()
        {
            GenericCourseSupported = false;
            GenericCourseId = string.Empty;
        }

        public bool Authenticated { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsInstructor { get; set; }

        public bool IsStudent { get; set; }

        public bool IsEntitled { get; set; }

        public bool IsProduct { get; set; }

        public string CourseTitle { get; set; }

        /// <summary>
        ///  populated by loading the item with itemid = "PX_PRODUCT_MARKETING" and getting the description of that item, which will be HTML. 
        /// </summary>
        public string MarketingInfo { get; set; }

        /// <summary>
        /// Populated by loading the item with itemid = "PX_PRODUCT_GETTING_STARTED" and getting the description of that item, which will be HTML.
        /// </summary>
        public string GettingStartedInfo { get; set; }

        public bool InMultipleDomains { get; set; }

    	public PxWebUserRights WebRights { get; set; }

        public bool GenericCourseSupported { get; set; }

        public string GenericCourseId { get; set; }

        public string SwitchEnrollFromCourse { get; set; }

        public bool AllowEditSandboxCourse { get; set; }

        public bool AllowQuestionBankAdmin { get; set; }

        //public Models.Course Course { get; set; }

        //public string Schools { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
