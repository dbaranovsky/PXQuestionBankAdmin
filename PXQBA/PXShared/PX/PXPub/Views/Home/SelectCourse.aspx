<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Home.Master" Inherits="System.Web.Mvc.ViewPage<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.Course>>" %>

<asp:Content ContentPlaceHolderID="CenterContent" runat="server">
    <% if (Model.Count() > 0)
       { %>
        <ul class="course-list">
            <% foreach (var course in Model)
               { %>
                <li><a href="../<%= course.Id %>"><%= course.Title%></a></li>
            <% } %>
        </ul>
    <% }
       else
       { %>
        You are not currently enrolled in any courses.
    <% } %>
</asp:Content>