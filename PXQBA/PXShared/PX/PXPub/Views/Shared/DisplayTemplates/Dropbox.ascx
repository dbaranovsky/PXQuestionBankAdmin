<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Dropbox>" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>

<script type="text/javascript">
    var deps = ['<%= Url.ContentCache("~/Scripts/Assignment/Assignment.js") %>'];
    PxPage.Require(deps, function () {
        jQuery(document).ready(function () {
            PxAssignment.Init();
            $("#showUploadandSubmitModal").remove();
        });
    });
</script>
<div id="assignmentViewContent" class="assignment-detail-viewer">
    <h2 class="content-title">
        <%= HttpUtility.HtmlDecode(Model.Title) %>
        <%= Html.Hidden("onCompleteScript", "PxAssignment.OnDropboxUploadAndSubmitComplete", new { id = "onCompleteScript" })%>
        <% if (!Model.ReadOnly && Model.UserAccess != AccessLevel.Student)
           {
               var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                        {
                                            id = Model.Id,
                                            mode = ContentViewMode.Edit,
                                            hasParentLesson = false,
                                            includeNavigation = false,
                                            isBeingEdited = true
                                        }); %>
        <div class="menu edit-link">
            <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a></div>
        <% } %>
    </h2>
    <% if (!string.IsNullOrEmpty(Model.Description))
       { %>
    <div class="html-container description-content">
        <%= Model.Description %></div>
    <% } %>
    <div>
        <br />
        <% if (Model.UserAccess == AccessLevel.Student)
           {%>
        <%  var gridPrefix = Model.ReadOnly ? "fne_" : "";
        %>
        <input type="hidden" id="gridSavedSubmissionAjaxUrl" value="<%=Url.Action("GetStudentDropboxDocuments","Assignment",new {id=Model.Id,isAllowSubmission = Model.IsAllowSubmission}) %>" />
        <% if ((Model.AssignmentStatus == AssignmentStatus.New || Model.AssignmentStatus == AssignmentStatus.Unsubmitted) && Model.DueDate.Year != DateTime.MinValue.Year)
           {
               var linkParams = new { @class = "fne-link px-default-text linkButton", id = "lnkCompose" };%>
        <div id="divComposeItems" itemid="<%=Model.Id %>">
            <div align="left" class="viewer-left-panel">
                <% if (Model.ReadOnly)
                   {
                       var className = "uploadandsubmit-link";

                       if (!Model.IsAllowSubmission)
                       {
                           className = "uploadandsubmit-link-disabled";
                       }
                       
                %>
                <%if (Model.IsAllowSubmission)
                  { %>
                <span><a href="#" id="lnkUploadAssignment" class="primary button large  <%= className %>">
                    Submit file</a></span>
                <%} %>
                <% } %>
            </div>
        </div>
        <% } %>
        <%else
           { %>
        <br />
        <br />
        <% } %>
        <% } %>
        <%if (Model.UserAccess == AccessLevel.Instructor)
          {
           %>
        <br />
        <br />
        <h3>
            Submissions</h3>
        <%if (Model.TotalSubmissions > 0)
          {%>
        <%
              var undgraded = Model.TotalSubmissions - Model.TotalGrades;

              string submissions = string.Format("{0} ungraded submission", undgraded.ToString());
              if (undgraded > 1)
              {
                  submissions += "s";
              }
                
        %>
        <div style="padding-top: 10px; font-size: 13px;">
            <%= Url.GetComponentLink(submissions, "item", Model.Id, new { mode = ContentViewMode.Results,includeNavigation = true, isBeingEdited = true, renderFne = true })%>
<%--            // do we need to call these functions?: new AjaxOptions() { UpdateTargetId = "content-item", OnSuccess = "PxPage.ResultLoadComplete", OnBegin = "ContentWidget.OnBeginLoad" }, --%>
        </div>
        <% }%>
        <%else if (Model.TotalGrades == 0)
          {%>
        <div style="padding-top: 10px; font-size: 13px;">
            No submissions received.</div>
        <% }%>
        <br />
        <%   if (Model.TotalGrades > 0)
             {
                 string graded = string.Format("{0} graded submission", Model.TotalGrades.ToString());

                 if (Model.TotalGrades > 1)
                 {
                     graded += "s";
                 } %>
        <div>
            <span class="px-default-text">
                <%= graded %>.</span>
        </div>
        <% } %>
        <% }%>
        <%else if (Model.UserAccess == AccessLevel.Student)
          {%>
        <div id="divComposeItems" itemid="<%=Model.Id %>">
            <% Html.RenderAction("GetStudentDropboxDocuments", "Assignment", new { id = Model.Id, isAllowSubmission = Model.IsAllowSubmission }); %>
        </div>
        <% }%>
    </div>
    <div class="showUploadandSubmitModal">
        <div class="placeHolderShowUploadandSubmitModal">
        </div>
    </div>
</div>
