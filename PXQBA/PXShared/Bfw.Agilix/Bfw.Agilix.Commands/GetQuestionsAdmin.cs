using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
	/// <summary>
	/// Provides and implementation of the GetQuestionAdminList Agilix commands
	/// </summary>
	public class GetQuestionsAdmin : GetQuestions
	{
		#region Data Members

		/// <summary>
		/// Gets or sets the search parameters.
		/// </summary>
		/// <value>
		/// The search parameters.
		/// </value>
		public new QuestionAdminSearch SearchParameters { get; set; }

		#endregion

		/// <summary>
		/// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList command.
		/// </summary>
		/// <returns>
		/// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionList
		/// </returns>
		public override DlapRequest ToRequest()
		{
			if (null != SearchParameters && !String.IsNullOrEmpty(SearchParameters.EntityId) )
			{
				var request = new DlapRequest
				{
					Type = DlapRequestType.Get,
					Parameters = new Dictionary<string, object>()
				};

				string questionIds = default(string);

				if (!SearchParameters.QuestionIds.IsNullOrEmpty())  questionIds = SearchParameters.QuestionIds.Fold("|");

				request.Parameters["cmd"] = "GetQuestionList";
				request.Parameters["entityid"] = SearchParameters.EntityId;

				if (!string.IsNullOrEmpty(questionIds) ) request.Parameters["questionid"] = questionIds;
				if (!string.IsNullOrEmpty(SearchParameters.QuestionsFilter)) request.Parameters["query"] = SearchParameters.QuestionsFilter;

				if (SearchParameters.Count!= 0) request.Parameters["count"] = SearchParameters.Count;
                if (SearchParameters.version != false) request.Parameters["allversions"] = SearchParameters.version;

				return request;
			}

			throw new InvalidOperationException("Invalid search parameters for generating GetQuestionsAdmin search request.");
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

			Questions = new List<Question>();

			IEnumerable<XElement> questionElements = response.ResponseXml.Element("responses").Element("response").Elements("question").ToList();

			foreach (var responseElement in questionElements)
			{
				Question q = new Question();
				q.ParseEntity(responseElement);
				q.EntityId = SearchParameters.EntityId;
				( (List<Question>)Questions ).Add(q);
			}
		}

	}
}

