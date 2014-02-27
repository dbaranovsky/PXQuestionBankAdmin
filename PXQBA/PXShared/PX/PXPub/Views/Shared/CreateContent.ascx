<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentItem>" %>


<div id="content">
    <% if (null != Model)
       { %>
            <%= Html.EditorForModel() %>
    <% }
       else
       { %>
            The type of content you are trying to create is not supported.   
    <% } %>
</div>
