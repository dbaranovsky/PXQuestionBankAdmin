using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.PX.PXPub.Controllers
{
    [Serializable]
    public class MappingDataMissingException : Exception
    {
        public MappingDataMissingException()
            : base() { }
        public MappingDataMissingException(string message)
            : base(message) { }
        public MappingDataMissingException(string message, Exception innerException)
            : base(message, innerException) { }
        public MappingDataMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}