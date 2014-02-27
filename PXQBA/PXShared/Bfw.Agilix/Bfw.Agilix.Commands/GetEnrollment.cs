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
    /// Implements the http://dev.dlap.bfwpub.com/js/docs/#!/command/getenrollment2 command.
    /// </summary>
    public class GetEnrollment : DlapCommand
    {
        #region Data Members
        /// <summary>
        /// The enrollment ID to get information for.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// The <see cref="Enrollment"/> objects found by the command.
        /// </summary>
        public List<Enrollment> Enrollments { get; protected set; }

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
            if (string.IsNullOrEmpty(EnrollmentId))
                throw new DlapException("GetEnrollment command requires an Enrollment Id");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getenrollment2" } }
            };

            
            request.Parameters["enrollmentid"] = EnrollmentId;
            
            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("GetEnrollment command failed with code {0}", response.Code));

            var single = new Enrollment();
            single.ParseEntity(response.ResponseXml.Root);
            Enrollments = new List<Enrollment>() { single };
            
            
        }
    }

        #endregion
}