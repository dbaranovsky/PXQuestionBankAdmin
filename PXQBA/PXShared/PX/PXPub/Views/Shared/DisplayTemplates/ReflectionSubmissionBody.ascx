<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Submission>" %>

<% if (!String.IsNullOrEmpty(Model.Body))
   { %>
        <div class="px-default-text submission-body">
            <h3 class="sub-title">Student Reflection (Submitted on <%=Model.DateSubmitted.ToShortDateString() %>):</h3>
                <%=HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(Model.Body))%>
            <hr/>
        </div>
 <% } %>