using System.Collections.Generic;
using System.Web;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    /// <summary>
    /// Helper to manage user capabilities
    /// </summary>
    public class UserCapabilitiesHelper
    {
        private const string CapabilitiesParamName = "user_capabilities";
        private readonly IUserManagementService userManagementService;

        public UserCapabilitiesHelper(IUserManagementService userManagementService)
        {
            this.userManagementService = userManagementService;
        }

        /// <summary>
        /// Gets list of user capabilities for current course
        /// </summary>
        /// <param name="courseId">Course id</param>
        /// <returns>List of capabilities</returns>
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

        /// <summary>
        /// Clears capabilities list in session
        /// </summary>
        public void ClearCache()
        {
            HttpContext.Current.Session[CapabilitiesParamName] = null;
        }
    }

  
    /// <summary>
    /// Represents container for user capabilities
    /// </summary>
    class UserCapabilitiesHelperSessionContainer
    {
        /// <summary>
        /// Course id capabilities belong to
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// List of capabilities
        /// </summary>
        public IEnumerable<Capability> Capabilities { get; set; }
    }
}