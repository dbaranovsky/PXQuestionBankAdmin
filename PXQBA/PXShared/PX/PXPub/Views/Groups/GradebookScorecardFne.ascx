<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="GradeBookFne">
    <% if (((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
   { %>
        <%Html.RenderAction("ViewAll", "ProgressWidget"); %>
    <%} else { %>
      <%Html.RenderAction("Summary", "ProgressWidget"); %>
<%} %>
</div>