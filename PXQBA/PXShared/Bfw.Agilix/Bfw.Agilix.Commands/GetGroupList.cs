using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Encapsulates the GetEnrollmentGradebook2 (Docs/Command/GetGroupList),
    /// GetUserGradebook2 (Docs/Command/GetGroupList), and GetGroupList
    /// (Docs/Command/GetGroupList) DLAP requests
    /// </summary>
    public class GetGroupList : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        /// <value>
        /// The course id.
        /// </value>
        public string CourseId { get; set; }

        /// <summary>
        /// Gets or sets the set id.
        /// </summary>
        /// <value>
        /// The set id.
        /// </value>
        public int SetId { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public IList<Group> Groups { get; set; }

        #endregion


        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetGroupList command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetGroupList
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };

            request.Parameters["cmd"] = "GetGroupList";
            request.Parameters["ownerid"] = CourseId;
            request.Parameters["includeenrollments"] = true;

            if (SetId > 0)
            {
                request.Parameters["setid"] = SetId;
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetGroupList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            Groups = new List<Group>();
            if (null != response.ResponseXml && response.ResponseXml.Element("groups") != null)
            {
                foreach (XElement el in response.ResponseXml.Element("groups").Elements("group"))
                {
                    var id = Convert.ToInt32(el.Attribute("id").Value);

                    var title = el.Attribute("title").Value;

                    var members = new List<Enrollment>();
                    foreach (var enrollmentEl in el.Elements("enrollment"))
                    {
                        Enrollment e = new Enrollment();
                        e.ParseEntity(enrollmentEl);
                        e.ParseEntity(enrollmentEl.Element("user"));
                        members.Add(e);
                    }

                    Groups.Add(new Group()
                    {
                        Id = id,
                        Title = title,
                        MemberEnrollments = members
                    });
                }
            }
        }
    }
}
