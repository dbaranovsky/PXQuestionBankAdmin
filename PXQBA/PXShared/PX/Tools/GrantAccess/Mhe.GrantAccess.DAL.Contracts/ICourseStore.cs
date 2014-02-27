using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mhe.GrantAccess.DAL.Contracts
{
    public interface ICourseStore
    {
        void SetupAdminToolSandboxCourse(string forCourse, string inDomain);
    }
}
