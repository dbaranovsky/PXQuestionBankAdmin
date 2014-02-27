<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>


<%--<% Html.RenderPartial("ResourceBreadcrumb"); %>--%>
<div class="available-questions resource-questions">
<% Html.RenderAction("BrowseMainPage", "QuizEdit", new RouteValueDictionary(new {collectionType = Model, mode = QuizBrowserMode.Resources})); %>
</div>