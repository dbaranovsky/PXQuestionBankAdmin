<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
<%  var quizTypeName = Model.GetQuizTypeName(Model.QuizType); 

    var hasParentLesson = ViewData["hasParentLesson"];
    var toc = Model.GetSubContainer();
%>

<div class="quiz-content">
  <!-- These fields are needed for taking unassigned quizzes in XB -->
    <%= Html.HiddenFor(m => m.BHParentId) %>
    <%= Html.HiddenFor(m => m.IsGradable) %>
    <%= Html.HiddenFor(m => m.MaxPoints) %>
    <%= Html.HiddenFor(m => m.OverrideDueDateReq) %>
<h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
    <div class="menu edit-link">
        
        <% if (!Model.ReadOnly)
           {
               var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                        {
                                            id = Model.Id,
                                            mode = ContentViewMode.Edit,
                                            hasParentLesson = hasParentLesson,
                                            includeNavigation = false,
                                            isBeingEdited = true
                                        });   
                %>
                <a href="<%=editUrl %>" class="linkButton nonmodal-link edit-button">Edit</a>
        <% } %>
    </div>
</h2>

<%
    var noDesc = string.IsNullOrEmpty(Model.Description);

    if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
        Model.Description = noDesc ? "You have not yet provided any directions. Click \"Edit\" and choose \"Basic Info\" to add directions." : Model.Description;
    else
        Model.Description = noDesc ? "" : Model.Description;
%>

<div class='html-container description-content <%= noDesc ? "no-description" : "" %>'>
    <%= Model.Description %>
</div>
    <%
        if (Model.Questions != null && Model.Questions.Count == 0)
        {
    %>
            <span>This quiz has no questions</span>
    <%
            return;
        }
    %>
<h3 class="sub-title">Policies</h3>
<div class="quiz-policies">
    <% var quizStudentStatus = "";
        if (!Model.PolicyDescription.IsNullOrEmpty())
       { %>
        <ul class ="quiz-policy-list" style="margin: 0">
            <% foreach (var descriptionItem in Model.PolicyDescription)
               { %>
                <li><%= descriptionItem %></li>
            <% } %>
        </ul>
    <% }
        if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student || Model.Display == Quiz.DisplayType.Student)
        {
            var count = (Model.Submissions == null) ? -1 : Model.Submissions.Count();
            // If this is a homework, the 'count' for this purpose is the lowest question attempt count.
            if (String.Equals("homework", Model.SubType, StringComparison.CurrentCultureIgnoreCase) && !Model.QuestionAttempts.IsNullOrEmpty())
            {
                foreach (var questionAttemptList in Model.QuestionAttempts.Values)
                {
                    count = Math.Min(questionAttemptList.Count, count);
                }
            }
            DateTime graceDueDate = (ViewData["GraceDueDate"] == null ? Model.DueDate : ViewData["GraceDueDate"] as DateTime?) ?? Model.DueDate;

            //If we are overriding the start, the means we are ignoring attempt limit and duedate
            if (!Model.OverrideDueDateReq)
            {
                if (count >= Model.AttemptLimit && Model.AttemptLimit > 0) //AttemptLimit = 0 means unlimited attempt.
                {
                    quizStudentStatus = "The number of attempts for this assignment has been exhausted. You can no longer submit this assignment.";
                }
                if (Model.DueDate.Year <= 1)
                {
                    quizStudentStatus = "This assignment has not been assigned.";
                }

                if (Model.DueDate.Year > 1 && (Model.DueDate < DateTime.Now.GetCourseDateTime() && graceDueDate < DateTime.Now.GetCourseDateTime()))
                {
                    quizStudentStatus = "The due date for this assignment was " + Model.DueDate.ToShortDateString() + " " + Model.DueDate.ToShortTimeString() + ". You can no longer submit this assignment.";
                }
            }
            if (((Model.DueDate >= DateTime.Now.GetCourseDateTime() || graceDueDate >= DateTime.Now.GetCourseDateTime()) && quizStudentStatus.Length == 0) 
                || Model.OverrideDueDateReq
                || Model.AllowResubmission)
            {
                var linkText = Model.QuizType == QuizType.Assessment ? "Start the Quiz" : "Start the Homework";
                if (Model.QuizType == QuizType.Assessment && Model.IsQuizSubmissionSaved)
                {
                    linkText = "Resume the Quiz";
                }
                var classes = new List<string> { "fne-link", "linkButton", "start-quiz", "require-confirm-custom" };
                if (!Model.AllowSaveAndContinue)
                {
                    classes.Add("auto-submit");
                }
                if (Model.CourseInfo.CourseType == CourseType.FACEPLATE)
                {
                    classes.Add("faceplatefne");
                }
           %>
            <%= Html.ActionLink(linkText, "ShowQuiz", "Quiz", new { enrollmentId = Model.EnrollmentId, id = Model.Id }, new {@id=Model.Id, @class = String.Join(" ", classes), title = Model.Title })%>
            <% }
        }%>
</div>

<% if((Model.Display == Quiz.DisplayType.Instructor || Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor) && !Model.IsProductCourse)
   { %>
        <h3 class="sub-title">Questions</h3>
        <div class="open-question-editor menu edit-link">
        </div>
        <h3 class="sub-title"></h3>
        <div class="quiz-overview">
            <% if (!Model.IsLC)
               { %>
                    <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/Questions.ascx"); %>
            <% }
               else
               { %>
                    <span>This is a LearningCurve quiz.  Please use the preview function to view the quiz in the LearningCurve player.</span>
            <% } %>
        </div>
<% }

   if ((Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student || Model.Display == Quiz.DisplayType.Student) && Model.QuizType == QuizType.Homework)
   { %>
    <div class="quiz-overview">
        <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/HomeworkQuestionList.ascx", Model); %>
    </div>      
<% }
    
   if ((Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student  || Model.Display == Quiz.DisplayType.Student) && !Model.Submissions.IsNullOrEmpty() && Model.QuizType != QuizType.Homework)
   { %>
        <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/PreviousAttempts.ascx"); %>
<% } %>
<span class="px-default-text"><%=quizStudentStatus %></span>
</div>

<div class="loading-quiz-msg" style="display:none;">
    Loading <%= quizTypeName %>...
</div>

<div id="divEditQuiz" class="divPopupWin">
    <%= Html.HiddenFor(m => m.Id) %>
    <%= Html.HiddenFor(m => m.ParentId) %>
    <%= Html.HiddenFor(m => m.Type) %>
    <%= Html.HiddenFor(m => m.Url) %>
    <%= Html.HiddenFor(m => m.IsAssignable) %>
    <%= Html.HiddenFor(m => m.Sequence)%>
    <%= Html.HiddenFor(m => m.QuizType) %>

    <h2 id="H1" class="divPopupTitle">
        POOL SETTINGS
    </h2>
    <div id="div2" class="divPopupContent">
        <% using (Ajax.BeginForm("EditPool", "Quiz", new { parentid = Model.Id }, new AjaxOptions { HttpMethod = "Post", OnSuccess = "PxQuiz.RefreshQuestionList", UpdateTargetId = "" }, null))
           { %>
        <span class="field buttons">
            <input type="hidden" class="bank-count" />
            <label for="QuizTitle">Title</label>
            <input id="EditTitle" name="editTitle" type="text" onclick="$('#spnEditTitleError').hide('slow');"/>
             <span id="spnEditTitleError" class="field-validation-error px-default-text" >Please enter the title</span>
            <br />
            <label for="lblPool">Pull how many questions from the pool?</label>
            <input id="EditPoolCount" name="poolCount" type="text" onclick="$('#spnEditCountError').hide('slow');"/>
            <span id="spnEditCountError" class="field-validation-error px-default-text">Please enter a valid pool count</span>
            <span id="spnEditIntegerError" class="field-validation-error px-default-text">Please enter a numeric value / The number entered is greater than the bank count.</span>
            <br />
            <input type="submit" name="behavior" value="Save" id="btnQuizSubmit" onclick="return PxQuiz.OnEdit();" /> | 
            <a href="#" id="linkQuizCancel"> Cancel </a>           
            <input type="hidden" class="question-id" name="questionId"/>
            <input type="hidden" class="question-type" name="questionType"/>
        </span><span class="clear"></span>
        <% } %>
    </div>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>, function () {
                PxQuiz.Init();
                PxQuiz.FneInit();
                PxQuiz.FneResize();
                PxQuiz.OverviewFneInit();
            });
         
        });

    } (jQuery));    
</script>
