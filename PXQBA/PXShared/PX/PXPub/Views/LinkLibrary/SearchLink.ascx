<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdvancedSearchResults>" %>
<ul class="ResultList">
    <%  foreach (SearchResultDoc doc in Model.Results.docs) %>
    <% { %>
    <li class="<%=doc.CssClass %> Result">
        <% Html.RenderPartial("SearchResultLinks", doc); %>
    </li>
    <%  } %>   
    <%= Html.HiddenFor(m => m.Query.Start) %>
    <%= Html.HiddenFor(m => m.Results.numFound) %>
</ul>
