<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ExternalDomainMappingConfig>" %>


<%
    if (null != Model)
    {
%>

<div>Enable: <%=Model.Enable %></div>

<div>Rules:</div>
<ul><%foreach (var mapping in Model.Mappings)
  {
      %>
    <li>Map From: <%=mapping.MapFrom %>, to: <%=mapping.MapTo %> </li>
    <%
  } %>
</ul>

<%
    }
%>