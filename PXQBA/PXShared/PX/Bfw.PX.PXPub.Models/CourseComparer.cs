using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class CourseComparer : IEqualityComparer<Course>
    {
        public bool Equals(Course x, Course y)
        {
            return x.Id.Equals(y.Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Course obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
