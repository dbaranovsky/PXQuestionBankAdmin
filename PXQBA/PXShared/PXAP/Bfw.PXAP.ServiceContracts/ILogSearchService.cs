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
    public interface ILogSearchService
    {
        [OperationContract]
        List<LogModel> GetLogs(string sSeverity, string sCategoryName, string sSource, string sMessage, string dtStartDate, string dtEndDate);

         [OperationContract]
        LogSearchModel GetLogSearchModel(PXEnvironment currentEnv);

         [OperationContract]
         string GetErrorMessage(int logID);
    }
}
