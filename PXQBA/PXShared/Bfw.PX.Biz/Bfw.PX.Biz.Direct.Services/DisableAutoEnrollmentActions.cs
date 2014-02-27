using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the IEnrollmentActions interface.
    /// Empty class that allows disabling of auto enrollments via unity configuration.
    /// </summary>
    public class DisableAutoEnrollmentActions : IAutoEnrollmentActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        // <summary>
        /// The IEnrollmentActions implementation to use.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// The IUserActions implementation to use.
        /// </summary>
        protected IUserActions UserActions { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableAutoEnrollmentActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation.</param>
        /// <param name="userActions">The IUserActions implementation.</param>
        public DisableAutoEnrollmentActions(IBusinessContext ctx, IEnrollmentActions enrollmentActions, IUserActions userActions)
        {
            EnrollmentActions = enrollmentActions;
            UserActions = userActions;
            Context = ctx;
        }

        #endregion

        #region IAutoEnrollmentActions Members

        /// <summary>
        /// Method simply returns true.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean CreateEnrollments()
        {
            return true;
        }   
        
        #endregion
    }
}