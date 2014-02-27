<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SearchResultDoc>" %>

<a href="<%=Model.Url%>" title="<%=Model.dlap_title %>" ><ins class="icon"></ins><%=Model.dlap_title.Truncate("...", 0, 50)%> <%--(<%=Model.dlap_itemtype%>)--%></a>