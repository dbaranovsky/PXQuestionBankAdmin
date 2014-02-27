<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TocItem>" %>

<% if (Model != null) {%>
   
   <%if(Model.IsHiddenFromStudents) {%>
   <span style="" class="tooltip">
   Hidden from students
   </span>
   <% } %>   
   
   <%if(Model.IsAssigned) {%>
   <span style="" class="tooltip">
       <%
         if (!string.IsNullOrEmpty(Model.ParentLesson))
         {
       %>
       <%=Model.ParentLesson%>
       <br />
       <%
       }
       %>

   <%=Model.Title%><br />
   Due <%=Model.DueDate.ToShortDateString() %>
   <br /><%=Model.MaxPoints %> possible points
   <br /><%--<span class="GetSubmissionStatus"></span>--%>
   <% Html.RenderAction("GetSubmissionStatus", "Assignment", new { assignmentId = Model.Id }); %>

   </span>
   <% } else { %>   
     [NO CONTENT]
   <%} %>
    
<% } %>
