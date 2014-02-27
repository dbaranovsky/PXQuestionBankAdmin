using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.DataContracts;
using System.IO;
using Bfw.Common.Database;
using Bfw.Common.Logging;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the IUserActivitiesActions interface.
    /// </summary>
    public class UserActivitiesActions : IUserActivitiesActions
    {
        #region Properties
        
        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }
                
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActivitiesActions"/> class.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        public UserActivitiesActions(IBusinessContext context)
        {
            Context = context;
        }

        #endregion

        #region IUserActivitiesActions Members

        /// <summary>
        /// List of item updates for an enrollment 
        /// </summary>
        /// <param name="enrollmentId">enrollment id of the student</param>
        /// <returns>list of student updates</returns>
        public List<ItemUpdate> GetItemUpdates(string enrollmentId)
        {
            using (Context.Tracer.DoTrace("UserActivitiesActions.GetItemUpdates(enrollmentId={0})", enrollmentId))
            {
                var itemUpdates = new List<Bdc.ItemUpdate>();
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    var records = db.Query("GetItemUpdates @0", enrollmentId);

                    if (!records.IsNullOrEmpty())
                    {
                        itemUpdates = records.Map(r => new Bdc.ItemUpdate()
                        {
                            CourseId = r.String("CourseId"),
                            EnrollmentId = r.String("EnrollmentId"),
                            ItemId = r.String("ItemId")
                        }).ToList();
                    }
                }
                finally
                {
                    db.EndSession();
                }

                return itemUpdates;
            }
        }

        /// <summary>
        /// List of item update count by enrollment
        /// </summary>
        /// <param name="enrollmentId">enrollemnt id</param>
        /// <returns>List of item update count</returns>
        public List<KeyValuePair<string, int>> GetItemUpdateCount(string courseId)
        {
            using (Context.Tracer.DoTrace("UserActivitiesActions.GetItemUpdates(courseId={0})", courseId))
            {
                var updateCount = new List<KeyValuePair<string, int>>();
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    var records = db.Query("GetItemUpdateCountByEnrollment @0", courseId);

                    if (!records.IsNullOrEmpty())
                    {
                        updateCount = records.Map(r => new KeyValuePair<string, int>(r.String("EnrollmentId"), r.Int32("UpdateCount"))).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Context.Logger.Log("error connecting to database PXData", Common.Logging.LogSeverity.Error);
                }
                finally
                {
                    db.EndSession();
                }

                return updateCount;
            }
        }

        /// <summary>
        /// Mark an item as updated
        /// </summary>
        /// <param name="update">updated item details</param>
        public void MarkItemAsUpdated(ItemUpdate update)
        {
            using (Context.Tracer.StartTrace("UserActivitiesActions.MarkItemAsUpdated(update)"))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("FlagItemAsUpdated @0, @1, @2", update.CourseId, update.EnrollmentId, update.ItemId);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        /// <summary>
        /// Unmark the item as updated
        /// </summary>
        /// <param name="update">updated item details</param>
        public void UnmarkItemAsUpdated(ItemUpdate update)
        {
            using (Context.Tracer.StartTrace("UserActivitiesActions.UnmarkItemAsUpdated(update)"))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("ResetItemUpdatedFlag @0, @1, @2", update.CourseId, update.EnrollmentId, update.ItemId);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        /// <summary>
        /// Remove item updates data
        /// </summary>
        /// <param name="removeBefore">till date</param>
        public void CleanupItemUpdatesData(DateTime removeBefore)
        {
            using (Context.Tracer.StartTrace("UserActivitiesActions.CleanupItemUpdatesData(removeBefore)"))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("RemoveItemUpdates @0", removeBefore);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        /// <summary>
        /// Delete a single item update data
        /// </summary>
        /// <param name="update">item detail</param>
        public void DeleteItemUpdateData(ItemUpdate update)
        {
            using (Context.Tracer.StartTrace("UserActivitiesActions.DeleteItemUpdateData(update)"))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("RemoveItemUpdate @0, @1, @2", update.CourseId, update.EnrollmentId, update.ItemId);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }
        
        #endregion

    }
}
