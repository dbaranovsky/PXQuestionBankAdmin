using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Exceptions
{
    /// <summary>
    /// Thrown when a new user is not found in any domains.
    /// </summary>
    public class DomainNotFoundException : PXException
    {
        public DomainNotFoundException() : base()
        {
            PXExceptionType = "DomainNotFoundException";
        }

        public DomainNotFoundException(string message) : base(message)
        {
            PXExceptionType = "DomainNotFoundException";
        }

        public DomainNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            PXExceptionType = "DomainNotFoundException";
        }
    }

    

}
