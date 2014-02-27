using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.Common.Caching;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class GroupsController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Actions for groups.
        /// </summary>
        /// <value>
        /// The group actions.
        /// </value>
        protected BizSC.IGroupActions GroupActions { get; set; }

        /// <summary>
        /// Actions for enrollments.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="groupActions">The group actions.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        public GroupsController(BizSC.IBusinessContext context, BizSC.IGroupActions groupActions, BizSC.IEnrollmentActions enrollmentActions)
        {
            Context = context;
            GroupActions = groupActions;
            EnrollmentActions = enrollmentActions;
        }

        /// <summary>
        /// Manages the groups.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageGroups()
        {
            GroupSetOverview groups = new GroupSetOverview()
            {
                CourseTitle = Context.Course.Title,
                StudentCount = GetCourseStudents().Count()
            };

            groups.GroupSets = Context.Course.GroupSets.Map(gs => gs.ToGroupSet());
            ViewData.Model = groups;
            return View();
        }

        /// <summary>
        /// Manages the groups.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageGroupsFne(bool? clearCache)
        {
            if (clearCache.HasValue && clearCache.Value)
            {
                Context.RefreshCourse();
            }

            GroupSetOverview groups = new GroupSetOverview()
            {
                CourseTitle = Context.Course.Title,
                StudentCount = GetCourseStudents().Count()
            };

            groups.GroupSets = Context.Course.GroupSets.Map(gs => gs.ToGroupSet());
            ViewData.Model = groups;
            return View();
        }

        /// <summary>
        /// Return a list of Groups.
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupList()
        {
            ViewData.Model = GroupActions.GetAllGroupSetsWithGroups(Context.Course.Id).ToGroupSets().Where(e => e.Id.ToString() != System.Configuration.ConfigurationManager.AppSettings["IndividualGroupSetId"]);
            return View();
        }

        /// <summary>
        /// Edits the group set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult EditGroupSet(GroupSetEditor.EditType type, int? id)
        {
            ViewData.Model = new GroupSetEditor()
            {
                EnrollmentId = Context.EntityId,
                GroupSetEditType = type,
                GroupSetId = id.HasValue ? id.Value : -1
            };

            return View();
        }

        /// <summary>
        /// Deletes the group set.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult DeleteGroupSet(int id)
        {
            GroupActions.DeleteGroupSet(Context.EntityId, id);
            return new EmptyResult();
        }

        /// <summary>
        /// Students the list.
        /// </summary>
        /// <param name="groupSetId">The group set id.</param>
        /// <returns></returns>
        public ActionResult StudentList(int? groupSetId)
        {
            GroupSet groupSet = new GroupSet();
            bool groupNotFound = false;

            if (groupSetId.HasValue)
            {
                var group = Context.Course.GroupSets.Filter(gs => gs.Id == groupSetId).FirstOrDefault();

                if (group != null)
                {
                    groupSet = group.ToGroupSet();
                    groupSet.Groups = GroupActions.ListGroups(Context.EntityId, groupSet.Id).Map(g => g.ToGroup()).ToList();
                }
                else
                {
                    //throw new ArgumentOutOfRangeException("groupSetId", "Could not locate GroupSet with ID " + groupSetId);
                    groupNotFound = true;
                }
            }
            else
            {
                groupNotFound = true;
            }

            if (groupNotFound)
            {
                groupSet = new GroupSet()
                {
                    Name = GroupSet.AllStudentsName,
                    Id = -1,
                    Groups = new List<Group>(new Group[] { new Group() { Members = GetCourseStudents() } })
                };
            }

            ViewData.Model = groupSet;

            return View();
        }

        /// <summary>
        /// Gets the course students.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Student> GetCourseStudents()
        {
            return EnrollmentActions.GetEntityEnrollments(Context.Course.Id, BizDC.UserType.Student).Map(e => e.ToStudent());
        }

        /// <summary>
        /// Gets the settings tab list.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSettingsTabList()
        {
            var groupSets = GroupActions.GetAllGroupSetsWithGroups(Context.Course.Id).ToGroupSets();

            SelectListItem model = null;
            List<SelectListItem> models = new List<SelectListItem>();
            if (groupSets.Any())
            {
                foreach (var grpSet in groupSets)
                {
                    for (int i = 0; i < grpSet.Groups.Count; i++)
                    {
                        model = new SelectListItem();
                        
                        if (grpSet.Id.ToString() != System.Configuration.ConfigurationManager.AppSettings["IndividualGroupSetId"])
                        {
                            model.Text = grpSet.Name + " - " + grpSet.Groups[i].Name;
                        }
                        else
                        {
                            model.Text = grpSet.Groups[i].Name;
                        }

                        model.Value = grpSet.Id + "," + grpSet.Groups[i].GroupId;

                        if (grpSet.Id.ToString() == System.Configuration.ConfigurationManager.AppSettings["IndividualGroupSetId"])
                        {
                            models.Add(model);
                        }
                        else
                        {
                            models.Insert(0, model);
                        }
                    }
                }

                return Json(models.ToList(), JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="studentId">The student id.</param>
        public string CreateGroup(string groupName, string studentId)
        {
            var group = new PX.Biz.DataContracts.Group();
            group.OwnerId = Context.Course.Id;
            group.SetId = System.Configuration.ConfigurationManager.AppSettings["IndividualGroupSetId"];
            group.Name = groupName;

            var domainId = System.Configuration.ConfigurationManager.AppSettings["DefaultDomainId"];
            var groupId = GroupActions.CreateGroup(Context.Course.Id, System.Configuration.ConfigurationManager.AppSettings["IndividualGroupSetId"], groupName, domainId);

            AddGroupMember(groupId, studentId);

            return groupId;
        }

        /// <summary>
        /// Adds the group member.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <param name="studentId">The student id.</param>
        public void AddGroupMember(string groupId, string studentId)
        {
            var enrollmentid = EnrollmentActions.GetUserEnrollmentId(studentId, Context.EntityId);
            GroupActions.AddGroupMember(groupId, enrollmentid);
        }

        /// <summary>
        /// Redirect based on the user;
        /// </summary>
        /// <returns></returns>
        public ActionResult GradebookScorecard()
        {
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View();
        }

        /// <summary>
        /// Redirect based on the user;
        /// </summary>
        /// <returns></returns>
        public ActionResult GradebookScorecardFne()
        {
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View();
        }

        public ActionResult FlushCourseFromCache()
        {
            GroupActions.FlushCourseFromCache();
            return new EmptyResult();
        }
    }
}
