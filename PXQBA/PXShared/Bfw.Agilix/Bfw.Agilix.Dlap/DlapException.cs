using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Base exception type for all APIs that interact with DLAP
    /// </summary>
    public class DlapException : Exception
    {
        /// <summary>
        /// Inherited from <see cref="System.Exception" />
        /// </summary>
        /// <param name="message">Message body of the exception</param>
        public DlapException(string message) : base(message) { }
    }
}
