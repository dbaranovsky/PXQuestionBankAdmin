using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using System.Xml.Linq;
using System.Linq;
namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutTeacherResponse command.
    /// </summary>
    public class GetTeacherResponseInfo : DlapCommand
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
        public List<TeacherResponse> TeacherResponses { get; set; }

        public Dictionary<string, TeacherResponse> TeacherResponseInfo {get; set;}

        public TeacherResponseSearch SearchParameters { get; set; }

        #endregion

       #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponseInfo command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponseInfo</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {

            var request = new DlapRequest
            {
                Type = DlapRequestType.Post,
                ContentType = "application/x-dlap-response-xml",
                SuppressWrapper = true,
                Parameters = new Dictionary<string, object> { 
                    { "cmd", "getteacherresponseinfo" }
                    
                }
            };
            foreach(TeacherResponse tr in TeacherResponses){
                request.AppendData(tr.ToTeacherResponseEntity());
            }
            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponseInfo command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse dlapResponse)
        {
           
            TeacherResponseInfo = new Dictionary<string, TeacherResponse>();
            if (DlapResponseCode.OK != dlapResponse.Code)
                throw new DlapException(string.Format("GetTeacherResponseInfo failed with code: {0}", dlapResponse.Code));

            var responses = dlapResponse.Batch.ToList();
            for (int i = 0; i < responses.Count(); i++)
            {
                if (DlapResponseCode.OK != responses[i].Code)
                {
                    TeacherResponses[i].DoesResponseExist = false;
                }
                else
                {
                    TeacherResponses[i].DoesResponseExist = true;
                }
            }
           
        }


        #endregion 
    }
}
