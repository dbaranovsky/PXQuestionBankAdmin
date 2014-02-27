using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Web.Mvc;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate Agilix tasks.
    /// See http://gls.agilix.com/Docs/Concept/Tasks.
    /// </summary>
    public interface ITaskActions
    {
        /// <summary>
        /// Immediately runs an Agilix task. If the task is already running, RunTask returns the running task's status and does not start a new task..
        /// </summary>
        /// <param name="Command">The command.</param>
        /// <returns></returns>
        Task RunTask(String Command);

        /// <summary>
        /// Runs the SolrIndexBatch task in Agilix, indexing any searchable content since the last SolrIndexBatch run.
        /// </summary>
        /// <returns></returns>
        Task RunDeltaIndex();        
    }
}