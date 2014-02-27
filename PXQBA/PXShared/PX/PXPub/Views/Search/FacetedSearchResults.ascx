<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FacetedSearchResults>" %>
<%--<span class="sectionTitle"><%=Model.Results.numFound%> Results Returned</span>--%>

<select>
    <% foreach (FacetField field in Model.FacetFields)
       {%>
      <% foreach (var value in field.FieldValues)
         {%>
           <option value="<%= value.Value %>"><%= value.Value %></option>
           <% }%>
       <%}%>
</select>

<ul class="ResultList">
    <%  foreach (SearchResultDoc doc in Model.docs) %>
    <% { %>
    <li class="<%=doc.CssClass %> Result">
        <% Html.RenderPartial("SearchResultLinkAdvanced", doc); %>
    </li>
    <%  } %>
</ul>
<ul>
    <% foreach (FacetField field in Model.FacetFields)
       { %>
       <li>
           <%= field.FieldName %>
           <ul>
           <% foreach (var value in field.FieldValues)
              {%>
                 <li><%= value.Value %> (Count: <%= value.Count %>)</li>   
            <%} %>
            </ul>
       </li>
    <% } %>
</ul>
