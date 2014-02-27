using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands
{
    public class DeleteUsers : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Courses to delete.
        /// </summary>
        public List<AgilixUser> Users { get; protected set; }

        /// <summary>
        /// Any failures that occured.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCourses"/> class.
        /// </summary>
        public DeleteUsers()
        {
            Users = new List<AgilixUser>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a course to the list of courses to delete.
        /// </summary>
        /// <param name="course">Course to delete.</param>
        public void Add(AgilixUser user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// Adds all courses to the list of courses to delete.
        /// </summary>
        /// <param name="courses">Courses to delete.</param>
        public void Add(IEnumerable<AgilixUser> users)
        {
            Users = users.ToList();
        }

        /// <summary>
        /// Clears the list of courses to delete.
        /// </summary>
        /// <value>Clears <see cref="Courses" />.</value>
        public void Clear()
        {
            Users.Clear();
        }

        #endregion

        public override Dlap.DlapRequest ToRequest()
        {
            if (Users.IsNullOrEmpty())
            {
                throw new DlapException("Cannot do a Delete User request if there are no Users in the collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "deleteusers" } }
            };

            foreach (var user in Users)
            {
                request.AppendData(user.ToEntity());
            }

            return request;
        }

        public override void ParseResponse(Dlap.DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteUsers request failed with response code {0}", response.Code));
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
    }
}
