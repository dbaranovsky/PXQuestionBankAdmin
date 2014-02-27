using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    public class SharedCourseDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public String AnonyousName { get; set; }

        /// <summary>
        /// Flag the student as anonymous
        /// </summary>
        public Boolean IsAnonymous { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Note { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String SharedCourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String OwnersUserId { get; set; }

        /// <summary>
        /// EnrollmentID of the owner for the current course
        /// </summary>
        public String OwnerEnrollmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<String> SharedUserIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<String> SharedItemIds { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public SharedCourseDefinition()
        {
            SharedUserIds = new List<String>();
            SharedItemIds = new List<String>();
            AnonyousName = string.Empty;
        }
    }
}
