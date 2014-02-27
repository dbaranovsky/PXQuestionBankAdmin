<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SearchResultDoc>" %>

<%--<%=Model.dlap_title.Truncate("...", 0, 50)%>--%>
<ins class="icon"></ins>
<div class="search-results-title"><%=Model.dlap_title%></div><br />
<%--(<%=Model.ResultType%>) --%>
<div class="advanced-results-links">
    <a href="javascript:void(0);" class="search-open" rel="<%=Model.itemid%>">Open</a>
    <a href="javascript:void(0);" class="search-preview" rel="<%=Model.itemid%>">Preview</a>
</div>