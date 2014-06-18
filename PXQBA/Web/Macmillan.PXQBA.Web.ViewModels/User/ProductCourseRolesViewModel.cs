using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    public class ProductCourseRolesViewModel
    {
        public string ProductCourseId { get; set; }
        public string ProductCourseName { get; set; }

        public RoleViewModel CurrentRole { get; set; }

        public Dictionary<string, string> AvailibleRoles { get; set; } 
    }
}
