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
    public interface IContentService
    {
        [OperationContract]
        void CopyContent(string entityId, string parentId, string category, string contentType, string contentSubType, string moveToEntityId, bool moveToParent, Int64 processId);
    }
}
