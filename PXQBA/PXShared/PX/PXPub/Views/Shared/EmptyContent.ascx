<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

 <div id="content-item" class="fne-local content">   
    <div id="content" class="content">
        <% if (ViewData["message"] != null)
           { %>
            <span class="message"><%= ViewData["message"]%></span>
        <% } %>        
    </div>
</div>