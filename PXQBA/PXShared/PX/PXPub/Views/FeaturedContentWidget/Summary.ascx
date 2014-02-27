<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.FeaturedContentWidget>" %>

<% if (!Model.FeaturedContentItems.IsNullOrEmpty()) 
   { %>
        <ul>
            <% foreach (var fci in Model.FeaturedContentItems)
               {
                   var linkUrl = Url.Action("DisplayItem", "ContentWidget", new { id = fci.Id, mode = ContentViewMode.Preview });
                   %>
                    <li>
                        <a class="fne-link" href="<%= linkUrl %>">
                            <span>
                                <%= fci.Title %>
                                <img src="<%= fci.ImageUrl.ToString() %>" alt="<%= fci.Title %>" />
                            </span>
                        </a>
                    </li>
            <% } %>
        </ul>
        <span class="clear"></span>
<% }
   else
   { %>
        <span class="noItemsMessage">No Featured Content</span>
<% } %>