using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Bfw.PXAP.Models;
using Bfw.Agilix.DataContracts;

namespace Bfw.PXAP.ServiceContracts
{
    [ServiceContract]
    public interface IEnrollmentService
    {
        [OperationContract]
        void EnrollStudent(string entityId, int studentCount, Int64 processId);
    }
}
