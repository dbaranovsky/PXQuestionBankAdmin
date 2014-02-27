using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Indicates that the user credentials supplied are invalid
    /// </summary>
    public class DlapAuthenticationException : DlapException
    {
        /// <summary>
        /// Inherited from <see cref="System.Exception" />
        /// </summary>
        /// <param name="message">Message body of the exception</param>
        public DlapAuthenticationException(string message) : base(message) { }
    }
}
