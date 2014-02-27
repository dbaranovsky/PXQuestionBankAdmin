using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PXWebAPI.Helpers
{
    public class DomainActions
    {
        #region Properties

        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public DomainActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
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

                SearchParameters = new Domain()
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
    }
}
