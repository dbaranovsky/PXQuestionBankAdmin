using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class GradeData
    {
        public List<Grade> ItemGrades { get; set; }

        public List<Grade> StudentGrades { get; set; }

        public double ClassAverage { get; set; }
    }
}
