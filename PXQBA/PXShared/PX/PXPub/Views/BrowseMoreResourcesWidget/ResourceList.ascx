<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IDictionary<string,string>>" %>
<div id="browseResultsPanel">
    <div class="close">
    </div>
    <div id="moreResourcesTitle">
        <div class="more-resources-string-search px-default-text">
            <a class="search-icon" href="#" onclick="PxFacePlate.StringSearch()"></a>
            <input size="25" id="more-resources-search-box" type="text" name="search-string" placeholder="Search Course"/><a class="clear-search" href="#" onclick="PxFacePlate.ClearSearchField()"></a>
        </div>
    </div>
    <div id="loadingBlockResources"></div>
    <div id="browseResults" class="px-default-text browseResults">
        <%Html.RenderAction("ResourceTypeList", "BrowseMoreResourcesWidget", Model); %>
    </div>
</div>
