<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuizQuestionSettings>" %>

<% using (Html.BeginForm("QuizQuestionSettings", "Quiz", FormMethod.Post, new { id = "edit-quiz-settings-form" }))
   { %>
    <%: Html.HiddenFor(x => x.Id) %>
    <%: Html.HiddenFor(x => x.QuizId) %>
    <%: Html.HiddenFor(x => x.EntityId) %>
    <div class="question-settings-form">
        <%: Html.LabelFor(x => x.Points) %>: <%: Html.TextBoxFor(x => x.Points, new { maxlength = 3 })%>
    </div>
<% } %>