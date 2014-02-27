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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/DeleteCourses command.
    /// </summary>
    public class DeleteCourses : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Courses to delete.
        /// </summary>
        public List<Course> Courses { get; protected set; }

        /// <summary>
        /// Any failures that occured.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCourses"/> class.
        /// </summary>
        public DeleteCourses()
        {
            Courses = new List<Course>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a course to the list of courses to delete.
        /// </summary>
        /// <param name="course">Course to delete.</param>
        public void Add(Course course)
        {
            Courses.Add(course);
        }

        /// <summary>
        /// Adds all courses to the list of courses to delete.
        /// </summary>
        /// <param name="courses">Courses to delete.</param>
        public void Add(IEnumerable<Course> courses)
        {
            Courses = courses.ToList();
        }

        /// <summary>
        /// Clears the list of courses to delete.
        /// </summary>
        /// <value>Clears <see cref="Courses" />.</value>
        public void Clear()
        {
            Courses.Clear();
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
            if (Courses.IsNullOrEmpty())
            {
                throw new DlapException("Cannot Delete a Course request if there are not Courses in the Courses collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "deletecourses" } }
            };

            foreach (var course in Courses)
            {
                request.AppendData(course.ToEntity());
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
                throw new DlapException(string.Format("DeleteCourses request failed with response code {0}", response.Code));
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
