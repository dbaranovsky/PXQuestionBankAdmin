<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseWidget>" %>

    <table>
        <tr>
            <th>
                Title
            </th>
            <th>
                CourseProductName
            </th>
        </tr>

    <% foreach (var item in Model.Courses) { %>
    
        <tr>
            <td>            
                 <% var link = Url.RouteUrl("CourseSectionHome", new { courseid = item.Id });%>
                 <a href="<%=link%>"><%= Html.Encode(item.Title)%></a>                 
            </td>
            <td>
                <%= Html.Encode(item.CourseProductName) %>
            </td>
        </tr>
    
    <% } %>

    </table>

