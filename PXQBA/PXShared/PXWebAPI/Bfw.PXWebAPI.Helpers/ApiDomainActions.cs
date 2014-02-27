using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Adc = Bfw.Agilix.DataContracts;
using System.Configuration;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiDomainActions
	/// </summary>
	public interface IApiDomainActions
	{
		/// <summary>
		/// GetDomainById
		/// </summary>
		/// <param name="domainId"></param>
		/// <returns></returns>
		Adc.Domain GetDomainById(string domainId);

		/// <summary>
		/// GetDomainByName
		/// </summary>
		/// <param name="domainName"></param>
		/// <returns></returns>
		List<Adc.Domain> GetDomainByName(string domainName);

	    /// <summary>
	    /// CreateDomain
	    /// </summary>
	    /// <param name="domainName"></param>
        /// <param name="reference"></param>
	    /// <returns></returns>
        Adc.Domain CreateDomain(string domainName, string reference);

        /// <summary>
        /// GetDomainList
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        Adc.Domain GetDomainList(string reference);
    }


	/// <summary>
	/// ApiDomainActions
	/// </summary>
	public class ApiDomainActions : IApiDomainActions
	{
		#region Properties

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiDomainActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
		public ApiDomainActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context)
		{
			SessionManager = sessionManager;
			Context = context;
		}
		#endregion

		/// <summary>
		/// Gets a domain details.
		/// </summary>
		/// <param name="domainId">The domain id.</param>
		/// <returns></returns>
		public Adc.Domain GetDomainById(string domainId)
		{
			Adc.Domain result = null;
			var cmd = new GetDomain
			{
				DomainId = domainId
			};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			if (cmd.Domain != null)
			{
				result = cmd.Domain;
			}
			return result;
		}

		/// <summary>
		/// Gets domains by name.
		/// </summary>
		/// <param name="domainName">The domain name.</param>
		/// <returns></returns>
		public List<Adc.Domain> GetDomainByName(string domainName)
		{
			List<Adc.Domain> result = null;
			var cmd = new GetDomainList
			{

				SearchParameters = new Adc.Domain()
				{
					Name = domainName
				}
			};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			if (!cmd.Domains.IsNullOrEmpty())
			{
				result = cmd.Domains.ToList();
			}
			return result;
		}

        /// <summary>
        /// Gets domains by name.
        /// </summary>
        /// <param name="domainName">The domain name.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public Adc.Domain CreateDomain(string domainName, string reference)
        {
            Adc.Domain result = null;
            Adc.Domain parentDomain = GetDomainById(ConfigurationManager.AppSettings["DlapCopySourceDomainId"]);

            var domain12 = new Adc.Domain()
            {
                Name = domainName,
                Userspace = ConfigurationManager.AppSettings["Onyx"] + reference,
                Reference = reference,
                Data = new XElement("data")
            };
            domain12.Data.Add(parentDomain.Data.Element("customization"));

            var cmd = new CreateDomains(ConfigurationManager.AppSettings["DlapDomainParentId"])
            {
                Domain = domain12
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            if (cmd.Domain != null)
            {
                result = cmd.Domain;
            }
            return result;
        }

        /// <summary>
        /// Gets domain by Reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public Adc.Domain GetDomainList(string reference)
	    {
            List<Adc.Domain> result = null;
            var cmd = new GetDomainList
            {
                SearchParameters = new Adc.Domain()
                {
                    Reference = reference
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            if (!cmd.Domains.IsNullOrEmpty())
            {
                result = cmd.Domains.ToList();
            }
            return result.FirstOrDefault();
        }

	}
}
