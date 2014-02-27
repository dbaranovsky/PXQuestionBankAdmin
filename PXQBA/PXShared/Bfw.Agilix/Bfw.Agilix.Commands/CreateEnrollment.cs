using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/CreateEnrollments command.
    /// </summary>
    public class CreateEnrollment : DlapCommand
    {
        #region Properties

        /// <summary>
        /// When true, this command generates an error for an attempt to create a duplicate enrollment which is the case where an enrollment already exists for the specified user ID on the specified entity ID.
        /// False (default) otherwise.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [Disallowduplicates]; otherwise, <c>false</c>.
        /// </value>
        public bool Disallowduplicates { get; set; }

        /// <summary>
        /// The list of items that are going to be added/updated.
        /// </summary>
        public List<Enrollment> Enrollments { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes command state.
        /// </summary>
        public CreateEnrollment()
        {
            Enrollments = new List<Enrollment>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an enrollment to the list of enrollments to create.
        /// </summary>
        /// <param name="enrollment">Enrollment to create.</param>
        public void Add(Enrollment enrollment)
        {
            Enrollments.Add(enrollment);
        }

        /// <summary>
        /// Clears the list of enrollments to create.
        /// </summary>
        /// <value>Clears the <see cref="Enrollments" />.</value>
        public void Clear()
        {
            Enrollments.Clear();
        }

        #endregion

        #region overrides from DlapCommand

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/CreateEnrollments command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>XML response conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateEnrollments.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("CreateEnrollments request failed with response code {0}", response.Code));
            }

            int index = 0;
            string message = string.Empty;
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                }
                else
                {
                    Enrollments[index].Id = response.ResponseXml.Element("responses").Element("response").Element("enrollment").Attribute("enrollmentid").Value.ToString();
                }
                ++index;
            }
        }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/CreateEnrollments command.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type request with XML body conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateEnrollments.</remarks>
        public override DlapRequest ToRequest()
        {
            if (Enrollments.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a Create Enrollments request if there are no enrollment in the Enrollments collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "createenrollments" } }
            };

            if (Disallowduplicates)
                request.Parameters.Add("disallowduplicates", Disallowduplicates);

            foreach (var enrollment in Enrollments)
            {
                request.AppendData(enrollment.ToEntity());
            }

            return request;
        }

        #endregion
    }
}
