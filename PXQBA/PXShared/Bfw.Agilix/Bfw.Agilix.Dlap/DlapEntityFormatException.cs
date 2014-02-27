using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Indicates that an entity couldn't be parsed or transformed because it was missing data or badly formatted
    /// </summary>
    public class DlapEntityFormatException : DlapException
    {
        /// <summary>
        /// Inherited from <see cref="System.Exception" />
        /// </summary>
        /// <param name="message">Message body of the exception</param>
        public DlapEntityFormatException(string message) : base(message) { }
    }
}
