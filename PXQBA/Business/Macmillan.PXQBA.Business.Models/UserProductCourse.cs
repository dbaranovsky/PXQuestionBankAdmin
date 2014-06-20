using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class UserProductCourse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Role CurrentRole { get; set; }

        private IList<Role> availableRoles { get; set; } 
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
    }
}