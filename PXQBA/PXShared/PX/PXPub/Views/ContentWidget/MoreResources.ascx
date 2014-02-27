<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TaxonomyRelationship>>" %>

<% if (Model.IsNullOrEmpty())
   { %>
    <div>There are no associated resources available.</div>
<% }
   else
   { %>
    <ul class="more-resources">
        <% foreach (var resource in Model)
           { %>
            <li><%= Html.ActionLink(resource.ItemTitle, "DisplayItem", "ContentWidget", new { id = resource.ItemId, mode = ContentViewMode.Preview, includeToc = false }, new { @class="fne-link", @resourceId=resource.ItemId, @title=resource.ItemTitle }) %></li>
        <% } %>
    </ul>
<% } %>
