using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class LogSummaryModel
    {
        public string Severity { get; set; }
        public string AvgDay { get; set; }
        public string AvgWeek { get; set; }
        public string TotalToday { get; set; }
        public string Total { get; set; }
    }

    public class LogSummaryViewModel
    {
        public PXEnvironment Environment { get; set; }
        public List<LogSummaryModel> LogSummary { get; set; }
    }
}
