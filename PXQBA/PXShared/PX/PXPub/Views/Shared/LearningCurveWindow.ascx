<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>

<div id="learningCurveWindow">
    <% if (Model != null && Model.Content != null) {
           ViewData["isLCResourceDialog"] = new Object();
           Html.RenderPartial("DisplayItem", Model);
     } %>
</div> 