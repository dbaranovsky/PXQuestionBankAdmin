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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/UpdateCourses command
    /// </summary>
    public class UpdateCourses : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be added/updated
        /// </summary>
        public List<Course> Courses { get; protected set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCourses"/> class.
        /// </summary>
        /// <remarks></remarks>
        public UpdateCourses()
        {
            Courses = new List<Course>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified course to the list of courses to update.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <remarks></remarks>
        public void Add(Course course)
        {
            Courses.Add(course);
        }

        /// <summary>
        /// Adds the specified courses to the list of courses to update.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <remarks></remarks>
        public void Add(List<Course> courses)
        {
            Courses = courses;
        }

        /// <summary>
        /// Clears this instance of all courses.
        /// </summary>
        /// <remarks></remarks>
        public void Clear()
        {
            Courses.Clear();
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/UpdateCourses command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/UpdateCourses</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (Courses.IsNullOrEmpty())
                throw new DlapException("Cannot create a UpdateCourses request if there are not Courses in the Courses collection");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "updatecourses" } }
            };

            foreach (var course in Courses)
            {
                request.AppendData(course.ToEntity());
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/UpdateCourses command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("UpdateCourses request failed with response code {0}", response.Code));
        }

        #endregion
    }
}
