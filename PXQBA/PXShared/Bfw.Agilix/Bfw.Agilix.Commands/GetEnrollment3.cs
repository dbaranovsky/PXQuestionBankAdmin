using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using System.ComponentModel;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/js/docs/#!/command/getenrollment3 command.
    /// </summary>
    public class GetEnrollment3 : DlapCommand
    {
        #region Data Members
        /// <summary>
        /// The enrollment ID to get information for.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// The flag to include additional data: course, course.data, domain, domain.data, user, user.data
        /// </summary>
        public EnrollmentSelect Select { get; set; }

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
            {
                throw new DlapException("GetEnrollment command requires an Enrollment Id");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getenrollment3" } }
            };
            
            request.Parameters["enrollmentid"] = EnrollmentId;
            request.Parameters["select"] =  GetSelectFlags();
            
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
                throw new DlapException(string.Format("GetEnrollmentDomainEntityUser command failed with code {0}", response.Code));
            }

            var single = new Enrollment();
            single.ParseEntity(response.ResponseXml.Root);

            var courseXML = response.ResponseXml.Descendants("course").FirstOrDefault();

            if (courseXML != null)
            {
                var course = new Course();
                course.ParseEntity(courseXML);
                single.Course = course;
            }

            var domainXML = response.ResponseXml.Descendants("domain").FirstOrDefault();

            if (domainXML != null)
            {
                var domain = new Domain();
                domain.ParseEntity(domainXML);
                single.Domain = domain;
            }

            var userXML = response.ResponseXml.Descendants("user").FirstOrDefault();

            if (userXML != null)
            {
                var user = new AgilixUser();
                user.ParseEntity(userXML);
                single.User = user;
            }

            Enrollments = new List<Enrollment>() { single };
        }

        private string GetSelectFlags()
        {
            StringBuilder result = new StringBuilder();

            if (Select.HasFlag(EnrollmentSelect.Course))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.Course.GetType().GetField(EnrollmentSelect.Course.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            if (Select.HasFlag(EnrollmentSelect.CourseData))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.CourseData.GetType().GetField(EnrollmentSelect.CourseData.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            if (Select.HasFlag(EnrollmentSelect.Domain))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.Domain.GetType().GetField(EnrollmentSelect.Domain.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            if (Select.HasFlag(EnrollmentSelect.DomainData))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.DomainData.GetType().GetField(EnrollmentSelect.DomainData.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            if (Select.HasFlag(EnrollmentSelect.User))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.User.GetType().GetField(EnrollmentSelect.User.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            if (Select.HasFlag(EnrollmentSelect.UserData))
            {
                result.AppendFormat("{0},",
                    (EnrollmentSelect.UserData.GetType().GetField(EnrollmentSelect.UserData.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute).Description);
            }

            return result.ToString().Substring(0, result.ToString().Length - 1);
        }
    }


        #endregion
}