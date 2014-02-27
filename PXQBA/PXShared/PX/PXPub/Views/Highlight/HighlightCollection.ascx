<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<HighlightModel>>" %>
<div id="highlightList">
<% foreach (var highlight in Model) { %>
<% Html.RenderPartial("Show", highlight);  %>
<% } %>
</div>