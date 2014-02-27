using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DataContracts;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Mhe.GrantAccess.ServiceContracts
{
    public interface IAccessService
    {
        AccessResponse Grant(Access access, string to, string forCourse, string inDomain);
    }
}
