<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Question>>" %>

Questions found:
<br />
<%
    foreach(Question q in Model) {%>
<%=q.Id %> | <%=q.Title %> 
<br />
<%} %>
