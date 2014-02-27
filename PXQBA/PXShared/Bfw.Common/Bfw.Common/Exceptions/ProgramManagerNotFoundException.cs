using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Exceptions
{
    /// <summary>
    /// Thrown when a e-Portfolio program manager is not found in current user's domain.
    /// </summary>
    public class ProgramManagerNotFoundException : PXException
    {
        public ProgramManagerNotFoundException()
            : base()
        {
            PXExceptionType = "ProgramManagerNotFoundException";
        }

        public ProgramManagerNotFoundException(string message)
            : base(message)
        {
            PXExceptionType = "ProgramManagerNotFoundException";
        }

        public ProgramManagerNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            PXExceptionType = "ProgramManagerNotFoundException";
        }
    }
}
