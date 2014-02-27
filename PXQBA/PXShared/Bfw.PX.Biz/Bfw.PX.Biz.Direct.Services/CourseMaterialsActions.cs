using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Bfw.Common.Logging;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Adc = Bfw.Agilix.DataContracts;

using Bdc = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.ServiceContracts;

using System.Xml.Linq;
using System.Collections;

namespace Bfw.PX.Biz.Direct.Services
{
    public class CourseMaterialsActions : ICourseMaterialsActions
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

        protected IContentActions ContentActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sessionManager"></param>
        public CourseMaterialsActions(IBusinessContext context, ISessionManager sessionManager, IContentActions contentActions)
        {
            Context = context;
            SessionManager = sessionManager;
            ContentActions = contentActions;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the list of course materials for the current course
        /// </summary>
        /// <returns></returns>
        public Bdc.CourseMaterials GetCourseMaterials()
        {
            Bdc.CourseMaterials courseMaterials = new Bdc.CourseMaterials();
            courseMaterials.ResourceList = new List<Bdc.ContentItem>();            

            courseMaterials.ResourceList.AddRange(ContentActions.ListContentForCourseMaterials(Context.EntityId));     
            if (courseMaterials.ResourceList.Count > 0)
            courseMaterials.AssestList.AddRange(ContentActions.ListAssestsForCourseMaterials(Context.EntityId, courseMaterials.ResourceList));

            var testItems = new List<Bdc.ContentItem>();

            foreach (var item in courseMaterials.ResourceList)
            {
                if (item.Metadata.ContainsKey("DOCUMENT_CREATED_DATE"))
                {
                    item.CreatedDate = DateTime.Parse(item.Metadata["DOCUMENT_CREATED_DATE"].Value.ToString());

                }
                var titem = courseMaterials.AssestList.Find(i => i.ParentId == item.Id);                
            }

            courseMaterials.ResourceList = courseMaterials.ResourceList.OrderByDescending(i => i.CreatedDate).ToList();

            courseMaterials.ResourceList.ForEach(r => { r.LockedCourseType = Context.IsCourseReadOnly ? Context.IsCourseReadOnly.ToString() : r.LockedCourseType; });

            return courseMaterials;
        }

        /// <summary>
        /// Deletes a course material resource from an eportfolio course
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public bool DeleteCourseMaterialResource(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
            {
                throw new Exception("itemID is missing [CourseMaterialsActions]DeleteCourseMaterialResource()");
            }

            using (this.Context.Tracer.DoTrace("CourseMaterialsAction.DeleteCourseMaterialResource(itemID = {0})", itemID))
            {
                var cmdDelete = new DeleteItems()
                {
                    Items = new List<Adc.Item>()
                    {
                        new Adc.Item()
                        {
                            EntityId = this.Context.CourseId,
                            Id = itemID                            
                        }
                    }
                };

                SessionManager.CurrentSession.Execute(cmdDelete);
            }

            return false;
        }

        #endregion
    }
}
