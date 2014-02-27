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
    public interface IMetadataService
    {
        [OperationContract]
        void AddMetadata(string entityId, string parentId, string xmlField, bool bExact, string sValue, Int64 processId, string parentCategory, bool recursive);


    }
}
