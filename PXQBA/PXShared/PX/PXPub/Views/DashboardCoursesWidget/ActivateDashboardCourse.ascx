<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<style type="text/css">
    .block-space {margin: 0 0 0 0; border: none; padding: 0;}
</style>
<div id="ActivateDashBoardCourse" class="activate-dashboard-course">
    <div id="ActivateDashBoardCourseHeader" class="activateheader">
        <h1 class="pre-activation">
            Activate this course?</h1>
        <h1 class="post-activation disable">
            Course Activated!</h1>
    </div>
    <input id="startdate" type="hidden" value="<%= DateTime.Now.ToString("MM/dd/yyyy") %>" />
    <%Html.RenderPartial("~/Views/DashboardCoursesWidget/CourseInfo.ascx", Model); %>
    <div>
        <div class="editactivatecourse-container">
            <a class="editactivatecourse" href="#">Edit this information</a></div>
        <div id="ActivatePanel" class="activate-panel">
            <p class="pre-activation activation-information">
                Once activated, students will be able to join your course</p>
            <div class="post-activation activation-information creation-info-text active">
                Your course has been activated and is ready for you and your students. Your unique course URL
                is below, along with language you can use to invite students to join (and cut down
                on the chances of students using you for technical support).
            </div><br />
            <p class="post-activation activation-information">
                Students can now join your course at this URL:</p>
            <div class="student-url-display post-activation disable activated-course">
            </div>
            <div class="post-activation activation-information">
                <%Html.RenderPartial("~/Views/Course/StudentInstructions.ascx", Model, ViewData); %>
            </div>
            <%--<p class="optional-text post-activation disable">
                We recommend bookmarking this link.</p>--%>
        </div>
    </div>
</div>
