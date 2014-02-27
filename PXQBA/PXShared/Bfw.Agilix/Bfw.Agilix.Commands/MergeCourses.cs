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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/MergeCourses command.
    /// </summary>
    public class MergeCourses : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Derivative course to merge.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MergeCourses"/> is success.
        /// </summary>
        /// <remarks></remarks>
        public bool Success { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeCourses"/> class.
        /// </summary>
        public MergeCourses()
        {
            
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
            if (CourseId == null)
            {
                throw new DlapException("Cannot Merge a Course request if no CourseId is provided.");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,                
                Parameters = new Dictionary<string, object>() { { "cmd", "mergecourses" } }
            };


            var requestDoc = new XDocument();
            var rootElem = new XElement("requests");
            var courseElem = new XElement("course");
            courseElem.Add(new XAttribute("courseid", CourseId));
            rootElem.Add(courseElem);
            requestDoc.Add(rootElem);
            request.AppendData(requestDoc.Root);

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
                throw new DlapException(string.Format("MergeCourses request failed with response code {0}", response.Code));
            }
            Success = true;            
        }

        #endregion
    }
}
