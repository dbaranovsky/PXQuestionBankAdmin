using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    public class FilterFieldDescriptor
    {

        public FilterFieldDescriptor()
        {
            Values = new List<string>();
        }

        public string Field { get; set; }

        public IEnumerable<string> Values { get; set; }

        public FilterFieldDescriptor Clone()
        {
            return new FilterFieldDescriptor {Field = Field, Values = new List<string>(Values.Select(item => item))};
        }
    }
}
