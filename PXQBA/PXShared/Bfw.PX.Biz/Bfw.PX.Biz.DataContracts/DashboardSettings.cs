using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class DashboardSettings
    {
        /// <summary>
        /// True if Instructor Dashboard is on for this course
        /// </summary>
        public bool IsInstructorDashboardOn { get; set; }
        /// <summary>
        /// True if Program Dashboard is on for this course
        /// </summary>
        public bool IsProgramDashboardOn { get; set; }
        /// <summary>
        /// Dashboard
        /// </summary>
        public string DashboardHomePageStart { get; set; }
        /// <summary>
        /// Dashboard
        /// </summary>
        public string ProgramDashboardHomePageStart { get; set; }

    }
}
