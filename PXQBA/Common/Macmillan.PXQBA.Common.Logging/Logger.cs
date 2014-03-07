using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Common.Logging;

namespace Macmillan.PXQBA.Common.Logging
{
    public class Logger : ILogger
    {
        public string CorrelationId { get; set; }
        public void Log(string message, LogSeverity severity)
        {
            //TODO:throw new NotImplementedException();
        }

        public void Log(string message, LogSeverity severity, IList<string> categories)
        {
            //TODO:throw new NotImplementedException();
        }

        public void Log(Exception ex)
        {
            //TODO:throw new NotImplementedException();
        }

        public void Log(Exception ex, LogSeverity severity)
        {
            //TODO:throw new NotImplementedException();
        }

        public void Log(Exception ex, LogSeverity severity, IList<string> categories)
        {
            //TODO:throw new NotImplementedException();
        }

        public void Log(LogMessage message)
        {
            //TODO:throw new NotImplementedException();
        }

        public bool ShouldLog(params string[] categories)
        {
            return true;
            //TODO:throw new NotImplementedException();
        }
    }
}
