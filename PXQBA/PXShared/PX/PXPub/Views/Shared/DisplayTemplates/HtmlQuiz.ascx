<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.HtmlQuiz>" %>
<%
    var guid = Guid.NewGuid().ToString();
    var quizid = string.Format("htmlquiz-content-{0}", guid);  
%>
<!-- Add guid to the div id for more exact selection -->
<div id="<%= quizid %>" class="htmlquiz-content <%=Model.AllowComments ? "allowComments" : "" %> <%= Model.BHHtmlQuiz ? "bh-player" : "" %>">
  <!-- These fields are needed for taking unassigned quizzes in XB -->
  <%= Html.HiddenFor(m => m.BHParentId) %>
  <%= Html.HiddenFor(m => m.IsGradable) %>
  <%= Html.HiddenFor(m => m.MaxPoints) %>
  <%= Html.HiddenFor(m => m.OverrideDueDateReq) %>
<% 
    // Ideally we should be showing this for all Quizzes, but for right now we are only going to show for HTML quiz until stuff is in place.
    var quiz = Model != null ? Model as HtmlQuiz : null;
    //Display the submission if we have an html quiz with questions and we arn't past the due date.
    var displaySubmission = quiz.DisplaySubmissionControls;

    //If we are displaying a brain honey html quiz, render using bh activity player
    if (quiz.BHHtmlQuiz)
    {
      BhComponent component = new BhComponent {Id = "quizplayer"};

      component.ComponentName = "ActivityPlayer";
      component.Parameters = new
      {
        EnrollmentId = Model.EnrollmentId,
        ItemId = Model.Id,
        ShowHeader = false,
        Extra = string.Format("autostart^true")
      };

      //I really just added this check for a quick way to avoid rendering this view for testing because
      //it is untestable in its current state.
      if (!string.IsNullOrWhiteSpace(Model.EnrollmentId))
      {
        Html.RenderPartial("BhIFrameComponent", component);
      }
    }
    else //Render using the old agilix component
    {
      var htmlplayerUrl = quiz.XBookAppParams.GetComponentUrl();
%>
    <div id="<%= Model.XBookAppParams.ComponentName + "-" + guid %>" class="htmlquiz" rel="<%= htmlplayerUrl %>" quizid="<%= guid %>">
    </div>
    <%
    }

    // Block for comments
    if (Model.AllowComments)
    { 
      var doc = new DocumentToView()
      {
          ItemId = Model.Id,
          HighlightType = 1,
          HighlightDescription = Model.Title,
          AllowComments = Model.AllowComments,
          DisciplineId = Model.DisciplineId,
          NoteId = Model.NoteId,
          IsProductCourse = Model.IsProductCourse,
          IsExernalContent = true
      };
    
      %>
      <div id="highlight-container">
          <div id="highlight-new-container" class="highlight-new-container" style="display:none;">
              <% Html.RenderAction("NewHighlightForm", "Highlight", doc); %>
          </div>
          <div id="highlightList"> <%--client call will get notes and highlights and add notes in this div--%>
          </div>
      </div>
<%  } %>
    <!-- Button to submit html quiz answers -->
    <div id="submission-controls" style="display:none">
        <label id="submission-status" class="submission-label"></label>
        <div class="submissionbuttonwrapper">
            <button id="submission-save" class="submission-button submission-save button secondary">Save</button>
            <button id="submission-submit" class="submission-button button primary">Submit</button>
        </div>
    </div>
    
    <!-- Submission history for students. Make sure they are a student, and make sure this is configured to show -->
<%  if (quiz.DisplayAttempts)
    { %>
    <div id="submission-history">
        <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/PreviousAttempts.ascx"); %>
    </div>
<%  } %>
</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            var selector = '#<%=quizid %>';
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/Quiz/HtmlQuiz.js") %>'], function () {
                var quiz = $(selector).HtmlQuiz({ displayFooter: <%= displaySubmission.ToString().ToLower() %>, bhPlayer: <%= Model.BHHtmlQuiz.ToString().ToLower() %>});
                $(quiz.el).trigger('htmlquiz-created', [quiz]);
            });
        });
    }(jQuery));
</script>
