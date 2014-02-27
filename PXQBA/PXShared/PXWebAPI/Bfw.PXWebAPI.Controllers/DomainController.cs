using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Helpers.Context;
using Bfw.PXWebAPI.Helpers.Services;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.PXWebAPI.Models.Response;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// DomainController
	/// </summary>
	public class DomainController : ApiController
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected Helpers.IApiDomainActions ApiDomainActions { get; set; }

        ///// <summary>
        ///// Wrapper for the HttpContext.Current object, so it can be mocked
        ///// </summary>
        protected IHttpContextAdapter HttpContextAdapter { get; set; }

		/// <summary>
		/// DomainController
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		/// <param name="apiDomainActions"> </param>
		public DomainController(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context,
                                    Helpers.IApiDomainActions apiDomainActions, IHttpContextAdapter httpContextAdapter)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiDomainActions = apiDomainActions;
            HttpContextAdapter = httpContextAdapter;
		}

		/// <summary>
		/// Get Domain Details by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>DomainResponse</returns>
		[HttpGet]
		[ActionName("Details")]
		public DomainResponse Details(string id)
		{
			var response = new DomainResponse();

			var domainActions = ApiDomainActions;
			var domain = domainActions.GetDomainById(id);
			if (domain != null)
			{
				var domainDto = domain.ToDomainDto();
				response.results = domainDto;
				return response;
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Get Domains By Name
		/// </summary>
		/// <param name="name"></param>
		/// <returns>DomainListResponse</returns>
		[HttpGet]
		[ActionName("Domains")]
		public DomainListResponse GetDomainsByName(string name)
		{
			var response = new DomainListResponse();

			var domainActions = ApiDomainActions;
			var domains = domainActions.GetDomainByName(name);
			if (!domains.IsNullOrEmpty())
			{
				var domainDtoList = domains.Select(domain => domain.ToDomainDto()).ToList();
				response.results = domainDtoList;
				return response;
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}

	    /// <summary>
	    /// Check and Create Domain By Institution using reference
	    /// The following POST parameter is required: domainname 
	    /// </summary>
        /// <param name="id">Reference</param>
        /// <param name="domainName">Domain Name</param>
	    /// <returns>DomainResponse</returns>
	    [HttpPost]
	    [ActionName("CheckandCreateDomainByInstitution")]
        public DomainResponse CheckandCreateDomainByInstitution(string id)
	    {
            var domainActions = ApiDomainActions;
            var response = new DomainResponse();

            //Check by id if it already exists
            Agilix.DataContracts.Domain domainByReference = null;
	        try
	        {
	            if (!String.IsNullOrWhiteSpace(id) && id != "0")
	            {
	                domainByReference = domainActions.GetDomainList(id);
	            }
	        }
	        catch (Exception)
	        {
	        }
            if (domainByReference != null)
            {
                response.results = domainByReference.ToDomainDto();
                return response;
            }

            //Check by name if it already exists
            //var domainName = ApiHelper.GetFormRequestParameter("domainname");
            var domainName = HttpContextAdapter.Current.Request.Form["domainname"];
            if (String.IsNullOrEmpty(domainName))
            {
                response.error_message = "Required parameter, domainname, is missing";
                return response;
            }
            
            //Create domain if it doesn't exist
            var domainCreated = domainActions.CreateDomain(domainName, id);
            if (domainCreated != null)
            {
                response.results = domainCreated.ToDomainDto();
                return response;
            }

            response.error_message = "No results found";
            return response;
        }

	}
}
