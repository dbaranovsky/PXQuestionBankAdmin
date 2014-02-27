using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetEntityWork2 command
    /// </summary>
    public class GetEntityWork2 : DlapCommand
    {
        /// <summary>
        /// Used to filter the submissions.
        /// </summary>
        public WorkSearch SearchParameters;

        /// <summary>
        /// Submissions that matched <see cref="SearchParameters" />.
        /// </summary>
        public IList<Work> Works;

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetEntityWork2 command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetEntityWork2</returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getentitywork2" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.EntityId))
            {
                request.Parameters["entityid"] = SearchParameters.EntityId;
            }

            if (SearchParameters.OutStanding)
            {
                request.Parameters["outstanding"] = true;
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetEntityWork2 command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                if (DlapResponseCode.AccessDenied == response.Code)
                {
                    AgilixUser oWork = null;
                    oWork = new AgilixUser();
                }
                else
                {
                    throw new DlapException(string.Format("GetEntityWork2 command failed with code {0}", response.Code));
                }
            }
            else
            {
                Work oWork = null;
                if ("works" == response.ResponseXml.Root.Name)
                {
                    Works = new List<Work>();
                    foreach (var userElm in response.ResponseXml.Root.Elements("work"))
                    {
                        oWork = new Work();
                        oWork.ParseEntity(userElm);
                        Works.Add(oWork);
                    }
                }
                else if ("work" == response.ResponseXml.Root.Name)
                {
                    oWork = new Work();
                    oWork.ParseEntity(response.ResponseXml.Root);
                    Works = new List<Work>() { oWork };
                }
                else
                {
                    throw new BadDlapResponseException(string.Format("GetEntityWork2 expected a response element of 'user' or 'users', but got '{0}' instead.", response.ResponseXml.Root.Name));
                }
            }
        }
    }
}
