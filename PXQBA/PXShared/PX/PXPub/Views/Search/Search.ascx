<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%var searchDisplay = (ViewData["AllowSiteSearch"] != null && ViewData["AllowSiteSearch"].Equals(false)) ? "display:none" : ""; %>
<div id="searchPanel" style="<%= searchDisplay%>">
     <%= Html.TextBox("SearchBox", "Search course", new { @class = "SearchBox" })%>
     <input type="button" id="btnSearch" name="behavior" value="Search" />            
        </div>
    
    <div id="searchResultsPanel">    
    <div id="title">SEARCH RESULTS</div> 
    <span title="Advanced Search" class="linkButton px-default-text fne-minimize search-link">ADVANCED SEARCH</span>
  
    <div class="close"></div>
    <div style="clear:both;"></div>
    
    <div id="searchResults" class="px-default-text">
    <div id="search-loading">
                <%--Loading <img alt="loading" src="<%= Url.Action("Index", "Style", new { path="images/ajax-loader.gif" }) %>" />--%>
                <div id="loadingBlock"></div>
                </div>
                <div id="searchResultsContent"></div>
                
    </div>
    
                </div>