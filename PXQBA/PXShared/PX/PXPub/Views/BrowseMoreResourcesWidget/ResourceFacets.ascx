<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FacetedSearchResults>" %>
<%--<span class="sectionTitle"><%=Model.Results.numFound%> Results Returned</span>--%>

<%  var userAccessLevel = ViewData["AccessLevel"] != null ? ViewData["AccessLevel"].ToString() : Bfw.PX.Biz.ServiceContracts.AccessLevel.None.ToString();
    var isStudentView = userAccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student.ToString();
    Func<string, object> convert = str =>
    {
        try { return int.Parse(str); }
        catch { return str; }
    };
%>
<% Html.RenderPartial("ResourceBreadcrumb"); %>
<div id="search-results" class="px-default-text sidepanel-body">
    <% if (Model.FacetFields == null || Model.FacetFields.Count == 0)
       {%>
    No resources available.
    <%}
       else
       {
           var fieldValues = Model.FacetFields.SelectMany(ff => ff.FieldValues);
           var fieldNames = String.Join(",",Model.FacetFields.Select(ff => ff.FieldName));
           var facetGroups = fieldValues
               .OrderBy(f => Regex.Split(f.Value.Replace(" ", ""), "([0-9]+)").Select(convert), new EnumerableComparer<object>())
               .GroupBy(
                   val =>
                   val.Value.StartsWith("instructor_")
                       ? "Instructor"
                       : val.Value.StartsWith("student_") ? "Student" : "")
               .OrderByDescending(group => group.Key);

           bool hasGroups = facetGroups.Count() > 1;
           var facetGrps = facetGroups.OrderByDescending(g => g.Key).ToList();
          
           foreach (var group in facetGrps)
           {%>
    <%if (hasGroups)
      {
          if (isStudentView && group.Key.ToLower().Equals("instructor"))
          {
              continue;
          }%>
    <h2>
        <%= group.Key %></h2>
    <% } %>
    <ul>
        <% foreach (var value in group)
           { %>
        <li class="fptitle">
            <% var linkText = value.Value.Trim().Replace("instructor_", "").Replace("student_", "");
               if (linkText.IsNullOrEmpty())
               {
                   linkText = "EMPTY CONTENT TYPE - PLEASE FIX";
               }
               var linkSearchString = value.Value.Trim();
               SearchQuery query = new SearchQuery()
                   {
                       ExactPhrase = linkSearchString,
                       MetaIncludeFields = fieldNames.Replace("_dlap_e", ""),//FacePlateBrowseResourcesResults runs a getitems list - we don't want the suffix _dlap_e
                       Start = 0,
                       Rows = 100 //TODO implement fromLearningCurve (fromLearningCurve: isFromLC)
                   }; %>
            <%= Ajax.ActionLink(linkText, "FacePlateBrowseResourcesResults", "BrowseMoreResourcesWidget",
            new {
                query.ExactPhrase,
                query.MetaIncludeFields,
                query.Start,
                query.Rows //TODO implement fromLearningCurve (fromLearningCurve: isFromLC)
            }, new AjaxOptions() { UpdateTargetId = "browseResults", LoadingElementId = "loadingBlockResources", LoadingElementDuration = 500 })%>
            <%--<a href="#" class="lnkResourceType" data-ft-searchstring='<%= value.Value.Trim() %>'>
                <%= value.Value.Trim().Replace("instructor_","").Replace("student_","") %></a>--%>
            <span class="count">(<%= value.Count %>)</span> </li>
        <% } %>
    </ul>
    <% } %>
    <% } %>
</div>
