<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    var quizTemplateId = ViewData["templateId"] == null ? "" : ViewData["templateId"].ToString();
     %>

     <input type="hidden" id="quiz-template-id" value="<%=quizTemplateId %>">