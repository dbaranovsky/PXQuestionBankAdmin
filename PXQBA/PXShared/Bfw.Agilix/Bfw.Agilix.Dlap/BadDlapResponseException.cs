using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Format of the DLAP response as received was NOT what was expected
    /// </summary>
    public class BadDlapResponseException : DlapException
    {
        /// <summary>
        /// Inherited from <see cref="System.Exception" />
        /// </summary>
        /// <param name="message">Message body of the exception</param>
        public BadDlapResponseException(string message) : base(message) { }
    }
}
