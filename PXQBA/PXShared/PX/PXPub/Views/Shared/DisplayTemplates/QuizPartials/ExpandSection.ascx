<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocItem>>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>
<%
    var mode = (QuizBrowserMode) ViewData["mode"];
    if (mode == QuizBrowserMode.Resources)
    {
        Html.RenderPartial("~/Views/BrowseMoreResourcesWidget/ResourceBreadcrumb.ascx", ViewData);
    }
%>
<h2 class="folder-title"></h2>

<% if (!Model.IsNullOrEmpty())
   {
%>
    <ul class="quiz-item-links">
        <% foreach (TocItem section in Model)
           {
               string title = string.IsNullOrEmpty(HttpUtility.HtmlDecode(section.Title)) ? "No Title" : HttpUtility.HtmlDecode(section.Title);
               string childContainerId = string.Format("quiz_toc_children_of_{0}", section.Id);
               bool isQuiz = QuizEditController.QuizTypeNames.Contains(section.ItemType.ToLower());
        %>
            <li id="<%= section.Id %>" class="level <%= isQuiz ? "quiz-item" : "" %>">
                <div class="click-target">
                    <% if (section.IsActive || isQuiz)
                       {
                           if (mode == QuizBrowserMode.Resources)
                           {
                               MvcHtmlString link = Ajax.ActionLink(HttpUtility.HtmlDecode(title), "ExpandSection", "QuizEdit", new RouteValueDictionary(new
                                   {
                                       id = section.Id,
                                       category = "",
                                       startIndex = "0",
                                       lastIndex = "20", mode
                                   }),
                                                                    new AjaxOptions { UpdateTargetId = "browseQuestionList", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 });
                    %>
                            <%= link %>
                        <% }
                           else
                           { %>

                            <a href="#" class="child-item" id="<%= section.Id %>"><%= HttpUtility.HtmlDecode(title) %></a>
                        <% } %>
                    <% }
                       else
                       { %>
                        <%= title %>
                    <% } %>
                    
                    <span id="<%= childContainerId %>" class="children" style="display: none;">
                    </span>
                </div>
            </li>
        <% } %>
    </ul>
<% } %>