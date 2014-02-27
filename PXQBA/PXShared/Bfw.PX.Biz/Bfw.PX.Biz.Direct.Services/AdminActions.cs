using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;

using Bfw.Common.Logging;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    public class AdminActions : IAdminActions
    {
        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        public AdminActions(IBusinessContext context, ISessionManager sessionManager)
		{
			Context = context;
            SessionManager = sessionManager;
		}

        /// <summary>
        /// Executes the status message against DLAP and returns the resulting
        /// status document.
        /// </summary>
        /// <returns>DLAP status document</returns>
        public XDocument GetStatus()
        {
            var cmd = new GetStatus();
            XDocument status = null;

            try
            {
                SessionManager.CurrentSession.Execute(cmd);
                status = cmd.Status;
            }
            catch (DlapException ex)
            {
                Context.Logger.Log(ex);
                status = null;
            }

            return status;
        }
    }
}
