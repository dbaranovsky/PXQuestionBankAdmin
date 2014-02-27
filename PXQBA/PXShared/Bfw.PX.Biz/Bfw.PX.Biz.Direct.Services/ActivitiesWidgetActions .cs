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
using Bfw.Common.Logging;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;

using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements IActivitiesWidgetActions using direct connection to DLAP.
    /// </summary>
    public class ActivitiesWidgetActions : IActivitiesWidgetActions
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivitiesWidgetActions"/> class.        
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>        
        public ActivitiesWidgetActions(IBusinessContext ctx, ISessionManager sessionManager)
        {
            Context = ctx;
            SessionManager = sessionManager;                    
        }

        #endregion

        #region ActivitiesWidgetActions Members

        /// <summary>
        /// Retrieving a list of activities from DLAP by the specific contentTypeFilter. 
        /// If enrollmentId is specified, will check for student submission against each item.
        /// </summary>
        /// <param name="contentTypeFilter">The activity content type to search for.</param>
        /// <param name="enrollmentId">If specified, will check for student submission against each item.</param>
        public Bdc.ActivitiesWidgetResults LoadActivitiesByType(string contentTypeFilter, string enrollmentId = null)
        {
            using (Context.Tracer.DoTrace("ActivitiesWidget.LoadActivitiesByType(contentTypeFilter={0}, enrollmentId={1})", contentTypeFilter, enrollmentId))
            {
                Bdc.ActivitiesWidgetResults result = null;

                string query = string.Format(" /meta-content-type='{0}' ", contentTypeFilter);

                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        Query = query
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);               

                if (!cmd.Items.IsNullOrEmpty())
                {
                    List<Bdc.ContentItem> bizItems = new List<Bdc.ContentItem>();                
                    bizItems = cmd.Items.Map(i => i.ToContentItem(Context)).ToList();

                    result = new Bdc.ActivitiesWidgetResults();
                    result.Activities = bizItems;
                }

                return result;
            }
        }
        #endregion
    }
}
