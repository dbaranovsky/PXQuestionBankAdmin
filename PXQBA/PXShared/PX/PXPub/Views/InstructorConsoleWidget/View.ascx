<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%     bool requireRefresh = (ViewData["requireRefresh"] != null) && Convert.ToBoolean(ViewData["requireRefresh"]);
       var isLoadStartOnInit = (ViewData["IsLoadStartOnInit"] != null) && Convert.ToBoolean(ViewData["IsLoadStartOnInit"]);
       bool isCourseSandbox = (ViewData["IsSandboxCourse"] != null) && Convert.ToBoolean(ViewData["IsSandboxCourse"]);
       var isCourseSandboxClass = "";
       if (isCourseSandbox)
       {
           isCourseSandboxClass = "sandbox-inactive";
       } %>
<div class="instructor-console-header">
    <div class="instructor-console-title">
        Instructor Console
    </div>
    <div class="instructor-console-link">
        <a href="#state/instructorconsole/editview" class="link">Edit console links appearing on home page</a>
    </div>
</div>


<ul>
    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <li class="first"><span class="icon resources"></span><input type="button" value="Resources" id="CW_ResourcesButton" /></li>
            <li><div class="instructor-console-resources">
                <%Html.RenderAction("Resources", "InstructorConsoleWidget"); %>
            </div></li>
        </ul>
    </li>
        
    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <li class="first"><a href="#state/instructorconsole/rosterandgroups"><span class="icon roster-group"></span><input type="button" class="<%=isCourseSandboxClass %>" value="Roster and Groups" id="CW_Roster-groupsButton" /></a></li>
            <li style="color: Gray">Export roster (coming soon)</li>
        </ul>
    </li>
        
    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <li class="first"><a href="#state/instructorconsole/general"><span class="icon settings"></span><input type="button" class="<%=isCourseSandboxClass %>" value="Settings" id="CW_SettingsButton" /></a></li>            
            <li><a href="#state/instructorconsole/general" class="link">General</a></li>
            <li><a href="#state/instructorconsole/navigation" class="link">Navigation</a></li>
            <li><a href="#state/instructorconsole/launchpad" class="link">Launch Pad</a></li>
        </ul>
    </li>    

    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <li class="first"><a href="#state/instructorconsole/gradebook"><span class="icon gradebook"></span><input class="<%=isCourseSandboxClass %>" type="button" id="CW_GradeBookButton" value="Gradebook" /></a></li>
            <li><a href="#state/instructorconsole/gradebookpref" class="link">Gradebook Preferences</a></li>
            <li><a href="javascript: window.open(PxPage.Routes.InstructorConsole_GradebookExport, '_blank');" class="link">Export Total Score and Category Totals</a></li>
        </ul>
    </li>

<%--    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <li class="first"><div class="MenuReports"><span class="icon reports"></span><a class="linkButton returnWelcome"> Reports</a></div></li>
            <li style="color: Gray">Log-ins (coming soon)</li>
            <li style="color: Gray">Page views (coming soon)</li>
            <li style="color: Gray">Submissions (coming soon)</li>
        </ul>
    </li>--%>

    <li class="ConsoleMenu">
        <ul class="ConsoleMenuItem">
            <% if (isLoadStartOnInit) { %>
            <li class="first"><div class="MenuReturn"><span class="icon return"></span><%= Html.ActionLink("Return to Welcome screen", "IndexStart", "Home", null, new { @class = "linkButton returnWelcome" })%></div></li><br />
            <% } %>
            <li style="color: Black"><b>Miscellaneous:</b></li>
            <li style="color: Gray">Create and manage announcements</li>
            <!--li><a href="#" class="link" id="CW_AnnouncementsLink">Create and manage announcements</a></li-->
            <li><a href="#state/instructorconsole/batchupdater" class="link">Batch Due Date Updater</a></li>
            <li style="color: Gray">Advanced content manager (coming soon)</li>
        </ul>
    </li>
</ul>
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