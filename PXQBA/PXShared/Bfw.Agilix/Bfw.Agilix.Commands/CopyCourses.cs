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
    /// Implements the http://http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses command, which is
    /// used to create new courses based on an existing one. The copied course is called the "derived course".
    /// </summary>
    public class CopyCourses : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be added/updated.
        /// </summary>
        public List<Course> Courses { get; protected set; }

        /// <summary>
        /// The domain into which to copy the courses.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        /// <summary>
        /// Method by which existing courses are copied. Please see
        /// http://http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses for further information.
        /// </summary>
        public string Method { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the command state.
        /// </summary>
        public CopyCourses()
        {
            Courses = new List<Course>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new course to copy.
        /// </summary>
        /// <param name="course">Course to copy.</param>
        public void Add(Course course)
        {
            Courses.Add(course);
        }

        /// <summary>
        /// Adds a list of courses to copy.
        /// </summary>
        /// <param name="courses">Courses to copy.</param>
        public void Add(List<Course> courses)
        {
            Courses = courses;
        }

        /// <summary>
        /// Removes all courses from the list of courses to copy
        /// </summary>
        /// <value>Clears <see cref="Courses" />.</value>
        public void Clear()
        {
            Courses.Clear();
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Parses resulting XML as defined by the http://http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses documentation.
        /// </summary>
        /// <param name="response">Response returned by DLAP.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("CopyCourses request failed with response code {0}", response.Code));
            }

            int index = 0;

            string message = "";
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());

                    if (resp.Code == DlapResponseCode.AccessDenied)
                    {
                        throw new DlapAuthenticationException(message);
                    }
                    else
                    {
                        throw new DlapException(string.Format("CopyCourses request failed with response code {0}", response.Code));
                    }
                }
                else
                {
                    Courses[index].Id = resp.ResponseXml.Element("course").Attribute("courseid").Value.ToString();
                    Courses[index].Domain = new Domain() { Id = DomainId };
                }

                ++index;
            }
        }

        /// <summary>
        /// Builds request required by http://http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type command conforming to XML described by http://http://dev.dlap.bfwpub.com/Docs/Command/CopyCourses.</remarks>
        public override DlapRequest ToRequest()
        {
            if (Courses.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a CopyCourses request if there are not Courses in the Courses collection");
            }

            var method = CopyMethod.Default;

            if (!string.IsNullOrEmpty(Method))
            {
                method = Method;
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>()
                { 
                    { "cmd", "copycourses" }                    
                }
            };

            request.Timeout = 600;
            foreach (var course in Courses)
            {
                var entity = course.ToEntity();
                var action = entity.Attribute("action");
                var status = entity.Attribute("status");
                var domainid = entity.Attribute("domainid");
                var indexrule = entity.Attribute("indexrule");

                if (indexrule == null)
                {
                    indexrule = new XAttribute("indexrule", ((int)IndexRule.Deltas).ToString());
                    entity.Add(indexrule);
                }

                if (null == action)
                {
                    action = new XAttribute("action", method);
                    entity.Add(action);
                }

                if (null == status)
                {
                    status = new XAttribute("status", ((int)EnrollmentStatus.Active).ToString());
                    entity.Add(status);
                }

                if (DomainId != null)
                {
                    if (null == domainid)
                    {
                        domainid = new XAttribute("domainid", DomainId);
                        entity.Add(domainid);
                    }
                    else
                    {
                        domainid.Value = DomainId;
                    }
                }

                action.SetValue(method);
                status.SetValue(((int)EnrollmentStatus.Active).ToString());
                request.AppendData(entity);
            }

            return request;
        }

        #endregion
    }
}
