using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Abstractions;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.Biz.Direct.Services;
using System.Configuration;
using Bfw.PX.PXPub.Models;
using System.Web;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class SocialCommentingWidgetController : Controller
    {
        protected IBusinessContext Context { get; set; }
        protected IContentActions Content { get; set; }
        protected bool DebuggingMode = false;
        public SocialCommentingWidgetController(IBusinessContext context, IContentActions content)
        {
            Context = context;
            Content = content;
            DebuggingMode = IsDebug();

        }
        /// <summary>
        /// Checks the compilation section in the web.config and returns true if the debug 
        /// attribute is set.
        /// </summary>
        /// <returns>True if the compilation.debug flag is true, false otherwise.</returns>
        protected static bool IsDebug()
        {
            bool debug = true;

            var compilation = ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;

            if (compilation != null)
            {
                debug = compilation.Debug;
            }

            return debug;
        }

        public ActionResult ChangeCommentingFlagOnItem(String itemId, bool enableCommenting)
        {
            var item = Content.GetContent(Context.Course.Id, itemId);
            
            if (null == item)
                return null;
                
            item.SocialCommentingIntegration = enableCommenting;
            Content.StoreContent(item, Context.Course.Id);
            
            return DisplayCommenting(item);
        }

        public ActionResult DisplayCommenting(dynamic item)
        {
            // determine if the course has commenting enabled
            bool IsSocialCommentingEnabledOnCourse = Context.Course.SocialCommentingIntegration;
            bool IsSocialCommentingEnabledOnItem = false;

            var modelItem = item as Models.ContentItem;
            var bizItem = item as Biz.DataContracts.ContentItem;
            var strItem = item as String[];

            if (null != strItem && strItem.Length > 0 && !String.IsNullOrEmpty(strItem[0]))
            {
                bizItem = Content.GetContent(Context.Course.Id, strItem[0]);
            }

            var itemid = String.Empty;
            // if only the id was passed in, use the id to load the entire item
            if (null != modelItem)
            {
                IsSocialCommentingEnabledOnItem = modelItem.SocialCommentingIntegration;
                ViewData["DisQusIdentifier"] = modelItem.Id + Context.Course.CourseOwner;
                itemid = modelItem.Id;
            }
            else if (null != bizItem)
            {
                IsSocialCommentingEnabledOnItem = bizItem.SocialCommentingIntegration;
                ViewData["DisQusIdentifier"] = bizItem.Id + Context.Course.CourseOwner;
                itemid = bizItem.Id;
            }

            // hide buttons when in preview mode
            var previewAsVisitorCookie = Request.Cookies[Context.PreviewAsVisitorCookieKey];
            var isPublicView = Context.IsPublicView;
            if (previewAsVisitorCookie != null)
            {
                isPublicView = true;
            }
            ViewData["HideButtons"] = isPublicView || Context.IsAnonymous || Context.IsCourseReadOnly;

            // until we can figure out how this will work in general
            // this might be a config property
            ViewData["DisQusShortName"] = "kclocal";

            var rc = new RequestContext(new HttpContextWrapper(System.Web.HttpContext.Current), this.RouteData);
            var helper = new UrlHelper(rc);

           

            ViewData["DisQusUrl"] = helper.RouteUrl("Assignment", new
                {
                    id = itemid,
                    mode = ContentViewMode.Preview,
                    includeDiscussion = true,
                    readOnly = true,
                    category = Context.Course.CourseOwner
                });
            // determine the url to view this comment page again
            ViewData["DisQusDebug"] = DebuggingMode ? 1 : 0;
            ViewData["IsSocialCommentingEnabledOnCourse"] = IsSocialCommentingEnabledOnCourse.ToString().ToLowerInvariant();
            ViewData["IsSocialCommentingEnabledOnItem"] = IsSocialCommentingEnabledOnItem.ToString().ToLowerInvariant();
            return View("DisplayCommenting");
        }
    }
}
