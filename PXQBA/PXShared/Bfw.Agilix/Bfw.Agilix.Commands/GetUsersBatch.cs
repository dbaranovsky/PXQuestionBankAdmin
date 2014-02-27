using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides a batched implementaion of the http://dev.dlap.bfwpub.com/Docs/Command/GetUserList command.
    /// </summary>
    public class GetUsersBatch : DlapCommand //LMS - Do we need to change this?
    {
        #region Properties

        /// <summary>
        /// It contains collection of list of UserSearch data contract.
        /// </summary>
        public List<UserSearch> SearchParameters { get; set; }

        /// <summary>
        /// This is list of list of users
        /// </summary>
        public List<List<AgilixUser>> Users { get; protected set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetUserList command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetUserList</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "" } }
            };

            foreach (var user in SearchParameters)
                request.AppendData(user.ToEntity());

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetUserList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("GetUsers batch request failed with response code {0}", response.Code));

            Users = new List<List<AgilixUser>>();
            Failures = new List<ItemStorageFailure>();

            int index = 0;
            foreach (var resp in response.Batch)
            {
                List<AgilixUser> batchUser = new List<AgilixUser>();
                AgilixUser single = new AgilixUser();

                single = null;
                if ((DlapResponseCode.OK != resp.Code) && 
                        ((DlapResponseCode.BadRequest == resp.Code && resp.Message.Contains("Bad ExtendedId"))
                        ||(DlapResponseCode.AccessDenied == resp.Code)))
                    {
                        single = null;
                        batchUser.Add(single);
                        Failures.Add(new ItemStorageFailure
                                         {
                                             ItemId = index.ToString(),
                                             Reason = !string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString()
                                         });
                    }
                else
                {
                    if ("users" == resp.ResponseXml.Root.Name)
                    {
                        batchUser = new List<AgilixUser>();
                        foreach (var userElm in resp.ResponseXml.Root.Elements("user"))
                        {
                            single = new AgilixUser();
                            single.ParseEntity(userElm);
                            batchUser.Add(single);
                        }
                    }
                    else if ("user" == resp.ResponseXml.Root.Name)
                    {
                        single = new AgilixUser();
                        single.ParseEntity(resp.ResponseXml.Root);
                        batchUser = new List<AgilixUser>() {single};
                    }
                    else
                    {
                        throw new BadDlapResponseException(
                            string.Format(
                                "GetUsers expected a response element of 'user' or 'users', but got '{0}' instead.",
                                response.ResponseXml.Root.Name));
                    }
                }
                Users.Add(batchUser);
                index++;
            }
        }

        #endregion
    }
}
