<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.BhComponent>" %>
<div class="bh-raw-component" id="<%= Model.Id %>" style="position:relative; width:800px; height:400px;">
    <% var url = String.Format("/brainhoney/rawcomponent/{0}?{1}", Model.ComponentName, Model.GetQueryString()); %>
    <input class="component-url" type="hidden" value="<%= url %>" />
</div>