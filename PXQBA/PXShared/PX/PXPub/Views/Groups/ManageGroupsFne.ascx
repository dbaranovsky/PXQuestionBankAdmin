<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GroupSetOverview>" %>

<div class="manage-groups">
    <div id="left">
        <div id="groups-header">
            <div id="groupsheader-title">Groups</div>
        </div>
        <div id="groups-setnav"> 
            <div class="course-title"><%= Model.CourseTitle %>, Section <%= Model.Section %></div>
            <div class="course-subtitle">
                <%= Ajax.ActionLink("All Students", "StudentList", new { }, new AjaxOptions() { UpdateTargetId = "student-list" }, new { @class = "group-link all-students" })%>
                <div class="group-count">(<%= Model.StudentCount %> students)</div>
            </div>
            <div class="group-sets-title">Group Sets:</div>
            <div class="group-sets-list">
                <% Html.RenderAction("GroupList"); %>
            </div>
            <%= Html.ActionLink("Create New Group Set", "EditGroupSet", new { type = GroupSetEditor.EditType.Create }, new { @class = "fne-link create-group-set", title = "Create Group Set" })%>
        </div>              
    </div>
    <div id="right">
        <div id="student-list" class="student-list"><% Html.RenderAction("StudentList"); %></div>
    </div>
</div>
<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/Groups/PxGroups.js") %>'], function () {
                jQuery(document).ready(function () { PxGroups.Init(); });
            });
        });

    } (jQuery));    
</script>