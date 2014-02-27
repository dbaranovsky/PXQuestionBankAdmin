<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FacetValue>>" %>
<%--<span class="sectionTitle"><%=Model.Results.numFound%> Results Returned</span>--%>
<% 
    var userAccessLevel = ViewData["AccessLevel"] != null ? ViewData["AccessLevel"].ToString() : Bfw.PX.Biz.ServiceContracts.AccessLevel.None.ToString();
    var isStudentView = userAccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student.ToString();
%>
<div id="search-results" class="px-default-text sidepanel-body">
    <% if (Model == null || Model.Count() == 0)
       {%>
    No resources available.
    <%}
       else
       {

           var facetGroups = Model
               .OrderBy(f => //order by chapter numbers
                            {
                                var chapterNum = 9999;
                                if (f.Value.Contains("Chapter"))
                                {
                                    try
                                    {
                                        var matches = System.Text.RegularExpressions.Regex.Matches(f.Value, @"(\d+)");
                                        var result = matches[0].Groups[0].Value;
                                        Int32.TryParse(result, out chapterNum);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                                return chapterNum;
                            })
               .GroupBy(
                   val =>
                   val.Value.StartsWith("instructor_")
                       ? "Instructor"
                       : val.Value.StartsWith("student_") ? "Student" : "")
               .OrderByDescending(group => group.Key);
              
           bool hasGroups = facetGroups.Count() > 1;
           foreach (var group in facetGroups)
           {%>
    <%if (hasGroups)
      {
          if (isStudentView && group.Key.ToLower().Equals("instructor"))
          {
              continue;
          }%>
    <h2 style="padding-left:20px">
        <%= group.Key %></h2>
    <% } %>
    <ul>
        <% foreach (var value in group) { %>   
        <li class="fptitle"><a href="javascript:" class="lnkResourceType" data-ft-searchstring='<%= value.Value.Trim().Replace("'","&#39;") %>' onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('chaptertype','<%= value.Value.Trim().Replace("'","&#39;") %>');">
        <%= value.Value.Trim().Replace("instructor_","").Replace("student_","") %></a> (<%= value.Count %>)</li>
        <% } %>
    </ul>
    <% } %>
    <% } %>
</div>
