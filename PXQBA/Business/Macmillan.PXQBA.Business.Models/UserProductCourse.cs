using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Product course per user
    /// </summary>
    public class UserProductCourse
    {
        /// <summary>
        /// Course id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Course name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current user role in this course
        /// </summary>
        public Role CurrentRole { get; set; }

        private IList<Role> availableRoles { get; set; } 

        /// <summary>
        /// List of available roles for this course
        /// </summary>
        public IList<Role> AvailableRoles {
            get
            {
                if (availableRoles == null)
                {
                    availableRoles = new List<Role>();
                }
                return availableRoles;
            }
            set
            {
                availableRoles = value;
            } 
        }

        /// <summary>
        /// Indicates if user can set roles for that course
        /// </summary>
        public bool CanSetRoles { get; set; }
    }
}