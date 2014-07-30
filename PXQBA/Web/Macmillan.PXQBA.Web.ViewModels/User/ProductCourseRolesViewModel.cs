using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    /// <summary>
    /// Product course role
    /// </summary>
    public class ProductCourseRolesViewModel
    {
        /// <summary>
        /// Product course id
        /// </summary>
        public string ProductCourseId { get; set; }

        /// <summary>
        /// Product course name
        /// </summary>
        public string ProductCourseName { get; set; }

        /// <summary>
        /// Current user role for this course
        /// </summary>
        public RoleViewModel CurrentRole { get; set; }

        /// <summary>
        /// List of available roles for this course
        /// </summary>
        public Dictionary<string, string> AvailibleRoles { get; set; } 
    }
}
