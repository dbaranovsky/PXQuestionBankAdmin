<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Question>" %>
<%
    if (!ViewData.Keys.Contains("isPoolQuestion"))
    {
        ViewData["isPoolQuestion"] = false;
    }
    
    bool isPoolQuestion = (bool)ViewData["isPoolQuestion"];

%>

<div class="question-editor">
    <%  var questionType = "Publisher question (not edited)";
        var isUserCreated = false;
        var isPublisherEdited = false;
        var isFromLearningCurve = Convert.ToBoolean(ViewData["isFromLearningCurve"]);
        if (!Model.SearchableMetaData.IsNullOrEmpty())
        {
            if (Model.SearchableMetaData.Keys.Contains("createdBy"))
            {
                questionType = "User-created question";
                isUserCreated = true;
            }
            else if (Model.SearchableMetaData.Keys.Contains("publisherSupplied"))
            {
                if (Model.SearchableMetaData.Keys.Contains("publisherEdited"))
                {
                    questionType = "Publisher question (edited)";
                    isPublisherEdited = true;
                }
            }
        }
        var isFeedbackProvided = false;
        if (!Model.Choices.IsNullOrEmpty())
        {
            isFeedbackProvided = (from c in Model.Choices where (!c.Feedback.IsNullOrEmpty() && c.Feedback.ToLower().Trim() != "<div></div>") select c).Count() > 0;
        }
        var feedbackText = isFeedbackProvided ? "Edit Feedback" : "Add Feedback";
        
    %>
    <input class="is-user-created" type="hidden" value="<%= isUserCreated %>" />
    <input class="is-publisher-edited" type="hidden" value="<%= isPublisherEdited %>" />
    <input class="former-id" type="hidden" value="<%= Model.FormerId %>" />
    <input class="id" type="hidden" value="<%= Model.Id %>" />
    <input class="quiz-id" type="hidden" value="<%= Model.ItemId %>" />
    <input class="question-xml" type="hidden" value="<%= HttpUtility.HtmlEncode(Model.QuestionXml) %>" />
    <input class="question-custom-url" type="hidden" value="<%= Model.CustomUrl %>" />
    <input class="is-last" type="hidden" value="<%= Model.IsLast %>" />
    <input class="is-hts" type="hidden" value="<%= Model.IsHts %>" />
    <input class="type" type="hidden" value="<%= Model.Type %>" />
    <% var url = String.Format("/brainhoney/rawcomponent/QuestionEditor?enrollmentid={0}&itemid={1}&questionid={2}", Model.EnrollmentId, Model.ItemId, Model.Id); %>
    <input type="hidden" id="question-editor-url" value="<%= url %>" />
    <input type="hidden" class="is-new-question" value="<%= Model.IsNewQuestion %>" />
    <div class="hts-data" style="display: none;">
        Title:
        <input type="text" class="hts-text" value="<%= HttpUtility.HtmlEncode(Model.Text) %>" />
        Points:
        <input type="text" class="hts-points" value="<%= Model.Points %>" />
    </div>
    <div class="content" id="hts-editor-ui">
        <div class="hts-editor-links-container">
            <div>
                <div class="question-type-info">
                    <span class="title">Question Editor</span> <span class="question-type">
                        <% var displayType = (questionType == "CUSTOM") ? Model.CustomUrl : questionType; %>
                        <%= displayType%>
                    </span>
                </div>
            </div>
            <div class="links hts-editor-links" style="clear: both;">
                <%= Model.CustomUrl == "FMA_GRAPH"? "Graphing" : Question.QuestionType(Model.Type) %>
                <%if (isFromLearningCurve)
                  { %>
                | <span class="difficultyLevel">Level
                    <%= Question.LevelFormatted(Model.LearningCurveQuestionSettings[0].DifficultyLevel) %></span>
                <%} %>
                | <span class="total-points">
                 <% if (Model.Type.ToLowerInvariant() != "bank")
                    { %>
                        <%if (!isPoolQuestion)
                        { %>
                            <a class="point-label" href="javascript:">
                                <span class="point-label">
                                    <%= String.Format("{0} {1}", Model.Points, Model.Points > 1 ? " points" : " point")%>
                                </span>
                            </a>
                        <%}
                        else
                        { %> 
                            <span class="point-label">-- pt (s)</span> <% 
                        } %>
                    <%} %>
                        
                <span class="default-point-label" style="display: none">--</span> <span class="point-textbox"
                    style="display: none">
                    <input type="text" class="questions-points" value="<%= Model.Points %>" />
                    <input type="hidden" class="questions-points-original" value="<%= Model.Points %>" />
                </span>
                    <%--<input id = "Points" type = "text" value= "<%= Model.Points %>" size="2" maxlength ="3" style="height:12px;"/>--%>
                </span>|
                <% if (Model.QuizType == QuizType.Homework)
                   { %>
                <span class="attempts">
                    <%= Model.Attempts%></span> attempts |
                <% }
                   else if (Model.QuizType == QuizType.Assessment && isFromLearningCurve)
                   {%>
                <a class="learning-curve-question-settings" href="javascript:">Question Settings</a> |
                <% }
                   if (!Model.Text.IsNullOrEmpty() || Model.Type == "CUSTOM")
                   { %>
                <a class="portal-question-preview" href="javascript:">Preview</a> <a class="lc-question-preview"
                    href="javascript:">Preview</a>
                <% } %>
                <% if (Model.CustomUrl != "HTS")
                   { %>
                | <a href="javascript:">Advanced</a>
                <% } %>
            </div>
        </div>
        <div id="question-editor">
            <% if ((Model.Type == "CUSTOM" && Model.CustomUrl == "HTS"))
               { %>
            <%  Html.RenderPartial("HTSIFrameComponent", new HTSComponent()
                {
                    PlayerUrl = System.Web.HttpUtility.UrlEncode(Model.HtsPlayerUrl),
                    EditorUrl = Url.GetHtsEditorUrl(),
                    QuestionId = Model.Id,
                    QuizId = Model.ItemId,
                    EntityId = Model.EntityId,
                    EnrollmentId = Model.EnrollmentId,
                    IsAdvancedConvert = Model.IsAdvancedConvert
                }); 
            %>
            <% }

               else if ((Model.Type == "CUSTOM"))
               {
            %>
            <%  Html.RenderAction("EditCustomQuestion", "Quiz", new {questionType = Model.CustomUrl, questionId = Model.Id, customXML = Model.InteractionData,title=Model.Title }); 
            %>
            <%
               }
               else
               { %>
            <% Html.RenderPartial("BhIFrameComponent", new BhComponent()
            {
                ComponentName = "QuestionEditor",
                Parameters = new
                {
                    Id = "quizeditorcomponent",
                    EnrollmentId = Model.EnrollmentId,
                    ItemId = Model.ItemId,
                    QuestionId = Model.Id,
                    ShowAdvanced = true,
                    ShowSave = false,
                    ShowCancel = false,
                    ShowProperties = !isFromLearningCurve,
                    ShowFeedback = false,
                    AllowEditLinkedQuestions = true,
                    ShowEditFeedback = isFromLearningCurve
                }
            }); 
            %>
         
            <% } %>
        </div>
        <script type="text/javascript" >
            (function ($) {
                PxPage.OnReady(function () {
                    //PxQuiz.SetHtsRpcHooks();

                    if (PxPage.FrameAPI) {
                        PxPage.FrameAPI.setShowBeforeUnloadPrompts(false);
                    }

                    // Wire up click event handler for the Question Settings link
                    $("#hts-editor-ui").find("a:contains('Question Settings')").click(function (event) {
                        // Load the question settings modal
                        if($(event.target).hasClass('portal-question-settings')){                            
                            $('.question-list').questionlist('editQuestionSettings', '<%= Model.ItemId %>', '<%= Model.Id %>', '<%= Model.EnrollmentId %>');
                        }
                        else if ($(event.target).hasClass('learning-curve-question-settings')){                            
                            var args = {
                                quizId: "<%= Model.ItemId %>",
                                questionId: "<%= Model.Id %>",
                                enrollmentId : "<%= Model.EnrollmentId %>"
                            };
                            $(this).LearningCurveMoreResources("editQuestionSettings",args);                                                        
                        }
                    });

                    // Wire up click event handler for the Advanced link
                    $("#hts-editor-ui").find("a:contains('Advanced')").click(function (event) {

                    <% if (Model.Type.ToLowerInvariant() == "mc" || Model.Type.ToLowerInvariant() == "txt") { %>                        
                        PxQuiz.ConvertToHtsQuestion(event);
                        <% } else { %>
                        PxPage.Toasts.Error('The system cannot convert this type of question into an advanced question. \nIf you want to start a new Advanced Question select Advanced Question from the +New menu to the left.');
                        <% } %>
                    });

                });
            } (jQuery));   
        </script>
        <div class="htc">
        </div>
    </div>
    <div id="related-content-editor-question" style="display: none">
        <!-- This will have the editor and tool tip at runtime-->
        <% Html.RenderAction("GetRelatedContent", "Quiz", new { itemId = Model.Id.ToString() }); %>
    </div>
    <div class="question-nav">
        <div class="question-rightnav">
        <% if (!(Model.Type == "CUSTOM" && Model.CustomUrl == "HTS"))
           {%>
             <a href="javascript:" class="edit-related-content">
                Add/Edit Related Content</a> <a href="javascript:" class="edit-question-feedback">
                    <%=feedbackText%></a>
            <input type="hidden" class="prev-question-feedback-text" value='<%=feedbackText%>' />
        <%} %>
           
        </div>
        <div class="question-rightnav-alternate" style="display: none">
            <a href="javascript:" class="back-question"><span class="icon"></span>Back to Question</a>
        </div>
        <div class="question-leftnav">
            <% if (!(Model.Type == "CUSTOM" && Model.CustomUrl == "HTS"))
               {%>
            <input type="button" class="save primary large button savebtn" value="Save" />
             <a href="javascript:" class="undo-changes">Revert to saved</a> 
            <%} %>
        </div>
    </div>
    <div id="divShowResourceContent">
    </div>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>, function () {
                PxQuiz.Init();
                $('.question-editor').questioneditor('questionFneInit');
                $('.question-editor').questioneditor('questionFneResize');
                
            });
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/LearningCurve/LearningCurveMoreResources.js") %>'], function () {
                $(".question-nav").LearningCurveMoreResources();
                //PxLearningCurve.Init();
            });
        });

    } (jQuery));    
</script>
