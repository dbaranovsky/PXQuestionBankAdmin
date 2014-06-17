using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.User
{
    public class TitleRolesViewModel
    {
        public string TitleId { get; set; }
        public string TitleName { get; set; }

        public RoleViewModel CurrentRole { get; set; }

        public Dictionary<string, string> AvailibleRoles { get; set; } 
    }
}
