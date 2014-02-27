<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>

<%= Html.DropDownList("accountActionsList", Model)%>

<%
    
    var hasMultipleDomains = (bool)ViewData["userHasMultipleDomains"];
    var linkClass = "fne-link fixed";

    if (!hasMultipleDomains)
    {
        linkClass = "fixed";
    }

    var courseId = ViewData["ProductCourseId"] != null ? ViewData["ProductCourseId"].ToString():string.Empty;
%>
<a id="marsProfileLink" href='<%= ConfigurationManager.AppSettings.Get("MarsProfile") %>' target="_blank" style="display:none"></a>
<span id="hasMultipleDomains" style="display:none;"><%= hasMultipleDomains %></span>
<%= Html.ActionLink("Create a Course", "ShowCreateCourse", "Course", new { CourseId = courseId }, new { @class = linkClass, ID = "createCourseLink", style = "display:none;" })%> 
<%= Html.ActionLink("View Courses", "CourseList", "ECommerce", null, new {ID = "courseListLink", style = "display:none;"})%> 