using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Logging
{
    public class NullTraceManager : ITraceManager
    {
        public void StartTracing()
        {
        }

        public void EndTracing()
        {
        }

        public ITraceHandle StartTrace(string message)
        {
            return new NoOpTraceHandle();
        }
    }
}
