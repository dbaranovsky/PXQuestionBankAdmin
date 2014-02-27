<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RelatedTemplate>" %>



<span class="relatedTemplateMessage"> <%= Model.Message%></span>
<%if(Model.DisplayName.IndexOf("Add a") >= 0){  %>
    <input type=button value="<%= Model.DisplayName %>" class="relatedTemplate linkButton" templateid="<%= Model.Id %>" 
<%} %>
<%else { %>
    <input type=button value="<%= String.Concat("Add a ", Model.DisplayName) %>" class="relatedTemplate linkButton" templateid="<%= Model.Id %>" 
<%} %>

templatename="<%= Model.DisplayName %>"  /> 



