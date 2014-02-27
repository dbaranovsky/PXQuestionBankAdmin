using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// Provides convenient helper methods for uploaded files.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Returns CssClass name for UI file Icons from File Type
        /// </summary>
        /// <param name="FileName">Name of Uploaded file</param>      
        /// <returns>CssClass Name for File icons</returns>
        public static string GetCSSClassName(string FileName)
        {
            string CSSClassName = "default";            
            if (!string.IsNullOrEmpty(FileName) && FileName.Split('.').Count() > 1)
            {
                CSSClassName = FileName.Split('.').Last().ToString();
            }
            return CSSClassName;
        }
    }
}
