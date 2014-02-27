using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/UpdateUsers command
    /// </summary>
    public class UpdateUsers : DlapCommand
    {
        #region Propeties

        /// <summary>
        /// The list of items that are going to be updated.
        /// </summary>
        public List<AgilixUser> Users { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether update succeeded.
        /// </summary>
        /// <value><c>true</c> if user updated; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool UpdateResult { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUsers"/> class.
        /// </summary>
        /// <remarks></remarks>
        public UpdateUsers()
        {
            Users = new List<AgilixUser>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified user to update.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks></remarks>
        public void Add(AgilixUser user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// Clears this instance of all users to update.
        /// </summary>
        /// <remarks></remarks>
        public void Clear()
        {
            Users.Clear();
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/UpdateUsers command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/UpdateUsers</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (Users.IsNullOrEmpty())
                throw new DlapException("Cannot update a UpdateUsers request if there are no user in the Users collection");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "updateusers" } }
            };

            foreach (var user in Users)
                request.AppendData(user.ToEntity());

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/UpdateUsers command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            bool localResult = false;
            UpdateResult = true;
            if (DlapResponseCode.OK != response.Code)
            {
                UpdateResult = false;
                throw new DlapException(string.Format("UpdateUsers request failed with response code {0}", response.Code));
            }

            string message = string.Empty;
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                    localResult = true;
                    UpdateResult = false;
                }
                else
                {
                    if (!localResult) UpdateResult = true;
                }
            }
        }

        #endregion
    }
}