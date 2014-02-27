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
    public interface IDlapService
    {
        [OperationContract]
        string RunCommand(string command,  ref string entityid, DlapCommandModel.HttpMethod httpMethod, string postdata = "");

        [OperationContract]
        DlapCommandModel ConvertGetToPost(DlapCommandModel command, string commandName = "putitems");
    }
}
