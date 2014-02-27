using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements IGradeActions using direct connection to DLAP.
    /// </summary>
    public class GroupActions : IGroupActions
    {

        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        public GroupActions(IBusinessContext context, ISessionManager sessionManager)
        {
            Context = context;
            SessionManager = sessionManager;
        }

        #endregion

        #region IGroupActions Members

        /// <summary>
        /// Lists groups in the specified course.
        /// </summary>
        /// <param name="courseId">ID of the owning course to list groups for.</param>
        /// <param name="setId">Group set ID by which to filter the list.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Group> ListGroups(string courseId, int setId = 0)
        {
            IEnumerable<Bdc.Group> result = null;

            using (Context.Tracer.StartTrace(string.Format("GroupActions.ListGroups(courseId={0}, setId={1})", courseId, setId)))
            {
                var cmd = new GetGroupList()
                {
                    CourseId = courseId,
                    SetId = setId
                };

                SessionManager.CurrentSession.Execute(cmd);

                result = cmd.Groups.Map(g => g.ToGroup());
            }

            return result;
        }

        /// <summary>
        /// Deletes a group set and all its child groups.
        /// </summary>
        /// <param name="courseId">ID of the owning course.</param>
        /// <param name="setId">ID of the group set to delete.</param>
        public void DeleteGroupSet(string courseId, int setId)
        {
            using (Context.Tracer.StartTrace(string.Format("GroupActions.DeleteGroupSet(courseId={0}, setId={1})", courseId, setId)))
            {
                var batch = new Batch();
                var groupsCmd = new GetGroupList()
                {
                    CourseId = courseId,
                    SetId = setId
                };

                var courseCmd = new GetCourse()
                {
                    SearchParameters = new Adc.CourseSearch()
                    {
                        CourseId = courseId
                    }
                };

                batch.Add(groupsCmd);
                batch.Add(courseCmd);

                SessionManager.CurrentSession.Execute(batch);

                batch = new Batch();
                batch.Add(new DeleteGroups()
                {
                    GroupIds = groupsCmd.Groups.Map(g => g.Id)
                });

                var course = courseCmd.Courses.First();
                var groupSetsEl = course.Data.Element("groupsets");
                if (groupSetsEl != null)
                {
                    var groupSetEls = groupSetsEl.Elements("groupset");
                    foreach (var groupSetEl in groupSetEls)
                    {
                        var id = Convert.ToInt32(groupSetEl.Attribute("id").Value);
                        if (id == setId)
                        {
                            groupSetEl.Remove();
                        }
                    }
                }

                var updateCourses = new UpdateCourses();
                updateCourses.Add(course);
                batch.Add(updateCourses);

                Context.Course.GroupSets = Context.Course.GroupSets.Where(o => !o.Id.Equals(setId));

                Context.CacheProvider.InvalidateCourseContent(Context.Course);

                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }
        }

        /// <summary>
        /// Loads all group sets for a specified course including groups.
        /// </summary>
        /// <param name="courseId">ID of the owning course.</param>
        /// <returns></returns>
        public IDictionary<Bdc.GroupSet, IList<Bdc.Group>> GetAllGroupSetsWithGroups(string courseId)
        {
            IDictionary<Bdc.GroupSet, IList<Bdc.Group>> result = new Dictionary<Bdc.GroupSet, IList<Bdc.Group>>();

            using (Context.Tracer.StartTrace(string.Format("GroupActions.GetAllGroupSetsWithGroups(courseId={0})", courseId)))
            {
                var batch = new Batch();
                foreach (var groupSet in Context.Course.GroupSets)
                {
                    batch.Add(groupSet.Id.ToString(), new GetGroupList()
                    {
                        CourseId = courseId,
                        SetId = groupSet.Id
                    });
                }

                if (!batch.Commands.IsNullOrEmpty())
                {
                    SessionManager.CurrentSession.Execute(batch);

                    foreach (var groupSet in Context.Course.GroupSets)
                    {
                        var group = batch.CommandAs<GetGroupList>(groupSet.Id.ToString());
                        result[groupSet] = group.Groups.Map(g => g.ToGroup()).ToList();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a group in the specified owner course.
        /// </summary>
        /// <param name="ownerId">ID of the course to which this group belongs.</param>
        /// <param name="setId">The ID of the group set within the owning entity to which this group belongs.</param>
        /// <param name="groupName">Title for the group.</param>
        /// <param name="domainId">ID of the domain to create the group in.</param>
        /// <returns></returns>
        public string CreateGroup(string ownerId, string setId, string groupName, string domainId)
        {
            string result = string.Empty;

            using (Context.Tracer.StartTrace(string.Format("GroupActions.CreateGroup(ownerId={0}, setId={1}, groupName={2}, domainId={3})", ownerId, setId, groupName, domainId)))
            {
                var group = new Adc.Group()
                {
                    OwnerId = ownerId,
                    SetId = setId,
                    Title = groupName,
                    DomainId = domainId
                };

                var cmd = new CreateGroups();
                cmd.Add(group);

                SessionManager.CurrentSession.Execute(cmd);

                result = cmd.GroupId;
            }

            return result;
        }

        /// <summary>
        /// Adds member enrollment to an existing group.
        /// </summary>
        /// <param name="groupid">ID of the group to add members to.</param>
        /// <param name="enrollmentId">ID of the enrollment to add to the group.</param>
        public void AddGroupMember(string groupid, string enrollmentId)
        {
            using (Context.Tracer.StartTrace(string.Format("GroupActions.AddGroupMember(groupId={0}, enrollmentId={1})", groupid, enrollmentId)))
            {
                var cmd = new AddGroupMembers()
                {
                    GroupMember = new Adc.GroupMember()
                    {
                        GroupId = groupid,
                        EnrollmentId = enrollmentId
                    }
                };
                SessionManager.CurrentSession.Execute(cmd);
            }
        }

        /// <summary>
        /// Flushes the cache after saving a group
        /// </summary>
        public void FlushCourseFromCache()
        {
            Context.CacheProvider.InvalidateCourse(Context.CourseId);
        }
        #endregion
    }
}
