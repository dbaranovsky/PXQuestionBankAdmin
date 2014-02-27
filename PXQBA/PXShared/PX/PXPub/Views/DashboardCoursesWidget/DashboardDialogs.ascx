<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>
<div id="display-container">
    <div id="create-branch-confirmation">
        <%  Html.RenderPartial("CreateBranchConfirmation", Model);%>
    </div>
    <div id="activate-dashboard-course">
        <%  Html.RenderPartial("ActivateDashboardCourse", Model.Course);%>
    </div>
    <div id="dashboard-course-creation-screen">
        <%  Html.RenderPartial("CreateCourse", Model);%>
    </div>
    <div id="create-course-option">
        <%  Html.RenderPartial("CreateCourseOption", Model.Course);%>
    </div>
    <div id="deactivate-dashboard-course">
        <%  Html.RenderPartial("DeactivateDashboardCourse", Model.Course);%>
    </div>
    <div id="delete-dashboard-course">
        <%  Html.RenderPartial("DeleteDashboardCourse", Model.Course);%>
    </div>
</div>
