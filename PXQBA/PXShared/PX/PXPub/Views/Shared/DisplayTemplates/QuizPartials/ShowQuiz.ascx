<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>

<div id="questionContents"  class="show-quiz">
    <% var envUrl = ConfigurationManager.AppSettings["EnvironmentUrl"];
       var devUrl = ConfigurationManager.AppSettings["LearningCurveUrl"];
       string url = String.Format("{0}?enrollmentId={1}&userId={2}&itemId={3}", devUrl, Model.EnrollmentId, Model.UserId, Model.Id);
       string showDirections = "0";
       string actionFlag = "|action=active";
       string showSpecificQuestion = "";
       if (!Model.Description.IsNullOrEmpty())
       {
           showDirections = "1";
       }

       
       
       if (!Model.QuestionId.IsNullOrEmpty() && !Model.Id.ToLower().Contains("preview"))
       {
           showSpecificQuestion = "|questionid^" + Model.QuestionId;
           actionFlag = "";
       }

       BhComponent component = new BhComponent { Id = "quizplayer" };

        component.ComponentName = "ActivityPlayer";
        component.Parameters = new
                    {
                        EnrollmentId = Model.EnrollmentId,
                        ItemId = Model.Id,
                        ShowHeader = false,
                        Extra = string.Format("autostart^true|group^{0}|showDirections^{1}{2}{3}", (int)ViewData["ordinal"], showDirections, showSpecificQuestion, actionFlag)
                    };
       
      
        
        // "Version" has to be one more than the number of past submissions, so that when the student clicks 
        // "Submit" on the quiz it redirects them to the summary for the quiz they were just taking    
       var submissionLink = Html.ActionLink("View Submission", "SubmissionHistory", "Quiz", new { quizId = Model.Id, Version = Model.Submissions.IsNullOrEmpty() ? 1 : Model.Submissions.Count() + 1 }, new { @class = "fne-link hidden submission-link" });

        // This is a url that points back to the submission history for this particular assessment. It will be hidden and 
        // triggered when taking a quiz and hitting the Save button. See PxCommon.js.
        var saveUrl = Url.Action("DisplayItem", "ContentWidget",
                                    new
                                        {
                                            id = Model.Id,
                                            mode = ContentViewMode.Preview,
                                            includeNavigation = true,
                                            isBeingEdited = false,
                                            renderFne = true
                                        }); 
       
       if (Model.IsLC)
           component = null;
    %>
    <div class="info">
    <%if(Model.SubType.ToLowerInvariant() == "homework" || Model.SubType.ToLowerInvariant() == "quiz") {%>
        <div class="qDescription" style="display: none;"><%=(Model.Description == null) ? "" : Model.Description %></div>
        <input type="hidden" class="quiz-show-review" value="<%=Model.ShowReviewScreen %>" />
        <%=submissionLink %>
    <%} %>        
        
    <a href="<%=saveUrl %>" id="fne-done" class="hidden save-link show-faceplate-home-icon faceplate-fne-home-icon fne-link loadFullFne"></a>

    </div>
    <% if (component != null)
       {
           Html.RenderPartial("BhIFrameComponent", component);
       }
       else
       { %>
        <iframe class="gradebook-component" src="<%= url %>"></iframe>
    <% } %>
    <div class="clear-float"></div>
</div>

  <div class="quizDirectionModal">
        <div class="placeHolderQuizDirection">
        </div>
  </div>
  
<% if (ViewData["fromQBA"].ToString().ToLower().Equals("true"))
   {
            Html.RenderPartial("~/Views/QuestionAdmin/QuestionFlagContainer.ascx",Model); 
   } %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>, function () {
                PxQuiz.Init();
                PxQuiz.ShowFneInit();
            });
            
        });

    } (jQuery));    
</script>
