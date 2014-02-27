using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Exceptions
{
    /// <summary>
    /// Base class for PX custom exceptions
    /// </summary>
    public class PXException : Exception
    {
        public string PXExceptionType;

        public PXException()
            : base()
        {
        }

        public PXException(string message)
            : base(message)
        {
        }

        public PXException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
