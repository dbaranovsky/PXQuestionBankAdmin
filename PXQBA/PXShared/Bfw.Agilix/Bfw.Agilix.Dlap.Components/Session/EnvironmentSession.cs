using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Components.Session
{
    internal class EnvironmentSession : ThreadSession
    {
        protected string Environment { get; set; }

        public EnvironmentSession(string environment, DlapConnection connection, Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
            : base(connection, logger, tracer)
        {
            Environment = environment;
        }

        public override void ExecuteAsAdmin(Dlap.Session.DlapCommand command)
        {
            base.ExecuteAsAdmin(Environment.ToLowerInvariant(), command);
        }
    }
}
