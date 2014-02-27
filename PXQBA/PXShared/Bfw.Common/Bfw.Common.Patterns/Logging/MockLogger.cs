using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Logging;

using Bfw.Common.Logging;

namespace Bfw.Common.Patterns.Logging {
    public class MockLogger : LoggerBase, IDisposable{
       
        public override void Log(LogMessage message) {
           
        }

        public override ITraceHandle StartTrace(string category) {
            return new MockTracer();
        }

        public override void Dispose() {
           
        }

        #region IDisposable Members

        void IDisposable.Dispose() {
           
        }

        #endregion
    }
}
