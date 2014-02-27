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
    public interface    IEnvironmentService
    {
        [OperationContract]
        int AddUpdateEnvironment(PXEnvironment env, out string message);

        [OperationContract]
        bool DeleteEnvironment(int envId);

        [OperationContract]
        List<PXEnvironment> GetEnvironments();

        void ChangeCachingConfiguration(string environment);
    }
}
