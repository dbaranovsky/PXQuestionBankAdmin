<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Widget>" %>

<% var Id = !Model.Id.IsNullOrEmpty() ? Model.Id : string.Empty;
   var widgetId = string.Format("{0}_{1}", "container", Id);
   %>
<div id="<%= widgetId %>" class="<%= widgetId %>">
</div>
