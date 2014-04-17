using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web
{
    public class FilterFieldDescriptor
    {
        public string Field { get; set; }

        public IEnumerable<string> Values { get; set; }

    }
}
