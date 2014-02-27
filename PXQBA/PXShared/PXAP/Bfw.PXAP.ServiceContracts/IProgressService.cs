using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Bfw.PXAP.Models;

namespace Bfw.PXAP.ServiceContracts
{
    [ServiceContract]
    public interface IProgressService
    {
        [OperationContract]
        ProgressModel GetProgress(Int64 processId);

        [OperationContract]
        Int64 AddUpdateProcess(ProgressModel process, out string Message);
    }


}
