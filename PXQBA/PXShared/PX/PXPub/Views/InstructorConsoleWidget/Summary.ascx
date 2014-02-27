<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% 
    bool requireRefresh = (ViewData["requireRefresh"] == null) ? false : Convert.ToBoolean(ViewData["requireRefresh"]);
    var viewType = (ViewData["ViewType"] == null) ? "" : ViewData["ViewType"].ToString().ToLower();
    var showWelcomeReturn = (ViewData["ShowWelcomeReturn"] == null) ? true : Convert.ToBoolean(ViewData["ShowWelcomeReturn"]);      
    var showBatchUpdater = (ViewData["ShowBatchUpdater"] == null) ? true : Convert.ToBoolean(ViewData["ShowBatchUpdater"]);
    var showManageAnnouncemets = (ViewData["ShowManageAnnouncemets"] == null) ? true : Convert.ToBoolean(ViewData["ShowManageAnnouncemets"]);
    bool isCourseSandbox = (ViewData["IsSandboxCourse"] == null) ? false : Convert.ToBoolean(ViewData["IsSandboxCourse"]);
    var isCourseSandboxClass = "";
    if (isCourseSandbox)
    {
        isCourseSandboxClass = "sandbox-inactive";
    }
%>
<div id="PX_InstructorConsoleWidget">
    <div class="CW_buttonbar">
        <span id="CW_ResourcesButton" class="gradebookbutton"><span class="pxicon pxicon-drawer"></span>Resources</span>
        <ul class="CW_ActionLinks">
        <% Html.RenderAction("BrowseResourcesItems", "InstructorConsoleWidget"); %>
        </ul>
        <a href="#state/instructorconsole/gradebook" class="consolebutton"><span id="CW_GradeBookButton" class="gradebookbutton <%=isCourseSandboxClass %>"><span class="pxicon pxicon-gradebook"></span>Gradebook</span></a>

        <% if (viewType == "instructor") { %>
        <a href="#state/instructorconsole/rosterandgroups" class="consolebutton"><span id="CW_Roster-groupsButton" class="gradebookbutton <%=isCourseSandboxClass %>"><span class="pxicon pxicon-public"></span>Roster & Groups</span></a>
<%--        <span id="CW_ReportsButton" class="gradebookbutton <%=isCourseSandboxClass %>"><span class="icon"></span>Reports (coming soon...)</span>--%>
        <a href="#state/instructorconsole/general" class="consolebutton"><span id="CW_SettingsButton" class="gradebookbutton"><span class="pxicon pxicon-gear"></span>Settings</span></a>
        <ul class="CW_ActionLinks ">
        <% Html.RenderAction("SettingsItems", "InstructorConsoleWidget"); %>
        </ul>
        <% } %>  

        <% if (showWelcomeReturn) 
        {                
        %>       
        <span id="CW_ReturnButton" class="gradebookbutton"><span class="pxicon pxicon-reply"></span>Return to Welcome screen</span>
        <%
        } 
        %>
    </div>
    <ul class="CW_ActionLinks">
        <% if (viewType == "instructor") { 
                if (showBatchUpdater) 
                {                
        %>       
            <li id="CW_UpdaterLink"><a href="#state/instructorconsole/batchupdater" class="link">Batch Due Date Updater</a></li>
        <%
                }
                if (showManageAnnouncemets) 
                {                
        %>       
            <li id="CW_AnnouncementsLink"><a href="#" class="link">Create and manage announcements</a></li>
        <%
                }
            } 
        %>  
    </ul>

    <% if (viewType == "instructor") { %>
    <br/>
    <ul class="CW_ActionLinks">
        <li><a href="#state/instructorconsole"><span id="CW_InstructorConsole" class="link <%=isCourseSandboxClass %>" style="cursor: pointer">View instructor console...</span></a></li>        
    </ul>
    <% } %>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
                <% if (requireRefresh) {%>
                PxInstructorConsoleWidget.ReloadLaunchPad();
                <% } %>
            });
        });
    } (jQuery));    
</script>