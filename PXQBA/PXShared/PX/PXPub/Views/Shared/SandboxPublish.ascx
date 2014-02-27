<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="sandbox_publish_notice">
<div class="course-publish-icon"></div>
<%
    if ((bool)ViewData["AllowPublish"] == true)
    {
%>
    <div class="course-publish-content">
        <h1>Course Publish Notice</h1>
        <p>When you publish your changes, all derivative courses
        using this sandbox course curriculum will be updated to
        reflect the changes here.</p>
        <b>Are you sure you want to publish the changes?</b>
        <br />
    </div>
    <div class="publish-btn-wrapper">
        <h1 class="publish-wait">Please wait while we are publishing the changes...</h1>
        <br />
        <a ref="<%= Url.Action("PublishCourse", "Sandbox")%>" href="#" id="PublishActionLink"><button id="PublishAction">Publish Changes</button></a> <button id="CancelPublish">Don't Publish</button>
    </div>
<%
    }
    else
    {
%>
    <div class="course-publish-content">
        <h1>Course Publish Notice</h1>
        <p>Sorry you don't have permission to Publish the Course.</p>
        <br />
    </div>
    <div class="publish-btn-wrapper">
       <button id="CancelPublish">Close</button>
    </div>
<%
    }
 %>
</div>
