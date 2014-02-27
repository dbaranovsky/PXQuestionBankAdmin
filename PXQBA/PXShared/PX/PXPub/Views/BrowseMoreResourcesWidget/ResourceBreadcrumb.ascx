<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>

    <% MvcHtmlString linkUrl = Ajax.ActionLink("Resources", "ResourceTypeList", "BrowseMoreResourcesWidget",
                                               new AjaxOptions { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 });
       var title = ViewData.GetValue("Title", MvcHtmlString.Empty).ToString();

       if (!title.IsNullOrEmpty())
       { %>
<div class="breadcrumb">
    <span id="resource-list">
        <%= !title.IsNullOrEmpty() ? linkUrl : MvcHtmlString.Empty %>
    </span><span>&raquo; </span>
    <%
             string updateContainer = "browseResults";
             object mode = ViewData.GetValue("mode", null);
             if (mode != null && QuizBrowserMode.Resources == (QuizBrowserMode)mode)
             {
                 updateContainer = "browseQuestionList";
             }

             var breadcrumbData = (List<BreadcrumbData>)ViewData["BreadcrumbData"];
             if (breadcrumbData != null)
             {
                 foreach (BreadcrumbData o in breadcrumbData)
                 {
                     MvcHtmlString link = Ajax.ActionLink(o.Title, o.Action, o.Controller, o.RouteValues, new AjaxOptions { UpdateTargetId = updateContainer, LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 });
    %>
    <span>
        <%= link %></span><span> &raquo; </span>
    <%
            }
        }

        string breadcrumbParent = ViewData["BreadcrumbParent"].IfNull("").ToString();
        string breadcrumbAction = ViewData["BreadcrumbAction"].IfNull("").ToString();
        string breadcrumbController = ViewData.GetValue("BreadcrumbController", "BrowseMoreResourcesWidget").ToString();
        var breadcrumbRoute = (RouteValueDictionary)ViewData["BreadcrumbRoute"];


        if (breadcrumbAction != "" && breadcrumbParent != "")
        { %>
    <%= Ajax.ActionLink(breadcrumbParent, breadcrumbAction, breadcrumbController, breadcrumbRoute, new AjaxOptions {UpdateTargetId = updateContainer, LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500}) %>
    <% } %>
</div>
<div class="modal_dialog_title" id="moreResourcesTitleName">
    <%= title %>
</div>
<% } %>