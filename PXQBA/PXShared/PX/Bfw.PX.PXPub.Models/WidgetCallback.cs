using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class MenuItemCallback
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
        /// Target dom element to place response
        /// </summary>        
        public string Target { get; set; }

        /// <summary>
        /// FNE value of callback
        /// </summary>       
        public string LinkType { get; set; }

        /// <summary>
        /// Route Name
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// URL name
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// CallbackUrl name
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// In cases where you want to override the default menuitem behavior for a student
        /// </summary>
        public string StudentOverride { get; set; }

        /// <summary>
        /// In cases where you want to override the default menuitem behavior for an instructor
        /// </summary>
        public string InstructorOverride { get; set; }

      /// <summary>
      /// call back params
      /// </summary>
        public IDictionary<string, string> Parameters { get; set; }

        public MenuItemCallback()
        {
            Parameters = new Dictionary<string, string>();
        }
    }
}
