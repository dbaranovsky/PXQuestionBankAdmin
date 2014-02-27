<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SearchResultDoc>" %>

<%--<%=Model.dlap_title.Truncate("...", 0, 50)%>--%>
<ins class="icon"></ins><%=Model.dlap_title%> 
<%--(<%=Model.ResultType%>) --%>
<a href="#" class="addLinkToNote" > </a>
<input type="hidden" value="<%=Model.Url%>" class="linkUrl">
<input type="hidden" value="<%=Model.dlap_title%>" class="linkSearchedName">