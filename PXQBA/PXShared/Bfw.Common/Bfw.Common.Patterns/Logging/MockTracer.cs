using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Logging;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging {
    /// <summary>
    /// Dummy class for Tracer
    /// </summary>
   public class MockTracer : ITraceHandle {
       #region IDisposable Members
       /// <summary>
       /// Dispose function
       /// </summary>
       public void Dispose() {
           
       }

       #endregion
   }
}
