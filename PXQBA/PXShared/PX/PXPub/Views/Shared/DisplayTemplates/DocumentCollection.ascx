<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentCollection>" %>
<% 
    var lessonId = ExtensionMethods.GetMultipartPartLessonId(Request, ViewData);
    var isSharedCourse = ViewData["IsSharedCourse"] != null ? Convert.ToBoolean(ViewData["IsSharedCourse"].ToString()) : false;
    var isInstructor = Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
%> 

<%  var isPublicView = (ViewData["isPublicView"] == null) ? false : Convert.ToBoolean(ViewData["isPublicView"]);        %>


<%

    if (!string.IsNullOrEmpty(Model.DocumentCollectionSubType) && Model.DocumentCollectionSubType.ToLower() == "uploadorcompose")
    { 
        %>
        <br />
        <h2 class="content-title">
            <%= HttpUtility.HtmlDecode(Model.Title) %>
            <% if (!isPublicView && !isSharedCourse && (!Model.ReadOnly || (Model.ApplicableEnrollmentId == Model.EnrollmentId)))

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

            <span style="display:none;" id="content-item-id" class="item-id"><%=Model.Id%></span>
            <input type="hidden" class="item-id" value="<%=Model.Id%>" />
        </h2>




        <%
        var userEnrollmentId = (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student) ? Model.EnrollmentId : Model.ApplicableEnrollmentId;
        var doc = new DocumentToView()
        {
            ItemId = Model.Id,
            SecondaryId = Model.ApplicableEnrollmentId,
            
            HighlightType = 2,//for assignment submission
            HighlightDescription = Model.Title,            
            IsCurrentUserContext = false,
            isAssignmentView = true,

            AllowComments = true,
            Url = Url.Action("ViewStudentFolderSubmission", "EportfolioBrowser", new { folderId = Model.Id, eid = userEnrollmentId, entityId = Model.EntityId, type = "DOCUMENT" }),            
            NoteId = Model.NoteId,
            IsInstructor = Model.IsInstructor
        };        
        Html.RenderPartial("DocumentViewer", doc, ViewData); 
    }
    else
    {
    var hasParentLesson = ViewData["hasParentLesson"];
%>
    <div class="doc-collection-content-view">
    <input type="hidden" class="item-id" value="<%=Model.Id%>" />
        <h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
        <% if (!Model.ReadOnly)
           { %>
            <div class="menu edit-link">
                <%
                    var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                        new
                                            {
                                                id = Model.Id,
                                                mode = ContentViewMode.Edit,
                                                hasParentLesson = hasParentLesson,
                                                includeNavigation = false,
                                                isBeingEdited = true
                                            });    
                    %>
                <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
            </div>
        <% } %>
    </h2>
        <div class="html-container description-content"><%= Model.Description %></div>
        <h3 class="sub-title">Documents</h3>

        <% 
        if (!Model.Documents.IsNullOrEmpty())
        {
            foreach (var doc in Model.Documents)
            {
                var fileType = System.IO.Path.GetExtension(doc.FileName).Replace(".", "");
                var fileImageTitle = fileType;
        %>
                <div style="float:left;border:1px solid #D5CCCC;width:100%;padding-top:10px;margin-top:0px;">
                    <div id="dropboxSubmission">
                        <div id="dropboxSubmissionImage">
                            <div class="<%= fileType %>" title="<%= fileImageTitle %>"></div>
                        </div>
                        <div id="dropboxSubmissionDesc">
                            <div class="docTitle">
                                <%= Html.RouteLink(doc.Title.IsNullOrEmpty() ? doc.FileName : doc.Title, "DownloadDocument", new { id = doc.Id, name = doc.FileName, docId = Model.Id }, new { target = "_blank" })%>
                            </div>
                            <br />
                            <div class="downloadLink">
                                <%= Html.RouteLink("Download", "DownloadDocument", new { id = doc.Id, name = doc.FileName, docId = Model.Id }, new { target = "_blank" }) %>
                                (<%= (doc.Size / 1024) %>Kb) 
                             </div>
                             <br />
                             <br />
                        </div>

                    </div>                
                </div>

        <% }
        }
        else
        {
        %>
        <div>
            <%=isInstructor?"You have not yet attached any documents. Click “Edit” and choose “Basic info” to attach a document." : "No documents have been attached." %>
        </div>
        
        <%
                
        }
        %>
    </div>

<%} %>