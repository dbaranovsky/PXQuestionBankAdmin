<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="sandbox_revert_confirmation">
    <div class="course-revert-icon"></div>
<%
    if ((bool)ViewData["AllowRevert"] == true)
    {
%>
    <div class="course-revert-content">
        <h1>Revert Changes Confirmation</h1>
        <p>
        Reverting your changes will remove anything you have
        currently changed in your sandbox course. Your course
        will revert to its master course state.
        </p>
        <b>Are you sure you want to revert the changes?</b>
        
    </div>
    <div class="revert-btn-wrapper">
        <h1 class="revert-wait">Please wait while we are reverting the changes...</h1>
        <br />
        <a href="#" id="RevertActionLink" ref="<%= Url.Action("RevertCourse","Sandbox") %>"><button id="RevertAction">Revert Changes</button></a> <button id="CancelRevert">Don't Revert</button>
    </div>
<%
    }
    else
    {
%>
    <div class="course-revert-content">
        <h1>Revert Changes Confirmation</h1>
        <p>Sorry you don't have permission to Revert the Course.</p>    
    </div>
    <div class="revert-btn-wrapper">
        <button id="CancelRevert">Close</button>
    </div>
<%
    }
%>
</div>
