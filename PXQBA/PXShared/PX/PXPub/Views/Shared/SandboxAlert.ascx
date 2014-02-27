<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="sandbox_alert_widget">
<% 
    if ((bool)ViewData["isCourseUpdated"] == false)
    {
 %>
    <div id="sandbox_alert_nochange">
        You are in a <b>sandbox course.</b> No changes have been made yet. <a href="#" id="sandbox_help_link">Click to learn more?</a>
        <a id="reload-alertbox" ref="<%= Url.Action("Index", "Sandbox")%>" href="#" style="float:right;">Check for Updates</a>
    </div>
    <div id="sandbox_helpbox" style="display:none;">
        <div class="course-publish-content">
            <h1>Sandbox Course</h1>
            <p>The <b>Sandbox course</b> allow you to make changes to your course template and then publish your changes to multiple derivative courses.
                After you make changes to anything in your course, you will be allowed to publish these changes or revert to the original course structure.</p>            
             <p><b>Publish:</b> When you publish your changes, all derivative courses
                using this sandbox course curriculum will be updated to
                    reflect the changes here.</p>
             <p><b>Revert:</b> Reverting your changes will remove anything you have
                currently changed in your sandbox course. Your course
                will revert to its master course state.</p>
            <br />
        </div>
        <div class="publish-btn-wrapper">
            <button id="CloseHelp">Close</button>
        </div>
    </div>
    <script type="text/javascript">
            (function ($) {
                PxPage.OnReady(function () {
                    PxPage.Require(['<%= Url.ContentCache("~/Scripts/Sandbox/SandboxAlert.js") %>'], function () {
                        PxSandboxAlert.InitHelp();
                    });
                });
            } (jQuery));    
    </script>
<%
    }
    else
    {
 %>
    <div id="sandbox_alert_change">
        <span>You are in a sandbox course, and have made changes. What would you like to do? Note that you may make more changes before you publish.</span>
        <div class="btn-wrapper"><button id="sandbox_publish_button">Publish Changes</button> <button id="sandbox_revert_button">Revert Changes</button></div>
    </div>
    <div id="sandbox_notice_modal" style="display:none;">
        <% Html.RenderAction("PublishModal");  %>
    </div>
    <div id="sandbox_revert_modal" style="display:none;">
        <% Html.RenderAction("RevertModal");  %>
    </div>
    <script type="text/javascript">
        (function ($) {
            PxPage.OnReady(function () {
                PxPage.Require(['<%= Url.ContentCache("~/Scripts/Sandbox/SandboxAlert.js") %>'], function () {
                    PxSandboxAlert.Init();
                });
            });
        } (jQuery));    
    </script>
 <%
    }
  %>
</div>
