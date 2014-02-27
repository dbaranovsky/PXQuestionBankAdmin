<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<%
    var isLoadStartOnInit = (ViewData["IsLoadStartOnInit"] == null) ? true : Convert.ToBoolean(ViewData["IsLoadStartOnInit"]);      
 %>

   <% using (Ajax.BeginForm("Update", null, new AjaxOptions() { OnBegin = "PxPage.Loading('fne-content');", OnSuccess = "PxPage.Loaded('fne-content');", UpdateTargetId = "instructor-console-wrapper" }, new { id = "courseInformationForm" }))
      { %>
        <%= Html.ValidationSummary(true) %>
        <%= Html.Hidden("View", ViewData["View"].ToString()) %>

            <label>What type of navigation would you like to use?</label>

            <div>
                <input type="checkbox" id="isLoadStartOnInit" name="isLoadStartOnInit" <%= isLoadStartOnInit ? "checked" : "" %> />
                <span class="navInput-title">Include a Welcome screen in my course</span>
            </div>

            
    <% } %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/CourseForm/CourseForm.js") %>'], function () {
                CourseForm.Init('0');
            });
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
            });
        });
    } (jQuery));    
</script>