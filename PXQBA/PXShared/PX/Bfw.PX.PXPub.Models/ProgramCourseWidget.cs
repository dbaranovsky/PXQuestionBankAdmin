using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class ProgramCourseWidget
    {
        public List<CourseAcademicTerm> AcademicTerm { get; set; }

        public string CurrentAcademicTerm { get; set; }
    }
}
