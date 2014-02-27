using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class WidgetInputHelper
    {
        /// <summary>
        /// Name of the current input helper
        /// This also the the variable name that has to be posted
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// selector of the element to retrieve the value from
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// Default value to be used if the required element is not found 
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// A flag which indicates whether to use the default value
        /// </summary>
        public bool UseDefaultValue { get; set; }
    }
}
