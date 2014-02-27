<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="header">
    <div id="student-info">
      <% Html.RenderAction("Summary", "AccountWidget"); %>
    </div>

     <%var courseType = (ViewData["CourseType"] != null) ? ViewData["CourseType"].ToString() : "" ; %>

       <div id="bannersearch">
            <%if (courseType == CourseType.FACEPLATE.ToString() || courseType == CourseType.XCLASS.ToString())
              {%>
            <% Html.RenderAction("Search", "Search"); %>            
             <%} %>
            </div>
   
</div>