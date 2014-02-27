using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class XBook
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssignmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DisplayAssignment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DisplayHideShow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Params { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
    }
}
