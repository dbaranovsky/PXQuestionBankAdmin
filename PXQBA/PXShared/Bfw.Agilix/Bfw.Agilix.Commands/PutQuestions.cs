using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutQuestions command
    /// </summary>
    public class PutQuestions : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be added/updated
        /// </summary>
        public List<Question> Questions { get; protected set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PutQuestions"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PutQuestions()
        {
            Questions = new List<Question>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <remarks></remarks>
        public void Add(Question question)
        {
            Questions.Add(question);
        }

        /// <summary>
        /// Adds the specified questions.
        /// </summary>
        /// <param name="questions">The questions.</param>
        /// <remarks></remarks>
        public void Add(IEnumerable<Question> questions)
        {
            Questions = questions.ToList();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <remarks></remarks>
        public void Clear()
        {
            Questions.Clear();
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutQuestions command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutQuestions</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (Questions.IsNullOrEmpty())
                throw new DlapException("Cannot create a PutQuestions request if there are not Questions in the Questions collection");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "putquestions" }, {"entityid", Questions.First().EntityId } }
            };

            foreach (var question in Questions)
            {
                request.AppendData(question.ToEntity());
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutQuestions command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutQuestions request failed with response code {0}", response.Code));

            string message = string.Empty;
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                    Failures.Add(new ItemStorageFailure()
                    {
                        Reason = message
                    });
                }
            }
        }

        #endregion
    }
}
