<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionSettings>" %>

<% using (Html.BeginForm("QuestionSettings", "Quiz", FormMethod.Post, new { id = "edit-settings-form" }))
   { %>
    <%: Html.HiddenFor(x => x.Id) %>
    <%: Html.HiddenFor(x => x.QuizId) %>
    <%: Html.HiddenFor(x => x.EntityId) %>
    <div class="question-settings-form">
        <%: Html.LabelFor(x => x.Points) %>: <%: Html.TextBoxFor(x => x.Points, new { maxlength = 3 })%>
    </div>
    <div class="question-settings-form">
        <%: Html.LabelFor(x => x.AttemptLimit) %>: <%: Html.EditorFor(x => x.AttemptLimit)%>
    </div>
    <div class="question-settings-form">
        <%: Html.LabelFor(x => x.TimeLimit) %>: <%: Html.EditorFor(x => x.TimeLimit) %>
    </div>
            <div class="assessment-settings-wrapper setting-defaults">
            <div class="setting-large-wrapper"></div>
            <div style="height: 40px">
                <span class="setting-header">Every</span>
                <span class="setting-header">Second</span>
                <span class="setting-header">Final</span>
                <span class="setting-header">Due Date</span>
                <span class="setting-header">Never</span>
            </div>            
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowQuestionsAnswers = Model.ShowQuestionsAnswers, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowQuestionsAnswers) }); %>
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowRightWrong = Model.ShowRightWrong, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowRightWrong) }); %>
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowScoreAfter = Model.ShowScoreAfter, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowScoreAfter) }); %>
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowAnswers = Model.ShowAnswers, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowAnswers) }); %>
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowFeedbackAndRemarks = Model.ShowFeedbackAndRemarks, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowFeedbackAndRemarks) }); %>
            <% Html.RenderPartial("~/Views/AssessmentSettings/ReviewSetting.ascx", new ReviewOption { AssessmentSettings = new AssessmentSettings { ShowSolutions = Model.ShowSolutions, AssessmentType = AssessmentType.Homework }, Option = (x => x.AssessmentSettings.ShowSolutions) }); %>
    </div>
<% } %>