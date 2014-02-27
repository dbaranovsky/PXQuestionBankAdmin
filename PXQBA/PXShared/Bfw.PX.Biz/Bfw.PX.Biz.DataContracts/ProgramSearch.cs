using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class ProgramSerach
    {
        public string ProgramId { get; set; }

        public string ProductCourseId { get; set; }

        public string DashboardId { get; set; }

        public string ManagerId { get; set; }

        public string ManagerRefId { get; set; }

        public string ManagerDomainId { get; set; }
    }

    public class UserProgramSerach
    {
        public string ProgramId { get; set; }

        public string ProductCourseId { get; set; }

        public string PMDashboardId { get; set; }

        public string ManagerId { get; set; }

        public string ManagerRefId { get; set; }

        public string ManagerDomainId { get; set; }

        public string UserId { get; set; }

        public string UserRefId { get; set; }

        public string UserDomainId { get; set; }
    }
}
