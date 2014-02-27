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
    /// Implements ITaskActions using direct connection to DLAP.
    /// </summary>
    public class TaskActions : ITaskActions
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
        /// Initializes a new instance of the <see cref="TaskActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public TaskActions(IBusinessContext ctx, ISessionManager sessionManager)
        {
            Context = ctx;
            SessionManager = sessionManager;            
        }

        #endregion

        #region ITaskActions Members

        /// <summary>
        /// Immediately runs an Agilix task. 
        /// If the task is already running, RunTask returns the running task's status and does not start a new task..
        /// </summary>
        /// <param name="Command">The DLAP task command to execute.</param>
        /// <returns>A DLAP task object.</returns>
        public Bdc.Task RunTask(String command)
        {
            Bdc.Task task = null;
            using (Context.Tracer.StartTrace("TaskActions.RunTask(command)"))
            {
                if (!string.IsNullOrEmpty(command))
                {

                    var cmd = new RunTask()
                    {
                        SearchParameters = new Adc.TaskSearch()
                        {
                            Command = command
                        }
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                    if (cmd.TaskResponse != null)
                    {
                        task = cmd.TaskResponse.ToTask();
                    }
                }
            }

            return task;
        }

        /// <summary>
        /// Runs the SolrIndexBatch task in Agilix, indexing any searchable content since the last SolrIndexBatch run.
        /// </summary>
        /// <returns></returns>
        public Bdc.Task RunDeltaIndex()
        {
            return RunTask("SolrIndexBatch");
        }        
        
        #endregion
    }
}