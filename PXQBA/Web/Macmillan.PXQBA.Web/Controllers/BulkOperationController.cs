using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.Helpers;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class BulkOperationController : MasterController
    {
        private readonly IBulkOperationService bulkOperationService;

        public BulkOperationController(IBulkOperationService bulkOperationService)
        {
            this.bulkOperationService = bulkOperationService;
        }


	}
}