using System.Collections.Generic;
using System.Web;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class UserCapabilitiesHelper
    {
        private const string CapabilitiesParamName = "user_capabilities";
        private readonly IUserManagementService userManagementService;

        public UserCapabilitiesHelper(IUserManagementService userManagementService)
        {
            this.userManagementService = userManagementService;
        }

        public IEnumerable<Capability> GetCapabilities(string courseId)
        {
            var container = HttpContext.Current.Session[CapabilitiesParamName] as UserCapabilitiesHelperSessionContainer;
            if (container != null)
            {
                if (container.CourseId == courseId)
                {
                    return container.Capabilities;
                }
            }

            container = new UserCapabilitiesHelperSessionContainer()
                        {
                            CourseId = courseId,
                            Capabilities = userManagementService.GetUserCapabilities(courseId)
                        };

            HttpContext.Current.Session[CapabilitiesParamName] = container;

            return container.Capabilities;
        }
    }
  
    class UserCapabilitiesHelperSessionContainer
    {
        public string CourseId { get; set; }
        public IEnumerable<Capability> Capabilities { get; set; }
    }
}