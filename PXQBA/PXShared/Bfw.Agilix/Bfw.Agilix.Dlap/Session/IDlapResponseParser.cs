using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors are able to parse a DlapResponse into internal object state
    /// </summary>
    public interface IDlapResponseParser
    {
        /// <summary>
        /// Provides the implementor with the response to parse
        /// </summary>
        /// <param name="response">DlapResponse to parse into object state</param>
        void ParseResponse(DlapResponse response);
    }
}
