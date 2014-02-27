using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class DashboardModel
    {
        public DashboardModel()
        {
            LogSummary = new LogSummaryViewModel();
            LogSummary.LogSummary = new List<LogSummaryModel>();
        }

        public LogSummaryViewModel LogSummary { get; set; }
    }
}
