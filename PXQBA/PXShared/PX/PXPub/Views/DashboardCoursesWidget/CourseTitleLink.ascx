<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>
<%
        string targetValue = "";

        if (ViewData["index"].ToString().ToLowerInvariant() == "true")
        {

            targetValue = "_blank";


        }%>

<% 
    string originalTitle = String.Empty;
    if (Model != null)
    {
        string cno = !(string.IsNullOrEmpty(Model.Course.CourseNumber)) ? string.Concat(Model.Course.CourseNumber.Trim(), " - ") : string.Empty;
        string sno = !(string.IsNullOrEmpty(Model.Course.SectionNumber)) ? string.Concat(Model.Course.SectionNumber.Trim(), " - ") : string.Empty;
        originalTitle = !(string.IsNullOrEmpty(Model.Course.Title)) ? Model.Course.Title.Trim() : "No Title";
        string title = string.Concat(cno, sno, originalTitle);
%>
 
<%= Html.RouteLink(title, "CourseSectionHome", new { courseid = Model.CourseId }, new { target=targetValue,  @class="course-title" })%>

 <%}else{%>
 <a href="#" target="<%=targetValue %>" class="course-title"></a>

 <%} %>

<input type="hidden" class="course-title-original" value="<%= originalTitle %>" />