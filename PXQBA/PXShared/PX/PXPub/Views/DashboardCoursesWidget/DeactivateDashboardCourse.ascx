<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="DeactivateDashBoardCourse" class="deactivate-dashboard-course">
    <div id="DeactivateDashBoardCourseHeader" class="deactivateheader">
        <h1>
            Deactivate this course?</h1>
    </div>
    <%Html.RenderPartial("CourseInfo", Model); %>
    <div id="DeactivateConsequence" class="deactivate-consequence">
        <ul class="deactivate-consequence-list">
            <li class="deactivate-consequence-item">Students will no longer be able to join.</li>
            <%--<li class="deactivate-consequence-item">{{ consequence 2}}</li>
            <li class="deactivate-consequence-item">{{ consequence 3}}</li>--%>
        </ul>
    </div>
</div>