<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="read-content-xbook-image">

</div>
<div id="read-content-list">
<% var read = ViewData["read"] as IEnumerable<ContentItem>;
   var desc = ViewData["contentDescription"] as string;
   var contentId = ViewData["contentId"] as string;

   if (read.IsNullOrEmpty())
   {
       if (!String.IsNullOrEmpty(contentId))
       {           
       %>     
       
    <p><%=desc %></p><br />
    <%= Html.ActionLink("Go to E-Book", "Index", "Content", new { id = contentId }, new { @class = "linkButton" })%>
    <% }
       else
       { %>
    <p>No items have been read yet</p>
<% }
   } 
   else 
   { %>
    <p><%=desc %></p><br />
       <%= Html.ActionLink("Go to E-Book", "Index", "Content", new { id = contentId }, new { @class = "linkButton" })%>
    <br /><br />
     <ul class="read">
     <% foreach (var item in read)
        {
            //PLATX -- 3767
            if (item.Type.ToLowerInvariant() != "folder" &&
                          item.Type.ToLowerInvariant() != "assignment" &&
                          item.Type.ToLowerInvariant() != "pxunit" &&
                          item.Type.ToLowerInvariant() != "quiz" &&
                          item.Type.ToLowerInvariant() != "widgetconfiguration" &&
                          item.Type.ToLowerInvariant() != "discussion")
            {  %>
                <li><%= Html.ActionLink(item.Title, "Index", "Content", new { id = item.Id }, null)%> <span class="readdate"><%=item.ReadDate%></span></li>
     <%     }
        } %>
     </ul>
<% } %>
</div>
<span style="display:block; clear: both;"></span>