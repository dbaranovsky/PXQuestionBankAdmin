using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Descriptor of field in the filter
    /// </summary>
    public class FilterFieldDescriptor
    {

        public FilterFieldDescriptor()
        {
            Values = new List<string>();
        }

        /// <summary>
        /// Field name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Selected values for filter field
        /// </summary>
        public IEnumerable<string> Values { get; set; }

        /// <summary>
        /// Clones filter field descriptor
        /// </summary>
        /// <returns>Cloned filter field descriptor</returns>
        public FilterFieldDescriptor Clone()
        {
            return new FilterFieldDescriptor {Field = Field, Values = new List<string>(Values.Select(item => item))};
        }
    }
}
