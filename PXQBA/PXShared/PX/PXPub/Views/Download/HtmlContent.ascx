<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HtmlDocument>" %>

<html>
<head></head>
<%
    var decodedBody = Model.Body.IsNullOrEmpty()? "" : HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(Model.Body));  
%>

<body>
<% if (!String.IsNullOrEmpty(decodedBody))
   { %> 
             <%=decodedBody%>
 <% } %>


 </body>

</html>