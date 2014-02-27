using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mhe.GrantAccess.DAL.Models;
using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DataContracts;
using Mhe.GrantAccess.ServiceContracts;

namespace Mhe.GrantAccess.Services
{
    public class AccessService : IAccessService
    {
        protected IUserStore UserStore { get; set; }

        protected IRightsStore RightsStore { get; set; }

        protected ICourseStore CourseStore { get; set; }

        public AccessService(IUserStore userStore, IRightsStore rightsStore, ICourseStore courseStore)
        {
            UserStore = userStore;
            RightsStore = rightsStore;
            CourseStore = courseStore;
        }

        public AccessResponse Grant(Access access, string to, string forCourse, string inDomain)
        {
            var userEmail = to;
            var courseId = forCourse;

            var response = new AccessResponse
            {
                Error = false
            };

            try
            {
                var referenceId = UserStore.UserReferenceId(forEmail: userEmail);
                RightsStore.StoreRights(new PxWebUserRights
                {
                    UserId = referenceId,
                    CourseId = forCourse,
                    Rights = (long)access,
                    Component = PxWebRights.AdminTool
                });
                CourseStore.SetupAdminToolSandboxCourse(forCourse: courseId, inDomain: inDomain);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Message = ex.Message;
            }

            return response;
        }        
    }
}
