using System;
using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of the http://dev.dlap.bfwpub.com/Docs/Command/GetTaskList2 command
    /// </summary>
    public class GetTaskList : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Identifies the task o run.
        /// </summary>
        public TaskSearch SearchParameters;

        /// <summary>
        /// Response of running the task.
        /// </summary>
        public IEnumerable<Task> Tasks { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetTaskList2 command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetTaskList2</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "GetTaskList2" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.TaskId)) { request.Parameters["taskid"] = SearchParameters.TaskId; }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetTaskList2 command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                if (Tasks == null)
                {
                    Tasks = new List<Task>();
                }

                var taskElements = response.ResponseXml.Element("responses").Elements("task");

                foreach (var responseElement in taskElements)
                {
                    var task = new Task {TaskId = SearchParameters.TaskId};
                    ((List<Task>)Tasks).Add(task);
                    var running = responseElement.Attribute("running");
                    if (running != null)
                    {
                        Boolean bln;
                        if (Boolean.TryParse(running.Value, out bln))
                        {
                            task.Finished = !bln;
                        }
                    }
                }
            }
        }

        #endregion
    }
}