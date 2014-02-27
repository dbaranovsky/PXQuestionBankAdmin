<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AccountWidget>" %>

<% 
    var helpUrl = null != ViewData["HelpUrl"] ? ViewData["HelpUrl"].ToString() : string.Empty;
%>

<a href="<%= helpUrl %>" id="help-link" <%= "#".Equals(helpUrl) ? "" : "target='_blank'" %>></a>

<%var path = ((bool)ViewData["isContentTab"] == true) ? "PxPage.PageReload" : "window.location.reload();";
       %>
<% if (Model.IsAuthenticated)
   {
       if (Model.StudentViewStatus != AccountWidget.StudentViewStates.NotInstructor && Model.StudentViewStatus != AccountWidget.StudentViewStates.HideLink)
       {
           string text = "";
           AccountWidget.StudentViewStates linkState = AccountWidget.StudentViewStates.NotInstructor;
           if (Model.StudentViewStatus == AccountWidget.StudentViewStates.InstructorView)
           { %>
<div class="view-select">

       <%= Ajax.ActionLink("Student View", "SwitchView", new { To = AccountWidget.StudentViewStates.StudentView }, new AjaxOptions { HttpMethod = "POST", OnBegin = "$.blockUI();", OnSuccess = path })%>

</div>
<% }
           else if (Model.StudentViewStatus == AccountWidget.StudentViewStates.StudentView)
           { %>
<div class="banner student-view-banner" style="display: none;">
    <span class="message">You are currently viewing the site as a Student. Hit cancel at
        anytime to return to your faculty view.</span>
    <%= Ajax.ActionLink("Cancel", "SwitchView", new { To = AccountWidget.StudentViewStates.InstructorView }, new AjaxOptions { HttpMethod = "POST", OnBegin = "$.blockUI();", OnSuccess = "window.location.reload();" })%>
</div>
<script type="text/javascript">    jQuery(document).ready(PxPage.SetBanners);</script>
<% }
       } %>
<span id="student-login" class="gradient"><a href="<%= Url.Action("Logout", "Account") %>">Sign Out</a> </span>


<span id="student" class="gradient">
    <span class="default"></span>
    <span class="pxicon pxicon-down-open"></span>

<% 
    var selectList = (ViewData["accountActionsList"] as IEnumerable<SelectListItem>).ToList();

    string viewMyCourse = Url.RouteUrl("Dashboard", new { courseId = ViewData["ProductCourseId"] });
    bool addCreateCourseLink = false;
    bool.TryParse(ViewData["AddCreateCourseLink"].ToString(), out addCreateCourseLink);

    if (addCreateCourseLink)
    {      
        selectList.Add(new SelectListItem()
            {
                Selected = false,
                Text = "Switch/Create New Courses",
                Value = viewMyCourse
            });
    }
%>

    <% Html.RenderPartial("AccountActionsList", selectList); %>
    
</span>
<script type="text/javascript">
    (function ($) {
        $(function () {
            var defaultselect = $("#accountActionsList option[value='user']").text();
            $("#student .default").text(defaultselect);
        });
    } (jQuery))
</script>
<% }
   else
   { %>
<span id="student-login" class="gradient"><a href="<%= Model.LoginUrl %>">Login</a> </span>
<% } %>