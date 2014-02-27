<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Widget>>" %>
<%
    foreach (var widget in Model)
    {
        Html.RenderAction("Index", "LaunchpadTreeWidget", widget);
    }
     %>

