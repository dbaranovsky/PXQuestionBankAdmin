<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.HtmlDocument>" %>

<%
    var lessonId = ExtensionMethods.GetMultipartPartLessonId(Request, ViewData);
%>
<div class="html-content">
<h2 class="content-title">
    <%= HttpUtility.HtmlDecode(Model.Title) %>
    <% if (!Model.ReadOnly && Model.UserAccess != Bfw.PX.Biz.ServiceContracts.AccessLevel.Student && !Model.IsItemLocked) 
       { %>
        <div class="menu edit-link on-top">
            <%
                var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                        {
                                            id = Model.Id,
                                            mode = ContentViewMode.Edit,
                                            hasParentLesson = lessonId,
                                            includeNavigation = false,
                                            isBeingEdited = true
                                        });    
                %>
                <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
        </div>
    <% } %>
</h2>
<div style="clear:both"></div>
<div class="html-container">
<%
       var doc = new DocumentToView()
                     {
                         ItemId = Model.Id,
                         IsCurrentUserContext = false,
                         HighlightDescription = Model.Title,
                         HighlightType = 1,
                         Url = Url.Action("HtmlDocument", "Download", new { id = Model.Id, applicableEnrollmentId = Model.ApplicableEnrollmentId }),
                         AllowComments = Model.AllowComments,
                         NoteId = Model.NoteId,
                         AllowRelativeUrl = false
                     };
       
       Html.RenderPartial("DocumentViewer", doc, ViewData); %>
</div>
</div>