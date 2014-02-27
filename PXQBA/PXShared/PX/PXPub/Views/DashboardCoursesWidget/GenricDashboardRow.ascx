<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashboardItem>" %>


         
        <tr class="entityidofcourse" data-dw-id="<%= Model.CourseId %>" >
            <td class="title-cell">
               <a href="<%= Model.CourseUrl %><%= Model.CourseId %>"><%= Model.CourseTitle%></a>
               
                <a href="JavaScript:void(0);" class="show-url-hover" >Show URL</a><br />
                <span href="<%= Model.CourseUrl %><%= Model.CourseId %>" class="show-url course-url" ><%= Model.CourseUrl %><%= Model.CourseId%></span>
            </td>
              <td class="domain-cell">
              <%= Model.DomainName%>
              
            </td>
            <td class="status-cell">
               <%= Model.Status%>
            </td>
            
            <td class="enrollment-count-cell">
           
                <%= Model.Count%>
                <%if (Model.Count <= 0)
                  { %>
                <a href="JavaScript:void(0);" class="show-url-delete">Delete Course</a><br />
                <%} %>
            </td>
           
        </tr>