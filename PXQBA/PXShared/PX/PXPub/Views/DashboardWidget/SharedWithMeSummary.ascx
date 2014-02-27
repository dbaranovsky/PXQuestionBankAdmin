<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.List<Bfw.PX.PXPub.Models.DashboardItem>>" %>

<% if (Model.Count > 0)
   {
       var courseTitle = Model.First().CourseTitle;
       %>

   <div class="sharedwithmeWidget">
   <br /><br />
    <div class="grid-title-wrapper">
        <h2>e-Portfolios Shared with Me </h2>
        <span class="title-description"></span>
    </div>
    <br />

    <table class="dashboardgrid">
        <tr>
	        <th>Title</th><th>Owner</th><th>Status</th><th>Students</th>
        </tr>
        <%foreach (var item in Model) { 
              %>
        <tr id="<%= item.CourseId %>">
	        <td class="titleCell">
                <% var link = Url.RouteUrl("CourseSectionHome", new { courseid = item.CourseId });%>
                <a href="<%=link%>"><%=item.CourseTitle%></a>

                <span class="displayOnRowHover noteslink" style="display:none; float:right">
                    Notes
                </span>
                <div style="display:none" class="sharednotes">

                    <div class="shownotesdialog">
                         <div class="studentlist">
                            <select class="ddlStudents">
                            <% foreach (var student in item.Users) 
                               {%>
                                <option value="<%= student.Key %>"> <%= student.Value %> </option>
                            <% } %>
                            </select>
                        </div>
                        <div class="instructorinfo">
                            Instructor: <span class="ownername"><%=item.OwnerName%></span>
                            <span class="owneremail"><a href="mailto:<%=item.OwnerEmail %>">contact</a></span>
                        </div>
                        <div class="noteslist">
                            <% foreach (var note in item.Notes)
                               {%>
                               <div class="notesitem" userid="<%= note.Key %>" style="display:none"> <%= note.Value%> </div>
                            <% } %>
                        </div>
                    </div>
                </div>

            </td>
            <td class="statusCell" ownerid="<%= item.OwnerId %>"><%=item.OwnerName%></td>
            <td class="statusCell open">Open</td>
            <td class="statusCell students"><%= item.Count %></td>
        </tr>
        <%}%>
    </table>

    <div class="showNotesDialog" style="display: none;" title="Notes for Shared e-Portfolio: <%= courseTitle %>">
    </div>

   </div>



<%} %>