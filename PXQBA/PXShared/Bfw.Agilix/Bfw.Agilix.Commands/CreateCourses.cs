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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/CreateCourses.
    /// </summary>
    public class CreateCourses : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be added/updated.
        /// </summary>
        public List<Course> Courses { get; protected set; }

        /// <summary>
        /// Entities created by successfull execution of the command.
        /// </summary>
        public List<Course> Entity { get; protected set; }

        /// <summary>
        /// Any items that failed to process.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the command state.
        /// </summary>
        public CreateCourses()
        {
            Courses = new List<Course>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a course to the list of courses to create.
        /// </summary>
        /// <param name="course">Course to create.</param>
        public void Add(Course course)
        {
            Courses.Add(course);
        }

        /// <summary>
        /// Adds all courses to the list of courses to create.
        /// </summary>
        /// <param name="courses">Courses to create.</param>
        public void Add(List<Course> courses)
        {
            Courses = courses;
        }

        /// <summary>
        /// Clears the list of courses to create.
        /// </summary>
        /// <value>Clears <see cref="Courses" />.</value>
        public void Clear()
        {
            Courses.Clear();
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("CreateCourses request failed with response code {0}", response.Code));
            }

            if (!response.IsBatch)
            {
                throw new DlapException("CreateCourses expects a response formated as a batch");
            }

            int index = 0;
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    throw new DlapException(resp.Message);
                }

                var courseElm = resp.ResponseXml.Element("course");
                Course c = new Course();
                c.Id = courseElm.Attribute("courseid").Value.ToString();
                Entity = new List<Course>();
                Entity.Add(c);
                Courses[index].Id = c.Id;
                ++index;
            }
        }

        /// <summary>
        /// Builds request required by http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type request with XML body conforming to http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses.</remarks>
        public override DlapRequest ToRequest()
        {
            if (Courses.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a CopyCourses request if there are not Courses in the Courses collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "createcourses" } }
            };

            foreach (var course in Courses)
            {
                request.AppendData(course.ToEntity());
            }
            return request;
        }

        #endregion
    }
}
