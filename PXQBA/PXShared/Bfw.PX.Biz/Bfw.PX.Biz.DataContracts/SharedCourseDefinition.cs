using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SharedCourseDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        
        [DataMember]
        public String AnonyousName { get; set; }

        /// <summary>
        /// Flag the student as anonymous
        /// </summary>
        [DataMember]
        public Boolean IsAnonymous { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String Note { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public String SharedCourseId { get; set; }
        
        /// <summary>
        /// User id of the owner of the shared student eportfolio
        /// </summary>
        [DataMember]
        public String OwnersUserId { get; set; }

        /// <summary>
        /// EnrollmentID of the owner for the current course
        /// </summary>
        [DataMember]
        public String OwnerEnrollmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IList<String> SharedUserIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IList<String> SharedItemIds { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IList<String> SharedInactiveUserIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public IList<String> SharedInactiveItemIds { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public SharedCourseDefinition()
        {
            SharedUserIds = new List<String>();
            SharedItemIds = new List<String>();
            SharedInactiveItemIds = new List<String>();
            SharedInactiveUserIds = new List<String>();
        }

    }
}
