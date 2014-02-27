using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    /// <summary>
    /// Used to satisfy the requirements of the using statement, but doesn't represent any
    /// actual resources or trace information.
    /// </summary>
    public class NoOpTraceHandle : ITraceHandle
    {
        #region IDisposable Members

        /// <summary>
        /// Dispose Method 
        /// </summary>
        public void Dispose()
        {            
        }

        #endregion
    }
}
