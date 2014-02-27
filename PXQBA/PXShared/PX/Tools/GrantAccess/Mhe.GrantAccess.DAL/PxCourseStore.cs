using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;

using Mhe.GrantAccess.DAL.Contracts;

namespace Mhe.GrantAccess.DAL
{
    public class PxCourseStore : ICourseStore
    {
        protected ISessionManager SessionManager { get; set; }

        public PxCourseStore(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public void SetupAdminToolSandboxCourse(string forCourse, string inDomain)
        {
            var courseId = forCourse;

            var exists = AdminToolSandboxExists(forCourse);

            if (!exists)
            {
                CreateAdminToolSandbox(forCourse, inDomain);
            }
        }

        private bool AdminToolSandboxExists(string forCourse)
        {
            var courseId = forCourse;

            var cmd = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    Query = string.Format("/meta-bfw_is_sandbox_course='true' AND /meta-bfw_course_type='FACEPLATE' AND /sourceid='{0}'", courseId)
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            return cmd.Courses.Count() > 0;
        }

        private void CreateAdminToolSandbox(string forCourse, string inDomain)
        {
            var getProductCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = forCourse
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(getProductCourse);

            var copyCourse = new CopyCourses
            {
                Method = "derivative"
            };
            var productCourse = getProductCourse.Courses.First();
            productCourse.Domain.Id = inDomain;
            copyCourse.Add(productCourse);

            SessionManager.CurrentSession.ExecuteAsAdmin(copyCourse);

            var copy = copyCourse.Courses.First();

            var updateCopy = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "updatecourses" } }
            };

            var element = new XElement("course");

            if (!string.IsNullOrEmpty(copy.Id))
                element.Add(new XAttribute("courseid", copy.Id));

            var dataElement = new XElement("data");
            dataElement.Add(new XElement("meta-bfw_is_sandbox_course", true));

            element.Add(dataElement);

            updateCopy.AppendData(element);

            SessionManager.CurrentSession.Send(updateCopy, true);
        }
    }
}
