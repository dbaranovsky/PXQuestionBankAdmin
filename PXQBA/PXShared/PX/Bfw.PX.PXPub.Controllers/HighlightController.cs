using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    [HandleError]
    public class HighlightController : Controller
    {
        private const string ITEM_ID = "PX_QUICKLINKS";
        private const string ABOUT_KEY_ID = "PX_ABOUT_ADD_NOTE";

        #region Properties

        /// <summary>
        /// Contains business layer Content information
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Contains business layer context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the note library actions.
        /// </summary>
        /// <value>
        /// The note library actions.
        /// </value>
        protected BizSC.INoteLibraryActions NoteLibraryActions { get; set; }

        /// <summary>
        /// Gets or sets the note actions.
        /// </summary>
        /// <value>
        /// The note actions.
        /// </value>
        protected BizSC.INoteActions NoteActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// IResponseProxy implementation to use.
        /// </summary>
        protected IResponseProxy ResponseProxy { get; set; }


        /// <summary>
        /// IUserActions implementation to use
        /// </summary>
        protected BizSC.IUserActions UserActions { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="noteLibrary">The note library.</param>
        /// <param name="noteActions">The note actions.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="gradeActions">The grade actions.</param>
        public HighlightController(BizSC.IBusinessContext context, BizSC.INoteLibraryActions noteLibrary, BizSC.INoteActions noteActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IContentActions contentActions, BizSC.IGradeActions gradeActions, IResponseProxy responseProxy, BizSC.IUserActions userActions)
        {
            Context = context;
            NoteLibraryActions = noteLibrary;
            NoteActions = noteActions;
            EnrollmentActions = enrollmentActions;
            ContentActions = contentActions;
            GradeActions = gradeActions;
            ResponseProxy = responseProxy;
            UserActions = userActions;
        }

        #region Action Methods

        /// <summary>
        /// Displays the document viewer control with appropriate settings passed in through the DocumentToView model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult DocumentViewer(DocumentToView model)
        {
            return View("DocumentViewer", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult HighlightCollection(DocumentToView model)
        {
            JsonResult result = Json(new { notesHtml = string.Empty, highlights = string.Empty });
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            ViewData["CurrentUserId"] = Context.CurrentUser.Id;

            if (!Context.CourseIsProductCourse)
            {

                model.UserId = Context.IsAnonymous ? "-1" : Context.CurrentUser.Id;
                

                List<BizDC.Highlight> hResults;
                List<Bfw.PX.PXPub.Models.HighlightModel> hList;

                if (model.HighlightType == (int)PxHighlightType.WritingAssignment)
                {
                    if (Context.AccessLevel == BizSC.AccessLevel.Student && String.IsNullOrEmpty(model.SecondaryId))
                        model.SecondaryId = Context.EnrollmentId;
                    var user = EnrollmentActions.GetEntityEnrollmentsAsAdmin(Context.EntityId).Where(i => i.Id == model.SecondaryId).FirstOrDefault();
                    var userId = user == null ? model.UserId : user.User.Id;
                    var noteSearch = new BizDC.NoteSearch
                    {
                        CourseId = Context.Course.Id,
                        EnrollmentId = model.SecondaryId,
                        ItemId = model.ItemId,
                        NoteId = model.NoteId,
                        ReviewId = model.PeerReviewId,
                        UserId = userId,
                        CurrentUserId = userId,
                        NoteType = (int)BizDC.NoteType.None,
                        HighlightType = (BizDC.PxHighlightType)model.HighlightType,
                        NotePublic = model.Shared
                    };

                    hResults = NoteActions.GetHighlights(noteSearch, GradeActions).ToList();
                    //HighlightStatus.hide actually means the highlight was deleted.  HighlightStatus.Deleted is no longer used. 
                    hList = hResults.Select(hResult => hResult.ToHighlight(Context)).Where(hResult => !hResult.Notes.IsNullOrEmpty() && hResult.Status != HighlightStatus.Hide).ToList();

                    hResults = (from c in hResults where ((c.UserId == Context.CurrentUser.Id) || (!c.Notes.IsNullOrEmpty())) select c).ToList();

                }
                else
                {
                    hResults = NoteActions.GetHighlights(model.ToNoteSearch(Context), GradeActions).ToList();
                    //HighlightStatus.hide actually means the highlight was deleted.  HighlightStatus.Deleted is no longer used.
                    hList = hResults.Select(hResult => hResult.ToHighlight(Context)).Where(hResult => !hResult.Notes.IsNullOrEmpty() && hResult.Status != HighlightStatus.Hide).ToList();

                    hResults = (from c in hResults where ((c.UserId == Context.CurrentUser.Id) || (!c.Notes.IsNullOrEmpty())) select c).ToList();
                }

                //Replace the user's firstname/lastname from the highlighting DB with the info inside UserAction service.
                for (int i = 0; i < hList.Count; i++)
                {
                    var user1 = UserActions.GetUser(hList[i].UserId);
                    hList[i].FirstName = user1.FirstName;
                    hList[i].LastName = user1.LastName;

                    for (int j = 0; j < hList[i].Notes.Count; j++)
                    {
                        var user2 = UserActions.GetUser(hList[i].Notes[j].UserId);
                        hList[i].Notes[j].FirstName = user2.FirstName;
                        hList[i].Notes[j].LastName = user2.LastName;
                    }

                }

                if (model.ShowRubrics)
                {
                    foreach (var highlight in hList)
                    {
                        highlight.ShowRubrics = true;
                        highlight.RubricsGuide = !string.IsNullOrEmpty(model.RubricsList)
                                        ? model.RubricsList.Split('|') : null;
                    }
                }
                string viewHtml;
                if (model.IsReflectionAssignment)
                {
                    viewHtml = GetViewHtml("TopNotesCollection", hList);
                }
                else
                {
                    var vd = new ViewDataDictionary();
                    if (Context != null && Context.CurrentUser != null)
                        vd["CurrentUserId"] = Context.CurrentUser.Id;
                    viewHtml = GetViewHtml("HighlightCollection", hList, vd);
                }

                foreach (BizDC.Highlight hl in hResults)
                {
                    NoteActions.SetClassName(hl);
                }
                var highlightDetails = (from h in hResults orderby h.Created ascending select new { h.HighlightId, h.Text, h.ClassName, h.Start, h.StartOffset, h.End, h.EndOffset, h.Status }).ToList();

                result = Json(new { notesHtml = viewHtml, highlights = highlightDetails });
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            return result;
        }

        /// <summary>
        /// Displays a list of comments for the given highlight id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        public ActionResult CommentList(string id, PxHighlightType highlightType)
        {
            return View(GetNoteList(id, highlightType));
        }

        /// <summary>
        /// Displays a highlight block/bubble with form elements for additional comments and actions.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        public ActionResult Show(string id, PxHighlightType highlightType)
        {
            var hl = NoteActions.GetHighlightByHighlightId(id, highlightType);
            return View(hl.ToHighlight(Context));
        }

        /// <summary>
        /// Loads up a highlight form with the selected text.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult NewHighlightForm(DocumentToView model)
        {
            model.HighlightDescription = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.HtmlDecode(model.HighlightDescription));
            ViewData["AllowCommentSharing"] = Context.Course.AllowCommentSharing;
            ViewData["FormattedName"] = Context.CurrentUser.FormattedName;
            //by default make instructor notes as public
            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                model.Shared = true;
                model.IsInstructor = true;
            }
            return View("Create", model);
        }

        /// <summary>
        /// Insert a new highlight and associated comment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(DocumentToView model)
        {
            model.UserId = Context.IsAnonymous ? "" : Context.CurrentUser.Id;
            var highlight = model.ToHighlight(Context);
            Guid? highlightId, noteId;

            if (highlight.HighlightId != Guid.Empty)
            {
                highlightId = highlight.HighlightId;
                var note = highlight.Notes.FirstOrDefault();
                note.HighlightId = highlightId;
                note.Description = highlight.Description;
                note.ItemId = highlight.ItemId;
                note.ReviewId = highlight.ReviewId;
                note.UserId = highlight.UserId;
                note.FirstName = highlight.FirstName;
                note.LastName = highlight.LastName;
                note.EnrollmentId = highlight.EnrollmentId;
                note.Public = highlight.Public;
                note.HighlightType = highlight.HighlightType;
                note.Status = highlight.Status;
                note.Public = false;
                NoteActions.AddNoteToHighlight(note);
                noteId = note.NoteId;
            }
            else if (string.IsNullOrEmpty(model.HighlightText))
            {
                highlightId = null;
                var note = highlight.Notes.FirstOrDefault();
                note.HighlightId = null;
                note.ItemId = highlight.ItemId;
                note.Description = highlight.Description;
                note.ReviewId = highlight.ReviewId;
                note.CourseId = highlight.CourseId;
                note.UserId = highlight.UserId;
                note.FirstName = highlight.FirstName;
                note.LastName = highlight.LastName;
                note.EnrollmentId = highlight.EnrollmentId;
                note.Status = highlight.Status;
                note.Public = highlight.Public;
                note.HighlightType = highlight.HighlightType;
                note.IsGeneral = true;

                noteId = NoteActions.AddNote(note);
            }
            else
            {
                NoteActions.AddHighlightNote(highlight, out highlightId, out noteId);
            }

            // When creating newnote, user may click on the share or lock button after entering newnote.
            if (model.Shared)
            {
                NoteActions.ShareHighlight(model.HighlightId.ToString(), model.Shared);
            }
            if (model.Locked)
            {
                var status = model.Locked ? HighlightStatus.Locked : HighlightStatus.Active;
                NoteActions.UpdateHighlightNoteStatus(model.HighlightId.ToString(), status, true, Context.EntityId);
            }

            if (highlightId.HasValue) highlight.HighlightId = highlightId.Value;
            if (noteId.HasValue) highlight.Notes.FirstOrDefault().NoteId = noteId.Value;
            var highlightmodel = highlight.ToHighlight(Context);
            if (model.ShowRubrics)
            {
                //code required for eportfolio rubrics.
                highlightmodel.RubricsGuide = (!model.RubricsGuide.IsNullOrEmpty() && !model.RubricsGuide[0].IsNullOrEmpty()) ? model.RubricsGuide[0].Split('|') : null;
                highlightmodel.ShowRubrics = model.ShowRubrics;
            }
            return View("Show", highlightmodel);
        }

        /// <summary>
        /// Saves the highlight.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveHighlight(DocumentToView model)
        {
            model.UserId = Context.IsAnonymous ? "" : Context.CurrentUser.Id;
            var highlight = model.ToHighlight(Context);
            var id = NoteActions.AddHighlight(highlight);

            return Content(id.ToString());
        }

        /// <summary>
        /// Saves the color of the highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveHighlightColor(string highlightId, string color)
        {
            NoteActions.UpdateHighlightColor(highlightId, color);
            return new EmptyResult();
        }

        /// <summary>
        /// Toggles the isShared flag on a selected highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="isShared">if set to <c>true</c> [is shared].</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ToggleShare(string highlightId, bool isShared)
        {
            NoteActions.ShareHighlight(highlightId, isShared);
            return new EmptyResult();
        }

        /// <summary>
        /// Toggles the isShared flag on a selected highlight.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="isShared">if set to <c>true</c> [is shared].</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ToggleShareNote(string noteId, bool isShared)
        {
            NoteActions.ShareNote(noteId, isShared);
            return new EmptyResult();
        }

        /// <summary>
        /// Removed highlight and all associated comments from database.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string highlightId)
        {
            NoteActions.UpdateHighlightStatus(highlightId, HighlightStatus.Hide, Context.EntityId);
            return new EmptyResult();
        }

        /// <summary>
        /// Deletes my highlights.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="secondaryId">The secondary id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteMyHighlights(string itemId, string secondaryId, string reviewId, PxHighlightType highlightType)
        {
            NoteActions.UpdateHighlightStatus(null, Context.CurrentUser.Id, itemId, reviewId, Context.EntityId, secondaryId, true, HighlightStatus.Hide);
            return new EmptyResult();
        }

        /// <summary>
        /// Deletes my notes.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="secondaryId">The secondary id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteMyNotes(string itemId, string secondaryId, string reviewId, PxHighlightType highlightType)
        {
            NoteActions.UpdateNoteStatus(null, Context.CurrentUser.Id, itemId, reviewId, Context.EntityId, secondaryId, true, HighlightStatus.Deleted);
            return new EmptyResult();
        }

        /// <summary>
        /// Deletes the notes.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteNotes(string highlightId)
        {
            NoteActions.UpdateHighlightNoteStatus(highlightId, HighlightStatus.Deleted, false, Context.EntityId);
            return new EmptyResult();
        }

        /// <summary>
        /// Action to handle new comment submissions from an existing highlight.
        /// Records comment and returns updated comment list for the given highlight id.
        /// </summary>
        /// <param name="CommentText">The comment text.</param>
        /// <param name="CommentLink">The comment link.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="secondaryId">The secondary id.</param>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="highlightDescription">The highlight description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult AddComment(String CommentText, String CommentLink, string itemId, string reviewId, string secondaryId, string highlightId, PxHighlightType highlightType, string highlightDescription, bool isPublic, HighlightStatus status)
        {
            ViewData["HLM_HighlightType"] = highlightType;
            ViewData["HLM_Public"] = isPublic;
            ViewData["HLM_Locked"] = status == HighlightStatus.Locked;
            ViewData["CurrentUserId"] = Context.CurrentUser.Id;
            ViewData["HLM_IsInstructor"] = Context.AccessLevel == BizSC.AccessLevel.Instructor; 
            
            if (CommentText == "Enter comment here") CommentText = "";
            if (CommentLink == "http://") CommentLink = "";
            var commentLink = String.IsNullOrEmpty(CommentLink) ? "" : String.Format(" <a href='{0}' target='_blank'>{0}</a>", CommentLink);
            var model = new List<Bfw.PX.PXPub.Models.Note>();

            var hid = new Guid(highlightId);
            if (!string.IsNullOrEmpty(highlightId) && hid != Guid.Empty)
            {
                var note = NoteActions.AddNoteToHighlight(highlightId, itemId, reviewId, CommentText + commentLink,
                                                          highlightDescription, highlightType, isPublic);
                return View("CommentList", new List<Models.Note> { note.ToNote(Context) });
            }
            else
            {
                var note = new Bfw.PX.Biz.DataContracts.Note
                {
                    ItemId = itemId,
                    ReviewId = reviewId,
                    CourseId = Context.CourseId,
                    HighlightType = highlightType,
                    Description = highlightDescription,
                    IsGeneral = true,
                    UserId = Context.CurrentUser.Id,
                    EnrollmentId = secondaryId,
                    FirstName = Context.CurrentUser.FirstName,
                    LastName = Context.CurrentUser.LastName,
                    Public = isPublic,
                    Text = CommentText + CommentLink
                };
                NoteActions.AddNote(note);

                var noteList = NoteActions.GetItemGeneralNotes(new NoteSearch
                {
                    ItemId = itemId,
                    ReviewId = reviewId,
                    CourseId = Context.CourseId,
                    HighlightType = highlightType,
                    EnrollmentId = Context.EnrollmentId
                });

                model = noteList.Map(n =>
                {
                    if ((!Context.IsAnonymous) && (note.UserId == Context.CurrentUser.Id))
                        n.FirstName = "";

                    return n.ToNote(Context);
                }).ToList();
            }

            return View("CommentList", model);
        }

        /// <summary>
        /// Action to handle new comment submissions from an existing highlight.
        /// Records comment and returns updated comment list for the given highlight id.
        /// </summary>
        /// <param name="CommentText">The comment text.</param>
        /// <param name="CommentLink">The comment link.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="topNoteId">The highlight id.</param>
        /// <param name="highlightDescription">The highlight description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <param name="status">Note Status</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult AddCommentToTopNote(String CommentText, String CommentLink, string itemId, string topNoteId, string highlightDescription, bool isPublic, HighlightStatus status)
        {
            if (topNoteId.IsNullOrEmpty())
                return Json(false);

            ViewData["HLM_Public"] = isPublic;
            ViewData["HLM_Locked"] = status == HighlightStatus.Locked;
            ViewData["CurrentUserId"] = Context.CurrentUser.Id;
            ViewData["HLM_IsInstructor"] = Context.AccessLevel == BizSC.AccessLevel.Instructor;

            if (CommentText == "Enter comment here") CommentText = "";
            if (CommentLink == "http://") CommentLink = "";
            var commentLink = String.IsNullOrEmpty(CommentLink) ? "" : String.Format(" <a href='{0}' target='_blank'>{0}</a>", CommentLink);

            if (!string.IsNullOrEmpty(topNoteId))
            {
                var note = NoteActions.AddNoteToTopNote(topNoteId, itemId, CommentText + commentLink,
                    highlightDescription, isPublic);
                note.UserId = Context.CurrentUser.Id;
                return View("CommentList", new List<Models.Note> {note.ToNote(Context)});
            }

            return View("CommentList", new List<Models.Note> { new Models.Note() });
            
        }


        /// <summary>
        /// Updates the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="noteText">The note text.</param>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult UpdateNote(string noteId, string noteText, string highlightId, PxHighlightType highlightType)
        {
            NoteActions.UpdateNote(noteId, noteText);
            return new EmptyResult();
        }

        /// <summary>
        /// Comments the library drop list.
        /// </summary>
        /// <returns></returns>
        public ActionResult CommentLibraryDropList()
        {
            var commentListResult = NoteLibraryActions.ListNotes(Context.EnrollmentId);
            IEnumerable<Models.Note> notes = null;

            if (!commentListResult.IsNullOrEmpty())
            {
                notes = commentListResult.OrderBy(n => n.Text).Map(n => n.ToNote(Context));
            }

            ViewData.Model = notes;

            return View();
        }

        /// <summary>
        /// Quicks the link list.
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickLinkList()
        {
            var linkListResult = ContentActions.ListChildren(Context.EntityId, ITEM_ID, 1, "AGX_PARENT_SEARCH");
            IEnumerable<QuickLink> quickLinks = !(linkListResult.IsNullOrEmpty()) ? new List<QuickLink>() : null;

            if (!linkListResult.IsNullOrEmpty())
            {
                quickLinks = linkListResult.Map(b => b.ToQuickLink(ContentActions));
            }

            ViewData.Model = quickLinks;
            return View();
        }

        /// <summary>
        /// Get the quick links for the ebook
        /// </summary>
        /// <returns></returns>
        public JsonResult QuickLinksList()
        {
            var linkListResult = ContentActions.ListChildren(Context.EntityId, ITEM_ID, 1, "AGX_PARENT_SEARCH");
            IEnumerable<QuickLnk> quickLnks = new List<QuickLnk>();

            if (!linkListResult.IsNullOrEmpty())
            {
                quickLnks = linkListResult.Map(b => b.ToQuickLnk(ContentActions));
            }

            return Json(quickLnks);
        }

        /// <summary>
        /// Toggles the lock.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="lockNotes">if set to <c>true</c> [lock notes].</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ToggleLock(string highlightId, bool lockNotes)
        {
            var status = lockNotes ? HighlightStatus.Locked : HighlightStatus.Active;
            NoteActions.UpdateHighlightNoteStatus(highlightId, status, true, Context.EntityId);
            return new EmptyResult();
        }

        /// <summary>
        /// Toggles the lock of top note.
        /// </summary>
        /// <param name="noteId">The top note id.</param>
        /// <param name="lockNotes">if set to <c>true</c> [lock notes].</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleLockTopNote(string noteId, bool lockNotes)
        {
            var status = lockNotes ? HighlightStatus.Locked : HighlightStatus.Active;
            NoteActions.UpdateNoteStatus(noteId, status, Context.EntityId);
            return new EmptyResult();
        }

        /// <summary>
        /// settings.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        public JsonResult NoteSettings(string itemId, string reviewId, string enrollmentId)
        {
            var biz = NoteActions.GetNoteSettings(Context.CurrentUser.Id, Context.EntityId, itemId, reviewId, enrollmentId);
            var model = biz.ToNoteSettings();

            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = model;

            return result;
        }

        /// <summary>
        /// Updates the note settings.
        /// </summary>
        /// <param name="sharerId">The sharer id.</param>
        /// <param name="highlights">The highlights.</param>
        /// <param name="notes">The notes.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateNoteSettings(string sharerId, bool? highlights, bool? notes)
        {
            NoteActions.UpdateNoteSettings(Context.CurrentUser.Id, Context.EntityId, sharerId, highlights, notes);
            return new EmptyResult();
        }

        /// <summary>
        /// Deletes the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteNote(string noteId)
        {
            NoteActions.UpdateNoteStatus(noteId, HighlightStatus.Deleted, Context.EntityId);
            return new EmptyResult();
        }

        public ActionResult GetMenuData()
        {
            var item = new List<dynamic>{new {name = "view-notes", text = "View notes"},
                       //new {name="add-note", text="Add Note", relation=ABOUT_KEY_ID.ToString()}, PX-1154
                       new {name = "add-top-note", text = "Add top note", className = "ignore-autosave"},
                       new {name = "clear-highlights", text = "Clear highlights"},
                       new {name = "delete-notes", text = "Delete notes"}};
            if (Context.Course.AllowCommentSharing)
            {
                item.Add(new { name = "note-settings", text = "Note Settings" });
            }

            return Json(new { id = "highlightmenuactions", options = item }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public static string GetMediaVaultUrl(string urlToChange)
        {
            return MediaVault.GetMediaValutUrlWithHash(urlToChange);
        }
        public string GetMediaVaultUrlAction(string urlToChange)
        {
            return MediaVault.GetMediaValutUrlWithHash(urlToChange);
        }
        #region Help Methods

        /// <summary>
        /// creates the view using the model and get the html for the view. This can be used in cases where you need to return 
        /// view as well as some other information from Action method to clinet side call. This method can be used to get view html and send as
        /// a property of json object.
        /// </summary>
        /// <param name="viewName">Name of View</param>
        /// <param name="model">Model for view</param>
        /// <returns></returns>
        private string GetViewHtml(string viewName, object model)
        {
            return GetViewHtml(viewName, model, this.ViewData);
        }

        /// <summary>
        /// creates the view using the model and get the html for the view. This can be used in cases where you need to return 
        /// view as well as some other information from Action method to clinet side call. This method can be used to get view html and send as
        /// a property of json object.
        /// </summary>
        /// <param name="viewName">Name of View</param>
        /// <param name="model">Model for view</param>
        /// <param name="viewData">View aata for view</param>
        /// <returns></returns>
        private string GetViewHtml(string viewName, object model, ViewDataDictionary viewData)
        {
            var html = string.Empty;
            viewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, viewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                html = sw.ToString();
            }

            return html;
        }
      
        /// <summary>
        /// Gets the note list.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        private List<Models.Note> GetNoteList(string highlightId, PxHighlightType highlightType)
        {
            var noteList = NoteActions.GetNotesByHighlightId(highlightId, highlightType);
            var model = new List<Models.Note>();

            foreach (var note in noteList)
            {
                if ((!Context.IsAnonymous) && (note.UserId == Context.CurrentUser.Id))
                {
                    note.FirstName = "";
                }

                model.Add(note.ToNote(Context));
            }

            return model;
        }


        #endregion


    }
}
