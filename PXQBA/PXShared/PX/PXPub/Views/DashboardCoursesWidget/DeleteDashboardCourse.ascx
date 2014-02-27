<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="DeleteDashBoardCourse" class="delete-dashboard-course">
    <div id="DeleteDashBoardCourseHeader" class="deleteheader">
        <h1>
            Delete this course?</h1>
    </div>
    <%Html.RenderPartial("CourseInfo", Model); %>
    <div id="DeleteConsequence" class="delete-consequence">
        <ul class="delete-consequence-list">
            <li class="delete-consequence-item">All enrolled students will be un-enrolled.</li>
            <li class="delete-consequence-item">Students can access an un-customized course for the remainder of their subscription period.</li>
            <li class="delete-consequence-item">Contact <a href="http://www.macmillanhighered.com/techsupport">Tech Support</a> to restore this course.</li>
        </ul>
    </div>
</div>