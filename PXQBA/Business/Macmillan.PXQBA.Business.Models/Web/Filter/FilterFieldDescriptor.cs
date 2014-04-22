using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web.Filter
{
    public class FilterFieldDescriptor
    {

        public FilterFieldDescriptor()
        {
            Values = new List<string>();
        }

        public string Field { get; set; }

        public IEnumerable<string> Values { get; set; }

    }
}
