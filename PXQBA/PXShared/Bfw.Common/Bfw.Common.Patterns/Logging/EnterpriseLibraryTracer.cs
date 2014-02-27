using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Logging;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging
{
    /// <summary>
    /// Wraps a Tracer object and exposes it as an ITraceHandle
    /// </summary>
    public class EnterpriseLibraryTracer : ITraceHandle
    {
        #region Properties

        private bool disposed = false;

        /// <summary>
        /// Tracer object that is being wrapped
        /// </summary>
        protected Tracer WrappedTracer { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrapped">Tracer which will be wrapped</param>
        public EnterpriseLibraryTracer(Tracer wrapped)
        {
            WrappedTracer = wrapped;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes any resources occupied by the object
        /// </summary>
        public void Dispose()
        {
            if (!disposed && WrappedTracer != null)
            {
                WrappedTracer.Dispose();
                WrappedTracer = null;
                disposed = true;
            }
        }

        #endregion
    }
}