<%@ Control Language="C#" %>

<%
    string questionId = ViewData["questionId"] == null ? "" : ViewData["questionId"].ToString();
     %>

<div class="used-elsewhere-quizzes">
    <input type="hidden" id="used-elsewhere-question-id" value="<%=questionId %>" />
    <ul>
        <li><a href="javascript:" class="add-to-new-assessment">Use in new assessment</a></li>
        <li><a href="javascript:" class="add-to-existing-quiz">Use in existing assessment</a></li>
    </ul>
</div>