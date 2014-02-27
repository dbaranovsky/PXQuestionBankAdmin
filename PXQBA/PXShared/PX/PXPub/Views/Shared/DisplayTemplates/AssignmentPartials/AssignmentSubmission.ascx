<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Submission>" %>     
<html>
<head></head>

<div class="submission-body" style="background-color:#fff;padding:15px;">
    <%=HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(Model.Body))%>
</div>
</body>
</html>