<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.TocCategory>>" %>

  <% if (!Model.IsNullOrEmpty()) { %> 
<div class="category-nav">
    <%
       foreach (var cat in Model)
       { %>
        <%= Html.RouteLink(cat.Text, "FeaturedContentItem", new { id = cat.ItemParentId, category = cat.Id }) %>
<%     } %>
    </div>

<%} %>