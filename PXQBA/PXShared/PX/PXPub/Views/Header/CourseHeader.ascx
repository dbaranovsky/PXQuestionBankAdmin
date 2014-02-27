<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseHeader>" %>

<% if (Model != null)
   { %>
    <h1><%= Model.CourseTitle%></h1>
    <span><%= Model.InstructorName%></span>
<% } %>