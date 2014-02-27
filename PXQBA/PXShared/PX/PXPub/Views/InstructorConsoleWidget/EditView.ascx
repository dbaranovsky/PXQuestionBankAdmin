<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.InstructorConsoleSettings>" %>
<% 
    var viewType = (ViewData["ViewType"] == null) ? "" : ViewData["ViewType"].ToString().ToLower();
    var isLoadStartOnInit = (ViewData["IsLoadStartOnInit"] == null) ? false : Convert.ToBoolean(ViewData["IsLoadStartOnInit"]);
    %>

<div class="instructor-console-wrapper" id="instructor-console-wrapper">
    <div class="instructor-console-header">
        <div class="instructor-console-title">
            Check the links that should appear in your console
        </div>
        <div class="instructor-console-link">
            <input id="settingsSave" type="button" value="Done" />
        </div>
    </div>


    <% using (Ajax.BeginForm("UpdateSettings", null, new AjaxOptions() { HttpMethod = "POST", OnBegin = "PxPage.Loading('fne-content');", OnSuccess = "PxPage.Loaded('fne-content'); PxInstructorConsoleWidget.Update();", UpdateTargetId = "instructor-console-wrapper" }, new { id = "settingsForm" }))
       { %>
    <ul>
        <li class="ConsoleMenu">
            <ul class="ConsoleMenuItem">
                <li class="first"><span class="icon resources"></span><input type="button" value="Resources" disabled="disabled" /></li>
                <div class="instructor-console-resources">
                    <%Html.RenderAction("ResourcesEdit", "InstructorConsoleWidget"); %>
                </div>
            </ul>
        </li>
        
        <li class="ConsoleMenu">
            <ul class="ConsoleMenuItem">
                <li class="first"><span class="icon roster-group"></span><input type="button" value="Roster and Groups" disabled="disabled" /></li>
                <li style="color: Gray">Export roster (coming soon)</li>
            </ul>
        </li>
        
        <li class="ConsoleMenu">
            <ul class="ConsoleMenuItem">
                <li class="first"><span class="icon settings"></span><input type="button" value="Settings" disabled="disabled" /></li>
                <li><%= Html.CheckBoxFor(m => m.ShowGeneral) %> General</li>
                <li><%= Html.CheckBoxFor(m => m.ShowNavigation)%> Navigation</li>
                <li><%= Html.CheckBoxFor(m => m.ShowLaunchPad)%> Launch Pad</li>
            </ul>
        </li>


        <li class="ConsoleMenu" style="visibility: hidden">
            <ul class="ConsoleMenuItem">
                <li class="first"><span class="icon gradebook"></span><input type="button" value="Gradebook" disabled="disabled" /></li>
                <li><%= Ajax.ActionLink("Gradebook Preferences", "GradebookPreferences", new { view = "GradebookPreferences" }, new AjaxOptions() { OnBegin = "PxPage.Loading('fne-content');", OnSuccess = "PxPage.Loaded('fne-content');", UpdateTargetId = "instructor-console-wrapper" }, new { @class = "link" })%></li>
            </ul>
        </li>

        <li class="ConsoleMenu Reports">
            <ul class="ConsoleMenuItem">
                <li class="first"><span class="icon reports"></span><input type="button" value="Reports (coming soon...)" disabled="disabled" style="color: Gray" /></li>
                <li style="color: Gray">Log-ins (coming soon)</li>
                <li style="color: Gray">Page views (coming soon)</li>
                <li style="color: Gray">Submissions (coming soon)</li>
            </ul>
        </li>

        <li class="ConsoleMenu">
            <ul class="ConsoleMenuItem">
                <% if (isLoadStartOnInit) { %>
                <li class="first"><span class="icon return"></span><input type="button" value="Return to Welcome screen" disabled="disabled" /></li>
                <li><%= Html.CheckBoxFor(m => m.ShowWelcomeReturn)%> Return to Welcome screen</li><br />
                <% } %>
                <!--li style="color: Black"><b>Miscellaneous:</b></li>
                <li style="color: Gray">Create and manage announcements</li>
                <!--li><%= Html.CheckBoxFor(m => m.ShowManageAnnouncemets)%> Create and manage announcements</li-->
                <li><%= Html.CheckBoxFor(m => m.ShowBatchUpdater) %> Batch Due Date Updater</li>
                <li style="color: Gray">Advanced content manager (coming soon)</li>
            </ul>
        </li>
    </ul>
    <% } %>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
            });
        });
    } (jQuery));    
</script>