using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Acts as a null logger. Messages sent to this logger will not be written anywhere.
    /// </summary>
    public class NullLogger : LoggerBase
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="message">Message that will not be logged.</param>
        public override void Log(LogMessage message)
        {            
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="categories">Categories, which will be ignored.</param>
        /// <returns>False in all cases.</returns>
        public override bool ShouldLog(params string[] categories)
        {
            return false;
        }
    }
}
