<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ThreeColumn.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.CourseWidget>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ViewAll
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
    <h2>ViewAll</h2>
      <!--<%= Html.RouteLink("Create Course", "CourseList", new { id = -1, mode = ContentViewMode.Create, includeToc = true, includeDiscussion = true }, new { @class = "fne-link" })%>-->
        <%= Html.ActionLink("Create Course", "Create", "Course") %> |
    <table>
        <tr>
            <th></th>
            <th>
                Id
            </th>
            <th>
                Title
            </th>
            <th>Activated Date</th>
            <th>
                Product Name
            </th>
            <th>
                Course User Names
            </th>
        </tr>

    <% foreach (var item in Model.Courses) { %>
    
        <tr>
            <td>              
                <%= Html.RouteLink("View", "CourseSectionHome", new { courseid = item.Id })%>   
                <%=Html.ActionLink("Delete", "Delete", new { controller = "CourseWidget", id = item.Id })%>
                
            </td>
            <td>
                <%= Html.Encode(item.Id) %>
            </td>
            <td>
                <%= Html.Encode(item.Title) %>
            </td>
            <td>
                <%= Html.Encode(item.ActivatedDate) %>
            </td>
            <td>
                <%= Html.Encode(item.CourseProductName) %>
            </td>
            <td>
                <%= Html.Encode(item.CourseUserName) %>
            </td>
        </tr>
    
    <% } %>

    </table>
  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
