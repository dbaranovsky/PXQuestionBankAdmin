<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SearchQuery>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>
<% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
%>

   
<% Html.RenderPartial("ResourceBreadcrumb"); %>


    <ul class="menuResultsModes">
        <li class="resources"><a href="javascript:" class="modeResources">Content and Assignments</a></li>
        <li class="questions active"><a href="javascript:" class="modeQuestions ">Questions</a></li>
    </ul>

<div class="sidepanel-body">
    <div id="search-results" class="px-default-text  <%= isInstructor ? "" : "studentView" %>">
        <div class="available-questions resource-questions">
            <% ViewData["mode"] = QuizBrowserMode.Resources; %>
            <% Html.RenderAction("SearchQuestions", "QuizSearch",
               new {
                   query = Model,
                   mode = QuizBrowserMode.Resources
               });%>
        </div>
    </div>
</div>

<input type="hidden" id="browse-resources-url" value="<%= String.Format("{0}?{1}",  Request.Url.ToString(), Request.Form.ToString()) %>" />
<script type="text/javascript">
    PxPage.OnReady(function () {
        $('.faceplate-browse-resources').FacePlateBrowseMoreResources("showMoreResourcesResults");
    });
</script>
