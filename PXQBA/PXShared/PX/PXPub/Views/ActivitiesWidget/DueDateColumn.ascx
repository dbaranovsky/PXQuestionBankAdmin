<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Activity>" %>

<% string assignRowClass = "";
   string assignedTextChange = "";
   if (Model.isAssigned)
   {
       assignRowClass = "assigned";
       assignedTextChange = "assignedTextChange";
   }
   %>


<td class="colIndex2 <%= assignRowClass %>">
    <% if (Model.DateText == "Completed")
        { %>
    <span class="completed-task"></span>
    <% }
        else
        { %>
    <span class="<%=Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor? "assignLink" : "" %> <%= assignedTextChange %>  assignStyles">
        <%= Model.DateText %></span>
    <% } %>
</td>