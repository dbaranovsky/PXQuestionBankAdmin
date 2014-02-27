<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div class="searchWidget">

    <div class="searchboxwrapper">
        <input type="text" class="searchBox" />
        <button type="button" class="searchBtn">Search</button>
    </div>
    <div class="searchtabwrapper">
        <span class="toctab selectedtab">Table of Contents</span>
        <span class="indextab">Index</span>
        <span class="resultstab disabledtab">Search Results</span>
    </div>
    <div class="searchcontent">
        <div class="indexcontent" style="display:none"> INDEX CONTENT </div>
        <div class="resultscontent" style="display:none"> 
            <div class="resultsFullHeader">Search results</div>
            <div class="searchTextWrapper">
                You searched for <span class="searchText"></span><span class="correctedSearch" style="display:none">, we think you meant <span class="correctedSearchText"></span></span>
            </div>
            <!-- Holds the contents rendered from the template -->
            <div id="searchResultsWrapper" class="searchResultsWrapper"></div>

            <!-- Holds the default text to be displayed if no search results are found  -->
            <div class="noSearchContent" style="display:none">
                <div>We're not sure what you're looking for. Try another word or phrase.</div>
                <div>Browse the <span class="indexlnk">index</span> or <span class="toclnk">table of contents</span></div>
            </div>
         </div>
    </div>

</div>


<!-- HTML Template  -->
<script id="being_meta_template" type="text/x-handlebars-template"> 
    {{#each clusters}}
        <div class="searchresult">
            <div class="searchResultHeader" clusterid="{{ indexid}}" itemcount="{{ items.length }}"> 
                <h4>{{{ headtext }}} <span class="itemcount" style="display:none">({{ items.length }})</span></h4>
                <span class="add"></span>
                <span class="remove"></span>
            </div>
            <ul class="searchResultContent" clusterid="{{ indexid }}" itemcount="{{ items.length }}">
            {{#each items}}
                <li class="resultitem" refids="{{ getRefs refs }}" indexid="{{ indexid }}" refscount="{{ refs.length }}">
                    <span>{{{ text }}}</span>
                    <div class="add"></div>
                    <div class="remove"></div>
                </li>
            {{/each}}
            </ul>
        </div>
    {{/each}}
</script>

<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/SearchWidget/SearchWidget.js") %>'], function () {
                PxSearchWidget.Init({ sel: { TocContent: '#PX_TOCWidget'} });
            });
        });

    } (jQuery))
</script>
