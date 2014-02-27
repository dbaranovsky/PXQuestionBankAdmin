using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Web.Mvc;
using Bfw.PXAP.Models;

namespace Bfw.PXAP.ServiceContracts
{
    [ServiceContract]
    public interface IDashboardService
    {
        [OperationContract]
        List<LogSummaryModel> GetLogSummary(string source);
    }
}
