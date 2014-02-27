using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of the http://dev.dlap.bfwpub.com/Docs/Command/RunTask command
    /// </summary>
    public class RunTask : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Identifies the task o run.
        /// </summary>
        public TaskSearch SearchParameters;

        /// <summary>
        /// Response of running the task.
        /// </summary>
        public Task TaskResponse { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/RunTask command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/RunTask</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "RunTask" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.TaskId)) { request.Parameters["taskid"] = SearchParameters.TaskId; }
            if (!string.IsNullOrEmpty(SearchParameters.Command)) { request.Parameters["command"] = SearchParameters.Command; }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/RunTask command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                TaskResponse = new Task();
                var taskElement = response.ResponseXml.Element("task");
                if (taskElement != null)
                {
                    var creationdate = taskElement.Attribute("creationdate");
                    if (creationdate != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(creationdate.Value, out dt))
                        {
                            TaskResponse.CreationDate = dt;
                        }
                    }


                    var lastrunenddate = taskElement.Attribute("lastrunenddate");
                    if (lastrunenddate != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(lastrunenddate.Value, out dt))
                        {
                            TaskResponse.LastRunEndDate = dt;
                        }
                    }

                    var lastrunstartdate = taskElement.Attribute("lastrunstartdate");
                    if (lastrunstartdate != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(lastrunstartdate.Value, out dt))
                        {
                            TaskResponse.LastRunStartDate = dt;
                        }
                    }

                    var startdate = taskElement.Attribute("startdate");
                    if (startdate != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(startdate.Value, out dt))
                        {
                            TaskResponse.StartDate = dt;
                        }
                    }

                    TaskResponse.Command = taskElement.Attribute("command").Value;

                    if (taskElement.Attribute("currentitem") != null)
                    {
                        TaskResponse.CurrentItem = taskElement.Attribute("currentitem").Value;
                    }

                    if (taskElement.Attribute("error") != null)
                    {
                        TaskResponse.Error = taskElement.Attribute("error").Value;
                    }

                    if (taskElement.Attribute("success") != null)
                    {
                        TaskResponse.Success = taskElement.Attribute("success").Value;
                    }


                    var finished = taskElement.Attribute("finished");
                    if (finished != null)
                    {
                        Boolean bln;
                        if (Boolean.TryParse(finished.Value, out bln))
                        {
                            TaskResponse.Finished = bln;
                        }
                    }

                    var periodminutes = taskElement.Attribute("periodminutes");
                    if (periodminutes != null)
                    {
                        Int32 num;
                        if (Int32.TryParse(periodminutes.Value, out num))
                        {
                            TaskResponse.PeriodMinutes = num;
                        }
                    }

                    var portioncomplete = taskElement.Attribute("portioncomplete");
                    if (portioncomplete != null)
                    {
                        Double num;
                        if (Double.TryParse(portioncomplete.Value, out num))
                        {
                            TaskResponse.PortionComplete = num;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
