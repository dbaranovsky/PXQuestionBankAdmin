<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Student>>" %>


<div class="course-roster-title"><span class="course-roster-title-text"></span></div>
<table id="studentsRosterInformation">
    <% if (!Model.IsNullOrEmpty())
       {
           foreach (Student student in Model)
           {%>
                <tr class="student-info">
                    <td class="title-cell"> <%=student.FirstName%> <%=student.LastName%></td>
                    <td> <%=student.Email%></td>
                </tr> 
            <%}
       }
       else
       {
           %>
            <tr class="no-student">
                <td>This course doesn't have any student enrollments.</td>
            </tr>
           <%
       } %>
</table>