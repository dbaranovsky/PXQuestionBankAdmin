<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/SingleColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.GroupSetOverview>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Manage Groups
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">
    
    
    <script language="javascript" type="text/javascript">
        jQuery(document).ready(function () {

            PxPage.equalizeWidth("#left", "#right", -15);
            var resizeCb = function () { PxPage.equalizeWidth("#left", "#right", -15); };
            var resizeTimer;
            window.onresize = function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(resizeCb, 100);
            };
            PxPage.SetActiveHeaderMenu('mygradebookandscorecard_menu');
        });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

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
</asp:Content>
