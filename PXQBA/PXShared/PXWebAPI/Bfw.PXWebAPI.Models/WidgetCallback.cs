using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class WidgetCallback
    {
        /// <summary>
        /// Name of the callback
        /// </summary>
        
        public string Name { get; set; }

        /// <summary>
        /// Controller for callback
        /// </summary>
        
        public string Controller { get; set; }

        /// <summary>
        /// Action for callback
        /// </summary>
       
        public string Action { get; set; }

        /// <summary>
        /// FNE value of callback
        /// </summary>
       
        public bool IsFNE { get; set; }

        /// <summary>
        /// Load using ASync methodologies
        /// </summary>
       
        public bool IsASync { get; set; }
    }
}
