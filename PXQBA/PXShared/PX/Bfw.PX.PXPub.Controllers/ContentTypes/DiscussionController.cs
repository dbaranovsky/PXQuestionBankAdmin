using System.Collections.Generic;
using System.Web.Mvc;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class DiscussionController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context. 
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// A public list of the Documents.
        /// </summary>
        public IList<Document> DocumentCollection;

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions. </param>
        /// <param name="helper">The helper.</param>
        public DiscussionController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, ContentHelper helper, AssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
        }

        /// <summary>
        /// Attempts to save the Html Document, if behavior is Cancel then nothing is saved and
        /// the action is redirected to CreateAndAssign
        /// </summary>
        /// <param name="content">The content. <see cref="Discussion"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveDiscussion(Discussion content, string behavior, Assign assign, string toc = "syllabusfilter")
        {
            ActionResult result = null;
            ContentView model = null;
            
            if (behavior != null)
            {
                switch (behavior.ToLowerInvariant())
                {
                    case "cancel":
                        var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                        model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                        result = View("DisplayItem", model);
                        break;

                    case "save":
                        if (ModelState.IsValid)
                        {
                            ContentHelper.StoreDiscussion(content, Context.EntityId);
                            if (assign != null && assign.DueDate.Year > System.DateTime.MinValue.Year)
                            {
                                assign.Id = content.Id;
                                AssignmentCenterHelper.AssignItem(assign, toc);
                            }
                            model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
                            result = View("DisplayItem", model);
                        }
                        else
                        {
                            content.EnvironmentUrl = Context.EnvironmentUrl;
                            content.CourseInfo = Context.Course.ToCourse();
                            content.EnrollmentId = Context.EnrollmentId;
                            content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                            content.Description = System.Web.HttpUtility.HtmlDecode(content.Description);
                            result = View("CreateContent", content);
                        }
                        break;

                    default:
                        result = View();
                        break;
                }
            }

            return result;
        }
    }
}
