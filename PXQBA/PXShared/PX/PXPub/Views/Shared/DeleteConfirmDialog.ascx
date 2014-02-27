<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% var removeMessage = ViewData["removeMesage"].ToString(); %>


    <div class="removeMessage">
        <%= removeMessage %>
    </div>
