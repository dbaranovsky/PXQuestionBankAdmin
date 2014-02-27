<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssessmentSettings>" %>
<%
    bool enableLearningCurveQuiz = ViewData["enableLearningCurveQuiz"] == null ? true : Convert.ToBoolean(ViewData["enableLearningCurveQuiz"]);
    bool enableHomeworkQuiz = ViewData["enableHomeworkQuiz"] == null ? true : Convert.ToBoolean(ViewData["enableHomeworkQuiz"]);
    
%>
<div class="<%= Model.AssessmentType.ToString().ToLower() %>">
    
    <% using (Html.BeginForm("Save", "AssessmentSettings", FormMethod.Post, new { id = "assessment-settings-form" }))
       { %>
       <%: Html.HiddenFor(x => x.EntityId)%>
       <%: Html.HiddenFor(x => x.AssessmentId)%>
       <%: Html.HiddenFor(x => x.AssessmentType)%>
       <% if ((bool)ViewData["IsTemplate"] == false)
          {
               %>
            <div class="assessment-toggles">
                <% if (enableHomeworkQuiz)
                   {%>
                    <input class="homeworktoggle" type="button" <% if (Model.AssessmentType == AssessmentType.Homework) { %>disabled="disabled"<% } %> value="Homework" style="width: 80px;" />
                 <%} %>
                <input class="quiztoggle" type="button" <% if (Model.AssessmentType == AssessmentType.Quiz) { %>disabled="disabled"<% } %> value="Quiz" style="width: 80px;" />
                <% if (enableLearningCurveQuiz)
                   {%>
                    <input class="lctoggle" type="button" <% if (Model.AssessmentType == AssessmentType.LearningCurve) { %>disabled="disabled"<% } %> value="LearningCurve" style="width: 100px;" />
                 <%} %>
            </div>
            <%
        } %>
       <% else
          { %>
            <div class="assessment-toggles" style="width: 320px;">
                <input class="homeworktoggle" type="button" disabled="disabled" value="Homework" style="width: 80px;" title="This option cannot be changed for a group or an individual"/>
                <input class="quiztoggle" type="button" disabled="disabled" value="Quiz" style="width: 80px;" title = "This option cannot be changed for a group or an individual" />
                <input class="lctoggle" type="button" disabled="disabled" value="LearningCurve" style="width: 100px;" title = "This option cannot be changed for a group or an individual" />
                <div class="lockIndividualStudent" style="float:right;" title="These options cannot be changed for a group or an individual" ></div>
            </div>
       <% } %>
        <div class="assessment-settings-title"><%= (Model.AssessmentType == AssessmentType.LearningCurve) ? "Learning Curve" : Model.AssessmentType.ToString()%> Mode Settings</div><br />

        <% if (Model.AssessmentType == AssessmentType.Homework)
           { %>
            <div class="homework-settings-prompt">
            Number of attempts, time limit, and review settings are set on a "question-by-question" basis in "Homework mode" assessments. Those options are highlighted.
            You should set the default values here then override the default settings on a question-by-question basis, if necessary, by editing question settings in the view tab.
            </div>      
    <% } %> 

    <% if (Model.AssessmentType == AssessmentType.LearningCurve)
       { %>
       <ul style="display:inline-block;">
    <li style="width:100%;display:inline-block;" class="target-score-settings">
    <%                
           var htmlAttribute = Model.AutoTargetScore ? new { disabled = "disabled" }: null ;
           var prevTargetScore = Model.LearningCurveTargetScore;
     %>
     <input type="hidden" class="hdnTargetScore" value="<%=prevTargetScore %>" />
    Target Score: <%: Html.TextBoxFor(x => x.LearningCurveTargetScore, htmlAttribute)%>  <%: Html.CheckBoxFor(y => y.AutoTargetScore)%> (Auto Calculate) 
    
    <span style="width:40%">
    <p>
        Enter the target score you would like to use. Students must reach this target score <br />
        to complete and receive a grade of 100% for the activity. The default target score is  <br />
        150 times the number of topics in the activity, which ensures that every student  <br />
        answers at least 5 questions for each topic (even if he/she gets every question right without hints or incorrect guesses).
        </p>
    </span>

    </li>
   <%-- <li> <%: Html.CheckBoxFor(x => x.AutoCalibrateDifficulty, new { @class = "settingschange" })%> Auto-calibrate Difficulty</li>--%>
</ul>
    <%}
       else
       {
           %>
                <div class="assessment-settings-wrapper attempts">
                    <div class="setting-defaults">
                        <div style="width: 200px; float: left"><%: Html.LabelFor(x => x.NumberOfAttempts)%>:</div>
                        <div><%: Html.EditorFor(x => x.NumberOfAttempts, new { @class = "settingschange" })%></div>
                    </div>
                    <div id="scored-attempt-container" style="display:none;">
                        <div style="width: 200px; float: left"><%: Html.LabelFor(x => x.SubmissionGradeAction)%>:</div>
                        <div><%: Html.EditorFor(x => x.SubmissionGradeAction, new { @class = "settingschange" })%></div>
                    </div>
                    <div id="Div1">
                        <div style="width: 200px; float: left"><%: Html.LabelFor(x => x.GradeRule)%>:</div>
                        <div><%: Html.EditorFor(x => x.GradeRule, new { @class = "settingschange" })%></div>
                    </div>
                </div>
         
            <%  if (Model.AssessmentType != AssessmentType.HtmlQuiz)
                {%>
                <div class="assessment-settings-wrapper setting-defaults">
                    <div>
                        <div class="setting-small-wrapper"><%: Html.LabelFor(x => x.TimeLimit)%>:</div>
                        <div style="height: 40px">
                            <% var hasTimeLimit = Model.TimeLimit > 0; %>
                            <div><input type="radio" name="time-limit" id="no-time-limit-radio" <% if (!hasTimeLimit) { %>checked="checked"<% } %> /><label for="no-time-limit-radio">No time limit</label></div>
                            <div class="time-limit-minutes">
                                <input type="radio" name="time-limit" id="time-limit-radio" <% if (hasTimeLimit) { %>checked="checked"<% } %> /><%: Html.TextBoxFor(x => x.TimeLimit)%>
                                <label for="time-limit-radio"> minutes</label>
                            </div>
                        </div>
                    </div>
                </div>
            <%  } 
             %>
        <% if (Model.AssessmentType == AssessmentType.Quiz)
           { %>
                <div class="assessment-settings-wrapper">
                    <div>
                        <div class="setting-small-wrapper" style="height: 40px;"><%: Html.LabelFor(x => x.QuestionDelivery)%>:
                        <% if (!Model.EntityIdIsCourseId || (bool)ViewData["IsTemplate"] == true)
                           { %>
                              <div class="lockIndividualStudent" style="margin-top: 10px;margin-left:500px;position:absolute;" title="This option cannot be changed for a group or an individual"></div>
                         <%} %>
                        </div>
                        <div style="height: 40px;width:450px;">
                        <% if (Model.EntityIdIsCourseId && (bool)ViewData["IsTemplate"] == false)
                           { %>
                               <%: Html.EditorFor(x => x.QuestionDelivery, new { @class = "settingschange" })%>
                         <%} %>
                         <% else
                           { %>
                               <%: Html.EditorFor(x => x.QuestionDelivery, new { disabled = "disabled" })%>

                         <%} %>
                        </div>
                    </div>
                </div>           
        <%  } %>

        <div class="assessment-settings-wrapper">
            <div>
                <div class="setting-small-wrapper"> Save and continue:</div>
                <div style="height: 40px">
                    <div><%: Html.CheckBoxFor(x => x.AllowSaveAndContinue, new { @class = "settingschange" })%> <%: Html.LabelFor(x => x.AllowSaveAndContinue)%></div>
                        <% if (Model.EntityIdIsCourseId && (bool)ViewData["IsTemplate"] == false)
                           { %>
                                <div>
                                    <%: Html.CheckBoxFor(x => x.AutoSubmitAssessments, new { @class = "settingschange" })%> 
                                    <%: Html.LabelFor(x => x.AutoSubmitAssessments)%>
                                </div>
                         <%} %>
                         <% else
                           { %>
                                <div>
                                    <div  style="float: left; color:Gray;"><%: Html.CheckBoxFor(x => x.AutoSubmitAssessments, new { @disabled = "disabled", @readonly = "readonly" })%>
                                        Auto-submit saved assessments and partially completed assessments at due time
                                    </div>                    
                                    <div class="lockIndividualStudent" style="float:left;" title="This option cannot be changed for a group or an individual"></div>
                                </div>
                         <%} %>

                </div>
            </div>
        </div>
    <%  if (Model.AssessmentType != AssessmentType.HtmlQuiz)
        {%>
        <div class="assessment-settings-wrapper">
            <div>
                <div class="setting-small-wrapper">Scrambling:</div>
                <div style="height: 40px">
                    <% if (Model.AssessmentType == AssessmentType.Quiz)
                       { %>
                        <div><%: Html.CheckBoxFor(x => x.RandomizeQuestionOrder)%> <%: Html.LabelFor(x => x.RandomizeQuestionOrder)%></div>
                    <%} %>
                    <div><%: Html.CheckBoxFor(x => x.RandomizeAnswerOrder)%> <%: Html.LabelFor(x => x.RandomizeAnswerOrder)%></div>
                </div>
            </div>
        </div>
    <%  } 
        
            if (Model.AssessmentType != AssessmentType.HtmlQuiz)
            {%>
                <div class="assessment-settings-wrapper">
                    <div>
                        <div class="setting-small-wrapper">Hints:</div>
                        <div style="height: 40px">
                            <div><%: Html.CheckBoxFor(x => x.AllowViewHints)%> Allow students to view hints for questions that have them</div>
                            <div>Subtract <%: Html.TextBoxFor(x => x.HintSubstractPercentage)%> &#37; of question points when student views a hint</div>
                        </div>
                    </div>
                </div>
        <%  } %>
                <div class="assessment-settings-wrapper setting-defaults">
                    <div class="setting-large-wrapper"></div>
                    <div style="height: 40px" class="setting-default-header">
                        <span class="setting-header" title="After every submission (or the due date passes), students are presented with this component in their review.">Every attempt</span>
                        <% if (Model.AssessmentType == AssessmentType.Homework)
                           { %>
                            <span class="setting-header" title="Students may see this review component after their second incorrect submission, after they get the question completely correct, or after the due date passes. This option is disabled when attempts per question is set to 1.">Second attempt</span>
                            <span class="setting-header" title="Students may see this review component after submitting their last available attempt, after they get the question completely correct, or after the due date passes. This option is disabled when attempts per question is set to 1.">Final attempt</span>
                        <% } %>
                        <span class="setting-header" title="Students may only see this component of a review once the due date has passed.">Due Date has passed</span>
                        <span class="setting-header" title="Students will never see this component of a review.">Never</span>
                    </div>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowQuestionsAnswers), Key = "show-questions-answers" }); %>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowRightWrong), Key = "show-right-wrong" }); %>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowScoreAfter), Key = "Show-score-after" }); %>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowAnswers), Key = "show-answers" }); %>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowFeedbackAndRemarks), Key = "show-feedback-remarks" }); %>
                    <% Html.RenderPartial("ReviewSetting", new ReviewOption { AssessmentSettings = Model, Option = (x => x.AssessmentSettings.ShowSolutions), Key = "show-solutions" }); %>
                </div>
                <div class="assessment-settings-wrapper">
                    <div>
                        <div style="height: 40px">
                            <div><%: Html.CheckBoxFor(x => x.StudentsCanEmailInstructors)%> <%: Html.LabelFor(x => x.StudentsCanEmailInstructors)%></div>
                        </div>
                    </div>  
                </div>
        <%  %>
        <% if ((bool)ViewData["IsTemplate"] == false)
           { %>
            <input type="button" value="Save Changes" id="assessment-settings-save" />       
        <% } %>
        
    <% }
       } %>
</div>