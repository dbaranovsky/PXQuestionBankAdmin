using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents the dashboard settings.
    /// </summary>
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
        /// Dashboard Zone ID 
        /// </summary>
        public string DashboardHomePageStart { get; set; }
        /// <summary>
        /// Program Dashboard Zone ID
        /// </summary>
        public string ProgramDashboardHomePageStart { get; set; }
    }


     
}
