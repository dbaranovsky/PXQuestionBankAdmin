using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class ItemGradeData
    {
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string Average { get; set; }
        public string GradeRule { get; set; }
        public IEnumerable<Grade> Grades { get; set; }
    }
}
