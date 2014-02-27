<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CustomWidget>" %>
<div class="customWidgetOuterDiv">
    <div><%= Model.WidgetContents%></div>
    <input type="hidden" class="customValues" value="<%= Model.Title %>" />
</div>

