using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Logging;
using Bfw.Common.Collections;
using Bfw.Common.Database;

namespace Bfw.PX.Biz.Direct.Services
{
    public class SharedCourseActions : ISharedCourseActions
    {

        #region Properties

		/// <summary>
		/// The IBusinessContext implementation to use.
		/// </summary>
		protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The IEnrollmentActions implementation to use.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        protected ICourseActions CourseActions { get; set; }

        protected IUserActions UserActions { get; set; }

        protected IDatabaseManager DbManager { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		///
		/// </summary>
		/// <param name="context">The business context.</param>
        public SharedCourseActions(IBusinessContext context, IEnrollmentActions enrollmentActions, ICourseActions courseActions, IUserActions userActions, IDatabaseManager dbManager)
		{
			Context = context;
            EnrollmentActions = enrollmentActions;
            CourseActions = courseActions;
            UserActions = userActions;
		    DbManager = dbManager;
		}

		#endregion

		#region IShareCourseActions Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharingUserId"></param>
        /// <returns></returns>
        public List<SharedCourseDefinition> getSharedCoursesBy(String sharingUserId)
        {
            List<SharedCourseDefinition> foundSharedCourseDefinitions = new List<SharedCourseDefinition>();
            using (Context.Tracer.DoTrace("ShareCourseActions.getSharedCoursesBy(sharingUserId={0}) - db (GetSharedCourseDefinitionBySharingId)", sharingUserId))
            {
                DbManager.ConfigureConnection("PXData");
                
                try
                {
                    DbManager.StartSession();
                    var courseRecords = DbManager.Query("GetSharedCourseDefinitionBySharingId @0", sharingUserId);

                    if (null != courseRecords)
                    {
                        foreach (var courseRecord in courseRecords)
                        {
                            foundSharedCourseDefinitions.Add(new SharedCourseDefinition()
                            {
                                OwnersUserId = courseRecord["UserId"].ToString(),
                                SharedCourseId = courseRecord["CourseId"].ToString(),
                            });
                        }
                    }
                }
                finally
                {
                    DbManager.EndSession();
                }
            }

            List<SharedCourseDefinition> sharedCourseDefinitions = new List<SharedCourseDefinition>();
            foreach (var sharedCourseDefinition in foundSharedCourseDefinitions)
            {
                sharedCourseDefinitions.AddRange(getSharedCoursesBy(sharedCourseDefinition.SharedCourseId, sharedCourseDefinition.OwnersUserId));
            }

            return sharedCourseDefinitions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        public List<SharedCourseDefinition> getSharedCoursesBy(String sharedCourseId, String sharedUserId, bool onlyActive = true)
        {
            Dictionary<String, SharedCourseDefinition> sharedCourseDefinitions = new Dictionary<string,SharedCourseDefinition>();

            using (Context.Tracer.DoTrace("ShareCourseActions.getSharedCourseDefintion(sharedCourseId={0}, sharedUserId={1}) - db (GetSharedCourseDefinition)", sharedCourseId, sharedUserId))
            {
                DbManager.ConfigureConnection("PXData");
                try
                {
                    DbManager.StartSession();
                    var courseRecords = DbManager.Query("GetSharedCourseDefinition @0, @1", sharedCourseId, sharedUserId);
                    var userRecords = DbManager.Query("GetSharedCourseDefinitionUsers @0, @1", sharedCourseId, sharedUserId);
                    var itemRecords = DbManager.Query("GetSharedCourseDefinitionItems @0, @1", sharedCourseId, sharedUserId);

                    if (null != courseRecords)
                    {
                        foreach (var courseRecord in courseRecords)
                        {
                            sharedCourseDefinitions.Add(courseRecord["SharedCourseId"].ToString(), 
                                new SharedCourseDefinition()
                            {
                                ID = courseRecord["SharedCourseId"].ToString(),
                                Note = courseRecord["Note"] as String,
                                AnonyousName = courseRecord["AnonymousName"] as String,
                                OwnersUserId = courseRecord["UserId"].ToString(),
                                SharedCourseId = courseRecord["CourseId"].ToString(),
                                SharedItemIds = new List<String>(),
                                SharedUserIds = new List<String>()
                            });
                        }
                    }

                    
                    if (null != userRecords)
                    {
                        foreach (var userRecord in userRecords)
                        {
                            String _sharedCourseId = userRecord["SharedCourseId"].ToString();
                            String _sharedWithUserId = userRecord["UserId"].ToString();
                            bool _isActive = Boolean.Parse(userRecord["IsActive"].ToString());
                            if (sharedCourseDefinitions.ContainsKey(_sharedCourseId))
                            {
                                var list = _isActive ?
                                    sharedCourseDefinitions[_sharedCourseId].SharedUserIds :
                                    sharedCourseDefinitions[_sharedCourseId].SharedInactiveUserIds;

                                if (!list.Contains(_sharedWithUserId))
                                {
                                    list.Add(_sharedWithUserId);
                                }
                            }
                        }
                    }

                    if (null != itemRecords)
                    {
                        foreach (var itemRecord in itemRecords)
                        {
                            String _sharedCourseId = itemRecord["SharedCourseId"].ToString();
                            String _sharedWithItemId = itemRecord["ItemId"].ToString();
                            bool _isActive = Boolean.Parse(itemRecord["IsActive"].ToString());
                            if (sharedCourseDefinitions.ContainsKey(_sharedCourseId))
                            {
                                var list = _isActive ?
                                    sharedCourseDefinitions[_sharedCourseId].SharedItemIds :
                                    sharedCourseDefinitions[_sharedCourseId].SharedInactiveItemIds;

                                if (!list.Contains(_sharedWithItemId))
                                {
                                    list.Add(_sharedWithItemId);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DbManager.EndSession();
                }
            }

            // filter out the ones that are not being shared with anyone and the ones that have nothing to share
            var definitions = sharedCourseDefinitions.Map(x => x.Value).ToList<SharedCourseDefinition>();

            // remove all shares that do not have any active instructors that are being shared or nothing in the eportfolio is being shared
            if (onlyActive)
                definitions = definitions.FindAll(x => !(x.SharedUserIds.IsNullOrEmpty() || x.SharedItemIds.IsNullOrEmpty())).ToList(); 

            return definitions;
        }

        /// <summary>
        /// Gets the list of courses which are shared with a user
        /// </summary>
        /// <param name="sharingUserId"></param>
        /// <param name="onlyActive">true to show active course</param>
        /// <returns></returns>
        public IEnumerable<DashboardItem> getSharedCourses(String sharingUserId, bool onlyActive = true)
        {
            var dashboardItems = new List<DashboardItem>();
            var sharedCourseDefinitions = getSharedCoursesBy(sharingUserId);

            if (onlyActive)
            {
                sharedCourseDefinitions = sharedCourseDefinitions.Filter(sc => !sc.SharedInactiveUserIds.Contains(sharingUserId)).ToList();
            }
            if (sharedCourseDefinitions.IsNullOrEmpty())
                return dashboardItems;

            // Batch get courses and users info instead of getting each in foreach loop
            var sharedCourses = CourseActions.GetCoursesByCourseIds(sharedCourseDefinitions.Where(c => !string.IsNullOrEmpty(c.SharedCourseId)).Select(c => c.SharedCourseId).Distinct());
            if (sharedCourses.IsNullOrEmpty())
                return dashboardItems;
            // Get instructors & students user information
            var users =
                UserActions.ListUsers(
                    sharedCourses.Where(c => !string.IsNullOrEmpty(c.CourseOwner))
                        .Select(c => c.CourseOwner)
                        .Union(
                            sharedCourseDefinitions.Where(sharedCourseDefinition => !string.IsNullOrEmpty(sharedCourseDefinition.OwnersUserId))
                                .Select(sharedCourseDefinition => sharedCourseDefinition.OwnersUserId))
                        .Distinct());
            var addedCourses = new Dictionary<string, DashboardItem>();
            foreach (var sharedCourseDefinition in sharedCourseDefinitions)
            {
                var sharedCourse =
                    sharedCourses.FirstOrDefault(course => course.Id == sharedCourseDefinition.SharedCourseId);
                // If course is deleted
                if (null == sharedCourse)
                    continue;
                var ownerId = sharedCourse.CourseOwner;
                var owner = users.FirstOrDefault(user => user.Id == ownerId);
                var sharedStudent =
                    users.FirstOrDefault(user => user.Id == sharedCourseDefinition.OwnersUserId);
                
                // If user is removed
                if (null == owner || null == sharedStudent)
                    continue;
                var ownerName = sharedCourseDefinition.AnonyousName.IsNullOrEmpty() ? owner.FormattedName : sharedCourseDefinition.AnonyousName;

                //If this course has not been added, then add this course to the list
                if (!addedCourses.ContainsKey(sharedCourse.Id))
                {
                    var notes = new Dictionary<string, string>();
                    var sharedUsers = new Dictionary<string, string>();
                    notes.Add(sharedCourseDefinition.OwnersUserId, sharedCourseDefinition.Note);
                    sharedUsers.Add(sharedStudent.Id, sharedStudent.FormattedName);

                    var dashboardItem = new DashboardItem()
                    {
                        CourseId = sharedCourse.Id,
                        CourseTitle = sharedCourse.Title,
                        OwnerId = ownerId,
                        OwnerName = ownerName,
                        OwnerFirstName = owner.FirstName,
                        OwnerLastName = owner.LastName,
                        OwnerEmail = owner.Email,
                        Count = sharedCourseDefinition.SharedUserIds.Count,
                        Notes = notes,
                        Users = sharedUsers
                    };
                    dashboardItems.Add(dashboardItem);
                    addedCourses.Add(sharedCourse.Id, dashboardItem);
                }
                else
                {
                    //Else, add the shared user to the course
                    var existingSharedCourse = addedCourses[sharedCourse.Id];
                    existingSharedCourse.Notes.Add(sharedStudent.Id, sharedCourseDefinition.Note);
                    existingSharedCourse.Users.Add(sharedStudent.Id, sharedStudent.FormattedName);
              
                }
                
            }
            return dashboardItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        public SharedCourseDefinition getSharedCourseDefintion(String sharedCourseId, String sharedUserId, bool onlyActive = true)
        {
            var sharedCourseDefinitions = getSharedCoursesBy(sharedCourseId, sharedUserId);
            return sharedCourseDefinitions.FirstOrDefault();
        }

        /// <summary>
        /// Gets the list of Users with whom the course has been shared with
        /// </summary>
        /// <param name="sharedCourseId">shared courseid</param>
        /// <returns></returns>
        public IEnumerable<UserInfo> getSharedToUsers(String sharedCourseId)
        {
            var sharedToUserIds = new List<string>();

            getSharedCoursesBy(sharedCourseId, null).ForEach( sd => sharedToUserIds.AddRange(sd.SharedUserIds));
            sharedToUserIds = sharedToUserIds.Distinct().ToList();
            var sharedToUsers = sharedToUserIds.Select(uid => UserActions.GetUser(uid));

            return sharedToUsers;
        }

        /// <summary>
        /// Gets the list of users whose eportfolio has been shared
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TocCategory> getSharedUsers(String sharedCourseId)
        {
            var sharedCourseDefinitions = getSharedCoursesBy(sharedCourseId, null);
            var sharedUsers = new List<TocCategory>();

            foreach (var sharedCourseDefinition in sharedCourseDefinitions)
            {
                var userInfo = UserActions.GetUser(sharedCourseDefinition.OwnersUserId);
                var userEnrollmentId = string.Format("enrollment_{0}", EnrollmentActions.GetEnrollment(userInfo.Id, sharedCourseId).Id);
                var userName = string.Empty;
                if (!sharedCourseDefinition.AnonyousName.IsNullOrEmpty())
                {
                    userName = sharedCourseDefinition.AnonyousName;
                }
                else
                {
                    userName = string.Format("{0} ({1})", userInfo.FormattedName, sharedCourseDefinition.SharedItemIds.Count);
                }
                
                var sharedUser = new TocCategory { Id = userEnrollmentId, Text = userName };
                sharedUsers.Add(sharedUser);
            }

            return sharedUsers;
        }

        /// <summary>
        /// Gets the list of items shared
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="sharedUserId"></param>
        /// <returns></returns>
        public IList<string> getSharedItems(String sharedEnrollmentId)
        {
            var sharedCourseDefinition = getSharedCourseDef(sharedEnrollmentId, Context.EntityId);
            return sharedCourseDefinition.SharedItemIds;
        }

        /// <summary>
        /// Get the shared note for a given user
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        public String getSharedNote(String sharedEnrollmentId)
        {
            var sharedCourseDefinition = getSharedCourseDef(sharedEnrollmentId, Context.EntityId);
            return sharedCourseDefinition.Note;
        }

        /// <summary>
        /// Gets the Shared Course details given an EnrollmentID
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        public SharedCourseDefinition getSharedCourseDef(String sharedEnrollmentId, string sharedCourseId)
        {
            var sharedUserId = GetUserId(sharedEnrollmentId, sharedCourseId);

            return getSharedCourseDefintion(sharedCourseId, sharedUserId);
        }

        /// <summary>
        /// Get the userid given the enrollmentid of the user for the current course
        /// </summary>
        /// <param name="sharedEnrollmentId"></param>
        /// <returns></returns>
        public String GetUserId(String sharedEnrollmentId, string sharedCourseId)
        {
            var sharedUserId = string.Empty;
            var enrollments = EnrollmentActions.GetEntityEnrollments(sharedCourseId);
            if (!enrollments.IsNullOrEmpty())
            {
                var userEnrollment = enrollments.ToList().Find(e => e.Id == sharedEnrollmentId);
                sharedUserId = null != userEnrollment ? userEnrollment.User.Id : string.Empty;
            }
            return sharedUserId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        public IList<string> getListOfSharedUserIds(String sharedCourseId)
        {
            List<String> sharedUserIds = new List<String>();
            var sharedCourseDefinitions = getSharedCoursesBy(sharedCourseId, null);

            foreach (var sharedCourseDefinition in sharedCourseDefinitions)
                sharedUserIds.AddRange(sharedCourseDefinition.SharedUserIds);

            return sharedUserIds.Distinct().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sharedCourseId"></param>
        /// <param name="ownerUserId"></param>
        /// <returns></returns>
        public IList<UserInfo> getListOfSharedUsers(String sharedCourseId, Bfw.PX.Biz.ServiceContracts.IUserActions userActions)
        {
            var listOfUserIds = getListOfSharedUserIds(sharedCourseId);
            var listOfUsers = userActions.ListUsers(listOfUserIds);
            return listOfUsers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CourseId"></param>
        /// <returns></returns>
        public bool IsCourseSharedWithCurrentUser(String sharedCourseId, String sharedUserId)
        {
            var allowShare = false;
            var sharedCourseDefinition = getSharedCoursesBy(sharedCourseId, sharedUserId);

            if (sharedCourseDefinition.IsNullOrEmpty() &&
                !sharedCourseDefinition[0].SharedUserIds.IsNullOrEmpty() &&
                !sharedCourseDefinition[0].SharedItemIds.IsNullOrEmpty() &&
                sharedCourseDefinition[0].SharedUserIds.Contains(Context.CurrentUser.Id))
            {
                allowShare = true;
            }
            
            return allowShare;
        }

        /// <summary>
        /// Generates a unique anonymous name for the shared user
        /// </summary>
        /// <returns></returns>
        public string getAnonymousName(string sharedCourseId, string anonymousPrefix)
        {
            var anonymousName = string.Empty;
            var maxid = 1;

            using (Context.Tracer.DoTrace("ShareCourseActions.GetAnonymousName(sharedCourseId={0}, anonymousPrefix={1})", sharedCourseId, anonymousPrefix))
            {
                DbManager.ConfigureConnection("PXData");

                try
                {
                    DbManager.StartSession();
                    var anonymousRecords = DbManager.Query("GetSharedCourseAnonymousUsers @0", sharedCourseId);

                    if (!anonymousRecords.IsNullOrEmpty())
                    {
                        var anonymousNames = anonymousRecords.Select(ar => ar["AnonymousName"] != null ? ar["AnonymousName"].ToString() : string.Empty);
                        maxid = anonymousNames.Select(an => Convert.ToInt32(Regex.Split(an, anonymousPrefix).ToList().Last())).Max();
                        maxid++;
                    }
                }
                finally
                {
                    DbManager.EndSession();
                }
            }

            anonymousName = string.Format("{0} {1}", anonymousPrefix, maxid);
            return anonymousName;
        }
        
        public void Store(SharedCourseDefinition sharedCourseDefinition)
        {
            var current = getSharedCoursesBy(sharedCourseDefinition.SharedCourseId, sharedCourseDefinition.OwnersUserId, false).FirstOrDefault();

            DbManager.ConfigureConnection("PXData");
            try
            {
                DbManager.StartSession();

                if (null == current)
                {
                    DbManager.ExecuteNonQuery("AddSharedCourse @0, @1, @2, @3", 
                        sharedCourseDefinition.SharedCourseId,
                        sharedCourseDefinition.OwnersUserId,
                        sharedCourseDefinition.AnonyousName,
                        sharedCourseDefinition.Note);

                    foreach (String itemId in sharedCourseDefinition.SharedItemIds)
                    {
                        DbManager.ExecuteNonQuery("AddSharedCourseItem @0, @1, @2",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            itemId);
                    }
                    foreach (String userId in sharedCourseDefinition.SharedUserIds)
                    {
                        DbManager.ExecuteNonQuery("AddSharedCourseUser @0, @1, @2",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            userId);
                    }
                }
                else
                {
                    DbManager.ExecuteNonQuery("UpdateSharedCourse @0, @1, @2, @3",
                        sharedCourseDefinition.SharedCourseId,
                        sharedCourseDefinition.OwnersUserId,
                        sharedCourseDefinition.AnonyousName,
                        sharedCourseDefinition.Note);

                    var EmptyEnumerable = new List<String>();

                    var sharedItems = sharedCourseDefinition.SharedItemIds.Intersect(current.SharedItemIds ?? EmptyEnumerable).ToArray();
                    var sharedUsers = sharedCourseDefinition.SharedUserIds.Intersect(current.SharedUserIds ?? EmptyEnumerable).ToArray();

                    var itemIdsThatNeedToBeActive = (current.SharedInactiveItemIds ?? EmptyEnumerable).Intersect(sharedCourseDefinition.SharedItemIds).ToArray();
                    var userIdsThatNeedToBeActive = (current.SharedInactiveUserIds ?? EmptyEnumerable).Intersect(sharedCourseDefinition.SharedUserIds).ToArray();

                    var itemIdsThatNeedToBeAdded = sharedCourseDefinition.SharedItemIds.Filter(itemid => !sharedItems.Contains(itemid) && !current.SharedInactiveItemIds.Contains(itemid)).ToArray();
                    var userIdsThatNeedToBeAdded = sharedCourseDefinition.SharedUserIds.Filter(userid => !sharedUsers.Contains(userid) && !current.SharedInactiveUserIds.Contains(userid)).ToArray();

                    var itemIdsThatNeedToBeDeActivated = current.SharedItemIds.Filter(itemid => !sharedItems.Contains(itemid)).ToArray();
                    var userIdsThatNeedToBeDeActivated = current.SharedUserIds.Filter(userid => !sharedUsers.Contains(userid)).ToArray();

                    foreach (String itemId in itemIdsThatNeedToBeActive)
                    {
                        DbManager.ExecuteNonQuery("UpdateSharedCourseItem @0, @1, @2, @3",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            itemId,
                            true);
                    }
                    foreach (String itemId in itemIdsThatNeedToBeDeActivated)
                    {
                        DbManager.ExecuteNonQuery("UpdateSharedCourseItem @0, @1, @2, @3",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            itemId,
                            false);
                    }
                    foreach (String itemId in itemIdsThatNeedToBeAdded)
                    {
                        DbManager.ExecuteNonQuery("AddSharedCourseItem @0, @1, @2",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            itemId);
                    }
                    foreach (String userId in userIdsThatNeedToBeActive)
                    {
                        DbManager.ExecuteNonQuery("UpdateSharedCourseUser @0, @1, @2, @3",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            userId,
                            true);
                    }
                    foreach (String userId in userIdsThatNeedToBeDeActivated)
                    {
                        DbManager.ExecuteNonQuery("UpdateSharedCourseUser @0, @1, @2, @3",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            userId,
                            false);
                    }
                    foreach (String userId in userIdsThatNeedToBeAdded)
                    {
                        DbManager.ExecuteNonQuery("AddSharedCourseUser @0, @1, @2",
                            sharedCourseDefinition.SharedCourseId,
                            sharedCourseDefinition.OwnersUserId,
                            userId);
                    }
                }
            }
            finally
            {
                DbManager.EndSession();
            }
        }

        #endregion
    }
}
