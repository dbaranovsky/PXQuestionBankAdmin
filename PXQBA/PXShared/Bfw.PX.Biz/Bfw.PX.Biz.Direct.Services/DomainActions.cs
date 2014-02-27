// -----------------------------------------------------------------------
// <copyright file="DomainActions.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Bfw.Common.Caching;

namespace Bfw.PX.Biz.Direct.Services
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Xml.Linq;
    using Bfw.Agilix.Commands;
    using Bfw.Agilix.Dlap.Session;
    using Bfw.Common.Collections;
    using Bfw.Common.Logging;
    using Bfw.PX.Biz.DataContracts;
    using Bfw.PX.Biz.ServiceContracts;
    using Bfw.PX.Biz.Services.Mappers;
    using Adc = Bfw.Agilix.DataContracts;
    using Bdc = Bfw.PX.Biz.DataContracts;

    /// <summary>
    /// DLAP Domain related actions
    /// </summary>
    public class DomainActions : IDomainActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Cache provide
        /// </summary>
        public ICacheProvider CacheProvider { get; set; }

        /// <summary>
        /// A const for that holds the Generic Domain Name.
        /// </summary>
        public readonly string GENERIC_DOMAIN_NAME = System.Configuration.ConfigurationManager.AppSettings["GenericCourseDomain"];

        /// <summary>
        /// A const for that holds the Generic Domain Parent Name.
        /// </summary>
        public readonly string GENERIC_DOMAIN_PARENT_NAME = System.Configuration.ConfigurationManager.AppSettings["GenericCourseParentDomain"];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        public DomainActions(IBusinessContext context, ISessionManager sessionManager, ICacheProvider cacheProvider)
        {
            this.Context = context;
            this.SessionManager = sessionManager;
            this.CacheProvider = cacheProvider;

        }

        #endregion

        /// <summary>
        /// Get Domain details for domain title name. It takes first matching domain title.
        /// </summary>
        /// <param name="domainName">Domain title name to search</param>
        /// <returns>Domain object</returns>
        public Domain GetDomain(string domainName)
        {
            Bdc.Domain result = CacheProvider.FetchDomainByName(domainName); ; ;

            if (result == null)
            {
                using (Context.Tracer.DoTrace("DomainActions.GetDomain(domainName={0})", domainName))
                {
                    var cmd = new GetDomainList()
                    {
                        SearchParameters = new Adc.Domain()
                        {
                            Name = domainName
                        }
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                    if (!cmd.Domains.IsNullOrEmpty() && cmd.Domains.Count() > 0)
                    {
                        result = cmd.Domains.FirstOrDefault().ToDomain();
                        CacheProvider.StoreDomain(result.Id, result);
                    }
                }
            }
            return result;
        }

        public Domain GetDomainById(string domainId)
        {
            Bdc.Domain result = CacheProvider.FetchDomain(domainId);;

            if (result == null)
            {
                using (Context.Tracer.DoTrace("DomainActions.GetDomain(domainName={0})", domainId))
                {
                    var cmd = new GetDomain()
                    {
                        DomainId = domainId
                    };

                    var dlapResonse = SessionManager.CurrentSession.Send(cmd.ToRequest(), true);
                    if (dlapResonse != null && dlapResonse.Code == Agilix.Dlap.DlapResponseCode.OK)
                    {
                        cmd.ParseResponse(dlapResonse);
                    }

                    if (cmd.Domain != null)
                    {
                        result = cmd.Domain.ToDomain();
                        CacheProvider.StoreDomain(domainId, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create domain
        /// </summary>
        /// <param name="parentId">Parent Id for domain to be created</param>
        /// <param name="domainName">Name of domain</param>
        /// <param name="domainUserSpace">Userspace of domain</param>
        /// <param name="reference">External reference for domain (optional)</param>
        /// <param name="Data">Any data for current domain (optional)</param>
        /// <returns></returns>
        public Bdc.Domain CreateDomain(string parentId, string domainName, string domainUserSpace, string reference, XElement Data)
        {
            Bdc.Domain result = null;

            using (Context.Tracer.DoTrace("DomainActions.GetDomain(domainName={0}, parentId={1})", domainName, parentId))
            {
                var cmd = new CreateDomains(parentId)
                {
                    Domain = new Adc.Domain()
                    {
                        Name = domainName,
                        Userspace = domainUserSpace,
                        Reference = reference,
                        Data = Data
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (cmd.Domain != null)
                {
                    result = new Bdc.Domain()
                    {
                        Id = cmd.Domain.Id,
                        Name = cmd.Domain.Name,
                        Userspace = cmd.Domain.Userspace,
                        Data = cmd.Domain.Data,
                        Reference = cmd.Domain.Reference
                    };
                }
            }

            return result;
        }

        public bool CheckDomainExists(string domainId, string domainName)
        {
            using (Context.Tracer.DoTrace("DomainActions.CheckDomainExists(domainId={0}, domainName={1})", domainId, domainName))
            {

                var cmd = new GetDomainList()
                {
                    SearchParameters = new Adc.Domain()
                    {
                        Id = domainId
                    }
                };

                if (!string.IsNullOrEmpty(domainName))
                {
                    cmd.SearchParameters.Name = domainName;
                }

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (cmd.Domains.Count() == 0)
                {
                    return false;
                }

                return true;
            }
        }

        public string GetGenericDomainId()
        {
            var domain = GetDomain(this.GENERIC_DOMAIN_NAME);
            if (domain != null)
            {
                return GetDomain(this.GENERIC_DOMAIN_NAME).Id;
            }
            else
            {
                domain = GetDomain(this.GENERIC_DOMAIN_PARENT_NAME);

                if (domain == null)
                {
                    throw new Exception(string.Format("Could not found Parent Generic Domain - {0}", this.GENERIC_DOMAIN_PARENT_NAME));
                }

                string ParentDomainId = domain.Id;
                domain = CreateDomain(ParentDomainId, this.GENERIC_DOMAIN_NAME, this.GENERIC_DOMAIN_NAME, string.Empty, GetNewDomainData());

                if (domain == null)
                {
                    throw new Exception(string.Format("Could not create Generic Domain - {0}", this.GENERIC_DOMAIN_NAME));
                }

                var cmdCopyResource = new CopyResources()
                {
                    SourceEntityId = ConfigurationManager.AppSettings["BfwUsersDomainId"],
                    DestEntityId = domain.Id
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmdCopyResource);

                return domain.Id;
            }
        }

        public Bdc.Domain GetOrCreateDomain(string domainName, string parentDomainName, string onyxReferenceId, string userspacePrefix = "", string userspaceSuffix = "", bool copyResourcesForNewDomain = true)
        {
            var domain = GetDomain(domainName);
            if (domain != null)
            {
                return domain;
            }
            else
            {
                domain = GetDomain(parentDomainName);

                if (domain == null)
                {
                    throw new Exception(string.Format("Could not found Parent Domain - {0}", parentDomainName));
                }

                string ParentDomainId = domain.Id;
                string userspace = string.Concat(userspacePrefix, onyxReferenceId, userspaceSuffix);
                domain = CreateDomain(ParentDomainId, domainName,userspace,onyxReferenceId, GetNewDomainData());

                if (domain == null)
                {
                    throw new Exception(string.Format("Could not create Domain - {0}", domainName));
                }

                if (copyResourcesForNewDomain)
                {
                    var cmdCopyResource = new CopyResources()
                    {
                        SourceEntityId = ConfigurationManager.AppSettings["BfwUsersDomainId"],
                        DestEntityId = domain.Id
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdCopyResource);
                }

                return domain;
            }
        }

        /// <summary>
        /// This method is used to get the default Data for all new Domain from BFWUsers domain
        /// </summary>
        /// <returns></returns>
        private XElement GetNewDomainData()
        {
            XElement domainData = null;
            try
            {
                Domain bfwusersDomain = GetDomainById(ConfigurationManager.AppSettings["BfwUsersDomainId"]);
                domainData = bfwusersDomain.Data;
            }
            catch { }
            return domainData;
        }
    }
}
