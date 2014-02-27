<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% 
    bool requireRefresh = (ViewData["requireRefresh"] != null) && Convert.ToBoolean(ViewData["requireRefresh"]);
%>

<div class="instructor-console-wrapper" id="instructor-console-wrapper">
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            $.blockUI();

            $.ajax({
                type: 'GET',
                url: '<%= Url.Action(ViewData["Action"].ToString(), "InstructorConsoleWidget", new { view = ViewData["View"].ToString() }) %>',
                success: function (result) {
                    $('#instructor-console-wrapper').html(result);
                    $.unblockUI();
                }
            });

            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
                <% if (requireRefresh) {%>
                PxInstructorConsoleWidget.ReloadLaunchPad();
            <% } %>
            });
        });
    } (jQuery));    
</script>