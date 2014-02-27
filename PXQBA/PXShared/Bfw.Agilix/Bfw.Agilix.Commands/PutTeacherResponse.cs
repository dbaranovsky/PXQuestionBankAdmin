using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutTeacherResponse command.
    /// </summary>
    public class PutTeacherResponse : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Enrollments Id of the student that the instructor is responding to.
        /// </summary>
        public string StudentEnrollmentId { get; set; }

        /// <summary>
        /// Id of the item the teacher is responding in regards to.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Content of the teacher's response.
        /// </summary>
        public TeacherResponse TeacherResponse { get; set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutTeacherResponse command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutTeacherResponse</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest
            {
                Type = DlapRequestType.Post,
                ContentType = "application/x-dlap-response-xml",
                SuppressWrapper = true,
                Parameters = new Dictionary<string, object> {
                    { "cmd", "putteacherresponse" },
                    { "enrollmentid", StudentEnrollmentId },
                    { "itemid", ItemId}
                }
            };
            request.AppendData(TeacherResponse.ToEntity());
            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutTeacherResponse command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutTeacherResponse failed with code: {0}", response.Code));

            var teacherResponse = new TeacherResponse();
            teacherResponse.ParseEntity(response.ResponseXml.Root);
        }

        #endregion
    }
}
