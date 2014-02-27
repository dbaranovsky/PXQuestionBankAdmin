<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="System.Collections.ObjectModel" %>

<div id="content-item" class="fne-local content <%= Model.Content.Type.ToLower() %>">
    <input type="hidden" class="item-id" value="<%= Model.Content.Id %>" />
    <input type="hidden" class="content-item-readonly" value="<%= Model.Content.ReadOnly %>" />
    <input type="hidden" class="content-item-title" value="<%= Model.Content.Title %>" />
    <div id="contentwrapper" class="contentwrapper "><br/>This item is restricted.
   </div>
</div>
