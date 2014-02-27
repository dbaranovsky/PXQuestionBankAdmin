using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Defines the course academic term
    /// </summary>
    public class CourseAcademicTerm
    {
        /// <summary>
        /// name of the term
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// unique id for the term
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// term start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// term end date.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
    
}