<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QuestionEditor>" %>
<div id="QuestionEditorTab" data-qba-questionid="<%= Model.QuestionId %>"
                    data-qba-entityid="<%= Model.EntityId %>" >   
<% if ((Model.QuestionType == "custom" && Model.CustomUrl == "HTS"))
   {
        Html.RenderPartial("HTSIFrameComponent", new HTSComponent()
        {
            PlayerUrl = System.Web.HttpUtility.UrlEncode(Model.HtsPlayerUrl),
            EditorUrl = Url.GetHtsEditorUrl(),
            QuestionId = Model.QuestionId,
            QuizId = Model.QuizId,
            EntityId = Model.EntityId,
            EnrollmentId = Model.EnrollmentId,
            IsAdvancedConvert = Model.IsAdvancedConvert,
            UseAsStandalone = true
        });
   }
   else if ((Model.QuestionType == "custom" && Model.CustomUrl == "FMA_GRAPH"))
   {
       Html.RenderAction("EditCustomQuestion", "Quiz", new { questionType = Model.CustomUrl, questionId = Model.QuestionId, customXML = Model.CustomXML });
       
   %>
   <div class="question-nav">   
           <button id="QBA-Save-Question" class="save QBA-button">Save</button>
           <a href="#" id="QBA-Undo-Changes" class="undo-changes"><span class="icon"></span>Revert to saved</a>
        </div>
   <%    
   }
   else
   {
       Html.RenderPartial("BhIFrameComponent", new BhComponent()
       {
            ComponentName = "QuestionEditor",
            Parameters = new
            {
                Id = "quizeditorcomponent",
                EnrollmentId = Model.EnrollmentId,
                ItemId = Model.QuizId,
                QuestionId = Model.QuestionId,
                AllowEditLinkedQuestions = true,
                ShowAdvanced = Model.ShowAdvanced,
                ShowSave = Model.ShowSave,
                ShowCancel = Model.ShowCancel,
                ShowProperties = Model.ShowProperties,
                ShowFeedback = Model.ShowFeedback
            }
       });
%>
         <div class="question-nav">   
            <button id="QBA-Save-Question" class="save QBA-button">Save</button>
            <a href="#" id="QBA-Undo-Changes" class="undo-changes"><span class="icon"></span>Revert to saved</a>
         </div>
<%
   }
%>
</div>
<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        var question_modified_action = "<%= Url.Action("QuestionModified","QuestionAdmin", new { questionId = Model.QuestionId } ) %>";
        if (PxQuestionAdmin) {
            PxQuestionAdmin.QuestionEditorTab(question_modified_action);
        }

        $(PxPage.switchboard).trigger("htsEditorLoaded");

    } (jQuery));   
</script>

