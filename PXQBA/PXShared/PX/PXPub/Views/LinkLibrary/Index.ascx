<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QuickLink>" %>

<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/Assignment/Assignment.js") %>'], function () {
                $(".addLinkItemClass").first().click();
            });
        });

    } (jQuery));    
</script>

<div id="linklibrary">
    <div class="searchLinkBox">
        <div class="addLinkText">
            <span class="titleBox"><b> Add Link </b></span>
        </div>
        <div class="searchLink">
            <input type="text" value="Search our Library" id="txtSearchLink" />
            <input type="button" value="Search Contents" id="btnSearchLink" />
        </div>
    </div>
    <div class="contentBox">
        <div class="addlink-menubar">
            <%= Ajax.ActionLink("Table of Contents", "ExpandSection", "LinkLibrary", new { id = "PX_TOC", linkId = Model.Id },
                        new AjaxOptions {
                            HttpMethod = "Post",
                            UpdateTargetId = "divToc",
                            OnSuccess = "PxLinkLibrary.SetSectionAddLink"
                        })%> <%--| 
            <a href="#" class="loLink">Learning Objectives</a> |
            <a href="#" class="linklibraryLink">Link Library</a>--%>
        </div>
        <div class="contentInner">            
            <div id="divToc" class="toc px-default-text int-link-toc">                
                <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/LinkPartials/TocLinks.ascx", Model); %>
            </div>
            <div id="learningObjectives" style="display:none;">
            
            </div>
            
            <div id="linkLibraryContent" style="display:none;">
            
            </div>
            
            <div id="search-results-panel" style="display:none;">
            <div class="blade"></div>
                <span id="search-total" class="px-default-text"></span>
                <div id="search-results" class="px-default-text"> 
                
                </div>
            </div>    
        </div>
        <div style="float: right; padding:15px;">
            <input type="button" id="btnClose" value="close" />
        </div>
    </div>
</div>