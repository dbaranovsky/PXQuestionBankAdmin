using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Runtime.Serialization;

namespace UploadResources.Exception
{
    [Serializable]
    public class DALException : System.Exception
    {
        public DALException()
        {
        }

        public DALException(string message)
            : base(message)
        {
        }

        public DALException(string message, System.Exception ex)
            : base(message, ex)
        {
        }

        protected DALException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}