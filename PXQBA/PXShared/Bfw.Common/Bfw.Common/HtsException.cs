using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.HtsConversion
{
    /// <summary>
    /// An exception for HTS errors.
    /// </summary>
    public class HtsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public HtsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HtsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
