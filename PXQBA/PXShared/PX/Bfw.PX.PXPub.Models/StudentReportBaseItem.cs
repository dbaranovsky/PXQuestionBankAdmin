using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class StudentReportBaseItem
    {
        public string CourseId { get; set; }

        public string EnrollmentId { get; set; }

        public int AssignmentCompleted { get; set; }

        public int ReviewedSubmissions { get; set; }

        public double PointsEarned { get; set; }

        public double PointsPossible { get; set; }
    }

}
