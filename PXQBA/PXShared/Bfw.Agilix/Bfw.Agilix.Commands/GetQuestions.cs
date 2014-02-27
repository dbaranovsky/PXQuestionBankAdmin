using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides and implementation of the GetQuestionList Agilix commands
    /// </summary>
    public class GetQuestions : DlapCommand
    {
        //number of questions to search for for each thread
        private const int GetQuestionsPartitionSize = 10;
        #region Data Members

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public QuestionSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        public IEnumerable<Question> Questions { get; set; }

        #endregion

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (null != SearchParameters && !String.IsNullOrEmpty(SearchParameters.EntityId))
            {
                var request = new DlapRequest()
                {
                    Type = DlapRequestType.Get,
                    Parameters = new Dictionary<string, object>()
                };

                request.Parameters["cmd"] = "GetQuestionList";
                request.Parameters["entityid"] = SearchParameters.EntityId;

                if (!SearchParameters.QuestionIds.IsNullOrEmpty())
                    request.Parameters["questionid"] = String.Join("|", SearchParameters.QuestionIds.ToArray());

                if (!SearchParameters.Query.IsNullOrEmpty())
                    request.Parameters["query"] = SearchParameters.Query;

                return request;
            }

            throw new InvalidOperationException("Invalid search parameters for generating question search request.");
        }

        /// <summary>
        /// Builds a collection of DlapRequests by the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList </returns>
        /// <remarks></remarks>
        public override IEnumerable<DlapRequest> ToRequestAsync()
        {
            if (null != SearchParameters && !String.IsNullOrEmpty(SearchParameters.EntityId))
            {
                if (!SearchParameters.Query.IsNullOrEmpty())
                {
                    var request = new DlapRequest()
                    {
                        Type = DlapRequestType.Get,
                        Parameters = new Dictionary<string, object>()
                    };
                    request.Parameters["cmd"] = "GetQuestionList";
                    request.Parameters["entityid"] = SearchParameters.EntityId;
                    request.Parameters["query"] = SearchParameters.Query;
                    return new List<DlapRequest>() { request };
                }
                else if (!SearchParameters.QuestionIds.IsNullOrEmpty())
                {
                    var requests = new List<DlapRequest>();

                    foreach (var questionIds in SearchParameters.QuestionIds.Partition(GetQuestionsPartitionSize))
                    {
                        var request = new DlapRequest()
                            {
                                Type = DlapRequestType.Get,
                                Parameters = new Dictionary<string, object>()
                            };

                        request.Parameters["cmd"] = "GetQuestionList";
                        request.Parameters["entityid"] = SearchParameters.EntityId;
                        request.Parameters["questionid"] = String.Join("|", questionIds.ToArray());
                        requests.Add(request);
                    }
                    return requests;
                }
                return null;
                
            }

            throw new InvalidOperationException("Invalid search parameters for generating question search request.");
        }


        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.AccessDenied == response.Code)
            {
                throw new DlapAuthenticationException("GetQuestions command failed with code AccessDenied");
            }

            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetQuestions command failed with code {0}", response.Code));
            }

            if (Questions == null)
            {
                Questions = new List<Question>();
            }

            var questionElements = response.ResponseXml.Element("responses").Element("response").Elements("question");

            foreach (var responseElement in questionElements)
            {
                Question q = new Question();
                q.ParseEntity(responseElement);
                q.EntityId = SearchParameters.EntityId;
                ((List<Question>)Questions).Add(q);
            }
        }

        /// <summary>
        /// Parses the asynchronous response of the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponseAsync(IEnumerable<DlapResponse> response)
        {
            foreach (var dlapResponse in response)
            {
                this.ParseResponse(dlapResponse);
            }
        }
    }
}
