<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IDictionary<string,string>>" %>
<div id="browseResultsPanel">
        <div class="close"></div>
    <%Html.RenderPartial("FaceplateBrowseResourcesDropdown", Model); %>
    <div id="browseResults" class="px-default-text browseResults">
        <div id="search-loading">
            <div id="loadingBlock" ></div>
        </div>
        <div id="browseResultsContent"></div>
    </div>
</div>
