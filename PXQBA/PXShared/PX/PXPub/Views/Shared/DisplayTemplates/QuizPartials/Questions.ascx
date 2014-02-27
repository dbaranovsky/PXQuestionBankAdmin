<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>

<% bool hasQuestions = Model.Questions.IsNullOrEmpty(); %>

<div class="question-display quiz" id="<%= Model.Id %>">
    <div class="overview-no-questions">
        <span class="fne-no-questions">This quiz has no questions.</span>

        <% if (!hasQuestions) { %>
        <div class="overview-no-questions html-container description-content no-description">
            You have not yet added any questions to this assessment. Click "Edit" and choose "Questions" to browse the question banks for questions or to create your own.
        </div>
        <% } %>
    </div>    
   
    <% 
        if (!Model.ShowContentView)
        {
    %>
            <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/QuestionList.ascx", Model); %>
    <%
        }
        else
        {            
    %>
    <% 
            if (!hasQuestions)
            {
    %>
            <div class="various-actions">
                <a class="collapseAll" href="javascript:">Collapse All</a>
                <a class="expandAll" href="javascript:">Expand All</a>
                <div class="select-menu-current"><div class="gearbox"></div></div>
                <a class="linkButton remove"  href="javascript:">Remove</a>
    <% 
                if (Model.SubType.Equals("homework", StringComparison.CurrentCultureIgnoreCase) || Model.SubType.Equals("quiz", StringComparison.CurrentCultureIgnoreCase))
                { 
    %>
                    <a class="linkButton edit-settings" href="javascript:">Edit question settings</a>
    <% 
                }
            } 
    %>
    <% var linkUrl = Url.GetComponentHash("item", Model.Id, new { mode = ContentViewMode.Questions, renderFne = true });

       //Note: we're using the (undocumented) Print.aspx page here instead of the AssessmentPrint component because
       //AssessmentPrint has a bug that only prints the first page of the quiz (as of 10/3/13) in chrome
       var printUrl = String.Format("/BrainHoney/Content/Exam/Print.aspx?enrollmentid={0}&itemid={1}&type=3", Model.EnrollmentId, Model.Id);
           %>

                <a class="linkButton" href="<%= linkUrl %>">Edit questions</a>
                <a class="linkButton printQuiz" data-href="<%= printUrl %>">Print</a>
            </div><br />
        <div class="overview-no-questions html-container description-content no-description">
            Click "Edit questions" to browse the question banks or to create your own questions.
        </div>
    <% 
            if (!hasQuestions)
            {
    %>
    <%                
                
                Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/DisplayQuestionList.ascx", Model); 
            } 
        } 
    %>
</div>