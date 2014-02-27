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
    /// Implementation of the http://dev.dlap.bfwpub.com/Docs/Command/DeleteQuestions DLAP command.
    /// </summary>
    public class DeleteQuestions : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of questions that are going to be deleted.
        /// </summary>
        public List<XElement> Questions { get; set; }

        /// <summary>
        /// Any items that failed to process.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a default DeleteQuestions Command.
        /// </summary>
        public DeleteQuestions()
        {
            Questions = new List<XElement>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (Questions.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a DeleteQuestions request if there are not items in the Items collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "deletequestions" } }
            };

            foreach (XElement question in Questions)
            {
                request.AppendData(question);
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteQuestions request failed with response code {0}", response.Code));
            }
            
            int index = 0;
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
                ++index;
            }
        }

        #endregion
    }
}
